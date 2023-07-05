using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using Scheduling.Data.Entities;
using Scheduling.Data.EventEntities;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddEditUserViewModel, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.RegisterAs, opt => opt.MapFrom(src => src.RegisterAs.RegisterAsId))
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Language.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country.CntryId));

            CreateMap<Country, CountryViewModel>();
            CreateMap<CountryViewModel, Country>();

            CreateMap<Language, LanguageViewModel>();
            CreateMap<LanguageViewModel, Language>();

            CreateMap<UserRegisterAsType, UserRegisterAsTypeViewModel>();
            CreateMap<UserRegisterAsTypeViewModel, UserRegisterAsType>();

            CreateMap<ProjectViewModel, Project>();
            CreateMap<Project, ProjectViewModel>();

            CreateMap<AspNetRolePrivilegeViewModel, AspNetRolePrivilege>();
            CreateMap<AspNetRolePrivilege, AspNetRolePrivilegeViewModel>();

            CreateMap<ProgramIndividualViewModel, ProgramIndividual>();
            CreateMap<ProgramIndividual, ProgramIndividualViewModel>();

            CreateMap<SequenceWeeklyScheduleViewModel, SequenceWeeklySchedule>();
            CreateMap<SequenceWeeklySchedule, SequenceWeeklyScheduleViewModel>();

            CreateMap<SequenceValveConfigViewModel, SequenceValveConfig>();
            CreateMap<SequenceValveConfig, SequenceValveConfigViewModel>();

            CreateMap<SequenceMasterConfigViewModel, SequenceMasterConfig>();
            CreateMap<SequenceMasterConfig, SequenceMasterConfigViewModel>();

            CreateMap<SequenceErrDetailsViewModel, SequenceErrDetails>();
            CreateMap<SequenceErrDetails, SequenceErrDetailsViewModel>();

            CreateMap<ProjectTypeViewModel, ProjectType>();
            CreateMap<ProjectType, ProjectTypeViewModel>();

            CreateMap<SequenceWeeklyScheduleViewModel, SequenceWeeklySchedule>();
            CreateMap<SequenceWeeklySchedule, SequenceWeeklyScheduleViewModel>();


            CreateMap<SequenceViewModel, Sequence>();
            CreateMap<Sequence, SequenceViewModel>();

            CreateMap<SequenceShortViewModel, Sequence>();
            CreateMap<Sequence, SequenceShortViewModel>();

            CreateMap<OperationTypeViewModel, OperationType>();
            CreateMap<OperationType, OperationTypeViewModel>();

            CreateMap<AdminPrivileges, AdminPrivilegesViewModel>();
            CreateMap<AdminPrivilegesViewModel, AdminPrivileges>();

            CreateMap<Zone, ZoneViewModel>();
            CreateMap<ZoneViewModel, Zone>();

            CreateMap<Network, NetworkViewModel>();
            CreateMap<NetworkViewModel, Network>();

            CreateMap<SubBlock, SubBlockViewModel>();
            CreateMap<SubBlockViewModel, SubBlock>();

            CreateMap<Motype, MOTypeViewModel>();
            CreateMap<MOTypeViewModel, Motype>();

            CreateMap<RuleElementsMetadata, RuleElementsMetadataViewModel>();
            CreateMap<RuleElementsMetadataViewModel, RuleElementsMetadata>();

            CreateMap<AlarmLevels, AlarmLevelsViewModel>();
            CreateMap<AlarmLevelsViewModel, AlarmLevels>();

            CreateMap<ActionTypes, ActionTypesViewModel>();
            CreateMap<ActionTypesViewModel, ActionTypes>();

            CreateMap<ManualOverrideMaster, ManualOverrideMasterViewModel>();
            CreateMap<ManualOverrideMasterViewModel, ManualOverrideMaster>();

            CreateMap<FilterValveGroupConfig, FilterValveGroupConfigViewModel>();
            CreateMap<FilterValveGroupConfigViewModel, FilterValveGroupConfig>();

            CreateMap<FilterValveGroupElementsConfig, FilterValveGroupElementsConfigViewModel>();
            CreateMap<FilterValveGroupElementsConfigViewModel, FilterValveGroupElementsConfig>();


            CreateMap<GroupDetails, GroupDetailsViewModel>();
            CreateMap<GroupDetailsViewModel, GroupDetails>();

            CreateMap<SequenceErrDetails, SequenceErrDetailsViewModel>();
            CreateMap<SequenceErrDetailsViewModel, SequenceErrDetails>();


            CreateMap<SubBlock, SubBlockViewModel>();
            CreateMap<SubBlockViewModel, SubBlock>();

            CreateMap<MultiNodeNwDataFrame, MultiNodeNwDataFrameViewModel>();
            CreateMap<MultiNodeNwDataFrameViewModel, MultiNodeNwDataFrame>();

            CreateMap<Vrtsetting, VrtsettingViewModel>();
            CreateMap<VrtsettingViewModel, Vrtsetting>();

            CreateMap<MultiGroupMaster, MultiGroupMasterViewModel>();
            CreateMap<MultiGroupMasterViewModel, MultiGroupMaster>();

            CreateMap<MultiGroupData, MultiGroupDataViewModel>();
            CreateMap<MultiGroupDataViewModel, MultiGroupData>();


            

        }
    }
}
