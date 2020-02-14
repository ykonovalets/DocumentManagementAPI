using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.System;
using DocumentManagement.Domain;
using DocumentManagement.Persistence;

namespace DocumentManagement.Services
{
    public class DocumentService
    {
        private readonly AzureBlobStorage _blobStorage;
        private readonly SqlDocumentRepository _documentRepository;
        private readonly Settings _settings;

        public DocumentService(
            AzureBlobStorage blobStorage,
            SqlDocumentRepository documentRepository,
            Settings settings)
        {
            _blobStorage = blobStorage;
            _documentRepository = documentRepository;
            _settings = settings;
        }

        public Task<IReadOnlyCollection<Document>> GetAll()
        {
            return _documentRepository.GetAll();
        }

        public async Task<Result> UploadNewDocument(string name, Stream content)
        {
            var createDocumentResult = await Document.Create(
                new DocumentInfo(name, content), 
                _settings,
                StoreDocument);

            if (!createDocumentResult.Successful)
            {
                return createDocumentResult;
            }

            await _documentRepository.Add(createDocumentResult.Value);
            return Result.Ok();
        }

        private Task<Uri> StoreDocument(DocumentInfo documentInfo)
        {
            return _blobStorage.UploadFromStream(documentInfo.Name, documentInfo.Content);
        }
    }
}
