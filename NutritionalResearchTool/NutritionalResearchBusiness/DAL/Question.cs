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
    
    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            this.ReferenceDiagram = new HashSet<ReferenceDiagram>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid CategoryId { get; set; }
        public int SerialNumber { get; set; }
        public int Type { get; set; }
        public int Sort { get; set; }
        public System.DateTime CreationTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReferenceDiagram> ReferenceDiagram { get; set; }
    }
}
