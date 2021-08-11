using Mzg.DataMapping.Domain;
using System.Collections.Generic;

namespace Mzg.DataMapping
{
    public interface IAttributeMapCreater
    {
        bool Create(AttributeMap entity);

        bool CreateMany(List<AttributeMap> entities);
    }
}