using Mzg.Sdk.Abstractions.Query;
using Mzg.Web.Framework.Paging;
using System;

namespace Mzg.Web.Models
{
    public class DialogModel
    {
        public string InputId { get; set; }
        public bool SingleMode { get; set; }
        public Guid? attrid { get; set; }

        public string entityid { get; set; }
        public Guid? baseentityid { get; set; }

        public string CallBack { get; set; } = "function(){}";
    }

    public class FilterModel
    {
        public Guid EntityId { get; set; }
        public Schema.Domain.RelationShip RelationShipMeta { get; set; }
        public string DataType { get; set; }
        public string Field { get; set; }

        public Schema.Domain.Attribute AttributeMeta { get; set; }
        public FilterExpression Filter { get; set; }
    }

    public class SimpleFilterModel
    {
        public Guid EntityId { get; set; }
        public FilterExpression Filter { get; set; }
    }

    public class EntityRecordsModel : BasePaged<dynamic>
    {
        public Guid? EntityId { get; set; }
        public Guid? QueryId { get; set; }
    }
}