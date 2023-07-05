using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Scheduling.Data.Entities;

namespace Scheduling.Data
{
    public partial class MainDBContext : DbContext
    {
        public MainDBContext()
        {
        }

        public MainDBContext(DbContextOptions<MainDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accessor> Accessor { get; set; }
        public virtual DbSet<AddressDetails> AddressDetails { get; set; }
        public virtual DbSet<AdminBasedActions> AdminBasedActions { get; set; }
        public virtual DbSet<AdminMoprivileges> AdminMoprivileges { get; set; }
        public virtual DbSet<AdminPrivileges> AdminPrivileges { get; set; }
        public virtual DbSet<AdminRulesPrivileges> AdminRulesPrivileges { get; set; }
        public virtual DbSet<AlarmAckTable> AlarmAckTable { get; set; }
        public virtual DbSet<AlarmActionList> AlarmActionList { get; set; }
        public virtual DbSet<AlarmLevelAction> AlarmLevelAction { get; set; }
        public virtual DbSet<AlarmLevels> AlarmLevels { get; set; }
        public virtual DbSet<AlarmMessages> AlarmMessages { get; set; }
        public virtual DbSet<AlarmMsgusers> AlarmMsgusers { get; set; }
        public virtual DbSet<AlarmReadTable> AlarmReadTable { get; set; }
        public virtual DbSet<Analog05vsensor> Analog05vsensor { get; set; }
        public virtual DbSet<Analog420mAsensor> Analog420mAsensor { get; set; }
        public virtual DbSet<AnalogSensorType> AnalogSensorType { get; set; }
        public virtual DbSet<ApproveUserAccessData> ApproveUserAccessData { get; set; }
        public virtual DbSet<AspNetRolePrivilege> AspNetRolePrivilege { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspUserLoginLogoutLogs> AspUserLoginLogoutLogs { get; set; }
        public virtual DbSet<BaseStation> BaseStation { get; set; }
        public virtual DbSet<BitValuesDescription> BitValuesDescription { get; set; }
        public virtual DbSet<Block> Block { get; set; }
        public virtual DbSet<BlockShadow> BlockShadow { get; set; }
        public virtual DbSet<BstdeviceType> BstdeviceType { get; set; }
        public virtual DbSet<CalenderMetadata> CalenderMetadata { get; set; }
        public virtual DbSet<CardSetting> CardSetting { get; set; }
        public virtual DbSet<CardType> CardType { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<ChannelShadow> ChannelShadow { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<CropTypeofSystem> CropTypeofSystem { get; set; }
        public virtual DbSet<CropTypes> CropTypes { get; set; }
        public virtual DbSet<CropUserDetails> CropUserDetails { get; set; }
        public virtual DbSet<DashBoardCriteria> DashBoardCriteria { get; set; }
        public virtual DbSet<DashBoardScreenCriteria> DashBoardScreenCriteria { get; set; }
        public virtual DbSet<DashboardColorCodes> DashboardColorCodes { get; set; }
        public virtual DbSet<DashboardGaugeSetting> DashboardGaugeSetting { get; set; }
        public virtual DbSet<DashboardGraphChannels> DashboardGraphChannels { get; set; }
        public virtual DbSet<DashboardGraphData> DashboardGraphData { get; set; }
        public virtual DbSet<DashboardMatrixSensors> DashboardMatrixSensors { get; set; }
        public virtual DbSet<DashboardScreens> DashboardScreens { get; set; }
        public virtual DbSet<DashboardScreensSettings> DashboardScreensSettings { get; set; }
        public virtual DbSet<DashboardSearchBy> DashboardSearchBy { get; set; }
        public virtual DbSet<DashboardSensorsUserBased> DashboardSensorsUserBased { get; set; }
        public virtual DbSet<DefaultUnits> DefaultUnits { get; set; }
        public virtual DbSet<DeletedBlock> DeletedBlock { get; set; }
        public virtual DbSet<DeletedGroup> DeletedGroup { get; set; }
        public virtual DbSet<DeletedHandHeld> DeletedHandHeld { get; set; }
        public virtual DbSet<DeletedNetwork> DeletedNetwork { get; set; }
        public virtual DbSet<DeletedProgramIndividual> DeletedProgramIndividual { get; set; }
        public virtual DbSet<DeletedRtu> DeletedRtu { get; set; }
        public virtual DbSet<DeletedSchedule> DeletedSchedule { get; set; }
        public virtual DbSet<DeletedSequence> DeletedSequence { get; set; }
        public virtual DbSet<DeletedSubBlock> DeletedSubBlock { get; set; }
        public virtual DbSet<DeletedZone> DeletedZone { get; set; }
        public virtual DbSet<DigitalCounter> DigitalCounter { get; set; }
        public virtual DbSet<DigitalCounterTypeSensor> DigitalCounterTypeSensor { get; set; }
        public virtual DbSet<DigitalNoNctypeSensor> DigitalNoNctypeSensor { get; set; }
        public virtual DbSet<DigitalNonc> DigitalNonc { get; set; }
        public virtual DbSet<DigitalOutput> DigitalOutput { get; set; }
        public virtual DbSet<DummySeqMasterConfig> DummySeqMasterConfig { get; set; }
        public virtual DbSet<DummySeqValveConfig> DummySeqValveConfig { get; set; }
        public virtual DbSet<DummySeqWeeklySch> DummySeqWeeklySch { get; set; }
        public virtual DbSet<DummySequence> DummySequence { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentConfig> EquipmentConfig { get; set; }
        public virtual DbSet<EquipmentConfigValues> EquipmentConfigValues { get; set; }
        public virtual DbSet<EquipmentConfigValuesTemplate> EquipmentConfigValuesTemplate { get; set; }
        public virtual DbSet<EquipmentType> EquipmentType { get; set; }
        public virtual DbSet<EventThresholdSetting> EventThresholdSetting { get; set; }
        public virtual DbSet<ExpansionCardType> ExpansionCardType { get; set; }
        public virtual DbSet<FertValveGroupConfig> FertValveGroupConfig { get; set; }
        public virtual DbSet<FertValveGroupElementsConfig> FertValveGroupElementsConfig { get; set; }
        public virtual DbSet<FertValveGroupSettingsConfig> FertValveGroupSettingsConfig { get; set; }
        public virtual DbSet<FieldTechNetElement> FieldTechNetElement { get; set; }
        public virtual DbSet<FieldTechnician> FieldTechnician { get; set; }
        public virtual DbSet<FilterValveGroupConfig> FilterValveGroupConfig { get; set; }
        public virtual DbSet<FilterValveGroupElementsConfig> FilterValveGroupElementsConfig { get; set; }
        public virtual DbSet<Footer> Footer { get; set; }
        public virtual DbSet<Gateway> Gateway { get; set; }
        public virtual DbSet<GatewayMaxSch> GatewayMaxSch { get; set; }
        public virtual DbSet<GatewayNode> GatewayNode { get; set; }
        public virtual DbSet<GroupDetails> GroupDetails { get; set; }
        public virtual DbSet<GwstatusData> GwstatusData { get; set; }
        public virtual DbSet<HandHeldDevice> HandHeldDevice { get; set; }
        public virtual DbSet<HandHeldElements> HandHeldElements { get; set; }
        public virtual DbSet<HandHeldMaster> HandHeldMaster { get; set; }
        public virtual DbSet<HandHeldShadow> HandHeldShadow { get; set; }
        public virtual DbSet<HistoryHours> HistoryHours { get; set; }
        public virtual DbSet<ImportOpvalveLogs> ImportOpvalveLogs { get; set; }
        public virtual DbSet<ImportSequenceLog> ImportSequenceLog { get; set; }
        public virtual DbSet<ImportUserLog> ImportUserLog { get; set; }
        public virtual DbSet<IrrigationMultiFactor> IrrigationMultiFactor { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<LoopDates> LoopDates { get; set; }
        public virtual DbSet<MasterPumpStationConfig> MasterPumpStationConfig { get; set; }
        public virtual DbSet<MasterPumpStationSteps> MasterPumpStationSteps { get; set; }
        public virtual DbSet<MasterValveGroupConfig> MasterValveGroupConfig { get; set; }
        public virtual DbSet<MasterValveGroupElementConfig> MasterValveGroupElementConfig { get; set; }
        public virtual DbSet<MetadataTables> MetadataTables { get; set; }
        public virtual DbSet<MultiAddonCardTypes> MultiAddonCardTypes { get; set; }
        public virtual DbSet<MultiAlarmTypes> MultiAlarmTypes { get; set; }
        public virtual DbSet<MultiDataLogger> MultiDataLogger { get; set; }
        public virtual DbSet<MultiFrameTypes> MultiFrameTypes { get; set; }
        public virtual DbSet<MultiGroupData> MultiGroupData { get; set; }
        public virtual DbSet<MultiGroupMaster> MultiGroupMaster { get; set; }
        public virtual DbSet<MultiHandShakeNonReach> MultiHandShakeNonReach { get; set; }
        public virtual DbSet<MultiHandShakeReach> MultiHandShakeReach { get; set; }
        public virtual DbSet<MultiLatLongValveSensor> MultiLatLongValveSensor { get; set; }
        public virtual DbSet<MultiNetworkRtu> MultiNetworkRtu { get; set; }
        public virtual DbSet<MultiNodeAlarm> MultiNodeAlarm { get; set; }
        public virtual DbSet<MultiNodeJoinDataFrame> MultiNodeJoinDataFrame { get; set; }
        public virtual DbSet<MultiNodeLatLong> MultiNodeLatLong { get; set; }
        public virtual DbSet<MultiNodeNwDataFrame> MultiNodeNwDataFrame { get; set; }
        public virtual DbSet<MultiRtuAnalysis> MultiRtuAnalysis { get; set; }
        public virtual DbSet<MultiSensorAlarmData> MultiSensorAlarmData { get; set; }
        public virtual DbSet<MultiSensorAlarmReason> MultiSensorAlarmReason { get; set; }
        public virtual DbSet<MultiSensorEvent> MultiSensorEvent { get; set; }
        public virtual DbSet<MultiSensorType> MultiSensorType { get; set; }
        public virtual DbSet<MultiSequenceUploading> MultiSequenceUploading { get; set; }
        public virtual DbSet<MultiUiversion> MultiUiversion { get; set; }
        public virtual DbSet<MultiValveAlarmData> MultiValveAlarmData { get; set; }
        public virtual DbSet<MultiValveAlarmReason> MultiValveAlarmReason { get; set; }
        public virtual DbSet<MultiValveEvent> MultiValveEvent { get; set; }
        public virtual DbSet<MultiValveEventRawProcessing> MultiValveEventRawProcessing { get; set; }
        public virtual DbSet<MultiValveReason> MultiValveReason { get; set; }
        public virtual DbSet<MultiValveState> MultiValveState { get; set; }
        public virtual DbSet<MultiValveType> MultiValveType { get; set; }
        public virtual DbSet<Network> Network { get; set; }
        public virtual DbSet<NetworkShadow> NetworkShadow { get; set; }
        public virtual DbSet<NewSequence> NewSequence { get; set; }
        public virtual DbSet<NewSequenceValveConfig> NewSequenceValveConfig { get; set; }
        public virtual DbSet<NewSequenceWeeklySchedule> NewSequenceWeeklySchedule { get; set; }
        public virtual DbSet<NewTypeRequest> NewTypeRequest { get; set; }
        public virtual DbSet<NoWaterWindow> NoWaterWindow { get; set; }
        public virtual DbSet<Node> Node { get; set; }
        public virtual DbSet<NodeLiveData> NodeLiveData { get; set; }
        public virtual DbSet<NodeSetting> NodeSetting { get; set; }
        public virtual DbSet<NodeUpdateData> NodeUpdateData { get; set; }
        public virtual DbSet<Nomenclature> Nomenclature { get; set; }
        public virtual DbSet<NonProgEquipments> NonProgEquipments { get; set; }
        public virtual DbSet<NonRechableNode> NonRechableNode { get; set; }
        public virtual DbSet<NotificationUserAlert> NotificationUserAlert { get; set; }
        public virtual DbSet<NotificationUserElementAlert> NotificationUserElementAlert { get; set; }
        public virtual DbSet<NrseqUpids> NrseqUpids { get; set; }
        public virtual DbSet<Nrvchannels> Nrvchannels { get; set; }
        public virtual DbSet<NumberRange> NumberRange { get; set; }
        public virtual DbSet<OperationType> OperationType { get; set; }
        public virtual DbSet<OpgroupConfig> OpgroupConfig { get; set; }
        public virtual DbSet<OpgroupElementConfig> OpgroupElementConfig { get; set; }
        public virtual DbSet<OpgroupType> OpgroupType { get; set; }
        public virtual DbSet<OutputGroupMembers> OutputGroupMembers { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Privilege> Privilege { get; set; }
        public virtual DbSet<PrivilegeNodeChildRelation> PrivilegeNodeChildRelation { get; set; }
        public virtual DbSet<PrivilegePageName> PrivilegePageName { get; set; }
        public virtual DbSet<ProductType> ProductType { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectAdmin> ProjectAdmin { get; set; }
        public virtual DbSet<ProjectConfiguration> ProjectConfiguration { get; set; }
        public virtual DbSet<ProjectDefaults> ProjectDefaults { get; set; }
        public virtual DbSet<ProjectElements> ProjectElements { get; set; }
        public virtual DbSet<ProjectStatus> ProjectStatus { get; set; }
        public virtual DbSet<ProjectType> ProjectType { get; set; }
        public virtual DbSet<QueueNetworkRelation> QueueNetworkRelation { get; set; }
        public virtual DbSet<QueueNetworkRelationBstLog> QueueNetworkRelationBstLog { get; set; }
        public virtual DbSet<RechableNode> RechableNode { get; set; }
        public virtual DbSet<RecordEditProjectClick> RecordEditProjectClick { get; set; }
        public virtual DbSet<ReportSettingforZone> ReportSettingforZone { get; set; }
        public virtual DbSet<Rtu> Rtu { get; set; }
        public virtual DbSet<RtuShadow> RtuShadow { get; set; }
        public virtual DbSet<Rtumodels> Rtumodels { get; set; }
        public virtual DbSet<Rtutemplate> Rtutemplate { get; set; }
        public virtual DbSet<SchemaChangesLog> SchemaChangesLog { get; set; }
        public virtual DbSet<SensorUnitTypeMapping> SensorUnitTypeMapping { get; set; }
        public virtual DbSet<Sequence> Sequence { get; set; }
        public virtual DbSet<SequenceDeleteStatus> SequenceDeleteStatus { get; set; }
        public virtual DbSet<SequenceErrDetails> SequenceErrDetails { get; set; }
        public virtual DbSet<SequenceLooping> SequenceLooping { get; set; }
        public virtual DbSet<SequenceMasterConfig> SequenceMasterConfig { get; set; }
        public virtual DbSet<SequenceValveConfig> SequenceValveConfig { get; set; }
        public virtual DbSet<SequenceWeeklySchedule> SequenceWeeklySchedule { get; set; }
        public virtual DbSet<ServiceProvider> ServiceProvider { get; set; }
        public virtual DbSet<Slot> Slot { get; set; }
        public virtual DbSet<SlotSubblockConnection> SlotSubblockConnection { get; set; }
        public virtual DbSet<Smslog> Smslog { get; set; }
        public virtual DbSet<StatusBstlog> StatusBstlog { get; set; }
        public virtual DbSet<StatusDataNwdetails> StatusDataNwdetails { get; set; }
        public virtual DbSet<StatusDataRtu20dodetails> StatusDataRtu20dodetails { get; set; }
        public virtual DbSet<StatusDataRtudetails> StatusDataRtudetails { get; set; }
        public virtual DbSet<StatusDataRtulineDetails> StatusDataRtulineDetails { get; set; }
        public virtual DbSet<StatusDataRtuprocessing> StatusDataRtuprocessing { get; set; }
        public virtual DbSet<StatusFrameNwdetails> StatusFrameNwdetails { get; set; }
        public virtual DbSet<StatusFrameRtuchannelDetails> StatusFrameRtuchannelDetails { get; set; }
        public virtual DbSet<StatusFrameRtudetails> StatusFrameRtudetails { get; set; }
        public virtual DbSet<StatusFrameValveData> StatusFrameValveData { get; set; }
        public virtual DbSet<SubBlock> SubBlock { get; set; }
        public virtual DbSet<SubBlockErrDetails> SubBlockErrDetails { get; set; }
        public virtual DbSet<SubBlockShadow> SubBlockShadow { get; set; }
        public virtual DbSet<SubBlockWithOwnerAndAccessor> SubBlockWithOwnerAndAccessor { get; set; }
        public virtual DbSet<SubscriptionDetails> SubscriptionDetails { get; set; }
        public virtual DbSet<SubscriptionUnit> SubscriptionUnit { get; set; }
        public virtual DbSet<SynFrameSize> SynFrameSize { get; set; }
        public virtual DbSet<TableUpdater> TableUpdater { get; set; }
        public virtual DbSet<TimeIntervals> TimeIntervals { get; set; }
        public virtual DbSet<TypeOfSystem> TypeOfSystem { get; set; }
        public virtual DbSet<UnApproveUserAccessData> UnApproveUserAccessData { get; set; }
        public virtual DbSet<UnitType> UnitType { get; set; }
        public virtual DbSet<Units> Units { get; set; }
        public virtual DbSet<UpdateIds> UpdateIds { get; set; }
        public virtual DbSet<UpdateIdsMainSch> UpdateIdsMainSch { get; set; }
        public virtual DbSet<UpdateIdsProject> UpdateIdsProject { get; set; }
        public virtual DbSet<UpdateIdsRequired> UpdateIdsRequired { get; set; }
        public virtual DbSet<UpdateStatisticLog> UpdateStatisticLog { get; set; }
        public virtual DbSet<UserCrop> UserCrop { get; set; }
        public virtual DbSet<UserImportErrDetails> UserImportErrDetails { get; set; }
        public virtual DbSet<UserLockUnlock> UserLockUnlock { get; set; }
        public virtual DbSet<UserRegisterAsType> UserRegisterAsType { get; set; }
        public virtual DbSet<UserRegisteration> UserRegisteration { get; set; }
        public virtual DbSet<ValveGroupConfig> ValveGroupConfig { get; set; }
        public virtual DbSet<ValveGroupElementConfig> ValveGroupElementConfig { get; set; }
        public virtual DbSet<ValveStatus> ValveStatus { get; set; }
        public virtual DbSet<Vrtsetting> Vrtsetting { get; set; }
        public virtual DbSet<WaterMeterCumulative> WaterMeterCumulative { get; set; }
        public virtual DbSet<WaterMeterSensorSetting> WaterMeterSensorSetting { get; set; }
        public virtual DbSet<WeekDays> WeekDays { get; set; }
        public virtual DbSet<Zone> Zone { get; set; }
        public virtual DbSet<ZoneInNetwork> ZoneInNetwork { get; set; }
        public virtual DbSet<ZoneShadow> ZoneShadow { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (DbManager.SiteName != null && !optionsBuilder.IsConfigured)
                {
                    var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
                    optionsBuilder.UseSqlServer(dbConnectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressDetails>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AddrDtlId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AdminBasedActions>(entity =>
            {
                entity.Property(e => e.Date).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AdminPrivileges>(entity =>
            {
                entity.Property(e => e.AllNetworks).HasDefaultValueSql("((0))");

                entity.Property(e => e.Network).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AlarmActionList>(entity =>
            {
                entity.HasKey(e => e.ActionId)
                    .HasName("PK_AlarmActionList_1");
            });

            modelBuilder.Entity<AlarmLevelAction>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<AlarmReadTable>(entity =>
            {
                entity.HasKey(e => e.ReadAlarmId)
                    .HasName("PK_AlarmReadTempTable");

                entity.Property(e => e.IsSmssend).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AnalogSensorType>(entity =>
            {
                entity.HasOne(d => d.EqpType)
                    .WithMany(p => p.AnalogSensorType)
                    .HasForeignKey(d => d.EqpTypeId)
                    .HasConstraintName("FK_AnalogSensorType_EquipmentType");
            });

            modelBuilder.Entity<ApproveUserAccessData>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("index_ApproveUserAccessData_UserId");
            });

            modelBuilder.Entity<AspNetRolePrivilege>(entity =>
            {
                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.AspNetRolePrivilege)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_dbo.AspNetRolePrivilege_dbo.AspNetRoles_RoleId");
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId })
                    .HasName("PK_dbo.AspNetUserLogins");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_dbo.AspNetUserRoles");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.IsRestrictedUser).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BitValuesDescription>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Block>(entity =>
            {
                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Block)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_Block_Project");
            });

            modelBuilder.Entity<CalenderMetadata>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasKey(e => new { e.ChannelId, e.Rtuid, e.EqpTypeId, e.EqpId, e.SlotIdInRtu });

                entity.Property(e => e.ChannelId).ValueGeneratedOnAdd();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SlotId).HasDefaultValueSql("((0))");

                entity.Property(e => e.UsedInSubBlock).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.EqpType)
                    .WithMany(p => p.Channel)
                    .HasForeignKey(d => d.EqpTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Slot_EquipmentType");

                entity.HasOne(d => d.Rtu)
                    .WithMany(p => p.Channel)
                    .HasForeignKey(d => d.Rtuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Slot_RTU");
            });

            modelBuilder.Entity<ChannelShadow>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.SlotId).HasDefaultValueSql("((0))");

                entity.Property(e => e.UsedInSubBlock).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DashboardColorCodes>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<DashboardScreensSettings>(entity =>
            {
                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.DashboardScreensSettingsPrj)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_DashboardUserScreens_DashboardScreens2");

                entity.HasOne(d => d.Screen)
                    .WithMany(p => p.DashboardScreensSettingsScreen)
                    .HasForeignKey(d => d.ScreenId)
                    .HasConstraintName("FK_DashboardUserScreens_DashboardScreens");

                entity.HasOne(d => d.View)
                    .WithMany(p => p.DashboardScreensSettingsView)
                    .HasForeignKey(d => d.ViewId)
                    .HasConstraintName("FK_DashboardUserScreens_DashboardScreens1");
            });

            modelBuilder.Entity<DashboardSearchBy>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.SearchId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DigitalCounter>(entity =>
            {
                entity.HasOne(d => d.EqpType)
                    .WithMany(p => p.DigitalCounter)
                    .HasForeignKey(d => d.EqpTypeId)
                    .HasConstraintName("FK_DigitalCounter_EquipmentType");
            });

            modelBuilder.Entity<DigitalNonc>(entity =>
            {
                entity.HasOne(d => d.EqpType)
                    .WithMany(p => p.DigitalNonc)
                    .HasForeignKey(d => d.EqpTypeId)
                    .HasConstraintName("FK_DigitalNONC_EquipmentType");
            });

            modelBuilder.Entity<DummySeqMasterConfig>(entity =>
            {
                entity.Property(e => e.StartTime).IsFixedLength();

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.DummySeqMasterConfig)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_dummySeqMasterConfig_dummySequence");
            });

            modelBuilder.Entity<DummySeqValveConfig>(entity =>
            {
                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.DummySeqValveConfig)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_dummySeqValveConfig_dummySequence");
            });

            modelBuilder.Entity<DummySeqWeeklySch>(entity =>
            {
                entity.HasKey(e => new { e.SeqWeeklyId, e.WeekDayId });

                entity.Property(e => e.SeqWeeklyId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.DummySeqWeeklySch)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_dummySeqWeeklySch_dummySequence");

                entity.HasOne(d => d.WeekDay)
                    .WithMany(p => p.DummySeqWeeklySch)
                    .HasForeignKey(d => d.WeekDayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dummySeqWeeklySch_WeekDays");
            });

            modelBuilder.Entity<DummySequence>(entity =>
            {
                entity.HasKey(e => e.SeqId)
                    .HasName("PK_dummySequence_1");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.DummySequence)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dummySequence_Project");

                entity.HasOne(d => d.PrjType)
                    .WithMany(p => p.DummySequence)
                    .HasForeignKey(d => d.PrjTypeId)
                    .HasConstraintName("FK_dummySequence_ProjectType");
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EqpId)
                    .HasName("PK_Equipment_1");

                entity.HasOne(d => d.EqpType)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.EqpTypeId)
                    .HasConstraintName("FK_Equipment_EquipmentType");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Equipment)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Equipment_Project");
            });

            modelBuilder.Entity<EquipmentConfig>(entity =>
            {
                entity.Property(e => e.EqpTypeId).IsFixedLength();
            });

            modelBuilder.Entity<EquipmentConfigValues>(entity =>
            {
                entity.HasKey(e => new { e.Rtuid, e.EqpId, e.FieldName })
                    .HasName("PK_EquipmentConfigValues_1");

                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");

                entity.HasOne(d => d.Rtu)
                    .WithMany(p => p.EquipmentConfigValues)
                    .HasForeignKey(d => d.Rtuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EquipmentConfigValues_RTU");
            });

            modelBuilder.Entity<ExpansionCardType>(entity =>
            {
                entity.HasKey(e => e.ExpCardTypeId)
                    .HasName("PK_ExpansionCard");
            });

            modelBuilder.Entity<FertValveGroupConfig>(entity =>
            {
                entity.HasKey(e => e.MstfertPumpId)
                    .HasName("PK_MasterFertPump");

                entity.Property(e => e.IfNoCommunication).HasDefaultValueSql("((0))");

                entity.Property(e => e.NoCommunicationTime).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FilterValveGroupConfig>(entity =>
            {
                entity.HasKey(e => e.MstfilterGroupId)
                    .HasName("PK_MasterFilterGroupConfig");

                entity.Property(e => e.RuleNoDirtSenseAlarm).HasDefaultValueSql("((0))");

                entity.Property(e => e.RuleNoWithPd).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FilterValveGroupElementsConfig>(entity =>
            {
                entity.HasOne(d => d.MstfilterGroup)
                    .WithMany(p => p.FilterValveGroupElementsConfig)
                    .HasForeignKey(d => d.MstfilterGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilterValveGroupElementsConfig_FilterValveGroupConfig");
            });

            modelBuilder.Entity<Footer>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<GatewayNode>(entity =>
            {
                entity.HasIndex(e => new { e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150507");

                entity.HasIndex(e => new { e.Id, e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150442");
            });

            modelBuilder.Entity<GroupDetails>(entity =>
            {
                entity.HasKey(e => e.GrpId)
                    .HasName("PK_Group");
            });

            modelBuilder.Entity<HandHeldDevice>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.HandHeldDevId })
                    .HasName("PK_HandHeldDevice_1");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ImportOpvalveLogs>(entity =>
            {
                entity.Property(e => e.Status).IsFixedLength();
            });

            modelBuilder.Entity<ImportSequenceLog>(entity =>
            {
                entity.Property(e => e.FailuareReason).IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<ImportUserLog>(entity =>
            {
                entity.HasKey(e => e.FileNo)
                    .HasName("PK__ImportUs__6F0CEAD82E4532A1");

                entity.Property(e => e.FailuareReason).IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<LoopDates>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<MasterPumpStationConfig>(entity =>
            {
                entity.Property(e => e.FailuareAction).HasDefaultValueSql("((0))");

                entity.Property(e => e.RuleNo).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Grp)
                    .WithMany(p => p.MasterPumpStationConfig)
                    .HasForeignKey(d => d.GrpId)
                    .HasConstraintName("FK_MasterPumpStationConfig_GroupDetails");
            });

            modelBuilder.Entity<MasterValveGroupConfig>(entity =>
            {
                entity.HasKey(e => e.MstValveConfigId)
                    .HasName("PK_MasterValveConfig");
            });

            modelBuilder.Entity<MasterValveGroupElementConfig>(entity =>
            {
                entity.HasOne(d => d.MstValveConfig)
                    .WithMany(p => p.MasterValveGroupElementConfig)
                    .HasForeignKey(d => d.MstValveConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MasterValveGroupElementConfig_MasterValveGroupConfig");
            });

            modelBuilder.Entity<MetadataTables>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<MultiGroupData>(entity =>
            {
                entity.Property(e => e.Ssno).IsFixedLength();
            });

            modelBuilder.Entity<MultiSensorEvent>(entity =>
            {
                entity.HasIndex(e => e.AddedDateTime)
                    .HasName("NonClusteredIndex-20220504-141059");

                entity.HasIndex(e => e.Id)
                    .HasName("NonClusteredIndex-20220505-094226");
            });

            modelBuilder.Entity<MultiSequenceUploading>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<MultiUiversion>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Network>(entity =>
            {
                entity.HasKey(e => new { e.NetworkId, e.PrjId })
                    .HasName("PK_Network_1");

                entity.Property(e => e.NetworkId).ValueGeneratedOnAdd();

                entity.Property(e => e.NetworkLock).HasDefaultValueSql("((0))");

                entity.Property(e => e.UseTemplateForRtu).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Network)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Network_Project");
            });

            modelBuilder.Entity<NewSequenceWeeklySchedule>(entity =>
            {
                entity.HasKey(e => new { e.SeqWeeklyId, e.WeekDayId });

                entity.Property(e => e.SeqWeeklyId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<NewTypeRequest>(entity =>
            {
                entity.Property(e => e.Acknowledge).HasDefaultValueSql("((0))");

                entity.Property(e => e.AcknowledgeDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RequestDateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<NonProgEquipments>(entity =>
            {
                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.NonProgEquipments)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_NonProgEquipments_Project");
            });

            modelBuilder.Entity<NonRechableNode>(entity =>
            {
                entity.HasIndex(e => new { e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150545");

                entity.HasIndex(e => new { e.Id, e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150341");
            });

            modelBuilder.Entity<NumberRange>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OpgroupElementConfig>(entity =>
            {
                entity.HasOne(d => d.OpgroupConfig)
                    .WithMany(p => p.OpgroupElementConfig)
                    .HasForeignKey(d => d.OpgroupConfigId)
                    .HasConstraintName("FK_OPGroupElementConfig_OPGroupConfig");
            });

            modelBuilder.Entity<OutputGroupMembers>(entity =>
            {
                entity.HasKey(e => new { e.OpgroupTypeId, e.EqpTypeId, e.ElementId });
            });

            modelBuilder.Entity<PrivilegePageName>(entity =>
            {
                entity.HasNoKey();

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrivilegePageName_AspNetRoles");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.PrjId).ValueGeneratedNever();

                entity.HasOne(d => d.Units)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.UnitsId)
                    .HasConstraintName("FK_Project_Units");
            });

            modelBuilder.Entity<ProjectAdmin>(entity =>
            {
                entity.HasKey(e => new { e.PrjId, e.UserId });

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.ProjectAdmin)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectAdmin_Project");
            });

            modelBuilder.Entity<ProjectElements>(entity =>
            {
                entity.HasKey(e => new { e.PrjId, e.TypeId, e.TypeChar });

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.ProjectElements)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectElements_Project");
            });

            modelBuilder.Entity<RechableNode>(entity =>
            {
                entity.HasIndex(e => new { e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150638");

                entity.HasIndex(e => new { e.Id, e.NodeId, e.GwSrn, e.ProductId })
                    .HasName("NonClusteredIndex-20220427-150257");
            });

            modelBuilder.Entity<ReportSettingforZone>(entity =>
            {
                entity.Property(e => e.BlockId).IsUnicode(false);
            });

            modelBuilder.Entity<Rtu>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsSentToBst).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Block)
                    .WithMany(p => p.Rtu)
                    .HasForeignKey(d => d.BlockId)
                    .HasConstraintName("FK_RTU_Block");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Rtu)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_RTU_Project");
            });

            modelBuilder.Entity<SchemaChangesLog>(entity =>
            {
                entity.HasKey(e => e.SchemaChangeLogId)
                    .HasName("PK_SchemaChangeLog");

                entity.Property(e => e.MajorReleaseNumber).IsUnicode(false);

                entity.Property(e => e.MinorReleaseNumber).IsUnicode(false);

                entity.Property(e => e.PointReleaseNumber).IsUnicode(false);

                entity.Property(e => e.ScriptName).IsUnicode(false);
            });

            modelBuilder.Entity<Sequence>(entity =>
            {
                entity.HasKey(e => e.SeqId)
                    .HasName("PK_Sequence_1");

                entity.Property(e => e.IsSent).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsValid).HasDefaultValueSql("((0))");

                entity.Property(e => e.ValidationState).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Sequence)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sequence_Project");

                entity.HasOne(d => d.PrjType)
                    .WithMany(p => p.Sequence)
                    .HasForeignKey(d => d.PrjTypeId)
                    .HasConstraintName("FK_Sequence_ProjectType");
            });

            modelBuilder.Entity<SequenceErrDetails>(entity =>
            {
                entity.Property(e => e.IsError).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.SequenceErrDetails)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SequenceErrDetails_Project");

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.SequenceErrDetails)
                    .HasForeignKey(d => d.SeqId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SequenceErrDetails_Sequence");
            });

            modelBuilder.Entity<SequenceLooping>(entity =>
            {
                entity.Property(e => e.IsValid).HasDefaultValueSql("((0))");

                entity.Property(e => e.StartTime).IsFixedLength();

                entity.Property(e => e.ValidationState).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SequenceMasterConfig>(entity =>
            {
                entity.Property(e => e.StartTime).IsFixedLength();

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.SequenceMasterConfig)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_SequenceMasterConfig_Sequence");
            });

            modelBuilder.Entity<SequenceValveConfig>(entity =>
            {
                entity.HasOne(d => d.Mstseq)
                    .WithMany(p => p.SequenceValveConfig)
                    .HasForeignKey(d => d.MstseqId)
                    .HasConstraintName("FK_SequenceValveConfig_SequenceMasterConfig");

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.SequenceValveConfig)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_SequenceValveConfig_Sequence");
            });

            modelBuilder.Entity<SequenceWeeklySchedule>(entity =>
            {
                entity.HasKey(e => new { e.SeqWeeklyId, e.WeekDayId });

                entity.Property(e => e.SeqWeeklyId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.SequenceWeeklySchedule)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_SequenceWeeklySchedule_Project");

                entity.HasOne(d => d.Seq)
                    .WithMany(p => p.SequenceWeeklySchedule)
                    .HasForeignKey(d => d.SeqId)
                    .HasConstraintName("FK_SequenceWeeklySchedule_Sequence");

                entity.HasOne(d => d.WeekDay)
                    .WithMany(p => p.SequenceWeeklySchedule)
                    .HasForeignKey(d => d.WeekDayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SequenceWeeklySchedule_WeekDays");
            });

            modelBuilder.Entity<Slot>(entity =>
            {
                entity.HasOne(d => d.Block)
                    .WithMany(p => p.Slot)
                    .HasForeignKey(d => d.BlockId)
                    .HasConstraintName("FK_ExpansionCard_Block");

                entity.HasOne(d => d.ExpCardType)
                    .WithMany(p => p.Slot)
                    .HasForeignKey(d => d.ExpCardTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExpansionCard_ExpansionCardType");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Slot)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_ExpansionCard_Project");

                entity.HasOne(d => d.Rtu)
                    .WithMany(p => p.Slot)
                    .HasForeignKey(d => d.Rtuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Slot_RTU1");
            });

            modelBuilder.Entity<SlotSubblockConnection>(entity =>
            {
                entity.HasKey(e => new { e.PrjId, e.Rtuid, e.SlotSeqId, e.BlockId });

                entity.HasOne(d => d.Block)
                    .WithMany(p => p.SlotSubblockConnection)
                    .HasForeignKey(d => d.BlockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SlotSubblockConnection_Block");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.SlotSubblockConnection)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SlotSubblockConnection_Project");

                entity.HasOne(d => d.Rtu)
                    .WithMany(p => p.SlotSubblockConnection)
                    .HasForeignKey(d => d.Rtuid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SlotSubblockConnection_RTU");

                entity.HasOne(d => d.Subblock)
                    .WithMany(p => p.SlotSubblockConnection)
                    .HasForeignKey(d => d.SubblockId)
                    .HasConstraintName("FK_SlotSubblockConnection_SubBlock");
            });

            modelBuilder.Entity<Smslog>(entity =>
            {
                entity.Property(e => e.SmssendDateTime).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<StatusDataNwdetails>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("('N')");
            });

            modelBuilder.Entity<StatusDataRtudetails>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");
            });

            modelBuilder.Entity<StatusDataRtulineDetails>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ValveStatus).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<StatusFrameNwdetails>(entity =>
            {
                entity.HasKey(e => e.TimeStamp)
                    .HasName("PK_NWTransactionData");

                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");
            });

            modelBuilder.Entity<StatusFrameRtuchannelDetails>(entity =>
            {
                entity.HasKey(e => new { e.PrjId, e.NetworkId, e.RtuidInNw, e.RtutimeStamp, e.ChannelId, e.SlotId })
                    .HasName("PK_StatusFrameData_1");

                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.StatusFrameRtuchannelDetails)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusFrameData_Project");
            });

            modelBuilder.Entity<StatusFrameRtudetails>(entity =>
            {
                entity.HasKey(e => new { e.TimeStamp, e.PrjId, e.NetworkId, e.RtuidinNw, e.SlotId });
            });

            modelBuilder.Entity<StatusFrameValveData>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.NetworkId, e.RtuidInNw, e.TypeId, e.EqpTypeId, e.EqpId, e.RtutimeStamp, e.SlotId });

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PrjId).HasDefaultValueSql("((2))");
            });

            modelBuilder.Entity<SubBlock>(entity =>
            {
                entity.HasOne(d => d.Block)
                    .WithMany(p => p.SubBlock)
                    .HasForeignKey(d => d.BlockId)
                    .HasConstraintName("FK_SubBlock_Block");

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.SubBlock)
                    .HasForeignKey(d => d.PrjId)
                    .HasConstraintName("FK_SubBlock_Project");
            });

            modelBuilder.Entity<SubBlockErrDetails>(entity =>
            {
                entity.Property(e => e.ImportDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SubBlockWithOwnerAndAccessor>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("SubBlockWithOwnerAndAccessor");

                entity.Property(e => e.SubBlockId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SubscriptionDetails>(entity =>
            {
                entity.HasKey(e => e.SubscriptionDetailId)
                    .HasName("PK__Subscrip__9E6919C6E3D51EA7");
            });

            modelBuilder.Entity<SynFrameSize>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<TableUpdater>(entity =>
            {
                entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<UnApproveUserAccessData>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("index_UnApproveUserAccessData_UserId");
            });

            modelBuilder.Entity<UpdateIds>(entity =>
            {
                entity.HasIndex(e => new { e.Id, e.Gwid, e.NodeId })
                    .HasName("NonClusteredIndex-20220427-143437");
            });

            modelBuilder.Entity<UpdateIdsRequired>(entity =>
            {
                entity.HasIndex(e => new { e.Id, e.NetworkNo, e.NodeId })
                    .HasName("NonClusteredIndex-20220427-143518");
            });

            modelBuilder.Entity<UserImportErrDetails>(entity =>
            {
                entity.Property(e => e.ImportDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ValveGroupConfig>(entity =>
            {
                entity.HasKey(e => e.ValveConfigId)
                    .HasName("PK_ValveGroupConfigMaster");

                entity.HasOne(d => d.Grp)
                    .WithMany(p => p.ValveGroupConfig)
                    .HasForeignKey(d => d.GrpId)
                    .HasConstraintName("FK_ValveGroupConfig_GroupDetails");
            });

            modelBuilder.Entity<ValveGroupElementConfig>(entity =>
            {
                entity.HasOne(d => d.ValveConfig)
                    .WithMany(p => p.ValveGroupElementConfig)
                    .HasForeignKey(d => d.ValveConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValveGroupElementConfig_ValveGroupConfig");
            });

            modelBuilder.Entity<ValveStatus>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Vrtsetting>(entity =>
            {
                entity.HasIndex(e => new { e.NodeId, e.GwSrn })
                    .HasName("NonClusteredIndex-20220427-150818");

                entity.HasIndex(e => new { e.NodeId, e.ProductType, e.GwSrn })
                    .HasName("NonClusteredIndex-20220427-150900");
            });

            modelBuilder.Entity<WeekDays>(entity =>
            {
                entity.Property(e => e.Name).IsFixedLength();
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.HasKey(e => new { e.ZoneId, e.PrjId });

                entity.Property(e => e.ZoneId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Prj)
                    .WithMany(p => p.Zone)
                    .HasForeignKey(d => d.PrjId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Zone_Project");
            });

            modelBuilder.Entity<ZoneShadow>(entity =>
            {
                entity.Property(e => e.ActionType).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
