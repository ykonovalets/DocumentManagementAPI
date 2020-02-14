using System.IO;

namespace DocumentManagement.Domain
{
    public class DocumentInfo
    {
        public DocumentInfo(string name, Stream content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }
        public Stream Content { get; }
    }
}