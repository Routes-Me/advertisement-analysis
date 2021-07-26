using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class AnalyticsContext : DbContext
    {
        public AnalyticsContext()
        {
        }

        public AnalyticsContext(DbContextOptions<AnalyticsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LinkLogs> LinkLogs { get; set; }
        public virtual DbSet<PromotionAnalytics> PromotionAnalytics { get; set; }
        public virtual DbSet<Playback> Playbacks { get; set; }
        public virtual DbSet<PlaybackSlots> PlaybackSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkLogs>(entity =>
            {
                entity.HasKey(e => e.LinkLogId)
                    .HasName("PRIMARY");

                entity.ToTable("link_logs");

                entity.Property(e => e.LinkLogId).HasColumnName("link_log_id");

                entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");

                entity.Property(e => e.ClientBrowser)
                    .HasColumnName("client_browser")
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ClientOs)
                    .HasColumnName("client_os")
                    .HasColumnType("varchar(15)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.InstitutionId).HasColumnName("institution_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            });

            modelBuilder.Entity<Playback>(entity =>
            {
                entity.HasKey(e => e.PlaybackId).HasName("PRIMARY");

                entity.ToTable("playbacks");

                entity.Property(e => e.PlaybackId).HasColumnName("playback_id");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MediaType)
                    .HasColumnName("media_type")
                    .HasColumnType("enum('video','image')")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<PlaybackSlots>(entity =>
            {
                entity.HasKey(e => e.PlaybackSlotId).HasName("PRIMARY");

                entity.ToTable("playback_slots");

                entity.Property(e => e.PlaybackSlotId).HasColumnName("playback_slot_id");

                entity.Property(e => e.PlaybackId).HasColumnName("playback_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasColumnType("enum('morning', 'noon', 'evening', 'night')")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Playbacks)
                    .WithMany(p => p.Slots)
                    .HasForeignKey(d => d.PlaybackId)
                    .HasConstraintName("playback_slots_ibfk_1");
            });

            modelBuilder.Entity<PromotionAnalytics>(entity =>
            {
                entity.HasKey(e => e.AnalyticId)
                    .HasName("PRIMARY");

                entity.ToTable("promotion_analytics");

                entity.Property(e => e.AnalyticId).HasColumnName("analytic_id");

                entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.InstitutionId).HasColumnName("institution_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("enum('copouns','links','places')")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
