using System;
using System.ComponentModel.DataAnnotations;

namespace CmsSample.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(150)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SentOn { get; set; } = DateTime.Now;
    }
}
