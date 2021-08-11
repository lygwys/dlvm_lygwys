﻿using Mzg.Module.Abstractions;
using System;

namespace Mzg.File
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
                return "Attachment";
            }
        }

        public Action<ModuleDescriptor> Configure()
        {
            return (o) =>
            {
                o.Identity = 21;
                o.Name = this.Name;
            };
        }

        public void OnStarting()
        {
        }
    }
}