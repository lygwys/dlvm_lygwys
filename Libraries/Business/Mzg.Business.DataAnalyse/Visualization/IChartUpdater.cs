using Mzg.Business.DataAnalyse.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Business.DataAnalyse.Visualization
{
    public interface IChartUpdater
    {
        bool Update(Chart entity);

        bool UpdateState(IEnumerable<Guid> ids, bool isEnabled);
    }
}