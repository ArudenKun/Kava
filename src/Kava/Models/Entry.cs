using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Entry : BaseEntity
{
    [Column(StringLength = 244)]
    public string Summary { get; set; } = "";

    [Column(StringLength = -1)]
    public string Description { get; set; } = "";

    [Navigate(nameof(Attachment.EntryId))]
    public List<Attachment> Attachments { get; set; } = [];

    [Navigate(nameof(Comment.EntryId))]
    public List<Comment> Comments { get; set; } = [];

    public required Guid CardId { get; set; }

    [Navigate(nameof(CardId))]
    public Card? Card { get; init; }
}
