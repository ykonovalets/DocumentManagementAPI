using System.Collections.Generic;
using Common.System;

namespace DocumentManagement.Domain
{
    public class Settings
    {
        public Settings(long maxDocumentSizeInBytes, IReadOnlyCollection<string> validDocumentExtensions)
        {
            Ensure.GreaterThan(maxDocumentSizeInBytes, 0, nameof(maxDocumentSizeInBytes));
            Ensure.NotNullOrEmpty(validDocumentExtensions, nameof(validDocumentExtensions));

            MaxDocumentSizeInBytes = maxDocumentSizeInBytes;
            ValidDocumentExtensions = validDocumentExtensions;
        }

        public long MaxDocumentSizeInBytes { get; }
        public IReadOnlyCollection<string> ValidDocumentExtensions { get; }
    }
}
