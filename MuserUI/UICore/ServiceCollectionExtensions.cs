using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tolltech.MuserUI.Authentications;
using Tolltech.MuserUI.Study;
using Tolltech.SqlEF;
using TolltechCore;

namespace Tolltech.MuserUI.UICore
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureContainer(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IQueryExecutorFactory<KeyValueHandler, KeyValue>,
                    QueryExecutorFactory<KeyValueHandler, KeyValue>>();

            IoCResolver.Resolve((x, y) => serviceCollection.AddSingleton(x, y), "Tolltech");
        }
        
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["connectionString"];

            services.AddDbContext<UserContext>(options => options.UseNpgsql(connectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddControllersWithViews();
        }
    }
}