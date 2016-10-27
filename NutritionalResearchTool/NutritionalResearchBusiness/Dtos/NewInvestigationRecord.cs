using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    /// <summary>
    /// 新的调查记录
    /// </summary>
    public class NewInvestigationRecord
    {
        /// <summary>
        /// 队列编号
        /// </summary>
        public string QueneId { get; set; }
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
        /// 调查者姓名
        /// </summary>
        public string InvestigatorName { get; set; }
    }
}
