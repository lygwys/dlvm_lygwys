using Mzg.Core.Context;
using Mzg.DataMapping.Domain;
using System;

namespace Mzg.DataMapping
{
    public interface IAttributeMapUpdater
    {
        bool Update(Func<UpdateContext<AttributeMap>, UpdateContext<AttributeMap>> context);

        bool Update(AttributeMap entity);
    }
}