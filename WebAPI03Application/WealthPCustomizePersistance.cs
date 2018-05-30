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
    public class WealthPCustomizePersistance
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

                List<string> check_port = new List<string>(new string[] { "BFIXED", "BKA", "REIT", "B-GLOBAL", "BGOLD" });

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
                Matrix<double> tmp = DenseMatrix.OfArray(new double[1, dim]);

                DateTime sDt = new DateTime();
                DateTime eDt = new DateTime();

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
                    tmp[0, i] = money * item.Weight / 100;
                    i++;
                }
                
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

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_WealthPCustomizeData";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@port", string.Join(",", _port));
                mySQLReader = command.ExecuteReader();

                /*
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                //string sqlString = "select * from bblamapidb.azure_dividend where id=1";// tran_date = '" + dt + "'";
                string sqlString = "SP_WealthPCustomize";
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dt", dt);
                */

                //double? double_null;
                //string<T>? string_null = null;

                //mySQLReader = cmd.ExecuteReader();

                /*
                    wpc.FUND_CODE = mySQLReader.GetValue(mySQLReader.GetOrdinal("PORT_CODE")).Equals(DBNull.Value) ? null : mySQLReader.GetString(mySQLReader.GetOrdinal("PORT_CODE"));
                    wpc.UPDATED_DATE = mySQLReader.GetValue(mySQLReader.GetOrdinal("VDATE")).Equals(DBNull.Value) ? null : mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"));
                */
                j = 0;
                while (mySQLReader.Read())
                {
                    if (j == 0)
                    {
                        sDt = mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE"));
                        j++;
                    }
                    else
                    {
                        eDt = mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE"));
                    }

                    //_port2 = mySQLReader.GetValue(mySQLReader.GetOrdinal("Cov1")).Equals(DBNull.Value) ? null : mySQLReader.GetString(mySQLReader.GetOrdinal("Cov1"));
                    //A[i, j] = mySQLReader.GetValue(mySQLReader.GetOrdinal("Covv")).Equals(DBNull.Value) ? 0 : (double)mySQLReader.GetDecimal(mySQLReader.GetOrdinal("Covv"));
                    //A[i,j] = mySQLReader.GetValue(mySQLReader.GetOrdinal("Covv")).Equals(DBNull.Value) ? 0 : mySQLReader.GetDouble(mySQLReader.GetOrdinal("Covv"));
                    if (rbDateArray.Contains(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"))))
                    {
                        money = 0;
                        i = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            money += tmp[0, i];
                            i++;
                        }
                        //reBalance
                        i = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            tmp[0, i] = money * item.Weight / 100;
                            i++;
                        }
                        /*
                        i = 0;
                        foreach (var item in cweight.OrderBy(t => t.PortCode))
                        {
                            A[0, i] = (mySQLReader.GetValue(mySQLReader.GetOrdinal(item.PortCode)).Equals(DBNull.Value) ? 0 : mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode)) / 100 + 1) * tmp[0, i];
                            tmp[0, i] = A[0, i];
                            i++;
                        }
                        */
                    }
                    i = 0;
                    tmp_money = 0;

                    foreach (var item in cweight.OrderBy(t => t.PortCode))
                    {
                        A[0, i] = (mySQLReader.GetValue(mySQLReader.GetOrdinal(item.PortCode)).Equals(DBNull.Value) ? 0 : mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode)) / 100 + 1) * tmp[0, i];
                        //A[0, i] = (mySQLReader.GetValue(mySQLReader.GetOrdinal(item.PortCode)).Equals(DBNull.Value) ? 0 : mySQLReader.GetDouble(mySQLReader.GetOrdinal(item.PortCode)) + 1) * tmp[0, i];
                        tmp[0, i] = A[0, i];
                        tmp_money += tmp[0, i];
                        i++;
                    }
                    /*
                    if (rbDateArray.Contains(mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"))))
                    {
                        ret = tmp_money / money - 1;
                    }
                    else {
                        ret = tmp_money / ret - 1;
                    }
                    */
                    ret = tmp_money / money - 1;
                    money = tmp_money;
                    sd = (ret + 1)*tmp_sd;
                    tmp_sd = sd;
                    retList.Add(ret);
                    //test += tmp_sd.ToString() + "==";
                }   // end while
                mySQLReader.Close();

                //-------------Return
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_FindWorkingDay";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@dt", sDt.ToString("dd/MM/yyy", CultureInfo.CreateSpecificCulture("en-US")));
                mySQLReader = command.ExecuteReader();

                while (mySQLReader.Read())
                {
                    sDt = mySQLReader.GetDateTime(mySQLReader.GetOrdinal("DT1"));
                }
                mySQLReader.Close();
                //-------------Return

                //yr = Convert.ToInt32(Math.Floor(eDt.Subtract(sDt).TotalDays / 365));
                //yr = Math.Ceiling(eDt.Subtract(sDt).TotalDays / 365);
                yr = eDt.Subtract(sDt).TotalDays / 365;
                pow = 1 / yr;
                //yr = eDt.ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"));

                var statistics = new DescriptiveStatistics(retList);
                /*
                    WealthPCustomize wpc = new WealthPCustomize();
                wpc.FUND_CODE = mySQLReader.GetValue(mySQLReader.GetOrdinal("PORT_CODE")).Equals(DBNull.Value) ? null : mySQLReader.GetString(mySQLReader.GetOrdinal("PORT_CODE"));
                wpc.UPDATED_DATE = mySQLReader.GetValue(mySQLReader.GetOrdinal("VDATE")).Equals(DBNull.Value) ? null : mySQLReader.GetDateTime(mySQLReader.GetOrdinal("VDATE")).ToString("yyy-MM-dd", CultureInfo.CreateSpecificCulture("en-US"));
                wpc.SD = cc;
                WealthPCustomizeArray.Add(wpc);
                */
                /*
                Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                    {0.00011481,0.00106950,0.00049960,-0.00019555,0.00000019},
                    {0.00106950,0.02540500,0.00812000,0.00248750,0.00000287},
                    {0.00049960,0.00812000,0.00375600,0.00039700,0.00000110},
                    {-0.00019555,0.00248750,0.00039700,0.00228025,0.00000086},
                    {0.00000019,0.00000287,0.00000110,0.00000086,0.03240000}
                });
                */
                /*
                Matrix<double> Bt = B.Transpose();
                Matrix<double> C = B.Multiply(A.Multiply(Bt));
                double sd = Math.Round(Math.Sqrt(C.Trace()), 2);
                */
                //WealthPCustomize wpc = new WealthPCustomize();
                //wpc.SD = A[0,0];
                // 1/12 = 0.08333333333333333333333333333333
                /*
                wpc.RET = Math.Round((double)(Math.Pow(sd, pow) - 1 ) * 100, 2);
                wpc.SD = Math.Round(statistics.StandardDeviation * Math.Sqrt(252) * 100, 2);
                */
                wpc.RET = (double)(Math.Pow(sd, pow) - 1) ;
                wpc.SD = statistics.StandardDeviation * Math.Sqrt(252);
                //var statistics2 = retList.PopulationStandardDeviation();
                //wpc.SD = GetStandardDev(retList);
                //wpc.XX = yr.ToString();
                wpc.STATUS = "Success";
                //wpc.XX = retList.Average().ToString() + (retList.Sum()/62).ToString() + "___" + retList.Count.ToString() + "___" + yr.ToString();
                //(long)Math.Pow(value, power)
                //wpc.RET = (double)Math.Pow(2.16412, 0.1)-1 ;
                //wpc.RET = A[0, 0].ToString() + "**" + A[0, 1].ToString() + "**" + A[0, 2].ToString() + "**" + A[0, 3].ToString() + "**" + A[0, 4].ToString();//money.ToString();
                //wpc.RET  = String.Join(",",_port);
                //wpc.RET = String.Join(",", _porttest);
                //wpc.FUND_CODE = string.Join(",", _port);
                //wpc.FUND_CODE = _port2;

                WealthPCustomizeArray.Add(wpc);

                //return WealthPCustomizeArray;
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
 