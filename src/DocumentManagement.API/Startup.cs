using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using DocumentManagement.API.Infrastructure;
using DocumentManagement.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DocumentManagement.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings = new Settings(configuration);
        }

        public IConfiguration Configuration { get; }

        public Settings Settings { get;  }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Infrastructure should be set up using ARM template or Terraform script
            SetupInfrastructure(Settings).GetAwaiter().GetResult();

            RegisterServices(services);
            ConfigureDapper();

            services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonFormatters()
                .AddApiExplorer();

            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "Document API", Version = "v1" });
                cfg.IncludeXmlComments(GetXmlDocPath(GetType()));
            });
        }

        private static void ConfigureDapper()
        {
            SqlMapper.AddTypeHandler(new UriTypeHandler());
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(sp => new AzureBlobStorage(
                Settings.DocumentBlobContainerConnectionString,
                Settings.DocumentBlobContainerName));

            services.AddScoped<IDbConnection>(sp => new SqlConnection(Settings.DocumentsDbSqlConnectionString));
            services.AddTransient<SqlDocumentRepository>();

            services.AddSingleton(sp => new DocumentService.Settings(
                Settings.MaxDocumentSizeInBytes,
                Settings.AllowedDocumentExtensions));

            services.AddTransient<DocumentService>();
        }

        private static string GetXmlDocPath(Type type)
        {
            return Path.Combine(AppContext.BaseDirectory, $"{type.Assembly.GetName().Name}.XML");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document API v1"); });
        }

        private static async Task SetupInfrastructure(Settings settings)
        {
            await CreatePublicBlobContainerIfNotExists(
                settings.DocumentBlobContainerConnectionString,
                settings.DocumentBlobContainerName);

            SetupSqlDb(settings.DocumentsDbSqlConnectionString);
        }

        private static async Task CreatePublicBlobContainerIfNotExists(string storageConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync();

            var permissions = await blobContainer.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await blobContainer.SetPermissionsAsync(permissions);
        }

        private static void SetupSqlDb(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var databaseUpgradeResult = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .WithTransaction()
                .LogToConsole()
                .Build()
                .PerformUpgrade();

            if (!databaseUpgradeResult.Successful)
            {
                throw databaseUpgradeResult.Error;
            }
        }
    }
}
