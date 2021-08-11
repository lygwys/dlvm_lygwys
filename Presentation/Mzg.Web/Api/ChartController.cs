using Microsoft.AspNetCore.Mvc;
using Mzg.Business.DataAnalyse.Visualization;
using Mzg.Core.Context;
using Mzg.Infrastructure.Utility;
using Mzg.Solution.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 图表接口
    /// </summary>
    [Route("{org}/api/chart")]
    public class ChartController : ApiControllerBase
    {
        private readonly IChartFinder _chartFinder;

        public ChartController(IWebAppContext appContext
            , IChartFinder chartFinder)
            : base(appContext)
        {
            _chartFinder = chartFinder;
        }

        [Description("图表")]
        public IActionResult Get(Guid entityId)
        {
            var result = _chartFinder.Query(n => n.Where(f => f.EntityId == entityId));
            return JOk(result);
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _chartFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.ChartId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = ChartDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
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