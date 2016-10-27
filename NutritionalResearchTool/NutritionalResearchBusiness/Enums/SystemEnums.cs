using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Enums
{
    public enum InvestigationRecordStateType:int
    {
        NoFinish = 0,
        FinishedAndNoAudit = 1,
        FinishedAndAudited = 2
    }
}
