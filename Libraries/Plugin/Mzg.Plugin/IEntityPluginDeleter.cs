using System;

namespace Mzg.Plugin
{
    public interface IEntityPluginDeleter
    {
        bool DeleteById(params Guid[] id);

        bool DeleteByEntityId(Guid entityId, string assembly);
    }
}