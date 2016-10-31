using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NutritionalResearchBusiness.DAL;

namespace NutritionalResearchBusiness.BLL
{
    public class NRDataProcessService : INRDataProcessService
    {
        public void CreateFoodNutritionsForTesting(List<FoodNutritionsPostDto> datas)
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                List<FoodNutritions> _foodnutritionList = datas.Select(nObj => new FoodNutritions()
                {
                    Id = Guid.NewGuid(),
                    FoodId = nObj.FoodId,
                    NutritiveElementId = nObj.NutritiveElementId,
                    Value = nObj.Value
                }).ToList();
                try
                {
                    mydb.FoodNutritions.AddRange(_foodnutritionList);
                    mydb.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<Foods> GetFoodsList()
        {
            using (NutritionalResearchDatabaseEntities mydb = new NutritionalResearchDatabaseEntities())
            {
                return mydb.Foods.OrderBy(f => f.CreationTime).ToList();
            }
        }
    }
}
