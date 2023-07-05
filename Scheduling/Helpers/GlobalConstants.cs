using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Helpers
{
    public static class GlobalConstants
    {
        public static int NoOfHorizontalCols = 24;
        //If there are more than 10000 valves configured in a sequence, we will show only 2 horizontal at a time with the prev-next button
        //HD
        //20 Oct 2015
        public static int MinNoOfHorizontalCols = 2;
        public static int MaxValvesDisplayedInOneSequence = 10000;

        public static int MaxValvesAllowed = 30;

        //Max timespan entry in database
        //HD
        //4 May 2015
        public static int LastTimespanEntry = 1440;

        //Programme Limits:
        //Max. 25 group per seq.
        //As per annaxure 3
        public static int MaxGroupsAllowed = 25;

        public static int MaxConditionsAllowed = 10;
        public static int MaxRulesAllowed = 1000;

        //Handheld device total count
        public static int totalCountH = 10000;

        //For Dashboard sensor Limit
        public static int maxsenLen = 3;

        //public static string ElementTimeErrMsg = "Start time of element var1 earlier than end time of element var2.";
        public static string AlarmTagName = "Tag Name";

        // Check for Water Meter
        public static int EqpTypeId = 3;
        public static int TypeId = 3;


        public static string ElementTimeErrMsg = "var1 overlapping with other start times.";

        public static string NullZoneErrMsg = "No zone is defined. Please set zone first.";
        public static string NullElementErrMsg = "No elements are defined for var1. Please configure sequence first.";
        public static string NullValveErrMsg = "No valves added for element var1 for var2. Please configure valve first.";
        public static string NullPrgErrMsg = "No program is defined. Please configure program first.";

        public static string NullValveCnt = "Valve count is null for o/p no. var1";

        public static string DayEndErrMsg = "Day end time for zone not in proper format.";
        public static string DayEndOverflowErrMsg = "Sequence schedule oveflowing beyond zone day end time for var1.";

        public static string ProgStDtErrMsg = "var1 start date before programme start date.";
        public static string ProgEndDtErrMsg = "var1 end date after programme end date.";
        public static string ProgStTimeNotDef = "Program start time not defined.";
        public static string ProgEndTimeNotDef = "Program end time not defined.";
        public static string ProgStTimeErrMsg = "var1 start time before programme start time.";
        public static string ProgStTimeErrMsg1 = "var1 start time after programme end time.";
        public static string ProgEndTimeErrMsg = "var1 end time after programme end time.";

        public static string ProgLpStDtErrMsg = "var1 start date before programme loop start date.";
        public static string ProgLpEndDtErrMsg = "var1 end date after programme loop end date.";
        public static string ProgLpStTimeErrMsg = "var1 start time before programme loop start time.";
        public static string ProgLpStTimeErrMsg1 = "var1 start time after programme loop end time.";
        public static string ProgLpEndTimeErrMsg = "var1 end time after programme loop end time.";

        public static string FertDurErrMsg = "Fert duration more than valve var1 duration for var2.";

        public static string ValveOpMoreThanThirtyTime = "O/p no. var1 trying to operate more than var2 times.";
        public static string GroupMoreThanTwentyFiveTime = "More than var3 groups configured for var1.";

        public static string SeqWeekdaysWarning = "The var1 will not be executed as selected weekdays does not occure within sequence date range.";
        public static string SeqIntervaldaysWarning = "The var1 will not be executed as selected interval does not occure within sequence date range.";
        public static string ProgLoopDtWarning = "The program var1 will not be executed within selected loop date.";

        public static string ValveDurationblank = "Valve start time or valve duration is blank.";

        #region original
        //public static string ElementTimeErrMsg = "var1 overlapping with other start times.";

        //public static string NullZoneErrMsg = "No zone is defined. Please set zone first.";
        //public static string NullElementErrMsg = "No elements are defined for var1. Please configure sequence first.";
        //public static string NullValveErrMsg = "No valves added for element var1 for var2. Please configure valve first.";
        //public static string NullPrgErrMsg = "No program is defined. Please configure program first.";

        //public static string NullValveCnt = "Valve count is null for o/p no. var1";

        //public static string DayEndErrMsg = "Day end time for zone not in proper format.";        
        //public static string DayEndOverflowErrMsg = "Sequence schedule oveflowing beyond zone day end time for var1.";

        //public static string ProgStDtErrMsg = "var1 start date before programme start date.";
        //public static string ProgEndDtErrMsg = "var1 end date after programme end date.";
        //public static string ProgStTimeNotDef = "Program start time not defined.";
        //public static string ProgEndTimeNotDef = "Program end time not defined.";
        //public static string ProgStTimeErrMsg = "var1 start time before programme start time.";
        //public static string ProgStTimeErrMsg1 = "var1 start time after programme end time.";
        //public static string ProgEndTimeErrMsg = "var1 end time after programme end time.";

        //public static string ProgLpStDtErrMsg = "var1 start date before programme loop start date.";
        //public static string ProgLpEndDtErrMsg = "var1 end date after programme loop end date.";        
        //public static string ProgLpStTimeErrMsg = "var1 start time before programme loop start time.";
        //public static string ProgLpStTimeErrMsg1 = "var1 start time after programme loop end time.";
        //public static string ProgLpEndTimeErrMsg = "var1 end time after programme loop end time.";

        //public static string FertDurErrMsg = "Fert duration more than valve var1 duration for var2.";

        //public static string ValveOpMoreThanThirtyTime = "O/p no. var1 trying to operate more than var2 times.";
        //public static string GroupMoreThanTwentyFiveTime = "More then " + MaxGroupsAllowed.ToString() + " groups configured for var1.";

        //public static string SeqWeekdaysWarning = "The var1 will not be executed as selected weekdays does not occure within sequence date range.";
        //public static string SeqIntervaldaysWarning = "The var1 will not be executed as selected interval does not occure within sequence date range.";
        //public static string ProgLoopDtWarning = "The program var1 will not be executed within selected loop date.";
        #endregion

        public static bool ProgramLockStatus = true;
        public static bool ProgramLoopStatus = false;
        public static int ProgramId = 0;
        public static int PrjId = 0;

        //For Max Program
        public static int MaxProgramId = 2;

        //for fert group
        public static int MaxSettingId = 15;

        // for noti sms project lock
        public static int ProcessAlertsForNoti = 30;
        public static int AlertDetails = 30;
        public static int CreateMessage = 27;
        //Error Messages while importing a CSV File
        public static string InvalidNetwork = "Invalid Network name";
        public static string InvalidZone = "Invalid Zone name";
        public static string InvalidSeqSTDate = "Invalid Sequence Start Date";
        public static string InvalidSeqEndDate = "Invalid Sequence End Date";
        public static string InvalidAppOfSeq = "Invalid Application of Sequence";
        public static string InvalidTypeofOP = "Invalid Type of Operation";
        public static string InvalidbasisofOP = "Invalid Basis of Operation";
        public static string ValveNotFound = "Valve var1 Not Found";
        public static string GrpNotFound = "Group var1 Not Found";

        public static string ValveAlreadyConfigured = "Valve var1 not added in sequence. Valve var1 specified in horizontal element var2 already configured for same duration";
        public static string ValveFromGroupAlreadyConfigured = "Valve var1 not added in sequence. Valve var1 from group var2 specified in horizontal element var3 already configured for same duration";

        public static string ValveOpMoreThanThirtyTimeWhileImport = "O/p no. var1 specified in horizontal element var2 trying to operate more than var3 times.";


        public static string ValveNetworkMismatch = "O/p no. var1 is not configured under network var2.";
        public static string ValveZoneMismatch = "O/p no. var1 is not configured under zone var2.";

        public static string GroupNetworkMismatch = "O/p no. var1 from Group var2 is not configured under network var3.";
        public static string GroupZoneMismatch = "O/p no. var1 from Group var2 is not configured under zone var3.";

        //Error messages while configuring new sequence from dummy sequence
        public static string ValveOpMoreThanThirtyTimeWhileDummyConfigure = "O/p no. var1 trying to operate more than var2 times.";
        public static string ValveAlreadyConfiguredWhileDummyConfigure = "Valve var1 not added in sequence. Valve var1 already configured for duration var2";
        public static string ValveFromGroupAlreadyConfiguredWhileDummyConfigure = "Valve var1 not added in sequence. Valve var1 from group var2 already configured for duration var3";


        //for dashboard
        public static int RestrictUserBasedSensor = 3;

        //for MO Status
        public static int TimeSpanforMOStatusAddMIn = 15;

        //for footer text dynamic from Footer table
        public static string FooterText = "";

        //To increase alarm performance in dashboard page 
        public static int AlarmShowLimit = 50;

        //To incrept decrept password key
        public static string EncreptDecreptKey = "($h@r!(u!8*M";

        public static string UserLanguage = "en-US";
        public static string Y1Axis = "Y1";
        public static string Y2Axis = "Y2";

        // For Graph view sensor Type

        public static int Analoginput = 1;
        public static int DigitalInputNoNc = 2;
        public static int DigitalInputCounter = 3;
        public static int DigitalOutput = 4;


        //Project type
        public static int ProjectTypeId = 1;

        //Resource
        #region resource

        #region Languages
        public static string English = "English";
        public static string Marathi = "मराठी";
        #endregion

        #region general resources
        public static string projectIsLocked = "Project is locked. Please start configuration and try again.";
        public static string save = "Save";
        public static string update = "Update";
        public static string delete = "Delete";
        public static string cancel = "Cancel";
        public static string TagName = "Tag Name";
        public static string CharactersRemaining = "characters remaining";
        public static string MaxCharacter = "Max character";
        public static string Description = "Description";
        public static string deleting = "Deleting";
        public static string areYouSure = "are you sure?";
        public static string deleteConfirmation = "This record will be deleted. Are you sure?";
        public static string Error = "Error";
        public static string ISECannotCmuunicateToServer = "Internal server error. Can not communicate to server.";
        public static string ISECannotCreateRecord = "Internal server error. Cannot create record.";
        public static string ISECannotUpdateRecord = "Internal server error. Cannot update record.";
        public static string ISECannotDeleteRecord = "Internal server error. Cannot delete record.";
        public static string ISECannotFetchRecord = "Internal server error. Cannot fetch record.";
        public static string UnauthorizedAccessErrorString = "Unauthorised Access of page";
        public static string DataSavedsuccessfully = "Data Saved successfully.";
        public static string edit = "Edit";
        public static string Nodatafound = "No Data Found...";

        #endregion

        #region MaintainProject page resources
        public static string projectBlockError = "Can not update Project as Blocks are already configured. Delete all Blocks in Project and reconfigure.";
        public static string projectBlockLimit = "Block number must be less than 255.";
        public static string projectNetworkError = "Can not update Project as Networks are already configured. Delete all Networks in Project and reconfigure.";
        public static string projectZoneError = "Can not update Project as Zones are already configured. Delete all Zones in Project and reconfigure.";
        public static string maxOpSubscribedError = "Can not update Project as Outputs are already configured. Delete all Outputs in Project and reconfigure.";
        public static string updateProjectSuccess = "Project detail updated Successfully.";
        public static string updateProjectFail = "Project detail updation Fail.";
        public static string projectNameErrorValidation = "Project name must be greater than 4 character.";
        public static string maxNetworkErrorValidation = "Please enter Maximum Networks.";
        public static string maxZoneErrorValidation = "Please enter Maximum Zones.";
        public static string countryErrorValidation = "Please select the country.";
        public static string maxOpSucribedErrorValidation = "Please enter Number of output subscribed.";
        public static string MaximumNoofRTUSperNetwork = "Maximum No of RTUS per Network";
        public static string MaximumNoofRTUSperNetworkError = "Can not update Project for Maximum Number of RTUs field. Networks are already configured in project. Delete all Networks in Project and reconfigure.";
        #endregion

        #region Zone Page resources
        public static string AvailableZones = "Available Zones";
        public static string ZoneName = "Zone Name";
        public static string EditZone = "Edit Zone";
        public static string ConfigWithNewZone = "Config With New Zone";
        public static string NumberofBlocksinZone = "Number of Blocks in Zone";
        public static string Nos = "Nos";
        public static string BlockNumberStartFrom = "Block Number Start From";
        public static string MaximumFlowRate = "Maximum Flow Rate";
        public static string DayStartTime = "Day Start Time";
        public static string NumberofPumpStations = "Number of Pump Stations Groups";
        public static string NumberofMasterValves = "Number Master Valve Groups";
        public static string NumberofFilterStations = "Number of Filter Station Groups";
        public static string NumberofFertCenters = "Number of Fert Centers Groups";
        public static string OperationHRS = "Operation HRS";
        public static string ConfigureBlocks = "Configure Blocks";
        public static string ConfigureGroups = "Configure Groups";
        public static string ZoneError1 = "Please select Zone Name.";
        public static string ZoneError2 = "Number of Blocks in Zone (Nos.) is required field. Please enter value.";
        public static string ZoneError3 = "Number of Blocks in Zone (Nos.) should be more than 0.";
        public static string ZoneError4 = "Please enter number value for Block No. Start From field.";
        public static string ZoneError5 = "Block No. can not start from 0. Please enter number value for Block No. Start From field.";
        public static string ZoneError6 = "Block No. Start From field value should be more than 0.";
        public static string ZoneError7 = "Number of Pump Stations (Nos.) is requested field. please enter value.";
        public static string ZoneError8 = "Number of Pump Stations (Nos.) should be more than 0.";
        public static string ZoneError9 = "Number of Master Valves (Nos.) is requested field. please enter value.";
        public static string ZoneError10 = "Number of Master Valves (Nos.) should be more than 0.";
        public static string ZoneError11 = "Number of Filter Stations (Nos.) is requested field. please enter value.";
        public static string ZoneError12 = "Number of Filter Stations (Nos.) should be more than 0.";
        public static string ZoneError13 = "Number of Fert Centers (Nos.) is requested field. please enter value.";
        public static string ZoneError14 = "Number of Fert Centers (Nos.) should be more than 0.";
        public static string ZoneError15 = "Maximum Flow Rate value is not entered. Do you want to continue?";
        public static string ZoneError16 = "Maximum Flow Rate value entered is 0. Do you want to continue?";
        public static string ZoneError17 = "Maximum Flow Rate value of Zone is less than Total Maximum Flow Rate of Block. Do you want to continue?";

        public static string ZoneError18 = "You have no privileges to configure Zone.";
        public static string ZoneError19 = "Can not add Zone. Project can contain only 255 blocks.";
        public static string ZoneError20 = "Can not add Zone. Units are not defined for project. Please define unit first and then reconfigure.";
        public static string ZoneError21 = "Can not update Zone ";
        public static string ZoneError22 = " as Blocks are already configured. Delete all Blocks in Zone ";
        public static string ZoneError23 = " and reconfigure.";
        public static string ZoneError24 = " as Block Number start from is already defined and blocks are already configured. Delete all Blocks in Zone ";
        public static string ZoneError25 = " as Pump station groups are already configured. Delete all Pump station groups in Zone ";
        public static string ZoneError26 = " as Master valve groups are already configured. Delete all Master valve groups in Zone ";
        public static string ZoneError27 = " as Filter groups are already configured. Delete all Filter groups in Zone ";
        public static string ZoneError28 = " as Fert Center groups are already configured. Delete all Fert Canter groups in Zone ";
        public static string ZoneError29 = "Zone contains Groups or Blocks. Please delete its references and then reconfigure.";


        #endregion

        #region error page
        public static string btnRedirectToLogin = "Login Page";
        public static string errorpage1 = "<strong>Oops...</strong> an unexpected event has just occured";
        public static string errorpage2 = "Return to the";
        #endregion

        #region ProgramPage resources
        public static string lblProgramLoopConflict1 = "Not all programs in loop are executed. Please reconfigure the looping dates.";
        public static string msgLoopingDateSave = "Looping details added successfully";
        public static string msgLoopingDetailsDateSave = "Looping details added successfully. Looping end date time saved as program end date time. Please check in below table.";
        public static string Pleaseselectatleastoneprogramforlooping = "Please select at least one program for looping.";
        public static string canNotSaveConflictInStartDateWithLoopingStartEndDate = "Can not save data.Start date conflict with Looping Start Date and Looping End date.";
        public static string canNotSaveConflictInEndDateWithLoopingStartEndDate = "Can not save data.End date conflict with Looping Start Date and Looping End date.";
        public static string canNotSaveConflictInStartEndDateWithLoopingStartEndDate = " Can not save data.Start date and End date conflict with Looping Start Date.";
        public static string canNotSaveConflictInStartEndDateWithLoopingStartStartDate = " Can not save data.Start date and End date conflict with Looping End Date.";
        public static string ProgramUpdatedSuccessfully = " Program Updated Successfully.";
        public static string Programdatanotsaved = "Program data not saved.";
        public static string CannotupdateprogramThisprogramiscurrent = "Can not update program. This program is current.";
        public static string Programdeletedsuccessfully = " Program deleted successfully.";
        public static string Programnotdeleted = "Program not deleted.";
        public static string SeqNotConfInProDeleAllSeqTry = " Sequence are configured under this program. Please delete all sequences in this program then try again.";
        public static string CannotdeleteprogramProgramisinloop = "Can not delete program. Program is in loop.";
        public static string CannotdeleteprogramLastProgram = "Can not delete program. This is last program. Please modify this Program or Create new Program and then delete this Program.";
        public static string CannotdeleteprogramThisprogramiscurrent = "Can not delete program. This program is current.";
        public static string seqnotValidNotLock = "The sequences in this program are not validated. You cannot lock the sequence now.Please validate all sequences and try again.";
        public static string ProgramisnowLocked = "Program is now Locked.";
        public static string ProgramisnowUnlocked = "Program is now Unlocked.";
        public static string PrograminLoopconfigureloopingdates = "Program is now in Loop. Please configure looping dates.";
        public static string ProgramisnowremovedfromLoop = "Program is now removed from Loop.";
        public static string ProgramremovedfromLoopconfigureloopingdates = "Program is now removed from Loop. Please configure looping dates.";
        public static string CannotaddloopThisprogramiscurrent = "Can not add in loop. This program is current.";
        public static string CannotRemovefromloopThisprogramcurrent = "Can not Remove from loop. This program is current.";
        public static string PleaseselectStartDate = "Please select Start Date.";
        public static string PleaseselectEndDate = "Please select End Date.";
        public static string ProgramStartDatemustbelessthanEndDate = "Program Start Date must be less than End Date.";
        public static string Programdatanotsaved1 = "Program data not saved.";
        public static string ProgramaddedSuccessfully = "Program added Successfully.";
        public static string ProgramnotsaveProgramEndDateconflictsprogramloopingdates = " Program can not save. Program EndDate conflicts with program looping dates.";
        public static string ProgramnotsaveProgramStartDateconflictsprogramloopingdates = " Program can not save. Program StartDate conflicts with program looping dates.";
        public static string ProgramcannotsaveProgramEndDateconflictswithotherprograms = "Program can not save. Program EndDate conflicts with other programs.";
        public static string ProgramcannotsaveProgramStartDateconflictswithotherprograms = "Program can not save. Program StartDate conflicts with other programs.";
        public static string ProgramStartDateandEndDatemustnotbeequal = " Program Start Date and End Date must not be equal.";
        public static string PleaseselectLoopStartDate = "Please select Loop Start Date.";
        public static string PleaseselectLoopEndDate = "Please select Loop End Date.";
        public static string Loopingenddatemustbeafteroroncurrentdateyear = "Looping end date must be after or on current date year.";
        public static string programpageError1 = "Looping start date must be before Looping end date";
        public static string programpageError2 = "Loop start time and Loop End time must be different.";
        public static string programpageError3 = "Loop start time and Loop End time must be different.";
        public static string programpageError4 = "Loop start time must be before than Loop end time.";
        public static string programpageError5 = "Loop start time must be before than Loop end time.";
        public static string programpageError6 = "Please enter Duration for Program A.";
        public static string programpageError7 = "Please enter valid value for Duration of Program A.";
        public static string programpageError8 = "Please enter Gap for Program A.";
        public static string programpageError9 = "Please enter valid value for Gap of Program A.";
        public static string programpageError10 = "Please enter Duration for Program B.";
        public static string programpageError11 = "Please enter valid value for Duration of Program B.";
        public static string programpageError12 = "Please enter Gap for Program B.";
        public static string programpageError13 = "Please enter valid value for Gap of Program B.";
        public static string programpageError14 = "Please enter Duration for Program C.";
        public static string programpageError15 = "Please enter valid value for Duration of Program C.";
        public static string programpageError16 = "Please enter Gap for Program C.";
        public static string programpageError17 = "Please enter valid value for Gap of Program C.";
        public static string programpageError18 = "Please enter Duration for Program D.";
        public static string programpageError19 = "Please enter valid value for Duration of Program D.";
        public static string programpageError20 = "Please enter Gap for Program D.";
        public static string programpageError21 = "Please enter valid value for Gap of Program D.";
        public static string ProgramnotsaveProgramStartDateOrEndDateconflictsprogramloopingdates = "Program can not save. Program StartDate Or EndDate conflicts with program looping dates.";

        public static string mpsm1 = "Can not update Program. Program is locked. Please unlock the Program and then try again";
        public static string mpsm2 = "Can not update Program. Program is in Loop. Please change looping dates or Remove program from Loop and Try again.";
        public static string mpsm3 = "Can not delete Program. Program is locked. Please unlock the Program and then try again.";
        public static string mpsm4 = "Can not add in loop. This Program is lock. Please unlock the Program and then try again.";
        public static string mpsm5 = "Can not remove from loop. This Program is lock. Please unlock the Program and then try again.";

        public static string Programmep = "Programme";
        public static string Programmetobeloopedp = "Programme to be looped";
        public static string StartDateddmmyyyyp = "Start Date(dd/mm/yyyy)";
        public static string StartTimeHHMMp = "Start Time (HH:MM)";
        public static string EndDateddmmyyyyp = "End Date(dd/mm/yyyy)";
        public static string EndTimeHHMMp = "EndTime (HH:MM)";
        public static string Programmelockedp = "Programme locked";
        public static string SequenceConfigurationp = "Sequence Configuration";
        public static string ProgramActionsp = "Program Actions";

        public static string ProgrammeABCDp = "Programme (A/B/C/D)";
        public static string ThiswilldeleteProgramDoyouwanttocontinue = "This will delete Program. Do you want to continue?";
        public static string ConfigureMP = "Configure";
        public static string AddProgramMP = "Add Program";
        public static string DeleteMP = "Delete";

        #endregion

        #region view seq
        public static string programlockedYoucannotconfigurenewsequencenow = "The program is locked. You cannot configure new sequence now.";
        public static string programlockedcannoteditsequecenow = "The program is locked. You cannot edit sequence now.";
        public static string programlockedYoucannotconfiguredeletesequencenow = "The program is locked. You cannot delete sequence now.";
        public static string Cannotdeletesequencitusedruletarget = "Cannot delete sequence as it is used as a rule target.";
        public static string Cannotdeletesequenceusedmanualoverrideelement = "Cannot delete sequence as it is used as a manual override element.";
        public static string programlockedcannotexportsequencesnow = "The program is locked. You cannot export sequences now.";
        public static string programlockedcannotimportsequencesnow = "The program is locked. You cannot import sequences now.";
        public static string SequencefileuploadedsuccessfullyimportYounotificationthroughSMSoncefileimportcompleted = "Sequence file has been uploaded successfully for import. You will get notification through SMS once file import is completed.";
        public static string programlockedcannotdeletesequencenow = "The program is locked. You cannot delete sequence now.";
        public static string programlockedcannoteditdummysequencenow = "The program is locked. You cannot edit dummy sequence now.";
        public static string NoRecordsFound = "No Records Found!";
        public static string programlockedcannotsuspendfilenow = "The program is locked. You cannot suspend file now.";
        public static string Doyouwantconfiguresequenceusingdummysequence = "Do you want to configure new sequence using dummy sequence?";
        public static string Novalvesconfigdummysequenceseqconfigureddummyseqnotcontainvalveschedulewantcontinue = "No valves are configured in dummy sequence. New sequence configured from this dummy sequence will not contain any valve schedule. Do you want to continue?";
        public static string AutoRefreshing = "AutoRefreshing...";
        public static string NoDummySequenceConfigured = "No Dummy Sequence Configured";
        public static string Doyouwanttodeletefailedsequencefiles = "Do you want to delete failed sequence files?";
        public static string Doyouwanttosuspendtheimportoperation = "Do you want to suspend the import operation?";
        public static string ValidatingsequencemaytakeseveralminutestofinishnDoyouwanttocontinue = "Validating sequence may take several minutes to finish. \n Do you want to continue?";
        public static string Doyouwanttodelete = "Do you want to delete?";
        public static string SeqType = "Sequence Type";

        public static string SequenceNoVs = "Sequence No.";
        public static string NetworkVS = "Network";
        public static string ZoneVS = "Zone";
        public static string BasisOfOpVS = "Basis Of Op";
        public static string SelectAllVS = "Select All";
        public static string lnkView = "View";
        public static string btnConfigureSeq1 = "Add New Sequence";
        public static string NoSequenceConfigured = "No Sequence Configured";
        public static string ErrorDetails = "Error Details";
        public static string DummySequenceNo = "Dummy Sequence No";
        public static string lnkConfigure = "Configure";
        public static string FileNoVS = "File No";
        public static string FileNameVS = "File Name";
        public static string UplodedDateVS = "Uploded Date";
        public static string UplodedByVS = "Uploded By";
        public static string StatusVS = "Status";
        public static string DateProcessed = "Date Processed";
        public static string lnkRemovePending = "Suspend";
        public static string ReasonVS = "Reason";
        public static string lnkRemoveFailed = "Delete";


        public static string Sequenceenddatebeforeprogramstartdate = "Sequence enddate before program start date.";
        public static string Sequencestartdatebeforeprogramstartdate = "Sequence start date before program start date.";
        public static string Sequencestartdateafterprogramenddate = "Sequence start date after program end date.";
        public static string Sequenceenddateafterprogramenddate = "Sequence enddate after program end date.";
        public static string Sequencestartdateisaftersequenceenddate = "Sequence start date is after sequence enddate.";

        public static string validationProOver = "Validation process complete for all sequences in current Program.";

        #endregion

        #region Network
        public static string AvailableNetworks = "Available Networks";
        public static string ConfigWithNewNetwork = "Config With New Network";
        public static string EditNetwork = "Edit Network";
        public static string Loadingrecords = "Loading records...";
        public static string Nodataavailable = "No data available!";
        public static string Usedefaulttemplate8DOforRTUs = "Use default template(8DO) for RTUs";
        public static string ConfigureRTUs = "Configure RTUs";
        public static string ConfigureBaseStations = "Configure BaseStations";
        public static string ConfigureFieldTechnician = "Configure Field Technician";

        public static string networkError1 = "Please select Network Name.";
        public static string networkError2 = "Zones in network is required field. Please enter value.";
        public static string networkError3 = "Zones in network should be more than 0.";
        public static string networkError4 = "RTUs in network is required field. Please enter value.";
        public static string networkError5 = "RTUs in network should be more than 0.";
        public static string networkError6 = "RTUs in network not in proper format..";

        public static string NetworkName = "Network Name";
        public static string TagNameNet = "Tag Name";
        public static string DescriptionNet = "Description";
        public static string RTUsinNetwork = "RTUs in Network";
        public static string ZonesInNetwork = "Zones In Network";
        public static string UseTemplateforRTU = "Use Template for RTU";
        public static string lnkConfigureRTUs = "Configure RTU";
        public static string lnkAdd = "Add Network";
        public static string lnkBaseStation = "Configure BaseStations";
        public static string lnkFieldTechncian = "Configure Field technician";
        public static string NetworkNoPrivilege = "You have no privileges to configure Network.";
        public static string neterr1 = "Are sure to delete this network?";
        public static string neterr2 = "Search string should be minimum 3 character long.";
        public static string neterr3 = "Network deleted successfully.";
        public static string neterr4 = "Network  or RTUs in network are in use. Cannot delete Network.";
        public static string neterr5 = "Please enter Number of RTU.";
        public static string neterr6 = "Please enter integer value for RTUs in Network.";
        public static string neterr7 = "Please enter Tag Name.";
        public static string neterr8 = "Please select at least one zone.";
        public static string neterr9 = "Please select Network.";
        public static string neterr10 = "Please enter Number of RTU.";
        public static string neterr11 = "Can not add Network as Maximum No. of RTUs per network limit is";
        public static string neterr12 = "You have selected Use default template for rtu but no default type rtu is set to project. Please set default type RTU in library.";
        public static string neterr13 = "Can not update Network as Outputs are already configured. Delete all Outputs in Project and reconfigure. Or increase Maximum output subscribed for project and reconfigure.";
        public static string neterr14 = "Can not add Network. Units are not defined for project. Please define unit first and then reconfigure.";
        public static string neterr15 = "Can not update Network as Maximum No. of RTU per network limit is";
        public static string neterr16 = "Can not change Zones. Blocks in Zones are used in RTU. Please remove all references and then try again.";
        public static string neterr17 = "Can not update Network";
        public static string neterr18 = "as RTU are already configured. Delete all RTU in Network";
        public static string neterr19 = "and reconfigure.";
        public static string neterr20 = "Can not update Network as Outputs are already configured. Delete all Outputs in Project and reconfigure. Or increase Maximum output subscribed for project and reconfigure.";
        public static string neterr21 = "Network Created Successfully.";
        public static string neterr22 = "Network Update Successfully.";

        #endregion

        #region block
        public static string YouhavenoprivilegeconfigureBlock = "You have no privilege to configure Block.";
        public static string AvailableBlocks = "Available Blocks";
        public static string ConfigWithNewBlock = "Config With New Block";
        public static string EditBlockk = "Edit Block";
        public static string BlockName = "Block Name";
        public static string PleaseSelectZone = "Please Select Zone.";
        public static string ZoneNameB = "Zone Name";
        public static string NetworkNameB = "Network Name";
        public static string SubBlockNosB = "SubBlock (Nos.)";
        public static string MaximumFlowRateB = "Maximum Flow Rate ";
        public static string ConfigureSubBlocksB = "Configure SubBlocks";

        public static string blockError1 = "Maximum number of blocks are already configured!";
        public static string blockError2 = "Please select Block Name.";
        public static string blockError3 = "Please select Network Name.";
        public static string blockError4 = "Number of SubBlocks is required field. Please enter value.";
        public static string blockError5 = "Number of SubBlocks should be more than 0.";
        public static string blockError7 = "Maximum Flow Rate value entered is 0. Do you want to continue?";
        public static string blockError8 = "Maximum Flow Rate of Block exceeds Zone max flow rate.";
        public static string blockError9 = "Maximum Flow Rate of Block is less than Total Maximum Flow Rate of SubBlocks. Do you want to continue?";
        public static string blockError10 = "You have no privilege to configure the blocks.";
        public static string blockError11 = "Cannot delete Block. Block is used in RTU as Location.";
        public static string blockError12 = "Can not update Network name of Block as Block is already used in RTU.";
        public static string blockError13 = "Can not update Block ";
        public static string blockError14 = " as SubBlocks are already configured. Delete all SubBlocks in Block ";
        public static string blockError15 = " and reconfigure.";

        #endregion

        #region RTU
        public static string YouhavenoprivilegetoconfigureNetwork = "You have no privilege to configure Network.";
        public static string AvailableRTUs = "Available RTUs";
        public static string ConfigwithNewRTU = "Config with New RTU";
        public static string PleaseselectNetworkR = "Please select Network.";
        public static string RTUName = "RTU Name";
        public static string BlockNameR = "Block Name";
        public static string RTUIdInNetwork = "RTU Id In Network";
        public static string NetworkNameR = "Network Name";
        public static string EditRTU = "Edit RTU";
        public static string RTUTypeR = "RTU Type";
        public static string ActiveR = "Active";
        public static string ExpansionCardsInRTUR = "Expansion Cards In RTU";
        public static string ProjectIdR = "Project Id";
        public static string RTUError2 = "You have no privileges to configure RTU.";
        public static string RTUError1 = "All RTU in Network are already configured:";
        public static string RTUError3 = "Channels in RTU OR RTU are in use. Cannot delete RTU.";
        #endregion

        #region RTUModel
        public static string appRMMM = "- for";
        public static string appRMMMM = "- used as";
        public static string appRMM = "Channels are not used.";
        public static string appRM = "Data not configured.";
        public static string appRM1 = "SubBlock";
        public static string appRM2 = "Valve Group";
        public static string appRM3 = "Filter Group";
        public static string appRM4 = "Fertilizer Group";
        public static string appRM5 = "Master Valve Group";
        public static string appRM6 = "Master Pump Station Group";
        public static string appRM7 = "Sequence Name";
        public static string appRM8 = "Filter Group Watermeter sensor AI";
        public static string appRM9 = "Filter Group PD sensor AI";
        public static string appRM10 = "Filter Group Press Sustaining sensor AI";
        public static string appRM11 = "Master Pump Station Group";
        public static string appRM12 = "Fertilizer Group";
        public static string appRM13 = "Filter valve Group";
        public static string appRM14 = "Master Valve Group";
        public static string appRM15 = "Manual Override";
        public static string appRM16 = "Rules Targets for Channels";
        public static string appRM17 = "Rules Conditions";
        public static string appRM18 = "Rules Targets for RTU";
        public static string appRM19 = "Used as Primary/Secondary sensor in DO configuration";


        #endregion

        #region MaintainConfigSensor
        public static string RTUnotAIDIRTUtype4DO8DO20DO = "Selected RTU does not contain AI Sensors and DI Sensors. The RTU type is 4DO/8DO/20DO.";
        public static string YouhavenoprivilegetoconfigureZoneMCS = "You have no privilege to configure Zone.";
        #endregion

        #region UserControlWizard
        public static string SelectedRTUnotcontainAIDISensors = "Selected RTU does not contain AI Sensors and DI Sensors.";
        public static string PleaseSelectRTUNameU = "Please Select RTU Name.";
        public static string charactersremainingU = "characters remaining";

        public static string PleaseenterTagNameU = "Please enterTag Name";
        public static string PleaseSelectTypesU = "Please Select Types";
        public static string PleaseSelectBlockNumberU = "Please Select Block Number";
        public static string PleaseSelectProgrammableNonProgrammableU = "Please Select Programmable/Non Programmable";
        public static string Pleaseenter1100valueforRFPowerLevelU = "Please enter 1-100 value for RF Power Level.";
        public static string Pleaseenter110valueforExpansionCardsU = "Please enter 1-10 value for Expansion Cards";
        public static string PleaseEnterOnlyNumbersinExpansionCardsU = "Please Enter Only Numbers in Expansion Cards";


        public static string PleasedefineallRTUDetails = "Please define all RTU Details.";
        public static string RTUnotsavedOutputexceedsMaximumoutputsubscribedProject = "RTU not saved. Output exceeds than Maximum output subscribed in Project.";
        public static string RTUnotsavedExpansionCardsalreadyconfiguredRTU = "RTU not saved. Expansion Cards are already configured in this RTU.";
        public static string DataNotSaved = "Data Not Saved";
        public static string CannotchangeRTUTypecontainsExpansionCards = "Cannot change RTU Type as it contains Expansion Cards.";
        public static string ucdefaultmsg1 = "Deafault values are assigned.";
        public static string ucdefaultmsg2 = "No Configure values for selected RTU Type.";

        public static string getdeRtuD = "Default";
        public static string getdeRtuND = "No Default";
        #endregion

        #region userControlGridview
        public static string Sessiontimeoutpleaseloginagain = "Session time out please login again.";

        public static string ChannelNameUG = "Channel Name";
        public static string DescriptionUG = "Description";
        public static string PhysicalLocation = "Physical Location";
        public static string TagNameUG = "Tag Name";
        public static string ClicktoEnableUG = "Enable All";
        public static string TypeUG = "Type";
        public static string ConfigureUG = "Configure";
        public static string SaveandConfigureUG = "Save and Configure";
        public static string nodafo = "No data found....";
        public static string appUg1 = "SubBlock";
        public static string appUg2 = "Valve Group";
        public static string appUg3 = "Filter Group";
        public static string appUg4 = "Fertilizer Group";
        public static string appUg5 = "Master Valve Group";
        public static string appUg6 = "Master Pump Station Group";
        public static string appUg7 = "Sequence Name";
        public static string appUg8 = "Filter Group";
        public static string appUg9 = "Water Meter in Fliter Group";
        public static string appUg10 = "Pressure sustaning OP in Fliter Group";
        public static string appUg11 = "Input Sensor in Master Pump Station Group";
        public static string appUg12 = "Input Sensor in Manual Override";
        public static string appUg13 = "Rules Targets";
        public static string appUg14 = "Rules Conditions";
        public static string appUg15 = "DO Configuration (Primary/Secondary sensor)";
        public static string appUg16 = "Valve is enabled but not used.";
        public static string appUg17 = "Valve is not enabled and not used.";
        public static string appUg18 = "Valve not used.";
        public static string appUg19 = "Sensor is enabled but not used.";
        public static string appUg20 = "Sensor is not enabled and not used.";
        public static string appUg21 = "Sensor not used.";
        public static string appUg22 = "Valve not configured.";
        public static string appUg23 = "Sensor not configured.";

        public static string CannotchangeTypeDigitalInputDigitalInputalreadyuse = "Cannot change Type of Digital Input. This Digital Input is already in use.";
        public static string CannotchangeTypeAnalogInputAnalogInputalreadyuse = "Cannot change Type of Analog Input. This Analog Input is already in use.";
        public static string CannotchangeTypeDigitalOutputDigitalOutputalreadyuse = "Cannot change Type of Digital Output. This Digital Output is already in use.";
        public static string CannotchangeSubtypeDigitalInputDigitalInputalreadyuse = "Cannot change Subtype of Digital Input. This Digital Input is already in use.";
        public static string PleaseentertheTagNameUG = "Please enter the Tag Name.";
        public static string PleaseselectTypeUG = "Please select the Type.";
        public static string PleaseselectDigitalInputSubTypeUG = "Please select the Digital Input SubType.";
        public static string PleaseEnableChannel = "Please Enable the Channel.";
        public static string Pleaseadddefaultunitfor = "Please add default unit for ";
        public static string DataNotSavedUG = "Data Not Saved";
        public static string CannotEnableDigitalOutputsYouhavenotaddedValvetypeProjectLiabrary = "Cannot Enable Digital Outputs. You have not added Valve type in Project Liabrary.";
        public static string ThisDigitalOutputalreadyuseCannotEnableDisableDigitalOutput = "This Digital Output is already in use. Cannot Enable/Disable Digital Output:";
        public static string AnalogInputalreadyuseCannotEnableDisableAnalogInput = "This Analog Input is already in use. Cannot Enable/Disable Analog Input :";
        public static string ThisDigitalInputalreadyuseCannotEnableDisableDigitalInput = "This Digital Input is already in use. Cannot Enable/Disable Digital Input :";
        public static string CannotEnableDigitalOutputhavenotaddedValvetypeProjectLiabrary = "Cannot Enable this Digital Output. You have not added Valve type in Project Liabrary.";
        public static string CannotEnableDisableDigitalOutputDigitalOutputalreadyuse = "Cannot Enable/Disable this Digital Output. This Digital Output is already in use.";
        public static string CannotEnableDisableAnalogInputAnalogInputalreadyuse = "Cannot Enable/Disable this Analog Input. This Analog Input is already in use.";
        public static string CannotEnableDisableDigitalInputDigitalInputalreadyuse = "Cannot Enable/Disable this Digital Input. This Digital Input is already in use.";

        #endregion

        #region Subblock
        public static string AssignOwnersSB = "Assign Owners";
        public static string EditSubBlock = "Edit SubBlock";
        public static string AvailableSubBlocks = "Available SubBlocks";
        public static string ConfigWithNewSubBlock = "Config With New SubBlock";
        public static string ViewAssignedAccessorsSB = "View Assigned Accessors";
        public static string SubBlockName = "SubBlock Name";
        public static string SelectRTUSB = "Select RTU";
        public static string ValveSB = "Valve";
        public static string SubBlockArea = "SubBlock Area";
        public static string OwnerNameSB = "Owner Name";
        public static string ViewOwnersSB = "ViewOwnersSB";
        public static string AccessorNameSB = "AccessorNameSB";

        public static string SubblockError1 = "Please select zone and block";
        public static string SubblockError2 = "Please select SubBlock Name";
        public static string SubblockError3 = "Please select Valve";
        public static string SubblockError4 = "Please enter SubBlock Area.";
        public static string SubblockError5 = "You have no privilege to configure Block.";
        public static string SubblockError6 = "No Data to import. Please add data in csv file and try again.";
        public static string SubblockError7 = "Unable to read csv file. File contain blank space in Header. Please try again later.";
        public static string SubblockError8 = "Unable to read csv file. Column names are different. Please try again later.";
        public static string SubblockError9 = "subblock added with Errors";
        public static string SubblockError10 = "subblock added successfully";
        public static string SubblockError11 = "Unable to read csv file. Please try again later.";
        public static string SubblockError12 = "Please select CSV File.";
        public static string SubblockError13 = "All Error data deleted successfully.";
        public static string SubblockError14 = "Please upload files having extensions csv only.";
        public static string ErrorDetailsGV = "Error Details";
        public static string ImportDateGV = "Import Date";
        public static string btnClearErrors = "Clear Error Table";
        public static string lblErrorDetails = "No error to display.";
        public static string Doyoureallywantclearerrortable = "Do you really want to clear the error table?";


        public static string suberr1 = "You have no previleges to perform this action.";
        public static string suberr2 = "Valve name can not be empty!";
        public static string suberr3 = "Please select Block.";
        public static string suberr4 = "Please select Zone.";
        public static string suberr5 = "SubBlock can not be configured. Please check all required fields and try again.";
        public static string suberr6 = "There are no Valve configured for selected RTU.";

        public static string appendsub1 = "Not Assigned";
        public static string submodOwner = "Owner Names";
        public static string submodAsccessor = "Accessor Names";

        #endregion

        #region MaintainSubBlockOwnerAccessor
        public static string Searchstringshouldbeminimum3characterlong = "Search string should be minimum 3 character long";
        public static string TotalareaownermustsmallerthanSubBlockarea = "Total area of owner must be smaller than SubBlock area.";
        public static string Pleaseselectatleastoneuser = "Please select atleast one user";
        public static string PleaseenterOwnerSubBlockArea = "Please enter Owner SubBlock Area";

        public static string FirstNameMSB = "First Name";
        public static string LastNameMSB = "Last Name";
        public static string AddressMSB = "Address";
        public static string MobileNumberMSB = "Mobile Number";
        public static string UserNameMSB = "User Name";
        public static string DesignationMSB = "Designation";
        public static string WorkAreaLocationMSB = "Work Area Location";
        public static string AccessAreaMSB = "Access Area";
        public static string RoleMSB = "Role";
        public static string OwnerSubBlockAreaMSB = "Owner SubBlock Area";
        public static string AssignOwnertoSubBlockMSB = "Assign Owner to SubBlock";
        public static string NodatafoundMSB = "No data found";
        public static string OwnerOfSubBlockMSB = "Owner Of SubBlock";
        public static string RemoveOwnerfromSubBlockMSB = "Remove Owner from SubBlock";
        #endregion

        #region RTUConfig
        public static string NoDataFoundRC = "No Data Found";
        public static string PleaseselectRC = "Please select";
        public static string SelectedFertPumpGroupisnotconfigured = "Selected Fert Pump Group is not configured.";
        public static string SelectedFilterGroupisnotconfigured = "Selected Filter Group is not configured.";
        public static string SelectedMasterGroupisnotconfigured = "Selected Master Group is not configured.";
        public static string shouldbelessthan100 = "should be less than 100.";
        public static string shouldnotbelessthan = "should not be less than";
        public static string shouldbewithin = "should be with in";
        public static string and = "and";
        public static string shouldbelessthan = "should be less than";
        public static string valueshouldbeGreaterthan0 = "value should be Greater than 0.";
        public static string Pleaseselectsensor = "Please select sensor";
        public static string Nousersaddedtoproject = "No users added to project.";

        public static string title1 = "Alarm Level Information";
        public static string content1 = "No Alarm";
        public static string content2 = "This alarm will be displayed for 1 min and will be recorded as general event in event logbook (no acknowledgement needed)";
        public static string content3 = "Display and after acknowledgement , recorded as critical event in event logbook";
        public static string content4 = "Display in different colour and after acknowledgement , recorded as critical event in event logbook";
        public static string content5 = "Display in different colour and blink. After acknowledgement , recorded as critical event in event logbook";
        public static string content6 = "Display in different colour and blink + Send text SMS to selected mobile no.  After acknowledgement , recorded as critical event in event logbook";
        public static string content7 = "Display in different colour and blink + Send text SMS to selected mobile no. + repeat message after prescheduled time.  After acknowledgement , recorded as critical event in event logbook";
        public static string content8 = "Display in different colour and blink + Send prerecorded voice tags to selected mobile no. + repeat message after prescheduled time.  After acknowledgement , recorded as critical event in event logbook";
        public static string defaultErrorMsg = "No default values are configured.";

        #endregion

        #region Rule
        public static string DoyouwanttodeleteRule = "Do you want to delete?";
        public static string Doyouwanttoresettherulethroughmanualoverride = "Do you want to reset the rule through manual override?";
        public static string EditRuleRule = "Edit Rule";
        public static string RuleNameRule = "Rule Name";
        public static string RuleExecutedFromRule = "Rule Executed From";
        public static string TagNameRule = "Tag Name";
        public static string AlarmRule = "Alarm";
        public static string ActionRule = "Action";
        public static string RuleStartTime = "Rule Start Time";
        public static string RuleEndTime = "Rule End Time";
        public static string ResetTypeRule = "ResetType";
        public static string StatusRule = "Status";
        public static string UserNameRule = "User Name?";
        public static string lnkRemove = "Delete";
        public static string lnkResetRule = "Reset";
        public static string btnConfigureRules = "Create Rule";
        public static string ruleresetsucc = "Rule reset successfully";
        public static string strWhereConditionForStatus = "0,1,2";
        public static string Executed = "Executed";
        public static string Reset = "Reset";
        #endregion

        #region RuleValidator
        public static string AnyunsaveddatawillbelostDoyouwanttocontinue = "Any unsaved data will be lost Do you want to continue?";
        public static string ConditionRV = "Condition";
        public static string Connector = "Connector";
        public static string RuleQuery = "RuleQuery";
        public static string NoConditionsAdded = "No Conditions Added";
        public static string TargetElementName = "Target Element Name";
        public static string YouhavenoaccessofNetwork = "You have no access of Network.";
        public static string Pleasereconfiguretheuseraccessarea = "Please reconfigure the user access area.";

        public static string Pleaseselectruleexefrom = "Please select rule exe from.";
        public static string NonetworksconfiguredPleaseconfigurenetworkstryagain = "No networks configured. Please configure networks try again.";
        public static string Pleaseselectnetworkno = "Please select network no.";
        public static string Pleaseselectrulestarttimehour = "Please select rule start time: hour.";
        public static string PleaseselectrulestarttimeMinutes = "Please select rule start time: Minutes.";
        public static string Pleaseselectruleendtimehour = "Please select rule end time: hour.";
        public static string PleaseselectruleendtimeMinutes = "Please select rule end time: Minutes.";
        public static string Pleaseselectdelaytoconfirmminutes = "Please select delay to confirm: minutes.";
        public static string PleaseselectdelaytoconfirmSeconds = "Please select delay to confirm: Seconds";
        public static string Pleaseselecteithermanualresetorruleresettime = "Please select either manual reset or rule reset time.";
        public static string Pleaseselectruleresettimehour = "Please select rule reset time: hour.";
        public static string PleaseselectruleresettimeMinutes = "Please select rule reset time: minutes.";
        public static string PleaseselectruleresettimeSeconds = "Please select rule reset time: Seconds.";
        public static string Pleaseaddtagnameforrule = "Please add tag name for rule.";
        public static string AlarmlevelsnotconfiguredPleaseconfigurealarmlevelsandtryagain = "Alarm levels not configured. Please configure alarm levels and try again";
        public static string Pleaseselectalarmlevel = "Please select alarm level.";
        public static string NoactionsconfiguredPleaseconfigureactionsandtryagain = "No actions configured. Please configure actions and try again.";
        public static string Pleaseselectaction = "Please select action.";
        public static string Pleaseselectrulerepeattimehour = "Please select rule repeat time: hour.";
        public static string Pleaseselectrulerepeattimeminute = "Please select rule repeat time: Minute.";
        public static string Pleaseselectrulerearmtimehour = "Please select rule rearm time: hour.";
        public static string PleaseselectrulerearmtimeMinute = "Please select rule rearm time: Minutes.";
        public static string Repeattimecannotbezero = "Repeat time can not be zero.";
        public static string Rearamtimecannotbezero = "Rearam time can not be zero.";
        public static string Rearmtimemustbegreaterthanrepeattime = "Rearm time must be greater than repeat time.";
        public static string Repeattimemustbeatleast5mins = "Repeat time must be atleast 5 mins.";
        public static string Rearmtimemustbeatleast30mins = "Rearm time must be atleast 30 mins.";
        public static string NoconditionsaddedforrulePleaseaddconditionsandtryagain = "No conditions added for rule. Please add conditions and try again.";
        public static string NotargetsaddedforrulePleaseaddtargetsandtryagain = "No targets added for rule. Please add targets and try again.";
        public static string YouhavecreatedruleconditionsonsamenetworkToexecutetheruleonserveryoushouldspecifycrossnetworkconditions = "You have created rule conditions on same network. To execute the rule on server, you should specify cross network conditions.";
        public static string YouhavecreatedruletargetsonsamenetworkToexecutetheruleonserveryoushouldspecifycrossnetworktargets = "You have created rule targets on same network. To execute the rule on server, you should specify cross network targets.";
        public static string Rulecreatedsuccessfully = "Rule created successfully.";
        public static string Pleaseselectelementtype = "Please select element type.";
        public static string Pleaseselectelementname = "Please select element name.";
        public static string Pleaseselectelement = "Please select element.";
        public static string NoanalogelementsareconfiguredforthisnetworkPleaseconfigureanalogsensorsandtryagain = "Select Element Type (No analog elements are configured for this network. Please configure analog sensors and try again.)";
        public static string NodigitalcountersareconfiguredforthisnetworkPleaseconfiguredigitalcounterandtryagain = "Select Element Type (No digital counters are configured for this network. Please configure digital counter and try again.)";
        public static string NoNONCsensorsareconfiguredforthisnetworkPleaseconfiguredigitalNONCandtryagain = "Select Element Type (No NO/NC sensors are configured for this network. Please configure digital NO/NC and try again.)";
        public static string NooutputvalvesareconfiguredforthisnetworkPleaseconfigureoutputvalveandtryagain = "Select Element Type (No output valves are configured for this network. Please configure output valve and try again.)";
        public static string NogroupsareconfiguredforthisnetworkPleaseconfigureatleastoneoutputgroupandtryagain = "Select Element Type (No groups are configured for this network. Please configure at least one output group and try again.)";
        public static string NoprogramsareconfiguredPleaseconfigureatleastoneprogramandtryagain = "No programs are configured. Please configure at least one program and try again.";
        public static string NosequencesareconfiguredPleaseconfigureatleastonesequenceandtryagain = "No sequences are configured. Please configure at least one sequence and try again.";
        public static string NorulesareconfiguredPleasedefineatleastoneruleandtryagain = "No rules are configured. Please define at least one rule and try again.";
        public static string Nooperatortypesspecified = "No operator types specified.";
        public static string Pleaseselectoperator = "Please select operator.";
        public static string NovalvesconfiguredforthisgroupPleaseaddvalvesinthegroupandtryagain = "No valves configured for this group. Please add valves in the group and try again.";
        public static string NovalvesconfiguredforthisprogramPleaseaddvalvesinthegroupandtryagain = "No valves configured for this program. Please add valves in the group and try again.";
        public static string NovalvesconfiguredforthissequencePleaseaddvalvesinthegroupandtryagain = "No valves configured for this sequence. Please add valves in the group and try again.";
        public static string Nofieldvalueconfiguredforruleexecution = "No field value configured for rule execution.";
        public static string Pleaseselectoperand2 = "Please select operand 2.";
        public static string Pleaseenterrangevalue1 = "Please enter range value 1.";
        public static string Pleaseenterrangevalue2 = "Please enter range value 2.";
        public static string Range1shouldbelessthanrange2 = "Range 1 should be less than range 2.";
        public static string Pleaseentercustomvalueforoprand2 = "Please enter custom value for oprand 2.";
        public static string Customvaluenotinrangeofselectedsensorscaleminmaxvalue = "Custom value not in range of selected sensor scale min,max value.";
        public static string Noframefieldconfiguredforruleexecution = "No frame field configured for rule execution.";
        public static string Pleaseselectoperand1 = "Please select operand 1.";
        public static string ThisconditionisalreadyaddedPleasespecifydifferentcondition = "This condition is already added. Please specify different condition.";
        public static string Youhaveadded = "You have added";
        public static string condtionstoruleYoucannotaddnewconditionnow = "condtions to rule. You cannot add new condition now.";
        public static string Ruleupdatedsuccessfully = "Rule updated successfully.";
        public static string YouhavenoaccessofZone = "You have no access of Zone.";
        public static string NosequencesareconfiguredNetPleaseconfigureatleastonesequenceandtryagain = "No sequence configured for this network. Please configure groups and try again.";
        public static string NozonesareconfiguredforthisprogramPleaseconfigurezoneandtryagain = "No zones are configured for this program. Please configure zone and try again.";
        public static string Pleaseselecttargettype = "Please select target type";
        public static string Notargetsconfigured = "No targets configured.";
        public static string Pleaseselecttargetstoviewsummary = "Please select targets to view summary.";
        public static string Pleaseselectsensorconditionstoviewsummary = "Please select sensor conditions to view summary.";
        public static string PleaseselectTargets = "Targets are not selected. ";
        public static string TargetAddedSucessfully = "Targets added Successfully. ";
        public static string TargetRemovedSuccessfully = "Targets Removed Successfully. ";
        #endregion

        #region ViewExecuteRules
        public static string RuleNameVER = "Rule Name";
        public static string RuleExecutedFromVER = "Rule Executed From";
        public static string DescriptionVER = "Description";
        public static string AlarmVER = "Alarm";
        public static string ActionVER = "Action";
        public static string ResetTypeVER = "ResetType";
        public static string StatusVER = "Status";
        public static string UserNameVER = "User Name";
        public static string lnkResetRuleVER = "Reset";
        public static string NoRulesExecutedVER = "No Rules Executed";
        public static string VER1 = "Do you want to reset the rule through manual override?";
        public static string VER2 = "Rule Name";
        public static string VER3 = "Rule Name";
        #endregion

        #region MasterValveGroupDetail

        public static string PleaseselectZoneMV = "Please select Zone.";//for all group
        public static string PleaseselectGroupTypeMV = "Please select Group Type.";//for all group
        public static string OutputValveMV = "Output Valve";
        public static string PrimarySensor1MV = "Primary Sensor1";
        public static string PrimarySensor2MV = "Primary Sensor2";
        public static string btnAddMS = "Save/ Add valve to Group";
        public static string AvailableGroupsMV = "Available Groups";//for all group
        public static string ConfigwithNewMasterMV = "Config with New Master";
        public static string EditGroupMV = "Edit Group";//for all group
        public static string ConfigwithNewMasterValveMV = "Config with New Master Valve";
        public static string GroupDetailsMV = "Group Details";//for all group

        public static string zoneMS = "Zone";
        public static string OutputGroupTypeMS = "Output Group Type";
        public static string GroupNameMS = "Group Name";
        public static string GroupNameMV = "Master Group No";


        public static string YouhavenoprivilegetoconfigureZone = "You have no privilege to configure Zone.";//for all group
        public static string GroupcreatedsuccessfullyMV = "Group created successfully.";//for all group
        public static string result1 = "Number of master valve group for selected zone is 0. To add new group please increase Number of Master Valves in Zone.";
        public static string result2 = "All master valve groups in selected zone are created. To add new group please increase Number of Master Valves in Zone.";
        public static string PleaseselectRTUNoMV = "Please select RTU  No.";//for all group
        public static string PleaseenterTagNameMV = "Please enter Tag Name.";//for all group
        public static string PleaseselectActiononcommunicationfailuare = "Please select Action on communication failuare.";
        public static string PleaseconfigureselectedGroupNoofUpStreammasterMV = "Please configure selected Group No. of Up Stream master.";
        public static string RTUNoandOutputValvehavingconflictcannotsave = "RTU No. and Output Valve having conflict cannot save.";
        public static string SelectedGroupNoofUpStreammasterisofdifferentZone = "Selected Group No. of Up Stream master is of different Zone.";
        public static string msg1 = "Selected Group No. of Up Stream master is of different Network.Opreation May Delayed.<br /> Master Valve Data Saved Successfully";
        public static string msg2 = "Selected Group No. of Up Stream master is of different Network.Opreation May Delayed.<br /> Master Valve Data Saved Successfully";
        public static string msg3 = "<br /> Master Valve Data Saved Successfully";
        public static string msg4 = "Master Valve Data Saved Successfully";
        public static string YouhavenotaddedMasterValveinProjectLiabrary = "You have not added Master Valve in Project Liabrary.";
        public static string msg5 = "Selected Group No. of Up Stream master is of different Network.Opreation May Delayed.";
        public static string PleaseselectPrimarysensorMV = "Please select Primary sensor.";//for all group
        public static string PleaseselectSecondarysensorMV = "Please select Secondary sensor.";//for all group
        public static string NoOPvalveconfiguredforselectedRTU = "No O/P valve configured for selected RTU.";//for all group
        public static string PleaseselectOutputValve = "Please select Output Valve.";//for all group
        public static string PleaseconfigureRTUfornetworkfirst = "Please configure RTU for network first.";
        public static string PleaseselectRTUNo = "Please select RTU No.";
        public static string YouhavenotConfiguretheOutputValvetoRTU = "You have not Configure the Out put Valve to RTU ";
        public static string msg6 = "Selected Group No. of Up Stream master is of different Network.Opreation May Delayed.";

        public static string errmsg1 = "Cannot delete group. It contains Output valves which are used in sequence or this Group is used in Valve Configuration or this Group is used in Rule or Dummy Sequence. Delete it's reference and reconfigure.";//for all group

        public static string mverr = "Master Valve used in MO.Cannot deleted.";
        #endregion

        #region PumpStationValveGroupDetail

        public static string PleaseenterNoofPumpstobeused = "Please enter No. of Pumps to be used.";
        public static string PleaseenterNoofstepstobeused = "Please enter No. of steps to be used.";
        public static string NoofPumpsshouldbegreaterthan0 = "No. of Pumps should be greater than 0.";
        public static string NoofPumpstobeusedvalueshouldgreaterthan0 = "No. of Pumps to be used value should greater than 0.";
        public static string NoofPumpstobeusedvalueshouldbelessthanorequalto20 = "No. of Pumps to be used value should be less than or equal to 20.";
        public static string Noofstepstobeusedvalueshouldbegreaterthan0 = "No of steps to be used value should be greater than 0.";
        public static string Noofstepstobeusedvalueshouldbelessthanorequalto10 = "No. of steps to be used value should be less than or equal to 10.";
        public static string PleaseconfigureOPNoasFertPumpforselectedRTUandthenreconfgure = "Please configure O/P No. as Fert. Pump for selected RTU and then reconfgure.";
        public static string Program = "Program";
        public static string TagNamePump = "Tag Name";
        public static string PumpNo = "Pump No";
        public static string opnoPU = "o/p no.";
        public static string PrimarySensorPU = "Primary Sensor";
        public static string SecondarySensorPU = "Secondary Sensor";
        public static string DesignedPumpFlowPU = "Designed Pump Flow";
        public static string NominalPressurePU = "Nominal Pressure";
        public static string THStep1 = "Step 1";
        public static string THStep2 = "Step 2";
        public static string THStep3 = "Step 3";
        public static string THStep4 = "Step 4";
        public static string THStep5 = "Step 5";
        public static string THStep6 = "Step 6";
        public static string THStep7 = "Step 7";
        public static string THStep8 = "Step 8";
        public static string THStep9 = "Step 9";
        public static string THStep10 = "Step 10";
        public static string CommunicationFailureStep = "Communication Failure Step";
        public static string Delay = "Delay";

        public static string ConfigwithNewMasterPumpStation = "Config with New Master Pump Station";

        public static string resultPU1 = "Number of master pump station group for selected zone is 0. To add new group please increase Number of Master Pump in Zone.";
        public static string resultPU2 = "All Master Pump groups in selected zone are created. To add new group please increase Number of Master Pump in Zone.";
        public static string ThisgroupisusedinfollowingSequence = "This group is used in following Sequence:";
        public static string ThisgroupisusedinfollowingRuleTargets = "This group is used in following Rule Targets:";
        public static string ThisgroupisusedinfollowingRuleConditions = "This group is used in following Rule Conditions:";
        public static string ThisgroupisusedinfollowingChannelsconfiguration = "This group is used in following Channels configuration:";
        public static string ThisgroupisusedinfollowingMasterValveGroupasUpstreamMaster = "This group is used in following Master Valve Group as Upstream Master:";
        public static string ThisgroupisusedinfollowingMannualOverride = "This group is used in following Mannual Override: ";
        public static string Thisgroupisnotinuse = "This group is not in use.";
        public static string PleaseconfigureRTUforthisnetworkbeforeconfiguringgroup = "Please configure RTU for this network before configuring group.";
        public static string AllpumpsareconfiguredPleasereducenoofpumpsandtryagain = "All pumps are configured. Please reduce no of pumps and try again.";
        public static string PumpalreadyconfiguredinotherstepPleaseselectdifferentpumpsandtryagain = "Pump already configured in other step. Please select different pumps and try again.";
        public static string PleaseselectOPno = "Please select O/P no.";
        public static string WarningTotaldesignflowexceedszoneflow = "Warning: Total design flow exceeds zone flow.";
        public static string errmsg2 = "Cannot change RTU. Steps are already configured. Please delete Group and Reconfigure.";
        public static string PumpstationgroupSavedSuccessfully = "Pump station group Saved Successfully";
        public static string PleaseselectRTUNofield = "Please select RTU No. field";
        public static string Pleaseenternoofpumps = "Please enter no of pumps";
        public static string NoofpumpsexceedsoutputcapacityofselectedRTU = "No of pumps exceeds output capacity of selected RTU.";
        public static string Youcanonlyconfigurepumpstationformaximum20pumps = "You can only configure pump station for maximum 20 pumps";
        public static string Pleaseenternoofsteps = "Please enter no of steps";
        public static string Youcanonlyconfigure10stepsperpumpstation = "You can only configure 10 steps per pump station";
        public static string YouhavenotaddedFertPumpinProjectLiabrary = "You have not added Fert Pump in Project Liabrary.";
        public static string NoinputsensorsconfiguredfortheselectedRTU = "No input sensors configured for the selected RTU.";
        #endregion

        #region FilterValveGroupDetail

        public static string PleaseselectOperationType = "Please select Operation Type";
        public static string PleaseselectPauserelatedFertpumpwhileFlush = "Please select Pause related Fert. pump while Flush.";
        public static string PleaseenterFlushTimeminvolume = "Please enter Flush Time, MM:SS/volume.";
        public static string PleaseselectDelayBetweenFlushsec = "Please select Delay Between Flush, sec.";
        public static string PleaseenterOffsetforfilterflushStartaftervalvestartmin = "Please enter Offset for filter flush Start after valve start, min.";
        public static string PleaseselectWaterMeterNoforvolumetricoperation = "Please select Water Meter No. for volumetric operation";
        public static string PleaseenterMaximumreiterationthroughPDmode = "Please enter Maximum reiteration through PD mode.";
        public static string PleaseselectActionafterMaxiteration = "Please select Action after Max. iteration.";
        public static string PleaseselectPDSensor = "Please select PD Sensor.";
        public static string resultFI = "Number of Filter group for selected zone is 0. To add new group please increase Number of Master Valves in Zone.";
        public static string resultFI1 = "All Filter groups in selected zone are created. To add new group please increase Number of Master Valves in Zone.";
        public static string resultFI2 = "Cannot delete group. It contains Output valves which are used in sequence or this Group is used in Valve Configuration or this Group is used in Rule or Dummy Sequence. Delete it's reference and reconfigure.";
        public static string OutputValveFI = "Output Valve";
        public static string OperatingsequenceofflushFI = "Operating sequence of flush";
        public static string Pressuresustainingopno = "Pressure sustaining o/p no.";
        public static string btnAddFI = "Save/ Add valve to Group";
        public static string ConfigwithNewFilter = "Config with New Filter";
        public static string ConfigwithNewFilterGroup = "Config with New Filter Group";
        public static string NoRTUconfigured = "No RTU configured.";
        public static string errorFI1 = "No Water Meter No. for volumetric operation configured for selected RTU.";
        public static string errorFI2 = "No Digital input type:Pressure switch configured for selected RTU.";
        public static string FilterValveDataSavedSuccessfully = "Filter Valve Data Saved Successfully";
        public static string OperatingsequenceofflushisemptyDatanotsaved = "Operating sequence of flush is empty. Data not saved.";
        public static string NoFilterTypeandPressuresustainingopnoconfiguredforselectedRTU = "No Filter Type and Pressure sustaining o/p no. configured for selected RTU.";
        public static string NoFilterTypeconfiguredforselectedRTU = "No Filter Type configured for selected RTU.";
        public static string NoPressuresustainingopnoconfiguredforselectedRTU = "No Pressure sustaining o/p no. configured for selected RTU.";
        public static string PleaseenterOperatingsequenceofflush = "Please enter Operating sequence of flush";
        public static string YouhavenotaddedPRSustaninginProjectLiabrary = "You have not added PR Sustaning in Project Liabrary.";
        public static string YouhavenotaddedFiltertypeinProjectLiabrary = "You have not added Filter type in Project Liabrary.";

        #endregion

        #region Fert

        public static string PleaseselectFertCounterNo = "Please select Fert Counter No.";
        public static string WaterBeforeFert = "Water Before Fert";
        public static string TypeOfOperationFR = "Type Of Operation";
        public static string DurationFR = "Duration";
        public static string NominalSuctionRateFR = "Nominal Suction Rate";
        public static string UnitFR = "Unit";
        public static string btnAddFR = "Save/ Add";
        public static string TypeofOpFR = "Type of O/p";
        public static string OutputValveFR = "Output Valve";
        public static string PrimarySensorFR = "Primary Sensor";
        public static string SecondarySensorFR = "Secondary Sensor";
        public static string DelaySecFR = "Delay, Sec";
        public static string btnAddGr = "Save/ Add valve to Group";
        public static string ConfigwithNewFert = "Config with New Fert";
        public static string ConfigwithNewFertGroup = "Config with New Fert Group";
        public static string resFR1 = "Number of Fert group for selected zone is 0. To add new group please increase Number of Fert in Zone.";
        public static string resFR2 = "All Fert groups in selected zone are created. To add new group please increase Number of Fert in Zone.";
        public static string errFERT1 = "Cannot delete group. It contains Output valves which are used in sequence or this Group is used in Valve Configuration or this Group is used in Rule or Dummy Sequence. Delete it's reference and reconfigure.";
        public static string msgFert1 = "Can not delete Setting. This Setting is already used in following sequence. Delete its reference and reconfigure.";
        public static string FertValveDataSavedSuccessfully = "Fert Valve Data Saved Successfully";
        public static string PleaseconfigurefertcounterforRTUfirst = "Please configure fert counter for RTU first.";
        public static string Waterbeforefertfieldvaluemustbegreaterthan0 = "Water before fert. field value must be greater than 0.";
        public static string Durationfieldvaluemustbegreaterthan0 = "Duration field value must be greater than 0.";
        public static string Nominalsuctionratefieldvaluemustbegreaterthan0 = "Nominal suction rate  field value must be greater than 0.";
        public static string Pleaseenterwaterbeforefert = "Please enter water before fert.";
        public static string Pleaseenterduration = "Please enter duration.";
        public static string PleaseselectTypeofOperation = "Please select Type of Operation.";
        public static string Pleaseenternominalsuctionrate = "Please enter nominal suction rate.";
        public static string PleaseselectUnitFR = "Please select Unit.";
        public static string PleaseselectTypeOfOpFERT = "Please select Type Of O/p.";
        public static string PleaseselectDelay = "Please select Delay.";
        public static string PleaseconfigureFertcounternoforselectedRTUandreconfigure = "Please configure Fert counter no. for selected RTU and reconfigure.";


        #endregion

        #region Maintain Group

        public static string ConfigwithNewGroup = "Config with New Group";
        public static string ConfigwithValveGroup = "Config with Valve Group";
        public static string PleaseselectGroupTypeGR = "Please select Group Type";

        public static string errGr1 = "Cannot delete group. It contains Output valves which are used in sequence or this Group is used in Valve Configuration or this Group is used in Rule or Dummy Sequence. Delete it's reference and reconfigure.";

        #endregion

        #region GeneralValveGroupConfig

        public static string genMsg1 = "If group is configured in sequence, this valve will also be deleted from those sequences. Do you want to continue?";
        public static string genMsg2 = "You have entered 0 value for Maximum Permissible Group Flow. Do you want to continue?";
        public static string genMsg3 = "If group is configured in sequence, this valve will also be configured in those sequences. Do you want to continue?";
        public static string genMsg4 = "If group is configured in sequence, this valve will also be configured in those sequences. Do you want to continue?";
        public static string genMsg5 = "If group is configured in sequence, this valve will also be configured in those sequences. Do you want to continue?";
        public static string PleaseenterNominalFlow = "Please enter Nominal Flow.";
        public static string btnUsedGroup = "Click to see where this Group is used";
        public static string NominalFlowGR = "Nominal Flow";
        public static string OutputValveGR = "Output Valve";
        public static string PrimarySensorGR = "Primary Sensor";
        public static string SecondarySensorGR = "Secondary Sensor";
        public static string FlowUnitGR = "Flow Unit";
        public static string MasterTypeGr = "Master Type";
        public static string MasterGroupGR = "Master Group";
        public static string FertGroupGR = "Fert Group";
        public static string FilterGroupGR = "Filter Group";
        public static string TotalNominalflowshouldbelessthanorequaltomaxpermissiblegroupflow = "Total Nominal flow should be less than or equal to max permissible group flow.";
        public static string Thegroup = "The group ";
        public static string isaconfiguredinasequenceofprogram = "is a configured in a sequence of program";
        public static string Pleaseunlocktheprogramtoaddvalvetothisgroup = "Please unlock the program to add valve to this group.";
        public static string grerror1 = "Can not save Data. Maximum Permissible Group Flow is less than Total of Nominal flow.";
        public static string grerror2 = "Valve Data Saved with Maximum Permissible Group Flow as 0.";
        public static string grerror3 = "Valve Data Saved Successfully";
        public static string grerror4 = "Please enter Maximum Permissible Group Flow.";
        public static string grerror5 = "You have selected Master group from different Zone.";
        public static string grerror6 = "You have selected Master group from different Network";
        public static string grerror7 = "You have selected Fert group from different Zone.";
        public static string grerror8 = "You have selected Fert group from different Network.";
        public static string grerror9 = "You have selected Filter group from different Zone.";
        public static string grerror10 = "You have selected Filter group from different Network.";
        public static string grerror11 = "You have selected valves from different Network..";
        public static string grerror12 = "Operation may delayed.";
        public static string NoOPvalveconfiguredPleaseconfigureOPvalve = "No O/P valve configured. Please configure O/P valve.";
        public static string grerror13 = "Maximum Permissible Group Flow must be greater than 0.";
        public static string grerror14 = "Please enter Maximum Permissible Group Flow.";
        public static string grerror15 = "Cannot add valve to group. It contains Output valves which are used in sequence. delete its reference and reconfigure.";
        public static string grerror16 = "Not able to add valve in dummy sequence.";
        public static string grerror17 = "Valve already configured for this duration. You cannot configure for same time now.";
        public static string grerror18 = "This group is not used in any Sequence.";
        public static string genv = "Group used in MO. Cannot deleted.";
        #endregion

        #region ViewManualOverride
        public static string NameVMO = "Name";
        public static string TypeVMO = "Type";
        public static string UsernameVMO = "User name";
        public static string CreatedDatetimeddmmyyyy = "Created Datetime (dd/mm/yyyy)";
        public static string OverrideForVMO = "Override For";
        public static string TargetsVMO = "Target";
        public static string ActionVMO = "Action";
        public static string alarmLevel = "Alarm Level";
        public static string StatusVMO = "Status";
        public static string ExecutionDatetimeddmmyyyy = "Execution Datetime (dd/mm/yyyy)";
        public static string btnConfigureMO = "Add New Manual Override";
        public static string btnAllDeleteMO = "Delete Manual Override";
        public static string NoManualOverrideConfigured = "No Manual Override Configured";

        public static string MODELETEMsg1 = "Please Select atleast one Manual Override.";
        public static string MODELETEMsg2 = "Manual Override deleted successfully.";

        public static string btnBack = "Back";
        #endregion

        #region ManageManualOverride

        public static string confirmMMO = "Any unsaved data will be lost. Do you want to continue?";
        public static string Doyouwanttocontinue = "Do you want to continue?";
        public static string MOerror1 = "Please select manual override type.";
        public static string MOerror2 = "Please enter manual override end date.";
        public static string MOerror3 = "Please select manual override end hour.";
        public static string MOerror4 = "Please select manual override end minute.";
        public static string MOerror5 = "Please enter manual override start date.";
        public static string MOerror6 = "Please select manual override start hour.";
        public static string MOerror7 = "Please select manual override start minute.";
        public static string MOerror8 = "Please enter manual override end date";
        public static string MOerror9 = "Please select manual override end hour.";
        public static string MOerror10 = "Please select manual override end minute.";
        public static string MOerror11 = "Please select action of manual override.";
        public static string MOerror12 = "Please select Alarm Level of manual override.";
        public static string MOerror13 = "Please select network or zone to create manual override.";
        public static string MOerror14 = "Please select network or zone to create manual override.";
        public static string MOerror18 = "Please select either network or block to create manual override.";
        public static string MOAccessRightsError1 = "You can create MO for block, output or rtu only as you do not have access of whole network or zone.";
        public static string StartMMO = "Start";
        public static string EndMMO = "End";
        public static string ValveMMO = "Valve";
        public static string StartTimeMMOG = "Start Time";
        public static string EndTimeMMOG = "End Time";
        public static string NoSchedulesdefinedfortoday = "No Schedules defined for today";
        public static string NoMMO = "No";
        public static string MOerror15 = "is defined for selected combination. Please configure at least one";
        public static string MOerror16 = "and try again!";
        public static string ManualOverridecreatedsuccessfully = "Manual Override created successfully.";
        public static string MOerror17 = "Manual Override could not be added. Please check all values and try again.";
        public static string All = "All";
        public static string willbe = "will be";
        public static string Network = "Network";
        public static string Zone = "Zone";
        public static string Block = "Block";

        public static string monew1 = "Please select MO start date.";
        public static string monew2 = "Please select MO end date.";
        public static string monew3 = "MO start date should be before MO end date.";
        public static string monew4 = "MO start date before Current date.";
        public static string monew5 = "MO end date before Current date.";
        public static string monew6 = "Please select Output valve.";
        public static string monew7 = "MO start date before Current date.Cannot change date.";
        public static string monew8 = "MO end date before Current date.Cannot change date.";
        public static string monew9 = "MO Start time should be less than MO End Time.";
        public static string monew10 = "MO end time should not less than or equal MO start Time.";
        #endregion

        #region Alarm Summery

        public static string alerterror1 = "Please select alarm for acknowledgement!";
        public static string alerterror2 = "Event:";
        public static string alerterror3 = "DateTime:";
        public static string alerterror4 = "Alarm Level:";
        public static string alerterror5 = "Clear";
        public static string Filter = "Filter";
        public static string alerterror6 = "Filter By:";
        public static string AlarmList = "Alarm List";
        public static string Alarms = "Alarms";
        public static string EventDateTime = "Event Date Time";
        public static string AlarmlevelAl = "Alarm level";
        public static string AckDateTime = "Ack DateTime";
        public static string ValuefromBST = "Value from BST";
        public static string ActualValue = "Actual Value";
        public static string ThresholdLimit = "Threshold Limit";
        public static string Status = "Status";


        #endregion

        #region DisplayEvents

        public static string Doyouwanttodeletealldata = "Do you want to delete all data?";
        public static string diseveerr1 = "From Date must be before End Date.";
        public static string btnExport1 = "Export To Excel";
        public static string btnDeleteAllData = "Delete all data";
        public static string DateTimeDE = "Date Time";
        public static string NetworkNameDE = "Network Name";
        public static string ElementTypeDE = "Element Type";
        public static string ElementNumberDE = "Element Number";
        public static string RTUNumberDE = "RTU Number";
        public static string StatusDE = "Status";
        public static string ReasonDE = "Reason";
        public static string ErrorDE = "Error";
        public static string BSTBatVoltDE = "BST Bat Volt";
        public static string BaseStationIdDE = "Base Station Id";
        public static string ReceivedDatetimeDE = "Received Datetime";
        public static string diseveerr2 = "Please enter valid username and password.";
        public static string diseveerr3 = "Unable to fetch username and password.";
        public static string diseveerr4 = "Please enter Date Time.";

        #endregion

        #region DisplayStatus

        public static string Pleaseenterusername = "Please enter user name";
        public static string Pleaseenterpassword = "Please enter password.";
        public static string StatusDetails = "Status Details";
        public static string DateTimeDS = "Date Time";
        public static string lblgridViewNWRTUDetailsNw = "Network";
        public static string BSTBatteryVoltageDS = "BST Battery Voltage (%)";
        public static string Label2 = "BST Charging Status";
        public static string SlotDS = "Slot";
        public static string RFSignalStrengthDS = "RF Signal Strength";
        public static string HopesDS = "Hopes";
        public static string RTUBatteryVoltageDS = "RTU Battery Voltage (%)";
        public static string Label3 = "RTU Charging Status";
        public static string RTUAuxillarySenseDS = "RTU Auxillary Sense";
        public static string RTUBoosterVoltageDS = "RTU Booster Voltage (%)";
        public static string ShowDetailsDS = "Show Details";
        public static string ChannelDS = "Channel";
        public static string AnalogValueDS = "Analog Value (%)";
        public static string CalculatedAnalogvalue = "Calculated Analog value";
        public static string DigitalStatus = "Digital Status";
        public static string DigitalCounterValue = "Digital Counter Value";
        public static string CalculatedDigitalCounterValueltr = "Calculated Digital Counter Value (ltr)";
        public static string DigitalFlow = "Digital Flow";
        public static string CalculatedDigitalFlowValuelps = "Calculated Digital Flow Value (lps)";
        public static string Nodatafound1 = "No data found";
        public static string Nodatafound2 = "No data found";
        public static string ValveStatusDS = "Valve Status";

        #endregion

        #region EventLogBook

        #endregion

        #region MaintainEventLogBook
        public static string evMsg1 = "From Date must be before End Date than To Date.";
        public static string evMsg2 = "Please Select From Date - To Date";
        public static string evMsg3 = "Please Select Zone";
        public static string evMsg4 = "Please Select Network";
        public static string evMsg5 = "Please Select Block";
        public static string evMsg6 = "Please Select RTU";
        public static string evMsg7 = "Please Select From Date - To Date";
        public static string valveGV1 = "Date & Time";
        public static string valveGV2 = "Event Type";
        public static string lblValveEventGridStatus = "Status";
        public static string lblValveEventGridChannelName = "Channel Name";
        public static string lblValveEventGridReason = "Reason";
        public static string valveGV3 = "MO ID/Rule ID";
        public static string NoDataFoundEV = "No Data Found...";
        public static string evMsg8 = "No Valve Data";
        public static string evMsg9 = "No Record";
        public static string evMsg10 = "No Threshold Data for";
        public static string evMsg11 = "No Data";
        public static string evMsg12 = "No Threshold Data";
        public static string evMsg13 = "No Sensor Data";
        public static string evMsg14 = "No Battery Low Events Data";
        public static string evMsg15 = "No User Data";
        public static string evMsg16 = "No Rule Data";
        public static string Server = "Server";
        public static string NetworkEV = "Network";
        public static string NONE = "NONE";
        public static string andEV = "AND";
        public static string OR = "OR";
        public static string evMsg17 = "No Manual Override Data";
        public static string evMsg18 = "No Pending Notifications Found";
        public static string evMsg19 = "No Pump Events Found";
        public static string AllEvents = "All Events";
        public static string RuleEvents = "Rule Events";
        public static string ValveEventsEV = "Valve Events";
        public static string SensorEvents = "Sensor Events";
        public static string CommunicationFailureEvents = "Communication Failure Events";
        public static string RTUBatteryVoltage = "RTU Battery Voltage Events";
        public static string BSTBatteryVoltage = "BST Battery Voltage Events";
        public static string BSTGSMSignalStrength = "BST GSM Signal Events";
        public static string HandheldDeviceEvents = "Handheld Device Events";
        public static string RTURFSignalStrength = "RTURF Signal Strength";
        public static string UserLoginEvents = "User Login Events";
        public static string BatteryEvents = "Battery Events";
        public static string MOEvents = "MO Events";
        public static string SensorStatusEvents = "Sensor Status Events";
        public static string WaterMeterEvents = "Water Accumulation";
        public static string PendingsNotificationsEvents = "Pendings Notifications Events";
        public static string BSTLogs = "BST Logs";
        public static string DO20Stats = "20 DO Valve Status";
        public static string PumpEvents = "Pump Station Events";
        public static string thsh1 = "Date & Time";
        public static string thsh2 = "Event Type";
        public static string thsh3 = "Status";
        public static string lblThresholdEventLogChannelName = "Element Type";
        public static string thsh4 = "Reason";
        public static string thsh5 = "Description";
        public static string thsh6 = "Alarm Level";
        public static string thsh7 = "Threshold";
        public static string thsh8 = "Actual Value";
        public static string NoDataFoundEV1 = "No Data Found...";
        public static string NoDataFoundEV2 = "No Data Found...";
        public static string sen1 = "Date";
        public static string sen2 = "Element Type";
        public static string sen3 = "Element Name";
        public static string sen4 = "Sensor Min";
        public static string sen5 = "Sensor Max";
        public static string sen6 = "Total";
        public static string NoDataFoundEV3 = "No Data Found...";
        public static string lblBatteryLowEventGridObjectName = "Event Type";
        public static string bt1 = "Date Time";
        public static string bt2 = "Event Type";
        public static string bt3 = "Status(%)";
        public static string bt4 = "Element Type";
        public static string bt5 = "Description";

        public static string bttt1 = "Date";
        public static string bttt2 = "Event Type";
        public static string bttt3 = "Element Type";
        public static string bttt4 = "Min Voltage";
        public static string bttt5 = "Max Voltage";
        public static string NoDataFoundEV4 = "No Data Found...";

        public static string rule1 = "Date & Time";
        public static string rule2 = "Event Type";
        public static string lblRulesEventGridStatus = "Status";
        public static string lblRulesEventGridtargetelements = "Target Elements";
        public static string rule3 = "Output";
        public static string lblRulesEventGridReason = "Reason";
        public static string rule4 = "Tag Name";
        public static string rule5 = "Rule Duration";
        public static string NoDataFoundEV5 = "No Data Found...";

        public static string NoDataFoundEV6 = "No Data Found...";
        public static string mo1 = "Date & Time";
        public static string mo2 = "Event Type";
        public static string mo3 = "Element Type";
        public static string mo4 = "Output";
        public static string mo5 = "Status";
        public static string mo6 = "Reason";
        public static string mo7 = "Description";

        public static string NoDataFoundEV7 = "No Data Found...";
        public static string ule1 = "First Name";
        public static string ule2 = "Last Name";
        public static string ule3 = "User Name";
        public static string ule4 = "User Role";
        public static string ule5 = "Mobile No.";
        public static string ule6 = "Login Time";
        public static string ule7 = "Logout Time";
        public static string ule8 = "Logout Reason";

        public static string NoDataFoundEV8 = "No Data Found...";
        public static string ssdl1 = "RTU TimeStamp";
        public static string ssdl2 = "Network";
        public static string ssdl3 = "RTU Name";
        public static string ssdl4 = "Element Name";
        public static string ssdl5 = "Analog Value (BST)";
        public static string ssdl6 = "Actual Value";
        public static string ssdl7 = "Digital Counter Value";
        public static string ssdl8 = "Calculated Digital Counter Value (ltr)";
        public static string ssdl9 = "Digital Flow (BST)";
        public static string ssdll0 = "Calculated Digital Flow Value (lps)";
        public static string ssdl11 = "Calculated Sensor Value";

        public static string NoDataFoundEV11 = "No Data Found...";
        public static string NoDataFoundEV9 = "No Data Found...";
        public static string pn1 = "Date Time";
        public static string pn2 = "Status";
        public static string pn3 = "Channel Name";
        public static string pn4 = "Sensor Type";
        public static string pn5 = "Reason";
        public static string pn6 = "Hand Held No";

        public static string rcon = "Condition";
        public static string rconn = "Connector";
        public static string rrq = "RuleQuery";
        public static string nca = "No Conditions Added";

        public static string pnd1 = "Network";
        public static string pnd2 = "BST Sent Date";
        public static string pnd3 = "Element Name";
        public static string pnd4 = "Processing";
        #endregion

        #region Change Password

        public static string chpass1 = "Please enter password.";
        public static string chpass2 = "Please enter confirm password.";
        public static string chpass3 = "Changing password will redirect to login page. Do you want to continue?";
        public static string chpass4 = "Password and confirm password must be same.";
        public static string chpass5 = "Password should contain at least six Caracters.";
        public static string chpass6 = "Password Updated Succesfully. Please login again with new password.";

        #endregion

        #region MyCaptcha



        #endregion

        #region UserSignUp
        /*Don't Delete.. Use in page already*/
        public static string usu1 = "Farmer";
        public static string usu2 = "Project Engineer";
        public static string usu3 = "Exec. Engineer";
        public static string usu4 = "Supervisory Officer";
        public static string usu5 = "Work Area Location Information";
        public static string usu6 = "eg.";
        public static string usu7 = "Supervisory Office: Work Area";
        public static string usu8 = "Farmer: Field No., Gat No., Survey No.";
        public static string usu9 = "User Asso. : Work Area, Nos of user";
        public static string usu10 = "Designation Information";
        public static string usu11 = "Please enter the First Name.";
        public static string usu12 = "First Name must be less than 255 character.";
        public static string usu13 = "Please enter the Last Name.";
        public static string usu14 = "Last Name must be less than 255 character.";
        public static string usu15 = "Contact Address must be less than 255 character.";
        public static string usu16 = "Designation must be less than 255 character.";
        public static string usu17 = "Please enter the Work Area Location.";
        public static string usu18 = "Work Area Location must be less than 255 character.";
        public static string usu19 = "Please select the Register As.";
        public static string usu20 = "Please enter Register As.";
        public static string usu21 = "Register As Other must be less than 100 character.";
        public static string usu22 = "Please enter the Mobile Number.";
        public static string usu23 = "Mobile Number must be 10 digit.";
        public static string usu24 = "Entered email id is not valid email id.";
        public static string usu25 = "Please enter the Password.";
        public static string usu26 = "Password length must be greater than 6 character.";
        public static string usu27 = "Please enter the Confirm Password.";
        public static string usu28 = "Password and Confirm Password must be equal.";
        public static string usu29 = "Registration is sent for approval. On approval you will receive a message on registered Mobile Number.";
        public static string usu30 = "UserName";
        public static string usu31 = "is alredy in use.";
        public static string usu32 = "oops!, invalid text was entered.";

        #endregion

        #region Configure User

        public static string cu = "UserName";
        public static string cuu = "is alredy in use.";

        public static string cu1 = "Can not change user role. User is owner of subblock";
        public static string cu2 = "User data saved successfully!";
        public static string cu3 = "User data not found. Data not saved.";
        public static string cu4 = "New User created successfully!";
        public static string cu5 = "Data not saved as no Network is assigned. Please select atleast one network in selected Zone and try again.";
        public static string cu6 = "Data Not saved as network and zone assignment having conflict.";
        public static string cu7 = "Data not saved as no element is assigned.";
        public static string cu8 = "Data Not Saved. User role is changed. Please save user details then try again.";
        public static string cu9 = "Data Saved Successfully.";
        public static string cu10 = "Data Saved Successfully. No sensors assigned to selected user.";
        public static string cu11 = "Data Not Saved. Please try again later.";
        public static string cu12 = "User data saved successfully!";
        public static string cu13 = "User data not found saved successfully!";
        public static string cu14 = "User data not found. Data not saved. Please try again.";
        public static string cu15 = "Type -";
        public static string cu16 = "TagName-";
        public static string cu17 = "Please enter First Name.";
        public static string cu18 = "First Name must be less than 255 character.";
        public static string cu19 = "Please enter Last Name.";
        public static string cu20 = "Last Name must be less than 255 character.";
        public static string cu21 = "Contact Address must be less than 255 character.";
        public static string cu22 = "Designation must be less than 255 character.";
        public static string cu23 = "Please enter Work Area Location.";
        public static string cu24 = "Work Area Location must be less than 255 character.";
        public static string cu25 = "Please select Register As.";
        public static string cu26 = "Please add Subscription Period";
        public static string cu27 = "Please select Subscription Period Unit";
        public static string cu28 = "Please enter Register As.";
        public static string cu29 = "Register As Other must be less than 100 character.";
        public static string cu30 = "Please enter Mobile Number.";
        public static string cu31 = "Mobile Number must be 10 digit.";
        public static string cu32 = "Entered email id is not valid email id.";
        public static string cu33 = "Please enter Password.";
        public static string cu34 = "Password length must be greater than 6 character.";
        public static string cu35 = "Please enter Confirm Password.";
        public static string cu36 = "Password and Confirm Password should match.";
        public static string cu37 = "Please select Role.";
        public static string cu38 = "This will reset all Access area of user. Do you want to continue?";
        public static string cu39 = "Please select country name.";
        public static string cu40 = "Zones are not configured in project.";
        public static string cu41 = "Networks are not configured in project";
        public static string cu42 = "Blocks are not configured in selected zone.";
        public static string cu43 = "Please select Block.";
        public static string cu44 = "SubBlocks are not configured in selected Block.";
        public static string cu45 = "Please select language.";

        #endregion

        #region MaintainConfigOutput
        #endregion

        #region My Profile

        public static string mp1 = "Please enter the First Name.";
        public static string mp2 = "First Name must be less than 255 character.";
        public static string mp3 = "Please enter the Last Name.";
        public static string mp4 = "Last Name must be less than 255 character.";
        public static string mp5 = "Please enter the Contact Address.";
        public static string mp6 = "Contact Address must be less than 255 character.";
        public static string mp7 = "Please enter the Work Area Location.";
        public static string mp8 = "Work Area Location must be less than 255 character.";
        public static string mp9 = "Please enter the Mobile Number.";
        public static string mp10 = "Mobile Number must be 10 digit.";
        public static string mp11 = "Entered email id is not valid email id.";
        public static string mp12 = "Please enter User Name.";
        public static string mp13 = "User data not found!";
        public static string mp14 = "JainIrriCare: Your access has been deactivated for JainIrricare. You cannot change info.";
        public static string mp15 = "User data saved successfully!";



        #endregion

        #region UserRoleManagement

        public static string urm1 = "Please enter Role name to create role.";
        public static string urm2 = "Please select Role.";
        public static string urm3 = "Role Name";
        public static string urm4 = "Role Created Successfully";
        public static string urm5 = "Role Not Created.";
        public static string urm6 = "Role already exists";
        public static string urm7 = "Privileges Assigned Successfully";
        public static string urm8 = "Privileges Updated Successfully";
        public static string urm9 = "User is removed from the role";
        public static string urm10 = "Can not delete role. This role is assign to one or more users.";
        public static string urm11 = "Please enter Role Name.";
        public static string urm12 = "Role deleted succesfully.";

        public static string lnkDelete = "Delete";
        #endregion

        #region User Approval

        public static string ua1 = "Do you want to Approve all selected users? If user is not configured then that user will not be approved.";
        public static string ua2 = "Do you want to Delete all selected users?";
        public static string ua3 = "Search string should be minimum 3 character long.";
        public static string ua4 = "Following users are not approved as they are not configured";
        public static string ua5 = "User Name is-";
        public static string ua6 = "Following users are approved successfully:";
        public static string ua7 = "Error occured during approve user. Please try again later.";
        public static string ua8 = "Please select at least one user.";
        public static string ua9 = "Selected user deleted successfully.";

        public static string uag1 = "First Name";
        public static string uag2 = "Last Name";
        public static string uag3 = "Address";
        public static string uag4 = "Mobile Number";
        public static string uag5 = "User Name";
        public static string uag6 = "Email Id";
        public static string uag7 = "Designation";
        public static string uag8 = "Work Area Location/Gut No.";
        public static string uag9 = "Register As";
        public static string uag10 = "Access Area";
        public static string uag11 = "Role";
        public static string uag12 = "Configure";
        public static string uag13 = "Approve All";
        public static string uag14 = "Delete All";

        #endregion

        #region RegisteredUser

        public static string ru1 = "Are you sure you want to Allow/ Restrict Access this user?";
        public static string ru2 = "Do you really want to clear the error table?";
        public static string ru3 = "Are you sure you want to delete this user?";
        public static string ru4 = "Really want to delete this user?";
        public static string ru5 = "Please upload files having extensions csv only.";
        public static string ru6 = "Please select at least one user.";
        public static string ru7 = "Search string should be minimum 3 character long.";
        public static string ru8 = "Selected user deleted successfully.";
        public static string ru9 = "Error occured during approve user. Please try again later.";
        public static string ru10 = "Please select at least one user.";
        public static string ru11 = "File imported successfully.";
        public static string ru12 = "User deleted succesfully.";
        public static string ru13 = "Please try again later.";
        public static string ru14 = "No Data to import. Please add data in csv file and try again.";
        public static string ru15 = "Unable to read csv file. File contain blank space in Header. Please try again later.";
        public static string ru16 = "Unable to read csv file. Column names are different. Please try again later.";
        public static string ru17 = "Unable to read csv file. Please try again later.";
        public static string ru18 = "Please select CSV File.";
        public static string ru19 = "UserName";
        public static string ru20 = "is alredy in use.";
        public static string ru21 = "All Error data deleted successfully.";

        public static string RUNDf = "No data found for Registered Users";
        public static string rug1 = "Check To Deactive User";
        public static string rug2 = "Configure Personal Details";
        public static string lnkConfigUserPersonal = "Personal Detail";
        public static string lnkConfigUserPersonaltooltip = "Config User Personal Details";
        public static string rug3 = "Error Details";
        public static string rug4 = "Import Date";
        public static string rug5 = "Clear Error Table";
        public static string rug6 = "No error to display.";

        public static string rug7 = "File No";
        public static string rug8 = "File Name";
        public static string rug9 = "Uploded Date";
        public static string rug10 = "Uploded By";
        public static string rug11 = "Status";
        public static string rug12 = "Date Processed";

        #endregion

        #region MaintainFieldTechnician

        public static string mf1 = "Available Field Technicians";
        public static string mf2 = "Config with Field Technician";
        public static string mf3 = "Edit Field Technician";
        public static string mf4 = "Field Technician Name";
        public static string mf5 = "Field Technician cell number";
        public static string mf6 = "SMS Alert Facility";
        public static string mf9 = "Authority To use Hand Held Device";
        public static string mf10 = "Address";
        public static string mf11 = "This cell number is already exist!";

        #endregion

        #region ExpansionCardType

        public static string ec1 = "Wrong Selection";
        public static string ec2 = "Data Inserted Successfully";
        public static string ec3 = "Please select Expansion Card Type.";
        public static string ec4 = "Session Timeout. Please Login again.";
        public static string ec5 = "Data Not Inserted";
        public static string ec6 = "Please save Expansion Card Type.";

        #endregion

        #region MaintainSlot

        public static string msl1 = "Contains";
        public static string msl2 = "Expansion Cards!";
        public static string msl3 = "Available Slots";
        public static string msl4 = "Add new Slot";
        public static string msl5 = "Edit Slot";
        public static string msl6 = "Name";
        public static string msl7 = "Description";
        public static string msl8 = "Network Name";
        public static string msl9 = "RTU Id In Network";
        public static string msl10 = "Expansion Card Type";
        public static string msl11 = "Active";
        public static string msl12 = "Project Id";
        public static string msl13 = "Edit Expansion Card";
        public static string msl14 = "Edit Slot";

        #endregion

        #region BaseStationMaintain

        public static string bsm1 = "Please enter Tag Name for BaseStation 1.";
        public static string bsm2 = "Time Out of BaseStation 1 shoud be greater than or equal to 15.";
        public static string bsm3 = "Sim Numbers of BaseStations cannot be same.";
        public static string bsm4 = "Sim Number of BaseStation 1 should be 20 Digit.";
        public static string bsm5 = "Attendance Interval For BaseStation 1 should be minimum 1 Min.";
        public static string bsm6 = "Please enter Cell/Mobile Number for BaseStation 1.";
        public static string bsm7 = "Cell/Mobile Number of BaseStation 1 should be 10 digit.";
        public static string bsm8 = "BaseStation data saved successfully";
        public static string bsm9 = "BaseStation data not saved.";

        #endregion

        #region MaintainElement

        public static string meanalog = "&nbsp;Analog Sensor&nbsp;&nbsp;";
        public static string medigitalc = "&nbsp;Digital Counter&nbsp;&nbsp;";
        public static string medigitalnonc = "&nbsp;Digital NO/NC&nbsp;";

        public static string mel1 = "Type";
        public static string mel2 = "New Type Name";
        public static string mel3 = "Description";
        public static string mel4 = "Acknowledge";
        public static string mel5 = "ProjectAdmin";
        public static string lblNoData = "No request found.";

        public static string mel6 = "Please select Type.";
        public static string mel7 = "Please enter New Type Name.";
        public static string mel8 = "Please enter Description.";
        public static string mel9 = "Please select Project Admin.";
        public static string mel10 = "Please press on OK button to redirect to Global Library.";
        public static string mel11 = "Please fill below form and send request to admin.";
        public static string mel12 = "Add New Sensor/ Output";
        public static string mel14 = "New Type Requests";
        public static string mel15 = "Library Update";
        public static string mel16 = "Request send successfully.";

        public static string mel17 = "Default units set successfully.";
        public static string mel18 = "Please select Unit and then try again.";
        public static string mel19 = "Please select";
        public static string mel20 = "unit.";
        public static string mel21 = "Can not send Request. The request with given type name is already exist.";
        public static string mel22 = "Request send by you:";
        public static string mel23 = "Library update successfully.";

        #endregion

        #region UCElementGrid

        public static string uce1 = "Type";
        public static string uce2 = "Add";
        public static string uce3 = "Assign unit";
        public static string uce4 = "Delete";
        public static string uce5 = "Config";
        public static string uce6 = "Set as Default";
        public static string uce7 = "Save";
        public static string uce8 = "Save All Selected Units";
        public static string uce9 = "This RTU Type is not in use.";
        public static string uce10 = "This Output Type is not in use.";
        public static string uce11 = "This Analog Sensor Type is not in use.";
        public static string uce12 = "This Digital Counter Sensor Type is not in use.";
        public static string uce14 = "This Digital NO/NC Sensor Type is not in use.";
        public static string uce15 = "This Output Group Type is already assigned to Digital Output.";
        public static string uce16 = "This Analog Sensor Type is already assigned to Analog Sensor.";
        public static string uce17 = "This Digital Counter Sensor Type is already assigned to Digital Counter Sensor.";
        public static string uce18 = "This Digital NO/NC Sensor Type is already assigned to Digital NO/NC Sensor.";
        public static string uce19 = "This RTU Type is already assigned to RTU.";
        public static string uce20 = "Available Output Group Type in project";
        public static string uce21 = "Selected Output Group Type for project";
        public static string uce22 = "Available Output Group Type in project";
        public static string uce23 = "Selected Output Group Type for project";
        public static string uce24 = "Available RTU Type in project";
        public static string uce25 = "Selected RTU Type for project";
        public static string uce26 = "Available RTU Type in project";
        public static string uce27 = "Selected RTU Type for project";
        public static string uce28 = "Available Analog Sensor Type in project";
        public static string uce29 = "Selected Analog Sensor Type For project";
        public static string uce30 = "Available Analog Sensor Type in project";
        public static string uce31 = "Selected Analog Sensor Type For project";
        public static string uce32 = "Available Digital Counter Sensor Type in project";
        public static string uce33 = "Selected Digital Counter Sensor Type For project";
        public static string uce34 = "Available Digital Counter Sensor Type in project";
        public static string uce35 = "Selected Digital Counter Sensor Type For project";
        public static string uce36 = "Available Digital NO/NC Sensor Type in project";
        public static string uce37 = "Selected Digital NO/NC Sensor Type For project";
        public static string uce38 = "Available Digital NO/NC Sensor Type in project";
        public static string uce39 = "Selected Digital NO/NC Sensor Type For project";
        public static string uce40 = "Default unit for";
        public static string uce41 = "set successfully.";
        public static string uce42 = "All Default unit set successfully.";
        public static string uce43 = "Units not defined for";
        public static string uce44 = "Please check metadata and try again.";
        public static string uce45 = "No analog sensors defined in project. Please define analog sensors and try again.";
        public static string uce46 = "No analog sensors defined in project. Please define analog sensors and try again.";
        public static string uce47 = "Units not defined for";
        public static string uce48 = "Please check metadata and try again.";
        public static string uce49 = "Please select default unit for";
        public static string uce50 = "No RTU Type is set as default.";
        public static string uce51 = "Please configure details for this RTU Type. And then try again. No RTU Type is set as default.";
        public static string uce52 = "RTU Type set as Default successfully.";

        #endregion

        #region RTUConfigForRainCounter

        public static string rcfr1 = "Please select sensor.";
        public static string rcfr2 = "Data saved Successfully.";
        public static string rcfr3 = "Max. freq value should be in multiple of Pulse value";
        public static string rcfr4 = "Minimum Rain to pause irrigation value should be multiple of Pulse value";
        public static string rcfr5 = "Minimum Rainfall to resume irrigation value should be multiple of Pulse value";
        public static string rcfr6 = "Please increase Scan duration or decrease Rain to resume.";
        public static string rcfr7 = "No default values are configure yet.";

        #endregion

        #region RTUDetailsTempConfig

        public static string rdtc1 = "RTU details saved successfully.";
        public static string rdtc2 = "Channels in RTUs having Expantion Card are used in Groups or Sublock for this RTU Type.Cannot reduce number of Expantion Card.";
        public static string rdtc3 = "Default values are not assigned for selected Type.";
        public static string rdtc4 = "Default values are assigned for selected RTU Type";
        #endregion

        #region MaintainSequence

        public static string msq1 = "Sequence No.";
        public static string msq2 = "StartId";
        public static string msq3 = "Time Of Op";
        public static string msq4 = "Group No";
        public static string msq5 = "Valve";
        public static string msq6 = "Duration";
        public static string msq7 = "No Sequence Configured";
        public static string msq8 = "Error Details";
        public static string msq9 = "Please select Zone.";
        public static string msq10 = "No networks configured in connection with selected zone.";
        public static string msq11 = "Please select Application of sequence.";
        public static string msq12 = "Please select Run Sequence as field.";
        public static string msq13 = "Please select Type of operation.";
        public static string msq14 = "Please select Sequence Type.";
        public static string msq15 = "Program";
        public static string msq16 = "is in loop";
        public static string msq17 = "Dummy Sequence";
        public static string msq18 = "Sequence";
        public static string msq19 = "Sequence start date should be within Program Start Date and Program End Date.";
        public static string msq20 = "Data Not Saved";
        public static string msq21 = "Please enter sequence tag name.";
        public static string msq22 = "Please select sequence start date.";
        public static string msq23 = "Please select sequence end date.";
        public static string msq24 = "Please select sequence start date.";
        public static string msq25 = "Please select sequence end date.";
        public static string msq26 = "Sequence cannot end before program start date.";
        public static string msq27 = "Sequence cannot end before program loop start date.";
        public static string msq28 = "Sequence cannot start before program start date.";
        public static string msq29 = "Sequence cannot start before program loop start date.";
        public static string msq30 = "Sequence cannot start after program end date.";
        public static string msq31 = "Sequence cannot start after program loop end date.";
        public static string msq32 = "Sequence cannot end after program end date.";
        public static string msq33 = "Sequence cannot end after program loop end date.";
        public static string msq34 = "Sequence start date should be before sequence end date.";
        public static string msq35 = "Please select start time for this sequence.";
        public static string msq36 = "Please select start time for this sequence.";
        public static string msq37 = "Please select weekdays for which you want to create sequence.";
        public static string msq38 = "The configured sequence will not be executed as selected weekdays are not present within sequence date range.";
        public static string msq39 = "Please enter interval id days for which you want to create sequence.";
        public static string msq40 = "Please enter sequence tag name.";
        public static string msq41 = "Please select start time for dummy sequence.";
        public static string msq42 = "Please select start time for dummy sequence.";
        public static string msq43 = "Please select weekdays for which you want to create sequence.";
        public static string msq44 = "Please enter interval id days for which you want to create sequence.";
        public static string msq45 = "The program is locked. You cannot make changes to sequence now.";
        public static string msq46 = "No networks configured in connection with selected zone.";
        public static string msq47 = "Please select network.";
        public static string msq48 = "Please select zone.";
        public static string msq49 = "Days Interval exceed number of days in sequence.";
        public static string msq50 = "Programming For";
        public static string msq51 = "Changing start time will overflow day start time. Please change start time and try again.";
        public static string msq52 = "Valve already configured for this duration. You cannot configure for same time now.";
        public static string msq53 = "Sequence Saved Successfully.";
        public static string msq54 = "Please click \"Refresh Sequence\" button if changes are not reflected in valve grid.";
        public static string msq55 = "The program is locked. You cannot configure sequence now.";
        public static string msq56 = "The program is locked. You cannot edit sequence start time now.";
        public static string msq57 = "Changing start time will overflow day start time. Please change start time and try again.";
        public static string msq58 = "The program is locked. You cannot edit sequence start time now.";
        public static string msq59 = "Changing start time will overflow day start time. Please change start time and try again.";
        public static string msq60 = "The program is locked. You cannot make changes to sequence now.";
        public static string msq61 = "No networks configured in connection with selected zone.";
        public static string msq62 = "Dummy Sequence Created Successfully";
        public static string msq63 = "Subblock";





        #endregion

        #region DisplayHorizontalSeq

        public static string dhs1 = "Sequence Details";
        public static string dhs2 = "Start Time:";
        public static string dhs3 = "Start Time(hh:mm): ";
        public static string dhs4 = "Duration (hh:mm): ";
        public static string dhs5 = "Change start time";
        public static string dhs6 = "Element";
        public static string dhs7 = "Save Sequence";
        public static string dhs8 = "Add New Start";
        public static string dhs9 = "Add Next Start";
        public static string dhs10 = "Error Details";
        public static string dhs11 = "Any unsaved data will be lost. Do you want to continue?";
        public static string dhs12 = "Validating sequence may take several minutes to finish. \n Do you want to continue?";
        public static string dhs13 = "Do you want to delete?";
        public static string dhs14 = "Changing start time will redefine the whole sequence. \nThis may take several minutes to finish. \nDo you want to continue?";
        public static string dhs15 = "Changing duration will redefine the whole sequence. \nThis may take several minutes to finish.\nDo you want to continue?";
        public static string dhs16 = "The sequence has more than";
        public static string dhs17 = "valves configured.";
        public static string dhs18 = "Hence only two horizontal durations will be visible at a time.";
        public static string dhs19 = "The sequence is too big to view. Please try to export sequence, make any changes you want and import again!.";
        public static string dhs20 = "The sequence is too big to view. Please clear your browser data and try again.";
        public static string dhs21 = "Day end is at.";
        public static string dhs22 = "You can not add new start time as it will go beyond day end.";
        public static string dhs23 = "Please enter start time for this sequence.";
        public static string dhs24 = "More than";
        public static string dhs25 = "The program is locked. You cannot add start time now.";
        public static string dhs26 = "Please enter start time for this sequence.";
        public static string dhs27 = "groups configured for";
        public static string dhs28 = "Valve already configured for this duration. You cannot configure for same time now.";
        public static string dhs29 = "Sequence dates not configured. Please configure sequence dates and try again.";
        public static string dhs30 = "Sequence saved successfully.";
        public static string dhs31 = "Sequence saved with below exceptions";
        public static string dhs32 = "Day end has reached. You cannot add new duration now.";
        public static string dhs33 = "No durations specified. Please select atleast one duration and try again.";
        public static string dhs34 = "The program is locked. You cannot create new sequence now.";
        public static string dhs35 = "O/p no.";
        public static string dhs36 = "trying to operate more than";
        public static string dhs37 = "Max 15 settings have been defined for this fert group. New setting will not be saved.";
        public static string dhs38 = "Valve from group";
        public static string dhs39 = "already configured for duration";
        public static string to = "to";
        public static string dhs40 = "You cannot configure for same time now.";
        public static string Valve = "Valve";
        public static string dhs41 = "Please create one sequence first.";
        public static string dhs42 = "The program is locked. You cannot delete sequence now.";
        public static string dhs43 = "Changing start time will overflow day start time. Please change start time and try again.";
        public static string dhs44 = "The program is locked. You cannot edit sequence start time now.";
        public static string dhs45 = "Changing start time will overflow day start time. Please change start time and try again.";
        public static string dhs46 = "Valve already configured for this duration. You cannot configure for same time now.";
        public static string dhs47 = "The program is locked. You cannot change duration now.";
        public static string dhs48 = "The program is locked. You cannot edit sequence now.";
        public static string dhs49 = "Please select duration.";
        public static string dhs50 = "Invalid Sequence Start Date";
        public static string dhs51 = "start date before programme start date.";
        public static string dhs52 = "start date before programme loop start date.";
        public static string dhs53 = "end date after programme end date.";
        public static string dhs54 = "end date after programme loop end date.";
        public static string dhs55 = "Invalid Sequence End Date";
        public static string dhs56 = "Program start time not defined.";
        public static string dhs57 = "Program end time not defined.";
        public static string dhs58 = "start time before programme start time.";
        public static string dhs59 = "start time before programme loop start time.";
        public static string dhs60 = "start time after programme end time.";
        public static string dhs61 = "start time after programme loop end time.";
        public static string dhs62 = "end time after programme end time.";
        public static string dhs63 = "Sequence schedule oveflowing beyond zone day end time for";
        public static string dhs64 = "overlapping with other start times.";
        public static string dhs65 = "No valves added for element";
        public static string forr = "for";
        public static string dhs66 = "No valves added for element";
        public static string dhs67 = "Please configure valve first.";
        public static string dhs68 = "No elements are defined for";
        public static string dhs69 = "Please configure sequence first.";
        public static string dhs70 = "Day end time for zone not in proper format.";
        public static string dhs71 = "No zone is defined. Please set zone first.";
        public static string dhs72 = "Error while validating zone end time for sequence";

        #endregion

        #region SequenceStepConfig

        public static string ssc1 = "Schedule For";
        public static string ssc2 = "Select Group";
        public static string ssc3 = "Configure Valves";
        public static string ssc4 = "No valves configured for this group. Please add valves in group and try again.";
        public static string ssc5 = "No valves configured for this network. Please add valves in network and try again.";
        public static string ssc6 = "The program is locked. You cannot delete sequence valve now.";
        public static string ssc7 = "No Groups Configured";
        public static string ssc8 = "No Valves Configured";

        #endregion

        #region SequenceStepsValveConfig

        public static string ssv1 = "Please enter time of irrigation";
        public static string ssv2 = "Please enter duration of fert";
        public static string ssv3 = "Valve";
        public static string ssv4 = "Flush Related Filter";
        public static string ssv5 = "Add Fertilizer";
        public static string ssv6 = "Type Of Operation";
        public static string ssv7 = "Water Before Fert";
        public static string ssv8 = "Duration";
        public static string ssv9 = "Unit";
        public static string ssv10 = "No valves configured";
        public static string ssv11 = "The program is locked. You cannot edit valves now.";
        public static string ssv12 = "";
        public static string ssv13 = "";
        public static string ssv14 = "";
        public static string ssv15 = "";
        public static string ssv16 = "";
        public static string ssv17 = "";
        public static string ssv18 = "";
        public static string ssv19 = "";
        public static string ssv20 = "";
        public static string toolfirtFilt = "Configure Valve in Valve Configuration";
        #endregion


        #region HandHeldDevice

        public static string hh1 = "Are you sure to delete HandHeld Device?";
        public static string hh2 = "HandHeld Device No";
        public static string hh3 = "Tag Name";
        public static string hh4 = "Description";
        public static string hh5 = "Networks";
        public static string hh6 = "Handheld Device Limit reached to 10000. Cannot add more HandHeld device";
        public static string hh7 = "HandHeld data add successfully";
        public static string hh8 = "Please enter Handheld NO. for HandHeld Device.";
        public static string hh9 = "Handheld no. should be greater than 0 or less than or equal to 10000";
        public static string hh10 = "Please enter Description for HandHeld Device.";
        public static string hh11 = "Please enter Tag Name for HandHeld Device.";
        public static string hh12 = "Please Select atleasst one Network for HandHeld Device.";
        public static string hh13 = "HandHeld No. must have integer value.";
        public static string hh14 = "HandHeld No. already exists. Please enter another HandHeld No.";
        public static string hh15 = "Record delete successfully";
        public static string hh16 = "Cannot add data.Following Network/Networks have already 5 handheld devices.";
        public static string hh17 = "Record update successfully";
        public static string hh18 = "Add";
        public static string hh19 = "No Element Found to Export.";
        public static string hh20 = "";
        public static string hh21 = "";
        public static string hh22 = "";

        #endregion

        #region Matrix Screen
        public static string MatrixViewNo = "1";
        #endregion

        #endregion

        //Update Ids Type
        public static string Config = "Config";
        public static string VRT = "VRT";
        public static string SensorUpid = "SensorUpid";
        public static string ScheduleNode = "ScheduleNode";
        public static string ScheduleSequence = "ScheduleSequence";
        public static string MainSCH = "MainSCH";
        public static string FilterSetting = "FilterSetting";




    }
}
