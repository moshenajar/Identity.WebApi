using Identity.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Service;
using Identity.WebApi.Service;
using Identity.Domain.Helpers;
using Identity.Domain.Repositories;
using Identity.Domain.Security.Hashing;
using Identity.WebApi.Security.Hashing;
using Identity.Domain.Security.Tokens;
using Identity.WebApi.Security.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace Identity.WebApi
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                         .AddJsonFile("appsettings.json")
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            //if (env.IsDevelopment())
            //{
            //    // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            //    builder.AddUserSecrets();
            //}
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddControllers();

            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppIdentityUser, AppIdentityRole>(options =>
            {
                //1
                options.Password.RequiredLength = 8;

                //3
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;

                //4
                options.Password.RequiredUniqueChars = 1;

                options.Password.RequireNonAlphanumeric = false;


            })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddUserManager<AppUserManager>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<AppIdentityCustomPasswordPolicy>();


            // configure strongly typed settings object
            services.Configure<EmailAppSettings>(Configuration.GetSection("EmailAppSettings"));

            //services.AddTransient<UserDbContext>();
            //////////////////////////////////////////////////////
            services.AddScoped<IUserRepository, UserRepository>();
            //////////////////////////////////////////////////////
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenHandler, Service.TokenHandler>();
            //////////////////////////////////////////////////////
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            // configure DI for application services
            services.AddScoped<IEmailService, EmailService>();


            services.AddMvc();

            services.Configure<Domain.Security.Tokens.TokenOptions>(Configuration.GetSection("TokenOptions"));
            var tokenOptions = Configuration.GetSection("TokenOptions").Get<Domain.Security.Tokens.TokenOptions>();

            var signingConfigurations = new SigningConfigurations(tokenOptions.Secret);
            services.AddSingleton(signingConfigurations);



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        //IssuerSigningKey = signingConfigurations.SecurityKey,
                        ValidateIssuer = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });


            services.AddAutoMapper(this.GetType().Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}
