using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DocumentManagement.API.Services;

namespace DocumentManagement.API.Infrastructure
{
    public class SqlDocumentRepository
    {
        private readonly IDbConnection _dbConnection;

        public SqlDocumentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IReadOnlyCollection<Document>> GetAll()
        {
            return (await _dbConnection.QueryAsync<Document>("SELECT Id, Name, Location, Size, SortOrder FROM [dbo].[Document]")).ToArray();
        }

        public Task Add(Document document)
        {
            return _dbConnection.ExecuteAsync(
                "INSERT INTO [dbo].[Document] (Name, Location, Size, SortOrder) VALUES (@Name, @Location, @Size, @SortOrder)",
                document);
        }
    }
}
