using Microsoft.EntityFrameworkCore;
using MeetCoVideoStreamingWebApi.Data;
using MeetCoVideoStreamingWebApi.Helpers;
using MeetCoVideoStreamingWebApi.Interfaces;
using MeetCoVideoStreamingWebApi.Services;
using MeetCoVideoStreamingWebApi.SignalRHub;

namespace MeetCoVideoStreamingWebApi.ApplicationExtentions
{
    public static class ServicesExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddSingleton<PresenceTracker>();
            services.AddSingleton<UserShareScreenTracker>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<LogUserActivity>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext<DataContext>(options =>
            {
                //Install-Package Microsoft.EntityFrameworkCore.SqlServer || options.UseSqlServer
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}
