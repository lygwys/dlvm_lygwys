using System;

namespace Mzg.Web.Api.Models
{
    public class WorkFlowInstanceCancleModel
    {
        public Guid Id { get; set; }
    }

    public class WorkFlowCancelModel
    {
        public Guid EntityId { get; set; }
        public Guid RecordId { get; set; }
    }
}