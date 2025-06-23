using System;
using System.ComponentModel.DataAnnotations;

namespace CmsSample.Models;
public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }

    [Required] public string AuthorId { get; set; } = null!;

    [Required, StringLength(2000)]
    public string Body { get; set; } = null!;

    public DateTime PostedOn { get; set; } = DateTime.Now;
}
