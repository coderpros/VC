using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tblContact> tblContacts { get; set; }

    public virtual DbSet<tblContactAddress> tblContactAddresses { get; set; }

    public virtual DbSet<tblContactEmail> tblContactEmails { get; set; }

    public virtual DbSet<tblContactTag> tblContactTags { get; set; }

    public virtual DbSet<tblContactTel> tblContactTels { get; set; }

    public virtual DbSet<tblCountry> tblCountries { get; set; }

    public virtual DbSet<tblCurrency> tblCurrencies { get; set; }

    public virtual DbSet<tblProperty> tblProperties { get; set; }

    public virtual DbSet<tblPropertyAvailability> tblPropertyAvailabilities { get; set; }

    public virtual DbSet<tblPropertyConfig> tblPropertyConfigs { get; set; }

    public virtual DbSet<tblPropertyContact> tblPropertyContacts { get; set; }

    public virtual DbSet<tblPropertyDistance> tblPropertyDistances { get; set; }

    public virtual DbSet<tblPropertyExtra> tblPropertyExtras { get; set; }

    public virtual DbSet<tblPropertyGroup> tblPropertyGroups { get; set; }

    public virtual DbSet<tblPropertyRate> tblPropertyRates { get; set; }

    public virtual DbSet<tblPropertyRelated> tblPropertyRelateds { get; set; }

    public virtual DbSet<tblPropertyRoom> tblPropertyRooms { get; set; }

    public virtual DbSet<tblPropertySeason> tblPropertySeasons { get; set; }

    public virtual DbSet<tblPropertySeasonDate> tblPropertySeasonDates { get; set; }

    public virtual DbSet<tblPropertySeasonExtra> tblPropertySeasonExtras { get; set; }

    public virtual DbSet<tblPropertyTag> tblPropertyTags { get; set; }

    public virtual DbSet<tblRegion> tblRegions { get; set; }

    public virtual DbSet<tblSysAudit> tblSysAudits { get; set; }

    public virtual DbSet<tblSysCacheMonitor> tblSysCacheMonitors { get; set; }

    public virtual DbSet<tblSysChangeLog> tblSysChangeLogs { get; set; }

    public virtual DbSet<tblSysEmail> tblSysEmails { get; set; }

    public virtual DbSet<tblSysError> tblSysErrors { get; set; }

    public virtual DbSet<tblSysSetting> tblSysSettings { get; set; }

    public virtual DbSet<tblTag> tblTags { get; set; }

    public virtual DbSet<tblUser> tblUsers { get; set; }

    public virtual DbSet<tblUserActivity> tblUserActivities { get; set; }

    public virtual DbSet<tblUserAuthCode> tblUserAuthCodes { get; set; }

    public virtual DbSet<tblUserIP> tblUserIPs { get; set; }

    public virtual DbSet<tblUserSession> tblUserSessions { get; set; }

    public virtual DbSet<tblCollection> tblCollections { get; set; }
    public virtual DbSet<tblPropertyCollection> tblPropertyCollections{ get; set; }

    public virtual DbSet<vwContactTag> vwContactTags { get; set; }

    public virtual DbSet<vwPropertyTag> vwPropertyTags { get; set; }
    public virtual DbSet<vwPropertyCollection> vwPropertyCollections { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("name=Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<tblContact>(entity =>
        {
            entity.ToTable("tblContact", tb => tb.HasTrigger("Trigger_CacheMonitor_tblContact"));

            entity.Property(e => e.tblContact_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblContact_editedUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblContactAddress>(entity =>
        {
            entity.ToTable("tblContactAddress", tb => tb.HasTrigger("Trigger_CacheMonitor_tblContactAddress"));

            entity.Property(e => e.tblContactAddress_createdUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblContactAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactAddress_tblContact");

            entity.HasOne(d => d.tblCountry).WithMany(p => p.tblContactAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactAddress_tblCountry");
        });

        modelBuilder.Entity<tblContactEmail>(entity =>
        {
            entity.ToTable("tblContactEmail", tb => tb.HasTrigger("Trigger_CacheMonitor_tblContactEmail"));

            entity.Property(e => e.tblContactEmail_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblContactEmail_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblContactEmails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactEmail_tblContact");
        });

        modelBuilder.Entity<tblContactTag>(entity =>
        {
            entity.ToTable("tblContactTag", tb => tb.HasTrigger("Trigger_CacheMonitor_tblContactTag"));

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblContactTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactTag_tblContact");

            entity.HasOne(d => d.tblTag).WithMany(p => p.tblContactTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactTag_tblTag");
        });

        modelBuilder.Entity<tblContactTel>(entity =>
        {
            entity.ToTable("tblContactTel", tb => tb.HasTrigger("Trigger_CacheMonitor_tblContactTel"));

            entity.Property(e => e.tblContactTel_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblContactTel_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblContactTels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblContactTel_tblContact");
        });

        modelBuilder.Entity<tblCountry>(entity =>
        {
            entity.ToTable("tblCountry", tb => tb.HasTrigger("Trigger_CacheMonitor_tblCountry"));

            entity.Property(e => e.tblCountry_A2).IsFixedLength();
            entity.Property(e => e.tblCountry_A3).IsFixedLength();
            entity.Property(e => e.tblCountry_createdUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblCountry_editedUtc).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblCurrency>(entity =>
        {
            entity.ToTable("tblCurrency", tb => tb.HasTrigger("Trigger_CacheMonitor_tblCurrency"));

            entity.Property(e => e.tblCurrency_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblCurrency_editedUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblProperty>(entity =>
        {
            entity.ToTable("tblProperty", tb => tb.HasTrigger("Trigger_CacheMonitor_tblProperty"));

            entity.Property(e => e.tblProperty_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblProperty_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblCountry).WithMany(p => p.tblProperties).HasConstraintName("FK_tblProperty_tblCountry");

            entity.HasOne(d => d.tblPropertyGroup).WithMany(p => p.tblProperties).HasConstraintName("FK_tblProperty_tblPropertyGroup");

            entity.HasOne(d => d.tblRegion).WithMany(p => p.tblProperties).HasConstraintName("FK_tblProperty_tblRegion");
        });

        modelBuilder.Entity<tblPropertyAvailability>(entity =>
        {
            entity.ToTable("tblPropertyAvailability", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyAvailability"));

            entity.Property(e => e.tblPropertyAvailability_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyAvailability_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyAvailability_tblProperty");
        });

        modelBuilder.Entity<tblPropertyConfig>(entity =>
        {
            entity.ToTable("tblPropertyConfig", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyConfig"));

            entity.Property(e => e.tblPropertyConfig_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyConfig_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblPropertyConfigs).HasConstraintName("FK_tblPropertyConfig_tblContact");

            entity.HasOne(d => d.tblCurrency).WithMany(p => p.tblPropertyConfigs).HasConstraintName("FK_tblPropertyConfig_tblCurrency");

            entity.HasOne(d => d.tblPropertyConfig_parent).WithMany(p => p.InversetblPropertyConfig_parent).HasConstraintName("FK_tblPropertyConfig_tblPropertyConfig");

            entity.HasOne(d => d.tblPropertySeason).WithMany(p => p.tblPropertyConfigs).HasConstraintName("FK_tblPropertyConfig_tblPropertySeason");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyConfigs).HasConstraintName("FK_tblPropertyConfig_tblProperty");
        });

        modelBuilder.Entity<tblPropertyContact>(entity =>
        {
            entity.ToTable("tblPropertyContact", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyContact"));

            entity.Property(e => e.tblPropertyContact_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyContact_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblContact).WithMany(p => p.tblPropertyContacts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyContact_tblContact");

            entity.HasOne(d => d.tblPropertyGroup).WithMany(p => p.tblPropertyContacts).HasConstraintName("FK_tblPropertyContact_tblPropertyGroup");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyContacts).HasConstraintName("FK_tblPropertyContact_tblProperty");
        });

        modelBuilder.Entity<tblPropertyDistance>(entity =>
        {
            entity.ToTable("tblPropertyDistance", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyDistance"));

            entity.Property(e => e.tblPropertyDistance_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyDistance_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyDistances)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyDistance_tblProperty");
        });

        modelBuilder.Entity<tblPropertyExtra>(entity =>
        {
            entity.ToTable("tblPropertyExtra", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyExtra"));

            entity.Property(e => e.tblPropertyExtra_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyExtra_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyExtras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyExtra_tblProperty");
        });

        modelBuilder.Entity<tblPropertyGroup>(entity =>
        {
            entity.ToTable("tblPropertyGroup", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyGroup"));

            entity.Property(e => e.tblPropertyGroup_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyGroup_editedUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblPropertyRate>(entity =>
        {
            entity.ToTable("tblPropertyRate", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyRate"));

            entity.Property(e => e.tblPropertyRate_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyRate_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblPropertyRate_parent).WithMany(p => p.InversetblPropertyRate_parent).HasConstraintName("FK_tblPropertyRate_tblPropertyRate");

            entity.HasOne(d => d.tblPropertySeason).WithMany(p => p.tblPropertyRates).HasConstraintName("FK_tblPropertyRate_tblPropertySeason");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyRates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyRate_tblProperty");
        });

        modelBuilder.Entity<tblPropertyRelated>(entity =>
        {
            entity.ToTable("tblPropertyRelated", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyRelated"));

            entity.Property(e => e.tblPropertyRelated_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyRelated_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyRelatedtblProperties)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyRelated_tblProperty");

            entity.HasOne(d => d.tblProperty_related).WithMany(p => p.tblPropertyRelatedtblProperty_relateds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyRelated_tblProperty_related");
        });

        modelBuilder.Entity<tblPropertyRoom>(entity =>
        {
            entity.ToTable("tblPropertyRoom", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyRoom"));

            entity.Property(e => e.tblPropertyRoom_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertyRoom_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyRoom_tblProperty");
        });

        modelBuilder.Entity<tblPropertySeason>(entity =>
        {
            entity.ToTable("tblPropertySeason", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertySeason"));

            entity.Property(e => e.tblPropertySeason_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertySeason_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertySeasons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertySeason_tblProperty");
        });

        modelBuilder.Entity<tblPropertySeasonDate>(entity =>
        {
            entity.Property(e => e.tblPropertySeasonDate_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertySeasonDate_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblPropertySeason).WithMany(p => p.tblPropertySeasonDates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertySeasonDate_tblPropertySeason");
        });

        modelBuilder.Entity<tblPropertySeasonExtra>(entity =>
        {
            entity.ToTable("tblPropertySeasonExtra", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertySeasonExtra"));

            entity.Property(e => e.tblPropertySeasonExtra_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblPropertySeasonExtra_editedUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblPropertyExtra).WithMany(p => p.tblPropertySeasonExtras).HasConstraintName("FK_tblPropertySeasonExtra_tblPropertyExtra");

            entity.HasOne(d => d.tblPropertySeason).WithMany(p => p.tblPropertySeasonExtras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertySeasonExtra_tblPropertySeason");
        });

        modelBuilder.Entity<tblPropertyTag>(entity =>
        {
            entity.ToTable("tblPropertyTag", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyTag"));

            entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyTag_tblProperty");

            entity.HasOne(d => d.tblTag).WithMany(p => p.tblPropertyTags)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPropertyTag_tblTag");
        });

        modelBuilder.Entity<tblPropertyCollection>(entity =>
            {
                entity.ToTable("tblPropertyCollection", tb => tb.HasTrigger("Trigger_CacheMonitor_tblPropertyCollection"));

                entity.HasOne(d => d.tblProperty).WithMany(p => p.tblPropertyCollections)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblPropertyCollection_tblProperty");

                entity.HasOne(d => d.tblCollection).WithMany(p => p.tblPropertyCollections)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblPropertyCollection_tblCollection");
            });

        modelBuilder.Entity<tblRegion>(entity =>
        {
            entity.ToTable("tblRegion", tb => tb.HasTrigger("Trigger_CacheMonitor_tblRegion"));

            entity.Property(e => e.tblRegion_createdUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblRegion_editedUtc).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblCountry).WithMany(p => p.tblRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRegion_tblCountry");
        });

        modelBuilder.Entity<tblSysAudit>(entity =>
        {
            entity.Property(e => e.tblSysAudit_createdLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblSysAudit_createdUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblSysCacheMonitor>(entity =>
        {
            entity.Property(e => e.tblSysCacheMonitor_created).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblSysChangeLog>(entity =>
        {
            entity.Property(e => e.tblSysChangeLog_applied).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblSysEmail>(entity =>
        {
            entity.Property(e => e.tblSysEmail_createdLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblSysEmail_createdUtc).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblUser).WithMany(p => p.tblSysEmails).HasConstraintName("FK_tblSysEmail_tblUser");
        });

        modelBuilder.Entity<tblSysError>(entity =>
        {
            entity.Property(e => e.tblSysErorr_priority).HasDefaultValueSql("((3))");
            entity.Property(e => e.tblSysError_occurredUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblSysSetting>(entity =>
        {
            entity.ToTable("tblSysSetting", tb => tb.HasTrigger("Trigger_CacheMonitor_tblSysSetting"));

            entity.Property(e => e.tblSysSetting_createdUtc).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblSysSetting_editedUtc).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblTag>(entity =>
        {
            entity.ToTable("tblTag", tb => tb.HasTrigger("Trigger_CacheMonitor_tblTag"));

            entity.Property(e => e.tblTag_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblTag_editedUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblUser>(entity =>
        {
            entity.ToTable("tblUser", tb => tb.HasTrigger("Trigger_CacheMonitor_tblUser"));

            entity.Property(e => e.tblUser_createdLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblUser_createdUTC).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblUser_editedLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblUser_editedUTC).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<tblUserActivity>(entity =>
        {
            entity.Property(e => e.tblUserActivity_createdLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblUserActivity_createdUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblSysAudit).WithMany(p => p.tblUserActivities).HasConstraintName("FK_tblUserActivity_tblSysAudit");

            entity.HasOne(d => d.tblUser).WithMany(p => p.tblUserActivities).HasConstraintName("FK_tblUserActivity_tblUser");
        });

        modelBuilder.Entity<tblUserAuthCode>(entity =>
        {
            entity.HasOne(d => d.tblUser).WithMany(p => p.tblUserAuthCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUserAuthCode_tblUser");
        });

        modelBuilder.Entity<tblUserIP>(entity =>
        {
            entity.Property(e => e.tblUserIP_createdLocal).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblUserIP_createdUTC).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.tblUser).WithMany(p => p.tblUserIPs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUserIP_tblUser");
        });

        modelBuilder.Entity<tblUserSession>(entity =>
        {
            entity.Property(e => e.tblUserSession_createdLocal).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblUserSession_createdUTC).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tblUserSession_lastActivityLocal).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.tblUserSession_lastActivityUTC).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.tblUser).WithMany(p => p.tblUserSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUserSession_tblUser");
        });

        modelBuilder.Entity<vwContactTag>(entity =>
        {
            entity.ToView("vwContactTag");
        });

        modelBuilder.Entity<vwPropertyTag>(entity =>
        {
            entity.ToView("vwPropertyTag");
        });

        modelBuilder.Entity<vwPropertyCollection>(entity =>
            {
                entity.ToView("vwPropertyCollection");
            });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
