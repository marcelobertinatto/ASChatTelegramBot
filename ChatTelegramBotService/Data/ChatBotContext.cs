using System;
using ChatTelegramBotService.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatTelegramBotService.Data
{
    public class ChatBotContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<UserLog> UserLog { get; set; }

        public ChatBotContext(DbContextOptions<ChatBotContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use Fluent API to configure  

            // Map entities to tables  
            modelBuilder.Entity<UserLog>().ToTable("User_LOG");
            modelBuilder.Entity<User>().ToTable("User");

            // Configure Primary Keys  
            //modelBuilder.Entity<UserLog>().HasKey(ug => ug.ID).HasName("PK_User_LOG");
            //modelBuilder.Entity<User>().HasKey(u => u.ID).HasName("PK_User");

            // Configure columns  
            //modelBuilder.Entity<UserLog>().Property(ug => ug.ID).HasColumnType("bigint").ValueGeneratedOnAdd().IsRequired();
            //modelBuilder.Entity<UserLog>().Property(ug => ug.Nome).HasColumnType("text").IsRequired(false);
            //modelBuilder.Entity<UserLog>().Property(ug => ug.Email).HasColumnType("text").IsRequired();
            //modelBuilder.Entity<UserLog>().Property(ug => ug.Telefone).HasColumnType("text").IsRequired(false);
            //modelBuilder.Entity<UserLog>().Property(ug => ug.Data).HasColumnType("datetime").IsRequired();
            //modelBuilder.Entity<UserLog>().Property(ug => ug.Data_LOG).HasColumnType("datetime").IsRequired();

            //modelBuilder.Entity<User>().Property(ug => ug.ID).HasColumnType("bigint").ValueGeneratedOnAdd().IsRequired();
            //modelBuilder.Entity<User>().Property(ug => ug.Nome).HasColumnType("text").IsRequired(false);
            //modelBuilder.Entity<User>().Property(ug => ug.Email).HasColumnType("text").IsRequired();
            //modelBuilder.Entity<User>().Property(ug => ug.Telefone).HasColumnType("text").IsRequired(false);
            //modelBuilder.Entity<User>().Property(ug => ug.Data).HasColumnType("datetime").IsRequired();

            // Configure relationships  
            //modelBuilder.Entity<User>()
            //    .HasOne(x => x.ID)
            //    .WithMany()
            //    .IsRequired();

            //modelBuilder.Entity<UserLog>()
            //   .HasOne(x => x.ID)
            //   .WithMany()
            //   .IsRequired();

            //base.OnModelCreating(modelBuilder);
        }
    }
}
