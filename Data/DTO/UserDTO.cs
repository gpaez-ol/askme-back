using System.ComponentModel.DataAnnotations;
using AskMe.Data.Models;
using Newtonsoft.Json;

namespace AskMe.Data.DTO
{
    public class UserSignUpDTO
    {
        public string Bio { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [Range(0, 1)]
        public UserType Type { get; set; }
    }
    public class UserSignUpResponseDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
    public class UserLoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class ForgotPasswordDTO
    {

        [Required]
        public string Email { get; set; }
    }
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string ConfirmationCode { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class AuthResponseDTO
    {
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public TokenDTO Tokens { get; set; }
    }

    public class TokenDTO
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
    public class UserConfirmSignUpDTO
    {
        public string EmailAddress { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
