using Mzg.Context;
using Mzg.Core;
using Mzg.Identity;
using Mzg.Infrastructure.Inject;
using Mzg.Infrastructure.Utility;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Domain;
using System;

namespace Mzg.Plugin
{
    /// <summary>
    /// 实体插件执行器
    /// </summary>
    public class PluginExecutor<TData, KMetadata> : IPluginExecutor<TData, KMetadata>
    {
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IEntityPluginFileProvider _entityPluginFileProvider;
        private readonly IAppContext _appContext;
        private readonly IServiceResolver _serviceResolver;
        private readonly ICurrentUser _currentUser;

        public PluginExecutor(IAppContext appContext
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
        /// 执行插件触发事件
        /// mzg
        /// 202011121730
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="businessObjectId"></param>
        /// <param name="typeCode"></param>
        /// <param name="op"></param>
        /// <param name="stage"></param>
        /// <param name="tData"></param>
        /// <param name="kMetadata"></param>
        public void Execute(Guid entityId, Guid? businessObjectId, PlugInType typeCode, OperationTypeEnum op, OperationStage stage, TData tData, KMetadata kMetadata)
        {
            var plugins = _entityPluginFinder.QueryByEntityId(entityId, Enum.GetName(typeof(OperationTypeEnum), op), businessObjectId, typeCode);

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
                        pinstance.Execute(new PluginExecutionContextT<TData, KMetadata>()
                        {
                            MessageName = op
                            ,
                            Stage = stage
                            ,
                            User = _currentUser
                            ,
                            Target = tData
                            ,
                            metadata = kMetadata
                        });
                    }
                }
            }
        }

        public IPlugin<TData, KMetadata> GetInstance(EntityPlugin entity)
        {
            IPlugin<TData, KMetadata> _instance = null;
            if (_entityPluginFileProvider.LoadAssembly(entity))
            {
                _instance = (IPlugin<TData, KMetadata>)_serviceResolver.ResolveUnregistered(Type.GetType(entity.ClassName, false, true));
            }
            return _instance;
        }
    }
}