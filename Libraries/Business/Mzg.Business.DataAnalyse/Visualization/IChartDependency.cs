using Mzg.Business.DataAnalyse.Domain;
using System;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartDependency
    {
        bool Create(Chart entity);

        bool Delete(params Guid[] id);

        bool Update(Chart entity);
    }
}