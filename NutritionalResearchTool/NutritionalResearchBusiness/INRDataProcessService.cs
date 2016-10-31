using NutritionalResearchBusiness.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness
{
    public interface INRDataProcessService
    {
        void CreateFoodNutritionsForTesting(List<FoodNutritionsPostDto> datas);

        List<Foods> GetFoodsList();
    }

    public class FoodNutritionsPostDto
    {
        public Guid FoodId { get; set; }

        public Guid NutritiveElementId { get; set; }

        public double Value { get; set; }
    }
}
