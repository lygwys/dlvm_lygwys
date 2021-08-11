using System.Collections.Generic;

namespace Mzg.Plugin.Domain
{
    public class PluginInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public AssemblyInfo Assembly { get; set; }
        public List<MethodInfo> MethodInfos { get; set; }
        public List<InstanceInfo> Instances { get; set; }
    }
}