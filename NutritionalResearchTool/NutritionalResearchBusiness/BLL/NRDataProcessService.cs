using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.DAL;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.Extensions;
using NutritionalResearchBusiness.Enums;
using NutritionalResearchBusiness.Common;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Table;
using OfficeOpenXml.Style;

using System.Diagnostics;

namespace NutritionalResearchBusiness.BLL
{
    public class NRDataProcessService : INRDataProcessService
    {
        
        public int ExportNutritionalInvestigationRecords2Excel(InvestigationRecordQueryConditions conditions, string fileName)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                bool queryTimeFlag = (conditions.QueryStartTime == null || conditions.QueryEndTime == null) ? false : true;
                var _result = from nObj in mydb.InvestigationRecord
                              where nObj.Name.Contains(conditions.Name)
                              && nObj.QueueId.Contains(conditions.QueueId)
                              && nObj.HealthBookId.Contains(conditions.HealthBookId)
                              && nObj.State == (int)InvestigationRecordStateType.FinishedAndAudited
                              select nObj;
                if (queryTimeFlag)
                {
                    _result = _result.Where(nObj => nObj.CreationTime >= conditions.QueryStartTime && nObj.CreationTime <= conditions.QueryEndTime);
                }
                if (_result.Count() == 0)
                {
                    return 0;
                }
                List<InvestigationRecord4Export> exportRecords = new List<InvestigationRecord4Export>();
                _result = _result.OrderByDescending(nObj => nObj.CreationTime);
                foreach (var item in _result)
                {
                    foreach (var a in item.InvestigationAnswer.OrderBy(a => a.QuestionSerialNumber).ToList())
                    {
                        var cate = (from _nObj1 in mydb.FoodCategory
                                    join _nObj2 in mydb.Question on _nObj1.Id equals _nObj2.CategoryId
                                    where _nObj2.Id == a.QuestionId
                                    select _nObj1).First();
                        InvestigationRecord4Export record = new InvestigationRecord4Export()
                        {
                            AuditorName = item.AuditorName,
                            BeforeWeight = item.BeforeWeight,
                            Birthday = item.Birthday.ToString("yyyy-MM-dd"),
                            CurrentWeight = item.CurrentWeight,
                            HealthBookId = item.HealthBookId,
                            Height = item.Height,
                            InvestigatorName = item.InvestigatorName,
                            InvestionTime = item.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Name = item.Name,
                            QueueId = item.QueueId,
                            Week = item.Week,
                            FoodCategoryCode = cate.SecondCategoryCode
                        };
                        if (a.AnswerValue1.HasValue)
                        {
                            switch ((AnswerType)a.AnswerType)
                            {
                                case AnswerType.Month:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Week:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value * 4;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Day:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value * 28;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Other:
                                    if (a.QuestionType == (int)QuestionType.Optional)
                                    {
                                        if (a.AnswerValue1.Value == 1)
                                        {
                                            record.MonthlyIntakeFrequency = (int)a.AnswerValue2.GetValueOrDefault();
                                        }
                                        else
                                        {
                                            record.MonthlyIntakeFrequency = 0;
                                        }
                                    }
                                    else
                                    {
                                        switch (a.AnswerValue1.Value)
                                        {
                                            case 1:
                                                record.Remark = "偏淡";
                                                break;
                                            case 2:
                                                record.Remark = "适中";
                                                break;
                                            case 3:
                                                record.Remark = "偏咸";
                                                break;
                                            default:
                                                record.Remark = "未知";
                                                break;
                                        }
                                        record.MonthlyIntakeFrequency = 0;
                                    }
                                    record.AverageIntakePerTime = null;
                                    break;
                                default:
                                    break; ;
                            }

                        }
                        else
                        {
                            record.MonthlyIntakeFrequency = 0;
                            record.AverageIntakePerTime = null;
                        }
                        exportRecords.Add(record);
                    }
                }
                try
                {
                    string templateFileName = Directory.GetCurrentDirectory()+ @"\Resources\Templates\InputRecord.xlsx";
                    FileInfo templateFile = new FileInfo(templateFileName);
                    FileInfo file = new FileInfo(fileName);

                    var group = exportRecords.GroupBy(g => g.QueueId).ToList();
                    using (ExcelPackage ep = new ExcelPackage(templateFile))
                    {
                        //ExcelWorksheet ws = ep.Workbook.Worksheets.Add(typeof(InvestigationRecord4Export).Name);
                        ExcelWorksheet ws = ep.Workbook.Worksheets[1];
                        //ws.Cells["A1"].LoadFromCollection(exportRecords, true);
                        //ws.Cells.Style.Font.Name = "微软雅黑";
                        //ws.Cells.Style.Font.Size = 10;
                        int startRow = 2;
                        int endRow = 1;
                        for (int i = 0; i < group.Count(); i++)
                        {
                            int currentRow = i + startRow;
                            

                            ws.Cells["A" + currentRow.ToString()].Value = i + 1;
                            ws.Cells["B" + currentRow.ToString()].Value = exportRecords[i].QueueId;
                            ws.Cells["C" + currentRow.ToString()].Value = exportRecords[i].Pregnancy;
                            ws.Cells["D" + currentRow.ToString()].Value = exportRecords[i].HealthBookId;
                            ws.Cells["E" + currentRow.ToString()].Value = exportRecords[i].Name;
                            ws.Cells["F" + currentRow.ToString()].Value = exportRecords[i].Birthday;
                            ws.Cells["G" + currentRow.ToString()].Value = exportRecords[i].Week;
                            ws.Cells["H" + currentRow.ToString()].Value = 0;
                            ws.Cells["I" + currentRow.ToString()].Value = exportRecords[i].Height;
                            ws.Cells["J" + currentRow.ToString()].Value = exportRecords[i].CurrentWeight;
                            ws.Cells["K" + currentRow.ToString()].Value = exportRecords[i].BeforeWeight;
                            ws.Cells["L" + currentRow.ToString()].Value = exportRecords[i].InvestionTime;
                            ws.Cells["M" + currentRow.ToString()].Value = exportRecords[i].InvestigatorName;
                            ws.Cells["N" + currentRow.ToString()].Value = exportRecords[i].AuditorName;
                            ws.Cells["O" + currentRow.ToString()].Value = exportRecords[i].InvestigatorName;

                            List<InvestigationRecord4Export> currentRecords = group[i].ToList();
                            foreach(var u in currentRecords)
                            {
                                switch(u.FoodCategoryCode.ToLower())
                                {
                                    case "f01a":
                                        ws.Cells["P" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["Q" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f01b":
                                        ws.Cells["R" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["S" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f01c":
                                        ws.Cells["T" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["U" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f01d":
                                        ws.Cells["V" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["W" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f01e":
                                        ws.Cells["X" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["Y" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02a":
                                        ws.Cells["Z" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AA" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02b":
                                        ws.Cells["AB" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AC" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02c":
                                        ws.Cells["AD" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AE" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02d":
                                        ws.Cells["AF" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AG" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02e":
                                        ws.Cells["AH" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AI" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02f":
                                        ws.Cells["AK" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AL" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f02g":
                                        ws.Cells["AM" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AN" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f03a":
                                        ws.Cells["AO" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AP" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f03b":
                                        ws.Cells["AQ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AR" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f03c":
                                        ws.Cells["AS" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AT" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f03d":
                                        ws.Cells["AU" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AV" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f03e":
                                        ws.Cells["AW" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AX" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f04a":
                                        ws.Cells["AY" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AZ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f05a":
                                        ws.Cells["BA" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BB" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f05b":
                                        ws.Cells["BC" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BD" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06a":
                                        ws.Cells["BE" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BF" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06b":
                                        ws.Cells["BG" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BH" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06c":
                                        ws.Cells["BI" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BJ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06d":
                                        ws.Cells["BK" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BL" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06e":
                                        ws.Cells["BM" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BN" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06f":
                                        ws.Cells["BO" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BP" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06g":
                                        ws.Cells["BQ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BR" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06h":
                                        ws.Cells["BS" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BT" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06i":
                                        ws.Cells["BU" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BV" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06j":
                                        ws.Cells["BW" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BX" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06k":
                                        ws.Cells["BY" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["BZ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f06l":
                                        ws.Cells["CA" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CB" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07a":
                                        ws.Cells["CC" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CD" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07b":
                                        ws.Cells["CE" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CF" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07c":
                                        ws.Cells["CG" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CH" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07d":
                                        ws.Cells["CI" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CJ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07e":
                                        ws.Cells["CK" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CL" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07f":
                                        ws.Cells["CM" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CN" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07g":
                                        ws.Cells["CO" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CP" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07h":
                                        ws.Cells["CQ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CR" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07i":
                                        ws.Cells["CS" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CT" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07j":
                                        ws.Cells["CU" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CV" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07k":
                                        ws.Cells["CW" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CX" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07l":
                                        ws.Cells["CY" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["CZ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07m":
                                        ws.Cells["DA" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DB" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f07n":
                                        ws.Cells["DC" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DD" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f08a":
                                        ws.Cells["DE" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DF" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f08b":
                                        ws.Cells["DG" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DH" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f09a":
                                        ws.Cells["DI" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DJ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f09b":
                                        ws.Cells["DK" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DL" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f09c":
                                        ws.Cells["DM" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DN" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10a":
                                        ws.Cells["DO" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        break;
                                    case "f10b":
                                        ws.Cells["DP" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DQ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10c":
                                        ws.Cells["DR" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DS" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10d":
                                        ws.Cells["DT" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DU" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10e":
                                        ws.Cells["DV" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DW" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10f":
                                        ws.Cells["DX" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["DY" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10g":
                                        ws.Cells["DZ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EA" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10h":
                                        ws.Cells["EB" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EC" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10i":
                                        ws.Cells["ED" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EE" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10j":
                                        ws.Cells["EF" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EG" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10k":
                                        ws.Cells["EH" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EI" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f10l":
                                        ws.Cells["EJ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EK" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f11a":
                                        ws.Cells["EL" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        break;
                                    case "f11b":
                                        ws.Cells["EM" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["EN" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f12a":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["EO" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["EO" + currentRow.ToString()].Value = 2;
                                        ws.Cells["EP" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f12b":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["EQ" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["EQ" + currentRow.ToString()].Value = 2;
                                        ws.Cells["ER" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f12c":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["ES" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["ES" + currentRow.ToString()].Value = 2;
                                        ws.Cells["ET" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f12d":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["EU" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["EU" + currentRow.ToString()].Value = 2;
                                        ws.Cells["EV" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f13a":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["EW" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["EW" + currentRow.ToString()].Value = 2;
                                        ws.Cells["EX" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f13b":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["EY" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["EY" + currentRow.ToString()].Value = 2;
                                        ws.Cells["EZ" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f13c":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["FA" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["FA" + currentRow.ToString()].Value = 2;
                                        ws.Cells["FB" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f13d":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["FC" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["FC" + currentRow.ToString()].Value = 2;
                                        ws.Cells["FD" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f13e":
                                        if (u.MonthlyIntakeFrequency > 0)
                                            ws.Cells["FE" + currentRow.ToString()].Value = 1;
                                        else
                                            ws.Cells["FE" + currentRow.ToString()].Value = 2;
                                        ws.Cells["FF" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f14":
                                        ws.Cells["FG" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                    case "f15":
                                        ws.Cells["FH" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
                                        break;
                                }
                            }
                        }
                        ep.SaveAs(file);
                    }
                    return group.Count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void ExportNutritionalAnalysisReport2Excel(Guid recordId, string fileName)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId && nObj.State == (int)InvestigationRecordStateType.FinishedAndAudited).SingleOrDefault();
                if(record==null)
                {
                    throw new ArgumentException("无效记录Id");
                }

                try
                {
                    string templateFileName = Directory.GetCurrentDirectory() + @"\Resources\Templates\AnalysisReport.xlsx";
                    FileInfo templateFile = new FileInfo(templateFileName);
                    FileInfo file = new FileInfo(fileName);
                    using (ExcelPackage ep = new ExcelPackage(templateFile))
                    {
                        ExcelWorksheet ws = ep.Workbook.Worksheets[1];
                        //Head
                        ws.Cells["B2"].Value = record.Name;
                        ws.Cells["H2"].Value = record.Week;
                        ws.Cells["B3"].Value = record.Height;
                        ws.Cells["E3"].Value = record.BeforeWeight;
                        //ws.Cells["H3"].Value = record.BeforeBMI;
                        ws.Cells["B2"].Value = record.CurrentWeight;
                        //StructOfMeals
                        var myStructOfMeals = (from u in record.StructureOfMeals
                                               orderby u.StructureCode
                                               select u).ToList();
                        for(int i=0;i<12;i++)
                        {
                            ws.Cells["B" + (9 + i).ToString()].Value = myStructOfMeals[i].Intake;
                        }
                        List< NutrtiveElementIntakeStatisticsViewDto > myStatisticsView = record.NutrtiveElementIntakeStatistics.Select(nObj => new NutrtiveElementIntakeStatisticsViewDto()
                        {
                            Id = nObj.Id,
                            Intake = nObj.IntakeValue,
                            NutritiveElementId = nObj.NutritiveElementId,
                            NutritiveName = mydb.NuritiveElement.Where(n => n.Id == nObj.NutritiveElementId).Select(n => n.EnglishName ).SingleOrDefault(),
                            RecordId = nObj.RecordId,
                            RNIRatio = nObj.RNIRatio,
                            RNI_AI = nObj.RNI_AIValue,
                            Unit = mydb.NuritiveElement.Where(n => n.Id == nObj.NutritiveElementId).Select(n => n.EnglishUnit).SingleOrDefault()
                        }).ToList();

                        //NutritionIntakeStatistics
                        List<string> NutritionElements = new List<string>() {"kcal","kj","Protein","Fat","carbo","fiber","Ca","P","K","Na","Mg","Fe","Zn","Se","Cu","Mn","cholestl","rentinol","caroten","vita","vc","ve","Thiamin","Riboflav","Niacin" };
                        for(int i=0;i<25;i++)
                        {
                            var p = myStatisticsView.Where(n => n.NutritiveName == NutritionElements[i]).SingleOrDefault();
                            ws.Cells["B" + (23+i).ToString()].Value = p.Intake;
                            ws.Cells["D" + (23 + i).ToString()].Value = p.RNI_AI;
                            //ws.Cells["E" + (23 + i).ToString()].Value = p.RNIRatio;
                        }

                        ep.SaveAs(file);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }

            
        }
        public int ExportNutritionalResearchReport2Excel(InvestigationRecordQueryConditions conditions, string fileName)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                bool queryTimeFlag = (conditions.QueryStartTime == null || conditions.QueryEndTime == null) ? false : true;
                var _result = from nObj in mydb.InvestigationRecord
                              where nObj.Name.Contains(conditions.Name)
                              && nObj.QueueId.Contains(conditions.QueueId)
                              && nObj.HealthBookId.Contains(conditions.HealthBookId)
                              && nObj.State == (int)InvestigationRecordStateType.FinishedAndAudited
                              select nObj;
                if (queryTimeFlag)
                {
                    _result = _result.Where(nObj => nObj.CreationTime >= conditions.QueryStartTime && nObj.CreationTime <= conditions.QueryEndTime);
                }
                if(_result.Count() == 0)
                {
                    return 0;
                }
                List<InvestigationRecord4Export> exportRecords = new List<InvestigationRecord4Export>();
                _result = _result.OrderByDescending(nObj => nObj.CreationTime);
                foreach (var item in _result)
                {
                    foreach (var a in item.InvestigationAnswer.OrderBy(a => a.QuestionSerialNumber).ToList())
                    {
                        var cate = (from _nObj1 in mydb.FoodCategory
                                    join _nObj2 in mydb.Question on _nObj1.Id equals _nObj2.CategoryId
                                    where _nObj2.Id == a.QuestionId
                                    select _nObj1).First();
                        InvestigationRecord4Export record = new InvestigationRecord4Export()
                        {
                            AuditorName = item.AuditorName,
                            BeforeWeight = item.BeforeWeight,
                            Birthday = item.Birthday.ToString("yyyy-MM-dd"),
                            CurrentWeight = item.CurrentWeight,
                            HealthBookId = item.HealthBookId,
                            Height = item.Height,
                            InvestigatorName = item.InvestigatorName,
                            InvestionTime = item.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Name = item.Name,
                            QueueId = item.QueueId,
                            Week = item.Week,
                            FoodCategoryCode = cate.SecondCategoryCode
                        };
                        if (a.AnswerValue1.HasValue)
                        {
                            switch ((AnswerType)a.AnswerType)
                            {
                                case AnswerType.Month:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Week:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value * 4;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Day:
                                    record.MonthlyIntakeFrequency = a.AnswerValue1.Value * 28;
                                    record.AverageIntakePerTime = a.AnswerValue2;
                                    break;
                                case AnswerType.Other:
                                    if (a.QuestionType == (int)QuestionType.Optional)
                                    {
                                        if (a.AnswerValue1.Value == 1)
                                        {
                                            record.MonthlyIntakeFrequency = (int)a.AnswerValue2.GetValueOrDefault();
                                        }
                                        else
                                        {
                                            record.MonthlyIntakeFrequency = 0;
                                        }
                                    }
                                    else
                                    {
                                        switch (a.AnswerValue1.Value)
                                        {
                                            case 1:
                                                record.Remark = "偏淡";
                                                break;
                                            case 2:
                                                record.Remark = "适中";
                                                break;
                                            case 3:
                                                record.Remark = "偏咸";
                                                break;
                                            default:
                                                record.Remark = "未知";
                                                break;
                                        }
                                        record.MonthlyIntakeFrequency = 0;
                                    }
                                    record.AverageIntakePerTime = null;
                                    break;
                                default:
                                    break; ;
                            }

                        }
                        else
                        {
                            record.MonthlyIntakeFrequency = 0;
                            record.AverageIntakePerTime = null;
                        }
                        exportRecords.Add(record);
                    }
                }
                try
                {
                    FileInfo file = new FileInfo(fileName);
                    var group = exportRecords.GroupBy(g => g.QueueId).ToList();
                    using (ExcelPackage ep = new ExcelPackage(file))
                    {
                        ExcelWorksheet ws = ep.Workbook.Worksheets.Add(typeof(InvestigationRecord4Export).Name);
                        ws.Cells["A1"].LoadFromCollection(exportRecords, true);
                        ws.Cells.Style.Font.Name = "微软雅黑";
                        ws.Cells.Style.Font.Size = 10;
                        int startRow = 2;
                        int endRow = 1;
                        for (int i = 0; i < group.Count(); i++)
                        {
                            endRow = endRow + group[i].Count();
                            for (int n = 1; n <= 12; n++)
                            {
                                ws.Cells[startRow, n, endRow, n].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                ws.Cells[startRow, n, endRow, n].Merge = true;
                            }
                            startRow = endRow + 1;
                        }
                        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        for (int i = 1; i <= 16; i++)
                        {
                            ws.Column(i).AutoFit();
                        }
                        ep.Save();
                    }
                    return group.Count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void ImportIntakeRecordsExcel(string fileName, out int insertRow, out int existRow)
        {
            //分类、平均摄入量、月摄入次数
            List<List<string>> CategoryQuestionList = new List<List<string>>();
            CategoryQuestionList.Add(new List<string>() { "f01a", "P", "Q" });
            CategoryQuestionList.Add(new List<string>() { "f01b", "R", "S" });
            CategoryQuestionList.Add(new List<string>() { "f01c", "T", "U" });
            CategoryQuestionList.Add(new List<string>() { "f01d", "V", "W" });
            CategoryQuestionList.Add(new List<string>() { "f01e", "X", "Y" });
            CategoryQuestionList.Add(new List<string>() { "f02a", "Z", "AA" });
            CategoryQuestionList.Add(new List<string>() { "f02b", "AB", "AC" });
            CategoryQuestionList.Add(new List<string>() { "f02c", "AD", "AE" });
            CategoryQuestionList.Add(new List<string>() { "f02d", "AF", "AG" });
            CategoryQuestionList.Add(new List<string>() { "f02e", "AH", "AI" });
            CategoryQuestionList.Add(new List<string>() { "f02f", "AK", "AL" });
            CategoryQuestionList.Add(new List<string>() { "f02g", "AM", "AN" });
            CategoryQuestionList.Add(new List<string>() { "f03a", "AO", "AP" });
            CategoryQuestionList.Add(new List<string>() { "f03b", "AQ", "AR" });
            CategoryQuestionList.Add(new List<string>() { "f03c", "AS", "AT" });
            CategoryQuestionList.Add(new List<string>() { "f03d", "AU", "AV" });
            CategoryQuestionList.Add(new List<string>() { "f03e", "AW", "AX" });
            CategoryQuestionList.Add(new List<string>() { "f04a", "AY", "AZ" });
            CategoryQuestionList.Add(new List<string>() { "f05a", "BA", "BB" });
            CategoryQuestionList.Add(new List<string>() { "f05b", "BC", "BD" });
            CategoryQuestionList.Add(new List<string>() { "f06a", "BE", "BF" });
            CategoryQuestionList.Add(new List<string>() { "f06b", "BG", "BH" });
            CategoryQuestionList.Add(new List<string>() { "f06c", "BI", "BJ" });
            CategoryQuestionList.Add(new List<string>() { "f06d", "BK", "BL" });
            CategoryQuestionList.Add(new List<string>() { "f06e", "BM", "BN" });
            CategoryQuestionList.Add(new List<string>() { "f06f", "BO", "BP" });
            CategoryQuestionList.Add(new List<string>() { "f06g", "BQ", "BR" });
            CategoryQuestionList.Add(new List<string>() { "f06h", "BS", "BT" });
            CategoryQuestionList.Add(new List<string>() { "f06i", "BU", "BV" });
            CategoryQuestionList.Add(new List<string>() { "f06j", "BW", "BX" });
            CategoryQuestionList.Add(new List<string>() { "f06k", "BY", "BZ" });
            CategoryQuestionList.Add(new List<string>() { "f06l", "CA", "CB" });
            CategoryQuestionList.Add(new List<string>() { "f07a", "CC", "CD" });
            CategoryQuestionList.Add(new List<string>() { "f07b", "CE", "CF" });
            CategoryQuestionList.Add(new List<string>() { "f07c", "CG", "CH" });
            CategoryQuestionList.Add(new List<string>() { "f07d", "CI", "CJ" });
            CategoryQuestionList.Add(new List<string>() { "f07e", "CK", "CL" });
            CategoryQuestionList.Add(new List<string>() { "f07f", "CM", "CN" });
            CategoryQuestionList.Add(new List<string>() { "f07g", "CO", "CP" });
            CategoryQuestionList.Add(new List<string>() { "f07h", "CQ", "CR" });
            CategoryQuestionList.Add(new List<string>() { "f07i", "CS", "CT" });
            CategoryQuestionList.Add(new List<string>() { "f07j", "CU", "CV" });
            CategoryQuestionList.Add(new List<string>() { "f07k", "CW", "CX" });
            CategoryQuestionList.Add(new List<string>() { "f07l", "CY", "CZ" });
            CategoryQuestionList.Add(new List<string>() { "f07m", "DA", "DB" });
            CategoryQuestionList.Add(new List<string>() { "f07n", "DC", "DD" });
            CategoryQuestionList.Add(new List<string>() { "f08a", "DE", "DF" });
            CategoryQuestionList.Add(new List<string>() { "f08b", "DG", "DH" });
            CategoryQuestionList.Add(new List<string>() { "f09a", "DI", "DJ" });
            CategoryQuestionList.Add(new List<string>() { "f09b", "DK", "DL" });
            CategoryQuestionList.Add(new List<string>() { "f09c", "DM", "DN" });
            CategoryQuestionList.Add(new List<string>() { "f10a", "DO", "" });
            CategoryQuestionList.Add(new List<string>() { "f10b", "DP", "DQ" });
            CategoryQuestionList.Add(new List<string>() { "f10c", "DR", "DS" });
            CategoryQuestionList.Add(new List<string>() { "f10d", "DT", "DU" });
            CategoryQuestionList.Add(new List<string>() { "f10e", "DV", "DW" });
            CategoryQuestionList.Add(new List<string>() { "f10f", "DX", "DY" });
            CategoryQuestionList.Add(new List<string>() { "f10g", "DZ", "EA" });
            CategoryQuestionList.Add(new List<string>() { "f10h", "EB", "EC" });
            CategoryQuestionList.Add(new List<string>() { "f10i", "ED", "EE" });
            CategoryQuestionList.Add(new List<string>() { "f10j", "EF", "EG" });
            CategoryQuestionList.Add(new List<string>() { "f10k", "EH", "EI" });
            CategoryQuestionList.Add(new List<string>() { "f10l", "EJ", "EK" });
            CategoryQuestionList.Add(new List<string>() { "f11a", "EL", "" });
            CategoryQuestionList.Add(new List<string>() { "f11b", "EM", "EN" });
            CategoryQuestionList.Add(new List<string>() { "f12a", "", "EP" });
            CategoryQuestionList.Add(new List<string>() { "f12b", "", "ER" });
            CategoryQuestionList.Add(new List<string>() { "f12c", "", "ET" });
            CategoryQuestionList.Add(new List<string>() { "f12d", "", "EV" });
            CategoryQuestionList.Add(new List<string>() { "f13a", "", "EX" });
            CategoryQuestionList.Add(new List<string>() { "f13b", "", "EZ" });
            CategoryQuestionList.Add(new List<string>() { "f13c", "", "FB" });
            CategoryQuestionList.Add(new List<string>() { "f13d", "", "FD" });
            CategoryQuestionList.Add(new List<string>() { "f13e", "", "FF" });
            CategoryQuestionList.Add(new List<string>() { "f14", "", "FG" });
            CategoryQuestionList.Add(new List<string>() { "f15", "", "FH" });
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                FileInfo myFile = new FileInfo(fileName);
                try
                {
                    using (ExcelPackage ep = new ExcelPackage(myFile))
                    {
                        ExcelWorksheet ws = ep.Workbook.Worksheets[1];
                        insertRow = 0;
                        existRow = 0;
                        for (int i = 2; i < 65536; i++)
                        {
                            if (ws.Cells["A" + i.ToString()].Value != null && ws.Cells["A" + i.ToString()].Value.ToString() != string.Empty)
                            {
                                string queueId = ws.Cells["B" + i.ToString()].Value.ToString();
                                var p = from u in mydb.InvestigationRecord
                                        where u.QueueId == queueId
                                        select u;
                                if (p != null && p.Count() >0)
                                {
                                    existRow++;
                                    continue;
                                }
                                #region InvestigationRecord
                                InvestigationRecord newRecord = new InvestigationRecord();
                                newRecord.QueueId = queueId;
                                newRecord.Id = Guid.NewGuid();
                                newRecord.CreationTime = DateTime.Now;
                                object birthday = ws.Cells["F" + i.ToString()].Value;
                                if (birthday is double)
                                {
                                    newRecord.Birthday = DateTime.FromOADate(double.Parse(birthday.ToString()));
                                }
                                else if(birthday is DateTime)
                                {
                                    newRecord.Birthday = Convert.ToDateTime(birthday);
                                }
                                newRecord.BeforeWeight = Convert.ToDouble(ws.Cells["K" + i.ToString()].Value);
                                newRecord.CurrentWeight = Convert.ToDouble(ws.Cells["J" + i.ToString()].Value);
                                newRecord.HealthBookId = ws.Cells["D" + i.ToString()].Value.ToString();
                                newRecord.Height = Convert.ToDouble(ws.Cells["I" + i.ToString()].Value);
                                newRecord.BeforeBMI = newRecord.BeforeWeight / newRecord.Height / newRecord.Height * 10000;
                                newRecord.Name = ws.Cells["E" + i.ToString()].Value.ToString();
                                newRecord.InvestigatorName = ws.Cells["M" + i.ToString()].Value.ToString();
                                if(ws.Cells["N" + i.ToString()].Value!=null)
                                {
                                    newRecord.AuditorName = ws.Cells["N" + i.ToString()].Value.ToString();
                                }
                                else
                                {
                                    newRecord.AuditorName = string.Empty;
                                }
                                
                                newRecord.UpdatetionTime = null;
                                newRecord.Week = Convert.ToInt32(ws.Cells["G" + i.ToString()].Value);
                                
                                if (newRecord.AuditorName != string.Empty)
                                {
                                    newRecord.AuditTime = DateTime.Now;
                                    newRecord.State = (int)InvestigationRecordStateType.FinishedAndAudited;
                                }
                                else
                                {
                                    newRecord.AuditTime = null;
                                    newRecord.State = (int)InvestigationRecordStateType.FinishedAndNoAudit;
                                }
                                switch (ws.Cells["C" + i.ToString()].Value.ToString())
                                {
                                    case "A":
                                        newRecord.Stage = 1;
                                        break;
                                    case "B":
                                        newRecord.Stage = 2;
                                        break;
                                    case "C":
                                        newRecord.Stage = 3;
                                        break;
                                }
                                mydb.InvestigationRecord.Add(newRecord);
                                
                                #endregion
                                #region  InvestigationAnswer;
                                foreach(var j in CategoryQuestionList)
                                {
                                    string categoryCode = j.First();
                                    var r = from u in mydb.FoodCategory
                                            where u.SecondCategoryCode == categoryCode
                                            select u;
                                    if(r == null || r.Count() !=1)
                                    {
                                        continue;
                                    }
                                    Guid categoryId = r.First().Id;
                                    var q = from u in mydb.Question
                                            where u.CategoryId == categoryId
                                            select u;
                                    if (q == null || q.Count() != 1)
                                    {
                                        continue;
                                    }
                                    int? frequence;
                                    double? averageIntake;
                                    if(j[2] == string.Empty)
                                    { frequence = null; }
                                    else
                                    {
                                        if (j[2] != string.Empty && ws.Cells[j[2] + i.ToString()].Value != null)
                                        {
                                            frequence = Convert.ToInt32(ws.Cells[j[2] + i.ToString()].Value);
                                            Debug.WriteLine("current frequence cells is ws.Cells[" + j[2] + i.ToString() + "] = " + ws.Cells[j[2] + i.ToString()].Value.ToString() + ";");
                                        }
                                        else
                                        {
                                            frequence = null;
                                            Debug.WriteLine("current frequence cells is ws.Cells[" + j[2] + i.ToString() + "] =  \"\";");
                                        }
                                    }
                                    if (j[1] == string.Empty)
                                    {
                                        averageIntake = null;
                                    }
                                    else
                                    {
                                        if (ws.Cells[j[1] + i.ToString()].Value != null)
                                        {
                                            averageIntake = Convert.ToDouble(ws.Cells[j[1] + i.ToString()].Value);
                                            Debug.WriteLine("current value cell is ws.Cells[" + j[1] + i.ToString() + "] = " + ws.Cells[j[1] + i.ToString()].Value.ToString() + ";");
                                        }
                                        else
                                        {
                                            averageIntake = null;
                                            Debug.WriteLine("current value cell is ws.Cells[" + j[1] + i.ToString() + "] =  \"\";");
                                        }
                                    }

                                    InvestigationAnswer newAnswer = new InvestigationAnswer()
                                    {
                                        Id = Guid.NewGuid(),
                                        InvestigationRecordId = newRecord.Id,
                                        AnswerType = 0,
                                        CreationTime = DateTime.Now,
                                        QuestionId = q.First().Id,
                                        QuestionType = q.First().Type,
                                        QuestionSerialNumber = q.First().SerialNumber
                                    };

                                    newAnswer.AnswerValue1 = frequence;
                                    newAnswer.AnswerValue2 = averageIntake;
                                  
                                    newRecord.InvestigationAnswer.Add(newAnswer);
                                }
                                #endregion
                                mydb.SaveChanges();
                                insertRow++;
                                new NRMainService().GenerateImportRecordReport(newRecord, mydb);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    throw;
                }
            }
        }
        //public void CreateFoodNutritionsForTesting(List<FoodNutritionsPostDto> datas)
        //{
        //    using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
        //    {
        //        List<FoodNutritions> _foodnutritionList = datas.Select(nObj => new FoodNutritions()
        //        {
        //            Id = Guid.NewGuid(),
        //            FoodId = nObj.FoodId,
        //            NutritiveElementId = nObj.NutritiveElementId,
        //            Value = nObj.Value
        //        }).ToList();
        //        try
        //        {
        //            mydb.FoodNutritions.AddRange(_foodnutritionList);
        //            mydb.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //}

        //public List<Foods> GetFoodsList()
        //{
        //    using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
        //    {
        //        return mydb.Foods.OrderBy(f => f.CreationTime).ToList();
        //    }
        //}

        public NutritionalResearchStatisticalReportViewDto GetSomeoneRecordStatisticalReport(Guid recordId)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId && nObj.State != (int)InvestigationRecordStateType.NoFinish).SingleOrDefault();
                if (record == null)
                {
                    throw new ArgumentException("无效记录Id");
                }
                NutritionalResearchStatisticalReportViewDto report = new NutritionalResearchStatisticalReportViewDto()
                {
                    RecordId = record.Id,
                    HealthBookId = record.HealthBookId,
                    InvestigationTime = record.CreationTime,
                    Age = DateTime.Now.GetAge(record.Birthday),
                    BeforeBMI = record.BeforeBMI,
                    BeforeWeight = record.BeforeWeight,
                    CurrentWeight = record.CurrentWeight,
                    Height = record.Height,
                    FillingRecords = new List<AnswerRecords>(),
                    NutrtiveElementIntakeStatistics = new List<NutrtiveElementIntakeStatisticsViewDto>(),
                    StructureOfMeals = new List<StructureOfMealsViewDto>(),
                    Name = record.Name,
                    Week = record.Week
                };

                //获取填表记录
                //record.InvestigationAnswer.Where(a => a.AnswerType != (int)AnswerType.Other).ToList().ForEach(item =>
                record.InvestigationAnswer.ToList().ForEach(item =>
                {
                    var cate = (from nObj in mydb.FoodCategory
                                join nObj2 in mydb.Question on nObj.Id equals nObj2.CategoryId
                                where nObj2.Id == item.QuestionId
                                select nObj).First();
                    AnswerRecords answer = new AnswerRecords()
                    {
                        AnswerId = item.Id,
                        FirstCategoryCode = cate.FirstCategoryCode,
                        FirstCategoryName = cate.FirstCategoryName,
                        SecondCategoryCode = cate.SecondCategoryCode,
                        SecondCategoryName = cate.SecondCategoryName
                    };
                    if(item.AnswerValue1.HasValue)
                    {
                        switch ((AnswerType)item.AnswerType)
                        {
                            case AnswerType.Month:
                                answer.MonthlyIntakeFrequency = item.AnswerValue1.Value.ToString() + "次";
                                break;
                            case AnswerType.Week:
                                answer.MonthlyIntakeFrequency = (item.AnswerValue1.Value * 4).ToString() + "次";
                                break;
                            case AnswerType.Day:
                                answer.MonthlyIntakeFrequency = (item.AnswerValue1.Value * 28).ToString() + "次";
                                break;
                            case AnswerType.Other:
                                if(item.QuestionType == (int)QuestionType.Optional)
                                {
                                    if(item.AnswerValue1.Value == 1)
                                    {
                                        answer.MonthlyIntakeFrequency = ((int)item.AnswerValue2.GetValueOrDefault()).ToString() + "次";
                                    }
                                    else
                                    {
                                        answer.MonthlyIntakeFrequency = "0次";
                                    }
                                }
                                else
                                {
                                    switch (item.AnswerValue1.Value)
                                    {
                                        case 1:
                                            answer.MonthlyIntakeFrequency = "偏淡";
                                            break;
                                        case 2:
                                            answer.MonthlyIntakeFrequency = "适中";
                                            break;
                                        case 3:
                                            answer.MonthlyIntakeFrequency = "偏咸";
                                            break;
                                        default:
                                            answer.MonthlyIntakeFrequency = "未知";
                                            break;
                                    }
                                }
                                break;
                            default:
                                break; ;
                        }

                    }
                    else
                    {
                        answer.MonthlyIntakeFrequency = "没吃过";
                    }
                    report.FillingRecords.Add(answer);
                });

                //获取膳食构成
                report.StructureOfMeals = record.StructureOfMeals.Select(nObj => new StructureOfMealsViewDto()
                {
                    Id = nObj.Id,
                    Intake = nObj.Intake,
                    RecordId = nObj.RecordId,
                    StructureCode = nObj.StructureCode,
                    StructureDescription = nObj.StructureDescription,
                    Unit = nObj.Unit
                }).ToList();

                //获取营养元素摄入统计
                report.NutrtiveElementIntakeStatistics = record.NutrtiveElementIntakeStatistics.Select(nObj => new NutrtiveElementIntakeStatisticsViewDto()
                {
                    Id = nObj.Id,
                    Intake = nObj.IntakeValue,
                    NutritiveElementId = nObj.NutritiveElementId,
                    NutritiveName = mydb.NuritiveElement.Where(n => n.Id == nObj.NutritiveElementId).Select(n => n.ChineseName + "(" + n.EnglishName + ")").SingleOrDefault(),
                    RecordId = nObj.RecordId,
                    RNIRatio = nObj.RNIRatio,
                    RNI_AI = nObj.RNI_AIValue,
                    Unit = mydb.NuritiveElement.Where(n => n.Id == nObj.NutritiveElementId).Select(n => n.EnglishUnit).SingleOrDefault()
                }).ToList();

                return report;
            }
        }

        public void ExportNutritionalAnalysisReport2Print(Guid recordId)
        {
            //string templateFileName = Directory.GetCurrentDirectory() + @"\Resources\Templates\AnalysisReport.xlsx";
            string fileName = Directory.GetCurrentDirectory() + @"\Output\PrintAnalysisReport.xlsx";
            string filePath = Directory.GetCurrentDirectory() + @"\Output";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                ExportNutritionalAnalysisReport2Excel(recordId, fileName);
            }
            catch (Exception)
            {

                throw;
            }
            Process proc;
            try
            {
                proc = Process.Start(fileName);
            }
            catch(System.ComponentModel.Win32Exception)
            {
                throw;
            }
        }
    }
}
