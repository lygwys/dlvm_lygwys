﻿using Mzg.DataMapping.Data;
using Mzg.DataMapping.Domain;
using System.Collections.Generic;

namespace Mzg.DataMapping
{
    /// <summary>
    /// 实体字段映射创建服务
    /// </summary>
    public class AttributeMapCreater : IAttributeMapCreater
    {
        private readonly IAttributeMapRepository _attributeMapRepository;

        public AttributeMapCreater(IAttributeMapRepository attributeMapRepository)
        {
            _attributeMapRepository = attributeMapRepository;
        }

        public bool Create(AttributeMap entity)
        {
            var flag = _attributeMapRepository.Create(entity);
            return flag;
        }

        public bool CreateMany(List<AttributeMap> entities)
        {
            return _attributeMapRepository.CreateMany(entities);
        }
    }
}