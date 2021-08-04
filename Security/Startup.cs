using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InnovationTest.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogProbApp.Crypto;

namespace InnovationTest
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.Cookie.Name = "InnovationTest";
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });
            services.AddRazorPages(options =>
            {
                //options.Conventions.AuthorizePage("/Contact", "AtLeast21")
                options.Conventions.AuthorizeFolder("/Universities", "RequireAdministratorRole");
                options.Conventions.AuthorizeFolder("/Testing", "RequireUserRole");
                options.Conventions.AuthorizeFolder("/AddTests", "RequireTestAdminControllerTeacherRoles");
                options.Conventions.AuthorizeFolder("/Reports", "RequireTestAdminControllerRoles");

            });
            // .AddSessionStateTempDataProvider();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTestAdminControllerTeacherRoles",
                policy => policy.RequireRole("testAdmin", "testController", "testTeacher"));

                options.AddPolicy("RequireTestAdminControllerRoles",
                 policy => policy.RequireRole("testAdmin", "testController"));


                options.AddPolicy("RequireTestAdminTeacherRoles",
                 policy => policy.RequireRole("testAdmin","testTeacher"));


                options.AddPolicy("RequireAdministratorRole",
                 policy => policy.RequireRole("testAdmin"));

                options.AddPolicy("RequireUserRole",
                 policy => policy.RequireRole("testUser"));

                options.AddPolicy("RequireTestTeacherRole",
                policy => policy.RequireRole("testTeacher"));


                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                });

            string ensc = Configuration.GetConnectionString("InnovationTestContext");

            AesCryptoUtil aes = new AesCryptoUtil();

            string connectionStr = aes.Decrypt(ensc);

            services.AddDbContext<InnovationTestContext>(options =>
                    options.UseSqlServer(connectionStr));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               // app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });

            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseCookiePolicy();
            app.UseSession();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
