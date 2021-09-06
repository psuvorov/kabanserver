using System;
using System.Linq;
using System.Security.Claims;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Kaban.Domain.Models.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Kaban.Database
{
    public class DataContext : DbContext
    {
        private readonly IStoredUser _storedUser;

        public DataContext(DbContextOptions options, IStoredUser storedUser) : base(options)
        {
            _storedUser = storedUser;
        }
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<Board> Boards { get; set; }
        
        public DbSet<Card> Cards { get; set; }
        
        public DbSet<List> Lists { get; set; }

        public DbSet<CardComment> CardComments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            // options.UseSqlServer(_configuration.GetConnectionString("Kaban"));
        }

        public override int SaveChanges()
        {
            var userId = _storedUser.GetUserId();
            
            var storedUser = Users.Find(userId);
            
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                var now = DateTime.UtcNow;
                
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = storedUser;
                        entry.Entity.Created = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = storedUser;
                        entry.Entity.LastModified = now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted))
            {
                if (entry.Entity is ICanBeDeleted entity)
                {
                    entry.State = EntityState.Unchanged;
                    entity.IsDeleted = true;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Board>().ToTable("Boards").HasKey(x => x.Id);
            modelBuilder.Entity<List>().ToTable("Lists").HasKey(x => x.Id);
            modelBuilder.Entity<Card>().ToTable("Cards").HasKey(x => x.Id);
            modelBuilder.Entity<CardComment>().ToTable("CardComments").HasKey(x => x.Id);
            modelBuilder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
            
            
            
            // modelBuilder.Entity<Card>()
            //     .HasOne<List>(x => x.List)
            //     .WithMany()
            //     .HasForeignKey(x => x.ListId);
            //
            // modelBuilder.Entity<List>()
            //     .HasOne<Board>(x => x.Board)
            //     .WithMany()
            //     .HasForeignKey(x => x.BoardId);
            
            
            
            // modelBuilder.Entity<Board>()
            //     .HasMany<List>()
            //     .WithOne(l => l.Board)
            //     .HasForeignKey(l => l.BoardId);
            //
            // modelBuilder.Entity<List>()
            //     .HasMany<Card>()
            //     .WithOne(c => c.List)
            //     .HasForeignKey(c => c.ListId);
            //
            // modelBuilder.Entity<Card>()
            //     .HasMany<CardComment>()
            //     .WithOne(cc => cc.Card)
            //     .HasForeignKey(cc => cc.CardId);
            
            modelBuilder.Entity<Board>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<List>().HasQueryFilter(x => !x.IsDeleted && !x.IsArchived);
            modelBuilder.Entity<Card>().HasQueryFilter(x => !x.IsDeleted && !x.IsArchived);
            modelBuilder.Entity<CardComment>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            
            base.OnModelCreating(modelBuilder);
        }

    }
}