using Dapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IUploadService
    {
        Task<OperationResult> UploadConfiguration(IFormFileCollection formFiles);
        Task Createnodes();
        Task<OperationResult> UploadConfigurationSeq(IFormFileCollection formFiles);

        Task<ProjectConfiguration> GetProjectConfiguration();
    }
    public class UploadService : IUploadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private MainDBContext _mainDBContext;

        public UploadService(ILogger<UploadService> logger, IWebHostEnvironment webHostEnvironment, MainDBContext mainDBContext,
            IConfiguration config)
        {
            _mainDBContext = mainDBContext;
            _logger = logger;
            _config = config;
            _webHostEnvironment = webHostEnvironment;

        }
        /// <summary>
        /// upload project configuration
        /// </summary>
        /// <param name="formFiles"></param>
        /// <returns></returns>
        public async Task<OperationResult> UploadConfiguration(IFormFileCollection formFiles)
        {
            try
            {
                var result = new OperationResult { Succeeded = false };
                var results = new OperationResult { Succeeded = false };
                string uniqueName = string.Empty;
                string fileName = string.Empty;
                bool addRecord = true;
                // check if user has uploaded new attachment file
                IFormFile fileToAdd = formFiles.FirstOrDefault();
                if (fileToAdd != null)
                {
                    var validationResult = IsValidFile(fileToAdd);
                    if (!validationResult.Succeeded)
                    {
                        // invalid file uploaded, so cannot add this record.
                        addRecord = false;
                        result.Errors.AddRange(validationResult.Errors);
                    }
                    else
                    {
                        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fileToAdd.OpenReadStream(), false))
                        {
                            //create the object for workbook part  
                            WorkbookPart wbPart = spreadsheetDocument.WorkbookPart;
                            Sheets thesheetcollection = (Sheets)wbPart.Workbook.GetFirstChild<Sheets>();
                            SharedStringTablePart stringTablePart = spreadsheetDocument.WorkbookPart.SharedStringTablePart;

                            // Loop through each of the sheets in the spreadsheet
                            foreach (Sheet thesheet in thesheetcollection)
                            {
                                // Throw an exception if there is no sheet.
                                if (thesheet == null)
                                {
                                    throw new ArgumentException("sheetName");
                                }

                                _logger.LogInformation(thesheet.Name);
                                //statement to get the worksheet object by using the sheet id  

                                //WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheet.Id);
                                //Worksheet worksheet = worksheetPart.Worksheet;
                                //var rows = worksheet.GetFirstChild<SheetData>().Elements<Row>();


                                Worksheet theWorksheet = ((WorksheetPart)wbPart.GetPartById(thesheet.Id)).Worksheet;
                                SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();

                                if (thesheet.Name == "Project_Configuration")
                                {
                                    await AddProjectConfigurationAsync(thesheetdata);
                                }
                                else if (thesheet.Name == "Rechargable Node")
                                {
                                    await AddRecharagableNodeSetting(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Non-Rechargable_Node")
                                {
                                    await AddNonRecharagableNodeSetting(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "GW-GW as Node")
                                {
                                    await AddGatewayNode(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Analog 4_20mA Sensor")
                                {
                                    await AddAnalog420(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Analog 0_5V Sensor")
                                {
                                    await AddAnalog05V(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Digital NO_NC Type Sensor")
                                {
                                    await AddDigitalNoNcSensor(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Digital Counter Type Sensor")
                                {
                                    await AddDigitalCounterSensor(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Water Meter Sensor")
                                {
                                    await AddWaterMeterSensor(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "VRT")
                                {
                                    await AddVrtSetting(thesheetdata, stringTablePart);
                                }
                                else if (thesheet.Name == "Schedule")
                                {
                                    results = await AddScheduleSetting(thesheetdata, stringTablePart);

                                }
                                else
                                {
                                    if (thesheet == null)
                                    {
                                        throw new ArgumentException("sheetName");
                                    }
                                }

                            }

                            spreadsheetDocument.Close();
                        }
                    }

                }
                if (results.Errors.Count == 0)
                    results.Succeeded = true;
                return results;
                if (result.Errors.Count == 0)
                    result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(UploadService) + "." + nameof(UploadConfiguration) + "]" + ex);
                var result = new OperationResult { Succeeded = false };
                result.Errors.Add($"Error occurred while adding attachment(s).");
                return result;
            }
        }
        /// <summary>
        /// get project
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectConfiguration> GetProjectConfiguration()
        {
            try
            {
                var project = await _mainDBContext.ProjectConfiguration.FirstOrDefaultAsync();
                //  List<ProjectViewModel> projectViewModel = _mapper.Map<List<ProjectViewModel>>(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetProjectConfiguration)}]{ex}");
                throw ex;
            }
        }

        public async Task<OperationResult> UploadConfigurationSeq(IFormFileCollection formFiles)
        {
            try
            {
                var result = new OperationResult { Succeeded = false };
                var sequpload = _mainDBContext.MultiSequenceUploading.FirstOrDefault();

                if(sequpload.SeqUploadingFlag == true)
                {
                    result.Succeeded = false;
                    return result;
                }
              
                string uniqueName = string.Empty;
                string fileName = string.Empty;
                bool addRecord = true;
                // check if user has uploaded new attachment file
                IFormFile fileToAdd = formFiles.FirstOrDefault();
                if (fileToAdd != null)
                {
                    var validationResult = IsValidFile(fileToAdd);
                    if (!validationResult.Succeeded)
                    {
                        // invalid file uploaded, so cannot add this record.
                        addRecord = false;
                        result.Errors.AddRange(validationResult.Errors);
                    }
                    else
                    {

                        using (var stream = new MemoryStream())
                        {
                            await fileToAdd.CopyToAsync(stream);
                            List<SequenceImportViewModel> modelLst = new List<SequenceImportViewModel>();

                            using (var package = new ExcelPackage(stream))
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                                var rowCount = worksheet.Dimension.Rows;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    modelLst.Add(new SequenceImportViewModel
                                    {
                                        SequenceNo = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        StartDate = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        EndDate = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                        Type = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                        weekly = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                        Interval = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                        StartTime = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                        Element1 = worksheet.Cells[row, 8].Value.ToString().Trim(),
                                        Duration1 = worksheet.Cells[row, 9].Value.ToString().Trim(),
                                        IsFert1 = worksheet.Cells[row, 10].Value.ToString().Trim(),
                                        IsFilter1 = worksheet.Cells[row, 11].Value.ToString().Trim(),
                                        Element2 = worksheet.Cells[row, 12].Value.ToString().Trim(),
                                        Duration2 = worksheet.Cells[row, 13].Value.ToString().Trim(),
                                        IsFert2 = worksheet.Cells[row, 14].Value.ToString().Trim(),
                                        IsFilter2 = worksheet.Cells[row, 15].Value.ToString().Trim(),
                                        // Age = int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),
                                    });
                                }
                            }
                        }
                    }

                }
                if (result.Errors.Count == 0)
                {
                    var sequploaded = _mainDBContext.MultiSequenceUploading.FirstOrDefault();
                    sequploaded.SeqUploadingFlag = false;
                    _mainDBContext.SaveChanges();
                    result.Succeeded = true;
                }                    
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(UploadService) + "." + nameof(UploadConfigurationSeq) + "]" + ex);
                var result = new OperationResult { Succeeded = false };
                result.Errors.Add($"Error occurred while adding attachment(s).");
                return result;
            }
        }

        #region Project Configuration
        //Save Project Configuration
        public async Task AddProjectConfigurationAsync(SheetData thesheetdata)
        {
            try
            {
                int MaxGwinProject = 0;
                int MaxNodeInProject = 0;
                int MaxNetworkinProject = 0;
                List<ProjectConfiguration> projectConfig = _mainDBContext.ProjectConfiguration.ToList();
                MaxGwinProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B23").FirstOrDefault().InnerText);
                MaxNodeInProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B25").FirstOrDefault().InnerText);
                if (projectConfig.Count > 0)
                {
                    //Edit
                    ProjectConfiguration toUpdate = projectConfig.FirstOrDefault();
                    toUpdate.MaxNodeSerialNo = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B6").FirstOrDefault().InnerText);
                    toUpdate.MaxNodeId = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B7").FirstOrDefault().InnerText);
                    toUpdate.MaxGwValves = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B8").FirstOrDefault().InnerText);
                    toUpdate.MaxGwSensors = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B9").FirstOrDefault().InnerText);
                    toUpdate.MaxNodeValves = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B10").FirstOrDefault().InnerText);
                    toUpdate.MaxNodeSensors = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B11").FirstOrDefault().InnerText);
                    toUpdate.MaxMobileNoSize = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B12").FirstOrDefault().InnerText);
                    toUpdate.MaxSequences = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B13").FirstOrDefault().InnerText);
                    toUpdate.MaxElementsInSequences = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B14").FirstOrDefault().InnerText);
                    toUpdate.MaxFilters = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B15").FirstOrDefault().InnerText);
                    toUpdate.MaxPumps = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B16").FirstOrDefault().InnerText);
                    toUpdate.MaxFert = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B17").FirstOrDefault().InnerText);
                    toUpdate.MaxRtuscheduleTransferDis = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B19").FirstOrDefault().InnerText);
                    toUpdate.MaxSchOperatedGw = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B20").FirstOrDefault().InnerText);
                    toUpdate.MaxGwinProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B23").FirstOrDefault().InnerText);
                    toUpdate.MaxNodePerGw = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B24").FirstOrDefault().InnerText);
                    toUpdate.MaxNodeInProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B25").FirstOrDefault().InnerText);
                    MaxNetworkinProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B26").FirstOrDefault().InnerText);
                    toUpdate.MaxNetworkInProject = MaxNetworkinProject;
                    _mainDBContext.ProjectConfiguration.Update(toUpdate);
                    await _mainDBContext.SaveChangesAsync();
                    //int MaxNetworkinProject = 
                }
                else
                {
                    //Add
                    Cell theCell = thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B6").FirstOrDefault();

                    ProjectConfiguration projectConfiguration = new ProjectConfiguration
                    {
                        Id = 0,
                        MaxNodeSerialNo = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B6").FirstOrDefault().InnerText),
                        MaxNodeId = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B7").FirstOrDefault().InnerText),
                        MaxGwValves = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B8").FirstOrDefault().InnerText),
                        MaxGwSensors = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B9").FirstOrDefault().InnerText),
                        MaxNodeValves = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B10").FirstOrDefault().InnerText),
                        MaxNodeSensors = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B11").FirstOrDefault().InnerText),
                        MaxMobileNoSize = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B12").FirstOrDefault().InnerText),
                        MaxSequences = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B13").FirstOrDefault().InnerText),
                        MaxElementsInSequences = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B14").FirstOrDefault().InnerText),
                        MaxFilters = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B15").FirstOrDefault().InnerText),
                        MaxPumps = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B16").FirstOrDefault().InnerText),
                        MaxFert = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B17").FirstOrDefault().InnerText),
                        MaxRtuscheduleTransferDis = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B19").FirstOrDefault().InnerText),
                        MaxSchOperatedGw = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B20").FirstOrDefault().InnerText),
                        MaxGwinProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B23").FirstOrDefault().InnerText),
                        MaxNodePerGw = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B24").FirstOrDefault().InnerText),
                        MaxNodeInProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B25").FirstOrDefault().InnerText),
                        MaxNetworkInProject = (int)Convert.ToDecimal(thesheetdata.Descendants<Cell>().Where(c => c.CellReference == "B26").FirstOrDefault().InnerText)

                    };
                    MaxNetworkinProject = (int)projectConfiguration.MaxNetworkInProject;
                    await _mainDBContext.ProjectConfiguration.AddAsync(projectConfiguration);
                    await _mainDBContext.SaveChangesAsync();
                }

                List<Node> nodesToAdd = new List<Node>();
                List<Gateway> gatewayToAdd = new List<Gateway>();
                List<UpdateIdsRequired> updateIdsRequiredsToAdd = new List<UpdateIdsRequired>();


                var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
                using (var sqlConnection = new SqlConnection(dbConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    var resultSP = await sqlConnection.QueryMultipleAsync("MultiDeleteAllProjectConf", null, null, null, CommandType.StoredProcedure);
                }

                //Delete and Create Node  table
                //var nodesPresent = _mainDBContext.Node.AsEnumerable();
                //_mainDBContext.Node.RemoveRange(nodesPresent.ToList());
                //await _mainDBContext.SaveChangesAsync();
                ////Delete and Create Gatewaytable
                //var gatewayPresent = _mainDBContext.Gateway.AsEnumerable();
                //_mainDBContext.Gateway.RemoveRange(gatewayPresent.ToList());
                //await _mainDBContext.SaveChangesAsync();
                ////Delte UpdateIdsRequired
                //var updateIdsRequiredPresent = _mainDBContext.UpdateIdsRequired.AsEnumerable();
                //_mainDBContext.UpdateIdsRequired.RemoveRange(updateIdsRequiredPresent.ToList());
                //await _mainDBContext.SaveChangesAsync();
                ////Delte updateIdsMaxSchPresent
                //var updateIdsMaxSchPresent = _mainDBContext.GatewayMaxSch.AsEnumerable();
                //_mainDBContext.GatewayMaxSch.RemoveRange(updateIdsMaxSchPresent.ToList());
                //await _mainDBContext.SaveChangesAsync();

                #region Create Node
                for (int i = 0; i <= MaxNetworkinProject; i++)
                {
                    for (int j = 1; j <= MaxNodeInProject; j++)
                    {
                        //Add node
                        Node node = new Node();
                        if (!nodesToAdd.Any(x => x.NodeNo == i))
                        {
                            node.NodeNo = j;
                            nodesToAdd.Add(node);
                        }

                        //UpdateIdsRequired for server
                        UpdateIdsRequired updateIdsRequired = new UpdateIdsRequired();
                        updateIdsRequired.NodeId = j;
                        updateIdsRequired.NetworkNo = i;
                        updateIdsRequired.ConfigUid = 0;
                        updateIdsRequired.VrtUid = 0;
                        updateIdsRequired.SensorUid = 0;
                        updateIdsRequired.ScheduleNodeUid = 0;
                        updateIdsRequired.ScheduleSequenceUid = 0;
                        updateIdsRequired.MainSchUid = 0;
                        updateIdsRequired.FilterUid = 0;
                        updateIdsRequiredsToAdd.Add(updateIdsRequired);

                    }
                }
                //ADD Node
                await _mainDBContext.Node.AddRangeAsync(nodesToAdd);
                await _mainDBContext.SaveChangesAsync();
                //Add Update Id Required fro server
                await _mainDBContext.UpdateIdsRequired.AddRangeAsync(updateIdsRequiredsToAdd);
                await _mainDBContext.SaveChangesAsync();
                #endregion



                //Update UpdateIds table with respect to Gateway and node
                //Delete Old Ids
                //var updateIdsPresent = await _mainDBContext.UpdateIds.ToListAsync();
                //_mainDBContext.UpdateIds.RemoveRange(updateIdsPresent);
                //await _mainDBContext.SaveChangesAsync();

                //var updNwids = await _mainDBContext.UpdateIdsProject.ToListAsync();
                //_mainDBContext.UpdateIdsProject.RemoveRange(updNwids);
                //await _mainDBContext.SaveChangesAsync();               

                //var nwrtus = _mainDBContext.MultiNetworkRtu.AsEnumerable();
                //_mainDBContext.MultiNetworkRtu.RemoveRange(nwrtus.ToList());
                //await _mainDBContext.SaveChangesAsync();

                //Add project id
                UpdateIdsProject updateIdsProject = new UpdateIdsProject();
                updateIdsProject.ProjectUpId = 0;
                await _mainDBContext.UpdateIdsProject.AddAsync(updateIdsProject);
                await _mainDBContext.SaveChangesAsync();

                List<MultiNetworkRtu> multiNetworkRtus = new List<MultiNetworkRtu>();

                //Insert new Mapping
                #region Commented for performance
                List<UpdateIds> upateIds = new List<UpdateIds>();
                List<GatewayMaxSch> GatewayMaxSchLst = new List<GatewayMaxSch>();


                for (int igw = 1; igw <= MaxGwinProject; igw++)
                {//Get Gateway Array
                    Gateway gateway = new Gateway();
                    gateway.GatewayNo = igw;
                    gatewayToAdd.Add(gateway);

                    GatewayMaxSch gatewayMaxSch = new GatewayMaxSch();
                    gatewayMaxSch.GatewayNo = igw;
                    gatewayMaxSch.MaxSchUpId = 0;
                    GatewayMaxSchLst.Add(gatewayMaxSch);
                    for (int network = 0; network <= MaxNetworkinProject; network++)
                    {
                        for (int inode = 1; inode <= MaxNodeInProject; inode++)
                        {
                            int shifts = network << 10;
                            int nodeID = inode + shifts;
                            //Network RTUs
                            MultiNetworkRtu multiNetworkRtu = new MultiNetworkRtu();
                            multiNetworkRtu.NetworkNo = network;
                            multiNetworkRtu.RtuId = inode;
                            multiNetworkRtu.NodeNo = nodeID;
                            multiNetworkRtus.Add(multiNetworkRtu);

                            UpdateIds model = new UpdateIds();
                            model.Gwid = igw;
                            model.NodeId = nodeID;
                            model.NodeUid = 0;
                            model.ConfigUid = 0;
                            model.VrtUid = 0;
                            model.SensorUid = 0;
                            model.ScheduleNodeUid = 0;
                            model.ScheduleSequenceUid = 0;
                            model.MainSchUid = 0;
                            model.FilterUid = 0;
                            upateIds.Add(model);
                        }
                    }

                }
                #endregion

                await _mainDBContext.Gateway.AddRangeAsync(gatewayToAdd);
                await _mainDBContext.SaveChangesAsync();

                await _mainDBContext.GatewayMaxSch.AddRangeAsync(GatewayMaxSchLst);
                await _mainDBContext.SaveChangesAsync();

                await _mainDBContext.UpdateIds.AddRangeAsync(upateIds);
                await _mainDBContext.SaveChangesAsync();

                await _mainDBContext.MultiNetworkRtu.AddRangeAsync(multiNetworkRtus);
                await _mainDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public async Task Createnodes() {
            try
            {
                //Insert new Mapping
                #region Commented for performance
                List<UpdateIds> upateIds = new List<UpdateIds>();
                List<GatewayMaxSch> GatewayMaxSchLst = new List<GatewayMaxSch>();
                List<Gateway> gatewayToAdd = new List<Gateway>();
                List<MultiNetworkRtu> multiNetworkRtus = new List<MultiNetworkRtu>();

                for (int igw = 1; igw <= 26; igw++)
                {//Get Gateway Array
                    Gateway gateway = new Gateway();
                    gateway.GatewayNo = igw;
                    gatewayToAdd.Add(gateway);

                    GatewayMaxSch gatewayMaxSch = new GatewayMaxSch();
                    gatewayMaxSch.GatewayNo = igw;
                    gatewayMaxSch.MaxSchUpId = 0;
                    GatewayMaxSchLst.Add(gatewayMaxSch);
                    for (int network = 0; network <= 26; network++)
                    {
                        for (int inode = 1; inode <= 250; inode++)
                        {
                            int shifts = network << 10;
                            int nodeID = inode + shifts;
                            //Network RTUs
                            MultiNetworkRtu multiNetworkRtu = new MultiNetworkRtu();
                            multiNetworkRtu.NetworkNo = network;
                            multiNetworkRtu.RtuId = inode;
                            multiNetworkRtu.NodeNo = nodeID;
                            multiNetworkRtus.Add(multiNetworkRtu);

                            UpdateIds model = new UpdateIds();
                            model.Gwid = igw;
                            model.NodeId = nodeID;
                            model.NodeUid = 0;
                            model.ConfigUid = 0;
                            model.VrtUid = 0;
                            model.SensorUid = 0;
                            model.ScheduleNodeUid = 0;
                            model.ScheduleSequenceUid = 0;
                            model.MainSchUid = 0;
                            model.FilterUid = 0;
                            upateIds.Add(model);
                        }
                    }

                }
                #endregion
                var sqlcon = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");

                // connect to SQL
                using (SqlConnection connn = new SqlConnection(sqlcon))
                {
                    connn.Open();
                    SqlTransaction transaction = connn.BeginTransaction();

                    using (var bulkCopy = new SqlBulkCopy(connn, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.BatchSize = 100;
                        bulkCopy.DestinationTableName = "dbo.UpdateIds";
                        try
                        {
                            DataTable dt = ToDataTable(upateIds);

                            bulkCopy.WriteToServer(dt);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            connn.Close();
                        }
                    }
                
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
     
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #region Non Recharagable Node Concentator Setting
        //Save Project Configuration
        public async Task AddNonRecharagableNodeSetting(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            int GwSrNo = 0;
            int NodeId = 0;
            try
            {// Extract the workbook part
                List<NonRechableNode> rechableNodes = new List<NonRechableNode>();

                List<Row> rowsData = thesheetdata.Elements<Row>().ToList();
                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.Count;
                int ColMax = 29;
                List<Row> rows = thesheetdata.Elements<Row>().Where(r => r.Elements<Cell>().Any(ce => ce.DataType != null)).ToList()
   ;
                for (int rowIndex = RowMin; rowIndex < rowsData.Count; rowIndex++)
                {
                    Node node = new Node();
                    NonRechableNode rechableInfo = new NonRechableNode();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = rowsData.Count;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = rowsData[rowIndex];
                    if (row == null)
                    {
                        break;
                    }
                    // if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    //Row is not empty
                    //Cell cell = GetCell(worksheet, currentRow, rowIndex);

                    //string value = GetCellValueByIndex(thesheetdata, "B" + rowIndex);
                    //Add
                    rechableInfo.Id = 0;
                    GwSrNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    //int shifts = i << 10;
                    //int nodeID = j + shifts;
                    if (GwSrNo > 0 && NodeId > 0)
                    {
                        var nodeexists = await _mainDBContext.NonRechableNode.Where(x => x.GwSrn == GwSrNo && x.NodeId == NodeId).FirstOrDefaultAsync();
                        if (nodeexists != null)
                        {
                            nodeexists.ThresholdforOcsc = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MinConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MinAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MtusizeValue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.HandshkInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.Pulsedelayvalue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.OperAttempt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AttemptForWaterFlow = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LongSleepHndshkIntervalMf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.Bttxpower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraSf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraPower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraFreq = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraCr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw1id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw2id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw3id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw4id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AwfdetectEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraSetting = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AutoSendStatusEnableBit = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PowerLoopLatchEnable = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.GlobalAlarmEnDs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxLoRaCommAtt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SensorAlarmEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LoRaRxWindowMasking = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.DummyByte3Lsb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.DummyByte3Msb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SaftetyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SlowDownCommDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.ForceDeepSleepDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                            await _mainDBContext.SaveChangesAsync();
                        }
                        else
                        {
                            rechableInfo.GwSrn = GwSrNo;
                            rechableInfo.NodeId = NodeId;
                            rechableInfo.ProductId = 1;
                            rechableInfo.ThresholdforOcsc = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MinConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MinAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MtusizeValue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.HandshkInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.Pulsedelayvalue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.OperAttempt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AttemptForWaterFlow = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.LongSleepHndshkIntervalMf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.Bttxpower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraSf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraPower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraFreq = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraCr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw1id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw2id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw3id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw4id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AwfdetectEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraSetting = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AutoSendStatusEnableBit = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PowerLoopLatchEnable = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.GlobalAlarmEnDs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxLoRaCommAtt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SensorAlarmEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.LoRaRxWindowMasking = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.DummyByte3Lsb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.DummyByte3Msb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SaftetyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SlowDownCommDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.ForceDeepSleepDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            rechableInfo.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            rechableNodes.Add(rechableInfo);


                        }
                        //var nodeInfo = await _mainDBContext.Node.Where(x => x.NodeNo == NodeId && x.ProductTypeId == 1).FirstOrDefaultAsync();
                        //if (nodeInfo != null)
                        //{
                        //    nodeInfo.ProductTypeId = 1;
                        //    nodeInfo.IsAddonCard = true;
                        //    _mainDBContext.Node.Update(nodeInfo);
                        //    await _mainDBContext.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    node = new Node();
                        //    node.NodeNo = NodeId;
                        //    node.ProductTypeId = 1;
                        //    node.IsAddonCard = true;
                        //    await _mainDBContext.Node.AddAsync(node);
                        //    await _mainDBContext.SaveChangesAsync();
                        //}
                        //Update Gateway and Node Upids
                        await UpdateUpidsProjNodeAsync(GwSrNo, NodeId, GlobalConstants.Config);
                        await AddNetwork(GwSrNo);
                    }
                }
                if (rechableNodes.Count != 0)
                {
                    await _mainDBContext.NonRechableNode.AddRangeAsync(rechableNodes);
                    await _mainDBContext.SaveChangesAsync();
                    //Update Gateway and Node Upids                   
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Recharagable Node Concentator Setting
        //Save Project Configuration
        public async Task AddRecharagableNodeSetting(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            int GwSrNo = 0;
            int NodeId = 0;
            try
            {// Extract the workbook part
                List<RechableNode> rechableNodes = new List<RechableNode>();
                List<Row> rowsData = thesheetdata.Elements<Row>().ToList();
                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.Count;
                int ColMax = 29;
                List<Row> rows = thesheetdata.Elements<Row>().Where(r => r.Elements<Cell>().Any(ce => ce.DataType != null)).ToList()
   ;
                for (int rowIndex = RowMin; rowIndex < rowsData.Count; rowIndex++)
                {
                    Node node = new Node();
                    RechableNode rechableInfo = new RechableNode();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = rowsData.Count;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = rowsData[rowIndex];
                    if (row == null)
                    {
                        break;
                    }
                    //if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    //Row is not empty
                    //Cell cell = GetCell(worksheet, currentRow, rowIndex);

                    //string value = GetCellValueByIndex(thesheetdata, "B" + rowIndex);
                    //Add
                    rechableInfo.Id = 0;
                    GwSrNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    //int shifts = i << 10;
                    //int nodeID = j + shifts;
                    if (GwSrNo > 0 && NodeId > 0)
                    {
                        var nodeexists = await _mainDBContext.RechableNode.Where(x => x.GwSrn == GwSrNo && x.NodeId == NodeId).FirstOrDefaultAsync();
                        if (nodeexists != null)
                        {
                            nodeexists.ThresholdforOcsc = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MinConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MinAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MtusizeValue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.HandshkInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.Pulsedelayvalue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.OperAttempt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AttemptForWaterFlow = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LongSleepHndshkIntervalMf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.Bttxpower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraSf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraPower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraFreq = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraCr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw1id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw2id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw3id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PreferredGw4id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AwfdetectEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FixLoraSetting = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AutoSendStatusEnableBit = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.PowerLoopLatchEnable = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.GlobalAlarmEnDs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.MaxLoRaCommAtt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SensorAlarmEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LoRaRxWindowMasking = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.DummyByte3Lsb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.DummyByte3Msb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SaftetyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SlowDownCommDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.ForceDeepSleepDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AutoSensorStatusSendInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.SamplingTimeInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.ApplicationEnbits = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.TimeForNocommWithGw = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            await _mainDBContext.SaveChangesAsync();
                        }
                        else
                        {
                            rechableInfo.GwSrn = GwSrNo;
                            rechableInfo.NodeId = NodeId;
                            rechableInfo.ProductId = 2;
                            rechableInfo.ThresholdforOcsc = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MinConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxConnInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MinAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxAdvInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MtusizeValue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.HandshkInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.Pulsedelayvalue = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.OperAttempt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AttemptForWaterFlow = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.LongSleepHndshkIntervalMf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.Bttxpower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraSf = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraPower = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraFreq = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraCr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw1id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw2id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw3id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PreferredGw4id = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AwfdetectEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.FixLoraSetting = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AutoSendStatusEnableBit = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.PowerLoopLatchEnable = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.GlobalAlarmEnDs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.MaxLoRaCommAtt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SensorAlarmEndis = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.LoRaRxWindowMasking = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.DummyByte3Lsb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.DummyByte3Msb = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SaftetyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SlowDownCommDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.ForceDeepSleepDurMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.AutoSensorStatusSendInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.SamplingTimeInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.ApplicationEnbits = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.TimeForNocommWithGw = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            rechableInfo.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            rechableInfo.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            rechableNodes.Add(rechableInfo);



                        }

                        //var nodeInfo = await _mainDBContext.Node.Where(x => x.NodeNo == NodeId && x.ProductTypeId == 2).FirstOrDefaultAsync();
                        //if (nodeInfo != null)
                        //{
                        //    nodeInfo.ProductTypeId = 2;
                        //    nodeInfo.IsAddonCard = true;
                        //    _mainDBContext.Node.Update(nodeInfo);
                        //    await _mainDBContext.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    node = new Node();
                        //    node.NodeNo = NodeId;
                        //    node.ProductTypeId = 2;
                        //    node.IsAddonCard = true;
                        //    await _mainDBContext.Node.AddAsync(node);
                        //    await _mainDBContext.SaveChangesAsync();
                        //}
                        //Update Gateway and Node Upids
                        await UpdateUpidsProjNodeAsync(GwSrNo, NodeId, GlobalConstants.Config);
                        await AddNetwork(GwSrNo);
                    }



                }
                if (rechableNodes.Count != 0)
                {
                    await _mainDBContext.RechableNode.AddRangeAsync(rechableNodes);
                    await _mainDBContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AddNetwork(int GwSrNo)
        {
            try
            {
                if (!_mainDBContext.Network.Any(x => x.NetworkNo == GwSrNo))
                {
                    Network network = new Network();
                    network.PrjId = 4;
                    network.Description = "@N {GwSrNo}";
                    network.Name = "N" + GwSrNo.ToString();
                    network.NetworkNo = GwSrNo;
                    network.NetworkLock = true;
                    network.TagName = GwSrNo.ToString();
                    await _mainDBContext.Network.AddAsync(network);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region GW/GW as Node
        //Save Project Configuration
        public async Task AddGatewayNode(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            int GwSrNo = 0;
            int NodeId = 0;
            int ProductType = 4;
            try
            {
                List<GatewayNode> gatewayNodes = new List<GatewayNode>();
                List<Node> nodeList = new List<Node>();

                //List<Row> rowsData = thesheetdata.Descendants<Row>().ToList();
                IEnumerable<Row> rowsData = thesheetdata.Descendants<Row>();
                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.ToList().Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= RowMax; rowIndex++)
                {
                    Node node = new Node();

                    GatewayNode modeltoAdd = new GatewayNode();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = RowMax;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    // if (string.IsNullOrWhiteSpace(row.InnerText)) continue;

                    //Add
                    ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart));
                    currentRow = GetNextColumn(currentRow);
                    GwSrNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart));
                    currentRow = GetNextColumn(currentRow);
                    NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart));
                    currentRow = GetNextColumn(currentRow);
                    //int shifts = i << 10;
                    //int nodeID = j + shifts;
                    if (NodeId > 0)
                    {
                        var nodeexists = await _mainDBContext.GatewayNode.Where(x => x.GwSrn == GwSrNo && x.NodeId == NodeId && x.ProductId == ProductType).FirstOrDefaultAsync();
                        if (nodeexists != null)
                        {
                            nodeexists.Operator1MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.Operator2MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.Operator3MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.Operator4MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.Operator5MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.TempSensorHighTh = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.TempSensorLowTh = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.VbatlowThvoltage = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.AlarmInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.FuaskInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.Maxiocurrent = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LogAutoTxEndis = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Debug = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Warning = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Error = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Info = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Direction = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.StatusStoreDuration = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.GsmportEn = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.SchtransferedEnableDis = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.ForceAlarmDisable = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.Comdelay = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.LoRaConcentratorEn = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            nodeexists.SettingRfport = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.CardNo1 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C1Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.CardNo2 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C2Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.CardNo3 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.C3Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.ProgramEndDayMode = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            nodeexists.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            nodeexists.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        }
                        else
                        {
                            modeltoAdd.Id = 0;
                            modeltoAdd.GwSrn = GwSrNo;
                            modeltoAdd.NodeId = NodeId;
                            modeltoAdd.ProductId = ProductType;
                            modeltoAdd.Operator1MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Operator2MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Operator3MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Operator4MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Operator5MobNo = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.TempSensorHighTh = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.TempSensorLowTh = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.VbatlowThvoltage = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.AlarmInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.FuaskInterval = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Maxiocurrent = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.LogAutoTxEndis = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Debug = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Warning = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Error = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Info = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Direction = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.StatusStoreDuration = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.GsmportEn = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.SchtransferedEnableDis = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.ForceAlarmDisable = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.Comdelay = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.LoRaConcentratorEn = Convert.ToBoolean(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart) == "1" ? true : false); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.SettingRfport = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.CardNo1 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C1Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.CardNo2 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C2Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.CardNo3 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Type = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Debug = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Warning = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Error = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Info = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3SleepEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3AuoStatusEn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3AutoStatusInt = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3LogIntSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3FirmareVer = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3SafteyTimeoutMin = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.C3Settings = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.ProgramEndDayMode = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.NetworkTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            modeltoAdd.NodeTagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                            gatewayNodes.Add(modeltoAdd);
                        }
                        //var nodeInfo = await _mainDBContext.Node.Where(x => x.NodeNo == NodeId).FirstOrDefaultAsync();
                        //if (nodeInfo != null)
                        //{
                        //    if (GwSrNo == 0)
                        //    {
                        //        nodeInfo.ProductTypeId = 4;
                        //    }
                        //    else
                        //    {
                        //        nodeInfo.ProductTypeId = 3;
                        //    }

                        //    nodeInfo.IsAddonCard = true;
                        //    _mainDBContext.Node.Update(nodeInfo);
                        //    await _mainDBContext.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    node = new Node();
                        //    node.NodeNo = NodeId;
                        //    if (GwSrNo == 0)
                        //    {
                        //        nodeInfo.ProductTypeId = 4;
                        //    }
                        //    else
                        //    {
                        //        nodeInfo.ProductTypeId = 3;
                        //    }
                        //    node.IsAddonCard = true;
                        //    await _mainDBContext.Node.AddAsync(node);
                        //    await _mainDBContext.SaveChangesAsync();
                        // }
                        //Update Gateway and Node Upids
                        await UpdateUpidsProjNodeAsync(GwSrNo, NodeId, GlobalConstants.Config);
                        await AddNetwork(GwSrNo);
                    }


                }
                if (gatewayNodes.Count != 0)
                {
                    await _mainDBContext.GatewayNode.AddRangeAsync(gatewayNodes);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Analog 4_20mA Sensor
        //Save Project Configuration
        public async Task AddAnalog420(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {

                List<Analog420mAsensor> modelLst = new List<Analog420mAsensor>();
                List<Analog420mAsensor> modelLstToUpdate = new List<Analog420mAsensor>();

                //List<Row> rowsData = thesheetdata.Descendants<Row>().ToList();
                IEnumerable<Row> rowsData = thesheetdata.Descendants<Row>();

                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.ToList().Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= RowMax; rowIndex++)
                {
                    Analog420mAsensor modeltoAdd = new Analog420mAsensor();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = RowMax;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    //Add
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int SSNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.Analog420mAsensor.Any(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo))
                    {
                        var ssToUpdate = await _mainDBContext.Analog420mAsensor.Where(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo).FirstOrDefaultAsync();
                        ssToUpdate.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.CtrHghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.HghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LwTthrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.CrtLwrThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ScaleMin = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ScaleMax = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SensorName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = NodeId;
                        modeltoAdd.SsNo = SSNo;
                        modeltoAdd.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.CtrHghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.HghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LwTthrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.CrtLwrThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ScaleMin = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ScaleMax = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SensorName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modelLst.Add(modeltoAdd);

                    }

                    await UpdateUpidsProjNodeAsync(GwSrn, NodeId, GlobalConstants.SensorUpid);
                    await AddNetwork(GwSrn);
                }

                if (modelLst.Count > 0)
                {
                    await _mainDBContext.Analog420mAsensor.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.Analog420mAsensor.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Analog 0_5V Sensor
        //Save Project Configuration
        public async Task AddAnalog05V(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {

                List<Analog05vsensor> modelLst = new List<Analog05vsensor>();
                List<Analog05vsensor> modelLstToUpdate = new List<Analog05vsensor>();
                // List<Row> rowsData = thesheetdata.Elements<Row>().ToList();
                IEnumerable<Row> rowsData = thesheetdata.Descendants<Row>();

                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.ToList().Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= RowMax; rowIndex++)
                {
                    Analog05vsensor modeltoAdd = new Analog05vsensor();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = RowMax;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    //Add
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int SSNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.Analog05vsensor.Any(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo))
                    {
                        var ssToUpdate = await _mainDBContext.Analog05vsensor.Where(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo).FirstOrDefaultAsync();
                        ssToUpdate.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.CtrHghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.HghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LwTthrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.CrtLwrThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ScaleMin = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ScaleMax = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SensorName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = NodeId;
                        modeltoAdd.SsNo = SSNo;
                        modeltoAdd.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.CtrHghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.HghThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LwTthrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.CrtLwrThrs = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ScaleMin = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ScaleMax = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SensorName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modelLst.Add(modeltoAdd);
                    }
                    await UpdateUpidsProjNodeAsync(GwSrn, NodeId, GlobalConstants.SensorUpid);
                    await AddNetwork(GwSrn);
                }

                if (modelLst.Count > 0)
                {
                    await _mainDBContext.Analog05vsensor.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.Analog05vsensor.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Digital NO_NC Type Sensor
        //Save Project Configuration
        public async Task AddDigitalNoNcSensor(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {
                List<DigitalNoNctypeSensor> modelLst = new List<DigitalNoNctypeSensor>();
                List<DigitalNoNctypeSensor> modelLstToUpdate = new List<DigitalNoNctypeSensor>();

                List<Row> rowsData = thesheetdata.Descendants<Row>().ToList();

                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= rowsData.Count; rowIndex++)
                {
                    DigitalNoNctypeSensor modeltoAdd = new DigitalNoNctypeSensor();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = rowsData.Count;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int SSNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.DigitalNoNctypeSensor.Any(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo))
                    {
                        var ssToUpdate = await _mainDBContext.DigitalNoNctypeSensor.Where(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo).FirstOrDefaultAsync();
                        ssToUpdate.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DfltStat = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.StatRevDly = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.AlrmLvl = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyTmeIfRevr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        //Add
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = NodeId;
                        modeltoAdd.SsNo = SSNo;
                        modeltoAdd.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DfltStat = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.StatRevDly = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.AlrmLvl = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyTmeIfRevr = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLst.Add(modeltoAdd);
                    }

                    await UpdateUpidsProjNodeAsync(GwSrn, NodeId, GlobalConstants.SensorUpid);
                    await AddNetwork(GwSrn);
                }
                if (modelLst.Count > 0)
                {
                    await _mainDBContext.DigitalNoNctypeSensor.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.DigitalNoNctypeSensor.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Digital Counter Type Sensor
        //Save Project Configuration
        public async Task AddDigitalCounterSensor(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {
                List<DigitalCounterTypeSensor> modelLst = new List<DigitalCounterTypeSensor>();
                List<DigitalCounterTypeSensor> modelLstToUpdate = new List<DigitalCounterTypeSensor>();
                List<Row> rowsData = thesheetdata.Descendants<Row>().ToList();

                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= rowsData.Count; rowIndex++)
                {
                    DigitalCounterTypeSensor modeltoAdd = new DigitalCounterTypeSensor();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = rowsData.Count;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);

                    int NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int SSNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.DigitalCounterTypeSensor.Any(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo))
                    {
                        var ssToUpdate = await _mainDBContext.DigitalCounterTypeSensor.Where(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo).FirstOrDefaultAsync();
                        ssToUpdate.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.HghFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LowFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.FilTmIniWtTm = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.PlsDivFct = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = NodeId;
                        modeltoAdd.SsNo = SSNo;
                        modeltoAdd.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.HghFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LowFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.FilTmIniWtTm = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.PlsDivFct = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLst.Add(modeltoAdd);
                    }
                    await UpdateUpidsProjNodeAsync(GwSrn, NodeId, GlobalConstants.SensorUpid);
                    await AddNetwork(GwSrn);
                }
                if (modelLst.Count > 0)
                {
                    await _mainDBContext.DigitalCounterTypeSensor.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.DigitalCounterTypeSensor.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Water Meter Sensor
        public async Task AddWaterMeterSensor(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {

                List<WaterMeterSensorSetting> modelLst = new List<WaterMeterSensorSetting>();
                List<WaterMeterSensorSetting> modelLstToUpdate = new List<WaterMeterSensorSetting>();

                List<Row> rowsData = thesheetdata.Descendants<Row>().ToList();

                // This is A1
                int RowMin = 8;
                int ColMin = 2;

                int RowMax = rowsData.Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= rowsData.Count; rowIndex++)
                {
                    WaterMeterSensorSetting modeltoAdd = new WaterMeterSensorSetting();
                    int rowStart = rowIndex;
                    string colStart = "B";
                    int rowEnd = rowsData.Count;
                    string colEnd = "AD";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);

                    int NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int SSNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.WaterMeterSensorSetting.Any(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo))
                    {
                        var ssToUpdate = await _mainDBContext.WaterMeterSensorSetting.Where(x => x.GwSrn == GwSrn && x.NodeId == NodeId && x.SsNo == SSNo).FirstOrDefaultAsync();
                        ssToUpdate.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.HghFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LowFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TmeCfrmNoFlwSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.PulseValue = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);

                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = NodeId;
                        modeltoAdd.SsNo = SSNo;
                        modeltoAdd.Sspriority = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.SamplingRate = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.HghFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LowFlwRtFrq = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TmeCfrmNoFlwSec = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.DlyCfmThrs = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.Rsved = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ProductType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.NodePorductId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagName = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.PulseValue = Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);

                        modelLst.Add(modeltoAdd);
                    }
                    await UpdateUpidsProjNodeAsync(GwSrn, NodeId, GlobalConstants.SensorUpid);
                    await AddNetwork(GwSrn);
                }

                if (modelLst.Count > 0)
                {
                    await _mainDBContext.WaterMeterSensorSetting.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.WaterMeterSensorSetting.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region VRT
        public async Task AddVrtSetting(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            try
            {

                List<Vrtsetting> modelLst = new List<Vrtsetting>();
                List<Vrtsetting> modelLstToUpdate = new List<Vrtsetting>();
                IEnumerable<Row> rowsData = thesheetdata.Descendants<Row>();
                // This is A2
                int RowMin = 2;
                int ColMin = 2;

                int RowMax = rowsData.ToList().Count;
                int ColMax = 29;

                for (int rowIndex = RowMin; rowIndex <= RowMax; rowIndex++)
                {
                    Vrtsetting modeltoAdd = new Vrtsetting();
                    int rowStart = rowIndex;
                    string colStart = "A";
                    int rowEnd = RowMax;
                    string colEnd = "AQ";
                    string currentRow = colStart;
                    var row = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
                    if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                    int GwSrn = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);

                    int nodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int upid = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int productType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    int valveno = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                    if (_mainDBContext.Vrtsetting.Any(x => x.GwSrn == GwSrn && x.NodeId == nodeId && x.ProductType == productType && x.ValveNo == valveno))
                    {
                        var ssToUpdate = await _mainDBContext.Vrtsetting.Where(x => x.GwSrn == GwSrn && x.NodeId == nodeId && x.ProductType == productType && x.ValveNo == valveno).FirstOrDefaultAsync();
                        ssToUpdate.ValveType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.MasterNodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.MasterValveNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.BlockNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.FertGrpNo = (int)Convert.ToDecimal(HexToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart))); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.FilterGrpNo = (int)Convert.ToDecimal(int.Parse(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart), System.Globalization.NumberStyles.HexNumber)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LinkedSensor1NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LinkedSensor1SensorNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LinkedSensor2NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.LinkedSensor2SensorNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ValveGrpNo1 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.ValveGrpNo2 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.HeadPumpGrNo = (int)Convert.ToDecimal(int.Parse(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart), System.Globalization.NumberStyles.HexNumber)); currentRow = GetNextColumn(currentRow);

                        ssToUpdate.TagNameValve = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameMasterValve = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameBlock = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameFertGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameFilterGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameValveGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNamePumpGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        ssToUpdate.TagNameMasterNode = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);


                        modelLstToUpdate.Add(ssToUpdate);
                    }
                    else
                    {
                        modeltoAdd.Id = 0;
                        modeltoAdd.GwSrn = GwSrn;
                        modeltoAdd.NodeId = nodeId;
                        modeltoAdd.UpId = upid;
                        modeltoAdd.ProductType = productType;
                        modeltoAdd.ValveNo = valveno;
                        modeltoAdd.ValveType = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.MasterNodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.MasterValveNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.BlockNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.FertGrpNo = (int)Convert.ToDecimal(HexToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart))); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.FilterGrpNo = (int)Convert.ToDecimal(int.Parse(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart), System.Globalization.NumberStyles.HexNumber)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LinkedSensor1NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LinkedSensor1SensorNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LinkedSensor2NodeId = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.LinkedSensor2SensorNo = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ValveGrpNo1 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.ValveGrpNo2 = (int)Convert.ToDecimal(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.HeadPumpGrNo = (int)Convert.ToDecimal(int.Parse(GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart), System.Globalization.NumberStyles.HexNumber)); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameValve = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameMasterValve = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameBlock = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameFertGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameFilterGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameValveGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNamePumpGroup = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        modeltoAdd.TagNameMasterNode = GetCellValueByIndex(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLst.Add(modeltoAdd);

                    }

                    await UpdateUpidsProjNodeAsync(GwSrn, nodeId, GlobalConstants.VRT);
                    await AddNetwork(GwSrn);
                }

                if (modelLst.Count > 0)
                {
                    await _mainDBContext.Vrtsetting.AddRangeAsync(modelLst);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (modelLstToUpdate.Count > 0)
                {
                    _mainDBContext.Vrtsetting.UpdateRange(modelLstToUpdate);
                    await _mainDBContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion


        #region Schedule

        public bool validateRunTime(SequenceImportViewModel horizontalElement)
        {
            try
            {
                DateTime temp;
                if (DateTime.TryParse(horizontalElement.StartDate, out temp))
                {
                    return true;
                }
                else
                {
                    return false;
                }

                DateTime temp2;
                if (DateTime.TryParse(horizontalElement.EndDate, out temp2))
                {
                    return true;
                }
                else
                {
                    return false;
                }

                if (horizontalElement.StartTime.Contains(":"))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }
        public async Task<OperationResult> AddScheduleSetting(SheetData thesheetdata, SharedStringTablePart stringTablePart)
        {
            var results = new DataTable();
            //Create a new DataTable.
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                //Remove All sequence
                //List<NewSequence> newSequencesS = await _mainDBContext.NewSequence.ToListAsync();
                //_mainDBContext.NewSequence.RemoveRange(newSequencesS);
                //await _mainDBContext.SaveChangesAsync();          

                //https://stackoverflow.com/questions/46222363/importing-excel-to-datatable-getting-data-incorrectly-using-openxml
                List<SequenceImportViewModel> modelLst = new List<SequenceImportViewModel>();
                //IEnumerable<Row> rows = thesheetdata.Descendants<Row>();
                List<Row> rows = thesheetdata.Elements<Row>().Where(r => r.Elements<Cell>().Any(ce => ce.DataType != null)).ToList();
                int rowno = 0;
                // Add rows into DataTable
                int OldSequenceNo = 0;
                int NewSequenceNo = 0;
                int rowIndex = 3;
                int RowMax = rows.ToList().Count;
                int ColMax = 29;
                var result = new OperationResult { Succeeded = false };

                foreach (Row row in rows)
                {
                    if (rowno > 1)
                    {
                        int rowStart = rowIndex;
                        string colStart = "A";
                        int rowEnd = RowMax;
                        string colEnd = "CY";
                        string currentRow = colStart;
                        SequenceImportViewModel model = new SequenceImportViewModel();
                        //  var singleRow = thesheetdata.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();



                        if (string.IsNullOrWhiteSpace(row.InnerText)) continue;
                        // var actualCells = row.Elements<Cell>().ToArray();
                        model.SequenceNo = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (model.SequenceNo == "")
                        {
                            rowIndex++;
                            continue;
                            //result.Errors.Add($"Could not upload sequence file");
                            //return result;
                        }

                        model.StartDate = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.EndDate = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);


                        //if (model.StartDate == "" || model.EndDate =="")
                        //{
                        //    rowIndex++;
                        //    result.Errors.Add($"Could not upload sequence file");
                        //    return result;
                        //}


                        model.Type = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.weekly = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Interval = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.StartTime = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.StartTime.Contains(":") && model.StartTime != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.Element1 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration1 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration1.Contains(":") && model.Duration1 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert1 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter1 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element2 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration2 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration2.Contains(":") && model.Duration2 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert2 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter2 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element3 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration3 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration3.Contains(":") && model.Duration3 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert3 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter3 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element4 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration4 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration4.Contains(":") && model.Duration4 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert4 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter4 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element5 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration5 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration5.Contains(":") && model.Duration5 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert5 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter5 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element6 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration6 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration6.Contains(":") && model.Duration6 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert6 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter6 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element7 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration7 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration7.Contains(":") && model.Duration7 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert7 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter7 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element8 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration8 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration8.Contains(":") && model.Duration8 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert8 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter8 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element9 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration9 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration9.Contains(":") && model.Duration9 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert9 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter9 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element10 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration10 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration10.Contains(":") && model.Duration10 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert10 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter10 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element11 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration11 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration11.Contains(":") && model.Duration11 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert11 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter11 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element12 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration12 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration12.Contains(":") && model.Duration12 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert12 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter12 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element13 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration13 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration13.Contains(":") && model.Duration13 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert13 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter13 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element14 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration14 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration14.Contains(":") && model.Duration14 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert14 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter14 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element15 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration15 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration15.Contains(":") && model.Duration15 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert15 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter15 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element16 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration16 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration16.Contains(":") && model.Duration16 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert16 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter16 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element17 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration17 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration17.Contains(":") && model.Duration17 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert17 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter17 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element18 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration18 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration18.Contains(":") && model.Duration18 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert18 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter18 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element19 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration19 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration19.Contains(":") && model.Duration19 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert19 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter19 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element20 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration20 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration20.Contains(":") && model.Duration20 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert20 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter20 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element21 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration21 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration21.Contains(":") && model.Duration21 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert21 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter21 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element22 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration22 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration22.Contains(":") && model.Duration22 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert22 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter22 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element23 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration23 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration23.Contains(":") && model.Duration23 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert23 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter23 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Element24 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.Duration24 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        if (!model.Duration24.Contains(":") && model.Duration24 != "")
                        {
                            rowIndex++;
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                        }
                        model.IsFert24 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.IsFilter24 = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);
                        model.TagName = GetCellValueByIndexForSeq(thesheetdata, currentRow + rowIndex, stringTablePart); currentRow = GetNextColumn(currentRow);

                        modelLst.Add(model);

                        #region old code
                        //for (int i = 0; i < 103; i++)
                        //{
                        //    try
                        //    {
                        //        Cell cell = row.Descendants<Cell>().ElementAt(i);
                        //        int actualCellIndex = CellReferenceToIndex(cell);

                        //        if (i == 0)
                        //            model.SequenceNo = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 1)
                        //            model.StartDate = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 2)
                        //            model.EndDate = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 3)
                        //            model.Type = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 4)
                        //            model.weekly = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 5)
                        //            model.Interval = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 6)
                        //            model.StartTime = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 7)
                        //            model.Element1 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 8)
                        //            model.Duration1 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 9)
                        //            model.IsFert1 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 10)
                        //            model.IsFilter1 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 11)
                        //            model.Element2 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 12)
                        //            model.Duration2 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 13)
                        //            model.IsFert2 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 14)
                        //            model.IsFilter2 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 15)
                        //            model.Element3 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 16)
                        //            model.Duration3 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 17)
                        //            model.IsFert3 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 18)
                        //            model.IsFilter3 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 19)
                        //            model.Element4 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 20)
                        //            model.Duration4 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 21)
                        //            model.IsFert4 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 22)
                        //            model.IsFilter4 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 23)
                        //            model.Element5 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 24)
                        //            model.Duration5 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 25)
                        //            model.IsFert5 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 26)
                        //            model.IsFilter5 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 27)
                        //            model.Element6 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 28)
                        //            model.Duration6 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 29)
                        //            model.IsFert6 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 30)
                        //            model.IsFilter6 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 31)
                        //            model.Element7 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 32)
                        //            model.Duration7 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 33)
                        //            model.IsFert7 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 34)
                        //            model.IsFilter7 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 35)
                        //            model.Element8 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 36)
                        //            model.Duration8 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 37)
                        //            model.IsFert8 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 38)
                        //            model.IsFilter8 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 39)
                        //            model.Element9 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 40)
                        //            model.Duration9 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 41)
                        //            model.IsFert9 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 42)
                        //            model.IsFilter9 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 43)
                        //            model.Element10 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 44)
                        //            model.Duration10 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 45)
                        //            model.IsFert10 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 46)
                        //            model.IsFilter10 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 47)
                        //            model.Element11 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 48)
                        //            model.Duration11 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 49)
                        //            model.IsFert11 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 50)
                        //            model.IsFilter11 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 51)
                        //            model.Element12 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 52)
                        //            model.Duration12 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 53)
                        //            model.IsFert12 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 54)
                        //            model.IsFilter12 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 55)
                        //            model.Element13 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 56)
                        //            model.Duration13 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 57)
                        //            model.IsFert13 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 58)
                        //            model.IsFilter13 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 59)
                        //            model.Element14 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 60)
                        //            model.Duration14 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 61)
                        //            model.IsFert14 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 62)
                        //            model.IsFilter14 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 63)
                        //            model.Element15 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 64)
                        //            model.Duration15 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 65)
                        //            model.IsFert15 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 66)
                        //            model.IsFilter15 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 67)
                        //            model.Element16 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 68)
                        //            model.Duration16 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 69)
                        //            model.IsFert16 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 70)
                        //            model.IsFilter16 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 71)
                        //            model.Element17 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 72)
                        //            model.Duration17 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 73)
                        //            model.IsFert17 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 74)
                        //            model.IsFilter17 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 75)
                        //            model.Element18 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 76)
                        //            model.Duration18 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 77)
                        //            model.IsFert18 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 78)
                        //            model.IsFilter18 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 79)
                        //            model.Element19 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 80)
                        //            model.Duration19 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 81)
                        //            model.IsFert19 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 82)
                        //            model.IsFilter19 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 83)
                        //            model.Element20 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 84)
                        //            model.Duration20 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 85)
                        //            model.IsFert20 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 86)
                        //            model.IsFilter20 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 87)
                        //            model.Element21 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 88)
                        //            model.Duration21 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 89)
                        //            model.IsFert21 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 90)
                        //            model.IsFilter21 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 91)
                        //            model.Element22 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 92)
                        //            model.Duration22 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 93)
                        //            model.IsFert22 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 94)
                        //            model.IsFilter22 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 95)
                        //            model.Element23 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 96)
                        //            model.Duration23 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 97)
                        //            model.IsFert23 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 98)
                        //            model.IsFilter23 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 99)
                        //            model.Element24 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 100)
                        //            model.Duration24 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 101)
                        //            model.IsFert24 = GetValueOfCell(stringTablePart, cell);
                        //        if (i == 102)
                        //            model.IsFilter24 = GetValueOfCell(stringTablePart, cell);
                        //    }
                        //    catch (Exception)
                        //    {

                        //    }
                        //} 
                        #endregion
                        rowIndex++;
                    }
                    rowno++;

                }
                //Get Validation


                #region New Valve Saving
                //Get Distinct valves from all elements
                List<string> eleValve1 = modelLst.Select(x => x.Element1).Distinct().ToList();
                List<string> eleValve2 = modelLst.Select(x => x.Element2).Distinct().ToList();
                List<string> eleValve3 = modelLst.Select(x => x.Element3).Distinct().ToList();
                List<string> eleValve4 = modelLst.Select(x => x.Element4).Distinct().ToList();
                List<string> eleValve5 = modelLst.Select(x => x.Element5).Distinct().ToList();
                List<string> eleValve6 = modelLst.Select(x => x.Element6).Distinct().ToList();
                List<string> eleValve7 = modelLst.Select(x => x.Element7).Distinct().ToList();
                List<string> eleValve8 = modelLst.Select(x => x.Element8).Distinct().ToList();
                List<string> eleValve9 = modelLst.Select(x => x.Element9).Distinct().ToList();
                List<string> eleValve10 = modelLst.Select(x => x.Element10).Distinct().ToList();
                List<string> eleValve11 = modelLst.Select(x => x.Element11).Distinct().ToList();
                List<string> eleValve12 = modelLst.Select(x => x.Element12).Distinct().ToList();
                List<string> eleValve13 = modelLst.Select(x => x.Element13).Distinct().ToList();
                List<string> eleValve14 = modelLst.Select(x => x.Element14).Distinct().ToList();
                List<string> eleValve15 = modelLst.Select(x => x.Element15).Distinct().ToList();
                List<string> eleValve16 = modelLst.Select(x => x.Element16).Distinct().ToList();
                List<string> eleValve17 = modelLst.Select(x => x.Element17).Distinct().ToList();
                List<string> eleValve18 = modelLst.Select(x => x.Element18).Distinct().ToList();
                List<string> eleValve19 = modelLst.Select(x => x.Element19).Distinct().ToList();
                List<string> eleValve20 = modelLst.Select(x => x.Element20).Distinct().ToList();
                List<string> eleValve21 = modelLst.Select(x => x.Element21).Distinct().ToList();
                List<string> eleValve22 = modelLst.Select(x => x.Element22).Distinct().ToList();
                List<string> eleValve23 = modelLst.Select(x => x.Element23).Distinct().ToList();
                List<string> eleValve24 = modelLst.Select(x => x.Element24).Distinct().ToList();

                var allValves = eleValve1.Concat(eleValve2)
                                    .Concat(eleValve3)
                                     .Concat(eleValve4)
                                      .Concat(eleValve5)
                                       .Concat(eleValve6)
                                        .Concat(eleValve7)
                                         .Concat(eleValve8)
                                          .Concat(eleValve9)
                                           .Concat(eleValve10)
                                            .Concat(eleValve11)
                                             .Concat(eleValve12)
                                              .Concat(eleValve13)
                                               .Concat(eleValve14)
                                                .Concat(eleValve15)
                                                 .Concat(eleValve16)
                                                  .Concat(eleValve17)
                                                   .Concat(eleValve18)
                                                    .Concat(eleValve19)
                                                     .Concat(eleValve20)
                                                      .Concat(eleValve21)
                                                       .Concat(eleValve22)
                                                        .Concat(eleValve23)
                                                         .Concat(eleValve24).ToList();
                var distinctValves = allValves.Distinct();
                List<Nrvchannels> nrvchannels = new List<Nrvchannels>();
                List<Nrvchannels> nrvUpdatechannels = new List<Nrvchannels>();

                List<NrseqUpids> nrseqUpids = new List<NrseqUpids>();
                List<NrseqUpids> nrUpdateseqUpids = new List<NrseqUpids>();
                foreach (string itemV in distinctValves)
                {
                    if (itemV != "")
                    {

                        string splN = itemV.Split("N")[1].Split("R")[0];
                        string splR = itemV.Split("N")[1].Split("R")[1].Split("V")[0];
                        string splV = itemV.Split("N")[1].Split("R")[1].Split("V")[1];
                        int shifts = Convert.ToInt32(splN) << 10; //eg: 5 << 10 == 5120
                        int rtuid = Convert.ToInt32(splR) + shifts; //5120+10 = 5130
                        if (!_mainDBContext.Nrvchannels.Any(x => x.ChannelName == itemV))
                        {
                            // int rtuid = (Convert.ToInt32(splN) * 1023) + Convert.ToInt32(splR);

                            //Add
                            Nrvchannels channels = new Nrvchannels();
                            channels.NetworkId = Convert.ToInt32(splN);
                            channels.Rtuno = Convert.ToInt32(splR);
                            channels.ValveNo = Convert.ToInt32(splV);
                            channels.RtuId = rtuid;
                            channels.ChannelName = itemV;
                            channels.UpIds = 0;
                            nrvchannels.Add(channels);
                            await UpdateUpidsProjNodeAsync(Convert.ToInt32(splN), Convert.ToInt32(splR), GlobalConstants.ScheduleNode);

                        }
                        else
                        {
                            var nrvchanel = _mainDBContext.Nrvchannels.Where(x => x.ChannelName == itemV).FirstOrDefault();
                            if (nrvchanel != null)
                            {
                                nrvchanel.UpIds = nrvchanel.UpIds + 1;
                                nrvUpdatechannels.Add(nrvchanel);
                            }
                            await UpdateUpidsProjNodeAsync(Convert.ToInt32(splN), Convert.ToInt32(splR), GlobalConstants.ScheduleNode);
                        }

                        //Update Network RTU
                        if (!_mainDBContext.NrseqUpids.Any(x => x.NetworkNo == Convert.ToInt32(splN) && x.RtuNo == Convert.ToInt32(splR)))
                        {
                            if (!nrseqUpids.Any(x => x.NetworkNo == Convert.ToInt32(splN) && x.RtuNo == Convert.ToInt32(splR)))
                            {
                                NrseqUpids channels = new NrseqUpids();
                                channels.NetworkNo = Convert.ToInt32(splN);
                                channels.RtuNo = Convert.ToInt32(splR);
                                channels.NodeRtuId = rtuid;
                                channels.UpId = 1;
                                nrseqUpids.Add(channels);
                            }
                            // await UpdateUpidsProjNodeAsync(Convert.ToInt32(splN), Convert.ToInt32(splR), GlobalConstants.ScheduleNode);
                        }
                        else
                        {
                            var nrvchanel = _mainDBContext.NrseqUpids.Where(x => x.NetworkNo == Convert.ToInt32(splN) && x.RtuNo == Convert.ToInt32(splR)).FirstOrDefault();
                            if (nrvchanel != null)
                            {
                                nrvchanel.UpId = nrvchanel.UpId + 1;
                                nrUpdateseqUpids.Add(nrvchanel);
                            }
                            // await UpdateUpidsProjNodeAsync(Convert.ToInt32(splN), Convert.ToInt32(splR), GlobalConstants.ScheduleNode);
                        }
                    }
                }

                if (nrvchannels.Count > 0)
                {
                    await _mainDBContext.Nrvchannels.AddRangeAsync(nrvchannels);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (nrvUpdatechannels.Count > 0)
                {
                    _mainDBContext.Nrvchannels.UpdateRange(nrvUpdatechannels);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (nrseqUpids.Count > 0)
                {
                    await _mainDBContext.NrseqUpids.AddRangeAsync(nrseqUpids);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (nrUpdateseqUpids.Count > 0)
                {
                    _mainDBContext.NrseqUpids.UpdateRange(nrUpdateseqUpids);
                    await _mainDBContext.SaveChangesAsync();
                }

                //updatet id seq main

                #region Update Main sequence Id
                var upseqMain = await _mainDBContext.UpdateIdsMainSch.FirstOrDefaultAsync();
                if (upseqMain != null)
                {
                    upseqMain.SeqMaxUpid = upseqMain.SeqMaxUpid + 1;
                    _mainDBContext.UpdateIdsMainSch.Update(upseqMain);
                    await _mainDBContext.SaveChangesAsync();
                }
                else
                {
                    UpdateIdsMainSch updateIdsMainSch = new UpdateIdsMainSch();
                    updateIdsMainSch.SeqMaxUpid = 0;
                    updateIdsMainSch.GwSeqMaxUpId = 0;
                    await _mainDBContext.UpdateIdsMainSch.AddAsync(updateIdsMainSch);
                    await _mainDBContext.SaveChangesAsync();
                }
                #endregion

                List<Nrvchannels> nrvchannelLst = await _mainDBContext.Nrvchannels.ToListAsync();
                #endregion
                List<string> sequenceNofromList = modelLst.Where(x => x.SequenceNo != null || x.SequenceNo != "").Select(c => c.SequenceNo).Distinct().ToList();

                foreach (var ScheduleNO in sequenceNofromList)
                {
                    var seqModel = await _mainDBContext.NewSequence.Where(x => x.SeqNo == Convert.ToInt32(ScheduleNO)).FirstOrDefaultAsync();
                    if (seqModel != null)
                    {
                        List<NewSequenceValveConfig> newSequencesV = await _mainDBContext.NewSequenceValveConfig.Where(x => x.SeqId == seqModel.SeqId).ToListAsync();
                        _mainDBContext.NewSequenceValveConfig.RemoveRange(newSequencesV);
                        await _mainDBContext.SaveChangesAsync();

                        List<NewSequenceWeeklySchedule> newSequencesW = await _mainDBContext.NewSequenceWeeklySchedule.Where(x => x.SeqId == seqModel.SeqId).ToListAsync();
                        _mainDBContext.NewSequenceWeeklySchedule.RemoveRange(newSequencesW);
                        await _mainDBContext.SaveChangesAsync();
                    }

                    List<NewSequenceValveConfig> valveConfig = new List<NewSequenceValveConfig>();
                    NewSequenceValveConfig valve = new NewSequenceValveConfig();
                    List<SequenceImportViewModel> modeltoProcess = new List<SequenceImportViewModel>();
                    modeltoProcess = modelLst.Where(x => x.SequenceNo == ScheduleNO).ToList();
                    if (modeltoProcess != null)
                    {
                        NewSequence newSequence = new NewSequence();
                        var horizontalElement = modeltoProcess.Where(x => x.StartDate != "" && x.EndDate != "" && x.Type != "" && x.StartTime != "").FirstOrDefault();
                        var verticleValve = modeltoProcess.Where(x => x.StartDate == "" && x.EndDate == "" && x.Type == "" && x.StartTime == "").ToList();

                        if (horizontalElement != null)
                        {
                            int newSeqId = 0;
                            if (_mainDBContext.NewSequence.Any(x => x.SeqNo == Convert.ToInt32(horizontalElement.SequenceNo)))
                            {
                                var sequenceToEdit = _mainDBContext.NewSequence.Where(x => x.SeqNo == Convert.ToInt32(horizontalElement.SequenceNo)).FirstOrDefault();
                                sequenceToEdit.PrjId = 4;
                                sequenceToEdit.SeqStartDate = DateTime.ParseExact(horizontalElement.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                sequenceToEdit.SeqEndDate = DateTime.ParseExact(horizontalElement.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                sequenceToEdit.BasisOfOp = "Time";
                                int intervaldays = horizontalElement.Interval == "" ? 0 : Convert.ToInt32(horizontalElement.Interval);
                                sequenceToEdit.IntervalDays = Convert.ToInt32(intervaldays);
                                sequenceToEdit.OperationTypeId = 1;
                                sequenceToEdit.PrjTypeId = 1;
                                sequenceToEdit.SeqName = "Sequence" + horizontalElement.SequenceNo;
                                sequenceToEdit.ValidationState = true;
                                sequenceToEdit.IsValid = true;
                                sequenceToEdit.SeqNo = Convert.ToInt32(horizontalElement.SequenceNo);
                                sequenceToEdit.SeqType = "NRV";
                                sequenceToEdit.StartTime = horizontalElement.StartTime;
                                sequenceToEdit.Upid = sequenceToEdit.Upid + 1;
                                sequenceToEdit.SeqTagName = horizontalElement.TagName;
                                var seq = _mainDBContext.NewSequence.Update(sequenceToEdit);
                                await _mainDBContext.SaveChangesAsync();
                                newSeqId = sequenceToEdit.SeqId;
                                //Weekly Schedule if any
                                if (horizontalElement.weekly != "")
                                {
                                    List<NewSequenceWeeklySchedule> weeklyScheduleLst = new List<NewSequenceWeeklySchedule>();

                                    List<string> weeksdays = horizontalElement.weekly.Split(",").ToList();
                                    if (weeksdays != null)
                                    {
                                        foreach (var itemweek in weeksdays)
                                        {
                                            NewSequenceWeeklySchedule newSequenceWeeklySchedule = new NewSequenceWeeklySchedule();
                                            newSequenceWeeklySchedule.PrjId = 4;
                                            newSequenceWeeklySchedule.PrgId = 1;
                                            newSequenceWeeklySchedule.SeqId = newSeqId;
                                            if (itemweek.ToLower().Trim() == "monday")
                                                newSequenceWeeklySchedule.WeekDayId = 1;
                                            if (itemweek.ToLower().Trim() == "tuesday")
                                                newSequenceWeeklySchedule.WeekDayId = 2;
                                            if (itemweek.ToLower().Trim() == "wednesday")
                                                newSequenceWeeklySchedule.WeekDayId = 3;
                                            if (itemweek.ToLower().Trim() == "thursday")
                                                newSequenceWeeklySchedule.WeekDayId = 4;
                                            if (itemweek.ToLower().Trim() == "friday")
                                                newSequenceWeeklySchedule.WeekDayId = 5;
                                            if (itemweek.ToLower().Trim() == "saturday")
                                                newSequenceWeeklySchedule.WeekDayId = 6;
                                            if (itemweek.ToLower().Trim() == "sunday")
                                                newSequenceWeeklySchedule.WeekDayId = 7;
                                            await _mainDBContext.NewSequenceWeeklySchedule.AddAsync(newSequenceWeeklySchedule);
                                            await _mainDBContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Add Sequence
                                newSequence.PrjId = 4;
                                newSequence.SeqStartDate = DateTime.ParseExact(horizontalElement.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                newSequence.SeqEndDate = DateTime.ParseExact(horizontalElement.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                newSequence.BasisOfOp = "Time";
                                int intervaldays = horizontalElement.Interval == "" ? 0 : Convert.ToInt32(horizontalElement.Interval);
                                newSequence.IntervalDays = Convert.ToInt32(intervaldays);
                                newSequence.OperationTypeId = 1;
                                newSequence.PrjTypeId = 1;
                                newSequence.SeqName = "Sequence" + horizontalElement.SequenceNo;
                                newSequence.ValidationState = true;
                                newSequence.IsValid = true;
                                newSequence.SeqNo = Convert.ToInt32(horizontalElement.SequenceNo);
                                newSequence.SeqType = "NRV";
                                newSequence.StartTime = horizontalElement.StartTime;
                                newSequence.Upid = 0;
                                var seq = await _mainDBContext.NewSequence.AddAsync(newSequence);
                                await _mainDBContext.SaveChangesAsync();
                                newSeqId = newSequence.SeqId;
                                //Weekly Schedule if any
                                if (horizontalElement.weekly != "")
                                {
                                    List<NewSequenceWeeklySchedule> weeklyScheduleLst = new List<NewSequenceWeeklySchedule>();

                                    List<string> weeksdays = horizontalElement.weekly.Split(",").ToList();
                                    if (weeksdays != null)
                                    {
                                        foreach (var itemweek in weeksdays)
                                        {
                                            NewSequenceWeeklySchedule newSequenceWeeklySchedule = new NewSequenceWeeklySchedule();
                                            newSequenceWeeklySchedule.PrjId = 4;
                                            newSequenceWeeklySchedule.PrgId = 1;
                                            newSequenceWeeklySchedule.SeqId = newSeqId;
                                            if (itemweek.ToLower() == "monday")
                                                newSequenceWeeklySchedule.WeekDayId = 1;
                                            if (itemweek.ToLower() == "tuesday")
                                                newSequenceWeeklySchedule.WeekDayId = 2;
                                            if (itemweek.ToLower() == "wednesday")
                                                newSequenceWeeklySchedule.WeekDayId = 3;
                                            if (itemweek.ToLower() == "thursday")
                                                newSequenceWeeklySchedule.WeekDayId = 4;
                                            if (itemweek.ToLower() == "friday")
                                                newSequenceWeeklySchedule.WeekDayId = 5;
                                            if (itemweek.ToLower() == "saturday")
                                                newSequenceWeeklySchedule.WeekDayId = 6;
                                            if (itemweek.ToLower() == "sunday")
                                                newSequenceWeeklySchedule.WeekDayId = 7;
                                            await _mainDBContext.NewSequenceWeeklySchedule.AddAsync(newSequenceWeeklySchedule);
                                            await _mainDBContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }

                            #region valve saving
                            //Add Valves
                            string nextValveStartTime = "00:00";
                            //Element 1
                            if (horizontalElement.Element1 != "")
                            {
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 1;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element1;
                                valve.ValveStartTime = horizontalElement.StartTime;
                                valve.ValveStartDuration = horizontalElement.Duration1;
                                valve.IsFertilizerRelated = horizontalElement.IsFert1 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter1 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element1).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(horizontalElement.StartTime, horizontalElement.Duration1);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element1 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 1;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element1;
                                            valve.ValveStartTime = horizontalElement.StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration1;
                                            valve.IsFertilizerRelated = vitem.IsFert1 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter1 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element1).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }
                                //await _mainDBContext.NewSequenceValveConfig.AddRangeAsync(valveConfig);
                                // await _mainDBContext.SaveChangesAsync();
                            }
                            //Element 2
                            if (horizontalElement.Element2 != "")
                            {
                                string element2StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 2;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element2;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration2;
                                valve.IsFertilizerRelated = horizontalElement.IsFert2 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter2 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element2).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration2);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element2 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 2;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element2;
                                            valve.ValveStartTime = element2StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration2;
                                            valve.IsFertilizerRelated = vitem.IsFert2 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter2 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element2).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 3
                            if (horizontalElement.Element3 != "")
                            {
                                string element3StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 3;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element3;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration3;
                                valve.IsFertilizerRelated = horizontalElement.IsFert3 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter3 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element3).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration3);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element3 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 3;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element3;
                                            valve.ValveStartTime = element3StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration3;
                                            valve.IsFertilizerRelated = vitem.IsFert3 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter3 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element3).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 4
                            if (horizontalElement.Element4 != "")
                            {
                                string element4StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 4;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element4;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration4;
                                valve.IsFertilizerRelated = horizontalElement.IsFert4 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter4 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element4).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration4);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element4 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 4;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element4;
                                            valve.ValveStartTime = element4StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration4;
                                            valve.IsFertilizerRelated = vitem.IsFert4 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter4 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element4).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 5
                            if (horizontalElement.Element5 != "")
                            {
                                string element5StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 5;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element5;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration5;
                                valve.IsFertilizerRelated = horizontalElement.IsFert5 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter5 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element5).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration5);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element5 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 5;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element5;
                                            valve.ValveStartTime = element5StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration5;
                                            valve.IsFertilizerRelated = vitem.IsFert5 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter5 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element5).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 6
                            if (horizontalElement.Element6 != "")
                            {
                                string element6StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 6;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element6;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration6;
                                valve.IsFertilizerRelated = horizontalElement.IsFert6 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter6 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element6).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration6);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element6 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 6;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element6;
                                            valve.ValveStartTime = element6StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration6;
                                            valve.IsFertilizerRelated = vitem.IsFert6 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter6 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element6).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 7
                            if (horizontalElement.Element7 != "")
                            {
                                string element7StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 7;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element7;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration7;
                                valve.IsFertilizerRelated = horizontalElement.IsFert7 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter7 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element7).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration7);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element7 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 7;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element7;
                                            valve.ValveStartTime = element7StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration7;
                                            valve.IsFertilizerRelated = vitem.IsFert7 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter7 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element7).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 8
                            if (horizontalElement.Element8 != "")
                            {
                                string element8StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 8;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element8;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration8;
                                valve.IsFertilizerRelated = horizontalElement.IsFert8 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter8 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element8).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration8);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element8 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 8;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element8;
                                            valve.ValveStartTime = element8StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration8;
                                            valve.IsFertilizerRelated = vitem.IsFert8 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter8 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element8).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 9
                            if (horizontalElement.Element9 != "")
                            {
                                string element9StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 9;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element9;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration9;
                                valve.IsFertilizerRelated = horizontalElement.IsFert9 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter9 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element9).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration9);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element9 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 9;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element9;
                                            valve.ValveStartTime = element9StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration9;
                                            valve.IsFertilizerRelated = vitem.IsFert9 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter9 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element9).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 10
                            if (horizontalElement.Element10 != "")
                            {
                                string element10StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 10;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element10;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration10;
                                valve.IsFertilizerRelated = horizontalElement.IsFert10 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter10 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element10).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration10);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element10 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 10;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element10;
                                            valve.ValveStartTime = element10StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration10;
                                            valve.IsFertilizerRelated = vitem.IsFert10 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter10 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element10).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 11
                            if (horizontalElement.Element11 != "")
                            {
                                string element11StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 11;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element11;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration11;
                                valve.IsFertilizerRelated = horizontalElement.IsFert11 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter11 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element11).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration11);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element11 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 11;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element11;
                                            valve.ValveStartTime = element11StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration11;
                                            valve.IsFertilizerRelated = vitem.IsFert11 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter11 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element11).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 12
                            if (horizontalElement.Element12 != "")
                            {
                                string element12StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 12;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element12;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration12;
                                valve.IsFertilizerRelated = horizontalElement.IsFert12 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter12 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element12).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration12);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element12 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 12;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element12;
                                            valve.ValveStartTime = element12StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration12;
                                            valve.IsFertilizerRelated = vitem.IsFert12 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter12 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element12).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 13
                            if (horizontalElement.Element13 != "")
                            {
                                string element13StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 13;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element13;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration13;
                                valve.IsFertilizerRelated = horizontalElement.IsFert13 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter13 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element13).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration13);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element13 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 13;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element13;
                                            valve.ValveStartTime = element13StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration13;
                                            valve.IsFertilizerRelated = vitem.IsFert13 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter13 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element13).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 14
                            if (horizontalElement.Element14 != "")
                            {
                                string element14StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 14;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element14;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration14;
                                valve.IsFertilizerRelated = horizontalElement.IsFert14 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter14 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element14).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration14);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element14 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 14;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element14;
                                            valve.ValveStartTime = element14StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration14;
                                            valve.IsFertilizerRelated = vitem.IsFert14 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter14 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element14).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 15
                            if (horizontalElement.Element15 != "")
                            {
                                string element15StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 15;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element15;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration15;
                                valve.IsFertilizerRelated = horizontalElement.IsFert15 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter15 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element15).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration15);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element15 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 15;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element15;
                                            valve.ValveStartTime = element15StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration15;
                                            valve.IsFertilizerRelated = vitem.IsFert15 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter15 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element15).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 16
                            if (horizontalElement.Element16 != "")
                            {
                                string element16StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 16;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element16;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration16;
                                valve.IsFertilizerRelated = horizontalElement.IsFert16 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter16 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element16).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration16);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element16 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 16;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element16;
                                            valve.ValveStartTime = element16StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration16;
                                            valve.IsFertilizerRelated = vitem.IsFert16 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter16 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element16).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 17
                            if (horizontalElement.Element17 != "")
                            {
                                string element17StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 17;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element17;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration17;
                                valve.IsFertilizerRelated = horizontalElement.IsFert17 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter17 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element17).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration17);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element17 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 17;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element17;
                                            valve.ValveStartTime = element17StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration17;
                                            valve.IsFertilizerRelated = vitem.IsFert17 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter17 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element17).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 18
                            if (horizontalElement.Element18 != "")
                            {
                                string element18StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 18;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element18;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration18;
                                valve.IsFertilizerRelated = horizontalElement.IsFert18 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter18 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element18).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration18);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element18 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 18;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element18;
                                            valve.ValveStartTime = element18StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration18;
                                            valve.IsFertilizerRelated = vitem.IsFert18 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter18 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element18).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 19
                            if (horizontalElement.Element19 != "")
                            {
                                string element19StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 19;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element19;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration19;
                                valve.IsFertilizerRelated = horizontalElement.IsFert19 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter19 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element19).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration19);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element19 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 19;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element19;
                                            valve.ValveStartTime = element19StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration19;
                                            valve.IsFertilizerRelated = vitem.IsFert19 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter19 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element19).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 20
                            if (horizontalElement.Element20 != "")
                            {
                                string element20StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 20;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element20;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration20;
                                valve.IsFertilizerRelated = horizontalElement.IsFert20 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter20 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element20).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration20);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element20 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 20;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element20;
                                            valve.ValveStartTime = element20StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration20;
                                            valve.IsFertilizerRelated = vitem.IsFert20 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter20 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element20).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 21
                            if (horizontalElement.Element21 != "")
                            {
                                string element21StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 21;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element21;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration21;
                                valve.IsFertilizerRelated = horizontalElement.IsFert21 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter21 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element21).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration21);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element21 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 21;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element21;
                                            valve.ValveStartTime = element21StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration21;
                                            valve.IsFertilizerRelated = vitem.IsFert21 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter21 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element21).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 22
                            if (horizontalElement.Element22 != "")
                            {
                                string element22StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 22;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element22;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration22;
                                valve.IsFertilizerRelated = horizontalElement.IsFert22 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter22 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element22).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration22);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element22 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 22;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element22;
                                            valve.ValveStartTime = element22StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration22;
                                            valve.IsFertilizerRelated = vitem.IsFert22 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter22 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element22).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 23
                            if (horizontalElement.Element23 != "")
                            {
                                string element23StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 23;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element23;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration23;
                                valve.IsFertilizerRelated = horizontalElement.IsFert23 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter23 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element23).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration23);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element23 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 23;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element23;
                                            valve.ValveStartTime = element23StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration23;
                                            valve.IsFertilizerRelated = vitem.IsFert23 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter23 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element23).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            //Element 24
                            if (horizontalElement.Element24 != "")
                            {
                                string element24StartTime = nextValveStartTime;
                                valve = new NewSequenceValveConfig();
                                valve.HorizGrId = 24;
                                valve.SeqId = newSeqId;
                                valve.Valve = horizontalElement.Element24;
                                valve.ValveStartTime = nextValveStartTime;
                                valve.ValveStartDuration = horizontalElement.Duration24;
                                valve.IsFertilizerRelated = horizontalElement.IsFert24 == "1" ? true : false;
                                valve.IsFlushRelated = horizontalElement.IsFilter24 == "1" ? true : false;
                                valve.IsHorizontal = true;
                                valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == horizontalElement.Element24).Select(x => x.Id).FirstOrDefault();

                                valveConfig.Add(valve);
                                nextValveStartTime = GetValveStartTime(nextValveStartTime, horizontalElement.Duration24);
                                //Get verticle and save
                                if (verticleValve != null)
                                {
                                    foreach (var vitem in verticleValve)
                                    {
                                        if (vitem.Element24 != "")
                                        {
                                            valve = new NewSequenceValveConfig();
                                            valve.HorizGrId = 24;
                                            valve.SeqId = newSeqId;
                                            valve.Valve = vitem.Element24;
                                            valve.ValveStartTime = element24StartTime;
                                            valve.ValveStartDuration = horizontalElement.Duration24;
                                            valve.IsFertilizerRelated = vitem.IsFert24 == "1" ? true : false;
                                            valve.IsFlushRelated = vitem.IsFilter24 == "1" ? true : false;
                                            valve.IsHorizontal = false;
                                            valve.ScheduleNo = Convert.ToInt32(ScheduleNO);
                                            valve.ChannelId = nrvchannelLst.Where(x => x.ChannelName == vitem.Element24).Select(x => x.Id).FirstOrDefault();
                                            valveConfig.Add(valve);
                                        }

                                    }
                                }

                            }
                            #endregion
                        }
                        else
                        {
                            result.Errors.Add($"Could not upload sequence file");
                            return result;
                            //continue;
                        }

                    }
                    await _mainDBContext.NewSequenceValveConfig.AddRangeAsync(valveConfig);
                    await _mainDBContext.SaveChangesAsync();


                    //Update Gateway and Node Upids
                    // await UpdateUpidsProjNodeAsync(GwSrNo, NodeId, GlobalConstants.Config);
                }

                if (result.Errors.Count == 0)
                    result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(UploadService) + "." + nameof(AddScheduleSetting) + "]" + ex);
                var result = new OperationResult { Succeeded = false };
                result.Errors.Add($"Error occurred while adding attachment(s).");
                return result;
            }
        }



        public string GetValveStartTime(string starttime, string valveSuration)
        {
            var totalInMinutes = (Convert.ToInt32(starttime.Split(":")[0]) * 60) + Convert.ToInt32(starttime.Split(":")[1]);
            var otherMinutes = (Convert.ToInt32(valveSuration.Split(":")[0]) * 60) + Convert.ToInt32(valveSuration.Split(":")[1]);
            var grandTotal = otherMinutes + totalInMinutes;
            var VH = grandTotal / 60;
            var VM = (grandTotal % 60);
            var FVM = VM.ToString().PadLeft(VM.ToString().Length + 1, '0');
            /// //string ss = (TimeSpan.Parse(starttime) + TimeSpan.Parse(valveSuration)).ToString();
            //TimeSpan s1 = TimeSpan.Parse("0:15");
            //TimeSpan s2 = TimeSpan.Parse("0:45");

            //TimeSpan s3 = s1 + s2;
            /////  var DurationToHour = ss.Split(":")[0] + ":" + ss.Split(":")[1];
            var DurationToHour = VH + ":" + VM;
            return DurationToHour.ToString();
        }

        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        public static int? GetColumnIndex(string columnNameOrCellReference)
        {
            int columnIndex = 0;
            int factor = 1;
            for (int pos = columnNameOrCellReference.Length - 1; pos >= 0; pos--) // R to L
            {
                if (Char.IsLetter(columnNameOrCellReference[pos])) // for letters (columnName)
                {
                    columnIndex += factor * ((columnNameOrCellReference[pos] - 'A') + 1);
                    factor *= 26;
                }
            }
            return columnIndex;

        }


        /// <summary>
        /// Get Value of Cell
        /// </summary>
        /// <param name="spreadsheetdocument">SpreadSheet Document Object</param>
        /// <param name="cell">Cell Object</param>
        /// <returns>The Value in Cell</returns>
        private static string GetValueOfCell(SharedStringTablePart sharedString, Cell cell)
        {
            // Get value in Cell
            // SharedStringTablePart sharedString = spreadsheetdocument.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue == null)
            {
                return string.Empty;
            }
            string cellValue = cell.CellValue.InnerText;

            // The condition that the Cell DataType is SharedString
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return sharedString.SharedStringTable.ChildElements[int.Parse(cellValue)].InnerText;
            }
            else
            {
                return cellValue;
            }
        }


        #endregion
        public async Task UpdateUpidsProjNodeAsync(int GwSrNo, int NodeId, string type)
        {
            try
            {
                var projectuids = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                if (projectuids != null)
                {
                    projectuids.ProjectUpId = projectuids.ProjectUpId + 1;
                    _mainDBContext.UpdateIdsProject.Update(projectuids);
                    await _mainDBContext.SaveChangesAsync();
                }

                //Get actual node no
                //int nodenoForDB = NodeId & 1023;
                //This will increment for any change in configuration setting of GW / Nodes
                if (type == GlobalConstants.Config)
                {
                    var updateIds = await _mainDBContext.UpdateIdsRequired.Where(x => x.NodeId == NodeId && x.NetworkNo == GwSrNo).FirstOrDefaultAsync();
                    updateIds.ConfigUid = updateIds.ConfigUid + 1;
                    _mainDBContext.UpdateIdsRequired.Update(updateIds);
                    await _mainDBContext.SaveChangesAsync();
                }
                //This will increment for any change in Valve Relation w.r.t. Application (Filter/fert/pump/master)
                if (type == GlobalConstants.VRT)
                {
                    var updateIds = await _mainDBContext.UpdateIdsRequired.Where(x => x.NodeId == NodeId && x.NetworkNo == GwSrNo).FirstOrDefaultAsync();
                    updateIds.VrtUid = updateIds.VrtUid + 1;
                    _mainDBContext.UpdateIdsRequired.Update(updateIds);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (type == GlobalConstants.SensorUpid)
                {
                    var updateIds = await _mainDBContext.UpdateIdsRequired.Where(x => x.NodeId == NodeId && x.NetworkNo == GwSrNo).FirstOrDefaultAsync();
                    updateIds.SensorUid = updateIds.SensorUid + 1;
                    _mainDBContext.UpdateIdsRequired.Update(updateIds);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (type == GlobalConstants.ScheduleNode)
                {
                    var updateIds = await _mainDBContext.UpdateIdsRequired.Where(x => x.NodeId == NodeId).FirstOrDefaultAsync();
                    updateIds.ScheduleNodeUid = updateIds.ScheduleNodeUid + 1;
                    _mainDBContext.UpdateIdsRequired.Update(updateIds);
                    await _mainDBContext.SaveChangesAsync();
                }

            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task ChangeUpdateIdsAsync(string type, int nodeid)
        {
            try
            {
                //This will increment for any change in Project 
                UpdateIdsProject updateIdsProject = _mainDBContext.UpdateIdsProject.FirstOrDefault();
                if (updateIdsProject == null)
                {
                    UpdateIdsProject addModel = new UpdateIdsProject
                    {
                        ProjectUpId = 1
                    };

                    await _mainDBContext.UpdateIdsProject.AddAsync(addModel);
                    await _mainDBContext.SaveChangesAsync();
                }
                else
                {
                    updateIdsProject.ProjectUpId++;
                    await _mainDBContext.SaveChangesAsync();

                }

                //UpdateIds updateIds = _mainDBContext.UpdateIds.Where(x => x.NodeId == nodeid).FirstOrDefault();
                //if (updateIds == null)
                //{
                //    UpdateIds addModel = new UpdateIds
                //    {
                //        NodeId = nodeid,
                //        NodeUid = 0,
                //        ConfigUid = 0,
                //        VrtUid = 0,
                //        SensorUid = 0,
                //        ScheduleNodeUid = 0,
                //        ScheduleSequenceUid = 0,
                //        MainSchUid = 0,
                //        FilterUid = 0
                //    };

                //    await _mainDBContext.UpdateIds.AddAsync(addModel);
                //    await _mainDBContext.SaveChangesAsync();

                //}
                ////This will increment for any change in configuration setting of GW / Nodes
                //if (type == GlobalConstants.Config)
                //{
                //    updateIds.ConfigUid = updateIds.ConfigUid++;
                //}
                ////This will increment for any change in Valve Relation w.r.t. Application (Filter/fert/pump/master)
                //if (type == GlobalConstants.VRT)
                //{
                //    updateIds.ConfigUid = updateIds.ConfigUid++;
                //}
                //if (type == GlobalConstants.SensorUpid)
                //{
                //    updateIds.SensorUid = updateIds.SensorUid++;
                //}
                //await _mainDBContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// validate file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private OperationResult IsValidFile(IFormFile file)
        {
            List<string> AllowedUploadFileExtensions = new List<string>();
            AllowedUploadFileExtensions.Add(".xls");
            AllowedUploadFileExtensions.Add(".xlsx");
            AllowedUploadFileExtensions.Add(".csv");
            string fileName = Path.GetFileName(file.FileName);
            if (file.Length == 0)
            {
                return new OperationResult { Succeeded = false, Errors = { $"{fileName} is empty." } };
            }
            if (fileName.Length > 255)
            {
                return new OperationResult { Succeeded = false, Errors = { $"{fileName} file name length is too large. Maximum file name length allowed is 255 characters." } };
            }
            var fileExtension = Path.GetExtension(fileName);
            if (!AllowedUploadFileExtensions.Contains(fileExtension.ToLower()))
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Errors = { $"Could not upload {fileName}. File type is not supported." }
                };
            }
            return new OperationResult { Succeeded = true };
        }


        private static string GetCellValueByIndex(SheetData thesheetdata, string cellReference, SharedStringTablePart stringTablePart)
        {

            string value = "0";
            if (thesheetdata.Descendants<Cell>().Where(c => c.CellReference == cellReference).FirstOrDefault() != null)
            {
                Cell thecurrentcell = thesheetdata.Descendants<Cell>().Where(c => c.CellReference == cellReference).FirstOrDefault();
                //if (cell.InnerXml != "")
                //{
                //    value = cell.CellValue.InnerXml;
                //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                //    {
                //        return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                //    }
                //    else
                //    {
                //        return value;
                //    }
                //}
                if (thecurrentcell.DataType != null)
                {
                    if (thecurrentcell.DataType == CellValues.SharedString)
                    {
                        int id;
                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                        {
                            SharedStringItem item = stringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                            //if (item.Text != null)
                            //{
                            //    //code to take the string value  
                            //    // excelResult.Append(item.Text.Text + " ");
                            //    string ss = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                            //    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;

                            //}
                            if (item.InnerText != null)
                            {
                                value = item.InnerText;
                            }
                            else if (item.InnerXml != null)
                            {
                                value = item.InnerXml;
                            }
                        }
                    }
                }
                else
                {
                    value = (thecurrentcell.InnerText == "" ? "0" : thecurrentcell.InnerText);
                }

            }
            return value;
        }
        private static string GetCellValueByIndexForSeq(SheetData thesheetdata, string cellReference, SharedStringTablePart stringTablePart)
        {

            string value = "";
            if (thesheetdata.Descendants<Cell>().Where(c => c.CellReference == cellReference).FirstOrDefault() != null)
            {
                Cell thecurrentcell = thesheetdata.Descendants<Cell>().Where(c => c.CellReference == cellReference).FirstOrDefault();
                //if (cell.InnerXml != "")
                //{
                //    value = cell.CellValue.InnerXml;
                //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                //    {
                //        return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                //    }
                //    else
                //    {
                //        return value;
                //    }
                //}
                if (thecurrentcell.DataType != null)
                {
                    if (thecurrentcell.DataType == CellValues.SharedString)
                    {
                        int id;
                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                        {
                            SharedStringItem item = stringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                            //if (item.Text != null)
                            //{
                            //    //code to take the string value  
                            //    // excelResult.Append(item.Text.Text + " ");
                            //    string ss = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                            //    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;

                            //}
                            if (item.InnerText != null)
                            {
                                value = item.InnerText;
                            }
                            else if (item.InnerXml != null)
                            {
                                value = item.InnerXml;
                            }
                        }
                    }
                }
                else
                {
                    value = (thecurrentcell.InnerText == "" ? "" : thecurrentcell.InnerText);
                }

            }
            return value;
        }
        private string GetValue(Cell thecurrentcell, SharedStringTablePart stringTablePart)
        {
            string value = "";
            if (thecurrentcell.CellValue != null)
            {
                value = thecurrentcell.CellValue.InnerText;
                //if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                //{
                //    return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
                //}
                //return value;
                if (thecurrentcell.DataType != null)
                {
                    if (thecurrentcell.DataType == CellValues.SharedString)
                    {
                        int id;
                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                        {
                            SharedStringItem item = stringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                            if (item.InnerText != null)
                            {
                                value = item.InnerText;
                            }
                            else if (item.InnerXml != null)
                            {
                                value = item.InnerXml;
                            }
                        }
                    }
                }
                else
                {
                    value = (thecurrentcell.InnerText == "" ? "0" : thecurrentcell.InnerText);
                }


            }
            return value;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            //if (cell.CellValue == null)
            //{
            //    return "";
            //}
            //string value = cell.CellValue.InnerXml;
            //if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            //{
            //    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            //}
            //else
            //{
            //    return value;
            //}

            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        //public int CellReferenceToIndex(Cell cell)
        //{
        //    int index = -1;
        //    string reference = cell.CellReference.ToString().ToUpper();
        //    foreach (char ch in reference)
        //    {
        //        if (Char.IsLetter(ch))
        //        {
        //            int value = (int)ch - (int)'A';
        //            index = (index + 1) * 26 + value;
        //        }
        //        else
        //            return index;
        //    }
        //    return index;
        //}
        private static int CellReferenceToIndex(Cell cell)
        {
            int index = 0;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index == 0) ? value : ((index + 1) * 26) + value;
                }
                else
                    return index;
            }
            return index;
        }


        // it will return a row based on worksheet and rowindex            
        public Row RetrieveRow(Worksheet worksheet, uint rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
            Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        /// <summary>
        /// Get list of rows for processing
        /// </summary>
        /// <param name="thesheetdata"></param>
        /// <returns></returns>
        public List<Row> GetListOfRowsToProcess(SheetData thesheetdata)
        {
            List<Row> rows = thesheetdata.Elements<Row>().Where(r => r.Elements<Cell>().Any(ce => ce.DataType != null)).ToList();
            return rows;
        }

        static string GetNextColumn(string col)
        {
            char[] charArr = col.ToCharArray();
            var cur = Convert.ToChar((int)charArr[charArr.Length - 1]);
            if (cur == 'Z')
            {
                if (charArr.Length == 1)
                {
                    return "AA";
                }
                else
                {
                    char[] newArray = charArr.Take(charArr.Length - 1).ToArray();
                    var ret = GetNextColumn(new string(newArray));
                    return ret + "A";
                }
            }
            charArr[charArr.Length - 1] = Convert.ToChar((int)charArr[charArr.Length - 1] + 1);
            return new string(charArr);
        }

        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, int rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
                Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }

        private static Cell GetCell(Worksheet worksheet, string columnName, int rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;

            Cell cell = row.Elements<Cell>().Where(c => string.Compare
                                                   (c.CellReference.Value, columnName +
                                                                           rowIndex, true) == 0).First();

            return cell;
        }

        private int HexToDecimal(string sHexValue)
        {
            int iNumber = int.Parse(sHexValue, System.Globalization.NumberStyles.HexNumber);
            return iNumber;
        }

        private string DecimalToHex(int sDecValue)
        {
            return sDecValue.ToString("x");
        }
    }

}
