using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoSecuritymvc
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
            services.AddControllersWithViews(option =>
            option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));



            //services.AddControllersWithViews(AntiforgeryOptions.DefaultCookiePrefix.)





            services.AddControllersWithViews();
            services.AddAntiforgery(options =>
            {
                //Setting Cookies properties
                options.FormFieldName = "AntiforgeryFieldname";
                options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
                options.SuppressXFrameOptionsHeader = false;
            });

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();



            app.UseRouting();



            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public void Configure(IApplicationBuilder app, IAntiforgery antiforgery)
        {
            app.Use(next => context =>
            {
                string path = context.Request.Path.Value;



                if (
                string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
                {
                    // The request token can be sent as a JavaScript-readable cookie,
                    // and Angular uses it by default.
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                    new CookieOptions() { HttpOnly = false });
                }



                return next(context);
            });
        }
    }
}
