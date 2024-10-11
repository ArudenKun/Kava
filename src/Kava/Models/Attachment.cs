using System;
using System.ComponentModel.DataAnnotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Attachment : BaseEntity
{
    public required Ulid CardId { get; init; }
    public required string Name { get; set; }
    public required ulong Size { get; set; }
    public required string MimeType { get; set; }
    public Card Card { get; init; } = null!;
}
