using Mzg.Authorization.Abstractions;
using Mzg.Context;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Abstractions;
using Mzg.Data.Provider;
using Mzg.Event.Abstractions;
using Mzg.Infrastructure.Utility;
using Mzg.Localization;
using Mzg.MultisDc.Data;
using Mzg.MultisDc.Domain;
using Mzg.SiteMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzg.MultisDc
{

    /// <summary>
    /// 多级字典服务接口的实现
    /// xmg
    /// 202007181004
    /// </summary>
    public class MultistagedcService : IMultistagedcService
    {
        private readonly IMultistagedcRepository _MultistageDcRepository; // 数据仓储接口
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IEventPublisher _eventPublisher;
        private readonly Caching.CacheManager<MultistageDc> _cacheService;
        private readonly IAppContext _appContext;

        public MultistagedcService(IAppContext appContext
            , IMultistagedcRepository MultistageDcRepository
            , ILocalizedLabelService localizedLabelService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _MultistageDcRepository = MultistageDcRepository;
            _localizedLabelService = localizedLabelService;
            _eventPublisher = eventPublisher;
            _cacheService = new Caching.CacheManager<MultistageDc>(_appContext.OrganizationUniqueName + ":MultistageDc", BuildKey, _appContext.PlatformSettings.CacheEnabled, PreCacheAll);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExists(MultistageDc entity)
        {
            if (entity.SystemName.IsNotEmpty() && entity.ClassName.IsNotEmpty() && entity.MethodName.IsNotEmpty())
            {
                var isExists = this.Find(n => n.Where(f => f.ClassName == entity.ClassName && f.MethodName == entity.MethodName && f.MultistagedcId != entity.MultistagedcId)) != null;
                if (isExists)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Create(MultistageDc entity)
        {
            //判断重复
            if (IsExists(entity))
            {
                return false;
            }

            if (!entity.ParentMultistagedcId.Equals(Guid.Empty))
            {
                var parent = FindById(entity.ParentMultistagedcId);
                if (parent != null)
                {
                    entity.Level = parent.Level + 1;
                }
            }
            if (entity.Level <= 0)
            {
                entity.Level = 1;
            }
            var flag = _MultistageDcRepository.Create(entity);
            if (flag)
            {
                //add to cache
                _cacheService.SetEntity(entity);
                //本地化标签
                //_localizedLabelService.Create(SolutionService.DefaultSolutionId, entity.DisplayName.IfEmpty(""), LabelTypeCodeEnum.MultistageDc, "DisplayName", entity.MultistageDcId, this._appContext.Org.LanguageId);
            }
            return flag;
        }

        public bool Update(MultistageDc entity)
        {
            //判断重复
            if (IsExists(entity))
            {
                return false;
            }
            var original = _MultistageDcRepository.FindById(entity.MultistagedcId);
            var flag = _MultistageDcRepository.Update(entity);
            if (flag)
            {
                //localization
                _localizedLabelService.Update(entity.DisplayName.IfEmpty(""), "DisplayName", entity.MultistagedcId, this._appContext.BaseLanguage);
                //assigning roles
                if (original.AuthorizationEnabled || !entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.MultistagedcId }
                        ,
                        State = false
                        ,
                        ResourceName = SiteMapDefaults.ModuleName
                    });
                }

                //add to cache
                _cacheService.SetEntity(entity);
            }
            return flag;
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<Domain.MultistageDc>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization);
            context.Where(f => f.MultistagedcId.In(id));
            var result = true;
            using (UnitOfWork.Build(_MultistageDcRepository.DbContext))
            {
                result = _MultistageDcRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = SiteMapDefaults.ModuleName
                });
                //set to cache
                var items = _MultistageDcRepository.Query(f => f.MultistagedcId.In(id)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }

        public MultistageDc FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("MultistageDcid", id.ToString());
            var entity = _cacheService.Get(dic, () =>
            {
                return _MultistageDcRepository.FindById(id);
            });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }

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

        public bool DeleteById(Guid id)
        {
            var entity = _MultistageDcRepository.FindById(id);
            var flag = _MultistageDcRepository.DeleteById(id);
            if (flag)
            {
                //localization
                _localizedLabelService.DeleteByObject(id);
                //remove from cache
                _cacheService.RemoveEntity(entity);
            }
            return flag;
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        public PagedList<MultistageDc> QueryPaged(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container)
        {
            QueryDescriptor<MultistageDc> q = container(QueryDescriptorBuilder.Build<MultistageDc>());

            var datas = _MultistageDcRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<MultistageDc> Query(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container)
        {
            QueryDescriptor<MultistageDc> q = container(QueryDescriptorBuilder.Build<MultistageDc>());
            var datas = _MultistageDcRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<MultistageDc> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
            {
                return PreCacheAll();
            });
            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        private List<MultistageDc> PreCacheAll()
        {
            return _MultistageDcRepository.FindAll()?.ToList();
        }

        public List<MultistageDc> AllMultistagedcs
        {
            get
            {
                List<MultistageDc> entities = _cacheService.GetVersionItems("allOrder", () =>
                {
                    return this.Query(n => n.Sort(s => s.SortDescending(f => f.DisplayOrder)));
                });
                return entities;
            }
        }

        public MultistageDc Find(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container)
        {
            QueryDescriptor<MultistageDc> q = container(QueryDescriptorBuilder.Build<MultistageDc>());
            return _MultistageDcRepository.Find(q);
        }

        public int Move(Guid moveid, Guid targetid, Guid parentid, string position)
        {
            int result = _MultistageDcRepository.MoveNode(moveid, targetid, parentid, position);
            return result;
        }

        private void WrapLocalizedLabel(IEnumerable<MultistageDc> entities)
        {
            //if (entities.NotEmpty())
            //{
            //    var ids = entities.Select(f => f.MultistageDcId);
            //    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
            //    foreach (var d in entities)
            //    {
            //        d.DisplayName = _localizedLabelService.GetLabelText(labels, d.MultistageDcId, "DisplayName", d.DisplayName);
            //    }
            //}
        }

        private void WrapLocalizedLabel(MultistageDc entity)
        {
            //var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.MultistageDcId));
            //entity.DisplayName = _localizedLabelService.GetLabelText(labels, entity.MultistageDcId, "DisplayName");
        }

        private string BuildKey(MultistageDc entity)
        {
            return entity.MultistagedcId + "/" + entity.SystemName + "/" + entity.ClassName + "/" + entity.MethodName + "/" + entity.Url.UrlEncode() + "/";
        }
    }
}
