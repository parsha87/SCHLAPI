using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Scheduling.Data.EventEntities;

namespace Scheduling.Data
{
    public partial class EventDBContext : DbContext
    {
        public EventDBContext()
        {
        }

        public EventDBContext(DbContextOptions<EventDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionTypes> ActionTypes { get; set; }
        public virtual DbSet<Alarms> Alarms { get; set; }
        public virtual DbSet<AlertConditions> AlertConditions { get; set; }
        public virtual DbSet<AlertDetails> AlertDetails { get; set; }
        public virtual DbSet<AlertObjects> AlertObjects { get; set; }
        public virtual DbSet<AlertReceiver> AlertReceiver { get; set; }
        public virtual DbSet<AlertTemplate> AlertTemplate { get; set; }
        public virtual DbSet<AlertWeeklyPortMessage> AlertWeeklyPortMessage { get; set; }
        public virtual DbSet<AlertsSent> AlertsSent { get; set; }
        public virtual DbSet<Bstevents> Bstevents { get; set; }
        public virtual DbSet<BsteventsConfig> BsteventsConfig { get; set; }
        public virtual DbSet<BsteventsError> BsteventsError { get; set; }
        public virtual DbSet<CustomRulesConditions> CustomRulesConditions { get; set; }
        public virtual DbSet<CustomRulesMaster> CustomRulesMaster { get; set; }
        public virtual DbSet<CustomRulesTarget> CustomRulesTarget { get; set; }
        public virtual DbSet<DeletedRules> DeletedRules { get; set; }
        public virtual DbSet<ElementOperatorDetails> ElementOperatorDetails { get; set; }
        public virtual DbSet<ElementStatus> ElementStatus { get; set; }
        public virtual DbSet<ElementTypeReason> ElementTypeReason { get; set; }
        public virtual DbSet<EventObjectTypes> EventObjectTypes { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<EventsProcessingStatus> EventsProcessingStatus { get; set; }
        public virtual DbSet<ManualOverrideElements> ManualOverrideElements { get; set; }
        public virtual DbSet<ManualOverrideElementsHistory> ManualOverrideElementsHistory { get; set; }
        public virtual DbSet<ManualOverrideMaster> ManualOverrideMaster { get; set; }
        public virtual DbSet<ManualOverrideMasterHistory> ManualOverrideMasterHistory { get; set; }
        public virtual DbSet<MappingEventRunningConfig> MappingEventRunningConfig { get; set; }
        public virtual DbSet<MetadataTables> MetadataTables { get; set; }
        public virtual DbSet<Motype> Motype { get; set; }
        public virtual DbSet<NotiActionList> NotiActionList { get; set; }
        public virtual DbSet<NotiAlerts> NotiAlerts { get; set; }
        public virtual DbSet<NotiElements> NotiElements { get; set; }
        public virtual DbSet<NotiSmsinfo> NotiSmsinfo { get; set; }
        public virtual DbSet<NotiSteps> NotiSteps { get; set; }
        public virtual DbSet<OperatorMetadata> OperatorMetadata { get; set; }
        public virtual DbSet<ReadyAlarms> ReadyAlarms { get; set; }
        public virtual DbSet<Remaining5kResponse> Remaining5kResponse { get; set; }
        public virtual DbSet<RuleElementsMetadata> RuleElementsMetadata { get; set; }
        public virtual DbSet<RuleExecutionHistory> RuleExecutionHistory { get; set; }
        public virtual DbSet<RuleOffsetMaxCount> RuleOffsetMaxCount { get; set; }
        public virtual DbSet<RunningEventsConfig> RunningEventsConfig { get; set; }
        public virtual DbSet<Smslog> Smslog { get; set; }
        public virtual DbSet<TestXml> TestXml { get; set; }
        public virtual DbSet<ThresholdCrossingAlarmsElements> ThresholdCrossingAlarmsElements { get; set; }
        public virtual DbSet<ThresholdCrossingAlarmsMaster> ThresholdCrossingAlarmsMaster { get; set; }
        public virtual DbSet<UnitEvents> UnitEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (DbManager.SiteName != null  && !optionsBuilder.IsConfigured)
                {
                    var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Events");
                    optionsBuilder.UseSqlServer(dbConnectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionTypes>(entity =>
            {
                entity.Property(e => e.IsActiveForRule).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Alarms>(entity =>
            {
                entity.Property(e => e.IsProcessed).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AlertWeeklyPortMessage>(entity =>
            {
                entity.HasKey(e => e.WeeklyId)
                    .HasName("PK__AlertWee__18038F18DF33676A");
            });

            modelBuilder.Entity<AlertsSent>(entity =>
            {
                entity.Property(e => e.MobileNumber).IsFixedLength();
            });

            modelBuilder.Entity<BsteventsConfig>(entity =>
            {
                entity.Property(e => e.EventObjectTypeId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReceivedDateTime).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Bstevent)
                    .WithMany(p => p.BsteventsConfig)
                    .HasForeignKey(d => d.BsteventId)
                    .HasConstraintName("FK_BSTEventsConfig_BSTEvents");

                entity.HasOne(d => d.EventObjectType)
                    .WithMany(p => p.BsteventsConfig)
                    .HasForeignKey(d => d.EventObjectTypeId)
                    .HasConstraintName("FK_BSTEventsConfig_EventObjectTypes");
            });

            modelBuilder.Entity<BsteventsError>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<CustomRulesConditions>(entity =>
            {
                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.CustomRulesConditions)
                    .HasForeignKey(d => d.RuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomRulesConditions_CustomRulesMaster");
            });

            modelBuilder.Entity<CustomRulesMaster>(entity =>
            {
                entity.Property(e => e.DelayToConfirm).IsFixedLength();

                entity.Property(e => e.EndTime).IsFixedLength();

                entity.Property(e => e.IsManualReset).HasDefaultValueSql("((0))");

                entity.Property(e => e.OffsetTime).IsFixedLength();

                entity.Property(e => e.RuleExeFrom)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RuleRearmTime).IsFixedLength();

                entity.Property(e => e.RuleRepeatTime).IsFixedLength();

                entity.Property(e => e.RuleResetTime).IsFixedLength();

                entity.Property(e => e.RuleStatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.StartTime).IsFixedLength();
            });

            modelBuilder.Entity<CustomRulesTarget>(entity =>
            {
                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.CustomRulesTarget)
                    .HasForeignKey(d => d.RuleId)
                    .HasConstraintName("FK_CustomRulesTarget_CustomRulesMaster");
            });

            modelBuilder.Entity<DeletedRules>(entity =>
            {
                entity.Property(e => e.ExeFrom)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK_Event");

                entity.Property(e => e.AddedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsSentToBst).HasDefaultValueSql("((0))");

                entity.Property(e => e.Priority).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("('N')");

                entity.HasOne(d => d.ObjType)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ObjTypeId)
                    .HasConstraintName("FK_Events_EventObjectTypes");
            });

            modelBuilder.Entity<ManualOverrideElements>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Mo)
                    .WithMany(p => p.ManualOverrideElements)
                    .HasForeignKey(d => d.Moid)
                    .HasConstraintName("FK_ManualOverrideElements_ManualOverrideMaster");
            });

            modelBuilder.Entity<ManualOverrideElementsHistory>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ManualOverrideMaster>(entity =>
            {
                entity.HasKey(e => e.Moid)
                    .HasName("PK_ManualOverride");

                entity.Property(e => e.BlockId).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MocreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NetworkId).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("('N')");

                entity.Property(e => e.TagName).IsUnicode(false);

                entity.Property(e => e.ZoneId).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.ManualOverrideMaster)
                    .HasForeignKey(d => d.ActionTypeId)
                    .HasConstraintName("FK_ManualOverrideMaster_ActionTypes");

                entity.HasOne(d => d.OverrideFor)
                    .WithMany(p => p.ManualOverrideMaster)
                    .HasForeignKey(d => d.OverrideForId)
                    .HasConstraintName("FK_ManualOverrideMaster_EventObjectTypes");
            });

