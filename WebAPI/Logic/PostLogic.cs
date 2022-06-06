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
    public class PostLogic
    {
        private readonly RepositoryManager _repositoryManager;

        public PostLogic(RepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }


        public async Task<PostDTO> CreatePost(PostCreateDTO post, Guid userId)
        {
            // To-Do validate user exists- 
            //To-Do add image upload functionality
            var newPost = new Post
            {
                Content = post.Content,
                Title = post.Title,
                CreatedById = userId,
                DeletedAt = null,
                CreatedAt = DateTime.Now
            };
            // await _repositoryManager.GoalRepository.Create(goal);
            var createdPost = await _repositoryManager.PostRepository.CreatePost(newPost);
            _repositoryManager.Save();
            var detailPost = new PostDTO
            {
                Id = createdPost.Id,
                Content = createdPost.Content,
                Title = createdPost.Title,
                CreatedById = createdPost.CreatedById ?? new Guid(),
                CreatedAt = createdPost.CreatedAt
            };
            return detailPost;

        }
        public async Task<PostDTO> UpdatePost(PostCreateDTO newPost, Guid postId)
        {
            // To-Do validate user exists, and is owner of the post
            var oldPost = await _repositoryManager.PostRepository.GetPostById(postId);
            oldPost.Title = newPost.Title;
            oldPost.Content = newPost.Content;
            //To-Do add image upload functionality
            var updatedPost = _repositoryManager.PostRepository.UpdatePost(oldPost);
            _repositoryManager.Save();
            var detailPost = new PostDTO
            {
                Id = updatedPost.Id,
                Content = updatedPost.Content,
                Title = updatedPost.Title,
                CreatedById = updatedPost.CreatedById ?? new Guid(),
                CreatedAt = updatedPost.CreatedAt
            };
            return detailPost;
        }
        public async Task<PostDTO> GetPost(Guid postId)
        {
            // To-Do validate user exists, and is owner of the post
            var post = await _repositoryManager.PostRepository.GetPostById(postId);
            var detailPost = new PostDTO
            {
                Id = post.Id,
                Content = post.Content,
                Title = post.Title,
                CreatedById = post.CreatedById ?? new Guid(),
                Likes = post.LikedBy.Count(),
                Comments = post.Comments.Select(comment => new CommentItemDTO { Content = comment.Content, Id = comment.Id }).ToList(),
                CreatedAt = post.CreatedAt,
                LikedByPrev = post.LikedBy.Select(user => new UserItemDTO
                {
                    Id = user.Id,
                    Name = user.FirstName + " " + user.LastName
                }).ToList()

            };
            return detailPost;
        }
        public PaginationResult<PostItemDTO> GetPosts(Guid userId, int page, int pageSize)
        {
            var posts = _repositoryManager.PostRepository.GetPosts()
                            .Where(post => post.CreatedById == userId)
                            .Select(post => new PostItemDTO
                            {
                                Id = post.Id,
                                ImageRef = post.ImageRef ?? null,
                                Title = post.Title,
                                Likes = post.LikedBy.Count(),
                                Content = post.Content,
                                CreatedAt = post.CreatedAt
                            });
            return posts.ToPagination(page, pageSize);
        }
        public async Task<PaginationResult<PostItemDTO>> GetDashboardPosts(Guid userId, int page, int pageSize)
        {
            var user = await _repositoryManager.UserRepository.GetUserById(userId);
            var followingIds = user.Following.Select(following => following.Id).ToList();
            followingIds.Add(userId);
            var posts = _repositoryManager.PostRepository.GetPosts()
                            .Where(post => post.CreatedById != null && followingIds.Contains((Guid)post.CreatedById))
                            .OrderByDescending(post => post.CreatedAt)
                            .Select(post => new PostItemDTO
                            {
                                Id = post.Id,
                                ImageRef = post.ImageRef ?? null,
                                Title = post.Title,
                                Likes = post.LikedBy.Count(),
                                Content = post.Content,
                                CreatedAt = post.CreatedAt
                            });
            return posts.ToPagination(page, pageSize);
        }
        public async Task LikePost(Guid postId, Guid userId)
        {
            var post = await _repositoryManager.PostRepository.GetPostById(postId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);

            if (post.LikedBy == null)
            {
                post.LikedBy = new List<User>();
            }
            post.LikedBy.Add(user);
            _repositoryManager.PostRepository.UpdatePost(post);
            _repositoryManager.Save();
        }
        public async Task CommentPost(CommentCreateDTO comment, Guid postId, Guid userId)
        {
            var post = await _repositoryManager.PostRepository.GetPostById(postId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);

            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }
            var newComment = new Comment
            {
                DeletedAt = null,
                PostId = post.Id,
                Pinned = false,
                Content = comment.Content,
                CreatedById = userId
            };
            post.Comments.Add(newComment);
            _repositoryManager.PostRepository.UpdatePost(post);
            _repositoryManager.Save();
        }
    }
}
