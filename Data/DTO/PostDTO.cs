
using System;
using System.Collections.Generic;
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
    public class UserItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
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
        public int Likes { get; set; }
        public ICollection<UserItemDTO>? LikedByPrev { get; set; }
        public ICollection<CommentItemDTO>? Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class PostItemDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("imageRef")]
        public string? ImageRef { get; set; }
        /// <summary> 
        /// Title of the post
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("likes")]
        public int Likes { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
    }

}
