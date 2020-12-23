using System.Text;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Shop
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
            //Allow requests from same origin of the api. This way we can
            //make requests from localhost, for example
            services.AddCors();

            //Compressing the all responses that are application/json;
            //It's possible to compress images and other formats;
            //Usually it make the response smaller and faster;
            services.AddResponseCompression(options => {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"application/json"});
            });
            //services.AddResponseCaching();
            services.AddControllers();
            
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // ** Dependency Injections are always handled by AddScoped, 
            // AddTransient and AddSingleton, but the AddScoped ensures we'll
            // have only 1 DataContext per request and close the connection.
            // The AddTransient would give us a new DataContext every time instead
            // of use the one we already have in memory for the request;
            // The Singleton would create a single DataContext per application,
            // that would be bad because we have multiple users on the application; 
            // The AdDbContext already handle just like the AddScoped;

            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            
            //services.AddDbContext<DataContext>(
            //    opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Shop Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
            });

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
