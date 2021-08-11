﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mzg.Core;
using Mzg.Infrastructure.Inject;

namespace Mzg.Event
{
    /// <summary>
    /// 事件模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            //event publisher
            services.AddScoped<Event.Abstractions.IEventPublisher, Event.EventPublisher>();
            //event consumers
            services.RegisterScope(typeof(Event.Abstractions.IConsumer<>));
        }
    }
}