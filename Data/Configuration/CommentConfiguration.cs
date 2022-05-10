using AskMe.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AskMe.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasMany(p => p.Replies).WithOne(p => p.PrevComment);
            builder.HasOne(p => p.Post).WithMany(p => p.Comments);
            builder.HasMany(p => p.LikedBy);
        }
    }
}