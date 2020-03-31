using HangfireMediatrSample.Messages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireMediatrSample.Handlers
{
    public class SomethingHappenedHandler : IRequestHandler<SomethingHappenedEvent, Unit>
    {
        public Task<Unit> Handle(SomethingHappenedEvent request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Something happened event received!");
            return Unit.Task;
        }
    }
}
