using System;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// �汾����
        /// </summary>
        public enum ApiVersion
        {
            /// <summary>
            /// v1�汾
            /// </summary>
            V1 = 1,
            /// <summary>
            /// v2�汾
            /// </summary>
            V2 = 2
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region MiniProfiler ���ܷ���
            services.AddMiniProfiler(options =>
            options.RouteBasePath = "/profiler"
             );
            #endregion
            #region ע��Swagger����
            services.AddSwaggerGen(c =>
              {
                  // ����ĵ���Ϣ
                  //�����汾��Ϣ
                  typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                  {
                      c.SwaggerDoc(version, new OpenApiInfo
                      {
                          //Version = "v1", //�汾��
                          Title = "SN���� API", //����
                          Description = "EFCore���ݲ��� ASP.NET Core Web API", //����
                          TermsOfService = new Uri("https://example.com/terms"), //��������
                          Contact = new OpenApiContact
                          {
                              Name = "Shayne Boyer", //��ϵ��
                              Email = string.Empty,  //����
                              Url = new Uri("https://twitter.com/spboyer"),//��վ
                          },
                          License = new OpenApiLicense
                          {
                              Name = "Use under LICX", //Э��
                              Url = new Uri("https://example.com/license"), //Э���ַ
                          }
                      });
                  });

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
            #endregion
            #region ע��DbContext
            services.AddDbContext<snblogContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            # region ����jwt
            services.ConfigureJwt(Configuration);
            //ע��JWT�����ļ�
            services.Configure<JwtConfig>(Configuration.GetSection("Authentication:JwtBearer"));
            #endregion
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
            #region DI����ע�����á�
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

            #endregion
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            #region Swagger+���ܷ�����MiniProfiler��+�Զ���ҳ��

            //����UseMiniProfiler
            app.UseMiniProfiler();
            //���Խ�Swagger��UIҳ��������Configure�Ŀ�������֮��
            // ����Swagger�м��
            app.UseSwagger();

            //����SwaggerUI
            app.UseSwaggerUI(c =>
            {
                typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                {
                c.IndexStream = () => GetType().GetTypeInfo()
                     .Assembly.GetManifestResourceStream("Snblog.index.html");
                ////������ҳΪSwagger
                c.RoutePrefix = string.Empty;
                //�Զ���ҳ�� �������ܷ���
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
                ////����Ϊnone���۵����з���
                c.DocExpansion(DocExpansion.None);
                ////����Ϊ-1 �ɲ���ʾmodels
                c.DefaultModelsExpandDepth(-1);
            });
            });
            #endregion
            app.UseHttpsRedirection();
            app.UseRouting();
            #region ����Cors���������м��
            app.UseCors("AllRequests");
            #endregion
            #region ����jwt
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
