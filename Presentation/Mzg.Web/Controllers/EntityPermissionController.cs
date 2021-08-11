using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mzg.Core;
using Mzg.Core.Context;
using Mzg.Core.Data;
using Mzg.Data.Provider;
using Mzg.Infrastructure.Utility;
using Mzg.Security.DataAuthorization;
using Mzg.Security.Domain;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Infrastructure;
using Mzg.Web.Framework.Models;
using Mzg.Web.Framework.Mvc;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 实体权限项控制器
    /// </summary>
    public class EntityPermissionController : AuthorizedControllerBase
    {
        private readonly IEntityPermissionService _entityPermissionService;

        public EntityPermissionController(IWebAppContext appContext
            , IEntityPermissionService entityPermissionService)
            : base(appContext)
        {
            _entityPermissionService = entityPermissionService;
        }

        [Description("实体权限列表")]
        public IActionResult Index(EntityPermissionsModel model)
        {
            model.SortBy = ExpressionHelper.GetPropertyName<EntityPermission>(n => n.Name);
            model.SortDirection = (int)SortDirection.Desc;
            FilterContainer<EntityPermission> container = FilterContainerBuilder.Build<EntityPermission>();
            if (model.Name.IsNotEmpty())
            {
                container.And(n => n.Name.Like(model.Name));
            }
            if (model.EntityName.IsNotEmpty())
            {
                container.And(n => n.EntityName.Like(model.EntityName));
            }
            if (model.State.HasValue)
            {
                container.And(n => n.State == model.State);
            }
            if (model.GetAll)
            {
                var result = _entityPermissionService.Query(x => x
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );
                model.Items = result;
                model.TotalItems = result.Count;
            }
            else
            {
                var result = _entityPermissionService.QueryPaged(x => x
                    .Page(model.Page, model.PageSize)
                    .Where(container)
                    .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                );
                model.Items = result.Items;
                model.TotalItems = result.TotalItems;
            }
            return DynamicResult(model, $"~/Views/Security/EntityPermissions.cshtml");
        }

        [HttpGet]
        [Description("实体权限编辑")]
        public IActionResult EditEntityPermission(Guid? id)
        {
            EditEntityPermissionModel model = new EditEntityPermissionModel();
            if (id.HasValue && !id.Value.Equals(Guid.Empty))
            {
                var entity = _entityPermissionService.FindById(id.Value);
                if (entity != null)
                {
                    entity.CopyTo(model);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.State = RecordState.Enabled;
            }
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = T["security_read"], Value = (AccessRightValue.Read).ToString() });
            types.Add(new SelectListItem() { Text = T["security_create"], Value = (AccessRightValue.Create).ToString() });
            types.Add(new SelectListItem() { Text = T["security_update"], Value = (AccessRightValue.Update).ToString() });
            types.Add(new SelectListItem() { Text = T["security_delete"], Value = (AccessRightValue.Delete).ToString() });
            types.Add(new SelectListItem() { Text = T["security_share"], Value = (AccessRightValue.Share).ToString() });
            types.Add(new SelectListItem() { Text = T["security_assign"], Value = (AccessRightValue.Assign).ToString() });
            types.Add(new SelectListItem() { Text = T["security_import"], Value = (AccessRightValue.Import).ToString() });
            types.Add(new SelectListItem() { Text = T["security_export"], Value = (AccessRightValue.Export).ToString() });
            types.Add(new SelectListItem() { Text = T["security_append"], Value = (AccessRightValue.Append).ToString() });
            types.Add(new SelectListItem() { Text = T["security_appendto"], Value = (AccessRightValue.AppendTo).ToString() });

            model.PermissionTypes = new SelectList(types, "value", "text");
            return View($"~/Views/Security/{WebContext.ActionName}.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("实体权限编辑")]
        public IActionResult EditEntityPermission(EditEntityPermissionModel model)
        {
            if (!Arguments.HasValue(model.EntityId))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                EntityPermission entity = new EntityPermission();
                model.CopyTo(entity);

                if (!entity.EntityPermissionId.Equals(Guid.Empty))
                {
                    return _entityPermissionService.Update(entity).UpdateResult(T);
                }
                else
                {
                    entity.EntityPermissionId = Guid.NewGuid();
                    return _entityPermissionService.Create(entity).CreateResult(T);
                }
            }
            return SaveFailure(GetModelErrors());
        }

        [Description("删除实体权限")]
        [HttpPost]
        public IActionResult DeleteEntityPermission([FromBody]DeleteManyModel model)
        {
            return _entityPermissionService.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}