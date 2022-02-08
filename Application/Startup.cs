using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using Interfaces;
using Lib;

using Demo.Classes;
using Demo.Models;
using Demo.Services;

namespace Demo
{
    public class Startup
    {
        public AppSettings AppSettings { get; } = new AppSettings();
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                // must be version 3.1.13 -- version 5's support .NET 5 only.
                // https://anthonygiretti.com/2020/05/10/why-model-binding-to-jobject-from-a-request-doesnt-work-anymore-in-asp-net-core-3-1-and-whats-the-alternative/
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var connection = Configuration.GetConnectionString(AppSettings.UseDatabase);

            services.AddDbContext<IAppDbContext, AppDbContext>(options => options.UseSqlServer(connection));

            services
                .AddAuthentication("tokenAuth")
                .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationService>("tokenAuth", ops => { });

            services
                .AddAuthorization(options => options.AddPolicy(Constants.ENTITY_AUTHORIZATION_SCHEME, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new UserHasEntityPermission());
                }));

            services.AddScoped<IAuthorizationHandler, EntityAuthenticationService>();

            services.LoadPlugins(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Do not halt execution.  I don't fully understand this.
                // See http://netitude.bc3tech.net/2017/07/31/using-middleware-to-trap-exceptions-in-asp-net-core/
                // "Notice the difference in order when in development mode vs not. This is important as the Developer Exception page
                // passes through the exception to our handler so in order to get the best of both worlds, you want the Developer Page handler first.
                // In production, however, since the default Exception Page halts execution, we definitely to not want that one first."
                app.UseDeveloperExceptionPage();
                app.UseHttpStatusCodeExceptionMiddleware();
            }
            else
            {
                app.UseHttpStatusCodeExceptionMiddleware();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/demo/swagger/v1/swagger.json", "Demo API V1");
                });

            app
                .UseAuthentication()
                .UseRouting()
                .UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
