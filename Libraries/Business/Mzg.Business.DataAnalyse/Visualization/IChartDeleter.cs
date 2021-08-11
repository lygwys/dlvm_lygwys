using System;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}