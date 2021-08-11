using Mzg.Authorization.Abstractions;
using Mzg.Business.DataAnalyse.Data;
using Mzg.Business.DataAnalyse.Domain;
using Mzg.Context;
using Mzg.Core;
using Mzg.Identity;
using Mzg.Infrastructure;
using Mzg.Infrastructure.Utility;
using Mzg.Localization.Abstractions;
using Mzg.Schema.Entity;
using Mzg.Schema.Extensions;
using Mzg.Schema.OptionSet;
using Mzg.Schema.StringMap;
using Mzg.Sdk.Abstractions.Query;
using Mzg.Sdk.Client;
using Mzg.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzg.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表生成器
    /// </summary>
    public class ChartBuilder : IChartBuilder
    {
        private readonly IChartRepository _chartRepository;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;
        private readonly IStringMapFinder _stringMapFinder;
        private readonly IFetchDataService _fetchDataService;
        private readonly IEntityFinder _entityFinder;
        private readonly IRoleObjectAccessEntityPermissionService _roleObjectAccessEntityPermissionService;
        private readonly ILocalizedTextProvider _loc;
        private readonly IAppContext _appContext;
        protected readonly ICurrentUser _user;

        public ChartBuilder(IAppContext appContext
            , IChartRepository chartRepository
            , IStringMapFinder stringMapFinder
            , IOptionSetDetailFinder optionSetDetailFinder
            , IFetchDataService fetchDataService
            , IEntityFinder entityFinder
            , IRoleObjectAccessEntityPermissionService roleObjectAccessEntityPermissionService)
        {
            _appContext = appContext;
            _user = _appContext.GetFeature<ICurrentUser>();
            _loc = appContext.GetFeature<ILocalizedTextProvider>();
            _chartRepository = chartRepository;
            _optionSetDetailFinder = optionSetDetailFinder;
            _stringMapFinder = stringMapFinder;
            _fetchDataService = fetchDataService;
            _entityFinder = entityFinder;
            _roleObjectAccessEntityPermissionService = roleObjectAccessEntityPermissionService;
        }

        /// <summary>
        /// 生成图表
        /// </summary>
        /// <param name="queryId">视图记录</param>
        /// <param name="chartId">图表记录</param>
        /// <param name="filter">附加过滤条件</param>
        /// <returns></returns>
        public ChartContext Build(QueryView.Domain.QueryView queryView, Chart chartEntity, FilterExpression filter = null, string drillGroup = "")
        {
            ChartContext context = new ChartContext();
            //图表对象
            ChartDescriptor chart = new ChartDescriptor();
            chart = chart.DeserializeFromJson(chartEntity.PresentationConfig);
            chart.Title = chartEntity.Name;
            //引用视图查询数据
            _fetchDataService.GetMetaDatas(queryView.FetchConfig);
            if (filter != null)
            {
                _fetchDataService.QueryExpression.Criteria.AddFilter(filter);
            }
            BindUserEntityPermissions(_fetchDataService.QueryExpression);

            //图表数据描述
            ChartDataDescriptor chartData = new ChartDataDescriptor();
            chartData = chartData.DeserializeFromJson(chartEntity.DataConfig);
            if (drillGroup.IsNotEmpty())
            {
                var categoryChartData = chartData.Fetch.Find(n => n.Type == ChartItemType.Category);
                categoryChartData.Attribute = drillGroup;
            }
            var dataSource = _chartRepository.GetChartDataSource(chartData, _fetchDataService.QueryExpression, _fetchDataService.QueryResolver);

            chart.Legend = new List<string>();
            var categories = new List<string>();//分类标签
            var emptyStr = _loc["chart_nodata"];
            foreach (var item in chartData.Fetch)
            {
                var attr = _fetchDataService.QueryResolver.AttributeList.Find(n => n.EntityId == chartEntity.EntityId && n.Name.IsCaseInsensitiveEqual(item.Attribute));
                var name = item.Attribute;
                if (item.Type == ChartItemType.Series)
                {
                    //legend
                    chart.Legend.Add(attr.LocalizedName);
                    var seriesData = new List<string>();
                    var seriesName = name + Enum.GetName(typeof(AggregateType), item.Aggregate);
                    foreach (var d in dataSource)
                    {
                        var line = d as IDictionary<string, object>;
                        seriesData.Add(line[seriesName] != null ? line[seriesName].ToString() : "0");
                    }
                    var s = chart.Series.Find(n => n.Name.IsCaseInsensitiveEqual(item.Attribute));
                    s.Name = attr.LocalizedName;
                    s.Data = seriesData;
                }
                else if (item.Type == ChartItemType.Category)
                {
                    if (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer() || attr.TypeIsPartyList())
                    {
                        name += "name";
                    }
                    else if (attr.TypeIsPrimaryKey())
                    {
                        name = attr.Name + "name";
                    }
                    foreach (var d in dataSource)
                    {
                        var line = d as IDictionary<string, object>;
                        //选项类型，获取选项显示名称
                        if (line[name] != null && (attr.TypeIsPickList() || attr.TypeIsStatus()))
                        {
                            var oname = _optionSetDetailFinder.GetOptionName(attr.OptionSetId.Value, int.Parse(line[name].ToString()));
                            categories.Add(oname.IfEmpty(emptyStr));
                        }
                        //是否类型，获取选项显示名称
                        else if (line[name] != null && (attr.TypeIsBit() || attr.TypeIsState()))
                        {
                            var oname = _stringMapFinder.GetOptionName(attr.AttributeId, int.Parse(line[name].ToString()));
                            categories.Add(oname.IfEmpty(emptyStr));
                        }
                        else
                        {
                            categories.Add(line[name] != null ? line[name].ToString() : emptyStr);
                        }
                    }
                    chart.XAxis = new XAxis()
                    {
                        Type = AxisType.Category,
                        Data = categories
                    };
                }
                chart.SubTitle = queryView.Name;

                context.Chart = chart;
                context.ChartData = chartData;
                context.Attributes = _fetchDataService.QueryResolver.AttributeList;
                context.DataSource = dataSource;
            }
            return context;
        }

        private void BindUserEntityPermissions(QueryBase q)
        {
            //get entity permissions
            var roles = _user?.Roles?.Select(r => r.RoleId);
            if (!roles.Any())
            {
                throw new XmsException(_loc["notspecified_userroles"]);
            }
            var entities = q is QueryExpression ? (q as QueryExpression).GetAllEntityNames() : new List<string>() { q.EntityName };
            var entIds = _entityFinder.FindByNames(entities.ToArray()).Select(n => n.EntityId);
            _user.RoleObjectAccessEntityPermission = _roleObjectAccessEntityPermissionService.GetPermissions(entIds, roles, AccessRightValue.Read);
        }
    }
}