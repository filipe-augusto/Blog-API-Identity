using Blog.Data.Mappings;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContext : DbContext
    {
        public BlogDataContext(DbContextOptions<BlogDataContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

      //  protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlServer(@"Data Source=DESKTOP-MCT2S1O;Integrated Security=True;Connect Timeout=30;Initial Catalog=Blog; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}
   // public class BlogDataContext : DbContext
    //{


    //    public BlogDataContext(DbContextOptions<BlogDataContext> options) : base(options) { }


    //    public DbSet<Category> Categories { get; set; }
    //    public DbSet<Post> Posts { get; set; }
    //    // public DbSet<PostTag> PostTags { get; set; }
    //    // public DbSet<Role> Roles { get; set; }
    //    // public DbSet<Tag> Tags { get; set; }
    //    public DbSet<User> Users { get; set; }
    //    // public DbSet<UserRole> UserRoles { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder options)
    //                          //=> options.UseSqlServer("Server=localhost,1433;Database=Blog;User ID=sa;Password=1q2w3e4r@#$");
    //                          //=> options.UseSqlServer(@"Data Source=DESKTOP-3MTK0H8;Integrated Security=True;Connect Timeout=30;Initial Catalog=Blog; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
    //                          => options.UseSqlServer("Data Source=DESKTOP-MCT2S1O;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.ApplyConfiguration(new CategoryMap());
    //        modelBuilder.ApplyConfiguration(new UserMap());
    //        modelBuilder.ApplyConfiguration(new PostMap());
    //    }

    //    //dotnet ef migrations add createdatabase
    //    //dotnet ef database update
    //}
//}
