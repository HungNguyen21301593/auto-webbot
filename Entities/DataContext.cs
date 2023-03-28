using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Logging;

namespace Entities
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public DbSet<DeviceRegistration> DeviceRegistrations { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<StepLog> StepLog { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>().ToTable("Request");
            modelBuilder.Entity<Post>().ToTable("Post");
            modelBuilder.Entity<StepLog>().ToTable("StepHistory");
            modelBuilder.Entity<Setting>().ToTable("Setting");
            modelBuilder.Entity<DeviceRegistration>().ToTable("DeviceRegistrations");
        }
    }

    public class DeviceRegistration: CreateAbstract
    {
        public Guid Id { get; set; }
        public int RemainingPostLimit { get; set; }
        public string? UserName { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime Created { get; set; }
    }

    public class Request
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
    }

    public class Setting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public long SleepInterval { get; set; }
        public long ReadInterval { get; set; }
        public long AdActiveInterval { get; set; }
        public long WaitAfterDeleteInterval { get; set; }
        public long RePostInterval { get; set; }

        public long PageToTrigger { get; set; }
        public string KijijiEmail { get; set; }
        public string KijijiPassword { get; set; }
        public Guid RegistrationId { get; set; }
    }

    public class Post: CreateAbstract
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Column(TypeName = "varchar(24)")]
        public AdStatus Status { get; set; }
        public string AdDetailJson { get; set; }
        public ICollection<StepLog> stepLogs { get; set; }
    }

    public enum AdStatus
    {
        New,
        Started,
        ReadSucceeded,
        ReadFailed,
        DeleteSucceeded,
        DeleteFailed,
        ValidateSucceeded,
        ValidateFailed,
        PostSucceeded,
        PostedFailed
    }

    public class CreateAbstract {
        public CreateAbstract()
        {
            Created = DateTime.UtcNow;
        }
        public DateTime Created { get; set; }
    }
}