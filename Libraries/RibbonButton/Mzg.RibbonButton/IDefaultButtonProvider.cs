using Mzg.Schema.Abstractions;
using System.Collections.Generic;

namespace Mzg.RibbonButton
{
    public interface IDefaultButtonProvider
    {
        List<Domain.RibbonButton> Get(EntityMaskEnum entityMask);
    }
}