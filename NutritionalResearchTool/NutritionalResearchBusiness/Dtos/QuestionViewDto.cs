using NutritionalResearchBusiness.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    /// <summary>
    /// 问题视图
    /// </summary>
    public class QuestionViewDto
    {
        /// <summary>
        /// 问题Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; }
        /// <summary>
        /// 一级分类编码
        /// </summary>
        public string FirstCategoryCode { get; set; }
        /// <summary>
        /// 一级分类名
        /// </summary>
        public string FirstCategoryName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public QuestionType Type { get; set; }
        /// <summary>
        /// 参考图示列表
        /// </summary>
        public List<ReferenceDiagramDto> ReferenceDiagramList { get; set; }
        /// <summary>
        /// 当前进度
        /// </summary>
        public double CurrentProgress { get; set; }
    }
}
