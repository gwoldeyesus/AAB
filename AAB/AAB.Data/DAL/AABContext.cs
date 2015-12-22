using AAB.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace AAB.Data.DAL
{
    public class AABContext : DbContext
    {
        public AABContext() : base("AABContext")
        {
        }

        public IDbSet<Genre> Genres { get; set; }
        public IDbSet<Book> Books { get; set; }
        //public IDbSet<BookGenre> BookGenres { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Book>()
                .HasMany(g => g.Genres)
                .WithMany(bk => bk.Books)
                .Map(m =>
                {
                    m.ToTable("BookGenre");
                    m.MapLeftKey("BookId");
                    m.MapRightKey("GenreId");
                });

            modelBuilder.Entity<Genre>().MapToStoredProcedures();
            modelBuilder.Entity<Book>().MapToStoredProcedures();
        }
    }
}