using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Model.Maintenance;
using IDM.Model.User;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class IncomingDataRepository : IIncomingDataRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IncomingDataRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(IncomingData incomingData)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                int insertedId = 0;
                try
                {
                    foreach (var parameter in incomingData.Parameters)
                    {
                        var sql = @"INSERT INTO DATA_PARAMETER_DETAILS 
                      (MATERIAL_NO, MATERIAL_NAME, LOTNUMBER, AREA_NAME, SUPPLIER_NAME, MANUFACTURER_NAME, 
                       DELIVERY_DATE, RECEIVED_DATE, MANUFACTURING_DATE, EXPIRATION_DATE, INSPECTION_DATE,
                       INSPECTEDBY, ENCODEDBY, JOB_NUMBER, TOOLID, PRE_QUALIFICATION_TEST,
                       VISUAL_APPEARANCE_CHECK, PACKAGING_DOCUMENT_CHECK, PARAMETER_NAME, UOM_NAME,
                       SITE_NAME, PARAMETER_VALUE, LOWER_SPECS_LIMIT, UPPER_SPECS_LIMIT, JUDGEMENT,
                       LOWER_CONTROL_LIMIT, UPPER_CONTROL_LIMIT, CONTROL_JUDGEMENT, REMARKS, RECEIVEDBY,
                       StoreBy, StoreTs)
                      VALUES 
                      (@material_No, @material_Name, @lotNumber, @area_Name, @supplier_Name, @manufacturer_Name,
                       @delivery_Date, @received_Date, @manufacturing_Date, @expiration_Date, @inspection_Date,
                       @inspectedBy, @encodedBy, @job_Number, @toolId, @pre_Qualification_Test,
                       @visual_Appearance_Check, @packaging_Document_Check, @parameter_Name, @uom_Name,
                       @site_Name, @parameter_Value, @lower_Specs_Limit, @upper_Specs_Limit, @judgement,
                       @lower_Control_Limit, @upper_Control_Limit, @control_Judgement, @remarks, @receivedBy,
                       @storedBy, @storeTs);
                      SELECT CAST(SCOPE_IDENTITY() as int)";

                        var parameters = new
                        {
                            material_No = incomingData.Material_No,
                            material_Name = incomingData.Material_Name,
                            lotNumber = incomingData.LotNumber,
                            area_Name = incomingData.Area_Name,
                            supplier_Name = incomingData.Supplier_Name,
                            manufacturer_Name = incomingData.Manufacturer_Name,

                            delivery_Date = incomingData.Delivery_Date,
                            received_Date = incomingData.Received_Date,
                            manufacturing_Date = incomingData.Manufacturing_Date,
                            expiration_Date = incomingData.Expiration_Date,
                            inspection_Date = incomingData.Inspection_Date,

                            inspectedBy = incomingData.InspectedBy,
                            encodedBy = incomingData.EncodedBy,
                            job_Number = incomingData.Job_Number,
                            toolId = incomingData.ToolId,
                            pre_Qualification_Test = incomingData.Pre_Qualification_Test,

                            visual_Appearance_Check = incomingData.View_Appearance_Check,
                            packaging_Document_Check = incomingData.Packaging_Document_Check,
                            parameter_Name = parameter.Parameter_Name,
                            uom_Name = parameter.Uom_Name,

                            site_Name = parameter.Site_Name,
                            parameter_Value = parameter.Parameter_Value,
                            lower_Specs_Limit = parameter.Lower_Specs_Limit,
                            upper_Specs_Limit = parameter.Upper_Specs_Limit,
                            judgement = parameter.Specs_Judgement,

                            lower_Control_Limit = parameter.Lower_Control_Limit,
                            upper_Control_Limit = parameter.Upper_Control_Limit,
                            control_Judgement = parameter.Control_Judgement,
                            remarks = incomingData.Remarks,
                            receivedBy = incomingData.ReceivedBy,
                            storedBy = incomingData.StoredBy,
                            storeTs = incomingData.StoreTs
                        };

                        insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
                    }
                }catch(Exception ex)
                { string ex1 = ""; }
                return insertedId; 
            }
        }

        public async Task<int> CreateTrial(IncomingData incomingData)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                int insertedId = 0;

                // Loop through each trial in the Trial list
                foreach (var trial in incomingData.Trial)
                {
                    var sql = @"INSERT INTO DATA_PARAMETER_TRIALS 
                      (DELIVERYDATE, RECEIVEDDATE, MATERIAL_NO, LOTNUMBER, JOB_NUMBER, TOOLID, 
                       PARAMETER_NAME, SITE_NAME, TRIAL_VALUE, TRIAL_COUNTER, ActiveFlag, StoreBy, StoreTs)
                      VALUES 
                      (@delivery_Date, @received_Date, @material_No, @lotNumber, @job_Number, @toolId, 
                       @parameter_Name, @site_Name, @trial_Value, @trial_Counter, 'Y', @storedBy, @storeTs);
                      SELECT CAST(SCOPE_IDENTITY() as int)";

                    // Create parameters for each trial
                    var parameters = new
                    {
                        delivery_Date = incomingData.Delivery_Date,
                        received_Date = incomingData.Received_Date,
                        material_No = incomingData.Material_No,
                        lotNumber = incomingData.LotNumber,
                        job_Number = incomingData.Job_Number,
                        toolId = incomingData.ToolId,
                        parameter_Name = trial.Parameter_Name,
                        site_Name = trial.Site_Name,
                        trial_Value = trial.Trial_Value,
                        trial_Counter = trial.Trial_Counter,
                        storedBy = incomingData.StoredBy,
                        storeTs = incomingData.StoreTs
                    };

                    // Insert each trial and get the ID
                    insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
                }
                return insertedId; // Return the ID of the last inserted trial
            }
        }

        public async Task<int> RetireTrial(IncomingData incomingData)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"
            UPDATE DATA_PARAMETER_TRIALS 
            SET ACTIVEFLAG = 'N'
            WHERE 
                DELIVERYDATE = @delivery_Date 
                AND RECEIVEDDATE = @received_Date 
                AND MATERIAL_NO = @material_No 
                AND 
                (
                    LOTNUMBER = @lotNumber 
                    OR (@lotNumber IS NULL AND LOTNUMBER IS NULL) 
                    OR (@lotNumber = '' AND LOTNUMBER = '')
                )
                AND 
                (
                    JOB_NUMBER = @job_Number 
                    OR (@job_Number IS NULL AND JOB_NUMBER IS NULL)
                    OR (@job_Number = '' AND JOB_NUMBER = '')
                )
                AND 
                (
                    TOOLID = @toolId 
                    OR (@toolId IS NULL AND TOOLID IS NULL)
                    OR (@toolId = '' AND TOOLID = '')
                );
        ";

                return await connection.ExecuteScalarAsync<int>(sql, incomingData);
            }
        }

        public async Task<IEnumerable<ParameterTrial>> GetTrialAsync(IncomingData incomingData)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT * FROM DATA_PARAMETER_TRIALS  
                            WHERE 
                                DELIVERYDATE = @delivery_Date 
                                AND ACTIVEFLAG = 'Y' 
                                AND RECEIVEDDATE = @received_Date 
                                AND MATERIAL_NO = @material_No 
                                AND 
                                (
                                    LOTNUMBER = @lotNumber 
                                    OR (@lotNumber IS NULL AND LOTNUMBER IS NULL) 
                                    OR (@lotNumber = '' AND LOTNUMBER = '')
                                )
                                AND 
                                (
                                    JOB_NUMBER = @job_Number 
                                    OR (@job_Number IS NULL AND JOB_NUMBER IS NULL)
                                    OR (@job_Number = '' AND JOB_NUMBER = '')
                                )
                                AND 
                                (
                                    TOOLID = @toolId 
                                    OR (@toolId IS NULL AND TOOLID IS NULL)
                                    OR (@toolId = '' AND TOOLID = '')
                                ); ";
                return await connection.QueryAsync<ParameterTrial>(sql, incomingData);
            }
        }

    }
}
