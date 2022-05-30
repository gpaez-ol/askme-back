using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Askme.Pagination;
using AskMe.Data.DTO;
using AskMe.Data.Models;
using AskMe.Repositories.Manager;
using AsKMe.Pagination;

namespace AskMe.Logic
{
    public class CreatorLogic
    {
        private readonly RepositoryManager _repositoryManager;

        public CreatorLogic(RepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<PaginationResult<CreatorItemDTO>> GetCreators(Guid userId, int page, int pageSize)
        {
            var user = await _repositoryManager.UserRepository.GetUserById(userId);
            var creators = _repositoryManager.CreatorRepository.GetCreators().Select(
                creator => new CreatorItemDTO
                {
                    Avatar = creator.Avatar,
                    Id = creator.Id,
                    Name = creator.FirstName + " " + creator.LastName,
                    Followers = creator.Followers.Count()
                });
            var paginatedCreators = creators.ToPagination(page, pageSize);
            paginatedCreators.Content.ToList().ForEach(
                 creator =>
                 {

                     creator.Followed = user.Following.Any(followed => followed.Id == creator.Id);
                 }
             );
            return paginatedCreators;
        }
        public async Task FollowCreator(Guid creatorId, Guid userId)
        {
            var creator = await _repositoryManager.CreatorRepository.GetCreatorById(creatorId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);
            if (creator.Followers == null)
            {
                creator.Followers = new List<User>();
            }
            if (creator.Followers.All(user => user.Id != userId))
                creator.Followers.Add(user);
            if (user.Following.All(user => user.Id != creatorId))
                user.Following.Add(creator);
            _repositoryManager.CreatorRepository.UpdateCreator(creator);
            _repositoryManager.UserRepository.UpdateUser(user);
            _repositoryManager.Save();
        }
        public async Task UnFollowCreator(Guid creatorId, Guid userId)
        {
            var creator = await _repositoryManager.CreatorRepository.GetCreatorById(creatorId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);
            if (creator.Followers != null && creator.Followers.Any(user => user.Id == userId))
                creator.Followers = creator.Followers.Where(follower => follower.Id != userId).ToList();
            if (user.Following != null && user.Following.Any(user => user.Id == creatorId))
                user.Following = user.Following.Where(following => following.Id != creatorId).ToList();
            _repositoryManager.CreatorRepository.UpdateCreator(creator);
            _repositoryManager.UserRepository.UpdateUser(user);
            _repositoryManager.Save();
        }

    }
}
