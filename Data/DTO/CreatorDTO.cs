
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AskMe.Data.Models;
using Newtonsoft.Json;

namespace AskMe.Data.DTO
{
    public class CreatorItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool Followed { get; set; }
        public int Followers { get; set; }
        public int Posts { get; set; }
    }

}
