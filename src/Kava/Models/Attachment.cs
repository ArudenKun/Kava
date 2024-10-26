using System;
using System.Diagnostics.CodeAnalysis;
using FreeSql.DataAnnotations;
using Kava.Models.Abstractions;
using Kava.Utilities.Helpers;

namespace Kava.Models;

public class Attachment : BaseEntity
{
    private string _fileName;

    [Column(StringLength = 128)]
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

    [Navigate(nameof(EntryId))]
    public Entry? Entry { get; init; }
}
