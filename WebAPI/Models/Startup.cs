using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WebAPI.Models;
using AskMe;
using AskMe.Data.Config;
using AskMe.Data.Context;
using AskMe.Repositories;
using AskMe.Repositories.Manager;
using Microsoft.EntityFrameworkCore;
using AskMe.Logic;

namespace WebAPI
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
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" }); });

            services.Configure<CookieConfig>(Configuration.GetSection("Cookie"));

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            services.AddDbContext<AskMeContext>(
                opt => ConfigureDatabaseService(opt),
                ServiceLifetime.Scoped
            );
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));
            services.AddScoped<UserRepository>();
            services.AddScoped<RepositoryManager>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Logic Scope starts
            services.AddScoped<PostLogic>();
            services.AddScoped<CommentLogic>();
            // Logic Scope ends
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));

            app.UseRouting();

            app.UseCors("MyPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AskMeContext>();
                context.Database.EnsureCreated();
            }
        }
        public void ConfigureDatabaseService(DbContextOptionsBuilder optionsAction)
        {
            var connectionStringEnvironment = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            var connectionString = connectionStringEnvironment ?? Configuration.GetConnectionString("Default");
            optionsAction.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        }
    }
}
