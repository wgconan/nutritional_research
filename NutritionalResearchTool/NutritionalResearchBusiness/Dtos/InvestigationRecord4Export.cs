using NutritionalResearchBusiness.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    public class InvestigationRecord4Export
    {
        /// <summary>
        /// 队列编号
        /// </summary>
        public string QueueId { get; set; }
        /// <summary>
        /// 孕期
        /// </summary>
        public PregnancyType Pregnancy
        {
            get
            {
                if(Week < 13)
                {
                    return PregnancyType.A;
                }
                else if(Week > 28)
                {
                    return PregnancyType.C;
                }
                else
                {
                    return PregnancyType.B;
                }
            }
        }
        /// <summary>
        /// 围产保健手册编号
        /// </summary>
        public string HealthBookId { get; set; }
        /// <summary>
        /// 孕妇姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 孕妇生日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 孕周
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 当前体重
        /// </summary>
        public double CurrentWeight { get; set; }
        /// <summary>
        /// 孕前体重
        /// </summary>
        public double BeforeWeight { get; set; }
        /// <summary>
        /// 调查时间
        /// </summary>
        public string InvestionTime { get; set; }
        /// <summary>
        /// 调查者姓名
        /// </summary>
        public string InvestigatorName { get; set; }
        /// <summary>
        /// 审核者名字
        /// </summary>
        public string AuditorName { get; set; }
        /// <summary>
        /// 食物分类编码
        /// </summary>
        public string FoodCategoryCode { get; set; }
        /// <summary>
        /// 月摄入频率
        /// </summary>
        public int MonthlyIntakeFrequency { get; set; }
        /// <summary>
        /// 平均每次摄入量
        /// </summary>
        public double? AverageIntakePerTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
