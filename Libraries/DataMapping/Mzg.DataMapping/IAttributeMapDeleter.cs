using System;

namespace Mzg.DataMapping
{
    public interface IAttributeMapDeleter
    {
        bool DeleteById(params Guid[] id);

        bool DeleteByParentId(Guid entityMapId);
    }
}