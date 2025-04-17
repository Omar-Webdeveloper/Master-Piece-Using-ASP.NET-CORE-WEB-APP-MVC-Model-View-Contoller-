using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Master_Piece.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Evaluation> Evaluations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }

    public virtual DbSet<ServiceWorkersJunctionTable> ServiceWorkersJunctionTables { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-T6EH1VU;Database=Home_Business_Services_Managment_Database;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD2DC98A2A");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.BookingMessae)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.BookingNotes)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.BookingTittle)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Bookings__Servic__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserID__0E6E26BF");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__ContactU__5C6625BBF8DF04DC");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Message)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ContactUs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ContactUs__UserI__6B24EA82");
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__Evaluati__36AE68D3DF919A4E");

            entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.Comments)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.HasOne(d => d.Admin).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__Evaluatio__Admin__09A971A2");

            entity.HasOne(d => d.Provider).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK__Evaluatio__Provi__08B54D69");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE342CE205");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.Comment)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReviewStatus)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Review_Status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Reviews__Booking__123EB7A3");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA68B60779");

            entity.HasIndex(e => e.ServiceName, "UQ__Services__A42B5F994862E1BB").IsUnique();

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StartingPrices)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Starting_Prices");
        });

        modelBuilder.Entity<ServiceProvider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK__ServiceP__B54C689D6D451BEB");

            entity.HasIndex(e => e.UserId, "UQ__ServiceP__1788CCAD123FF72D").IsUnique();

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.Achievements)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Intro)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Photos)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.RegisterAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Register_at");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkerAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Worker_Address");
            entity.Property(e => e.WorkerName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.ServiceProvider)
                .HasForeignKey<ServiceProvider>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ServicePr__UserI__3C69FB99");
        });

        modelBuilder.Entity<ServiceWorkersJunctionTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("service_Workers_JunctionTable_");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Provider).WithMany()
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__service_W__Provi__4F7CD00D");

            entity.HasOne(d => d.Service).WithMany()
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__service_W__Servi__5070F446");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949D140809B41");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.AfterPhoto)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.BeforePhoto)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.TaskName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TaskStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("TO DO");
            entity.Property(e => e.TasksDetails)
                .HasMaxLength(200)
                .HasDefaultValue("waiting");

            entity.HasOne(d => d.Provider).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Tasks__ProviderI__4D94879B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC9873536A");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
