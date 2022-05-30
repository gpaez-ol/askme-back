using System;
using System.Threading.Tasks;
using AskMe.Data.Context;
using AskMe.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AskMe.Repositories
{
    public class CreatorRepository
    {
        private readonly AskMeContext _context;
        public CreatorRepository(AskMeContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetCreators()
        {
            return _context.Users.Where(creator => creator.DeletedAt == null && creator.Type == UserType.Creator).Include(creator => creator.Followers);
        }

        public async Task<User> GetCreatorById(Guid id)
        {
            return await _context.Users.Where(creator => creator.Id.Equals(id)).FirstOrDefaultAsync();
        }
        public User UpdateCreator(User user)
        {
            var updatedUser = _context.Users.Update(user);
            return updatedUser.Entity;
        }
    }
}