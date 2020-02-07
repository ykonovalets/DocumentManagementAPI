using System;

namespace DocumentManagement.API.Services
{
    public class Document
    {
        internal Document(int id, string name, Uri location, long size, int sortOrder)
        {
            Id = id;
            Name = name;
            Location = location;
            Size = size;
            SortOrder = sortOrder;
        }

        public Document(string name, Uri location, long size) : this(0, name, location, size, 0)
        {
        }

        public int Id { get; }
        public string Name { get; }
        public Uri Location { get; }
        public long Size { get; }
        public int SortOrder { get; }
    }
}
