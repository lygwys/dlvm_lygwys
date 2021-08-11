using Mzg.Core.Data;
using System;
using System.Collections.Generic;

namespace Mzg.Business.FormStateRule
{
    public interface ISystemFormStatusSetter
    {
        bool IsDisabled(List<Guid> rulesId, Entity data);
    }
}