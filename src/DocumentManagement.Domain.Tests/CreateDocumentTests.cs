using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentManagement.Domain.Tests
{
    [TestFixture]
    public class CreateDocumentTests
    {
        private static readonly Uri _dummyDocumentLocation = new Uri("https://github.com/document");
        private readonly StoreDocument _dummyStoreDocumentFunction = (documentInfo => Task.FromResult(_dummyDocumentLocation));

        private static DocumentInfo GenerateDocumentInfo(string name, long size) => new DocumentInfo(name, new MemoryStream(new byte[size]));

        [Test]
        public async Task Given_FileWithInvalidExtension_When_CreateDocument_Then_InvalidDocumentExtensionError()
        {
            // Given
            var documentInfo = GenerateDocumentInfo("document.txt", 10);
            var settings = new Settings(2000, new[] { ".pdf" });

            // When
            var createDocumentResult = await Document.Create(
                documentInfo,
                settings,
                _dummyStoreDocumentFunction);

            // Then
            createDocumentResult.Successful.Should().BeFalse();
            createDocumentResult.Error.Should().BeEquivalentTo(Errors.InvalidDocumentExtension(settings.ValidDocumentExtensions));
        }

        [TestCase(100, 100)]
        [TestCase(100, 101)]
        public async Task Given_FileEqualsOrGreaterThanMaxDocumentSize_When_CreateDocument_Then_InvalidDocumentSizeError(
            long maxDocumentSizeInBytes, 
            long documentSizeInBytes)
        {
            // Given
            var settings = new Settings(maxDocumentSizeInBytes, new[] { ".pdf" });
            var documentInfo = GenerateDocumentInfo("document.pdf", documentSizeInBytes);

            // When
            var createDocumentResult = await Document.Create(
                documentInfo,
                settings,
                _dummyStoreDocumentFunction);

            // Then
            createDocumentResult.Successful.Should().BeFalse();
            createDocumentResult.Error.Should().BeEquivalentTo(Errors.InvalidDocumentSize(settings.MaxDocumentSizeInBytes));
        }

        [TestCase("document.pdf", 2000)]
        public async Task Given_ValidDocument_When_CreateDocument_Then_DocumentShouldBeCreatedSuccessfully(string name, long size)
        {
            // Given
            var settings = new Settings(50000, new[] { ".pdf" });
            var documentInfo = GenerateDocumentInfo(name, size);

            // When
            var createDocumentResult = await Document.Create(
                documentInfo,
                settings,
                _dummyStoreDocumentFunction);

            // Then
            createDocumentResult.Successful.Should().BeTrue();
            createDocumentResult.Error.Should().BeNull();
            createDocumentResult.Value.Should().NotBeNull();

            var document = createDocumentResult.Value;
            document.Id.Should().Be(0);
            document.Name.Should().Be(name);
            document.Size.Should().Be(size);
            document.Location.Should().Be(_dummyDocumentLocation);
            document.SortOrder.Should().Be(0);
        }
    }
}
