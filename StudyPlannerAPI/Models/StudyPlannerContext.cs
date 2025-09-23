using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudyPlannerAPI.Models;

public partial class StudyPlannerContext : DbContext
{
    public StudyPlannerContext()
    {
    }

    public StudyPlannerContext(DbContextOptions<StudyPlannerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountManagement> AccountManagements { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<AssignmentDetail> AssignmentDetails { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<GroupFunction> GroupFunctions { get; set; }

    public virtual DbSet<GroupManagement> GroupManagements { get; set; }

    public virtual DbSet<Messaging> Messagings { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<StatusMaster> StatusMasters { get; set; }

    public virtual DbSet<StudentClass> StudentClasses { get; set; }

    public virtual DbSet<TaskManagement> TaskManagements { get; set; }

    public virtual DbSet<TeacherClass> TeacherClasses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

        if (!optionsBuilder.IsConfigured)
        {
            // Không cấu hình gì ở đây cả
        }

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountManagement>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK__AccountM__C9F28457F7405936");

            entity.ToTable("AccountManagement");

            entity.HasIndex(e => e.Email, "UQ_Account_Email").IsUnique();

            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.ParentEmail).HasMaxLength(255);

            entity.HasOne(d => d.Group).WithMany(p => p.AccountManagements)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__AccountMa__Group__52593CB8");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E57DEE56025");

            entity.ToTable("Assignment");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.ClassId)
                .HasMaxLength(100)
                .HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(100)
                .HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Assignmen__Class__534D60F1");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Assignmen__Teach__5441852A");
        });

        modelBuilder.Entity<AssignmentDetail>(entity =>
        {
            entity.HasKey(e => new { e.AssignmentId, e.StudentId }).HasName("PK__Assignme__B165CCF045578B00");

            entity.ToTable("AssignmentDetail");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(100)
                .HasColumnName("StudentID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Assignment).WithMany(p => p.AssignmentDetails)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__Assig__5535A963");

            entity.HasOne(d => d.Status).WithMany(p => p.AssignmentDetails)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Assignmen__Statu__5629CD9C");

            entity.HasOne(d => d.Student).WithMany(p => p.AssignmentDetails)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__Stude__571DF1D5");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927A04A86734F");

            entity.ToTable("Class");

            entity.Property(e => e.ClassId)
                .HasMaxLength(100)
                .HasColumnName("ClassID");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FunctionId).HasName("PK__Function__31ABF9189805C1E3");

            entity.ToTable("Function");

            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");
        });

        modelBuilder.Entity<GroupFunction>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.FunctionId }).HasName("PK__GroupFun__87804C9B084ECC03");

            entity.ToTable("GroupFunction");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.FunctionId)
                .HasMaxLength(100)
                .HasColumnName("FunctionID");

            entity.HasOne(d => d.Function).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.FunctionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Funct__5812160E");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupFunctions)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GroupFunc__Group__59063A47");
        });

        modelBuilder.Entity<GroupManagement>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupMan__149AF30A55614189");

            entity.ToTable("GroupManagement");

            entity.Property(e => e.GroupId)
                .HasMaxLength(100)
                .HasColumnName("GroupID");
            entity.Property(e => e.GroupName).HasMaxLength(100);
        });

        modelBuilder.Entity<Messaging>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messagin__C87C037C272EFF53");

            entity.ToTable("Messaging");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(100)
                .HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId)
                .HasMaxLength(100)
                .HasColumnName("SenderID");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessagingReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__Messaging__Recei__59FA5E80");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessagingSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__Messaging__Sende__5AEE82B9");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E326604634D");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserName)
                .HasConstraintName("FK__Notificat__UserN__5BE2A6F2");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A830A7FE6F919C");

            entity.ToTable("Reminder");

            entity.Property(e => e.ReminderId).HasColumnName("ReminderID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ParentId)
                .HasMaxLength(100)
                .HasColumnName("ParentID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(100)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.Parent).WithMany(p => p.ReminderParents)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Reminder__Parent__5CD6CB2B");

            entity.HasOne(d => d.Status).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Reminder__Status__5DCAEF64");

            entity.HasOne(d => d.Student).WithMany(p => p.ReminderStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Reminder__Studen__5EBF139D");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69734A4053");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.ClassId)
                .HasMaxLength(100)
                .HasColumnName("ClassID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(100)
                .HasColumnName("StudentID");
            entity.Property(e => e.Subject).HasMaxLength(100);
            entity.Property(e => e.TeacherId)
                .HasMaxLength(100)
                .HasColumnName("TeacherID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Schedule__ClassI__5FB337D6");

            entity.HasOne(d => d.Status).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Schedule__Status__60A75C0F");

            entity.HasOne(d => d.Student).WithMany(p => p.ScheduleStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Schedule__Studen__619B8048");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ScheduleTeachers)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Schedule__Teache__628FA481");
        });

        modelBuilder.Entity<StatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__StatusMa__C8EE20434317A56D");

            entity.ToTable("StatusMaster");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.StudentId }).HasName("PK__StudentC__48357507A5275373");

            entity.ToTable("StudentClass");

            entity.HasIndex(e => e.StudentId, "idx_student_unique").IsUnique();

            entity.Property(e => e.ClassId)
                .HasMaxLength(100)
                .HasColumnName("ClassID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(100)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentCl__Class__6383C8BA");

            entity.HasOne(d => d.Student).WithOne(p => p.StudentClass)
                .HasForeignKey<StudentClass>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StudentCl__Stude__6477ECF3");
        });

        modelBuilder.Entity<TaskManagement>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TaskMana__7C6949D16043B7FD");

            entity.ToTable("TaskManagement");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StudentId)
                .HasMaxLength(100)
                .HasColumnName("StudentID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskManagements)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__TaskManag__Statu__656C112C");

            entity.HasOne(d => d.Student).WithMany(p => p.TaskManagements)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__TaskManag__Stude__66603565");
        });

        modelBuilder.Entity<TeacherClass>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.TeacherId }).HasName("PK__TeacherC__95C60234CA1FA2DE");

            entity.ToTable("TeacherClass");

            entity.Property(e => e.ClassId)
                .HasMaxLength(100)
                .HasColumnName("ClassID");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(100)
                .HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherCl__Class__6754599E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherCl__Teach__68487DD7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
