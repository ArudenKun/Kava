using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Kava.Models.Abstractions;

namespace Kava.Models;

public class Board : BaseEntity
{
    [Column(StringLength = 64)]
    public required string Name { get; set; }

    [Navigate(nameof(Card.BoardId))]
    public List<Card> Categories { get; set; } = [];
}
