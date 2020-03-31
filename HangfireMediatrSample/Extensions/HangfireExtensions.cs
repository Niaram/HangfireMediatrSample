using Hangfire.Common;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire
{
    public static class HangfireExtensions
    {
        public static IGlobalConfiguration UseMediatR(this IGlobalConfiguration config, IMediator mediator)
        {
            config.UseActivator(new MediatRJobActivator(mediator));

            GlobalConfiguration.Configuration.UseSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            });

            return config;
        }
    }

    public class MediatRJobActivator : JobActivator
    {
        private readonly IMediator _mediator;

        public MediatRJobActivator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override object ActivateJob(Type type)
        {
            return new HangfireMediator(_mediator);
        }
    }
}
