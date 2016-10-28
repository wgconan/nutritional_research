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
        private static INRDataProcessService NRDataProcessServiceInstance = null;
        private static INRMainService NRMainServiceInstance=null;

        public static INRDataProcessService GetSingleDataProcessServiceInstance()
        {
            return (NRDataProcessServiceInstance != null) ? NRDataProcessServiceInstance : new NRDataProcessService();
        }
        public static INRMainService GetSingleMainServiceInstance()
        {
            return (NRMainServiceInstance != null) ? NRMainServiceInstance : new NRMainService();
        }
    }
}
