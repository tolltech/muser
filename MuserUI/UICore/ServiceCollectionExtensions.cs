﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tolltech.MuserUI.Authentications;
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
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddControllersWithViews();
        }
    }
}