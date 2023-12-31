


namespace Paradigm.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using System.Text.Json;

    [Table("user")]
    public class User
    {
        public User()
        {

        }
        public User(UserSignup signup, string salt, string hash)
        {
            this.UserId = Guid.NewGuid();
            this.Username = signup.Username;
            this.DisplayName = signup.DisplayName;
            this.CultureName = "en";
            this.TimeZoneId = "Pakistan Standard Time";
            this.Enabled = true;
            this.ProviderId = "LOCAL AUTHORITY";
            this.IsSuperAdmin = false;
            this.MobileNumber = signup.MobileNumber;
            this.PasswordHash = hash;
            this.PasswordSalt = salt;
        }
        public User(AddEditUser addEdit, string salt, string hash)
        {
            this.UserId = Guid.NewGuid();
            this.Username = addEdit.Username;
            this.DisplayName = addEdit.FirstName + addEdit.LastName;
            this.CultureName = "en";
            this.TimeZoneId = "Pakistan Standard Time";
            this.Enabled = true;
            this.ProviderId = "LOCAL AUTHORITY";
            this.IsSuperAdmin = false;
            this.MobileNumber = addEdit.MobileNumber;
            this.PasswordHash = hash;
            this.PasswordSalt = salt;
            Global.UserID = this.UserId;
        }

        /// <summary>
        /// Get and Set Value for UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>null</returns>
        [Key]
        [Required(ErrorMessage = "value for UserId is required")]
        [Column("UserId")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Get and Set Value for Username
        /// </summary>
        /// <param name="Username"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for Username is required")]
        [Column("Username")]
        public string Username { get; set; }
        /// <summary>
        /// Get and Set Value for DisplayName
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for DisplayName is required")]
        [Column("DisplayName")]
        public string DisplayName { get; set; }
        /// <summary>
        /// Get and Set Value for CultureName
        /// </summary>
        /// <param name="CultureName"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for CultureName is required")]
        [Column("CultureName")]
        public string CultureName { get; set; }
        /// <summary>
        /// Get and Set Value for TimeZoneId
        /// </summary>
        /// <param name="TimeZoneId"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for TimeZoneId is required")]
        [Column("TimeZoneId")]
        public string TimeZoneId { get; set; }
        /// <summary>
        /// Get and Set Value for Enabled
        /// </summary>
        /// <param name="Enabled"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for Enabled is required")]
        [Column("Enabled")]
        public bool Enabled { get; set; }
        /// <summary>
        /// Get and Set Value for PasswordSalt
        /// </summary>
        /// <param name="PasswordSalt"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for PasswordSalt is required")]
        [Column("PasswordSalt")]
        public string PasswordSalt { get; set; }
        /// <summary>
        /// Get and Set Value for PasswordHash
        /// </summary>
        /// <param name="PasswordHash"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for PasswordHash is required")]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; }
        /// <summary>
        /// Get and Set Value for ProviderId
        /// </summary>
        /// <param name="ProviderId"></param>
        /// <returns>null</returns>
        [Required(ErrorMessage = "value for ProviderId is required")]
        [Column("ProviderId")]
        public string ProviderId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string MobileNumber { get; set; }
    }
    public class UserSignup
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string MobileNumber { get; set; }
    }
    [Table("userdetail")]
    public class UserDetail
    {
        public UserDetail()
        {

        }
        public UserDetail(AuditTrack audit, AddEditUser addEdit)
        {
            this.UserDetailId = Guid.NewGuid();
            this.CreatedOn = audit.Time;
            this.CreatedBy = audit.UserId;
            this.UserId = addEdit.UserId ?? Global.UserID;
            this.Status = 1;
            this.FirstName = addEdit.FirstName;
            this.LastName = addEdit.LastName;
            this.Address = addEdit.Address;
            this.DOB = addEdit.DOB;
            this.CNIC = addEdit.CNIC;
            this.Role = addEdit.Role;
            //this.ImagePath = addEdit.ImagePath;
        }
        [Key]
        [Required(ErrorMessage = "value for UserId is required")]
        public Guid UserDetailId { get; set; }
        public Guid? CreatedBy { get; set; }
        public int? CreatedOn { get; set; }
        public int Status { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int? UpdatedOn { get; set; }
        public Guid UserId { get; set; }
#nullable enable
        public string? ImagePath { get; set; }
#nullable disable
        public int? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string DOB { get; set; }
        public string CNIC { get; set; }
#nullable enable
        public string? About { get; set; }
        public string? Occupation { get; set; }
        public string? Major { get; set; }
        public string? University { get; set; }
        public string? Gender { get; set; }
        public string? HighestDegree { get; set; }
#nullable disable

    }
    public class AddEditUser
    {
        public Guid? UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string CNIC { get; set; }
        [Required]
        public string DOB { get; set; }
        //       #nullable enable
        // public List<string>? Role { get; set; }
        // #nullable disable
        public string Role { get; set; }
        //public string ImagePath { get; set; }
    }
    public class EditProfile
    {
        public Guid UserId { get; set; }
        public string MobileNumber { get; set; }
        public string About { get; set; }
        public string Occupation { get; set; }
        public string Major { get; set; }
        public string University { get; set; }
        public string Gender { get; set; }
        public string HighestDegree { get; set; }
        public string Address { get; set; }
        public string DOB { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CNIC { get; set; }
    }
    // public class VW_User
    // {
    //     public int TotalCount { get; set; }
    //     public long SerialNo { get; set; }
    //     [Key]
    //     public Guid UserId { get; set; }
    //     public string Username { get; set; }
    //     public string DisplayName { get; set; }
    //     public string UniqueUserName { get; set; }
    //     public string MobileNumber { get; set; }
    //     public string CreatedBy { get; set; }
    //     public bool Enabled { get; set; }
    //     public int CreatedOn { get; set; }
    //     public string CreatedDate
    //     {
    //         get
    //         {
    //             Int32 dateTime = Convert.ToInt32(CreatedOn);
    //             DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
    //             dtDateTime = dtDateTime.AddSeconds(dateTime);
    //             return dtDateTime.ToString("MMM dd, yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture);
    //         }
    //     }
    // }

    public class List_User : TableParam
    {
        public string Username { get; set; }
        public string CNIC { get; set; }
        public string Role { get; set; }
        public int? Status { get; set; }
    }
    public class VW_User : ListingLogFields
    {
        [Key]
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public string DOB { get; set; }
        public string Role { get; set; }

    }
    public class VM_UserProfile
    {
        public string CNIC { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }

    }
    public class ResetPassword
    {
        public Guid UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}