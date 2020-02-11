using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lista_de_compras_core.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Lista_de_compras_core
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
            //permite que n�o tenhamos problema com o CORS(cross-origin resource sharing),
            //se quisermos fazer requisi��o atraves do localhost, sme o cors nao vai funcionar
            services.AddCors();

            //vai comprimir(zip) nosso json para mandar para a tela, e dps o html consegue descompactar
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/json" });

            });

            //vai adicionar por padrao um cabe�alho de caching
            //services.AddResponseCaching(); 

            services.AddControllers();

            //transforma a chave(Secret) para bytes
            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            //adicionar uma autentica��o
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



            //preciso informar para a minha aplica��o que tenho uma datacontext chamado DataContext
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database")); //opt => opt.UseInMemoryDatabase("Database") -> tem essa op��o para mostrarmos qual banco estamos usando
            //o .AddDbContext ja faz a fun��o do .AddScoped(services.AddScoped<DataContext, DataContext>();)
            //se eu usar o sql server usar esse de baixo e comentar o de cima
            /*services.AddDbContext<DataContext>(
                opt => opt.UseSqlServer(Configuration
                .GetConnectionString("connectionString")));
            */

           //� necessario tornar essa datacontext disponivel para os nosso controllers(s�o dependentes do datacontext)
           //No modelo api cada requisi��o que � feita na api(ela � executada e dps o usu�rio � desligado da api) pelo usu�rio, a api faz uma requisi��o ao banco, para fornecer os dados, ap�s isso ele tem que fechar essa execu��o com o banco e rotornar para a api, essa api tem q retornar para o usu�rio e fechar a conex�o, pois o banco tem um limite de conex�es
           //para o banco n�o cair � necessario fechar a conex�o ap�s a execu��o  
           /*
            * As dependesias do APIs.netCore s�o tratadas de algumas formas:
           services.AddTransient  -> cada vez que � dado um dadocontext, ele me da um dadocontext novo criando desse modo uma outra conex�o com o banco de dados

           services.AddSingleton  -> ele cria(uma vez somente, se por algum motivo o banco fechar n�o ir� reabrir) uma instansia do datacontext por aplica��o, apos isso ele utiliza o mesmo datacontext para todas as requisi��o independentimente se houver outro usuario

           services.AddScoped  -> so cria uma vez, e utiliza sempre esse. Esse comando abaixo so permite uma conex�o aberta, a partir do momento que a minha requisi��o acaba esse datacontext criado na memoria � excluido
                                   nesse caso se houver outro usuario ele troca de conex�o
            */

           //ferramentas para documenta��o da api
           services.AddSwaggerGen(c =>
           {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "lista Api", Version = "v1" });
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

            app.UseSwagger(); //permite ja termos uma especifica��o para essa api, no formato json
            app.UseSwaggerUI(c =>   //ferramenta visual para testar
            {
                
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "lista Api V1");
            });

            app.UseRouting();

            //pode-se assim fazer chamadas no localhost
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication(); //diz quem o usuario �
            app.UseAuthorization();  //diz quem o usuario pode fazer

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
