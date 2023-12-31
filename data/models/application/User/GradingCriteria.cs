using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    [Table("GradingCriteria", Schema = "User")]
    public class GradingCriteria : LogFields
    {
        [Key]
        public Guid GradingId { get; set; }
        public string GradeName { get; set; }
        public int Grade_S_R { get; set; }
        public int Grade_E_R { get; set; }
        public GradingCriteria()
        {

        }
        public GradingCriteria(VW_AddEditGradingCriteria model,Guid userId, int timestamp)
        {
            this.GradingId = Guid.NewGuid();
            this.GradeName = model.GradeName;
            this.Grade_S_R=model.Grade_S_R;
            this.Grade_E_R=model.Grade_E_R;
            this.CreatedBy = userId;
            this.CreatedOn = timestamp;
        }
    }
    public class VW_AddEditGradingCriteria : LogFields
    {
       public Guid? GradingId { get; set; }
       [Required]
        public string GradeName { get; set; }
        [Required]
        public int Grade_S_R { get; set; }
        [Required]
        public int Grade_E_R { get; set; }
    }
     public class VW_GradingCriteria : ListingLogFields
    {
        public Guid? GradingId { get; set; }
        public string GradeName { get; set; }
        public int Grade_S_R { get; set; }
        public int Grade_E_R { get; set; }

    }
    public class List_GradingCriteria : TableParam
    {
        public string GradeName { get; set; }
        // public int Grade_S_R { get; set; }
        // public int Grade_E_R { get; set; }
    }
}