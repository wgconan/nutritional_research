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
        Guid CreateNewInvestigationRecord(NewInvestigationRecordDto newrecord);
        /// <summary>
        /// 获取已完成的调查记录数
        /// </summary>
        /// <returns></returns>
        int GetFinishedInvestigationRecordCount();
        /// <summary>
        /// 通过序号获取指定的问题视图
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <returns>问题视图</returns>
        QuestionViewDto GetQuestionViewBySerialNumber(int serialNumber);
        /// <summary>
        /// 完成指定的调查记录
        /// </summary>
        /// <param name="recordId">记录Id</param>
        void FinishSomeoneInvestigationRecord(Guid recordId);
        /// <summary>
        /// 添加或更新某记录指定问题的调查答案
        /// </summary>
        /// <param name="answer">答案</param>
        void AddOrUpdateSomeoneInvestigationAnswer(InvestigationAnswerInputDto answer);
        /// <summary>
        /// 按指定条件分页查询调查记录
        /// </summary>
        /// <param name="conditions">查询条件</param>
        /// <returns>调查记录分页结果</returns>
        PageQueryOutput<InvestigationRecordViewDto> QueryInvestigationRecordListByConditions(PageQueryInput<InvestigationRecordQueryConditions> conditions);
        /// <summary>
        /// 审核指定调查记录
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="AuditorName"></param>
        void AuditSomeoneInvestigationRecord(Guid recordId, string AuditorName);
        /// <summary>
        /// 重新生成统计报告（用于修改未审核记录问题答案后使用）
        /// </summary>
        /// <param name="recordId">记录Id</param>
        void ReGenerateReport(Guid recordId);
    }
}
