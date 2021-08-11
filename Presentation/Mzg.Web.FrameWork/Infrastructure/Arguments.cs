using Mzg.Infrastructure.Utility;
using System;

namespace Mzg.Web.Framework.Infrastructure
{
    /// <summary>
    /// 判断参数值是否为空可以是多个值，如果其中一个值为空则返回false
    /// xmg
    /// 202011292041
    /// </summary>
    public static class Arguments
    {
        public static bool HasValue(params Guid[] value)
        {
            if (value == null || value.Length == 0)
            {
                return false;
            }
            var result = true;
            foreach (var item in value)
            {
                if (item.IsEmpty())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}