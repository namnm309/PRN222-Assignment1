namespace PresentationLayer.Models.Auth
{
    public class LoginViewModel
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}



