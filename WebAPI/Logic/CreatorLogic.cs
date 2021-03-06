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
        public async Task<ProfileDTO> GetProfile(Guid userId, Guid creatorId)
        {
            var creator = await _repositoryManager.UserRepository.GetUserById(creatorId);
            if (creator == null)
            {
                Console.WriteLine("user was null");
            }
            var profile = new ProfileDTO
            {
                Avatar = creator.Avatar ?? "",
                Name = creator.FirstName + " " + creator.LastName,
                Bio = creator.Bio,
                CreatedAt = creator.CreatedAt,
                Email = creator.Email,
                Followed = userId == creator.Id || creator.Followers.Any(user => user.Id == userId),
                Followers = creator.Followers.Count(),
                Following = creator.Following.Count()
            };
            return profile;
        }
        public async Task<PaginationResult<PostItemDTO>> GetPosts(Guid creatorId, Guid userId, int page, int pageSize)
        {
            var user = await _repositoryManager.UserRepository.GetUserById(creatorId);
            var posts = _repositoryManager.PostRepository.GetPosts()
                            .Where(post => post.CreatedById == creatorId)
                            .Select(post => new PostItemDTO
                            {
                                Id = post.Id,
                                ImageRef = post.ImageRef ?? null,
                                Title = post.Title,
                                Likes = post.LikedBy.Count(),
                                Liked = post.LikedBy.Any(liker => liker.Id == userId),
                                Content = post.Content,
                                CreatedAt = post.CreatedAt,
                                User = new PostProfileItemDTO { Id = user.Id, Name = user.FirstName + " " + user.LastName }
                            });
            return posts.ToPagination(page, pageSize);
        }
        public async Task<ICollection<ProfileItemDTO>> GetFollowers(Guid userId, Guid creatorId)
        {
            var creator = await _repositoryManager.UserRepository.GetFullUserById(creatorId);
            Console.WriteLine(userId);
            Console.WriteLine(creatorId);
            Console.WriteLine(creator);
            if (creator == null)
            {
                Console.WriteLine("user was null");

            }
            Console.WriteLine(creator.Followers.Count());
            var followers = creator.Followers.ToList().Select(
                follower => new ProfileItemDTO
                {
                    Avatar = follower.Avatar ?? "",
                    Id = follower.Id,
                    Name = follower.FirstName + " " + follower.LastName,
                    Followers = follower.Followers.Count(),
                    Following = follower.Following.Count(),
                    Followed = follower.Followers.Any(followed => followed.Id == userId)
                }).ToList();
            return followers;
        }
        public async Task<ICollection<ProfileItemDTO>> GetFollowing(Guid userId, Guid creatorId)
        {
            var creator = await _repositoryManager.UserRepository.GetFullUserById(creatorId);
            var followings = creator.Following.Select(
                following => new ProfileItemDTO
                {
                    Avatar = following.Avatar,
                    Id = following.Id,
                    Name = following.FirstName + " " + following.LastName,
                    Followers = following.Followers.Count(),
                    Following = following.Following.Count(),
                    Followed = following.Followers.Any(followed => followed.Id == userId)
                }).ToList();
            return followings;
        }

    }
}
