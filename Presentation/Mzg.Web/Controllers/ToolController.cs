using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Security.Verify;
using Mzg.SiteMap;
using Mzg.Web.Framework;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Controllers
{
    /// <summary>
    /// 常用服务控制器
    /// </summary>
    public class ToolController : WebControllerBase
    {
        private readonly IVerifyProvider _verifyProvider;

        public ToolController(IWebAppContext appContext
            , IVerifyProvider verifyProvider)
            : base(appContext)
        {
            _verifyProvider = verifyProvider;
        }

        /// <summary>
        /// 验证图片
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Description("验证码")]
        public ImageResult VerifyImage()
        {
            var verifyValue = _verifyProvider.Get();
            return new ImageResult(verifyValue.Value, verifyValue.MediaType);
        }

        [Description("选择控制器方法对话框")]
        public IActionResult SelectActionsDialog(PrivilegeModel model, DialogModel dm)
        {
            ViewData["DialogModel"] = dm;
            var result = AssemblyService.GetAllActionByAssembly();
            if (model.ClassName.IsNotEmpty())
            {
                result = result.Where(n => n.ClassName.IndexOf(model.ClassName, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            model.Items = result.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize).ToList();
            model.TotalItems = result.Count;
            return View(model);
        }
    }
}