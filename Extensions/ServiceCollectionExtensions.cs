using BackEnd.Common;
using BackEnd.Repositories.ChatRepository;
using BackEnd.Repositories.FriendsRepository;
using BackEnd.Repositories.MessagesRepository;
using BackEnd.Repositories.UserRepository;
using BackEnd.Services.Auth;
using BackEnd.Services.ChatService;
using BackEnd.Services.CommonService;
using BackEnd.Services.FrinedsService;
using BackEnd.Services.GoogleService;
using BackEnd.Services.ImageService;
using BackEnd.Services.MessagesService;
using BackEnd.Services.UserServices;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BackEnd.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 註冊服務與資料庫上下文
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFriendsService, FriendsService>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMessagesService, MessagesService>();
            services.AddScoped<IMessagesRepository, MessagesRepository>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var account = new Account(
                    config["Cloudinary:CloudName"],
                    config["Cloudinary:ApiKey"],
                    config["Cloudinary:ApiSecret"]);
                return new Cloudinary(account);
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins(
                            "http://127.0.0.1:5500",
                            "https://chatroom-frontend-uc36.onrender.com"

                        )
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            return services;
        }


        public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration config)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:ConnectionString"] ?? "localhost:6379";
            });

            return services;
        }


        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,                
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,     

                    ValidIssuer = jwtSettings["Issuer"],    
                    ValidAudience = jwtSettings["Audience"],  
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = JsonConvert.SerializeObject(new
                        {
                            Code = Constants.UNAUTHORIZED,
                            Message = Constants.UNAUTHORIZED_MESSAGE
                        });

                        return context.Response.WriteAsync(result);
                    },

                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    },
                };

            });

            return services;
        }

        public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var googleSettings = configuration.GetSection("Google");

            services.AddAuthentication(options =>
            {

                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = googleSettings["ClientId"];
                options.ClientSecret = googleSettings["ClientSecret"];
                options.CallbackPath = "/signin-google";
            });

            return services;
        }
    }
}
