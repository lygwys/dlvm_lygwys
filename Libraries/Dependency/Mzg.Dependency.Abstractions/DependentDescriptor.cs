using System;

namespace Mzg.Dependency.Abstractions
{
    public class DependentDescriptor
    {
        public Guid DependentId { get; set; }
        public string Name { get; set; }
        public int ComponentType { get; set; }
    }
}