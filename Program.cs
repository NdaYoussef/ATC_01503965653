
using CloudinaryDotNet;
using EventManagmentTask.Data;
using EventManagmentTask.Interfaces;
using EventManagmentTask.Mapping;
using EventManagmentTask.Models;
using EventManagmentTask.Repositories;
using EventManagmentTask.Services;
using EventManagmentTask.UploadSetiings;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Principal;
using System.Text;

namespace EventManagmentTask
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

            #region Connection String
            builder.Services.AddDbContext<EventManagmentDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            #endregion

            #region Add identity
            builder.Services.AddIdentity<User, IdentityRole>()
                             .AddEntityFrameworkStores<EventManagmentDbContext>()
                             .AddDefaultTokenProviders(); 
            #endregion

            #region Add Authentication 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(op =>
            {
                op.SaveToken = true;
                op.RequireHttpsMetadata = false;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            }); 
            #endregion

            #region Cloudinary Settings&Dependency Injection
            builder.Services.Configure<CloudinarySettings>(
               builder.Configuration.GetSection("CloudinarySettings"));
            var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            if (cloudinarySettings == null ||
                string.IsNullOrEmpty(cloudinarySettings.CloudName) ||
                string.IsNullOrEmpty(cloudinarySettings.ApiKey) ||
                string.IsNullOrEmpty(cloudinarySettings.ApiSecret))
            {
                throw new InvalidOperationException("Cloudinary settings are not properly configured.");
            }
            var account = new Account(
            cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret
            );
            var cloudinary = new Cloudinary(account);

            builder.Services.AddSingleton(cloudinary); 
            #endregion


            #region  Inject Services 
            builder.Services.AddScoped<ICloudinaryRepository, CloudinaryService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEventRepository, EventService>();
            builder.Services.AddScoped<IAccountRepository, AccountService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryService>();
            builder.Services.AddScoped<IBookingRepository, BookingService>();
            #endregion

            #region Adding mapster 
            builder.Services.AddMapster();
            TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfig).Assembly);
            #endregion

            
            #region Add authorization headers
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme , Id = "Bearer"}
                        },
                        new string []{}
                    }
                });
            });
            #endregion

            #region Add authorization policy
            builder.Services.AddAuthorization(options =>
             {
                 options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                 options.AddPolicy("Organizer", policy => policy.RequireRole("Organizer"));
                 options.AddPolicy("CLient", policy => policy.RequireRole("CLient"));
                 options.AddPolicy("Admin and Organizer", policy => policy.RequireRole("Admin", "Organizer"));
             });

            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();

            #region seed roles in DB
            using (var scope = app.Services.CreateScope())
            {
                var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedingRoles.SeedingRolesAsync(rolemanager);
            } 
            #endregion
        }
    }
}
