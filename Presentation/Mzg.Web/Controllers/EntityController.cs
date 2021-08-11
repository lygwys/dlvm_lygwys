using Microsoft.AspNetCore.Mvc;
using Mzg.Context;
using Mzg.Data.Export;
using Mzg.Identity;
using Mzg.Infrastructure.Utility;
using Mzg.QueryView;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.ComponentModel;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 实体数据控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityController : AuthenticatedControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IDataExporter _dataExporter;


        private readonly ICurrentUser _user;
        private readonly IAppContext _appContext;
        public EntityController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IDataExporter dataExporter
            )
            : base(appContext)
        {
            _queryViewFinder = queryViewFinder;
            _dataExporter = dataExporter;
            _appContext = appContext;
            _user = _appContext.GetFeature<ICurrentUser>();
        }

        #region 列表

        [Description("导出记录")]
        public IActionResult Export([FromBody]EntityGridModel model)
        {

            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            OrderExpression orderExp = null;
            if (model.IsSortBySeted)
            {
                orderExp = new OrderExpression(model.SortBy, model.SortDirection == 0 ? OrderType.Ascending : OrderType.Descending);
            }
            string filename = queryView.Name;

            if (!string.IsNullOrEmpty(model.FileName))
            {
                filename = model.FileName;
            }

            string path = _dataExporter.ToExcelFile(queryView, model.Filter, orderExp, filename, includePrimaryKey: model.includePrimaryKey, includeIndex: model.IncludeIndex, title: model.ExportTitle);
            if (path.IsEmpty())
            {
                return JError(T["list_nodata"]);
            }
            return JOk(path);
        }

        #endregion 列表

        #region 新建/编辑/查看记录

        [Description("更改记录状态")]
        [HttpPost]
        public IActionResult SetRecordState([FromBody]SetEntityRecordStateModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            return View(model);
        }

        #endregion 新建/编辑/查看记录
    }
}