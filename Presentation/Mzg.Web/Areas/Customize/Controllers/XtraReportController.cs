using Microsoft.AspNetCore.Mvc;
using Mzg.Business.XtraReportManager;
using Mzg.Business.XtraReportManager.Domain;
using Mzg.Core;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Entity;
using Mzg.Solution;
using Mzg.Web.Customize.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Customize.Controllers
{
    /// <summary>
    /// 报表管理管理控制器
    /// </summary>
    public class XtraReportController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IXtraReportCreater _xtraReportCreater;
        private readonly IXtraReportUpdater _xtraReportUpdater;
        private readonly IXtraReportFinder _xtraReportFinder;
        private readonly IXtraReportDeleter _xtraReportDeleter;
        private readonly IXtraReportConditionService _xtraReportConditionService;

        public XtraReportController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            , IXtraReportCreater xtraReportCreater
            , IXtraReportUpdater xtraReportUpdater
            , IXtraReportFinder xtraReportFinder
            , IXtraReportDeleter xtraReportDeleter
            , IXtraReportConditionService xtraReportConditionService)
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
            _xtraReportCreater = xtraReportCreater;
            _xtraReportUpdater = xtraReportUpdater;
            _xtraReportFinder = xtraReportFinder;
            _xtraReportDeleter = xtraReportDeleter;
            _xtraReportConditionService = xtraReportConditionService;
        }

        [Description("数据报表管理列表")]
        public IActionResult Index(XtraReportModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }

            FilterContainer<XtraReport> filter = FilterContainerBuilder.Build<XtraReport>();
            if (!model.EntityId.Equals(Guid.Empty))
            {
                filter.And(n => n.EntityId == model.EntityId);
            }
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name.Like(model.Name));
            }
            if (model.GetAll)
            {
                List<XtraReport> result = _xtraReportFinder.Query(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                if (CurrentUser.UserSettings.PagingLimit > 0)
                {
                    model.PageSize = CurrentUser.UserSettings.PagingLimit;
                }
                PagedList<XtraReport> result = _xtraReportFinder.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(filter)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                    );

                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            model.SolutionId = SolutionId.Value;
            return DynamicResult(model);
        }

        [HttpGet]
        [Description("新建数据报表管理")]
        public IActionResult CreateXtraReport(Guid entityid)
        {
            EditXtraReportModel model = new EditXtraReportModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = RecordState.Enabled
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建数据报表管理-保存")]
        public IActionResult CreateXtraReport(EditXtraReportModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new XtraReport();
                model.CopyTo(entity);
                entity.XtraReportId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                var conditions = new List<XtraReportCondition>();
                int i = 0;
                foreach (var item in model.AttributeId)
                {
                    var cd = new XtraReportCondition
                    {
                        XtraReportParametersId = Guid.NewGuid(),
                        XtraReportId = entity.XtraReportId,
                        EntityId = model.EntityId,
                        CreatedOn = DateTime.Now,
                        AttributeId = item,
                        parameter = model.parameter[i]
                    };
                    conditions.Add(cd);
                    i++;
                }
                entity.Conditions = conditions;

                _xtraReportCreater.Create(entity);
                return CreateSuccess(new { id = entity.XtraReportId });
            }
            var msg = GetModelErrors(ModelState);
            return CreateFailure(msg);
        }

        [HttpGet]
        [Description("数据报表管理编辑")]
        public IActionResult EditXtraReport(Guid id)
        {
            EditXtraReportModel model = new EditXtraReportModel();
            if (!id.Equals(Guid.Empty))
            {
                var entity = _xtraReportFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Conditions = _xtraReportConditionService.Query(n => n.Where(w => w.XtraReportId == entity.XtraReportId));
                    model.EntityMeta = _entityFinder.FindById(entity.EntityId);
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("数据报表管理保存")]
        public IActionResult EditXtraReport(EditXtraReportModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _xtraReportFinder.FindById(model.XtraReportId.Value);
                entity.Description = model.Description;
                entity.Name = model.Name;

                var conditions = _xtraReportConditionService.Query(n => n.Where(w => w.XtraReportId == entity.XtraReportId));
                int i = 0;
                entity.Conditions = new List<XtraReportCondition>();
                foreach (var item in model.AttributeId)
                {
                    var id = model.DetailId[i];
                    var condition = new XtraReportCondition
                    {
                        XtraReportId = entity.XtraReportId,
                        EntityId = model.EntityId,
                        AttributeId = item,
                        parameter = model.parameter[i]
                    };
                    if (id.Equals(Guid.Empty))
                    {
                        condition.XtraReportParametersId = Guid.NewGuid();
                        _xtraReportConditionService.Create(condition);
                    }
                    else
                    {
                        condition.XtraReportParametersId = id;
                        _xtraReportConditionService.Update(condition);
                        conditions.Remove(conditions.Find(n => n.XtraReportParametersId == id));
                    }
                    entity.Conditions.Add(condition);

                    i++;
                }
                //delete lost detail
                var lostid = conditions.Select(n => n.XtraReportParametersId).ToList();
                _xtraReportConditionService.DeleteById(lostid);//如果是删除操作，把循环更新后剩余的删掉

                _xtraReportUpdater.Update(entity);

                return UpdateSuccess(new { id = entity.XtraReportId });
            }
            var msg = GetModelErrors(ModelState);
            return UpdateFailure(msg);
        }

        [Description("删除数据报表管理")]
        [HttpPost]
        public IActionResult DeleteXtraReport([FromBody]DeleteManyModel model)
        {
            return _xtraReportDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }

        [Description("设置重复规则可用状态")]
        [HttpPost]
        public IActionResult SetXtraReportState([FromBody]SetXtraReportStateModel model)
        {
            return _xtraReportUpdater.UpdateState(model.RecordId, model.IsEnabled).UpdateResult(T);
        }

        [HttpGet]
        [Description("新建数据报表管理")]
        public IActionResult DesigReport(Guid entityid)
        {
            EditXtraReportModel model = new EditXtraReportModel
            {
                SolutionId = SolutionId.Value,
                EntityId = entityid,
                StateCode = RecordState.Enabled
            };
            return View(model);
        }
    }
}