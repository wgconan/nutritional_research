using NutritionalResearchBusiness.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    public class InvestigationAnswerOutputDto
    {
        /// <summary>
        /// 答案Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 调查记录Id
        /// </summary>
        public Guid InvestigationRecordId { get; set; }
        /// <summary>
        /// 问题Id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 问题类型
        /// </summary>
        public QuestionType Question_Type { get; set; }
        /// <summary>
        /// 问题序号
        /// </summary>
        public int QuestionSerialNumber { get; set; }
        /// <summary>
        /// 答案类型
        /// </summary>
        public AnswerType Answer_Type { get; set; }
        /// <summary>
        /// 答案值1
        /// </summary>
        public int? AnswerValue1 { get; set; }
        /// <summary>
        /// 答案值2
        /// </summary>
        public double? AnswerValue2 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdationTime { get; set; }
    }
}
