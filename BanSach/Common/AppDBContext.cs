using Microsoft.EntityFrameworkCore;
using BanSach.Book;
using BookEntity = BanSach.Book.Book;

namespace BanSach.Common
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Table duy nhất hiện tại
        public DbSet<BookEntity> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ví dụ constraint (optional)
            modelBuilder.Entity<BookEntity>()
                .Property(b => b.Title)
                .IsRequired();
        }
    }
}