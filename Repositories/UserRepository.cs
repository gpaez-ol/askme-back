using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using AskMe.Data.Context;
using AskMe.Data.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using AskMe.Data.Config;
using Askme.Data.DTO;

namespace AskMe.Repositories
{
    public class UserRepository
    {
        private readonly AppConfig _cloudConfig;
        private readonly AmazonCognitoIdentityProviderClient _provider;
        private readonly CognitoUserPool _userPool;
        private readonly AskMeContext _context;
        private readonly IMapper _mapper;
        static readonly HttpClient client = new HttpClient();
        public UserRepository(IOptions<AppConfig> config, AskMeContext context, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            //TO-DO: Isolate Cognito Repository from Normal User Repository
            _cloudConfig = config.Value;
            _provider = new AmazonCognitoIdentityProviderClient
            (
                _cloudConfig.AccessKeyId,
                _cloudConfig.AccessSecretKey,
                RegionEndpoint.GetBySystemName(_cloudConfig.Region)
            );
            _userPool = new CognitoUserPool
            (
                _cloudConfig.UserPoolId,
                _cloudConfig.AppClientId,
                _provider
            );
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users.Where(user => user.Id.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void UpdateUser(User newUser)
        {
            _context.Update(newUser);
        }

        public Task<User> UpdateUserAsync(User newUser)
        {
            try
            {
                var user = _context.Update(newUser);
                return Task.FromResult(user.Entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"User could not be updated: {ex.Message}", ex);
            }
        }

        public void DeleteUser(Guid id)
        {
            var user = _context.Users.Where(user => user.Id == id).FirstOrDefaultAsync();
            _context.Remove(user);
        }

        public async Task<UserSignUpResponseDTO> CreateUserAsync(UserSignUpDTO model)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _cloudConfig.AppClientId,
                Password = model.Password,
                Username = model.Email
            };

            try
            {
                SignUpResponse response = await _provider.SignUpAsync(signUpRequest);
                var newUser = new User()
                {
                    Id = new Guid(response.UserSub),
                    Type = model.Type,
                    CognitoClientId = response.UserSub,
                    Phone = model.PhoneNumber,
                    Avatar = "profile.jfif", // TO-DO add preset of avatar
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Bio = model.Bio
                };
                await CreateUser(newUser);
                _context.SaveChanges();
                return new UserSignUpResponseDTO
                {
                    UserId = response.UserSub,
                    Email = model.Email,
                    Message = "Confirmation code sent"
                };
            }
            catch (UsernameExistsException e)
            {
                // return new UserSignUpResponse
                // {
                //     IsSuccess = false,
                //     Message = "Email already exists",
                //     ResponseError = AuthResponseError.EmailAlreadyExists

                // };
                Console.WriteLine("Email already exists");
                return null;
            }
        }

