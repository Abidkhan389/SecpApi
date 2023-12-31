


namespace Paradigm.Data.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("userrole")]
    public class UserRole
    {
        
		/// <summary>
        /// Get and Set Value for RoleId
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns>null</returns>
		[Key]
		[Required(ErrorMessage="value for RoleId is required")]
		[Column("RoleId")]
		public string RoleId { get; set; }
		/// <summary>
        /// Get and Set Value for UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>null</returns>
		[Required(ErrorMessage="value for UserId is required")]
		[Column("UserId")]
		public Guid UserId { get; set; }
    }
}