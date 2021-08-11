using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Domain;
using Mzg.Schema.Entity;
using Mzg.Sdk.Client;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Mvc;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 数据删除接口
    /// </summary>
    [Route("{org}/api/data/delete")]
    public class DataDeleterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDataDeleter _dataDeleter;

        public DataDeleterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IDataDeleter dataDeleter)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _dataDeleter = dataDeleter;
        }

        [Description("删除记录")]
        [HttpPost]
        public IActionResult Delete(DeleteEntityRecordModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            Entity entityMetadata = null;
            if (model.EntityName.IsNotEmpty())
            {
                entityMetadata = _entityFinder.FindByName(model.EntityName);
            }
            else
            {
                entityMetadata = _entityFinder.FindById(model.EntityId);
            }
            if (entityMetadata == null)
            {
                return NotFound();
            }
            return _dataDeleter.Delete(entityMetadata.Name, model.RecordId, model.IsPermission).DeleteResult(T);
        }
    }
}