using Mzg.Business.DataAnalyse.Data;
using Mzg.Business.DataAnalyse.Domain;
using Mzg.Context;
using Mzg.Infrastructure.Utility;
using Mzg.Localization;

namespace Mzg.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表创建服务
    /// </summary>
    public class ChartCreater : IChartCreater
    {
        private readonly IChartRepository _chartRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;
        private readonly IChartDependency _dependencyService;

        public ChartCreater(IAppContext appContext
            , IChartRepository chartRepository
            , ILocalizedLabelService localizedLabelService
            , IChartDependency dependencyService)
        {
            _appContext = appContext;
            _chartRepository = chartRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
        }

        public bool Create(Chart entity)
        {
            var result = _chartRepository.Create(entity);
            if (result)
            {
                //依赖于字段
                _dependencyService.Create(entity);
                //本地化标签
                _localizedLabelService.Create(entity.SolutionId, entity.Name.IfEmpty(""), ChartDefaults.ModuleName, "LocalizedName", entity.ChartId, _appContext.BaseLanguage);
                _localizedLabelService.Create(entity.SolutionId, entity.Description.IfEmpty(""), ChartDefaults.ModuleName, "Description", entity.ChartId, _appContext.BaseLanguage);
            }
            return result;
        }
    }
}