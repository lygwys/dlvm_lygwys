using Mzg.Context;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Identity;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using System;

namespace Mzg.File
{
    /// <summary>
    /// 附件查找服务
    /// </summary>
    public class AttachmentFinder : AttachmentService, IAttachmentFinder
    {
        private readonly IAppContext _appContext;
        private readonly IDataFinder _dataFinder;

        public AttachmentFinder(IAppContext appContext
            , IDataFinder dataFinder) : base(appContext)
        {
            _appContext = appContext;
            _dataFinder = dataFinder;
        }

        public virtual Entity FindById(Guid id)
        {
            return _dataFinder.RetrieveById(EntityName, id);
        }

        public virtual PagedList<Entity> QueryPaged(int page, int pageSize, Guid entityId, Guid objectId)
        {
            var query = new QueryExpression(EntityName, _appContext.GetFeature<ICurrentUser>().UserSettings.LanguageId);
            query.PageInfo = new PagingInfo() { PageNumber = page, PageSize = pageSize };
            query.ColumnSet.AddColumns("attachmentid", "name", "cdnpath", "description", "createdon", "createdby", "ownerid", "owningbusinessunit");
            query.Criteria.AddCondition("entityid", ConditionOperator.Equal, entityId);
            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, objectId);
            query.AddOrder("createdon", OrderType.Descending);
            return _dataFinder.RetrieveMultiple(query);
        }
    }
}