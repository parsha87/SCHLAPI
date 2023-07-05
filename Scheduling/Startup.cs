using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Buffers;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Serialization;

namespace Scheduling
{
    public class Startup
    {
        public static string ConnectionString { get; private set; }
        private SymmetricSecurityKey _signingKey;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            var secretKey = Configuration.GetValue<string>("AuthSecretKey");
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
           .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<MainDBContext>(ServiceLifetime.Transient);

            services.AddDbContext<EventDBContext>(ServiceLifetime.Transient);

            services.AddDbContext<TimestampDBContext>(ServiceLifetime.Transient);

            services.AddDbContext<ApplicationDbContext>();

            services.AddDbContext<GlobalDBContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionGlobal")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            // options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<MainDBContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<TimestampDBContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionTimeStamp")));

            //services.AddDbContext<EventDBContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionEvents")));


            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IUserService, UsersService>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IZoneTimeService, ZoneTimeService>();
            services.AddTransient<ISequenceService, SequenceService>();
            services.AddTransient<IZoneService, ZoneService>();
            services.AddTransient<INetworkService, NetworkService>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IValveService, ValveService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ISequenceDatetimeService, SequenceDatetimeService>();
            services.AddTransient<ISubBlockService, SubBlockService>();
            services.AddTransient<IManualOverrideService, ManualOverrideService>();
            services.AddTransient<ISubBlockService, SubBlockService>();
            services.AddTransient<IFilterService, FilterService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IEventLogService, EventLogService>();
            services.AddTransient<INodeService, NodeService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IFramesService, FramesService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IResponseCacheService, ResponseCacheService>();

            //PA-For JSON case
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });


            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {

                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.ValidFor = TimeSpan.FromMinutes(Convert.ToDouble(jwtAppSettingOptions[nameof(JwtIssuerOptions.ValidFor)]));
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // add identity
            var builder = services.AddIdentityCore<ApplicationUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddRoles<IdentityRole>();
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //services.AddCors(options =>
            //{
            //    var config = Configuration.GetSection("AllowSpecificOrigin").Get<OriginsModel>();
            //    string[] origins = config.Origins.Split(',');
            //    options.AddPolicy("AllowSpecificOrigin",
            //        builder1 => builder1.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader());
            //});
            services.AddCors(c =>
            {
                c.AddPolicy("AllowSpecificOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JISL API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                         new OpenApiSecurityScheme
                         {
                             Reference = new OpenApiReference
                               {
                                 Type = ReferenceType.SecurityScheme,
                                 Id = "Bearer"
                               }
                          },
                          new string[] { }
                        }
                      });
            });

            //// Register the Swagger generator, defining 1 or more Swagger documents
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JISL API", Version = "v1" });
            //});

            //Commented on PA
            //services.AddMvc(o =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //    o.Filters.Add(new AuthorizeFilter(policy));
            //});

            //Add permission, authorization handler and provider
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddAutoMapper(typeof(Startup));

            //https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
            services.AddControllers()
                  //Configure API Behaviour Options for invalid model state.
                  .ConfigureApiBehaviorOptions(options =>
                  {
                      options.InvalidModelStateResponseFactory = context =>
                      {
                          var problems = new CustomBadRequest(context.ModelState);
                          return new BadRequestObjectResult(problems);
                      };
                  });

            //Add default authorization policy to have authenticated user
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
            });
            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = null;


            //});
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize);
            //services.AddControllers().AddJsonOptions(options => {
            //    options.JsonSerializerOptions.IgnoreNullValues = true;
            //});
            //services.AddControllers();
            //services.AddMvc(options =>
            //{
            //    options.OutputFormatters.Clear();
            //    options.OutputFormatters.Add(new JsonOutputFormatter(new JsonSerializerSettings()
            //    {
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    }, ArrayPool<char>.Shared));
            //});
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                o.JsonSerializerOptions.IgnoreNullValues = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JISL API");
                c.RoutePrefix = "swagger/ui";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddLog4Net();

            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            // Enable serving files from the configured web root folder (i.e., WWWROOT)
            app.UseStaticFiles();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
