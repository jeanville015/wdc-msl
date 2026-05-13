using IDM.Model;
using IDM.Model.Common;
using IDM.Web.DataAccessModel;
using IDM.Web.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Services.Description;
using WDHelpers.EDCSPCHelper;

namespace IDM.Web.DataAccess
{
    public class EDCSPC
    {
        private readonly Message _log = new Message();

        private static bool IsParameter(string columnName) =>
            columnName.Contains("WDC") || columnName.Contains("SUPPLIER");

        public SubmitResult Submit(DataTable dataTable, EDCSPCDataSending config)
        {
            _log.Log("Entering Submit");

            for (int i = 0; i < config.ChartName.Count; i++)
            {
                var result = SubmitSingle(dataTable, config, i);
                if (!result.Success)
                    return result;
            }

            _log.Log("Submit complete");
            return SubmitResult.Ok();
        }

        private SubmitResult SubmitSingle(DataTable dataTable, EDCSPCDataSending config, int index)
        {
            try
            {
                var parameters = BuildParameters(dataTable);
                var tags = BuildTags(dataTable);

                var dataModel = new DataModel
                {
                    Data = dataTable,
                    TestDate = DateTime.Now,
                    OperatorID = config.Operator,
                    WaferID = config.WaferLot,
                    LotID = config.WaferLot,
                    ToolEntity = config.SourceEntity[index],
                    Parameters = parameters,
                    Tags = tags,
                    Product = config.Product
                };

                return SubmitToEdcspc(dataModel);
            }
            catch (Exception ex)
            {
                _log.Log(ex.Message);
                return SubmitResult.Fail(ex.Message);
            }
        }

        private static List<Parameter> BuildParameters(DataTable dataTable)
        {
            var result = new List<Parameter>();

            foreach (DataColumn col in dataTable.Columns)
            {
                if (!IsParameter(col.ColumnName)) continue;

                var param = new Parameter(col.ColumnName);
                foreach (DataRow row in dataTable.Rows)
                    param.Samples.Add(double.TryParse(row[col].ToString(), out double v) ? v : 0);

                result.Add(param);
            }

            return result;
        }

        private static List<Tag> BuildTags(DataTable dataTable)
        {
            var result = new List<Tag>();
            var lastRow = dataTable.Rows[dataTable.Rows.Count - 1];
            var firstRow = dataTable.Rows[0];

            foreach (DataColumn col in dataTable.Columns)
            {
                if (IsParameter(col.ColumnName)) continue;

                string value;
                if (col.ColumnName == "Date_Tested")
                    value = lastRow[col].ToString();
                else if (col.ColumnName == "ABS_BATCH_NO")
                    value = Regex.Replace(firstRow[col].ToString(), "[^a-zA-Z0-9_.-]+", "", RegexOptions.Compiled);
                else
                    value = firstRow[col].ToString();

                result.Add(new Tag(col.ColumnName, value));
            }

            return result;
        }

        private SubmitResult SubmitToEdcspc(DataModel dataModel)
        {
            var sub = new WDHelpers.EDCSPCHelper.EDCSubmission(
                dataModel.WaferID, dataModel.ToolEntity, dataModel.TestDate);

            if (sub.ParameterNames.Count == 0)
                return SubmitResult.Fail("No parameters found for entity: " + dataModel.ToolEntity);

            foreach (var p in dataModel.Parameters)
                if (sub.IsParameterDefined(p.Name))
                    sub.SetDataForParameter(p.Name, p.Samples);

            foreach (var t in dataModel.Tags)
                if (sub.IsTagDefined(t.Name))
                    sub.SetTag(t.Name, t.Value, t.TargetParameter, t.TargetSample);

            if (!sub.Submit(dataModel.OperatorID, dataModel.LotID, dataModel.Product))
            {
                var errors = string.Join("\n", sub.SubmissionErrors);
                _log.Log("EDCSPC submission failed:\n" + errors);
                return SubmitResult.Fail(errors);
            }

            _log.Log("EDCSPC submission succeeded");

            if (sub.SpecViolations.Count > 0)
            {
                var violationLines = new List<string>();
                foreach (var v in sub.SpecViolations)
                {
                    if (v != null)
                        violationLines.Add(string.Format("Param[{0}] Rule[{1}] BadSamples[{2}] OCAP[{3}]",
                            v.ParamName, v.Rule, v.BadSamplesCount, v.OCAP));
                }
                var warning = string.Join("\n", violationLines);
                _log.Log(warning);
                return SubmitResult.OkWithWarning(warning);
            }

            return SubmitResult.Ok();
        }
    }
}