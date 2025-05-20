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

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<ContactU> ContactUs { get; set; }

    public virtual DbSet<Evaluation> Evaluations { get; set; }

    public virtual DbSet<LocationArea> LocationAreas { get; set; }

    public virtual DbSet<MainService> MainServices { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceWorkersJunctionTable> ServiceWorkersJunctionTables { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<WorkerAchievement> WorkerAchievements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-T6EH1VU;Database=Home_Business_Services_Managment_Database;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.AchievementId).HasName("PK__Achievem__276330E040D2D095");

            entity.Property(e => e.AchievementId).HasColumnName("AchievementID");
            entity.Property(e => e.AchievementDescription)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.AchievementName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.AchievementPatchImage).HasColumnName("Achievement_Patch_Image");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD10F3B377");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Booking_End_Date");
            entity.Property(e => e.BookingMessae)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.BookingNotes)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.BookingStartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Booking_Start_Date");
            entity.Property(e => e.BookingTittle)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkerId).HasColumnName("WorkerID");

            entity.HasOne(d => d.Service).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__Bookings__Servic__5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserID__59FA5E80");

            entity.HasOne(d => d.Worker).WithMany(p => p.BookingWorkers)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bookings_Worker");
        });

        modelBuilder.Entity<ContactU>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__ContactU__5C6625BB0F0CAB18");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.ContactUsMessage)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("ContactUs_Message");
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
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__Evaluati__36AE68D3523511A7");

            entity.Property(e => e.EvaluationId).HasColumnName("EvaluationID");
            entity.Property(e => e.Comments)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.WrokerId).HasColumnName("WrokerID");

            entity.HasOne(d => d.Wroker).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.WrokerId)
                .HasConstraintName("FK__Evaluatio__Wroke__6EF57B66");
        });

        modelBuilder.Entity<LocationArea>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__D2BA00E2431204B9");

            entity.ToTable("Location_Areas");

            entity.HasIndex(e => e.AreasCovered, "UQ_Areas_Covered").IsUnique();

            entity.HasIndex(e => e.ManagerId, "UQ__Location__3BA2AA80387B3FA5").IsUnique();

            entity.Property(e => e.LocationId).HasColumnName("Location_Id");
            entity.Property(e => e.AreasCovered)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Areas_Covered");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");

            entity.HasOne(d => d.Manager).WithOne(p => p.LocationArea)
                .HasForeignKey<LocationArea>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Location___Manag__4316F928");
        });

        modelBuilder.Entity<MainService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Main_Ser__C51BB0EA15E971A5");

            entity.ToTable("Main_Service");

            entity.HasIndex(e => e.ServiceName, "UQ__Main_Ser__A42B5F9901EA47A4").IsUnique();

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ServicePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Service_Price");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC279ACF774D");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.BookingId, "UQ__Payment__73951ACCB946B915").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.Cvc)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("CVC");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Payment_Method");

            entity.HasOne(d => d.Booking).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Booking__5EBF139D");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE7D8ACC3E");

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
                .HasConstraintName("FK__Reviews__Booking__6383C8BA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A2B25029C");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ServiceWorkersJunctionTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Service___3214EC27460D9A5B");

            entity.ToTable("Service_Workers_JunctionTable");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.WrokerId).HasColumnName("WrokerID");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceWorkersJunctionTables)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Service_W__Servi__5535A963");

            entity.HasOne(d => d.Wroker).WithMany(p => p.ServiceWorkersJunctionTables)
                .HasForeignKey(d => d.WrokerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Service_W__Wroke__5441852A");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949D17D93C7D0");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
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
            entity.Property(e => e.WrokerId).HasColumnName("WrokerID");

            entity.HasOne(d => d.Wroker).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.WrokerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Tasks__WrokerID__68487DD7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC2362718C");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053453A783AD").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
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
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LocationId).HasColumnName("Location_Id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PersonalAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Personal_Address");
            entity.Property(e => e.PersonalImage).HasColumnName("Personal_Image");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WorkerIntro)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("Worker__Intro");
            entity.Property(e => e.WorkerRating).HasColumnName("Worker__Rating");
            entity.Property(e => e.WorkerServiceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Worker_Service_Type");

            entity.HasOne(d => d.Location).WithMany(p => p.Users)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Users__Location___440B1D61");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserRoles_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__UserRoles__RoleI__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__UserRoles__UserI__3E52440B");
        });

        modelBuilder.Entity<WorkerAchievement>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.AchievementId).HasColumnName("AchievementID");
            entity.Property(e => e.WorkerId).HasColumnName("WorkerID");

            entity.HasOne(d => d.Achievement).WithMany()
                .HasForeignKey(d => d.AchievementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkerAch__Achie__4CA06362");

            entity.HasOne(d => d.Worker).WithMany()
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkerAch__Worke__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
