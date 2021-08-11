using Microsoft.AspNetCore.Mvc;
using Mzg.Business.Filter;
using Mzg.Core.Context;
using Mzg.Infrastructure.Utility;
using Mzg.Solution.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 拦截规则接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class FilterRuleController : ApiControllerBase
    {
        private readonly IFilterRuleFinder _filterRuleFinder;

        public FilterRuleController(IWebAppContext appContext
            , IFilterRuleFinder filterRuleFinder)
            : base(appContext)
        {
            _filterRuleFinder = filterRuleFinder;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _filterRuleFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.FilterRuleId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = FilterRuleDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }
}