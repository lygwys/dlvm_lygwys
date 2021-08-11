using Mzg.DataMapping.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.DataMapping
{
    public interface IEntityMapUpdater
    {
        bool Update(EntityMap entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}