using AutoMapper;
using IDM.DTO;
using IDM.DTO.Main;
using IDM.Model;
using IDM.Model.Common;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Repository.Main.Interface;
using IDM.Service.Common.Interface;
using IDM.Service.Common.Service;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IDM.Service.Main.Service
{
    public class DynamicStagingService : IDynamicStagingService
    {
        private readonly IDynamicStagingRepository _dynamicStagingRepository;
        private readonly IMapper _mapper;
        //protected readonly IEmailService _emailService;
        //protected readonly IUserService _userService;

        public DynamicStagingService(IDynamicStagingRepository dynamicStagingRepository, IMapper mapper)
        {
            _dynamicStagingRepository = dynamicStagingRepository;
            _mapper = mapper;
            //_emailService = emailService;
            //_userService = userService;
        }

        //protected ConfigDTO GetConfiguration()
        //{
        //    return new ConfigDTO
        //    {
        //        MQConnectionFile = GetAppSetting("MQConnectionFile"),
        //        MQTransaction = GetAppSetting("MQTransaction"),
        //        MQTransactionTrial = GetAppSetting("MQTransactionTrial"),
        //        MQVersion = GetAppSetting("MQVersion"),
        //        MQExcludeColumn = GetAppSetting("MQExcludeColumn"),
        //        MQAdjustColumn = GetAppSetting("MQAdjustColumn"),

        //        SMTPHost = GetAppSetting("SMTPHost"),
        //        SMTPPort = GetAppSetting("SMTPPort"),
        //        EmailSender = GetAppSetting("EmailSender"),
        //        DefaultEmailRecipients = GetAppSetting("DefaultEmailRecipients"),
        //        Website = GetAppSetting("Website")
        //    };
        //}

        //protected string GetAppSetting(string key)
        //{
        //    return ConfigurationManager.AppSettings[key]?.ToString() ?? string.Empty;
        //}


        //public DynamicStagingService()
        //{
        //    // Temporary bridge - manually instantiate services until all controllers use proper DI
        //    var config = GetConfiguration();
        //    _userService = new IDM.Service.Common.Service.UserService();
        //    _emailService = new IDM.Service.Common.Service.EmailService(_userService, config, new IDM.Web.Utility.MailSender());
        //}

        public class TableData
        {
            public List<string> Columns { get; set; }
            public List<List<string>> Data { get; set; }
        }

        public async Task<IEnumerable<DynamicStagingDTO>> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var entities = await _dynamicStagingRepository.GetByJobAndAnalysisAsync(table, amethystJob, analysis, analysisTrial);
            // return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity));
            // Add .ToList() here to force the mapping to happen NOW
            return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity)).ToList();
        }

        public async Task<IEnumerable<DynamicStagingDTO>> GetDataStagingDefectAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea)
        {
            var entities = await _dynamicStagingRepository.GetDataStagingDefectAsync(table, amethystJob, analysis, analysisTrial, area, subArea);
            // return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity));
            // Add .ToList() here to force the mapping to happen NOW
            return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity)).ToList();
        }

        public async Task<IEnumerable<DynamicStagingDTO>> GetDataStagingDefectWithDataStagingWImageParamsAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var entities = await _dynamicStagingRepository.GetDataStagingDefectWithDataStagingWImageParamsAsync(table, amethystJob, analysis, analysisTrial);
            // return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity));
            // Add .ToList() here to force the mapping to happen NOW
            return entities.Select(entity => _mapper.Map<DynamicStagingDTO>(entity)).ToList();
        }
        public async Task<DataTable> GetByJobAndAnalysisDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var entities = await _dynamicStagingRepository.GetByJobAndAnalysisDataTableAsync(table, amethystJob, analysis, analysisTrial);
            return entities;
        }

        public async Task<DataTable> GetDataStagingDefectDataTableAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea)
        {
            var entities = await _dynamicStagingRepository.GetDataStagingDefectDataTableAsync(table, amethystJob, analysis, analysisTrial, area, subArea);
            return entities;
        }


        public async Task<bool> UpdateStagingDataValueAsync(string Table, DataIdValuePair _DataIdValuePair, DataNameValuePair _DataNameValuePair)
        {
            try
            {
                return await _dynamicStagingRepository.UpdateStagingDataValueAsync(Table, _DataIdValuePair, _DataNameValuePair);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception($"Error in service updating staging data value: {ex.Message}", ex);
            }
        }
        public async Task<bool> SetApprovalAsync(string table, string amethystJob, string analysis, int analysisTrial, string status, string toolName, object tableData, string updatedBy)
        {
            try
            {
                // Call repository to update the approval status
                var result = await _dynamicStagingRepository.SetApprovalAsync(table, amethystJob, analysis, analysisTrial, status, toolName, tableData, updatedBy);
                
                // Add any additional business logic here if needed
                // For example: logging, notifications, audit trail, etc.
                
                return result;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception($"Error in service setting approval status: {ex.Message}", ex);
            }
        }

        public async Task<string> GetCustomerAsync(string amethystJob, string analysis, int analysisTrial)
        {
            try
            {
                // Call repository to get customer information
                var customer = await _dynamicStagingRepository.GetCustomerAsync(amethystJob, analysis, analysisTrial);
                
                return customer;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception($"Error in service getting customer: {ex.Message}", ex);
            }
        }

        public async Task<string> GetStatusAsync(string amethystJob, string analysis, int analysisTrial)
        {
            try
            {
                // Call repository to get status information
                var status = await _dynamicStagingRepository.GetStatusAsync(amethystJob, analysis, analysisTrial);
                
                return status;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw new Exception($"Error in service getting status: {ex.Message}", ex);
            }
        }

        public async Task<bool> MQUploadPreparationParameter(object table, ConfigDTO configDTO, string destinationTable, string approver)
        {
            return await CreateAndUploadMQTable(configDTO, destinationTable, table, approver);
        }

        private async Task<bool> CreateAndUploadMQTable(ConfigDTO configDTO, string transaction, object table, string approver)
        {
            DataTable dataTable = new DataTable();

            // Code logic cleanup; applied on smart-quality items(DATA_STAGING_2KX, DATA_STAGING_DEFECT); 
            //can be applied on previous existing, non-image items as well (DATA_STAGING_GRAVIMETRY, DATA_STAGING_ICMATERIALS, DATA_STAGING_PH, DATA_STAGING_PSD) 
            if (table is DataTable dt)
            {
                // 1. Create a new table to hold the string versions of everything
                DataTable stringTable = new DataTable();

                foreach (DataColumn col in dt.Columns)
                {
                    // Add every column as a string type
                    stringTable.Columns.Add(col.ColumnName, typeof(string));
                }

                // 2. Migrate and format the data
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = stringTable.NewRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string colName = dt.Columns[i].ColumnName;
                        object rawValue = row[i];

                        if (rawValue == DBNull.Value || rawValue == null)
                        {
                            newRow[i] = string.Empty;
                        }
                        // 3. Apply the specific "Date" formatting logic
                        else if (colName.Contains("Date"))
                        {
                            if (DateTime.TryParse(rawValue.ToString(), out DateTime dtValue))
                            {
                                newRow[i] = dtValue.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                newRow[i] = rawValue.ToString();
                            }
                        }
                        else
                        {
                            // Just convert any other type (int, decimal, etc.) to string
                            newRow[i] = rawValue.ToString();
                        }
                    }
                    stringTable.Rows.Add(newRow);
                }

                // 4. Update the reference to the new string-based table
                dataTable = stringTable;
                dataTable.AcceptChanges();
            }
            // if its a TableData that has (List<string> Columns, List<string> Data)
            else
            {
                // Cast the table object to TableData type
                var tableData = table as dynamic;

                // Create columns from the Columns property
                if (tableData?.Columns != null)
                {
                    foreach (var column in tableData.Columns)
                    {
                        dataTable.Columns.Add(column.ToString());
                    }
                }

                // Add rows from the Data property
                if (tableData?.Data != null)
                {
                    foreach (var row in tableData.Data)
                    {
                        var dataRow = dataTable.NewRow();
                        int columnIndex = 0;
                        foreach (var cell in row)
                        {
                            if (columnIndex < dataTable.Columns.Count)
                            {
                                string cellValue = cell?.ToString() ?? string.Empty;

                                // Check if this is a date column and format the date
                                if (dataTable.Columns[columnIndex].ColumnName.ToUpper().Contains("DATE") && !string.IsNullOrEmpty(cellValue))
                                {
                                    // Try to parse and convert to DB2 format (YYYY-MM-DD)
                                    if (DateTime.TryParse(cellValue, out DateTime dateValue))
                                    {
                                        cellValue = dateValue.ToString("yyyy-MM-dd");
                                    }
                                }

                                dataRow[columnIndex] = cellValue;
                                columnIndex++;
                            }
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            } 

            return await MQUpload(dataTable, configDTO, transaction);
        }

        public async Task<bool> MQUpload(DataTable MQDataTable, ConfigDTO configDTO, string transaction)
        {
            try
            {
                foreach (DataRow row in MQDataTable.Rows)
                {
                    PDBAXLib.PdbClass PDB = new PDBAXLib.PdbClass();
                    PDB.init(transaction, configDTO.MQVersion, configDTO.MQConnectionFile);
                    while (PDB.reupload()) ;
                    PDB.format("Detail");

                    foreach (DataColumn col in MQDataTable.Columns)
                    {
                        string fieldValue = row[col].ToString();
                        
                        // Additional date formatting for MQ
                        if (!string.IsNullOrEmpty(fieldValue) && col.ColumnName.ToUpper().Contains("DATE"))
                        {
                            if (DateTime.TryParse(fieldValue, out DateTime dateValue))
                            {
                                // Use DB2 ISO format for MQ
                                fieldValue = dateValue.ToString("yyyy-MM-dd");
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(fieldValue))
                            PDB.field(col.ColumnName, fieldValue);
                        else if (fieldValue.Equals(" "))
                            PDB.field(col.ColumnName, fieldValue);
                    }
                    PDB.formatEnd("Detail");
                    PDB.transmit(null);
                }
            }
            catch (Exception ex)
            {
                string strerr = ex.Message;
                return false;
            }
            return true;
        }

        public async Task<OperationResult> PrepareMQInputDataTable(DataTable dataTable, string sourceTable, string destinationTable, string userId)
        {
            var result = new OperationResult() { OperationStatus = true, OperationStatusMessage = "MQ Process Successful!" }; 
            try
            {
                //NOTE: object <DataTable> needed in MQ upload, 
                //For dynamic fields identfying, just have the list of to be ignored fields and exclude them

                var config = ConfigFactory.GetMqConfiguration(); 

                string primaryKeyIdLabel = FormatDataName(sourceTable);
                string[] excludedFields = { primaryKeyIdLabel, "UPDATEDBY", "UPDATEDTS", "STOREDBY", "STORETS" };
                foreach (var field in excludedFields)
                {
                    if (dataTable.Columns.Contains(field))
                    {
                        dataTable.Columns.Remove(field);
                    }
                }

                // Sort by 'amethystJob' then 'analysis'
                if (dataTable.Columns.Contains("amethystJob") && dataTable.Columns.Contains("analysis"))
                {
                    DataView dv = dataTable.DefaultView;
                    dv.Sort = "amethystJob ASC, analysis ASC";
                    dataTable = dv.ToTable();
                }

                var uploaded = await MQUploadPreparationParameter(dataTable, config, destinationTable, userId);
                if (!uploaded)
                {
                    result.OperationStatus = false;
                    result.OperationStatusMessage = "Error uploading on MQ: " + destinationTable;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.OperationStatus = false;
                result.OperationStatusMessage = $"Error: {ex.Message}";

                // Optional: Log the full exception (ex) to your logging framework
            }
            return result;
        }

        public async Task<OperationResult> SetApprovalStagingWImage(string sourceTable, string table, string amethystJob, string analysis, int analysisTrial, string analyzedBy, string status, string tableContent, string userId)
        {
            var result = new OperationResult() { OperationStatus=true, OperationStatusMessage="Set Approval Successful!"};

            //CODE CLEANUP------------------------------------------------------------------------------------------------------------------------------------------------------------------
            ////////////////////////////try
            ////////////////////////////{
            ////////////////////////////    //--lines with (--) on the start, disable/bypass for now to give way to new logic QA
            ////////////////////////////    var resultSetApproval = await SetApprovalAsync(table, amethystJob, analysis, analysisTrial, status, analyzedBy, null, userId);

            ////////////////////////////    if (status == "REJECTED")
            ////////////////////////////    {
            ////////////////////////////        //--var resultEmailSendRejectionEmail = _emailService.SendRejectionEmailAsync(analyzedBy, amethystJob, analysis, analysisTrial, status, userId).Result;
            ////////////////////////////    }
            ////////////////////////////    else
            ////////////////////////////    {
            ////////////////////////////        //Note: MQ input execution---------------------------------------------------------------------------------------------------------
            ////////////////////////////        DataTable dataTableMain = await GetByJobAndAnalysisDataTableAsync(sourceTable, amethystJob, analysis, analysisTrial);
            ////////////////////////////        OperationResult MainTableMQInput = await PrepareMQInputDataTable(dataTableMain, sourceTable, table, userId); 
            ////////////////////////////        DataTable dataTableDefect = await GetDataStagingDefectDataTableAsync("DATA_STAGING_DEFECT", amethystJob, analysis, analysisTrial, null, null);
            ////////////////////////////        OperationResult DefectTableMQInput = await PrepareMQInputDataTable(dataTableDefect, "DATA_STAGING_DEFECT", "DEFECTIDM", userId);
            ////////////////////////////        //---------------------------------------------------------------------------------------------------------------------------------

            ////////////////////////////        OperationResult executeBDPRequirementsProcess = await ExecuteBDPRequirementsProcess(sourceTable, amethystJob, analysis, analysisTrial);

            ////////////////////////////        var customer = await GetCustomerAsync(amethystJob, analysis, analysisTrial);
            ////////////////////////////        //--var resultEmailSendApprovalEmail = _emailService.SendApprovalEmailAsync(analyzedBy, amethystJob, analysis, analysisTrial, status, userId, customer).Result;
            ////////////////////////////    }
            ////////////////////////////}
            ////////////////////////////catch (Exception ex)
            ////////////////////////////{
            ////////////////////////////    result.OperationStatus = false;
            ////////////////////////////    result.OperationStatusMessage = ex.Message;
            ////////////////////////////    return result;
            ////////////////////////////}  
            ////////////////////////////return result;

            bool setItemStatus = true;
            setItemStatus = await SetApprovalAsync(table, amethystJob, analysis, analysisTrial, status, analyzedBy, null, userId);
            if (setItemStatus==false) { return result = new OperationResult() { OperationStatus = false, OperationStatusMessage = "Upating of the item status was unsuccessful" }; }

            //Note: MQ input execution---------------------------------------------------------------------------------------------------------
            //NOTE:DISABLE FOR AWHILE UNTIL THE MQSENDING LEVEL ERROR WAS RESOLVED
            //sample tables: DATA_STAGING_2KX, DATA_STAGING_100KX
            DataTable dataTableMain = await GetByJobAndAnalysisDataTableAsync(sourceTable, amethystJob, analysis, analysisTrial);
            OperationResult MainTableMQInput = await PrepareMQInputDataTable(dataTableMain, sourceTable, table, userId);
            if (MainTableMQInput.OperationStatus == false) { return MainTableMQInput; }

            //sample tables: DATA_STAGING_DEFECT
            DataTable dataTableDefect = await GetDataStagingDefectDataTableAsync("DATA_STAGING_DEFECT", amethystJob, analysis, analysisTrial, null, null);
            OperationResult DefectTableMQInput = await PrepareMQInputDataTable(dataTableDefect, "DATA_STAGING_DEFECT", "DEFECTIDM", userId);
            if (DefectTableMQInput.OperationStatus == false) { return DefectTableMQInput; }
            //---------------------------------------------------------------------------------------------------------------------------------

            //Note: File transfer and DB updating operations
            OperationResult executeBDPRequirementsProcess = await ExecuteBDPRequirementsProcess(sourceTable, amethystJob, analysis, analysisTrial);
            if (executeBDPRequirementsProcess.OperationStatus == false) { return executeBDPRequirementsProcess; }

            //no errors on executed process above
            return result;
        }

        public async Task<OperationResult> ExecuteBDPRequirementsProcess(string sourceTable, string amethystJob, string analysis, int analysisTrial)
        {
            var result = new OperationResult();

            try
            {
                
                /* ----------------------------------------------------------
                   1. formulate and create the BDP directory and url for the 
                        DATA_STAGING_2KX
                            - BDPImagePath
                            - BDPAnnotationPath
                        DATA_STAGING_DEFECT
                            - BDPSemImagePath
                            - BDPEdxImagePath
                -----------------------------------------------------------*/

                var staging_wImage_data = await GetByJobAndAnalysisAsync(sourceTable, amethystJob, analysis, analysisTrial);
                var staging_wImage_details_data = await GetDataStagingDefectWithDataStagingWImageParamsAsync("DATA_STAGING_DEFECT", amethystJob, analysis, analysisTrial);

                var now = DateTime.Now;
                string yearMonth = now.ToString("yyyy_MM");

                // 1. Process Main Image and Annotation
                var StagingWImageMainBDPDirectories_Image = new List<BDPUploadVariables>();
                var StagingWImageMainBDPDirectories_Annotation = new List<BDPUploadVariables>();

                // Prepare the DataName for Main tables
                string mainDataName = FormatDataName(sourceTable);
                //string mainDataIdPropertyName = "Staging2KxId";  //note: use mainDataName (eg. Staging2KxId, Staging100KxId) a dynamic Id identifying function [FormatDataName(..)]

                foreach (var item in staging_wImage_data)
                {
                    int currentId = Convert.ToInt32(item.GetProperty(mainDataName)); // Get the primary key of the row
                    // Local helper to grab values from the DTO
                    string GetVal(string key) => item.GetProperty(key)?.ToString() ?? "";

                    string _analysis = GetVal("Analysis");
                    string sampleClass = GetVal("SampleClass");
                    string sampleType = GetVal("SampleType");
                    string job = GetVal("AmethystJob");
                    string seq = GetVal("SampleSequence");

                    string commonDir = $@"\\pbt-op-onefs02.wdc.com\MSL\SEM3\BDP_Upload\{yearMonth}\{_analysis}\{sampleClass}\{sampleType}\{job}\{seq}\";
                    string commonFileName = $"{GetVal("Product")}-{GetVal("WaferId")}-{GetVal("SliderSN")}-{GetVal("Area")}-{GetVal("SubArea")}-{GetVal("ToolType")}-{GetVal("Tool")}.tif";
                    string urlBase = $"https://hdd-bdp-imgprxy.edge.wdc.com/2PNG/mho%2Fpho%2Fimages%2Fmnt%2FPHO_MSL%2FBDP_Upload%2F{yearMonth}%2F{_analysis}%2F{sampleClass}%2F{sampleType}%2F{job}%2F{seq}%2F";

                    // Image Object
                    string imagePath = GetVal("Image");
                    string imageExtracted = Path.GetFileName(imagePath);
                    StagingWImageMainBDPDirectories_Image.Add(new BDPUploadVariables
                    {

                        //ObjectId = currentId,
                        //DataName = mainDataName,
                        _DataIdValuePair = new DataIdValuePair
                        {
                            IdName = mainDataName,
                            IdValue = currentId
                        },
                        _DataNameValuePair = new DataNameValuePair
                        {
                            DataName = "BDPImagePath",
                            DataValue = urlBase + imageExtracted
                        },
                        OriginFileDirectory = imagePath,
                        FileDirectory = commonDir,
                        FileName = commonFileName,
                        ExtractedFileName = imageExtracted,
                        //NewURL = urlBase + imageExtracted
                    });

                    // Annotation Object
                    string annoPath = GetVal("Annotation");
                    string annoExtracted = Path.GetFileName(annoPath);
                    StagingWImageMainBDPDirectories_Annotation.Add(new BDPUploadVariables
                    {
                        //ObjectId = currentId,
                        //DataName = mainDataName,
                        _DataIdValuePair = new DataIdValuePair
                        {
                            IdName = mainDataName,
                            IdValue = currentId
                        },
                        _DataNameValuePair = new DataNameValuePair
                        {
                            DataName = "BDPAnnotationPath",
                            DataValue = urlBase + imageExtracted
                        },
                        OriginFileDirectory = annoPath,
                        FileDirectory = commonDir,
                        FileName = commonFileName,
                        ExtractedFileName = annoExtracted,
                        //NewURL = urlBase + annoExtracted
                    });
                }

                // 2. Process Details (SEM and EDX)
                var StagingWImageDetailsBDPDirectories_Sem = new List<BDPUploadVariables>();
                var StagingWImageDetailsBDPDirectories_Edx = new List<BDPUploadVariables>();
                string detailsDataIdPropertyName = "StagingDefectId";
                foreach (var detail in staging_wImage_details_data)
                {
                    // Access the primary key using the specific column name "StagingDefectId"
                    int detailId = Convert.ToInt32(detail.GetProperty(detailsDataIdPropertyName));
                    string GetDet(string key) => detail.GetProperty(key)?.ToString() ?? "";

                    // Link to main data to get SampleClass, Product, ToolType, etc.
                    var parent = staging_wImage_data.FirstOrDefault(x =>
                        x.GetProperty("AmethystJob")?.ToString() == GetDet("AmethystJob") &&
                        x.GetProperty("SampleSequence")?.ToString() == GetDet("SampleSequence"));

                    string GetPar(string key) => parent?.GetProperty(key)?.ToString() ?? "";

                    string detailDir = $@"\\pbt-op-onefs02.wdc.com\MSL\SEM3\BDP_Upload\{yearMonth}\{GetDet("Analysis")}\{GetPar("SampleClass")}\{GetPar("SampleType")}\{GetDet("AmethystJob")}\{GetDet("SampleSequence")}\image_details\";

                    // Construct FileName using mix of Detail and Parent properties
                    string detailFileName = $"{GetPar("Product")}-{GetPar("WaferId")}-{GetPar("SliderSN")}-{GetDet("Area")}-{GetDet("SubArea")}-{GetDet("Defect")}-{GetDet("DefectGroup")}-Reference_Filename-Image_Type-{GetPar("ToolType")}-{GetPar("Tool")}.tif";

                    string urlBase = $"https://hdd-bdp-imgprxy.edge.wdc.com/2PNG/mho%2Fpho%2Fimages%2Fmnt%2FPHO_MSL%2FBDP_Upload%2F{yearMonth}%2F{GetDet("Analysis")}%2F{GetPar("SampleClass")}%2F{GetPar("SampleType")}%2F{GetDet("AmethystJob")}%2F{GetDet("SampleSequence")}%2F";

                    // SEM
                    string semPath = GetDet("SemImage");
                    string semExtracted = Path.GetFileName(semPath);
                    StagingWImageDetailsBDPDirectories_Sem.Add(new BDPUploadVariables
                    {
                        //ObjectId = detailId,
                        //DataName = detailsDataIdPropertyName,
                        _DataIdValuePair = new DataIdValuePair
                        {
                            IdName = detailsDataIdPropertyName,
                            IdValue = detailId
                        },
                        _DataNameValuePair = new DataNameValuePair
                        {
                            DataName = "BDPSemImagePath",
                            DataValue = urlBase + semExtracted
                        },
                        OriginFileDirectory = semPath,
                        FileDirectory = detailDir,
                        FileName = detailFileName,
                        ExtractedFileName = semExtracted,
                        //NewURL = urlBase + semExtracted
                    });

                    // EDX
                    string edxPath = GetDet("EdxImage");
                    string edxExtracted = Path.GetFileName(edxPath);
                    StagingWImageDetailsBDPDirectories_Edx.Add(new BDPUploadVariables
                    {
                        //ObjectId = detailId,
                        //DataName = detailsDataIdPropertyName,
                        _DataIdValuePair = new DataIdValuePair
                        {
                            IdName = detailsDataIdPropertyName,
                            IdValue = detailId
                        },
                        _DataNameValuePair = new DataNameValuePair
                        {
                            DataName = "BDPEdxImagePath",
                            DataValue = urlBase + edxExtracted
                        },
                        OriginFileDirectory = edxPath,
                        FileDirectory = detailDir,
                        FileName = detailFileName,
                        ExtractedFileName = edxExtracted,
                        //NewURL = urlBase + edxExtracted
                    });
                }

                /// Copy the image files subject for BDP Upload
                OperationResult ProcessBDPUpload_DataStagingMain_Image = await ProcessBDPUpload(StagingWImageMainBDPDirectories_Image);
                OperationResult ProcessBDPUpload_DataStagingMain_Annotation = await ProcessBDPUpload(StagingWImageMainBDPDirectories_Annotation);
                OperationResult ProcessBDPUpload_DataStagingDefect_Sem = await ProcessBDPUpload(StagingWImageDetailsBDPDirectories_Sem);
                OperationResult ProcessBDPUpload_DataStagingDefect_Edx = await ProcessBDPUpload(StagingWImageDetailsBDPDirectories_Edx);

                /*---------------------------------------------------------------------------------------               
                3. formulate http url and use that value to update the related BDP urls in database for:
                    DATA_STAGING_2KX
                        - BDPImagePath
                        - BDPAnnotationPath
                    DATA_STAGING_DEFECT
                        - BDPSemImagePath
                         - BDPEdxImagePath
                ----------------------------------------------------------------------------------------*/

                //update the related DATA_STAGING database (e.g. DATA_STAGING_2KX, DATA_STAGING_DEFECT) 
                OperationResult UpdateDataStagingMainImage = await ProcessDataStagingDataUpdate(StagingWImageMainBDPDirectories_Image, sourceTable);
                OperationResult UpdateDataStagingMainAnnotation = await ProcessDataStagingDataUpdate(StagingWImageMainBDPDirectories_Annotation, sourceTable);
                OperationResult UpdateDataStagingDefectSem = await ProcessDataStagingDataUpdate(StagingWImageDetailsBDPDirectories_Sem, "DATA_STAGING_DEFECT");
                OperationResult UpdateDataStagingDefectEdx = await ProcessDataStagingDataUpdate(StagingWImageDetailsBDPDirectories_Edx, "DATA_STAGING_DEFECT");

                result.OperationStatus = true;
                result.OperationStatusMessage = "Success";
            }
            catch (Exception ex)
            {
                result.OperationStatus = false;
                result.OperationStatusMessage = $"Error: {ex.Message}";

                // Optional: Log the full exception (ex) to your logging framework
            }
            return result;
        }

        /// <summary>
        /// Will do the transferring of files to BDPUpload directory
        /// </summary>
        /// <param name="uploadList"></param>
        private async Task<OperationResult> ProcessBDPUpload(List<BDPUploadVariables> uploadList)
        {
            OperationResult result = new OperationResult();

            foreach (var item in uploadList)
            {
                try
                {
                    // Use System.IO.Directory to avoid controller naming conflicts
                    if (!System.IO.Directory.Exists(item.FileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(item.FileDirectory);
                    }

                    string destinationPath = System.IO.Path.Combine(item.FileDirectory, item.ExtractedFileName);

                    // Use System.IO.File to specifically target the IO class
                    if (System.IO.File.Exists(item.OriginFileDirectory))
                    {
                        System.IO.File.Copy(item.OriginFileDirectory, destinationPath, true);
                    }

                    result.OperationStatus = true;
                }
                catch (Exception ex)
                {
                    result.OperationStatus = false;
                    result.OperationStatusMessage = ex.Message;
                    // Log error (e.g., using your local logging service)
                    Console.WriteLine($"IO Error: {ex.Message}");
                }
            }

            return result;
        }

        /// <summary>
        /// will update the related DATA_STAGING database (e.g. DATA_STAGING_2KX, DATA_STAGING_DEFECT)
        /// </summary>
        /// <param name="uploadList"></param>
        private async Task<OperationResult> ProcessDataStagingDataUpdate(List<BDPUploadVariables> uploadList, string Table)
        {
            OperationResult operationResult = new OperationResult();
            foreach (var item in uploadList)
            {
                try
                {
                    operationResult.OperationStatus = await UpdateStagingDataValueAsync(Table, item._DataIdValuePair, item._DataNameValuePair);
                }
                catch (Exception ex)
                {
                    operationResult.OperationStatus = false;
                    operationResult.OperationStatusMessage = ex.Message;
                }

            }

            return operationResult;
        }

        private string FormatDataName(string tableName)
        {
            // Example: DATA_STAGING_2KX -> STAGING_2KX
            var parts = tableName.Split('_').Skip(1).ToArray();

            var formatted = string.Join("", parts.Select(p =>
            {
                if (string.IsNullOrEmpty(p)) return p;

                // Find the index of the first letter
                int firstLetterIndex = p.Select((c, i) => new { c, i })
                                        .FirstOrDefault(x => char.IsLetter(x.c))?.i ?? -1;

                // If there's no letter (it's all numbers), just return the part
                if (firstLetterIndex == -1) return p;

                // Capitalize the first letter found, and lowercase everything after it
                return p.Substring(0, firstLetterIndex) +
                       char.ToUpper(p[firstLetterIndex]) +
                       p.Substring(firstLetterIndex + 1).ToLower();
            })) + "Id";

            return formatted; // Returns "Staging2KxId"

            //--NOTE:------------------------------------------------------
            //This line is returning Staging2kxId instead of Staging2KxId
            //// Example: DATA_STAGING_2KX -> STAGING_2KX
            //var parts = tableName.Split('_').Skip(1).ToArray();  
            //// Transform parts to PascalCase and join: Staging + 2Kx + Id
            //var formatted = string.Join("", parts.Select(p =>
            //    char.ToUpper(p[0]) + p.Substring(1).ToLower())) + "Id"; 
            //return formatted; // Returns "Staging2KxId"
            //-------------------------------------------------------------
        }

    }
}
