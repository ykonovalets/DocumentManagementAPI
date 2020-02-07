using System.Collections.Generic;

namespace DocumentManagement.API.Services
{
    public static class Errors
    {
        public static Error InvalidDocumentExtension(IEnumerable<string> allowedDocumentExtensions) =>
            new Error(
                "InvalidDocumentExtension",
                $"Invalid document extension. Allowed extensions: '{string.Join(",", allowedDocumentExtensions)}'");

        public static Error InvalidDocumentSize(long maxSizeInBytes) =>
            new Error("InvalidDocumentSize", $"Document size should be less than '{maxSizeInBytes}' bytes");
    }

    public class Error
    {
        public Error(string name, string message)
        {
            Name = name;
            Message = message;
        }

        /// <summary>
        /// Unique error name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Meaningful error description
        /// </summary>
        public string Message { get; }
    }
}