using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.DAL;
using NutritionalResearchBusiness.Enums;
using NutritionalResearchBusiness.Common;

namespace NutritionalResearchBusiness.BLL
{
    public class NRMainService : INRMainService
    {
        public void AddOrUpdateSomeoneInvestigationAnswer(InvestigationAnswerInputDto answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException("答案参数不能为空!");
            }
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == answer.InvestigationRecordId).SingleOrDefault();
                if(record == null)
                {
                    throw new ArgumentException("无效的调查记录!");
                }
                if(record.State == (int)InvestigationRecordStateType.FinishedAndAudited)
                {
                    throw new InvalidOperationException("不能修改已审核的调查记录答案!");
                }
                var _answer = (from nObj in mydb.InvestigationAnswer
                               where nObj.InvestigationRecordId == answer.InvestigationRecordId
                               && nObj.QuestionId == answer.QuestionId
                               select nObj).SingleOrDefault();
                if (_answer == null)
                {
                    InvestigationAnswer _newAnswer = new InvestigationAnswer()
                    {
                        AnswerType = (int)answer.Answer_Type,
                        AnswerValue1 = answer.AnswerValue1,
                        AnswerValue2 = answer.AnswerValue2,
                        CreationTime = DateTime.Now,
                        Id = Guid.NewGuid(),
                        InvestigationRecordId = answer.InvestigationRecordId,
                        QuestionId = answer.QuestionId,
                        QuestionSerialNumber = answer.QuestionSerialNumber,
                        QuestionType = (int)answer.Question_Type
                    };
                    mydb.InvestigationAnswer.Add(_newAnswer);
                }
                else
                {
                    _answer.AnswerType = (int)answer.Answer_Type;
                    _answer.AnswerValue1 = answer.AnswerValue1;
                    _answer.AnswerValue2 = answer.AnswerValue2;
                    _answer.UpdationTime = DateTime.Now;
                }
                try
                {
                    mydb.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void AuditSomeoneInvestigationRecord(Guid recordId, string AuditorName)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId).SingleOrDefault();
                if (record == null)
                {
                    throw new ArgumentException("无效记录Id");
                }
                if ((InvestigationRecordStateType)record.State != InvestigationRecordStateType.FinishedAndNoAudit)
                {
                    throw new InvalidOperationException("该记录状态无效，不能完成审核");
                }
                record.AuditorName = AuditorName;
                record.AuditTime = DateTime.Now;
                record.State = (int)InvestigationRecordStateType.FinishedAndAudited;
                try
                {
                    mydb.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Guid CreateNewInvestigationRecord(NewInvestigationRecordDto newrecord)
        {
            if (newrecord == null)
            {
                throw new ArgumentNullException("记录参数不能为空!");
            }
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var queneId = mydb.InvestigationRecord.Where(nObj => nObj.QueueId == newrecord.QueueId).FirstOrDefault();
                if (queneId != null)
                {
                    throw new ArgumentException("输入的队列编码重复");
                }
                int _stage = 1;
                if (newrecord.Week < 13)
                {
                    _stage = 1;
                }
                else if (newrecord.Week >= 13 && newrecord.Week <= 28)
                {
                    _stage = 2;
                }
                else
                {
                    _stage = 3;
                }
                InvestigationRecord record = new InvestigationRecord()
                {
                    Id = Guid.NewGuid(),
                    BeforeWeight = newrecord.BeforeWeight,
                    BeforeBMI = 10000 * newrecord.BeforeWeight / (newrecord.Height * newrecord.Height),
                    Birthday = newrecord.Birthday,
                    CreationTime = DateTime.Now,
                    CurrentWeight = newrecord.CurrentWeight,
                    HealthBookId = newrecord.HealthBookId,
                    Height = newrecord.Height,
                    InvestigatorName = newrecord.InvestigatorName,
                    Name = newrecord.Name,
                    QueueId = newrecord.QueueId,
                    Stage = _stage,
                    State = (int)InvestigationRecordStateType.NoFinish,
                    Week = newrecord.Week
                };
                mydb.InvestigationRecord.Add(record);
                try
                {
                    mydb.SaveChanges();
                    return record.Id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void FinishSomeoneInvestigationRecord(Guid recordId)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId).SingleOrDefault();
                if (record == null)
                {
                    throw new ArgumentException("无效记录Id");
                }
                if ((InvestigationRecordStateType)record.State != InvestigationRecordStateType.NoFinish)
                {
                    throw new InvalidOperationException("该记录已完成，不能重复操作");
                }
                //由于初始数据还未导入完整，现为方便调试，暂时屏蔽完成记录就生成报告的逻辑
                //GenerateOrUpdateReport(record, mydb);
                record.State = (int)InvestigationRecordStateType.FinishedAndNoAudit;
                try
                {
                    mydb.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int GetFinishedInvestigationRecordCount()
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                return mydb.InvestigationRecord.Where(nObj => nObj.State != (int)InvestigationRecordStateType.NoFinish).Count();
            }
        }

        public QuestionViewDto GetQuestionViewBySerialNumber(int serialNumber)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var questionCount = mydb.Question.Count();
                var result = (from nObj in mydb.Question
                              join nObj2 in mydb.FoodCategory on nObj.CategoryId equals nObj2.Id
                              where nObj.SerialNumber == serialNumber
                              select new QuestionViewDto()
                              {
                                  Id = nObj.Id,
                                  CurrentProgress = Math.Round((double)serialNumber / (double)questionCount, 3),
                                  //CurrentProgress = serialNumber / questionCount,
                                  Description = nObj2.Description,
                                  FirstCategoryCode = nObj2.FirstCategoryCode,
                                  FirstCategoryName = nObj2.FirstCategoryName,
                                  SecondCategoryCode = nObj2.SecondCategoryCode,
                                  SecondCategoryName = nObj2.SecondCategoryName,
                                  FoodUnit = nObj2.FoodUnit,
                                  ReferenceDiagramList = nObj.ReferenceDiagram
                                  .OrderBy(r => r.Sort)
                                  .Select(r => new ReferenceDiagramDto()
                                  {
                                      Id = r.Id,
                                      IconPath = r.IconPath,
                                      Sort = r.Sort,
                                      Value = r.Value
                                  }).ToList(),
                                  SerialNumber = nObj.SerialNumber,
                                  Type = (QuestionType)nObj.Type
                              }).SingleOrDefault();
                if (result == null)
                {
                    throw new ArgumentException("无效序号");
                }
                return result;
            }
        }

        public PageQueryOutput<InvestigationRecordViewDto> QueryInvestigationRecordListByConditions(PageQueryInput<InvestigationRecordQueryConditions> conditions)
        {
            PageQueryOutput<InvestigationRecordViewDto> result = new PageQueryOutput<InvestigationRecordViewDto>();
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                bool queryTimeFlag = (conditions.QueryConditions.QueryStartTime == null || conditions.QueryConditions.QueryEndTime == null) ? false : true;
                bool pageFlag = (conditions.PageIndex != null && conditions.PageSize != null) ? true : false;
                var _result = from nObj in mydb.InvestigationRecord
                              where nObj.Name.Contains(conditions.QueryConditions.Name)
                              && nObj.QueueId.Contains(conditions.QueryConditions.QueueId)
                              && nObj.HealthBookId.Contains(conditions.QueryConditions.HealthBookId)
                              select nObj;
                if (queryTimeFlag)
                {
                    _result = _result.Where(nObj => nObj.CreationTime >= conditions.QueryConditions.QueryStartTime && nObj.CreationTime <= conditions.QueryConditions.QueryEndTime);
                }
                result.TotalCount = _result.Count();
                if(pageFlag)
                {
                    result.PageIndex = conditions.PageIndex.Value;
                    result.PageCount = PageComputer.ComputePageCount(result.TotalCount, conditions.PageSize.Value);
                    result.QueryResult = _result.OrderByDescending(nObj => nObj.CreationTime)
                        .Select(nObj => new InvestigationRecordViewDto()
                        {
                            AuditorName = nObj.AuditorName,
                            BeforeBMI = nObj.BeforeBMI,
                            BeforeWeight = nObj.BeforeWeight,
                            Birthday = nObj.Birthday,
                            CurrentWeight = nObj.CurrentWeight,
                            HealthBookId = nObj.HealthBookId,
                            Height = nObj.Height,
                            Id = nObj.Id,
                            InvestigatorName = nObj.InvestigatorName,
                            InvestionTime = nObj.CreationTime,
                            LastFinishQuestionSN = nObj.InvestigationAnswer.OrderByDescending(a => a.QuestionSerialNumber).Select(a => a.QuestionSerialNumber).FirstOrDefault(),
                            Name = nObj.Name,
                            QueueId = nObj.QueueId,
                            State = (InvestigationRecordStateType)nObj.State,
                            Week = nObj.Week
                        })
                        .Skip(conditions.PageSize.Value * (conditions.PageIndex.Value - 1))
                        .Take(conditions.PageSize.Value)
                        .ToList();
                }
                else
                {
                    result.QueryResult = _result.OrderByDescending(nObj => nObj.CreationTime)
                        .Select(nObj => new InvestigationRecordViewDto()
                        {
                            AuditorName = nObj.AuditorName,
                            BeforeBMI = nObj.BeforeBMI,
                            BeforeWeight = nObj.BeforeWeight,
                            Birthday = nObj.Birthday,
                            CurrentWeight = nObj.CurrentWeight,
                            HealthBookId = nObj.HealthBookId,
                            Height = nObj.Height,
                            Id = nObj.Id,
                            InvestigatorName = nObj.InvestigatorName,
                            InvestionTime = nObj.CreationTime,
                            LastFinishQuestionSN = nObj.InvestigationAnswer.OrderByDescending(a => a.QuestionSerialNumber).Select(a => a.QuestionSerialNumber).FirstOrDefault(),
                            Name = nObj.Name,
                            QueueId = nObj.QueueId,
                            State = (InvestigationRecordStateType)nObj.State,
                            Week = nObj.Week
                        }).ToList();
                }
            }
            return result;
        }

        private void GenerateOrUpdateReport(InvestigationRecord record, NutritionalResearchDatabaseEntities mydb)
        {
            Dictionary<FoodCategory, double> foodCategoryAverageDailyIntake = new Dictionary<FoodCategory, double>();
            //计算并生成膳食构成报告
            try
            {
                var foodGroup = mydb.FoodCategory
                    .Where(nObj => nObj.StatisticsCategoryCode != null)
                    .OrderBy(nObj => nObj.StatisticsCategoryCode)
                    .GroupBy(nObj => nObj.StatisticsCategoryCode)
                    .ToList();
                foodGroup.ForEach(item =>
                {
                    double totalAverageDailyIntake = 0;
                    item.ToList().ForEach(cate =>
                    {
                        var _answer = (from nObj in mydb.Question
                                       join nObj2 in record.InvestigationAnswer on nObj.Id equals nObj2.QuestionId
                                       where nObj.CategoryId == cate.Id
                                       select nObj2).SingleOrDefault();
                        if (_answer != null && _answer.AnswerValue1.HasValue && _answer.AnswerValue2.HasValue)
                        {
                            var _intake = ComputerAverageDailyIntake((AnswerType)_answer.AnswerType, _answer.AnswerValue1.Value, _answer.AnswerValue2.Value);
                            foodCategoryAverageDailyIntake.Add(cate, _intake);
                            totalAverageDailyIntake += _intake;
                        }
                    });
                    var _structure = mydb.StructureOfMeals.Where(nObj => nObj.RecordId == record.Id && nObj.StructureCode == item.Key).SingleOrDefault();
                    if (_structure == null)
                    {
                        StructureOfMeals structure = new StructureOfMeals()
                        {
                            Id = Guid.NewGuid(),
                            CreationTime = DateTime.Now,
                            RecordId = record.Id,
                            StructureCode = item.Key,
                            StructureDescription = item.First().StatisticsCategoryName,
                            Unit = "g",
                            Intake = totalAverageDailyIntake
                        };
                        mydb.StructureOfMeals.Add(structure);
                    }
                    else
                    {
                        _structure.Intake = totalAverageDailyIntake;
                        _structure.UpdationTime = DateTime.Now;
                    }
                });
                mydb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("计算并生成膳食构成报告出错！",ex);
            }
            //计算并生成营业元素摄入量报告
            try
            {
                Dictionary<Foods, double> foodAverageDailyIntake = new Dictionary<Foods, double>();
                foreach (var item in foodCategoryAverageDailyIntake)
                {
                    foreach (var food in item.Key.Foods)
                    {
                        foodAverageDailyIntake.Add(food, item.Value * food.Proportion);
                    }
                }
                mydb.NuritiveElement.ToList().ForEach(item =>
                {
                    double nutrtiveElementIntake = 0;
                    foreach (var item2 in foodAverageDailyIntake)
                    {
                        nutrtiveElementIntake += ComputerNutrtiveElementIntake(item.Id, item2.Key, item2.Value);
                    }
                    double? _RNI_AI = null;
                    double? _RNIRatio = null;
                    if(item.Stage1StandardValue != null && item.Stage2StandardValue != null && item.Stage3StandardValue != null)
                    {
                        if(record.Stage == 1)
                        {
                            _RNI_AI = item.Stage1StandardValue.Value;
                        }
                        else if(record.Stage == 2)
                        {
                            _RNI_AI = item.Stage2StandardValue.Value;
                        }
                        else
                        {
                            _RNI_AI = item.Stage3StandardValue.Value;
                        }
                        _RNIRatio = 100 * nutrtiveElementIntake / _RNI_AI.Value;
                    }
                    var _statistics = mydb.NutrtiveElementIntakeStatistics.Where(nObj => nObj.RecordId == record.Id && nObj.NutritiveElementId == item.Id).SingleOrDefault();
                    if(_statistics == null)
                    {
                        NutrtiveElementIntakeStatistics statistics = new NutrtiveElementIntakeStatistics()
                        {
                            Id = Guid.NewGuid(),
                            CreationTime = DateTime.Now,
                            RecordId = record.Id,
                            NutritiveElementId = item.Id,
                            IntakeValue = nutrtiveElementIntake,
                            RNI_AIValue = _RNI_AI,
                            RNIRatio = _RNIRatio
                        };
                        mydb.NutrtiveElementIntakeStatistics.Add(statistics);
                    }
                    else
                    {
                        _statistics.IntakeValue = nutrtiveElementIntake;
                        _statistics.RNI_AIValue = _RNI_AI;
                        _statistics.RNIRatio = _RNIRatio;
                        _statistics.UpdationTime = DateTime.Now;
                    }
                });
                mydb.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("计算并生成营业元素摄入量报告出错！", ex);
            }
        }

        private double ComputerAverageDailyIntake(AnswerType type, int value1, double value2)
        {
            switch (type)
            {
                case AnswerType.Month:
                    return (value1 * value2) / 28;
                case AnswerType.Week:
                    return (value1 * 4 * value2) / 28;
                case AnswerType.Day:
                    return value1 * value2;
                case AnswerType.Other:
                default:
                    return 0;
            }
        }

        private double ComputerNutrtiveElementIntake(Guid nutrtiveElementId, Foods food, double foodAverageDailyIntake)
        {
            var foodNutrition = food.FoodNutritions.Where(nObj => nObj.NutritiveElementId == nutrtiveElementId).SingleOrDefault();
            if(foodNutrition != null)
            {
                return foodNutrition.Value * foodAverageDailyIntake;
            }
            else
            {
                return 0;
            }
        }

        public void ReGenerateReport(Guid recordId)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var record = mydb.InvestigationRecord.Where(nObj => nObj.Id == recordId).SingleOrDefault();
                if (record == null)
                {
                    throw new ArgumentException("无效记录Id");
                }
                if ((InvestigationRecordStateType)record.State == InvestigationRecordStateType.FinishedAndAudited)
                {
                    throw new InvalidOperationException("该记录已审核，不能重新生成报告");
                }
                try
                {
                    GenerateOrUpdateReport(record, mydb);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
