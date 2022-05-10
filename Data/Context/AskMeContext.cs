using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Http;
using AskMe.Data.Context;
using AskMe.Data.Models;
using AskMe.Data.Configurations;

namespace AskMe.Data.Context
{
    public class AskMeContext : DbContext
    {
        public readonly IServiceProvider serviceProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private IDbContextTransaction transaction;

        public AskMeContext(DbContextOptions<AskMeContext> opt, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) : base(opt)
        {
            this.serviceProvider = serviceProvider;
            this.httpContextAccessor = httpContextAccessor;
            if (httpContextAccessor.HttpContext != null)
            {
                BeginTransaction();
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        private void BeginTransaction()
        {
            transaction = Database.CurrentTransaction ?? Database.BeginTransaction();
        }
        private void CommitChanges()
        {
            try
            {
                transaction?.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw new Exception("Error while saving transactions to the database");
            }
        }
        public override int SaveChanges()
        {
            var numberOfEntries = base.SaveChanges();
            CommitChanges();
            return numberOfEntries;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}