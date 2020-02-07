using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DocumentManagement.API
{
    public class Settings
    {
        private readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string DocumentBlobContainerConnectionString =>
            _configuration.GetValue<string>("DocumentsBlobContainer:ConnectionString");

        public string DocumentBlobContainerName =>
            _configuration.GetValue<string>("DocumentsBlobContainer:Name");

        public string DocumentsDbSqlConnectionString =>
            _configuration.GetValue<string>("DocumentsSqlConnectionString");

        public long MaxDocumentSizeInBytes => _configuration.GetValue<long>("MaxDocumentSizeInBytes");

        public IReadOnlyCollection<string> AllowedDocumentExtensions =>
            _configuration.GetSection("AllowedDocumentExtensions").Get<string[]>();
    }
}
