using System;
using System.Collections.Generic;

namespace HomestayWeb.Model;

public partial class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int HomestayId { get; set; }

    public string Content { get; set; } = null!;

    public int? CommentId { get; set; }

    public DateTime Time { get; set; }

    public virtual Comment? CommentNavigation { get; set; }

    public virtual Homestay Homestay { get; set; } = null!;

    public virtual ICollection<Comment> InverseCommentNavigation { get; set; } = new List<Comment>();

    public virtual User User { get; set; } = null!;
}
