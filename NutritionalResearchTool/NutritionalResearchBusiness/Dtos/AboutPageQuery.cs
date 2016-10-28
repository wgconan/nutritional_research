using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NutritionalResearchBusiness.Dtos
{
    public class PageQueryInput<Parameters>
    {
        public int? PageIndex { get; set; }

        public int? PageSize { get; set; }

        public Parameters QueryConditions { get; set; }
    }

    public class PageQueryOutput<Result>
    {
        public int TotalCount { get; set; }

        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        public List<Result> QueryResult { get; set; }
    }
}
