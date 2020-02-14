using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.System;

namespace DocumentManagement.Domain
{
    public delegate Task<Uri> StoreDocument(DocumentInfo documentInfo);

    public class Document
    {
        internal Document(int id, string name, Uri location, long size, int sortOrder)
        {
            Ensure.NotNullOrEmpty(name, nameof(name));
            Ensure.NotNull(location, nameof(location));
            Ensure.GreaterThan(size, 0, nameof(size));

            Id = id;
            Name = name;
            Location = location;
            Size = size;
            SortOrder = sortOrder;
        }

        private Document(string name, Uri location, long size) : this(0, name, location, size, 0)
        {
        }

        public static async Task<Result<Document>> Create(
            DocumentInfo documentInfo, 
            Settings settings,
            StoreDocument storeDocument)
        {
            var validateDocumentResult = ValidateDocument(documentInfo, settings);
            if (!validateDocumentResult.Successful)
            {
                return validateDocumentResult.Error;
            }

            var documentUri = await storeDocument(documentInfo);
            return new Document(documentInfo.Name, documentUri, documentInfo.Content.Length);
        }

        public int Id { get; }
        public string Name { get; }
        public Uri Location { get; }
        public long Size { get; }
        public int SortOrder { get; }

        private static Result ValidateDocument(DocumentInfo documentInfo, Settings settings)
        {
            Func<Result>[] orderedValidators =
            {
                () => ValidateExtension(documentInfo, settings.ValidDocumentExtensions),
                () => ValidateSize(documentInfo, settings.MaxDocumentSizeInBytes)
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
            DocumentInfo documentInfo,
            IReadOnlyCollection<string> allowedDocumentExtensions)
        {
            if (string.IsNullOrWhiteSpace(documentInfo.Name))
            {
                return Errors.InvalidDocumentName;
            }

            var name = Path.GetFileName(documentInfo.Name);
            var extension = Path.GetExtension(name).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedDocumentExtensions.Contains(extension))
            {
                return Errors.InvalidDocumentExtension(allowedDocumentExtensions);
            }

            // TODO: Validate that extension matches the content of the file
            return Result.Ok();
        }

        private static Result ValidateSize(DocumentInfo documentInfo, long maxDocumentSizeInBytes)
        {
            return documentInfo.Content.Length >= maxDocumentSizeInBytes
                ? Errors.InvalidDocumentSize(maxDocumentSizeInBytes)
                : Result.Ok();
        }
    }
}
