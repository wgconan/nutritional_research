using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NutritionalResearchBusiness;
using NutritionalResearchBusiness.BLL;

namespace NutritionalResearchToolApplication
{
    public static class BusinessStaticInstances
    {
        private static INRDataService NRDataServiceInstance = null;
        private static INRMainService NRMainServiceInstance=null;

        public static INRDataService GetSingleDataServiceInstance()
        {
            return (NRDataServiceInstance != null) ? NRDataServiceInstance : new NRDataService();
        }
        public static INRMainService GetSingleMainServiceInstance()
        {
            return (NRMainServiceInstance != null) ? NRMainServiceInstance : new NRMainService();
        }
    }
}
