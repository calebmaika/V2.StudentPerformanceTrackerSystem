using System.ComponentModel.DataAnnotations;

namespace StudentPerformanceTracker.WebApp.Models.Admin
{
    /// <summary>
    /// View model for the admin login form
    /// This represents the data that comes from the login page
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Username field
        /// Required means user MUST fill this in
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password field
        /// DataType.Password makes it show as ******* in the browser
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Remember me checkbox
        /// If checked, user stays logged in for 30 days
        /// If unchecked, session expires in 2 hours
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}