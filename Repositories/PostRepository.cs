using System;
using System.Threading.Tasks;
using AskMe.Data.Context;
using AskMe.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            return _context.Posts.Include(post => post.Comments).Include(post => post.LikedBy).Include(post => post.CreatedBy).Where(post => post.DeletedAt == null);
        }

        public async Task<Post> GetPostById(Guid id)
        {
            return await _context.Posts
                .Where(post => post.Id.Equals(id))
                .Include(post => post.Comments)
                .ThenInclude(comment => comment.CreatedBy)
                .Include(post => post.LikedBy).FirstOrDefaultAsync();
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