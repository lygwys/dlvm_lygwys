using Mzg.Core;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Domain;
using System;

namespace Mzg.Plugin
{
    public interface IPluginExecutor<TData, KMetadata>
    {
        void Execute(Guid entityId, Guid? businessObjectId, PlugInType typeCode, OperationTypeEnum op, OperationStage stage, TData tData, KMetadata kMetadata);

        IPlugin<TData, KMetadata> GetInstance(EntityPlugin entity);
    }
}