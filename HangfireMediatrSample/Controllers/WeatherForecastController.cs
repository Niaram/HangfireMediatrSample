using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangfireMediatrSample.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HangfireMediatrSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMediator mediator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        [HttpGet]
        public string Get()
        {
            mediator.Enqueue(new SomethingHappenedEvent());
            return "OK, Enqueued!";
        }
    }
}
