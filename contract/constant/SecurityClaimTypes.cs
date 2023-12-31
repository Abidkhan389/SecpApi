namespace Paradigm.Contract.Constant
{
    public static class SecurityClaimTypes
    {
		public const string AllowedValuesPattern = @"^([X]|([CRUD]{1,4}))$";
		public const string Example = "example";

		public const string Applications = "applications";
		public const string Language = "language";
		public const string Strings = "strings";
		public const string TranslationStatus = "translationstatus";
		public const string Translations = "translations";
		public const string Provider = "provider";
		public const string Role = "role";
		public const string RoleSecurityClaim = "rolesecurityclaim";
		public const string SecurityClaim = "securityclaim";
		public const string User = "user";
		public const string UserRole = "userrole";
	}
}