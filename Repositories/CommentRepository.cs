using System;
using System.Threading.Tasks;
using AskMe.Data.Context;
using AskMe.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AskMe.Repositories
{
    public class CommentRepository
    {
        private readonly AskMeContext _context;
        public CommentRepository(AskMeContext context)
        {
            _context = context;
        }

        public IQueryable<Comment> GetComments()
        {
            return _context.Comments.Include(comment => comment.CreatedBy).Where(comment => comment.DeletedAt == null);
        }

        public async Task<Comment> GetCommentById(Guid id)
        {
            return await _context.Comments
                        .Where(comment => comment.Id.Equals(id))
                        .Include(comment => comment.PrevComment)
                        .Include(comment => comment.Post)
                        .Include(commment => commment.CreatedBy).FirstOrDefaultAsync();
        }
        public async Task<Comment> CreateComment(Comment comment)
        {
            var createdComment = await _context.Comments.AddAsync(comment);
            return createdComment.Entity;
        }
        public Comment UpdateComment(Comment comment)
        {
            var createdComment = _context.Comments.Update(comment);
            return createdComment.Entity;
        }
    }
}