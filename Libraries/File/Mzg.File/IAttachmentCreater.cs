using Microsoft.AspNetCore.Http;
using Mzg.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzg.File
{
    public interface IAttachmentCreater
    {
        Task<Entity> CreateAsync(Guid entityId, Guid objectId, IFormFile file);

        Task<List<Entity>> CreateManyAsync(Guid entityId, Guid objectId, List<IFormFile> files, bool autoFileName = true);

    }
}