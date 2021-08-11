using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;

namespace Mzg.Web.Reports
{
    public class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["XtraReport"] = () => new XtraReport()
        };
    }
}
