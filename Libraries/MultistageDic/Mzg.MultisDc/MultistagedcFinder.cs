using Mzg.Context;
using Mzg.Core.Context;
using Mzg.Data.Provider;
using Mzg.Dependency.Abstractions;
using Mzg.Infrastructure.Utility;
using Mzg.Module.Core;
using Mzg.MultisDc.Abstractions;
using Mzg.MultisDc.Data;
using Mzg.MultisDc.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.MultisDc
{
    public class MultistagedcFinder : IMultistagedcFinder, IDependentLookup<Domain.MultistageDc>
    {
        private readonly IMultistagedcRepository _MultistageDcRepository;
        private readonly Caching.CacheManager<MultistageDc> _cacheService;
        private readonly IAppContext _appContext;



        public MultistagedcFinder(IAppContext appContext
            , IMultistagedcRepository MultistagedcRepository)
        {
            _appContext = appContext;
            _MultistageDcRepository = MultistagedcRepository;
            _cacheService = new Caching.CacheManager<Domain.MultistageDc>(_appContext.OrganizationUniqueName + ":Multistagedc", _appContext.PlatformSettings.CacheEnabled);
        }
        public MultistageDc FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("MultistageDcid", id.ToString());
            var entity = _cacheService.Get(dic, () =>
            {
                return _MultistageDcRepository.FindById(id);
            });


            return entity;
        }

        public MultistageDc Find(string systemName, string className, string methodName)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("systemName", systemName);
            dic.Add("className", className);
            dic.Add("MethodName", methodName);
            var entity = _cacheService.Get(dic, () =>
            {
                return this.Find(n => n.Where(f => f.SystemName == systemName && f.ClassName == className && f.MethodName == methodName));
            });
            return entity;
        }

        public MultistageDc Find(string url)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("url", url.UrlEncode());
            var entity = _cacheService.Get(dic, () =>
            {
                return this.Find(n => n.Where(f => f.Url == url));
            });
            return entity;
        }

        public MultistageDc Find(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container)
        {
            QueryDescriptor<MultistageDc> q = container(QueryDescriptorBuilder.Build<MultistageDc>());
            return _MultistageDcRepository.Find(q);
        }

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.DisplayName } : null;
        }
        public int ComponentType => ModuleCollection.GetIdentity(MultisDcDefaults.ModuleName);
    }
}
