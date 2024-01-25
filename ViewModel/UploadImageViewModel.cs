using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Imagem inválida")]
        public string Base64Image { get; set; }
    }
}
