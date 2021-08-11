﻿using Mzg.Module.Abstractions;
using System;

namespace Mzg.Organization
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
                return "Organization";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 29;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}