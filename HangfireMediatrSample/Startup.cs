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
            services.AddMediatR(Assembly.GetExecutingAssembly());

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
                                    }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                            IBackgroundJobClient backgroundJobs,
                            IWebHostEnvironment env,
                            IServiceProvider serviceProvider)
        {
            app.UseHangfireMediatR(serviceProvider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
