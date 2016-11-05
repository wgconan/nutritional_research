﻿using System;
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
                                        ws.Cells["AJ" + currentRow.ToString()].Value = u.AverageIntakePerTime;
                                        ws.Cells["AK" + currentRow.ToString()].Value = u.MonthlyIntakeFrequency;
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
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId && nObj.State != (int)InvestigationRecordStateType.FinishedAndAudited).SingleOrDefault();
                if(record==null)
                {
                    throw new ArgumentException("无效记录Id");
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
    }
}
