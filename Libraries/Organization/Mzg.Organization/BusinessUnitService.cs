using Mzg.Core.Context;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.Organization.Data;
using Mzg.Organization.Domain;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Mzg.Organization
{
    /// <summary>
    /// 业务部门服务
    /// </summary>
    public class BusinessUnitService : IBusinessUnitService
    {
        private readonly IBusinessUnitRepository _businessUnitRepository;
        //private readonly ILocalizedTextProvider _loc;
        //private readonly ISystemUserService _systemUserService;

        public BusinessUnitService(//IWebAppContext appContext
            IBusinessUnitRepository businessUnitRepository
            //, ISystemUserService systemUserService
            )
        {
            //_loc = appContext.T;
            _businessUnitRepository = businessUnitRepository;
            //_systemUserService = systemUserService;
        }

        public bool Create(BusinessUnit entity)
        {
            return _businessUnitRepository.Create(entity);
        }

        public bool Update(BusinessUnit entity)
        {
            return _businessUnitRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<BusinessUnit>, UpdateContext<BusinessUnit>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<BusinessUnit>());
            return _businessUnitRepository.Update(ctx);
        }

        public BusinessUnit FindById(Guid id)
        {
            return _businessUnitRepository.FindById(id);
        }

        public bool DeleteById(Guid id)
        {
            //检查部门下是否有用户
            //var hasUser = _systemUserService.Find(n => n.BusinessUnitId == id) != null;
            //if (hasUser)
            //{
            //    throw new XmsException(_loc["referenced"]);
            //}
            return _businessUnitRepository.DeleteById(id);
        }

        public bool DeleteById(List<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
            //return _repository.DeleteById(ids);
        }

        public PagedList<BusinessUnit> QueryPaged(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container)
        {
            QueryDescriptor<BusinessUnit> q = container(QueryDescriptorBuilder.Build<BusinessUnit>());

            return _businessUnitRepository.QueryPaged(q);
        }

        public List<BusinessUnit> Query(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container)
        {
            QueryDescriptor<BusinessUnit> q = container(QueryDescriptorBuilder.Build<BusinessUnit>());

            return _businessUnitRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 获取所有下级部门
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<BusinessUnit> GetChilds(Guid parentId)
        {
            return _businessUnitRepository.GetChilds(parentId);
        }

        /// <summary>
        /// 是否子部门
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public bool IsChild(Guid parentId, Guid businessUnitId)
        {
            return _businessUnitRepository.IsChild(parentId, businessUnitId);
        }

        public string Build(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container, bool nameLower = true)
        {
            List<BusinessUnit> list = Query(container);

            List<dynamic> dlist = Build(list, Guid.Empty);
            dynamic contact = new ExpandoObject();
            contact.label = "root";
            contact.id = Guid.Empty;
            contact.children = dlist;

            List<dynamic> results = new List<dynamic>();
            results.Add(contact);

            var json = results.SerializeToJson(nameLower);
            return json;
        }

        public List<dynamic> Build(List<BusinessUnit> BusinessUnitList, Guid parentId)
        {
            List<dynamic> dynamicList = new List<dynamic>();
            List<BusinessUnit> childList = new List<BusinessUnit>();
            if (parentId.IsEmpty())
            {
                childList = BusinessUnitList.Where(n => n.ParentBusinessUnitId is null).OrderBy(n => n.Name).ToList();
            }
            else
            {
                childList = BusinessUnitList.Where(n => n.ParentBusinessUnitId == parentId).OrderBy(n => n.Name).ToList();
            }
            if (childList != null && childList.Count > 0)
            {
                List<dynamic> ddList = new List<dynamic>();
                dynamic contact = new ExpandoObject();
                foreach (var item in childList)
                {
                    contact = new ExpandoObject();
                    contact.label = item.Name;
                    contact.id = item.BusinessUnitId;

                    if (BusinessUnitList.Find(n => n.ParentBusinessUnitId == item.BusinessUnitId) != null)
                    {
                        ddList = Build(BusinessUnitList, item.BusinessUnitId);
                        if (ddList.Count > 0)
                        {
                            contact.children = ddList;
                            ddList = new List<dynamic>();
                        }
                    }
                    dynamicList.Add(contact);
                }
            }
            return dynamicList;
        }

    }
}