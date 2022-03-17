using Api.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Infrastructure.DBContexts;
using MandradeFrameworks.Repositorios.Configuration;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string NOME_POLITICA_CORS = "Autorizacao Front-end";
        private readonly string[] ORIGENS_CORS = new string[] { "http://localhost:4200", "https://localhost:4200" };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options => {
                options.AddPolicy(
                    NOME_POLITICA_CORS,
                    builder => builder
                        .WithOrigins(ORIGENS_CORS)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            });

            services.AdicionarPacotesFramework(Configuration);
            services.AdicionarMiddlewares(Configuration);
            services.AdicionarRepositorios();

            services.AddDbContextSqlServer<LivrosContext>(Configuration, "Livros");

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddMvc(options => options.AdicionarPacotesFramework());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string>() { "index.html" } });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.AdicionarMiddlewaresAplicacao();
        }
    }
}
