using System;
using System.Collections.Generic;

namespace Master_Piece.Models;

public partial class UserRole
{
    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public int Id { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
