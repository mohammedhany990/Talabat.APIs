using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Contexts;
using Talabat.Repository.Data;
using Talabat.Repository.IdentityContexts;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Configure Sevice
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // To Allow DI
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            //To Allow DI for IConnectionMultiplexer in BasketReop class : In memory DB
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection"); 
                return ConnectionMultiplexer.Connect(Connection);                
            });

            builder.Services.AddScoped<IBasketRepository, BasketReository>();
            

            // Extension Method
            builder.Services.AddApplicationServices();


            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppIdentityDbContext>();
           
            builder.Services.AddAuthentication();
            #endregion


            var app = builder.Build();


            #region Update database
            // Group of Services LifeTime Scopped
            using var Scope = app.Services.CreateScope();
            // Hold the Services
            var Services = Scope.ServiceProvider;
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                // Hold DbContext -> StoreContext
                // Ask CLR for Creating object from DbContext Explicitly
                var dbContext = Services.GetRequiredService<StoreContext>();
                await dbContext.Database.MigrateAsync();

                var IdentitydbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentitydbContext.Database.MigrateAsync();
                var usermanager = Services.GetRequiredService<UserManager<AppUser>>();

                await AppIdentityDbContextSeed.UserSeedAsync(usermanager);

                await StoreContextSeed.DataSeed(dbContext);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An Error while applying Migration");
            }
            #endregion




            #region Configure Http request pipeline 
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}