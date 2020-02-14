using System.Collections.Generic;
using Common.System;

namespace DocumentManagement.Domain
{
    public static class Errors
    {
        public static readonly Error InvalidDocumentName = new Error("InvalidDocumentName", "Document name cannot be null or empty");

        public static Error InvalidDocumentExtension(IEnumerable<string> validDocumentExtensions) =>
            new Error(
                "InvalidDocumentExtension",
                $"Invalid document extension. Allowed extensions: '{string.Join(",", validDocumentExtensions)}'");

        public static Error InvalidDocumentSize(long maxSizeInBytes) =>
            new Error("InvalidDocumentSize", $"Document size should be less than '{maxSizeInBytes}' bytes");
    }
}