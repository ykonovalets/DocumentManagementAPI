using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DocumentManagement.API.Extensions;
using DocumentManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v1/documents")]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        public class Document
        {
            [Required]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public Uri Location { get; set; }

            [Required]
            public long Size { get; set; }
        }

        /// <summary>
        /// Gets a list of uploaded documents
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<Document>>> GetAll()
        {
            var documents = await _documentService.GetAll();

            return documents.Select(d => new Document
            {
                Id = d.Id,
                Name = d.Name,
                Location = d.Location,
                Size = d.Size
            }).ToArray();
        }

        /// <summary>
        /// Uploads new document
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Upload([Required] IFormFile formFile)
        {
            // Using IFormFile means that files are buffered. If the size or frequency of file uploads
            // is exhausting app resources, then streaming approach should be used.

            using (var stream = formFile.OpenReadStream())
            {
                // TODO: Do not use the FileName property of IFormFile other than for display and logging.
                // When displaying or logging, HTML encode the file name. An attacker can provide a malicious filename,
                // including full paths or relative paths.
                var uploadNewDocumentResult = await _documentService.UploadNewDocument(formFile.FileName, stream);
                return this.Result(uploadNewDocumentResult);
            }
        }

        public class ReorderDocumentRequest
        {
            /// <summary>
            /// Ordered document ids
            /// </summary>
            public IReadOnlyCollection<int> OrderedDocumentIds { get; set; }
        }

        /// <summary>
        /// Reorders list of uploaded documents
        /// </summary>
        [HttpPut("reorder")]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public ActionResult Reorder([FromBody, Required] ReorderDocumentRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes uploaded document by id
        /// </summary>
        /// <param name="id">Document id</param>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
