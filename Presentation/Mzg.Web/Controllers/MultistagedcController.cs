using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.MultisDc;
using Mzg.MultisDc.Domain;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Mvc;
using Mzg.Web.Models;
using System;
using System.ComponentModel;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 多级字典控制器
    /// xmg
    /// 202007181113
    /// </summary>
    public class MultistagedcController : AuthorizedControllerBase
    {
        private readonly IMultistagedcService _MultistagedcService;
        private readonly IMultistagedcTreeBuilder _MultistagedcTreeBuilder;

        public MultistagedcController(IWebAppContext appContext
            , IMultistagedcService MultistagedcService
            , IMultistagedcTreeBuilder MultistagedcTreeBuilder)
            : base(appContext)
        {
            _MultistagedcService = MultistagedcService;
            _MultistagedcTreeBuilder = MultistagedcTreeBuilder;
        }

        [Description("权限项")]
        public IActionResult Index(MultistageModel model, bool isAutoComplete = false)
        {
            if (IsRequestJson)
            {
                if (!isAutoComplete)
                {
                    FilterContainer<MultistageDc> filter = FilterContainerBuilder.Build<MultistageDc>();
                    filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
                    if (model.IsEnable.HasValue)
                    {
                        filter.And(n => n.AuthorizationEnabled == model.IsEnable.Value);
                    }
                    if (model.IsShowAsMenu.HasValue)
                    {
                        filter.And(n => n.IsVisibled == model.IsShowAsMenu.Value);
                    }
                    var result = _MultistagedcTreeBuilder.Build(x => x
                    .Where(filter)
                    .Sort(s => s.SortAscending(ss => ss.DisplayOrder))
                    );
                    return JOk(result);
                }
                else
                {
                    FilterContainer<MultistageDc> filter = FilterContainerBuilder.Build<MultistageDc>();
                    filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
                    if (model.MethodName.IsNotEmpty())
                    {
                        filter.And(n => n.MethodName.Like(model.MethodName));
                    }
                    if (model.DisplayName.IsNotEmpty())
                    {
                        filter.And(n => n.DisplayName.Like(model.DisplayName));
                    }
                    if (model.GetAll)
                    {
                        var result = _MultistagedcService.Query(x => x
                            .Where(filter)
                            .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                            );
                        model.Items = result;
                        model.TotalItems = result.Count;
                    }
                    else
                    {
                        PagedList<MultistageDc> result = _MultistagedcService.QueryPaged(x => x
                            .Page(model.Page, model.PageSize)
                            .Where(filter)
                            .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                            );

                        model.Items = result.Items;
                        model.TotalItems = result.TotalItems;
                    }
                    return JOk(model);
                }
            }
            return View("~/Views/MultistageDic/Index.cshtml");
        }

        [HttpGet]
        [Description("权限项编辑")]
        public IActionResult EditMultistagedc(Guid? id)
        {
            EditMultistageModel model = new EditMultistageModel();
            if (id.HasValue && !id.Equals(Guid.Empty))
            {
                MultistageDc entity = _MultistagedcService.FindById(id.Value);
                if (IsRequestJson)
                {
                    return JOk(entity);
                }
            }

            return JOk(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("权限项编辑")]
        public IActionResult EditMultistagedc(EditMultistageModel model)
        {
            if (ModelState.IsValid)
            {
                MultistageDc entity = (model.MultistagedcId.HasValue && !model.MultistagedcId.Value.Equals(Guid.Empty)) ? _MultistagedcService.FindById(model.MultistagedcId.Value) : new MultistageDc();

                model.CopyTo(entity);

                if (model.MultistagedcId.HasValue && !model.MultistagedcId.Value.Equals(Guid.Empty))
                {
                    return _MultistagedcService.Update(entity).UpdateResult(T);
                }
                else
                {
                    entity.MultistagedcId = Guid.NewGuid();
                    entity.OrganizationId = CurrentUser.OrganizationId;
                    return _MultistagedcService.Create(entity).CreateResult(T);
                }
            }
            return JError(GetModelErrors());
        }

        [Description("权限项移动排序")]
        public IActionResult MoveMultistagedc(Guid moveid, Guid targetid, Guid parentid, string position)
        {
            int status = _MultistagedcService.Move(moveid, targetid, parentid, position);

            if (status == 1)
            {
                return SaveSuccess();
            }
            else
            {
                return SaveFailure();
            }
        }

        [Description("删除权限项")]
        [HttpPost]
        public IActionResult DeleteMultistagedc([FromBody]DeleteManyModel model)
        {
            return _MultistagedcService.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}