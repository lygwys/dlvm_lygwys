﻿using Mzg.Core.Context;
using Mzg.Plugin.Abstractions;
using Mzg.Plugin.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.Plugin
{
    public interface IEntityPluginFinder
    {
        EntityPlugin FindById(Guid id);

        List<EntityPlugin> FindAll();

        List<EntityPlugin> Query(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container);

        List<EntityPlugin> QueryByEntityId(Guid entityid, string eventName, Guid? businessObjectId = null, PlugInType typeCode = PlugInType.Entity);

        PagedList<EntityPlugin> QueryPaged(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container);

        PagedList<EntityPlugin> QueryPaged(Func<QueryDescriptor<EntityPlugin>, QueryDescriptor<EntityPlugin>> container, Guid solutionId, bool existInSolution);
    }
}