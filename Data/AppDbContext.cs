using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Post>()
                .HasOne(p => p.ParentPost)
                .WithMany(p => p.CommentsList)
                .HasForeignKey(p => p.ParentPostId)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany() // Assuming a User can have many Likes
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Or Restrict if you don't want cascading deletes

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes) // Assuming a Post can have many Likes
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Or Restrict depending on your needs
    
        }
    }
}