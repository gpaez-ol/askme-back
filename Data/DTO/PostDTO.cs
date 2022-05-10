
using System;
using System.ComponentModel.DataAnnotations;
using AskMe.Data.Models;
using Newtonsoft.Json;

namespace AskMe.Data.DTO
{
    public class PostCreateDTO
    {
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
    }
    public class PostDTO
    {
        public string? ImageRef { get; set; }
        public Guid Id { get; set; }
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
        public Guid CreatedById { get; set; }
    }

}
