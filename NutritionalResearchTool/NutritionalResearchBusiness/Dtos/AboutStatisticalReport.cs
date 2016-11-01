using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    public class NutritionalResearchStatisticalReportViewDto
    {
        /// <summary>
        /// 调查记录Id
        /// </summary>
        public Guid RecordId { get; set; }
        /// <summary>
        /// 孕妇姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
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
        /// 膳食构成
        /// </summary>
        public List<StructureOfMealsViewDto> StructureOfMeals { get; set; }
        /// <summary>
        /// 营养元素摄入统计
        /// </summary>
        public List<NutrtiveElementIntakeStatisticsViewDto> NutrtiveElementIntakeStatistics { get; set; }
        /// <summary>
        /// 填表记录
        /// </summary>
        public List<AnswerRecords> FillingRecords { get; set; }
    }

    /// <summary>
    /// 膳食构成视图
    /// </summary>
    public class StructureOfMealsViewDto
    {
        public Guid Id { get; set; }

        public Guid RecordId { get; set; }

        public string StructureCode { get; set; }

        public string StructureDescription { get; set; }

        public double Intake { get; set; }

        public string Unit { get; set; }
    }

    /// <summary>
    /// 营养元素摄入统计
    /// </summary>
    public class NutrtiveElementIntakeStatisticsViewDto
    {
        public Guid Id { get; set; }

        public Guid RecordId { get; set; }

        public Guid NutritiveElementId { get; set; }

        public string NutritiveName { get; set; }

        public double Intake { get; set; }

        public string Unit { get; set; }

        public double? RNI_AI { get; set; }

        public double? RNIRatio { get; set; }
    }

    /// <summary>
    /// 答题记录
    /// </summary>
    public class AnswerRecords
    {
        public Guid AnswerId { get; set; }

        public string FirstCategoryCode { get; set; }

        public string FirstCategoryName { get; set; }

        public string SecondCategoryCode { get; set; }

        public string SecondCategoryName { get; set; }

        public int MonthlyIntakeFrequency { get; set; }
    }
}
