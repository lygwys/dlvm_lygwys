using Mzg.Context;
using Mzg.Core.Context;
using Mzg.Data.Provider;
using Mzg.DataMapping.Data;
using Mzg.DataMapping.Domain;
using System;

namespace Mzg.DataMapping
{
    /// <summary>
    /// 实体字段映射更新服务
    /// </summary>
    public class AttributeMapUpdater : IAttributeMapUpdater
    {
        private readonly IAttributeMapRepository _attributeMapRepository;

        public AttributeMapUpdater(IAppContext appContext
            , IAttributeMapRepository attributeMapRepository)
        {
            _attributeMapRepository = attributeMapRepository;
        }

        public bool Update(AttributeMap entity)
        {
            var flag = _attributeMapRepository.Update(entity);
            return flag;
        }

        public bool Update(Func<UpdateContext<AttributeMap>, UpdateContext<AttributeMap>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<AttributeMap>());
            return _attributeMapRepository.Update(ctx);
        }
    }
}