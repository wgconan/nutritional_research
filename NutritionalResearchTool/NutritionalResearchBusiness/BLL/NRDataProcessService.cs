using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.DAL;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.Extensions;
using NutritionalResearchBusiness.Enums;

namespace NutritionalResearchBusiness.BLL
{
    public class NRDataProcessService : INRDataProcessService
    {
        public void CreateFoodNutritionsForTesting(List<FoodNutritionsPostDto> datas)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                List<FoodNutritions> _foodnutritionList = datas.Select(nObj => new FoodNutritions()
                {
                    Id = Guid.NewGuid(),
                    FoodId = nObj.FoodId,
                    NutritiveElementId = nObj.NutritiveElementId,
                    Value = nObj.Value
                }).ToList();
                try
                {
                    mydb.FoodNutritions.AddRange(_foodnutritionList);
                    mydb.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<Foods> GetFoodsList()
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                return mydb.Foods.OrderBy(f => f.CreationTime).ToList();
            }
        }

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
                record.InvestigationAnswer.Where(a => a.AnswerType != (int)AnswerType.Other).ToList().ForEach(item =>
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
                        SecondCategoryName = cate.StatisticsCategoryName
                    };
                    if(item.AnswerValue1.HasValue)
                    {
                        switch ((AnswerType)item.AnswerType)
                        {
                            case AnswerType.Month:
                                answer.MonthlyIntakeFrequency = item.AnswerValue1.Value;
                                break;
                            case AnswerType.Week:
                                answer.MonthlyIntakeFrequency = item.AnswerValue1.Value * 4;
                                break;
                            case AnswerType.Day:
                                answer.MonthlyIntakeFrequency = item.AnswerValue1.Value * 28;
                                break;
                            case AnswerType.Other:
                            default:
                                break; ;
                        }
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
