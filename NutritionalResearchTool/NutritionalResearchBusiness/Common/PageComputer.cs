using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Common
{
    public static class PageComputer
    {
        public static int ComputePageCount(int totalCount, int pageSize)
        {
            if (pageSize != 0)
            {
                return (totalCount % pageSize == 0) ? totalCount / pageSize : totalCount / pageSize + 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
