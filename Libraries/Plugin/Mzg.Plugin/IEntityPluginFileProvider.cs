using Microsoft.AspNetCore.Http;
using Mzg.Plugin.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzg.Plugin
{
    public interface IEntityPluginFileProvider
    {
        string GetBaseDirectory();

        bool LoadAssembly(EntityPlugin entity);

        Task<string> Save(IFormFile file);

        string Save(string fileName);

        Task<List<PluginAnalysis>> BeforehandLoad(IFormFile file);

        List<PluginAnalysis> BeforehandLoad(string fileName);
    }
}