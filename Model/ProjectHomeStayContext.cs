using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HomestayWeb.Model;

public partial class ProjectHomeStayContext : DbContext
{
    public ProjectHomeStayContext()
    {
    }

    public ProjectHomeStayContext(DbContextOptions<ProjectHomeStayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Homestay> Homestays { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vote> Votes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.CommentNavigation).WithMany(p => p.InverseCommentNavigation)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_Comments_Comments");

            entity.HasOne(d => d.Homestay).WithMany(p => p.Comments)
                .HasForeignKey(d => d.HomestayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Homestays");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6D96C46FDA67");

            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Discount1).HasColumnName("Discount");

            entity.HasOne(d => d.Homstay).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.HomstayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Homst__3C69FB99");
        });

        modelBuilder.Entity<Homestay>(entity =>
        {
            entity.HasKey(e => e.HomestayId).HasName("PK__Homestay__EDCB5CBA0F8E0D90");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.Commune).HasMaxLength(255);
            entity.Property(e => e.Country).HasMaxLength(255);
            entity.Property(e => e.District).HasMaxLength(255);
            entity.Property(e => e.HomestayName).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70C1C7A9EAB");

            entity.Property(e => e.Link).IsUnicode(false);

            entity.HasOne(d => d.Homstay).WithMany(p => p.Images)
                .HasForeignKey(d => d.HomstayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Images__HomstayI__3F466844");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF0A076EF6");

            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Homestay).WithMany(p => p.Orders)
                .HasForeignKey(d => d.HomestayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Homestay__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__4222D4EF");

            entity.HasMany(d => d.Services).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OrderServ__Servi__4BAC3F29"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OrderServ__Order__4AB81AF0"),
                    j =>
                    {
                        j.HasKey("OrderId", "ServiceId").HasName("PK__OrderSer__7FC1E0CF8863DB43");
                        j.ToTable("OrderServices");
                    });
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36CCE0C4141");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.PriceWhenSell).HasColumnType("money");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__45F365D3");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB00AA7303C15");

            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ServiceName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6E02740C");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4607F3DAD").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053452BF4E09").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fullname).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.HomestayId });

            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Review)
                .HasColumnType("ntext")
                .HasColumnName("review");

            entity.HasOne(d => d.Homestay).WithMany(p => p.Votes)
                .HasForeignKey(d => d.HomestayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Homestays");

            entity.HasOne(d => d.User).WithMany(p => p.Votes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
