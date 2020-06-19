using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetProject.Application.Configuration;
using PetProject.Application.Login;
using PetProject.Persistence;
using PetProject.Persistence.Interfaces;
using PetProject.Web.API.Configurations.Middlewares;

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
            services.AddDbContext<IDataContext, DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options => options.AddPolicy("local-angular", builder => builder
				.WithOrigins("http://localhost:4200")
				.WithMethods("POST")
				.AllowAnyHeader()
				.AllowCredentials()
				// AllowCredentials() => set Access-Control-Allow-Credentials header, which tells the browser that the server allows credentials for a cross-origin request.
				// You need it if you use "withCredentials = true" in XMLHttpRequest.
				// https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1#set-the-allowed-origins
			));

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDataContext dataContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
				app.UseCors("local-angular");
			}

			dataContext.Migrate();

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
