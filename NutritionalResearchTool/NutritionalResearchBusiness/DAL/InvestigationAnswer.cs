//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NutritionalResearchBusiness.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class InvestigationAnswer
    {
        public System.Guid Id { get; set; }
        public System.Guid InvestigationRecordId { get; set; }
        public System.Guid QuestionId { get; set; }
        public int QuestionType { get; set; }
        public int QuestionSerialNumber { get; set; }
        public int AnswerType { get; set; }
        public Nullable<int> AnswerValue1 { get; set; }
        public Nullable<double> AnswerValue2 { get; set; }
        public System.DateTime CreationTime { get; set; }
        public Nullable<System.DateTime> UpdationTime { get; set; }
    
        public virtual InvestigationRecord InvestigationRecord { get; set; }
    }
}