        public async Task<AuthResponseDTO> ResendConfirmationCode(UserLoginDTO model)
        {
            try
            {
                CognitoUser user = new CognitoUser(model.Email, _cloudConfig.AppClientId, _userPool, _provider);
                InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest
                {
                    Password = model.Password
                };

                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest);
                var result = authResponse.AuthenticationResult;

                // return new AuthResponseDTO
                // {
                //     IsSuccess = false,
                //     Message = "User is already confirmed",
                //     ResponseError = AuthResponseError.UserAlreadyConfirmed
                // };
                return null;
            }
            catch (UserNotFoundException e)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User not found",
                //     ResponseError = AuthResponseError.UserNotFound
                // };
                return null;
            }
            catch (NotAuthorizedException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "Incorrect username or password",
                //     ResponseError = AuthResponseError.IncorrectUsernameOrPassword
                // };
                return null;
            }
            catch (UserNotConfirmedException)
            {
                ResendConfirmationCodeRequest request = new ResendConfirmationCodeRequest()
                {
                    ClientId = _cloudConfig.AppClientId,
                    Username = model.Email,
                };
                await _provider.ResendConfirmationCodeAsync(request);
                return new AuthResponseDTO
                {
                    Message = "Confirmation Code has been resent"
                };
            }

        }
        public async Task<AuthResponseDTO> ForgotPassword(ForgotPasswordDTO forgotPasswordModel)
        {
            try
            {
                var request = new ForgotPasswordRequest()
                {
                    ClientId = _cloudConfig.AppClientId,
                    Username = forgotPasswordModel.Email
                };
                await _provider.ForgotPasswordAsync(request);

                return new AuthResponseDTO
                {
                    Message = "Forgot Password code sent"
                };
            }
            catch (UserNotFoundException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User not found",
                //     ResponseError = AuthResponseError.UserNotFound
                // };
                return null;
            }
            catch (UserNotConfirmedException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User has not been confirmed",
                //     ResponseError = AuthResponseError.UserNotConfirmed
                // };
                return null;
            }
        }
        public async Task<AuthResponseDTO> ResetPassword(ResetPasswordDTO resetPasswordModel)
        {
            try
            {
                var request = new ConfirmForgotPasswordRequest()
                {
                    ClientId = _cloudConfig.AppClientId,
                    Username = resetPasswordModel.Email,
                    ConfirmationCode = resetPasswordModel.ConfirmationCode,
                    Password = resetPasswordModel.NewPassword,
                };
                await _provider.ConfirmForgotPasswordAsync(request);

                return new AuthResponseDTO
                {
                    Message = "Password reset succesfully"
                };
            }
            catch (UserNotFoundException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User not found",
                //     ResponseError = AuthResponseError.UserNotFound
                // };
                return null;
            }
            catch (UserNotConfirmedException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User has not been confirmed",
                //     ResponseError = AuthResponseError.UserNotConfirmed

                // };
                return null;
            }
            catch (CodeMismatchException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "Confirmation Code is invalid",
                //     ResponseError = AuthResponseError.InvalidConfirmationCode
                // };
                return null;
            }
            catch (ExpiredCodeException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "Confirmation Code is expired",
                //     ResponseError = AuthResponseError.ExpiredConfirmationCode
                // };
                return null;
            }
        }
        public async Task<UserSignUpResponseDTO> ConfirmUserSignUpAsync(UserConfirmSignUpDTO model)
        {
            var user = await GetUserByEmail(model.EmailAddress);
            if (user == null || user == default(User))
            {
                // return new UserSignUpResponse
                // {
                //     IsSuccess = false,
                //     Message = "User does not exist",
                //     Email = model.EmailAddress,
                //     ResponseError = AuthResponseError.UserNotFound
                // };
                return null;
            }
            ConfirmSignUpRequest request = new ConfirmSignUpRequest
            {
                ClientId = _cloudConfig.AppClientId,
                ConfirmationCode = model.ConfirmationCode,
                Username = model.EmailAddress
            };

            try
            {
                var response = await _provider.ConfirmSignUpAsync(request);
                return new UserSignUpResponseDTO
                {
                    UserId = user.CognitoClientId,
                    Email = model.EmailAddress,
                    Message = "User Confirmed"
                };
            }
            catch (CodeMismatchException e)
            {
                // return new UserSignUpResponse
                // {
                //     IsSuccess = false,
                //     Message = "Invalid confirmation code",
                //     Email = model.EmailAddress,
                //     ResponseError = AuthResponseError.InvalidConfirmationCode
                // };
                return null;
            }
        }

        public async Task<AuthResponseDTO> Login(UserLoginDTO model)
        {
            try
            {
                CognitoUser user = new CognitoUser(model.Email, _cloudConfig.AppClientId, _userPool, _provider);
                InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest
                {
                    Password = model.Password
                };

                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest);
                var result = authResponse.AuthenticationResult;
                var databaseUser = await GetUserById(Guid.Parse(user.Username));

                if (databaseUser == null || databaseUser == default(User))
                {
                    // return new AuthResponseModel
                    // {
                    //     IsSuccess = false,
                    //     Message = "User not found in database",
                    //     ResponseError = AuthResponseError.UserNotFound
                    // };
                    Console.WriteLine("User not found in database");
                    return null;
                }
                return new AuthResponseDTO
                {
                    EmailAddress = model.Email,
                    UserId = user.Username,
                    Tokens = new TokenDTO
                    {
                        IdToken = result.IdToken,
                        AccessToken = result.AccessToken,
                        ExpiresIn = result.ExpiresIn,
                        RefreshToken = result.RefreshToken
                    },
                };
            }
            catch (UserNotFoundException e)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User not found",
                //     ResponseError = AuthResponseError.UserNotFound
                // };
                Console.WriteLine("User not found exception");
                return null;
            }
            catch (NotAuthorizedException e)
            {
                Console.WriteLine(e.Message);
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "Incorrect username or password",
                //     ResponseError = AuthResponseError.IncorrectUsernameOrPassword
                // };
                Console.WriteLine("NotAuthorizedException");
                return null;
            }
            catch (UserNotConfirmedException)
            {
                // return new AuthResponseModel
                // {
                //     IsSuccess = false,
                //     Message = "User has not been confirmed",
                //     ResponseError = AuthResponseError.UserNotConfirmed
                // };
                Console.WriteLine("UserNotConfirmedException");
                return null;
            }
        }

        public async Task<AdminInitiateAuthResponse> AuthenticateWithToken(string refreshToken)
        {
            var authParams = new Dictionary<string, string>();
            authParams.Add("REFRESH_TOKEN", refreshToken);

            var authRequest = new AdminInitiateAuthRequest
            {
                AuthFlow = AuthFlowType.REFRESH_TOKEN,
                ClientId = _cloudConfig.AppClientId,
                UserPoolId = _cloudConfig.UserPoolId,
                AuthParameters = authParams
            };

            var authResponse = await _provider.AdminInitiateAuthAsync(authRequest);
            return authResponse;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var cognitoIssuer = $"https://cognito-idp.{_cloudConfig.Region}.amazonaws.com/{_cloudConfig.UserPoolId}";
            var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    var json = new WebClient().DownloadString(jwtKeySetUrl);
                    return JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                },
                ValidIssuer = cognitoIssuer,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false
            };

            SecurityToken validatedToken;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception)
            {
                return false;
            }

            return validatedToken != null;
        }
    }
}