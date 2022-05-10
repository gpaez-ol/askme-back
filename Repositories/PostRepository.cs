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
using AskMe.Data.DTO;

namespace AskMe.Repositories
{
    public class PostRepository
    {
        private readonly AskMeContext _context;
        public PostRepository(AskMeContext context)
        {
            _context = context;
        }

        public IQueryable<Post> GetPosts()
        {
            return _context.Posts.Where(post => post.DeletedAt == null);
        }

        public async Task<Post> GetPostById(Guid id)
        {
            return await _context.Posts.Where(post => post.Id.Equals(id)).FirstOrDefaultAsync();
        }
        public async Task<Post> CreatePost(Post post)
        {
            var createdPost = await _context.Posts.AddAsync(post);
            return createdPost.Entity;
        }
        public Post UpdatePost(Post post)
        {
            var createdPost = _context.Posts.Update(post);
            return createdPost.Entity;
        }
    }
}