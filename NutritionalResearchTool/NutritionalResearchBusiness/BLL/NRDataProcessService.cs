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

namespace NutritionalResearchBusiness.BLL
{
    public class NRDataProcessService : INRDataProcessService
    {
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
