using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using MeetCoVideoStreamingWebApi.ApplicationExtentions;
using MeetCoVideoStreamingWebApi.Middleware;
using MeetCoVideoStreamingWebApi.SignalRHub;

namespace MeetCoVideoStreamingWebApi
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        readonly string MyAllowSpecificOrigins = "_MyAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_configuration);
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:4200", "http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                                  });
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //});

            services.AddIdentityServices(_configuration);
            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeetCoVideoStreamingWebApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeetCoVideoStreamingWebApi v1"));
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            //app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            //publish app
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ActiveHub>("hubs/active");
                endpoints.MapHub<ChatHub>("hubs/chathub");
                //  endpoints.MapFallbackToController("Index", "Fallback");//publish
            });
        }

    }
}
