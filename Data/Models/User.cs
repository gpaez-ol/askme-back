using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AskMe.Data.BaseEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AskMe.Data.Models
{
    public class User : Entity
    {
        /// <summary>
        /// Cognito Client Id
        /// </summary>
        [MaxLength(500)]
        public string CognitoClientId { get; set; }

        /// <summary>
        /// AWS Bucket reference to Avatar image
        /// </summary>
        public string Avatar { get; set; }

        /// <summary> 
        /// The first name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary> 
        /// The last name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Phone number of user
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Free Biography for the user to put information
        /// </summary>
        [JsonProperty("bio")]
        [MaxLength(250)]
        public string Bio { get; set; }

        /// <summary>
        /// Type of user
        /// </summary>
        [JsonProperty("type")]
        [Required]
        public UserType Type { get; set; }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserType
    {
        [EnumMember(Value = "Fan")]
        Fan,
        [EnumMember(Value = "Creator")]
        Creator,
        [EnumMember(Value = "Admin")]
        Admin
    }
}
