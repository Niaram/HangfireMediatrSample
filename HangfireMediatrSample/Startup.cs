using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HangfireMediatrSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Add MediatR
            services.AddMediatR(config => config.AsTransient(),
                                Assembly.GetExecutingAssembly());

            var sp = services.BuildServiceProvider(); // => How to remove this?
            var mediatr = sp.GetService<IMediator>();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"),
                                    new SqlServerStorageOptions
                                    {
                                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                        QueuePollInterval = TimeSpan.Zero,
                                        UseRecommendedIsolationLevel = true,
                                        UsePageLocksOnDequeue = true,
                                        DisableGlobalLocks = true
                                    })
                .UseMediatR(mediatr));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
