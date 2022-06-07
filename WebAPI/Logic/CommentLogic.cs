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
    public class CommentLogic
    {
        private readonly RepositoryManager _repositoryManager;

        public CommentLogic(RepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }


        public async Task<CommentItemDTO> UpdateComment(CommentCreateDTO newComment, Guid commentId)
        {
            // To-Do validate user exists, and is owner of the comment
            var oldComment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            oldComment.Content = newComment.Content;
            //To-Do add image upload functionality
            var updatedComment = _repositoryManager.CommentRepository.UpdateComment(oldComment);
            _repositoryManager.Save();
            var detailComment = new CommentItemDTO
            {
                Id = oldComment.Id,
                Content = oldComment.Content
            };
            return detailComment;
        }
        public PaginationResult<CommentItemDTO> GetCommentsOnPost(Guid postId, int page, int pageSize)
        {
            var posts = _repositoryManager.CommentRepository.GetComments().Where(comment => comment.PostId != null && comment.PostId == postId)
                            .OrderByDescending(comment => comment.Pinned)
                            .ThenByDescending(comment => comment.CreatedAt)
                            .Select(comment => new CommentItemDTO
                            {
                                Id = comment.Id,
                                Content = comment.Content,
                                Pinned = comment.Pinned,
                                CreatedBy = comment.CreatedBy,
                                CreatedAt = comment.CreatedAt
                            });
            return posts.ToPagination(page, pageSize);
        }
        public PaginationResult<CommentItemDTO> GetCommentsOnComment(Guid commentId, int page, int pageSize)
        {
            var posts = _repositoryManager.CommentRepository.GetComments().Where(comment => comment.CommentId != null && comment.CommentId == commentId)
                            .OrderByDescending(comment => comment.Pinned)
                            .ThenByDescending(comment => comment.CreatedAt)
                            .Select(comment => new CommentItemDTO
                            {
                                Id = comment.Id,
                                Content = comment.Content,
                                Pinned = comment.Pinned,
                                CreatedBy = comment.CreatedBy,
                                CreatedAt = comment.CreatedAt
                            });
            return posts.ToPagination(page, pageSize);
        }
        public async Task PinComment(Guid commentId, Guid userId)
        {
            var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            if (comment.PostId != null && comment.PostId != default(Guid))
            {
                var post = await _repositoryManager.PostRepository.GetPostById(comment.PostId.GetValueOrDefault());
                if (post != null && post != default(Post) && post.CreatedById == userId)
                {
                    comment.Pinned = true;
                }
                _repositoryManager.CommentRepository.UpdateComment(comment);
                _repositoryManager.Save();
            }
        }
        public async Task UnpinComment(Guid commentId, Guid userId)
        {
            var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            if (comment.PostId != null && comment.PostId != default(Guid))
            {
                var post = await _repositoryManager.PostRepository.GetPostById(comment.PostId.GetValueOrDefault());
                if (post != null && post != default(Post) && post.CreatedById == userId)
                {
                    comment.Pinned = false;
                }
                _repositoryManager.CommentRepository.UpdateComment(comment);
                _repositoryManager.Save();
            }
        }
        public async Task LikeComment(Guid commentId, Guid userId)
        {
            var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);

            if (comment.LikedBy == null)
            {
                comment.LikedBy = new List<User>();
            }
            comment.LikedBy.Add(user);
            _repositoryManager.CommentRepository.UpdateComment(comment);
            _repositoryManager.Save();
        }
        public async Task DeleteComment(Guid commentId)
        {
            var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            comment.DeletedAt = DateTime.Now;
            _repositoryManager.CommentRepository.UpdateComment(comment);
            _repositoryManager.Save();
        }
        public async Task CommentComment(CommentCreateDTO commentDTO, Guid commentId, Guid userId)
        {
            var comment = await _repositoryManager.CommentRepository.GetCommentById(commentId);
            var user = await _repositoryManager.UserRepository.GetUserById(userId);

            var newComment = new Comment
            {
                DeletedAt = null,
                Pinned = false,
                Content = commentDTO.Content,
                CommentId = comment.Id,
                PrevComment = comment,
                CreatedById = userId,
                CreatedAt = DateTime.Now
            };
            if (comment.Replies == null)
            {
                comment.Replies = new List<Comment>();
            }
            comment.Replies.Add(newComment);
            _repositoryManager.CommentRepository.UpdateComment(comment);
            _repositoryManager.Save();
        }
    }
}
