using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;

namespace Paradigm.Server.Application
{
    public class UserService : IUserService
    {
        private DbContextBase _dbContext;
        private IResponse _response;
        private ICryptoService _crypto;
        private ICountResponse _countResp;
        public UserService(DbContextBase dbContext, IResponse response, ICryptoService crypto, ICountResponse countResp)
        {
            _dbContext = dbContext;
            _response = response;
            _crypto = crypto;
            _countResp = countResp;
        }
        public async Task<IResponse> GetAllByProc(List_User model, int diff)
        {

            model.Sort = model.Sort == null || model.Sort == "" ? "Username" : model.Sort;
            var data = (from user in _dbContext.User
                        join userdetail in _dbContext.UserDetail on user.UserId equals userdetail.UserId
                        where (
                                     (EF.Functions.ILike(user.Username, $"%{model.Username}%") || String.IsNullOrEmpty(model.Username))
                                     && (EF.Functions.ILike(userdetail.Role, $"%{model.Role}%") || String.IsNullOrEmpty(model.Role))
                                     && (userdetail.Status == model.Status || model.Status == null)
                                      && (userdetail.CNIC == model.CNIC || String.IsNullOrEmpty(model.CNIC))
                                     //   && (userdetail.Role == "User")
                                     )
                        select new VW_User
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            FirstName = userdetail.FirstName,
                            Lastname = userdetail.LastName,
                            MobileNumber = user.MobileNumber,
                            Address = userdetail.Address,
                            Status = userdetail.Status,
                            CNIC = userdetail.CNIC,
                            DOB = userdetail.DOB,
                            Role = userdetail.Role,
                        }).AsQueryable();

            //Sort and Return
            var count = data.Count();
            var sorted = await HelperStatic.OrderBy(data, model.SortEx, model.OrderEx == "desc").Skip(model.Start).Take(model.LimitEx).ToListAsync();
            foreach (var item in sorted)
            {
                item.TotalCount = count;
                item.SerialNo = ++model.Start;
            }
            _countResp.DataList = sorted;
            _countResp.TotalCount = sorted.Count > 0 ? sorted.First().TotalCount : 0;
            _response.Success = Constants.ResponseSuccess;
            _response.Data = _countResp;
            return _response;
        }
        public async Task<IResponse> AddEdit(AddEditUser addEdit, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            if (addEdit.UserId == null)
            {
                var userObj = await _dbContext.User.FirstOrDefaultAsync(x => x.Username == addEdit.Username);
                if (userObj != null)
                {
                    _response.Message = Constants.Exists.Replace("{data}", "User");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                var userObjforCnic = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.CNIC == addEdit.CNIC);
                if (userObjforCnic != null)
                {

                    _response.Message = Constants.Exists.Replace("{data}", "User CNIC");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }
                //Add User
                var salt = _crypto.CreateSalt();
                var hash = _crypto.CreateKey(salt, addEdit.Password);
                var user = new User(addEdit, salt, hash);
                _dbContext.User.Add(user);
                //await _dbContext.SaveChangesAsync();

                //Add User Detail
                addEdit.UserId = user.UserId;
                UserDetail userDetail = new(audit, addEdit);
                _dbContext.UserDetail.Add(userDetail);

                //Add User Role
                // List<UserRole> role = new List<UserRole>() { };
                // foreach (var item in addEdit.Role)
                // {
                UserRole r1 = new()
                {
                    UserId = user.UserId,
                    RoleId = userDetail.Role
                };
                _dbContext.UserRole.Add(r1);
                await _dbContext.SaveChangesAsync();

                //}
                var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, addEdit.UserId == null ? Constants.ActionMethods.Add : Constants.ActionMethods.Update);
                _dbContext.AuditHistory.Add(_auditLogs);
                await _dbContext.SaveChangesAsync();

                _response.Message = Constants.DataSaved;
                _response.Success = Constants.ResponseSuccess;
                return _response;

            }
            else
            {
                User user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == addEdit.UserId);
                if (user == null)
                {
                    _response.Message = Constants.NotFound.Replace("{data}", "User");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                UserDetail userdetail = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == addEdit.UserId);
                if (userdetail == null)
                {
                    _response.Message = Constants.NotFound.Replace("{data}", "User");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                var role = await _dbContext.UserRole.Where(x => x.UserId == addEdit.UserId).ToListAsync();
                _dbContext.UserRole.RemoveRange(role);
                await _dbContext.SaveChangesAsync();

                auditOldValue = JsonSerializer.Serialize(user, audit.Options) + "*" + JsonSerializer.Serialize(userdetail, audit.Options);
                user.DisplayName = addEdit.FirstName + addEdit.LastName;
                user.MobileNumber = addEdit.MobileNumber;
                user.Username = addEdit.Username;
                _dbContext.User.Update(user);
                // await _dbContext.SaveChangesAsync();

                //userdetail.ImagePath = addEdit.ImagePath;
                userdetail.UpdatedBy = audit.UserId;
                userdetail.UpdatedOn = audit.Time;
                userdetail.FirstName = addEdit.FirstName;
                userdetail.LastName = addEdit.LastName;
                userdetail.Address = addEdit.Address;
                userdetail.DOB = addEdit.DOB;
                userdetail.CNIC = addEdit.CNIC;
                userdetail.Role = addEdit.Role;
                _dbContext.UserDetail.Update(userdetail);
                //await _dbContext.SaveChangesAsync();

                // List<UserRole> roleAdd = new List<UserRole>() { };
                // foreach (var item in addEdit.Role)
                // {
                //     UserRole role1 = new UserRole();
                //     role1.UserId = user.UserId;
                //     role1.RoleId = item;
                //     roleAdd.Add(role1);
                // }
                // _dbContext.UserRole.AddRange(roleAdd);
                await _dbContext.SaveChangesAsync();

                auditNewValue = JsonSerializer.Serialize(user, audit.Options) + "*" + JsonSerializer.Serialize(userdetail, audit.Options);
                var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, addEdit.UserId == null ? Constants.ActionMethods.Add : Constants.ActionMethods.Update);
                _dbContext.AuditHistory.Add(_auditLogs);
                await _dbContext.SaveChangesAsync();

