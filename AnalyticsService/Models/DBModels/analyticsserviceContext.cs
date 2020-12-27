using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AnalyticsService.Models.DBModels
{
    public partial class analyticsserviceContext : DbContext
    {
        public analyticsserviceContext()
        {
        }

        public analyticsserviceContext(DbContextOptions<analyticsserviceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LinkLogs> LinkLogs { get; set; }
        public virtual DbSet<PromotionAnalytics> PromotionAnalytics { get; set; }
        public virtual DbSet<Playbacks> Playbacks { get; set; }
        public virtual DbSet<PlaybacksSlots> PlaybacksSlots { get; set; }
        public virtual DbSet<DeviceRunningTimes> DeviceRunningTimes { get; set; }

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

            modelBuilder.Entity<Playbacks>(entity =>
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

            modelBuilder.Entity<PlaybacksSlots>(entity =>
            {
                entity.HasKey(e => e.PlaybackSlotId).HasName("PRIMARY");

                entity.ToTable("playbackslots");

                entity.Property(e => e.PlaybackSlotId).HasColumnName("playbackslot_id");

                entity.Property(e => e.PlaybackId).HasColumnName("playback_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasColumnType("enum('mo', 'no', 'ev', 'ni')")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<DeviceRunningTimes>(entity =>
            {
                entity.HasKey(e => e.DeviceRunningTimeId).HasName("PRIMARY");

                entity.ToTable("device_running_times");

                entity.Property(e => e.DeviceRunningTimeId).HasColumnName("device_running_time_id");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("timestamp");
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
