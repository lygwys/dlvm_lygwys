﻿using Mzg.Module.Abstractions;
using System;

namespace Mzg.Dependency
{
    /// <summary>
    /// 模块描述
    /// </summary>
    public class ModuleEntry : IModule
    {
        public string Name
        {
            get
            {
                return "Dependency";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 20;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}