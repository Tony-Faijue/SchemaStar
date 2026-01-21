using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SchemaStar.Models;

public partial class SchemastarContext : DbContext
{
    public SchemastarContext()
    {
    }

    public SchemastarContext(DbContextOptions<SchemastarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Edge> Edges { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<Nodeweb> Nodewebs { get; set; }

    public virtual DbSet<User> Users { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Edge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("edge");

            entity.HasIndex(e => e.FromNodeId, "from_node_id");

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.HasIndex(e => e.ToNodeId, "to_node_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EdgeType)
                .HasDefaultValueSql("'directed'")
                .HasColumnType("enum('directed','undirected')")
                .HasColumnName("edge_type");
            entity.Property(e => e.FromNodeId).HasColumnName("from_node_id");
            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .HasDefaultValueSql("'uuid_to_bin(uuid(),1)'")
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.ToNodeId).HasColumnName("to_node_id");

            entity.HasOne(d => d.FromNode).WithMany(p => p.EdgeFromNodes)
                .HasForeignKey(d => d.FromNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("edge_ibfk_1");

            entity.HasOne(d => d.ToNode).WithMany(p => p.EdgeToNodes)
                .HasForeignKey(d => d.ToNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("edge_ibfk_2");
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("node");

            entity.HasIndex(e => e.CreatedBy, "created_by");

            entity.HasIndex(e => e.NodeWebId, "node_web_id");

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.HasIndex(e => e.UpdatedBy, "updated_by");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.NodeAudioMime)
                .HasMaxLength(64)
                .HasColumnName("node_audio_mime");
            entity.Property(e => e.NodeAudioSize).HasColumnName("node_audio_size");
            entity.Property(e => e.NodeAudioUrl)
                .HasMaxLength(512)
                .HasColumnName("node_audio_url");
            entity.Property(e => e.NodeDescription)
                .HasColumnType("text")
                .HasColumnName("node_description");
            entity.Property(e => e.NodeImageMime)
                .HasMaxLength(64)
                .HasColumnName("node_image_mime");
            entity.Property(e => e.NodeImageSize).HasColumnName("node_image_size");
            entity.Property(e => e.NodeImageUrl)
                .HasMaxLength(512)
                .HasColumnName("node_image_url");
            entity.Property(e => e.NodeName)
                .HasMaxLength(255)
                .HasColumnName("node_name");
            entity.Property(e => e.NodeWebId).HasColumnName("node_web_id");
            entity.Property(e => e.PositionX).HasColumnName("position_x");
            entity.Property(e => e.PositionY).HasColumnName("position_y");
            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .HasDefaultValueSql("'uuid_to_bin(uuid(),1)'")
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'unlocked'")
                .HasColumnType("enum('locked','pinned','unlocked')")
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NodeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("node_ibfk_1");

            entity.HasOne(d => d.NodeWeb).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.NodeWebId)
                .HasConstraintName("node_ibfk_3");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.NodeUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("node_ibfk_2");
        });

        modelBuilder.Entity<Nodeweb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nodeweb");

            entity.HasIndex(e => e.NodeWebName, "node_web_name").IsUnique();

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LastLayoutAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_layout_at");
            entity.Property(e => e.NodeWebName).HasColumnName("node_web_name");
            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .HasDefaultValueSql("'uuid_to_bin(uuid(),1)'")
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Nodewebs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("nodeweb_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Pass)
                .HasMaxLength(255)
                .HasColumnName("pass");
            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .HasDefaultValueSql("'uuid_to_bin(uuid(),1)'")
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
