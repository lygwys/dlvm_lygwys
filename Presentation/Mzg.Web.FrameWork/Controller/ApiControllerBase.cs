﻿using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Filters;
using Mzg.Web.Framework.Mvc;

namespace Mzg.Web.Framework.Controller
{
    /// <summary>
    /// 接口控制器基类
    /// </summary>
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    [ApiController]
    //[EnableCors("CorsPolicy")]//跨域
    public class ApiControllerBase : XmsControllerBase
    {
        protected ApiControllerBase(IWebAppContext appContext) : base(appContext)
        {
        }

        #region 常用返回信息

        protected IActionResult JError(object content, object extra = null)
        {
            return JResult.Error(content, extra);
        }

        protected IActionResult JOk(object content, object extra = null)
        {
            return JResult.Ok(content, extra);
        }

        protected IActionResult JError(string content, object extra = null)
        {
            return JResult.Error(content, extra);
        }

        protected IActionResult JOk(string content, object extra = null)
        {
            return JResult.Ok(content, extra);
        }

        protected IActionResult JOk()
        {
            return JResult.Ok(null);
        }

        protected IActionResult CreateFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["created_error"] + ":" + appendMsg, extra);
        }

        protected IActionResult CreateSuccess(object extra = null)
        {
            return JResult.Ok(T["created_success"], extra);
        }

        protected IActionResult UpdateFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["updated_error"] + ":" + appendMsg, extra);
        }

        protected IActionResult UpdateSuccess(object extra = null)
        {

            return JResult.Ok(T["updated_success"] + "", extra);
        }

        protected IActionResult DeleteFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["deleted_error"] + ":" + appendMsg, extra);
        }

        protected IActionResult DeleteSuccess(object extra = null)
        {
            return JResult.Ok(T["deleted_success"], extra);
        }

        protected IActionResult SaveFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["saved_error"] + ":" + appendMsg, extra);
        }

        protected IActionResult SaveSuccess(object extra = null)
        {
            return JResult.Ok(T["saved_success"], extra);
        }

        protected IActionResult NotSpecifiedRecord(object extra = null)
        {
            return JResult.Error(T["notspecified_record"], extra);
        }

        protected IActionResult JModelError(string title = "")
        {
            return JResult.Error((title.IsNotEmpty() ? title + ": " : "") + GetModelErrors());
        }

        /// <summary>
        /// 提示记录不存在
        /// </summary>
        /// <returns></returns>
        protected new IActionResult NotFound()
        {
            return JResult.NotFound(T);
        }

        /// <summary>
        /// 提示没有权限
        /// </summary>
        /// <returns></returns>
        protected new IActionResult Unauthorized()
        {
            return JResult.Unauthorized(T);
        }

        #endregion 常用返回信息
    }
}