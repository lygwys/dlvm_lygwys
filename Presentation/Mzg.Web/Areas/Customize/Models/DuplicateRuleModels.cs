using Mzg.Business.DuplicateValidator.Domain;
using Mzg.Core;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Paging;
using System;
using System.Collections.Generic;

namespace Mzg.Web.Customize.Models
{
    public class DuplicateRuleModel : BasePaged<DuplicateRule>
    {
        public string Name { get; set; }
        public RecordState? StateCode { get; set; }

        public Guid EntityId { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditDuplicateRuleModel
    {
        public Guid? DuplicateRuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Intercepted { get; set; }
        public Guid EntityId { get; set; }
        public string EventName { get; set; }
        public List<bool> IgnoreNullValues { get; set; }
        public List<bool> IsCaseSensitive { get; set; }

        public RecordState StateCode { get; set; }

        public Guid CreatedBy { get; set; }
        public List<Guid> AttributeId { get; set; }
        public List<Guid> DetailId { get; set; }

        public List<DuplicateRuleCondition> Conditions { get; set; }
        public Guid SolutionId { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
    }

    public class SetDuplicateRuleStateModel : SetRecordStateModel
    {
        public Guid EntityId { get; set; }
    }
}