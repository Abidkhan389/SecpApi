


namespace Paradigm.Data.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("securityclaim")]
    public class SecurityClaim
    {
        
		/// <summary>
        /// Get and Set Value for SecurityClaimId
        /// </summary>
        /// <param name="SecurityClaimId"></param>
        /// <returns>null</returns>
		[Key]
		[Required(ErrorMessage="value for SecurityClaimId is required")]
		[Column("SecurityClaimId")]
		public string SecurityClaimId { get; set; }
		/// <summary>
        /// Get and Set Value for Description
        /// </summary>
        /// <param name="Description"></param>
        /// <returns>null</returns>
		[Required(ErrorMessage="value for Description is required")]
		[Column("Description")]
		public string Description { get; set; }
		/// <summary>
        /// Get and Set Value for Enabled
        /// </summary>
        /// <param name="Enabled"></param>
        /// <returns>null</returns>
		[Required(ErrorMessage="value for Enabled is required")]
		[Column("Enabled")]
		public bool Enabled { get; set; }
		/// <summary>
        /// Get and Set Value for Origin
        /// </summary>
        /// <param name="Origin"></param>
        /// <returns>null</returns>
		[Column("Origin")]
		public string Origin { get; set; }
		/// <summary>
        /// Get and Set Value for ValidationPattern
        /// </summary>
        /// <param name="ValidationPattern"></param>
        /// <returns>null</returns>
		[Column("ValidationPattern")]
		public string ValidationPattern { get; set; }
    }
}