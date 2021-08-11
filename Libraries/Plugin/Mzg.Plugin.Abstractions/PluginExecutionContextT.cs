using Mzg.Core;
using Mzg.Core.Context;

namespace Mzg.Plugin.Abstractions
{
    public class PluginExecutionContextT<TData, KMetadata>
    {
        public OperationTypeEnum MessageName { get; set; }
        public OperationStage Stage { get; set; }
        public IUserContext User { get; set; }

        public TData Target { get; set; }
        public KMetadata metadata { get; set; }
    }
}