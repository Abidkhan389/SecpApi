using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    public class MenuService : IMenuService
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        public MenuService(DbContextBase dbContext, IResponse response)
        {
            _dbContext = dbContext;
            _response = response;
        }
        #region Menu Mapping
        public async Task<IResponse> GetScreensByRole(string id)
        {
            var role = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == id);
            if (role == null)
            {
                _response.Success = Constants.ResponseFailure;
                _response.Message = Constants.NotFound.Replace("{data}", "Role");
                return _response;
            }
            ScreenRoleMap map = new ScreenRoleMap();
            map.RoleId = role.RoleId;
            map.RoleName = role.Name;
            List<ScreenRoleViewModel> screenMapViewModels = new List<ScreenRoleViewModel>() { };
            var screensMap = await _dbContext.RoleScreen.Where(x => x.RoleId == id).ToListAsync();
            var screens = await _dbContext.Screen.ToListAsync();
            foreach (var item in screens)
            {
                ScreenRoleViewModel screenMapViewModel = new ScreenRoleViewModel();
                screenMapViewModel.ScreenId = item.ScreenId;
                screenMapViewModel.ScreenName = item.ScreenName;
                screenMapViewModel.Add = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.Add ?? false;
                screenMapViewModel.Edit = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.Edit ?? false;
                screenMapViewModel.Delete = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.Delete ?? false;
                screenMapViewModel.View = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.View ?? false;
                screenMapViewModel.Print = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.Print ?? false;
                screenMapViewModel.Download = screensMap.FirstOrDefault(x => x.ScreenId == item.ScreenId)?.Download ?? false;
                screenMapViewModels.Add(screenMapViewModel);
            }
            map.Screens = screenMapViewModels;
            _response.Data = map;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> MapScreensToRole(ScreenRoleMap model)
        {
            var screens = await _dbContext.RoleScreen.Where(x => x.RoleId == model.RoleId).ToListAsync();
            _dbContext.RoleScreen.RemoveRange(screens);
            await _dbContext.SaveChangesAsync();
            var screensToAdd = model.Screens.Where(x => x.Add == true || x.Edit == true || x.Delete == true || x.View == true || x.Print == true || x.Download == true).ToList();
            List<RoleScreen> screenMaps = new List<RoleScreen>();
            foreach (var item in screensToAdd)
            {
                RoleScreen map = new RoleScreen();
                map.RoleScreenId = Guid.NewGuid();
                map.RoleId = model.RoleId;
                map.ScreenId = item.ScreenId;
                map.Add = item.Add;
                map.Edit = item.Edit;
                map.Delete = item.Delete;
                map.View = item.View;
                map.Print = item.Print;
                map.Download = item.Download;
                screenMaps.Add(map);
            }
            _dbContext.RoleScreen.AddRange(screenMaps);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            _response.Message = Constants.DataSaved;
            return _response;
        }
        #endregion
        #region Menu
        public async Task<IResponse> GetMenu(AuditTrack audit)
        {
            var userRoles = await _dbContext.UserRole.Where(x => x.UserId == audit.UserId).Select(x => x.RoleId).ToListAsync();
            var menu = await _dbContext.Role.Where(x => userRoles.Contains(x.RoleId) && x.Enabled).ToListAsync();

            List<UsersMenu> listUserMenu = new List<UsersMenu>() { };
            foreach (var item in menu)
            {
                UsersMenu men = new UsersMenu();
                men.RoleId = item.RoleId;
                men.RoleName = item.Name;
                List<ScreenRoleViewModel> menuScreens = new List<ScreenRoleViewModel>() { };
                var screens = await (from screenMap in _dbContext.RoleScreen
                                     join screen in _dbContext.Screen
                                     on screenMap.ScreenId equals screen.ScreenId
                                     where screenMap.RoleId == item.RoleId
                                     select new
                                     {
                                         screenMap,
                                         screen
                                     }).ToListAsync();
                foreach (var item1 in screens)
                {
                    ScreenRoleViewModel smv = new ScreenRoleViewModel(item1.screenMap, item1.screen);
                    menuScreens.Add(smv);
                }
                men.Screens = menuScreens;
                listUserMenu.Add(men);
            }
            _response.Data = listUserMenu;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        #endregion
    }
}