using Mzg.Core;
using Mzg.Core.Data;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Domain;
using System.Collections.Generic;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件执行器
    /// xmg
    /// 202006301746
    /// </summary>
    public interface IEntityPluginExecutor
    {
        void Execute(OperationTypeEnum op, OperationStage stage, Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas);

        IEntityPlugin GetInstance(EntityPlugin entity);
    }
}