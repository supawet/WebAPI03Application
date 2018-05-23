using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI03Application.Models
{
    public class WealthPCustomize
    {
        //public string UPDATED_DATE { get; set; }
        //public string FUND_CODE { get; set; }
        public double? SD { get; set; }
        public double? RET { get; set; }
        public string STATUS { get; set; }
        //public string XX { get; set; }
    }

    public class CWeight
    {
        public string PortCode { get; set; }
        public double Weight { get; set; }
    }
}