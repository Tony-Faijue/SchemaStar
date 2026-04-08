using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SchemaStar.Models;

public partial class SchemastarContext : IdentityDbContext<User, IdentityRole<ulong>, ulong>
{
    public SchemastarContext()
    {
    }

    public SchemastarContext(DbContextOptions<SchemastarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Edge> Edges { get; set; } = null!;

    public virtual DbSet<Node> Nodes { get; set; } = null!;

    public virtual DbSet<Nodeweb> Nodewebs { get; set; } = null!;
    public virtual DbSet<NodeAsset> NodeAssets { get; set; } = null!;

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //needed for identity framework

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
                .HasColumnName("edge_type")
                .HasConversion<string>(); //Convert enum to string, since default is int
            entity.Property(e => e.FromNodeId).HasColumnName("from_node_id");

            entity.Property(e => e.UiMetadata)
                .HasColumnType("json")
                .HasColumnName("ui_metadata");

            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.ToNodeId).HasColumnName("to_node_id");

            entity.Property(e => e.NodeWebId)
                .HasColumnName("node_web_id");

            entity.HasOne(d => d.FromNode).WithMany(p => p.EdgeFromNodes)
                .HasForeignKey(d => d.FromNodeId)
                .OnDelete(DeleteBehavior.Cascade) //Delete edge when node is deleted
                .HasConstraintName("edge_ibfk_1");

            entity.HasOne(d => d.ToNode).WithMany(p => p.EdgeToNodes)
                .HasForeignKey(d => d.ToNodeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("edge_ibfk_2");
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("node");

            entity.HasIndex(e => e.NodeWebId, "node_web_id");

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Height).HasColumnName("height");

            entity.Property(e => e.NodeDescription)
                .HasColumnType("text")
                .HasColumnName("node_description");

            entity.Property(e => e.NodeName)
                .HasMaxLength(255)
                .HasColumnName("node_name");
            entity.Property(e => e.NodeWebId).HasColumnName("node_web_id");
            entity.Property(e => e.PositionX).HasColumnName("position_x");
            entity.Property(e => e.PositionY).HasColumnName("position_y");

            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("public_id");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'unlocked'")
                .HasColumnType("enum('locked','pinned','unlocked')") //Used enum for limited number of states
                .HasColumnName("state")
                .HasConversion<string>(); //Convert enum to string, since default is int
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.NodeWeb).WithMany(p => p.Nodes)
                .HasForeignKey(d => d.NodeWebId)
                .HasConstraintName("node_ibfk_3");
        });

        modelBuilder.Entity<Nodeweb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("nodeweb");

            //Create composite index for uniqueness of nodewebnames for each user
            entity.HasIndex(e => new { e.UserId, e.NodeWebName }, "idx_user_nodename_unique")
            .IsUnique();

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.LastLayoutAt)
                .HasColumnType("timestamp")
                .HasColumnName("last_layout_at");
            entity.Property(e => e.NodeWebName).HasColumnName("node_web_name");

            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
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
            entity.HasIndex(e => e.UserName, "username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd() //Tells EF to read it after Insert, Generated by the database
                .HasColumnType("timestamp")
                .HasColumnName("created_at");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("pass");
            
            entity.Property(e => e.PublicId)
            //Handle publicId for user instead of MySQL
                .HasMaxLength(16)
                .IsFixedLength()
                .IsRequired()
                .HasColumnName("public_id");
            
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate() //Tells Ef to read it after Insert or Update, Generated by the database
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<NodeAsset>(entity => 
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("node_asset");

            entity.HasIndex(e => e.NodeId, "node_id");

            entity.HasIndex(e => e.PublicId, "public_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NodeId).HasColumnName("node_id");

            entity.Property(e => e.PublicId)
                .HasMaxLength(16)
                .IsFixedLength()
                .IsRequired()
                .HasColumnName("public_id");

            entity.Property(e => e.NodeAssetName)
                .HasMaxLength(255)
                .HasColumnName("node_asset_name");

            entity.Property(e => e.NodeAssetType)
                .HasDefaultValueSql("'link'")
                .HasColumnName("asset_type")
                .HasColumnType("enum('image', 'audio', 'video', 'link')")
                .HasConversion<string>();

            entity.Property(e => e.NodeAssetSource)
                .HasDefaultValueSql("'external'")
                .HasColumnName("asset_source")
                .HasColumnType("enum('upload', 'external')")
                .HasConversion<string>();

            entity.Property(e => e.Url)
                .HasMaxLength(2048)
                .HasColumnName("url");

            entity.Property(e => e.MimeType)
                .HasMaxLength(127)
                .HasColumnName("mime_type");

            entity.Property(e => e.FileSize)
                .HasColumnName("file_size");

            entity.Property(e => e.BlobPath)
                .HasMaxLength(1024)
                .HasColumnName("blob_path");

            entity.HasOne(d => d.Node)
                .WithMany(p => p.NodeAssets)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_node_assets_node");
        });

        //----------Identity Tables------------
        // For custom modifications for later

        modelBuilder.Entity<IdentityRole<ulong>>(entity => 
        {
            entity.ToTable("AspNetRoles");
        });

        modelBuilder.Entity<IdentityUserRole<ulong>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<ulong>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<ulong>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        modelBuilder.Entity<IdentityUserToken<ulong>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });

        modelBuilder.Entity<IdentityRoleClaim<ulong>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
