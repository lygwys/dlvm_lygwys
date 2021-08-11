using Mzg.Business.DataAnalyse.Domain;
using Mzg.Sdk.Abstractions.Query;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartBuilder
    {
        ChartContext Build(QueryView.Domain.QueryView queryView, Chart chartEntity, FilterExpression filter = null, string drillGroup = "");
    }
}