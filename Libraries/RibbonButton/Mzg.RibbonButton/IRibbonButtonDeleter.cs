using System;

namespace Mzg.RibbonButton
{
    public interface IRibbonButtonDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}