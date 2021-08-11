using Mzg.DataMapping.Domain;
using System;

namespace Mzg.DataMapping
{
    public interface IEntityMapDependency
    {
        bool Create(EntityMap entity);

        bool Delete(params Guid[] id);

        bool Update(EntityMap entity);
    }
}