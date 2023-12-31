using System;
using System.Threading.Tasks;
using Paradigm.Data.Model;
using Paradigm.Server.Application;

namespace Paradigm.Server.Interface
{
    public interface IDashboardService
    {
        Task<IResponse> GetOverViewForAdminDashboard();      
    }
}