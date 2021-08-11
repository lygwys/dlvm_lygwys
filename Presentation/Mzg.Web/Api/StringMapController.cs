using Microsoft.AspNetCore.Mvc;
using Mzg.Schema.StringMap;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 字段选项接口
    /// </summary>
    [Route("{org}/api/schema/stringmap")]
    public class StringMapController : ApiControllerBase
    {
        private readonly IStringMapFinder _stringMapFinder;

        public StringMapController(IWebAppContext appContext
            , IStringMapFinder stringMapService)
            : base(appContext)
        {
            _stringMapFinder = stringMapService;
        }

        [Description("查询字段选项")]
        [HttpGet("{attributeid}")]
        public IActionResult Get(Guid attributeId)
        {
            var result = _stringMapFinder.Query(n => n.Where(f => f.AttributeId == attributeId));
            return JOk(result);
        }
    }
}