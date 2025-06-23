using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace CmsSample.Models
{
    public class StaticPage
    {
        public int Id { get; set; }

        [Required]
        public string Slug { get; set; }  // örnek: "hakkimizda", "iletisim"

        [Required]
        public string Title { get; set; }

        public string HtmlBody { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
