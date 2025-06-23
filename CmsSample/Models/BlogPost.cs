using System;
using System.ComponentModel.DataAnnotations;

namespace CmsSample.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public string Title { get; set; }

        public string Excerpt { get; set; }
        public bool IsFeatured { get; set; }

        public string HtmlBody { get; set; }

        public string?CoverImageUrl { get; set; }

        public DateTime PublishedOn { get; set; } = DateTime.Now;
        [Required] public string AuthorId { get; set; } = null!;
    }
}
