using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskMe.Data.DTO;
using AskMe.Data.Models;
using AskMe.Repositories.Manager;

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
                DeletedAt = null
            };
            // await _repositoryManager.GoalRepository.Create(goal);
            var createdPost = await _repositoryManager.PostRepository.CreatePost(newPost);
            _repositoryManager.Save();
            var detailPost = new PostDTO
            {
                Id = createdPost.Id,
                Content = createdPost.Content,
                Title = createdPost.Title,
                CreatedById = createdPost.CreatedById ?? new Guid()
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
                CreatedById = updatedPost.CreatedById ?? new Guid()
            };
            return detailPost;
        }
    }
}
