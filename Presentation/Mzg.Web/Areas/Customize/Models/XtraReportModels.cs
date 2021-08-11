using Mzg.Business.XtraReportManager.Domain;
using Mzg.Core;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Paging;
using System;
using System.Collections.Generic;

namespace Mzg.Web.Customize.Models
{
    public class XtraReportModel : BasePaged<XtraReport>
    {
        public string Name { get; set; }
        public RecordState? StateCode { get; set; }

        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }

        public string reportName { get; set; }
        public Guid? id { get; set; }
        public Guid? attribut1 { get; set; }
        public Guid? attribut2 { get; set; }
        public Guid? attribut3 { get; set; }
        public Guid? attribut4 { get; set; }
        public Guid? attribut5 { get; set; }
        public Guid? attribut6 { get; set; }
        public Guid? attribut7 { get; set; }
        public Guid? attribut8 { get; set; }
        public Guid? attribut9 { get; set; }
        public Guid? attribut10 { get; set; }

        public string p1 { get; set; }
        public string p2 { get; set; }
        public string p3 { get; set; }
        public string p4 { get; set; }
        public string p5 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public string p8 { get; set; }
        public string p9 { get; set; }
        public string p10 { get; set; }
    }

    public class EditXtraReportModel
    {
        public Guid? XtraReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Intercepted { get; set; }
        public Guid EntityId { get; set; }
        public List<string> parameter { get; set; }


        public RecordState StateCode { get; set; }

        public Guid CreatedBy { get; set; }
        public List<Guid> AttributeId { get; set; }
        public List<Guid> DetailId { get; set; }

        public List<XtraReportCondition> Conditions { get; set; }
        public Guid SolutionId { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
    }

    public class SetXtraReportStateModel : SetRecordStateModel
    {
        public Guid EntityId { get; set; }
    }
}
