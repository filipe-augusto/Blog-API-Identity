using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatorio")]
        [EmailAddress(ErrorMessage = "O e-mail é inválido")]
        public string Email { get; set; }
    }
}
