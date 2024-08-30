using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helper;
using Talabat.APIs.MIddlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Repository.Identity.DataSeed;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure services

            builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();


            //Allow DI For TalabatDbContext
            builder.Services.AddDbContext<TalabatDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );


            //Allow DI For AppUserDbContext
            builder.Services.AddDbContext<AppUserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
            );

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddTransient<IEmailSettings, EmailSettings>();

            


            //Extension Methods
            builder.Services.AddIdentityService(builder.Configuration);
            builder.Services.AddAppServices();

            builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var connection = builder.Configuration.GetConnectionString("redis");
                return ConnectionMultiplexer.Connect(connection);
            });



            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("Policy", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins(builder.Configuration["FrontendBaseUrl"]);
                });
            });

            #endregion


            var app = builder.Build();


            #region Update Database

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<TalabatDbContext>();
            var _IdentitydbContext = services.GetRequiredService<AppUserDbContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeeding.DataSeedAsync(_dbContext);

                await _IdentitydbContext.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppUserDbContextSeed.AddAppUserDbContextSeed(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "an Error has been occured during applying Migrations");
            }
            #endregion


 #region Cingigure Kestrel [Middlewares]
            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("Policy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            #endregion

        }
    }
}
