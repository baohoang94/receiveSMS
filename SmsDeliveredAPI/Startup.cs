using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SmsDeliveredAPI.Middlewares;
using SmsDeliveredAPI.Models;
using SmsDeliveredAPI.Services;

namespace SmsDeliveredAPI
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
            services.AddDbContext<DbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddSingleton<IAntiDuplicateDeliveredService, AntiDuplicateDeliveredService>();

            services.AddSingleton<ISystemService, SystemService>();

            services.AddSingleton<Crypto.IRsaCrypto, Crypto.RsaCrypto>();
            
            services.AddTransient<IMessageDeliveredRepository, MessageDeliveredWaitingRepository>();

            services.AddTransient<IMessagesDeliveredService, MessagesDeliveredService>();

            services.AddControllers();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Messsage Delivered API",
                    Version = "v1",
                    Description = "API receive MO from SAMI-S"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles(); // For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/api/references"
            });

            app.UseMiddleware<ClientSafeListMiddleware>(Configuration["IPClientSafeList"]);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Delivered API v1");
            });
        }
    }
}
