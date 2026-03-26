using System.Text;
using HMS.API.Extensions;
using HMS.Core.Contracts;
using HMS.Core.Entities.IdentityEntities;
using HMS.Infrastructure.Data.DataSeed;
using HMS.Infrastructure.Data.DbContexts;
using HMS.Infrastructure.ExternalServices;
using HMS.Infrastructure.Repositories;
using HMS.Services.Abstraction;
using HMS.Services.Helpers;
using HMS.Services.Profiles;
using HMS.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService = HMS.Services.Services.AuthenticationService;
using IAuthenticationService = HMS.Services.Abstraction.IAuthenticationService;

namespace HMS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<HotelDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(ProfilesAssemblyReference).Assembly);
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddTransient<IAttachementService, AttachementService>();
            builder
                .Services.AddIdentityCore<HotelUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HotelDbContext>();
            builder.Services.AddScoped<IDataIntializer, IdentityDataIntializer>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                        ValidAudience = builder.Configuration["JWTOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)
                        ),
                    };
                });

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings")
            );
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.Configure<PayMobSettings>(builder.Configuration.GetSection("PayMob"));
            builder.Services.AddHttpClient<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            var app = builder.Build();

            await app.MigrateDatabaseAsync();
            await app.SeedIdentityDataAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
