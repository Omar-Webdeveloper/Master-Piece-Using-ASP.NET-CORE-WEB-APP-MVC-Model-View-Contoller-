using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryDesc { get; set; }
}
