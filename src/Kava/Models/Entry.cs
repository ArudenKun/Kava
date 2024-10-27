using System;
using System.Collections.Generic;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Entry : BaseEntity
{
    public string Summary { get; set; } = "";

    public string Description { get; set; } = "";

    public List<Attachment> Attachments { get; set; } = [];

    public List<Comment> Comments { get; set; } = [];

    public required Guid CardId { get; set; }

    public Card? Card { get; init; }
}
