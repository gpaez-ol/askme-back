
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AskMe.Data.Models;
using Newtonsoft.Json;

namespace AskMe.Data.DTO
{
    public class CommentCreateDTO
    {

        /// <summary> 
        /// Content of the Post
        /// </summary>
        [Required]
        [JsonProperty("content")]
        /*To-Do: Develop a JSON Schema for the content to have bold, italics, and images */
        public string Content { get; set; }
    }
    public class CommentItemDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
    }

}
