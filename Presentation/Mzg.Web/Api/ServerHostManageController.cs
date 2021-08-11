using Microsoft.AspNetCore.Mvc;
using Mzg.ServerHostManage;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 服务器主机管理
    /// </summary>
    [Route("{org}/api/serverhostmanage")]
    public class ServerHostManageController : ApiControllerBase
    {
        private readonly IServerHostManageService _serverHostManageService;

        public ServerHostManageController(IWebAppContext appContext
            , IServerHostManageService serverHostManageService
           )
            : base(appContext)
        {
            _serverHostManageService = serverHostManageService;
        }

        [Description("获取用户个性化")]
        [HttpGet("getSystemInfomation")]
        public IActionResult GetSystemInfomation()
        {
            var result = _serverHostManageService.GetSystemInfomation();
            return JOk(result);
        }
    }
}