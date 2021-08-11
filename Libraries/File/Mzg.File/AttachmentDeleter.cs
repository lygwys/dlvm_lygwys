using Mzg.Context;
using Mzg.Identity;
using Mzg.Infrastructure.Utility;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzg.File
{
    /// <summary>
    /// 附件删除服务
    /// </summary>
    public class AttachmentDeleter : AttachmentService, IAttachmentDeleter
    {
        private readonly IAppContext _appContext;
        private readonly IDataFinder _dataFinder;
        private readonly IDataDeleter _dataDeleter;
        private readonly IWebHelper _webHelper;

        public AttachmentDeleter(IAppContext appContext
            , IDataFinder dataFinder
            , IDataDeleter dataDeleter
            , IWebHelper webHelper)
            : base(appContext)
        {
            _appContext = appContext;
            _dataFinder = dataFinder;
            _dataDeleter = dataDeleter;
            _webHelper = webHelper;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id">记录id</param>
        /// <returns></returns>
        public virtual bool DeleteById(Guid id)
        {
            return _dataDeleter.Delete(EntityName, id);
        }

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteById(List<Guid> ids)
        {
            return _dataDeleter.Delete(EntityName, ids);
        }

        /// <summary>
        /// 删除记录,删除单条数据对应的多个附件（批量删除）
        /// </summary>
        /// <param name="entityId">关联实体id</param>
        /// <param name="objectId">关联记录id</param>
        /// <returns></returns>
        public virtual bool DeleteByObjId(Guid entityId, Guid objectId)
        {
            //查询
            var query = new QueryExpression("attachment", _appContext.GetFeature<ICurrentUser>().UserSettings.LanguageId);
            query.ColumnSet.AddColumns("attachmentid", "cdnpath");
            query.Criteria.AddCondition("entityid", ConditionOperator.Equal, entityId);
            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, objectId);
            var entities = _dataFinder.RetrieveAll(query);
            var result = false;
            if (entities.NotEmpty())
            {
                result = _dataDeleter.Delete("attachment", entities.Select(x => x.GetIdValue()).ToList());
                if (result)
                {
                    //delete files
                    foreach (var item in entities)
                    {
                        var cdnPath = item.GetStringValue("cdnpath");
                        if (cdnPath.IsNotEmpty() && System.IO.File.Exists(_webHelper.MapPath(cdnPath)))
                        {
                            System.IO.File.Delete(_webHelper.MapPath(cdnPath));
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 删除记录,删除单个附件
        /// xmg
        /// 202011291300
        /// </summary>
        /// <param name="entityId">关联实体id</param>
        /// <param name="objectId">关联记录id</param>
        /// <returns></returns>
        public virtual bool DeleteFujianById(Guid AttachmentId)
        {
            //查询
            var query = new QueryExpression("attachment", _appContext.GetFeature<ICurrentUser>().UserSettings.LanguageId);
            query.ColumnSet.AddColumns("attachmentid", "cdnpath");
            query.Criteria.AddCondition("AttachmentId", ConditionOperator.Equal, AttachmentId);
            var entities = _dataFinder.RetrieveAll(query);
            var result = false;
            if (entities.NotEmpty())
            {
                result = _dataDeleter.Delete("attachment", entities.Select(x => x.GetIdValue()).ToList());
                if (result)
                {
                    //delete files
                    foreach (var item in entities)
                    {
                        var cdnPath = item.GetStringValue("cdnpath");
                        if (cdnPath.IsNotEmpty() && System.IO.File.Exists(_webHelper.MapPath(cdnPath)))
                        {
                            System.IO.File.Delete(_webHelper.MapPath(cdnPath));
                        }
                    }
                }
            }
            return result;
        }
    }
}