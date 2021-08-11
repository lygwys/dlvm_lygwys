using System;

namespace Mzg.DataMapping
{
    public interface IEntityMapDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}