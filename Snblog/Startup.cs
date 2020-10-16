using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Snblog.IRepository;
using Snblog.IService;
using Snblog.Models;
using Snblog.Repository;
using Snblog.Service;

namespace Snblog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";//���������

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //ע��swagger����
            services.AddSwaggerGen(c =>
              {
                  c.SwaggerDoc("v1", new OpenApiInfo
                  {
                      Version = "v1",
                      Title = "ToDo API",
                      Description = "EFCore���ݲ��� ASP.NET Core Web API",
                      TermsOfService = new Uri("https://example.com/terms"),
                      Contact = new OpenApiContact
                      {
                          Name = "Shayne Boyer",
                          Email = string.Empty,
                          Url = new Uri("https://twitter.com/spboyer"),
                      },
                      License = new OpenApiLicense
                      {
                          Name = "Use under LICX",
                          Url = new Uri("https://example.com/license"),
                      }
                  });

                  // Ϊ Swagger ����xml�ĵ�ע��·��
                  var basePath2 = AppContext.BaseDirectory;// xml·��
                  var xmlModelPath = Path.Combine(basePath2, "Snblog.Enties.xml");//Model���xml�ļ���
                  var corePath = Path.Combine(basePath2, "Snblog.xml");//API���xml�ļ���
                  c.IncludeXmlComments(xmlModelPath);
                  c.IncludeXmlComments(corePath, true);
                  //��ӶԿ������ı�ǩ(����)
                  c.CustomSchemaIds(type => type.FullName);// ���Խ����ͬ�����ᱨ�������

                  // c.OperationFilter<AddResponseHeadersFilter>();
                  // c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                  //
                  // c.OperationFilter<SecurityRequirementsOperationFilter>();

              });

            //ע��DbContext
            services.AddDbContext<snblogContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            //��һ�ҽ̳���ԭ������

            #region Cors��������
            services.AddCors(c =>
            {
                c.AddPolicy("AllRequests", policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

                });
            });
            #endregion
            services.AddControllers();

            //DI����ע�����á�

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();//���͹���
            services.AddScoped<IconcardContext, snblogContext>();//db
            services.AddScoped<ISnArticleService, SnArticleService>();//ioc
            services.AddScoped<ISnNavigationService, SnNavigationService>();//ioc
            services.AddScoped<ISnLabelsService, SnLabelsService>();//ioc
            services.AddScoped<ISnSortService, SnSortService>();//ioc
            services.AddScoped<ISnOneService, SnOneService>();//ioc
            services.AddScoped<ISnVideoService, SnVideoService>();//ioc
            services.AddScoped<ISnVideoTypeService, SnVideoTypeService>();//ioc
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();


                #region Swagger
                //���Խ�Swagger��UIҳ��������Configure�Ŀ�������֮��
                app.UseSwagger();
                //��Swagger UI
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty;
                });
                #endregion
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //����Cors���������м��
            app.UseCors("AllRequests");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
