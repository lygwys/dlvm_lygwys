﻿using Mzg.Module.Abstractions;
using System;

namespace Mzg.Logging.AppLog
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
                return "AppLog";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 23;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}