﻿using Mzg.Business.Filter.Domain;
using Mzg.Core;
using Mzg.Web.Framework.Paging;
using System;

namespace Mzg.Web.Customize.Models
{
    public class FilterRulesModel : BasePaged<FilterRule>
    {
        public Guid EntityId { get; set; }
        public bool LoadData { get; set; }
    }

    public class CreateFilterRuleModel
    {
        public Guid EntityId { get; set; }
        public string Name { get; set; }

        //public List<ConditionExpression> Conditions { get; set; }
        //public LogicalOperator LogicalOperator { get; set; }
        public string Conditions { get; set; }

        public string ToolTip { get; set; }
        public string EventName { get; set; }
        public RecordState StateCode { get; set; }
    }

    public class EditFilterRuleModel
    {
        public Guid FilterRuleId { get; set; }
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public string Conditions { get; set; }
        public string ToolTip { get; set; }
        public string EventName { get; set; }
        public RecordState StateCode { get; set; }
        public Schema.Domain.Entity EntityMeta { get; set; }
    }
}