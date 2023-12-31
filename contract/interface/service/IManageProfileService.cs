namespace Paradigm.Contract.Interface
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Paradigm.Contract.Model;

    public interface IManageProfileService
    {
        Task<IUserExtended> UpdateUserCulture(long userId, string cultureName, string timeZoneId);
    }
}
