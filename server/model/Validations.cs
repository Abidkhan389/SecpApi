namespace Paradigm.Server
{
    public static partial class Constants
    {
        public const string EmailAddressInUse = "validation.login.EmailAddressInUse";
        public const string ExpiredNonce = "login.nonce.expired";
        public const string FailedToVerifyUser = "validation.login.FailedToVerifyUser";
        public const string InvalidNonce = "login.nonce.invalid";
        public const string InvalidUsernameOrPassword = "validation.login.InvalidUsernameOrPassword";
        public const string InProgressNonce = "login.nonce.inProgress";
        public const string InvalidAccessToken = "validation.login.InvalidAccessToken";
        public const string SecureContent = "dict.secureContent";
        public const string UnknownCulture = "validation.unknownCulture";
        public const string UnknownUser = "validation.unknownUser";
        public const bool ResponseSuccess = true;
        public const bool ResponseFailure = false;
        public const string DataSaved = "Your data has been saved successfully";
        public const string DataUpdate = "Your data has been updated successfully";
        public const string DataDelete = "You data deleted Successfully";
        public const string IncorrectUsernamePassword = "Username or Password is incorrect";
        public const string ModelStateStateIsInvalid = "Model state is invalid";
        public const string UniqueUserDetails = "Username/Email/MobileNumber already exist";
        public const string UniqueEmail = "This email is taken. Try another.";
        public const string UniqueUsername = "This username is taken. Try another.";
        public const string UniqueMobileNumber = "This mobile number is taken. Try another.";
        public const string SelectOption = "Please select a correct option";
        public const string ErrorWhileSavingData = "Error while saving data ";
        public const string FileNameRequired = "File name required";
        public const string FileNotFound = "Unable to download the required file";
        public const string UnAuthorized = "User Unauthorized";
        public const string NotFound = "{data} not found";
        public const string Exists = "{data} name already exits";
        public const string IdRequired = "Id is required";
        public const string TimeZone = "Time-zone";
        public const string Origin = "Origin";
        public const string Referer = "Referer";
        public static partial class ActionMethods
        {
            public const string Logout = "Logout";
            public const string LoginSuccess = "LoginSuccess";
            public const string LoginFailure = "LoginFailure";
            public const string NewAccount = "NewAccount";
            public const string AccountLocked = "AccountLocked";
            public const string AccountUnlocked = "AccountUnlocked";
            public const string LockUser = "Lock User";
            public const string Login = "Login";
            public const string ResetPassword = "ResetPassword";
            public const string Anomaly = "Anomaly";
            public const string TwoFactorAuthSet = "TwoFactorAuth";
            public const string Add = "Add";
            public const string Update = "Update";
            public const string Active = "Active";
            public const string Deactive = "Deactive";
            public const string Delete = "Delete";
            public const string Submission = "Submission";
            public const string Rejection = "Rejection";
            public const string Approval = "Approval";
        }
    }
}
