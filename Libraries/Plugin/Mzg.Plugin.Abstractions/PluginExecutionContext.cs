using Mzg.Core;
using Mzg.Core.Context;
using Mzg.Core.Data;
using System.Collections.Generic;

namespace Mzg.Plugin.Abstractions
{
    public class PluginExecutionContext
    {
        public OperationTypeEnum MessageName { get; set; }
        public Entity Target { get; set; }

        public OperationStage Stage { get; set; }

        public IUserContext User { get; set; }
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}