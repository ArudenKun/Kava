using System;
using System.Diagnostics.CodeAnalysis;
using Kava.Core.Helpers;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Attachment : BaseEntity
{
    private string _fileName;

    public required string FileName
    {
        get => _fileName;
        [MemberNotNull(nameof(_fileName))]
        set
        {
            _fileName = value;
            MimeType = MimeTypes.GetMimeType(value);
            Hash = $"{Id}-{value}-{MimeType}".GetMD5Hash();
        }
    }

    public required double Size { get; set; }

    public string MimeType { get; set; } = string.Empty;

    public string Hash { get; set; } = string.Empty;

    public required Guid EntryId { get; set; }

    public Entry? Entry { get; init; }
}
