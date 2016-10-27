using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.Dtos;
using NutritionalResearchBusiness.DAL;
using NutritionalResearchBusiness.Enums;

namespace NutritionalResearchBusiness.BLL
{
    public class NRMainService : INRMainService
    {
        public Guid CreateNewInvestigationRecord(NewInvestigationRecord newrecord)
        {
            if (newrecord == null)
            {
                throw new ArgumentNullException("newrecord", "记录参数不能为空!");
            }
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                var queneId = mydb.InvestigationRecord.Where(nObj => nObj.QueueId == newrecord.QueneId).FirstOrDefault();
                if (queneId != null)
                {
                    throw new ArgumentException("输入的队列编码重复", "newrecord.QueneId");
                }
                int _stage = 1;
                if(newrecord.Week < 13)
                {
                    _stage = 1;
                }
                else if(newrecord.Week >= 13 && newrecord.Week <= 28)
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
                    QueueId = newrecord.QueneId,
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

        public int GetInvestigationRecordCount()
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                return mydb.InvestigationRecord.Where(nObj => nObj.State != (int)InvestigationRecordStateType.NoFinish).Count();
            }
        }
    }
}
