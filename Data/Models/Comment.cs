using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AskMe.Data.BaseEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AskMe.Data.Models
{
    public class Comment : Entity
    {
        public Guid? PostId { get; set; }
        public Post Post { get; set; }
        public Guid? CommentId { get; set; }
        public Comment PrevComment { get; set; }
        public Boolean Pinned { get; set; }
        public ICollection<Comment> Replies { get; set; }
        public ICollection<User> LikedBy { get; set; }
    }
}
