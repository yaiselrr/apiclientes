using APIClientes.Data;
using APIClientes.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIClientes
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
            // Agrego mi DbContext POR INYECCION DE DEPENDENCIA
            services.AddDbContext<ApplicationDbContext>(options =>
                          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //FIN DBCONTEXT

            // AGREGAR MI MAPEO POR INYECCION DE DEPENDENCIA PARA EL MAPEO DE LAS CLASES
            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // FIN MAPEO

            // AGREGAR MIS INTERFACES Y REPOSITORIOS PARA PODERLOS UTILIZAR POR INYECCION DE DEPENDENCIA
            services.AddScoped<IClientesRepository, ClienteRepository>();
            services.AddScoped<IUsersRepository, UserRepository>();
            // FIN REPOSITORIO E INTERFACES

            // AUTORIZACION PAR EL USUARIO
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                System.Text.Encoding.ASCII.GetBytes(
                                    Configuration.GetSection("AppSettings:Token").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false

                        };
                    });
            // FIN DE AUTORIZACION

            // AGREGANDO LOS FILTERS PARA EL BOTON AUTHORIZE
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Autorizacion Standar, Usar Bearer. Ejemplo \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
            });
            // FIN FILTERS

            // AGREGANDO CORS PARA EL BOTON AUTHORIZE
            services.AddCors();
            // FIN CORS

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIClientes", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIClientes v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // OBLIGATORIA ANTES DE UseAuthorization PARA QUE FUNCIONE LA AUTORIZACION EN LOS USUAIROS
            app.UseAuthentication();
            // FIN

            app.UseAuthorization();

            // AGREGAR CORS PARA QUE CUALQUIER APLICACION PUEDA USAR MI BACKEND API
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            // FIN CORS

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
