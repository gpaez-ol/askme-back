using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Models;
using Askme.Data.DTO;
using AskMe.Repositories;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly CookieConfig _cookieConfig;

        public IdentityController(UserRepository userRepository, IOptions<CookieConfig> config)
        {
            _userRepository = userRepository;
            _cookieConfig = config.Value;
        }

        //POST identity/signup
        /// <summary>
        /// This POST method creates a SignUpRequest
        /// </summary>
        /// <returns>SignUpRequest</returns>
        [HttpPost("/signup")]
        public Task<UserSignUpResponseDTO> SignUp([FromBody] UserSignUpDTO signUpModel)
        {
            return _userRepository.CreateUserAsync(signUpModel);
        }

        //PUT identity/confirm-signup
        /// <summary>
        /// This PUT method validates an user email is real
        /// </summary>
        /// <returns>UserSignUpResponse</returns>
        [HttpPut("/confirm-signup")]
        public Task<UserSignUpResponseDTO> ConfirmSignUp([FromBody] UserConfirmSignUpDTO confirmSignUpModel)
        {
            return _userRepository.ConfirmUserSignUpAsync(confirmSignUpModel);
        }

        //PUT identity/confirm-signup
        /// <summary>
        /// This PUT method resends the confirmation code
        /// </summary>
        /// <returns>UserSignUpResponse</returns>
        [HttpPut("/resend-confirmation-code")]
        public async Task<AuthResponseDTO> ResendConfirmationCode([FromBody] UserLoginDTO loginModel)
        {
            var authResponseModel = await _userRepository.ResendConfirmationCode(loginModel);
            return authResponseModel;
        }
        //PUT identity/confirm-signup
        /// <summary>
        /// This PUT method sends the password if it was forgotten
        /// </summary>
        /// <returns>UserSignUpResponse</returns>
        [HttpPut("/forgot-password")]
        public async Task<AuthResponseDTO> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordModel)
        {
            var authResponseModel = await _userRepository.ForgotPassword(forgotPasswordModel);
            return authResponseModel;
        }
        //PUT identity/confirm-signup
        /// <summary>
        /// This PUT method resets the password
        /// </summary>
        /// <returns>UserSignUpResponse</returns>
        [HttpPut("/reset-password")]
        public async Task<AuthResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordModel)
        {
            var authResponseModel = await _userRepository.ResetPassword(resetPasswordModel);
            return authResponseModel;

        }
        //PUT identity/login
        /// <summary>
        /// This PUT method validates an user email is real
        /// </summary>
        /// <returns>AuthResponseModel</returns>
        [HttpPut("/login")]
        public async Task<AuthResponseDTO> Login([FromBody] UserLoginDTO loginModel)
        {
            var authResponseModel = await _userRepository.Login(loginModel);


            var cookieOptions = new CookieOptions
            {
                Domain = _cookieConfig.Domain,
                Secure = _cookieConfig.Secure,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            };
            if (authResponseModel == null)
            {
                Console.WriteLine("There was an error");
                return authResponseModel;
            }
            HttpContext.Response.Cookies.Append("UserId", authResponseModel.UserId, cookieOptions);
            HttpContext.Response.Cookies.Append("AccessToken", authResponseModel.Tokens.AccessToken, cookieOptions);
            HttpContext.Response.Cookies.Append("RefreshToken", authResponseModel.Tokens.RefreshToken, cookieOptions);
            HttpContext.Response.Cookies.Append("IdToken", authResponseModel.Tokens.IdToken, cookieOptions);
            return authResponseModel;
        }

        //GET logout
        /// <summary>
        /// This GET method logouts the session
        /// </summary>
        /// <returns>Empty</returns>
        [HttpGet("/logout")]
        [Authorize]
        public void Logout()
        {
            HttpContext.Response.Cookies.Delete("AccessToken");
            HttpContext.Response.Cookies.Delete("RefreshToken");
            HttpContext.Response.Cookies.Delete("IdToken");
        }

        //GET authorized
        /// <summary>
        /// This GET method validates the actual session is authorized
        /// </summary>
        /// <returns>string</returns>
        [HttpGet("/authorized")]
        public string IsAuthorized()
        {
            if (HttpContext.Request.Cookies.ContainsKey("AccessToken"))
            {
                HttpContext.Response.StatusCode = 200;
                return HttpContext.Request.Cookies["UserId"];
            }

            HttpContext.Response.StatusCode = 401;
            return null;
        }
    }
}
