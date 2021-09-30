using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using MovieApi.APIBehavior;
using MovieApi.Filters;
using MovieApi.Helpers;
using MovieApi.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.UseNetTopologySuite());                
            });


            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyGlobalExceptionFilter));
                options.Filters.Add(typeof(ParseBadRequest));
            }).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);

            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton(provider => new MapperConfiguration(config =>
            {
                var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                config.AddProfile(new AutoMapperProfiles(geometryFactory));
            }).CreateMapper());

            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));


            services.AddScoped<IFileStorageService, AzureStorageService>();
            //services.AddScoped<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();

            //services.AddResponseCaching();
            //services.AddSingleton<IRepository, InMemoryRepository>();
            //services.AddTransient<MyActionFilter>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieApi", Version = "v1" });
            });

            services.AddCors(options =>
            {
                var frontendURL = Configuration.GetValue<string>("frontend_url");
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendURL)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
                });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["keyJwt"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //app.Use(async (context, next) =>
            //{
            //    using (var swapStream = new MemoryStream())
            //    {
            //        var originalResponseBody = context.Response.Body;
            //        context.Response.Body = swapStream;

            //        await next.Invoke();

            //        swapStream.Seek(0, SeekOrigin.Begin);
            //        string responseBody = new StreamReader(swapStream).ReadToEnd();
            //        swapStream.Seek(0, SeekOrigin.Begin);

            //        await swapStream.CopyToAsync(originalResponseBody);
            //        context.Response.Body = originalResponseBody;

            //        logger.LogInformation(responseBody);
            //    }
            //});

            //app.Map("/map1", (app) =>
            //{
            //    app.Run(async context =>
            //    {
            //        await context.Response.WriteAsync("short circuiting the pipeline");
            //    });
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            //app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
