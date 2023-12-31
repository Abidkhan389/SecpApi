using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    [Table("TempEnrollment", Schema = "User")]
    public class TempEnrollment : LogFields
    {
        [Key]
        public Guid TempEnrolledId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public int ApprovalAgain { get; set; }
        public TempEnrollment()
        {

        }
        public TempEnrollment(VW_EnrolledViewModel model, int TimeStamp)
        {
            this.TempEnrolledId = Guid.NewGuid();
            this.UserName = model.UserName;
            this.UserId = model.UserId;
            this.CourseId = model.CourseId;
            this.CreatedOn = TimeStamp;
            this.CreatedBy = model.UserId;
            this.Status = 0;
            this.ApprovalAgain = 0;
        }
    }

    public class AddEditTempEnrollment : LogFields
    {
        public Guid? EnrolledId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid CategoryId { get; set; }
    }
}