using Microsoft.AspNetCore.Http;
using Mzg.Plugin.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzg.Plugin
{
    public interface IEntityPluginCreater
    {
        bool Create(EntityPlugin entity);

        Task<bool> Create(EntityPlugin entity, IFormFile file);

        bool Create(EntityPlugin entity, string fileName);

        Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file);

        List<PluginAnalysis> BeforehandLoad(string fileName);
    }
}