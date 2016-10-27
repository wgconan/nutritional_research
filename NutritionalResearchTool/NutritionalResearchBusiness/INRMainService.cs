using NutritionalResearchBusiness.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness
{
    public interface INRMainService
    {
        /// <summary>
        /// 创建新的调查记录
        /// </summary>
        /// <param name="newrecord">新的调查记录</param>
        /// <returns>记录Id</returns>
        Guid CreateNewInvestigationRecord(NewInvestigationRecord newrecord);

        int GetInvestigationRecordCount();
    }
}
