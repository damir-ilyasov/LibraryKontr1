using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfApp2
{
    class ApplicationContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<BookGenre> BookGenres { get; set; } = null!;
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=library;Username=postgres;Password=Shpak2006");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId);

            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.books)
                .HasForeignKey(b => b.GenreId)
                .HasPrincipalKey(g => g.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.books)
                .HasForeignKey(b => b.AuthorId)
                .HasPrincipalKey(a => a.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>(p =>
            {
                p.Property(d => d.Title).IsRequired().HasMaxLength(50);
                p.Property(d => d.PublishYear).HasMaxLength(4);
                p.Property(d => d.ISBN).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<Genre>(p =>
            {
                p.Property(d => d.Name).IsRequired().HasMaxLength(50);
                p.Property(d => d.Description).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<Author>(p =>
            {
                p.Property(d => d.FirstName).IsRequired().HasMaxLength(50);
                p.Property(d => d.LastName).HasMaxLength(50);
                p.Property(d => d.BirthDate).HasMaxLength(25);
                p.Property(d => d.Country).HasMaxLength(25);
            });
        }
    }
}
