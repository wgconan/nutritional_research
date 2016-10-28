using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    /// <summary>
    /// 参考图示
    /// </summary>
    public class ReferenceDiagramDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconPath { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }
    }
}
