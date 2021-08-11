using System;

namespace Mzg.Web.Api.Models
{
    public class SavePrivilegeResourceModel
    {
        public string ResourceName { get; set; }

        public Guid[] ObjectId { get; set; }
    }

    public class UpdateAuthorizationStateModel
    {
        public Guid[] ObjectId { get; set; }
    }
}