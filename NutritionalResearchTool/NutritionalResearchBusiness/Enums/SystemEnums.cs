using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Enums
{
    public enum InvestigationRecordStateType : int
    {
        NoFinish = 0,
        FinishedAndNoAudit = 1,
        FinishedAndAudited = 2
    }

    public enum QuestionType : int
    {
        Standard = 0,
        Optional = 1,
        Choice = 2
    }

    public enum AnswerType : int
    {
        Month = 0,
        Week = 1,
        Day = 2,
        Other = 3
    }
}
