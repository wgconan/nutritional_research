using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.Enums;

namespace NutritionalResearchBusiness.Dtos
{
    /// <summary>
    /// 调查记录列表视图Dto
    /// </summary>
    public class InvestigationRecordViewDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 队列编号
        /// </summary>
        public string QueueId { get; set; }
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
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 孕期
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 孕前体重
        /// </summary>
        public double BeforeWeight { get; set; }
        /// <summary>
        /// 当前体重
        /// </summary>
        public double CurrentWeight { get; set; }
        /// <summary>
        /// 孕前BMI
        /// </summary>
        public double BeforeBMI { get; set; }
        /// <summary>
        /// 调查者姓名
        /// </summary>
        public string InvestigatorName { get; set; }
        /// <summary>
        /// 调查时间
        /// </summary>
        public DateTime InvestionTime { get; set; }
        /// <summary>
        /// 调查状态
        /// </summary>
        public InvestigationRecordStateType State { get; set; }
        /// <summary>
        /// 审核者名字
        /// </summary>
        public string AuditorName { get; set; }
        /// <summary>
        /// 最后完成的问题序号
        /// </summary>
        public int LastFinishQuestionSN { get; set; }
    }
}
