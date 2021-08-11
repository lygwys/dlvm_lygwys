using Mzg.Dependency.Abstractions;
using System;
using System.Collections.Generic;

namespace Mzg.Dependency
{
    /// <summary>
    /// 依赖项查找工厂
    /// </summary>
    public interface IDependencyLookupFactory
    {
        List<DependentDescriptor> GetDependents<TRequired>(int requiredComponentType, Guid requiredId);

        List<DependentDescriptor> GetDependents(int requiredComponentType, Guid requiredId);
    }
}