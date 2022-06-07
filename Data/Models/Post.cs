using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AskMe.Data.BaseEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AskMe.Data.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        /// <summary>
        /// AWS Bucket reference to the Post image
        /// </summary>
        [JsonProperty("imageRef")]
        public string? ImageRef { get; set; }

        /// <summary> 
        /// Title of the post
        /// </summary>
        [Required]
        [MaxLength(60)]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary> 
        /// Content of the Post
        /// </summary>
        [Required]
        [JsonProperty("content")]
        /*To-Do: Develop a JSON Schema for the content to have bold, italics, and images */
        public string Content { get; set; }

        /// <summary>
        /// Comments in the post
        /// </summary>
        [JsonProperty("comments")]
        [Required]
        public ICollection<Comment> Comments { get; set; }
        /// <summary>
        /// Comments in the post
        /// </summary>
        [JsonProperty("likedBy")]
        [Required]
        public ICollection<User> LikedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
