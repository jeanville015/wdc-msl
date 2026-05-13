using Dapper;
using IDM.Data;
using IDM.Model.Main;
using IDM.Repository.Main.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Main.Repository
{
    public class SmallToolEntryRepository : ISmallToolEntryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SmallToolEntryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(SmallToolEntry smallToolEntry)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    var sql = @"INSERT INTO DATA_ENTRYITEMS 
                      (AmethystJob, Analysis, AnalysisTrial, ToolName, DateAnalyzed, AnalyzedBy, 
                       Customer, Status, Image,
                       StoredBy, StoreTs)
                      VALUES 
                      (@amethystJob, @analysis, @analysisTrial, @toolName, @dateAnalyzed, @analyzedBy,
                       @customer, 'PENDING', 0,
                       @storedBy, @storeTs);
                      SELECT CAST(SCOPE_IDENTITY() as int)";

                    var parameters = new
                    {
                        amethystJob = smallToolEntry.AmethystJob,
                        analysis = smallToolEntry.Analysis,
                        analysisTrial = smallToolEntry.AnalysisTrial,
                        toolName = smallToolEntry.ToolName,
                        dateAnalyzed = smallToolEntry.StoreTs,
                        analyzedBy = smallToolEntry.StoredBy,

                        customer = smallToolEntry.Customer,

                        storedBy = smallToolEntry.StoredBy,
                        storeTs = smallToolEntry.StoreTs
                    };

                    return await connection.ExecuteScalarAsync<int>(sql, parameters);
                }
                catch(Exception ex)
                { throw; }
            }
        }

        public async Task<int> GetAnalysisTrial(SmallToolEntry smallToolEntry)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return 0;
            }
        }

        public async Task<int> CreateTrial(SmallToolEntry smallToolEntry)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                int insertedId = 0;

                // Loop through each trial in the Trial list
                foreach (var trial in smallToolEntry.Trial)
                {
                    var sql = @"INSERT INTO DATA_PARAMETER_TRIALS 
                      (MATERIAL_NO, LOTNUMBER, JOB_NUMBER, TOOLID, ANALYSIS, ANALYSISTRIAL
                       PARAMETER_NAME, SITE_NAME, TRIAL_VALUE, TRIAL_COUNTER, ActiveFlag, StoreBy, StoreTs)
                      VALUES 
                      (@delivery_Date, @received_Date, @material_No, @lotNumber, @job_Number, @toolId, 
                       @parameter_Name, @site_Name, @trial_Value, @trial_Counter, 'Y', @storedBy, @storeTs);
                      SELECT CAST(SCOPE_IDENTITY() as int)";

                    // Create parameters for each trial
                    var parameters = new
                    {
                        analysis = smallToolEntry.Analysis,
                        analysisTrial = smallToolEntry.AnalysisTrial,
                        material_No = smallToolEntry.Material_No,
                        lotNumber = smallToolEntry.LotNumber,
                        job_Number = smallToolEntry.AmethystJob,
                        toolId = smallToolEntry.ToolName,
                        parameter_Name = trial.Parameter_Name,
                        site_Name = trial.Site_Name,
                        trial_Value = trial.Trial_Value,
                        trial_Counter = trial.Trial_Counter,
                        storedBy = smallToolEntry.StoredBy,
                        storeTs = smallToolEntry.StoreTs
                    };

                    // Insert each trial and get the ID
                    insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
                }
                return insertedId; // Return the ID of the last inserted trial
            }
        }

        public async Task<int> RetireTrial(SmallToolEntry smallToolEntry)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = @"
            UPDATE DATA_PARAMETER_TRIALS 
            SET ACTIVEFLAG = 'N'
            WHERE 
                ANALYSISTRIAL = @analysis 
                AND ANALYSIS = @analysisTrial 
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

                return await connection.ExecuteScalarAsync<int>(sql, smallToolEntry);
            }
        }
    }
}
