using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    public class InvestigationRecordQueryConditions
    {
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
        /// 查询开始时间
        /// </summary>
        public DateTime? QueryStartTime { get; set; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? QueryEndTime { get; set; }
    }
}
