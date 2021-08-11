using Mzg.Organization.Domain;
using System.Collections.Generic;

namespace Mzg.Web.Models
{
    public class DbConfigurationModel
    {
        public string DataServerName { get; set; }
        public string DataAccountName { get; set; }
        public string DataPassword { get; set; }
        public string DatabaseName { get; set; }
        public int CommandTimeOut { get; set; }
    }

    public class InitializationModel
    {
        public string DataServerName { get; set; }
        public string DataAccountName { get; set; }
        public string DataPassword { get; set; }
        public string DatabaseName { get; set; }
        public int CommandTimeOut { get; set; }
        public List<OrganizationBase> OrganizationBases { get; set; }
    }
}