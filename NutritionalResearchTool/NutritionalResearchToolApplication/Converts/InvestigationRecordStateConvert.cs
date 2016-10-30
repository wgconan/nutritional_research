using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

using NutritionalResearchBusiness.Enums;

namespace NutritionalResearchToolApplication.Converts
{
    [ValueConversion(typeof(InvestigationRecordStateType), typeof(String))]
    public class InvestigationRecordStateConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            InvestigationRecordStateType state = (InvestigationRecordStateType)value;
            switch (state)
            {
                case InvestigationRecordStateType.NoFinish:
                    return "未完成";
                case InvestigationRecordStateType.FinishedAndNoAudit:
                    return "待审核";
                case InvestigationRecordStateType.FinishedAndAudited:
                    return "已审核";
                default:
                    return state.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stateStr = value as string;
            if(stateStr == "未完成")
            {
                return InvestigationRecordStateType.NoFinish;
            }
            if (stateStr == "待审核")
            {
                return InvestigationRecordStateType.FinishedAndNoAudit;
            }
            if (stateStr == "已审核")
            {
                return InvestigationRecordStateType.FinishedAndAudited;
            }
            return InvestigationRecordStateType.NoFinish;
        }
    }
}
