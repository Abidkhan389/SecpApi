using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IMenuService
    {
        Task<IResponse> GetScreensByRole(string id);
        Task<IResponse> MapScreensToRole(ScreenRoleMap model);
        Task<IResponse>  GetMenu(AuditTrack audit);
    }
}