            modelBuilder.Entity<ManualOverrideMasterHistory>(entity =>
            {
                entity.Property(e => e.BlockId).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MocreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NetworkId).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("('N')");

                entity.Property(e => e.TagName).IsUnicode(false);

                entity.Property(e => e.ZoneId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MappingEventRunningConfig>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<MetadataTables>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<NotiActionList>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.ActionList)
                    .WithMany()
                    .HasForeignKey(d => d.ActionListId)
                    .HasConstraintName("FK_NotiActionList_NotiElements");

                entity.HasOne(d => d.Step)
                    .WithMany()
                    .HasForeignKey(d => d.StepId)
                    .HasConstraintName("FK_NotiActionList_NotiSteps");
            });

            modelBuilder.Entity<NotiElements>(entity =>
            {
                entity.Property(e => e.ActionListId).ValueGeneratedNever();
            });

            modelBuilder.Entity<ReadyAlarms>(entity =>
            {
                entity.Property(e => e.IsProcessed).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RuleElementsMetadata>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ExeFrom)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.IsActiveForMo).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RuleExecutionHistory>(entity =>
            {
                entity.Property(e => e.ParentId).HasDefaultValueSql("((-1))");

                entity.Property(e => e.RuleExecutedDate).HasDefaultValueSql("(((1)/(1))/(1))");

                entity.Property(e => e.RuleOffsetDate).HasDefaultValueSql("(((1)/(1))/(1))");
            });

            modelBuilder.Entity<RuleOffsetMaxCount>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.MaxCount).HasDefaultValueSql("((3))");
            });

            modelBuilder.Entity<RunningEventsConfig>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("(N'N')");
            });

            modelBuilder.Entity<Smslog>(entity =>
            {
                entity.Property(e => e.SmssendDateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TestXml>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.Job).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.UserName).IsUnicode(false);
            });

            modelBuilder.Entity<ThresholdCrossingAlarmsElements>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ThresholdCrossingAlarmsMaster>(entity =>
            {
                entity.HasKey(e => e.ThresholdAlarmId)
                    .HasName("PK_ThresholdCrossingAlarms");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
