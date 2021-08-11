using Microsoft.AspNetCore.Http;
using Mzg.Plugin.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzg.Plugin
{
    public interface IEntityPluginUpdater
    {
        Task<bool> Update(EntityPlugin entity, IFormFile file);

        bool Update(EntityPlugin entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}