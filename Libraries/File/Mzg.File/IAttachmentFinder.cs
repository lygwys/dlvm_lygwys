using Mzg.Core.Context;
using Mzg.Core.Data;
using System;

namespace Mzg.File
{
    public interface IAttachmentFinder
    {
        Entity FindById(Guid id);

        PagedList<Entity> QueryPaged(int page, int pageSize, Guid entityId, Guid objectId);
    }
}