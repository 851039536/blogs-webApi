using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Snblog.IRepository;
using Snblog.IService;
using Snblog.IService.IService;
using Snblog.Jwt;
using Snblog.Models;
using Snblog.Repository;
using Snblog.Service;
using Snblog.Service.Service;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Snblog
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

            // ע��Swagger����
            services.AddSwaggerGen(c =>
              {
                  // ����ĵ���Ϣ
                  c.SwaggerDoc("v1", new OpenApiInfo
                  {
                      Version = "v1",
                      Title = "SN���� API",
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
                  //var basePath2 = AppContext.BaseDirectory;// xml·��
                  //                                         // var xmlModelPath = Path.Combine(basePath2, "Snblog.Enties.xml");//Model���xml�ļ���
                  //var corePath = Path.Combine(basePath2, "Snblog.xml");//API���xml�ļ���
                  //                                                     //  c.IncludeXmlComments(xmlModelPath);
                  //c.IncludeXmlComments(corePath, true);
                  //��ӶԿ������ı�ǩ(����)

                  // ʹ�÷����ȡxml�ļ�����������ļ���·��
                  var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                  var xmlpath = Path.Combine(AppContext.BaseDirectory, xmlfile);
                  // ����xmlע��. �÷����ڶ����������ÿ�������ע�ͣ�Ĭ��Ϊfalse.
                  c.IncludeXmlComments(xmlpath, true);
                  c.CustomSchemaIds(type => type.FullName);// ���Խ����ͬ�����ᱨ�������



                  #region ����Authorization
                   //Bearer ��scheme����
                  var securityScheme = new OpenApiSecurityScheme()
                  {
                      Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
                      Name = "Authorization",
                      //���������ͷ��
                      In = ParameterLocation.Header,
                      //ʹ��Authorizeͷ��
                      Type = SecuritySchemeType.Http,
                      //����Ϊ�� bearer��ͷ
                      Scheme = "bearer",
                      BearerFormat = "JWT"
                  };

                  //�����з�������Ϊ����bearerͷ����Ϣ
                  var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearerAuth"
                                }
                            },
                            new string[] {}
                    }
                };

                  //ע�ᵽswagger��
                  c.AddSecurityDefinition("bearerAuth", securityScheme);
                  c.AddSecurityRequirement(securityRequirement);
                  #endregion
                  
                 

        });

            //ע��DbContext
            services.AddDbContext<snblogContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            //��һ�ҽ̳���ԭ������


            //����jwt
            services.ConfigureJwt(Configuration);
            //ע��JWT�����ļ�
            services.Configure<JwtConfig>(Configuration.GetSection("Authentication:JwtBearer"));

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
services.AddScoped<ISnNavigationService, SnNavigationService>();
services.AddScoped<ISnLabelsService, SnLabelsService>();
services.AddScoped<ISnSortService, SnSortService>();
services.AddScoped<ISnOneService, SnOneService>();
services.AddScoped<ISnVideoService, SnVideoService>();
services.AddScoped<ISnVideoTypeService, SnVideoTypeService>();
services.AddScoped<ISnUserTalkService, SnUserTalkService>();
services.AddScoped<ISnUserService, SnUserService>();
services.AddScoped<ISnOneTypeService, SnOneTypeService>();
services.AddScoped<ISnPictureService, SnPictureService>();
services.AddScoped<ISnPictureTypeService, SnPictureTypeService>();
services.AddScoped<ISnTalkService, SnTalkService>();
services.AddScoped<ISnTalkTypeService, SnTalkTypeService>();
services.AddScoped<ISnNavigationTypeService, SnNavigationTypeService>();
services.AddScoped<ISnleaveService, SnleaveService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    #region Swagger
    //���Խ�Swagger��UIҳ��������Configure�Ŀ�������֮��
    // ����Swagger�м��
    app.UseSwagger();
    //����SwaggerUI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SN����API");
                //������ҳΪSwagger
                c.RoutePrefix = string.Empty;
                //����Ϊnone���۵����з���
                c.DocExpansion(DocExpansion.None);
                //����Ϊ-1 �ɲ���ʾmodels
                c.DefaultModelsExpandDepth(-1);
    });
    #endregion

    app.UseHttpsRedirection();

    app.UseRouting();

    //����Cors���������м��
    app.UseCors("AllRequests");
    //jwt
    app.UseAuthentication();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
    }
}
