using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Paradigm.Data.Model
{
    [Table("screen")]
    public class Screen
    {
        [Key]
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string Url { get; set; }
        public Guid? ParentScreen { get; set; }
        public int OrderBy { get; set; }
        public int ScreenType { get; set; }
        public int Status { get; set; }
    }
    [Table("rolescreen")]
    public class RoleScreen
    {
        [Key]
        public Guid RoleScreenId { get; set; }
        public string RoleId { get; set; }
        public Guid ScreenId { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Print { get; set; }
        public bool Download { get; set; }
    }
    public class ScreenRoleMap
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<ScreenRoleViewModel> Screens { get; set; }
    }
    public class ScreenRoleViewModel
    {
        public ScreenRoleViewModel()
        {

        }
        public ScreenRoleViewModel(RoleScreen roleScreen, Screen screen)
        {
            this.ScreenId = screen.ScreenId;
            this.ScreenName = screen.ScreenName;
            this.Add = roleScreen.Add;
            this.Edit = roleScreen.Edit;
            this.Delete = roleScreen.Delete;
            this.View = roleScreen.View;
            this.Print = roleScreen.Print;
            this.Download = roleScreen.Download;
        }
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool Print { get; set; }
        public bool Download { get; set; }
    }
    public class UsersMenu
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<ScreenRoleViewModel> Screens { get; set; }
    }
}