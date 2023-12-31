namespace Paradigm.Server
{    
    using Microsoft.AspNetCore.Mvc;
    using Paradigm.Contract.Model;

    public static partial class Extensions
    {
        internal static string HttpContextCurrentUserKey = "CurrentUser";

        public static IUser ApplicationUser(this ControllerBase controller)
        {
            var user = controller.HttpContext.Items[HttpContextCurrentUserKey];

            return user == null ? null : (IUser)user;
        }
    }
}