                _response.Message = Constants.DataUpdate;
                _response.Success = Constants.ResponseSuccess;
                return _response;
            }


        }
        public async Task<IResponse> GetSingle(Guid Id)
        {
            User User = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == Id);
            if (User == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            UserDetail userdetail = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == User.UserId);
            if (userdetail == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            var role = await _dbContext.UserRole.Where(x => x.UserId == User.UserId).ToListAsync();
            AddEditUser addEdit = new();
            addEdit.UserId = User.UserId;
            addEdit.Username = User.Username;
            addEdit.MobileNumber = User.MobileNumber;
            addEdit.CNIC = userdetail.CNIC;
            addEdit.FirstName = userdetail.FirstName;
            addEdit.LastName = userdetail.LastName;
            addEdit.DOB = userdetail.DOB;
            addEdit.Address = userdetail.Address;
            addEdit.Password = User.PasswordHash;
            addEdit.Role = userdetail.Role;
            //addEdit.Role = role.Select(x => x.RoleId).ToList();
            _response.Data = addEdit;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> getUserDetailForCertificate(Guid UserId)
        {
            User User = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (User == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            UserDetail userdetail = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == User.UserId);
            if (userdetail == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            Enrollment userenrollment = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.UserId == User.UserId);

            if (userenrollment == null)
            {
                _response.Message = ("Not Enrolled Yet in Any Course");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            // Get user details
            VM_UserReportViweModal UserReport = new();
            UserReport = await (from userdetails in _dbContext.UserDetail
                                join enrollment in _dbContext.Enrollment on UserId equals enrollment.UserId
                                join user in _dbContext.User on userdetail.UserId equals UserId
                                select new VM_UserReportViweModal
                                {
                                    FullName = userdetail.FirstName + " " + userdetail.LastName,
                                    Address = userdetail.Address,
                                    CNIC = userdetail.CNIC,
                                    MobileNumber = user.MobileNumber,
                                }).FirstOrDefaultAsync();
            // Get  course grades result
            // var usergradereport =await (from enrollment in _dbContext.Enrollment
            //                join course in _dbContext.Course on enrollment.CourseId equals course.CourseId
            //                join grade in _dbContext.UserGrade on enrollment.CourseId equals grade.CourseId
            //                where(
            //                 enrollment.UserId== UserId && grade.UserId == UserId
            //                )
            //             //    group grade by course into courseGrades
            //                select new CourseGradeReport{
            //                    CourseName= course.Title,
            //                    CourseStatus= enrollment.EnrolledStatus,
            //                    CourseGrade= grade.Grade //CourseGrade = courseGrades.Max(g => g.Grade)
            //                }).ToListAsync();
            var usergradereport = await (from enrollment in _dbContext.Enrollment
                                         join course in _dbContext.Course on enrollment.CourseId equals course.CourseId
                                         join grade in _dbContext.UserGrade.Where(g => g.UserId == UserId)
                                         on enrollment.CourseId equals grade.CourseId into gradeGroup
                                         from grade in gradeGroup.DefaultIfEmpty()
                                         where enrollment.UserId == UserId
                                         select new CourseGradeReport
                                         {
                                             CourseName = course.Title,
                                             CourseStatus = enrollment.EnrolledStatus == "1" ? "Completed" : "OnGoing",
                                             CourseGrade = grade == null ? null : grade.Grade
                                         }).ToListAsync();
            // Add course grade list to the repost coursegrade for user
            UserReport.courseGradeReport = usergradereport;
            _response.Data = UserReport;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> ActiveInactive(ActiveInactiveBool model, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            User User = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == model.Id);
            if (User == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            UserDetail userdetails = await _dbContext.UserDetail.FirstOrDefaultAsync(y => y.UserId == model.Id);
            if (userdetails == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "userdetails");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            auditOldValue = JsonSerializer.Serialize(User, audit.Options);
            User.Enabled = model.Status;
            _dbContext.User.Update(User);
            if (model.Status)
            {
                userdetails.Status = 1;
            }
            else
            {
                userdetails.Status = 0;
            }
            //await _dbContext.SaveChangesAsync();
            auditNewValue = JsonSerializer.Serialize(User, audit.Options);

            var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, model.Status == true ? Constants.ActionMethods.Active : Constants.ActionMethods.Deactive);
            _dbContext.AuditHistory.Add(_auditLogs);
            await _dbContext.SaveChangesAsync();

            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public string ListingQuery(TableParamModel model, int diff)
        {
            string query = "SELECT Count(usr.\"UserId\") OVER () TotalCount, ROW_NUMBER() over(Order by (Select 1)) as SerialNo," +
            "usr.\"UserId\"," +
            "usr.\"Username\"," +
            "usr.\"DisplayName\"," +
            "usr.\"MobileNumber\"," +
            "usr.\"Enabled\"," +
            "COALESCE(u1.\"DisplayName\", u.\"DisplayName\") AS \"CreatedBy\"," +
            "(COALESCE(d.\"UpdatedOn\", d.\"CreatedOn\") +" + diff + ") AS \"CreatedOn\" " +
            "FROM \"Roles\".\"User\" AS usr " +
            "INNER JOIN \"Roles\".\"UserDetail\" AS d ON d.\"UserId\" = usr.\"UserId\" " +
            "INNER JOIN \"Roles\".\"User\" AS u ON u.\"UserId\" = d.\"CreatedBy\" " +
            "LEFT OUTER JOIN \"Roles\".\"User\" AS u1 ON u1.\"UserId\" = d.\"UpdatedBy\" " +
            HelperStatic.QueryFinalize(model);
            return query;
        }
        public async Task<IResponse> EditProfile(EditProfile editUser, AuditTrack audit)
        {
            string auditOldValue = "N/A";
            string auditNewValue = "N/A";
            User user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == editUser.UserId);
            if (user == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            UserDetail userdetail = await _dbContext.UserDetail.FirstOrDefaultAsync(x => x.UserId == editUser.UserId);
            if (userdetail == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "User");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            var role = await _dbContext.UserRole.Where(x => x.UserId == editUser.UserId).ToListAsync();
            _dbContext.UserRole.RemoveRange(role);
            await _dbContext.SaveChangesAsync();

            auditOldValue = JsonSerializer.Serialize(user, audit.Options) + "*" + JsonSerializer.Serialize(userdetail, audit.Options);
            user.MobileNumber = editUser.MobileNumber;
            _dbContext.User.Update(user);

            userdetail.UpdatedBy = audit.UserId;
            userdetail.UpdatedOn = audit.Time;
            userdetail.Address = editUser.Address;
            userdetail.FirstName = editUser.FirstName;
            userdetail.LastName = editUser.LastName;
            userdetail.DOB = editUser.DOB;
            userdetail.About = editUser.About;
            userdetail.Major = editUser.Major;
            userdetail.University = editUser.University;
            userdetail.HighestDegree = editUser.HighestDegree;
            userdetail.Gender = editUser.Gender;
            userdetail.Occupation = editUser.Occupation;

            _dbContext.UserDetail.Update(userdetail);
            await _dbContext.SaveChangesAsync();

            auditNewValue = JsonSerializer.Serialize(user, audit.Options) + "*" + JsonSerializer.Serialize(userdetail, audit.Options);
            var _auditLogs = new AuditHistory(audit.UserId, audit.Username, auditOldValue, auditNewValue, audit.Time, audit.SessionId, audit.ActionScreen, Constants.ActionMethods.Update);
            _dbContext.AuditHistory.Add(_auditLogs);
            await _dbContext.SaveChangesAsync();

            _response.Message = Constants.DataUpdate;
            _response.Success = Constants.ResponseSuccess;
            return _response;



        }

    }
}