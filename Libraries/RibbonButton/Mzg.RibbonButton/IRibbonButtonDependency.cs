using System;

namespace Mzg.RibbonButton
{
    public interface IRibbonButtonDependency
    {
        bool Create(params Domain.RibbonButton[] entities);

        bool Delete(params Guid[] id);

        bool Update(Domain.RibbonButton entity);
    }
}