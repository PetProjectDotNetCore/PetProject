using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetProject.Web.API.Configurations.Middlewares;
using PetProject.Web.API.Configurations.Settings;
using PetProject.Web.API.Data;
using PetProject.Web.API.Interfaces;
using PetProject.Web.API.Services;

namespace PetProject.Web.API
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
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options => options.AddPolicy("local-angular", builder => builder
				.WithOrigins("http://localhost:4200")
				.WithMethods("POST")
				.AllowAnyHeader()
				.AllowCredentials()));

            services.AddControllers();

            var jwtSection = Configuration.GetSection("Jwt");
            services.Configure<JwtSettings>(jwtSection);

            services.AddTokenAuthentication(Configuration);
            ConfigureDependencies(services);
        }

        private static void ConfigureDependencies(IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IJwtService, JwtService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
				app.UseCors("local-angular");
			}

			dataContext.Database.Migrate();

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
