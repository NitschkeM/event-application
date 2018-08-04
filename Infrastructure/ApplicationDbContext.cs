using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using $safeprojectname$.Models;
using System.Web;
using $safeprojectname$.Migrations;

namespace $safeprojectname$.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {


        //public ApplicationDbContext() : base("name=EventAuthDB")
        //My databaseContext's constructor.
        //public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        public ApplicationDbContext() : base("My_Event_AppDb", throwIfV1Schema: false)
        {
            // log queries in debug output
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
            Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }

        // "Indirect" access to data through DbSets
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        // This method is how instances of the class will be created when needed by the OWIN,
        // using the Startup class in Startup.Auth.cs in App_Start
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Modifying codefirst behaviour with the fluid API
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Many-To-Many: Event-Participants
            modelBuilder.Entity<Event>()
                .HasMany<ApplicationUser>(s => s.Participants)
                .WithMany(c => c.ParticipantIn)
                .Map(cs =>
                {
                    cs.MapLeftKey("EventId");
                    cs.MapRightKey("UserId");
                    cs.ToTable("ParticipantEvent");
                });

            // Many-To-Many: Pending Event-Participants
            modelBuilder.Entity<Event>()
                .HasMany<ApplicationUser>(s => s.Pending)
                .WithMany(c => c.PendingFor)
                .Map(cs =>
                {
                    cs.MapLeftKey("EventId");
                    cs.MapRightKey("UserId");
                    cs.ToTable("PendingParticipants");
                });

            //// Make Foreign keys nullable (Image-User and Image-Event)
            //modelBuilder.Entity<Image>()
            //    .HasOptional<ApplicationUser>(i => i.ApplicationUser);
            //modelBuilder.Entity<Image>()
            //    .HasOptional<Event>(i => i.Event);


            //// Make Foreign key nullable (Comment - ParentId)
            //modelBuilder.Entity<Comment>()
            //    .HasOptional<Comment>(c => c.Parent)
            //    .WithMany()
            //    .HasForeignKey(c => c.ParentId);

            //modelBuilder.Entity<Comment>()
            //    .Property(c => c.ParentId).IsOptional();

            modelBuilder.Entity<Comment>()
                .HasMany(p => p.Replies)
                .WithOptional(c => c.Parent)
                .HasForeignKey(c => c.ParentId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

// Julie Lerman, microsoft nov2014
// You can: SetDatabaseInitializer<MyContext>(null)



// tutorialspoint.com: 
// To handle the property migration you need to set AutomaticMigrationDataLossAllowed = true in the configuration class constructor.
// ***But this seems to be for automatic***
//
// public Configuration()
// {
//     AutomaticMigrationsEnabled = true;
//     AutomaticMigrationDataLossAllowed = true;
//     ContextKey = "EFCodeFirstDemo.MyContext";
// }