﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tolltech.Muser.Domain;
using Tolltech.MuserUI.Authentications;
using Tolltech.MuserUI.Common;
using Tolltech.YandexClient;
using Tolltech.YandexClient.Authorizations;
using TolltechCore;

namespace Tolltech.MuserUI.UICore
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureContainer(this IServiceCollection serviceCollection)
        {
            //serviceCollection
            //    .AddSingleton<IQueryExecutorFactory<KeyValueHandler, KeyValue>,
            //        QueryExecutorFactory<KeyValueHandler, KeyValue>>();

            var ignoreTypes = new HashSet<Type>
            {
                typeof(IYandexMusicClient),
                typeof(IYandexCredentials),
                typeof(ISpecialTrackGetter),
            };

            IoCResolver.Resolve((x, y) => serviceCollection.AddSingleton(x, y), ignoreTypes, "Tolltech");
        }
        
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["connectionString"];

            services.AddDbContext<UserContext>(options => options.UseNpgsql(connectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/account/login");
                });
            services.AddControllersWithViews();
        }
        
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(Constants.MuserCorsPolicy,
                    builder =>
                    {
                        builder.WithOrigins("https://vk.com",
                            "https://www.shazam.com/")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }
    }
}