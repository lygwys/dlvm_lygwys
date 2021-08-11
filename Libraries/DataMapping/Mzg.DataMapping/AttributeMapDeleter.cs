﻿using Mzg.Core.Data;
using Mzg.DataMapping.Data;
using Mzg.DataMapping.Domain;
using Mzg.Infrastructure;
using Mzg.Infrastructure.Utility;
using System;
using System.Linq;

namespace Mzg.DataMapping
{
    /// <summary>
    /// 实体字段映射删除服务
    /// </summary>
    public class AttributeMapDeleter : IAttributeMapDeleter, ICascadeDelete<EntityMap>
    {
        private readonly IAttributeMapRepository _attributeMapRepository;

        public AttributeMapDeleter(IAttributeMapRepository attributeMapRepository)
        {
            _attributeMapRepository = attributeMapRepository;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体映射</param>
        public void CascadeDelete(params EntityMap[] parent)
        {
            if (parent.NotEmpty())
            {
                var ids = parent.Select(f => f.EntityMapId).ToArray();
                _attributeMapRepository.DeleteMany(x => x.EntityMapId.In(ids));
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            bool result = _attributeMapRepository.DeleteMany(id);
            return result;
        }

        public bool DeleteByParentId(Guid entityMapId)
        {
            return _attributeMapRepository.DeleteMany(x => x.EntityMapId == entityMapId);
        }
    }
}