//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NutritionalResearchBusiness.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Foods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Foods()
        {
            this.FoodNutritions = new HashSet<FoodNutritions>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid CategoryId { get; set; }
        public string Code { get; set; }
        public string TypicalFood { get; set; }
        public string FoodsDescription { get; set; }
        public System.DateTime CreationTime { get; set; }
        public double Proportion { get; set; }
        public Nullable<double> Edible { get; set; }
    
        public virtual FoodCategory FoodCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FoodNutritions> FoodNutritions { get; set; }
    }
}
