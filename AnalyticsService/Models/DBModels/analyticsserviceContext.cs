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

        public virtual DbSet<PromotionAnalytics> PromotionAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
