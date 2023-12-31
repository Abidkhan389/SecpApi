


namespace Paradigm.Data.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("rolesecurityclaim")]
    public class RoleSecurityClaim
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
        /// Get and Set Value for SecurityClaimId
        /// </summary>
        /// <param name="SecurityClaimId"></param>
        /// <returns>null</returns>
		[Required(ErrorMessage="value for SecurityClaimId is required")]
		[Column("SecurityClaimId")]
		public string SecurityClaimId { get; set; }
		/// <summary>
        /// Get and Set Value for Value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>null</returns>
		[Column("Value")]
		public string Value { get; set; }
    }
}