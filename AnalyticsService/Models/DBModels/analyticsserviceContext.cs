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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkLogs>(entity =>
            {
                entity.HasKey(e => e.LinkLogId)
                    .HasName("PRIMARY");

                entity.ToTable("link_logs");

                entity.Property(e => e.LinkLogId).HasColumnName("link_log_id");

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

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            });

            modelBuilder.Entity<PromotionAnalytics>(entity =>
            {
                entity.HasKey(e => e.AnalyticId)
                    .HasName("PRIMARY");

                entity.ToTable("promotion_analytics");

                entity.Property(e => e.AnalyticId).HasColumnName("analytic_id");

                entity.Property(e => e.AdvertismentId).HasColumnName("advertisment_id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("enum('coupons','links','places')")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
