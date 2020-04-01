using Hangfire.Common;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Hangfire
{
    public static class HangfireExtensions
    {
        public static IApplicationBuilder UseHangfireMediatR(this IApplicationBuilder appBuilder, IServiceProvider serviceProvider)
        {
            GlobalConfiguration.Configuration
                                    .UseSerializerSettings(new JsonSerializerSettings
                                                            {
                                                                TypeNameHandling = TypeNameHandling.Objects,
                                                            })
                                    .UseActivator(new MediatRJobActivator(serviceProvider));

            return appBuilder;
        }
    }

    public class MediatRJobActivator : JobActivator
    {
        private readonly IServiceProvider serviceProvider;

        public MediatRJobActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            IMediator mediator = serviceProvider.GetService<IMediator>();
            return new HangfireMediator(mediator);
        }
    }
}
