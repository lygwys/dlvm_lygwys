using System;
using System.Collections.Generic;

namespace Mzg.File
{
    public interface IAttachmentDeleter
    {
        bool DeleteById(Guid id);

        bool DeleteFujianById(Guid Id);
        bool DeleteByObjId(Guid entityId, Guid objectId);

        bool DeleteById(List<Guid> ids);
    }
}