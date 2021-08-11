using Mzg.Core.Data;
using Mzg.Form.Abstractions;
using System.Collections.Generic;

namespace Mzg.RibbonButton
{
    public interface IRibbonButtonStatusSetter
    {
        void Set(List<Domain.RibbonButton> buttons, FormState? formState = null, Entity record = null);
    }
}