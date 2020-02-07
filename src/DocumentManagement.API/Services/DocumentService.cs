using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentManagement.API.Common;
using DocumentManagement.API.Infrastructure;

namespace DocumentManagement.API.Services
{
    public class DocumentService
    {
        public class Settings
        {
            public Settings(long maxDocumentSizeInBytes, IReadOnlyCollection<string> allowedDocumentExtensions)
            {
                MaxDocumentSizeInBytes = maxDocumentSizeInBytes;
                AllowedDocumentExtensions = allowedDocumentExtensions;
            }

            public long MaxDocumentSizeInBytes { get; }
            public IReadOnlyCollection<string> AllowedDocumentExtensions { get; }
        }

        private readonly AzureBlobStorage _blobStorage;
        private readonly SqlDocumentRepository _sqlDocumentRepository;
        private readonly Settings _settings;

        public DocumentService(
            AzureBlobStorage blobStorage, 
            SqlDocumentRepository sqlDocumentRepository,
            Settings settings)
        {
            _blobStorage = blobStorage;
            _sqlDocumentRepository = sqlDocumentRepository;
            _settings = settings;
        }

        public Task<IReadOnlyCollection<Document>> GetAll()
        {
            return _sqlDocumentRepository.GetAll();
        }

        public async Task<Result> UploadNewDocument(string name, Stream stream)
        {
            var validateDocumentResult = ValidateDocument(name, stream, _settings);
            if (!validateDocumentResult.Successful)
            {
                return validateDocumentResult;
            }

            var location = await _blobStorage.UploadFromStream(name, stream);

            var document = new Document(name, location, stream.Length);
            await _sqlDocumentRepository.Add(document);

            return Result.Ok();
        }

        private static Result ValidateDocument(string name, Stream stream, Settings settings)
        {
            Func<Result>[] orderedValidators =
            {
                () => ValidateExtension(name, settings.AllowedDocumentExtensions),
                () => ValidateSize(stream, settings.MaxDocumentSizeInBytes)
            };

            foreach (var validator in orderedValidators)
            {
                var validationResult = validator();
                if (!validationResult.Successful)
                {
                    return validationResult.Error;
                }
            }
            return Result.Ok();
        }

        private static Result ValidateExtension(
            string untrustedDocumentName, 
            IReadOnlyCollection<string> allowedDocumentExtensions)
        {
            var name = Path.GetFileName(untrustedDocumentName);
            var extension = Path.GetExtension(name).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedDocumentExtensions.Contains(extension))
            {
                return Errors.InvalidDocumentExtension(allowedDocumentExtensions);
            }

            // TODO: Validate that extension matches the content of the file
            return Result.Ok();
        }

        private static Result ValidateSize(Stream stream, long maxDocumentSizeInBytes)
        {
            return stream.Length >= maxDocumentSizeInBytes
                ? Errors.InvalidDocumentSize(maxDocumentSizeInBytes)
                : Result.Ok();
        }
    }
}
