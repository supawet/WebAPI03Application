using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Globalization;
using WebAPI03Application.Models;
using System.Configuration;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net;


namespace WebAPI03Application
{
    public class WealthPCustomizePersistance2
    {
        public static double GetStandardDev(List<double> list)
        {
            double stdev = 0;
            if(list.Count != 0)
            {
                double average = list.Average();
                stdev = Math.Sqrt((list.Sum(x => Math.Pow(x - average,2))) / (list.Count() ) );
            }

            return stdev;
        }

        //public ArrayList getWealthPCustomize(CWeight[] cweight)
        public WealthPCustomize getWealthPCustomize(CWeight[] cweight)
        {
            //var jss = new JavaScriptSerializer();
            //var results = jss.Deserialize<CWeight>(cweight);
            //Dictionary<string, double> results = jss.Deserialize<Dictionary<string, double>>(cweight);


            //double _lenght = results .Count;

            /*
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            */

            ArrayList WealthPCustomizeArray = new ArrayList();

            OleDbConnection conn = null;
            OleDbCommand command = null;
            OleDbDataReader mySQLReader = null;


            try
            {
                WealthPCustomize wpc = new WealthPCustomize();

                List<string> check_port = new List<string>(new string[] { "BFIXED", "BKA", "BKA2", "REIT", "B-GLOBAL", "BGOLD", "B-TREASURY"
                , "B-TNTV", "BKD", "BCAP", "BBASIC", "B-INFRA", "BTK", "BTP", "B-ASEAN"
                , "B-HY (H75) AI", "B-HY (UH) AI", "B-NIPPON", "B-BHARATA", "B-ASIA", "BCARE", "B-INNOTECH" });

                if (cweight.Count() == 0)
                {
                    wpc.RET = null;
                    wpc.SD = null;
                    wpc.STATUS = "Error : Value can not be null.";
                    //wpc.XX = cweight.Count().ToString() + "_00_" + cweight.Length.ToString();

                    WealthPCustomizeArray.Add(wpc);
                                                                                            
                    return wpc;
                }

                double check_weight = 0;
                foreach (var item in cweight.OrderBy(t => t.PortCode))
                {
                    if (!check_port.Any(c => c.Equals(item.PortCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        wpc.RET = null;
                        wpc.SD = null;
                        wpc.STATUS = "Error : Can not find port '" + item.PortCode + "'.";
                        //wpc.XX = cweight.Count().ToString() + "_00_" + cweight.Length.ToString();

                        WealthPCustomizeArray.Add(wpc);

                        return wpc;
                    }
                    else
                    {
                        check_weight += item.Weight;
                    }
                }

                if (check_weight != 100)
                {
                    wpc.RET = null;
                    wpc.SD = null;
                    wpc.STATUS = "Error : Weight not equal 100.";
                    //wpc.XX = cweight.Count().ToString() + "_00_" + cweight.Length.ToString();

                    WealthPCustomizeArray.Add(wpc);

                    return wpc;
                }

                /*
                foreach (var item in cweight.OrderBy(t => t.PortCode))
                {
                    if (item.Weight.Equals(null))
                    {
                        WealthPCustomize wpc2 = new WealthPCustomize();
                        wpc2.RET = 0.0;
                        wpc2.SD = 0.0;
                        wpc2.XX = cweight.Count().ToString();

                        WealthPCustomizeArray.Add(wpc2);

                        return WealthPCustomizeArray;
                    }
                }
                */

                int dim = cweight.Count();
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

                double money = 1000000;
                double tmp_money = 0;
                double tmp_sd = 1;

                Matrix<double> A = DenseMatrix.OfArray(new double[1, dim]);
                //Matrix<double> tmp = DenseMatrix.OfArray(new double[1, dim]);

                double monthly_multi_asset = 0;
                double multi_asset = 0;
                double multi_asset_start = 0;
                double multi_asset_end = 0;
                double tmp = 0;
                double div = 0;
                double tt = 0;
                Matrix<double> tmp_weight = DenseMatrix.OfArray(new double[1, dim]);
                Matrix<double> tmp_index = DenseMatrix.OfArray(new double[1, dim]);
                Matrix<double> tmp_index_old = DenseMatrix.OfArray(new double[1, dim]);
                Matrix<double> tmp_daily = DenseMatrix.OfArray(new double[1, dim]);

                DateTime iDt = new DateTime();
                DateTime sDt = new DateTime();
                DateTime eDt = new DateTime();

                double assetMonth = 0; //  return ของเดือน
                //  วันทำการสิ้นเดือน  เพื่อหา monthly return
                List<DateTime> eMonth = new List<DateTime>();
                /*eMonth.Add(new DateTime(2015,12,31));
                eMonth.Add(new DateTime(2016, 01, 31));
                eMonth.Add(new DateTime(2016, 02, 29));
                eMonth.Add(new DateTime(2016, 03, 31));
                eMonth.Add(new DateTime(2016, 04, 30));*/
                eMonth.Add(new DateTime(2016, 05, 31));
                eMonth.Add(new DateTime(2016, 06, 30));
                eMonth.Add(new DateTime(2016, 07, 31));
                eMonth.Add(new DateTime(2016, 08, 31));
                eMonth.Add(new DateTime(2016, 09, 30));
                eMonth.Add(new DateTime(2016, 10, 31));
                eMonth.Add(new DateTime(2016, 11, 30));
                eMonth.Add(new DateTime(2016, 12, 31));
                eMonth.Add(new DateTime(2017, 01, 31));
                eMonth.Add(new DateTime(2017, 02, 28));
                eMonth.Add(new DateTime(2017, 03, 31));
                eMonth.Add(new DateTime(2017, 04, 30));
                eMonth.Add(new DateTime(2017, 05, 31));
                eMonth.Add(new DateTime(2017, 06, 30));
                eMonth.Add(new DateTime(2017, 07, 31));
                eMonth.Add(new DateTime(2017, 08, 31));
                eMonth.Add(new DateTime(2017, 09, 30));
                eMonth.Add(new DateTime(2017, 10, 31));
                eMonth.Add(new DateTime(2017, 11, 30));
                eMonth.Add(new DateTime(2017, 12, 31));
                eMonth.Add(new DateTime(2018, 01, 31));
                eMonth.Add(new DateTime(2018, 02, 28));
                eMonth.Add(new DateTime(2018, 03, 31));
                eMonth.Add(new DateTime(2018, 04, 30));
                eMonth.Add(new DateTime(2018, 05, 31));
                eMonth.Add(new DateTime(2018, 06, 30));
                eMonth.Add(new DateTime(2018, 07, 31));
                eMonth.Add(new DateTime(2018, 08, 31));
                eMonth.Add(new DateTime(2018, 09, 30));
                eMonth.Add(new DateTime(2018, 10, 31));
                eMonth.Add(new DateTime(2018, 11, 30));
                eMonth.Add(new DateTime(2018, 12, 28));

                double sd = 0;
                double ret = 0;
                List<double> retList = new List<double>();

                List<string> _port = new List<string>();
                int i = 0;
                int j = 0;
                double yr;
                double pow;
                
                foreach (var item in cweight.OrderBy(t => t.PortCode))
                {
                    _port.Add(item.PortCode);
                    tmp_weight[0, i] = item.Weight/100;
                    i++;
                }
                
                //wpc.STATUS = tmp[0, 0].ToString() + "__" + _port[0];
                //return wpc;

                /*
                Matrix<double> B = DenseMatrix.OfArray(new double[,] {
                     { 20.0, 20.0, 20.0, 20.0, 20.0 }
                });
                */

                /*
                conn.ConnectionString = myConnectionString;
                conn.Open();
                */

                string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString; ;
                conn = new OleDbConnection(myConnectionString);

                conn.Open();

                command = new OleDbCommand();
                command.Connection = conn;
                command.CommandTimeout = 0;

                //-------------Return
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_WealthPFindReBalanceDate";
                command.Parameters.Clear();
                mySQLReader = command.ExecuteReader();

                ArrayList rbDateArray = new ArrayList();

                while (mySQLReader.Read())
                {
                    rbDateArray.Add(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("RBDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US")));
                }
                mySQLReader.Close();
                //-------------Return

                iDt = new DateTime(2015, 12, 01);
                sDt = new DateTime(2016, 05, 31);
                eDt = new DateTime(2018, 12, 28);

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_WealthPCustomizeData";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@port", string.Join(",", _port));
                mySQLReader = command.ExecuteReader();

                j = 0;
                string test = "";
                while (mySQLReader.Read())
                {
                    if (mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).Equals(iDt))   //  if init
                    {
                        multi_asset = 100;
                        i = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            //_port.Add(item.PortCode);
                            tmp_weight[0, i] = item.Weight/100;
                            tmp_index[0, i] = 100;
                            tmp_index_old[0, i] = 100;
                            i++;
                        }
                    }
                    else
                    {
                        i = 0;
                        tmp = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            tmp_index[0, i] = ((mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode))/100) + 1) * tmp_index_old[0, i];
                            tmp += (mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode))* tmp_weight[0, i] / 100);

                            i++;
                            //A[0, i] = (mySQLReader.GetValue(mySQLReader.GetOrdinal(item.PortCode)).Equals(DBNull.Value) ? 0 : mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode)) + 1) * tmp[0, i];
                        }
                        multi_asset = multi_asset * (1 + tmp);
                        i = 0;
                        div = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            div += tmp_weight[0, i] * tmp_index[0, i] / tmp_index_old[0, i];
                            //tmp_weight[0, i] =
                            i++;
                        }

                        i = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            tmp_weight[0, i] = (tmp_weight[0, i] * tmp_index[0, i] / tmp_index_old[0, i] ) / div;

                            tmp_index_old[0, i] = tmp_index[0, i];
                            i++;
                        }

                        if (rbDateArray.Contains(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"))))
                        {
                            i = 0;
                            foreach (var item in cweight.OrderBy(t => t.PortCode))
                            {
                                tmp_weight[0, i] = item.Weight / 100;
                                i++;
                            }
                        }
                        //--------------------------------- SD
                        if (eMonth.Contains(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE"))))
                        {
                            if (!mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).Equals(new DateTime(2016, 05, 31))) {
                                retList.Add((multi_asset / monthly_multi_asset) - 1);

                                //wpc.STATUS += ((multi_asset / monthly_multi_asset) - 1).ToString() + "__";
                            }
                            monthly_multi_asset = multi_asset;
                        }
                        //--------------------------------- /SD

                            /*
                            if (mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).Equals(new DateTime(2016, 06, 03)))
                            {
                                //wpc.STATUS += multi_asset + "__" + tmp_weight[0, 1].ToString();
                                wpc.STATUS = tmp_index[0, 0].ToString() + "__" + tmp_index[0, 1].ToString() + "__" + tmp_index[0, 2].ToString() + "__" + tmp_index[0, 3].ToString() + "__" + multi_asset.ToString() + "__" + tmp_weight[0, 0] + "__" + tmp_weight[0, 1] + "__" + tmp_weight[0, 2] + "__" + tmp_weight[0, 3];
                            }
                            */
                    }   //  end if init

                    if (mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).Equals(sDt))
                    {
                        multi_asset_start = multi_asset;
                    }
                    if (mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).Equals(eDt))
                    {
                        multi_asset_end = multi_asset;
                    }
                    //wpc.STATUS += multi_asset + "__";
                }   // end while
                mySQLReader.Close();

                //wpc.STATUS = tt.ToString();
                //wpc.RET = (multi_asset_end / multi_asset_start);

                yr = eDt.Subtract(sDt).TotalDays;// / 365;
                pow = 365.25 / yr;

                //wpc.STATUS = yr.ToString();   //941

                wpc.RET = (double)(Math.Pow((multi_asset_end / multi_asset_start), pow) - 1);

                var statistics = new DescriptiveStatistics(retList);
                wpc.SD = statistics.StandardDeviation* Math.Sqrt(12);
                wpc.STATUS = "Success";

                WealthPCustomizeArray.Add(wpc);

                return wpc;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (mySQLReader != null)
                {
                    mySQLReader.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
 