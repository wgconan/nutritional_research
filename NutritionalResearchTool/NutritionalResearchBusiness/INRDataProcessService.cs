using NutritionalResearchBusiness.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.Dtos;

namespace NutritionalResearchBusiness
{
    public interface INRDataProcessService
    {
        //void CreateFoodNutritionsForTesting(List<FoodNutritionsPostDto> datas);

        //List<Foods> GetFoodsList();

        NutritionalResearchStatisticalReportViewDto GetSomeoneRecordStatisticalReport(Guid recordId);

        int ExportNutritionalResearchReport2Excel(InvestigationRecordQueryConditions conditions,string fileName);

        int ExportNutritionalInvestigationRecords2Excel(InvestigationRecordQueryConditions conditions, string fileName);
    }

    public class FoodNutritionsPostDto
    {
        public Guid FoodId { get; set; }

        public Guid NutritiveElementId { get; set; }

        public double Value { get; set; }
    }
}
