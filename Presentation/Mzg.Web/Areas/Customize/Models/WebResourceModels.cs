using Microsoft.AspNetCore.Http;
using Mzg.Web.Framework.Paging;
using Mzg.WebResource.Abstractions;
using System;

namespace Mzg.Web.Customize.Models
{
    public class WebResourceModel : BasePaged<WebResource.Domain.WebResource>
    {
        public string Name { get; set; }
        public WebResourceType? WebResourceType { get; set; }
        public Guid? SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditWebResourceModel
    {
        public Guid? WebResourceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public string OldContent { get; set; }

        public WebResourceType WebResourceType { get; set; }
        public Guid SolutionId { get; set; }
        public int Type { get; set; }
        public IFormFile ResourceFile { get; set; }
    }
}