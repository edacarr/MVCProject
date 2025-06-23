using System.ComponentModel.DataAnnotations;

namespace CmsSample.Models
{
    public class SliderItem
    {
        public int Id { get; set; }
      

        [Required]
        public string Caption { get; set; }  // Slider üzerindeki yazı

        public string? ImageUrl { get; set; }  // Görselin dosya yolu

        public string TargetUrl { get; set; }  // Tıklanınca gidilecek link

        public int DisplayOrder { get; set; }  // Slider sırası (1, 2, 3...)
    }
}
