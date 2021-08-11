using Microsoft.AspNetCore.Http;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Paging;
using System;
using System.Collections.Generic;

namespace Mzg.Web.Models
{
    public class AttachmentsModel : BasePaged<Core.Data.Entity>
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }

        public Schema.Domain.Entity EntityMetaData { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetaDatas { get; set; }
    }

    public class CreateAttachmentModel
    {
        public string Name { get; set; }

        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    public class DeleteAttachmentModel : DeleteManyModel
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid AttachmentId { get; set; }
    }
}