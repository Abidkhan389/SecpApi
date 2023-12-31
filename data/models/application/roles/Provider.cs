


namespace Paradigm.Data.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("provider")]
    public class Provider
    {
        
		/// <summary>
        /// Get and Set Value for ProviderId
        /// </summary>
        /// <param name="ProviderId"></param>
        /// <returns>null</returns>
		[Key]
		[Required(ErrorMessage="value for ProviderId is required")]
		[Column("ProviderId")]
		public string ProviderId { get; set; }
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
        /// Get and Set Value for Name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>null</returns>
		[Required(ErrorMessage="value for Name is required")]
		[Column("Name")]
		public string Name { get; set; }
    }
}