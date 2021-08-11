using System;

namespace Mzg.Module.Abstractions
{
    public interface IModule
    {
        //string Name { get; }

        Action<ModuleDescriptor> Configure();

        void OnStarting();
    }
}