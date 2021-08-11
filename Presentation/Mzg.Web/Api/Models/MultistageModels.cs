using Mzg.Web.Framework.Paging;
using System;

namespace Mzg.Web.Api.Models
{
    public class RetrieveMultistageModel : BasePaged<Schema.Domain.Multistagedc>
    {
        public string[] Name { get; set; }
        public bool? IsLoged { get; set; }
        public bool? IsCustomizable { get; set; }
        public bool? IsAuthorization { get; set; }
        public bool? DuplicateEnabled { get; set; }
        public bool? WorkFlowEnabled { get; set; }
        public bool? BusinessFlowEnabled { get; set; }
        public Guid? SolutionId { get; set; }
    }
}