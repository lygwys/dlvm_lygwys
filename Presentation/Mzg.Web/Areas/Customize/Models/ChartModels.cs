using Mzg.Business.DataAnalyse.Domain;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Paging;
using System;

namespace Mzg.Web.Customize.Models
{
    public class ChartModel : BasePaged<Chart>
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditChartModel
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public Guid SolutionId { get; set; }
        public string DataConfig { get; set; }
        public string PresentationConfig { get; set; }
        public Guid ChartId { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
    }

    public class SetChartStateModel : SetRecordStateModel
    {
        public Guid EntityId { get; set; }
    }
}