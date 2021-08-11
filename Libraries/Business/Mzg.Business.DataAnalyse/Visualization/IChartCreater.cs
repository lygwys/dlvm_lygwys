using Mzg.Business.DataAnalyse.Domain;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartCreater
    {
        bool Create(Chart entity);
    }
}