using System;
using Kava.Data.Abstractions;

namespace Kava.Models;

public class Attachment : BaseEntity
{
    public Attachment(string name, ulong size, string mimeType)
    {
        Name = name;
        Size = size;
        MimeType = mimeType;
    }

    public string Name { get; set; }
    public ulong Size { get; set; }
    public string MimeType { get; set; }
    public Ulid CardId { get; set; }
}
