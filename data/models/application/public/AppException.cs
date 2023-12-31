
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Paradigm.Data.Model
{
    [Table("appexception")]
    public class AppException
    {
		[Key]
		[Required(ErrorMessage="value for Id is required")]
		[Column("Id")]
		public Guid Id { get; set; }
        

		[Required(ErrorMessage="value for Message is required")]
		[Column("Message")]
		public string Message { get; set; }


		[Required(ErrorMessage="value for DateTime is required")]
		[Column("DateTime")]
		public int DateTime { get; set; }
    }
}