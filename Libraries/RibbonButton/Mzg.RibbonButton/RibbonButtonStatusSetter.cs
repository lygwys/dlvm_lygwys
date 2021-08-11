using Mzg.Core.Data;
using Mzg.Form.Abstractions;
using System.Collections.Generic;

namespace Mzg.RibbonButton
{
    /// <summary>
    /// 按钮状态设置
    /// </summary>
    public class RibbonButtonStatusSetter : IRibbonButtonStatusSetter
    {
        public RibbonButtonStatusSetter()
        {
        }

        public void Set(List<Domain.RibbonButton> buttons, FormState? formState = null, Entity record = null)
        {
            foreach (var item in buttons)
            {
                item.SetButtonStatus(formState, record);
            }
        }
    }
}