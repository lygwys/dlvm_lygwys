using Mzg.Context;
using Mzg.Core;
using Mzg.Core.Data;
using Mzg.Identity;
using Mzg.Infrastructure.Inject;
using Mzg.Infrastructure.Utility;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件执行器
    /// xmg
    /// 20200526
    /// </summary>
    public class EntityPluginExecutor : IEntityPluginExecutor
    {
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IEntityPluginFileProvider _entityPluginFileProvider;
        private readonly IAppContext _appContext;
        private readonly IServiceResolver _serviceResolver;
        private readonly ICurrentUser _currentUser;

        public EntityPluginExecutor(IAppContext appContext
            , IEntityPluginFinder entityPluginFinder
            , IEntityPluginFileProvider entityPluginFileProvider
            , IServiceResolver serviceResolver)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _entityPluginFinder = entityPluginFinder;
            _entityPluginFileProvider = entityPluginFileProvider;
            _serviceResolver = serviceResolver;
        }

        /// <summary>
        /// 执行实体触发事件
        /// mzg
        /// 202011121724
        /// </summary>
        /// <param name="op"></param>
        /// <param name="stage"></param>
        /// <param name="entity"></param>
        /// <param name="entityMetadata"></param>
        /// <param name="attributeMetadatas"></param>
        public void Execute(OperationTypeEnum op, OperationStage stage, Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas)
        {
            var plugins = _entityPluginFinder.QueryByEntityId(entityMetadata.EntityId, Enum.GetName(typeof(OperationTypeEnum), op));
            if (plugins.NotEmpty())
            {
                foreach (var pg in plugins)
                {
                    if (pg.StateCode == RecordState.Disabled)
                    {
                        continue;
                    }
                    var pinstance = GetInstance(pg);
                    if (pinstance != null)
                    {
                        pinstance.Execute(new PluginExecutionContext()
                        {
                            MessageName = op
                            ,
                            Stage = stage
                            ,
                            Target = entity
                            ,
                            User = _currentUser
                            ,
                            EntityMetadata = entityMetadata
                            ,
                            AttributeMetadatas = attributeMetadatas
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 创建插件实例
        /// xmg
        /// 20200526
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEntityPlugin GetInstance(EntityPlugin entity)
        {
            IEntityPlugin _instance = null;
            if (_entityPluginFileProvider.LoadAssembly(entity))
            {

                _instance = (IEntityPlugin)_serviceResolver.ResolveUnregistered(Type.GetType(entity.ClassName, false, true));
            }
            return _instance;
        }
    }
}