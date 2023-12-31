using System.Threading.Tasks;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
// using MimeKit;
// using Paradigm.Data.ViewModels;
// using MailKit.Security;

namespace Paradigm.Server.Application
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        //private readonly MailSettings _mailSettings;
        private readonly ICountResponse _countResp;

        public EnrollmentService(DbContextBase dbContext, IResponse response, ICountResponse countResponse)
        {
            _dbContext = dbContext;
            _response = response;
            this._countResp = countResponse;
        }
        public async Task<IResponse> GetById(Guid Id)
        {
            var enrolledUser = await (from enrollment in _dbContext.Enrollment
                                      join course in _dbContext.Course on enrollment.CourseId equals course.CourseId
                                      //join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                                      where (
                                          enrollment.EnrolledId == Id)
                                      select new AddEditEnrollment
                                      {
                                          CategoryId = course.CategoryId,
                                          CourseId = enrollment.CourseId,
                                          UserId = enrollment.UserId,
                                      }).FirstOrDefaultAsync();
            if (enrolledUser != null)
            {
                _response.Data = enrolledUser;
                _response.Success = Constants.ResponseSuccess;
                return _response;

            }
            _response.Message = ("Enrollment Not Found");
            _response.Success = Constants.ResponseFailure;
            return _response;
        }
        public async Task<IResponse> AddEditEnrollment(AddEditEnrollment model, Guid userId)
        {
            if (model.EnrolledId == null)
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == model.UserId);
                var enrolleduser = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.UserId == model.UserId && x.CourseId == model.CourseId);
                if (enrolleduser != null)
                {
                    _response.Message = ("User already enrolled.");
                    _response.Success = Constants.ResponseFailure;
                    return _response;

                }

                Enrollment enrollment = new(model, userId, HelperStatic.GetCurrentTimeStamp(), user.Username);

                await _dbContext.Enrollment.AddAsync(enrollment);
                await _dbContext.SaveChangesAsync();
                _response.Data = enrollment;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataSaved;
                return _response;
            }
            else
            {
                var user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == model.UserId);
                var alreadyAssignCourse = await _dbContext.Enrollment.FirstOrDefaultAsync(x =>
                                        x.UserId == model.UserId && x.CourseId == model.CourseId);
                if (alreadyAssignCourse != null)
                {
                    _response.Message = ("User already enrolled in this course!");
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                else
                {
                    var editenrollment = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.EnrolledId == model.EnrolledId);
                    if (editenrollment != null)
                    {
                        editenrollment.UserName = user.Username;
                        editenrollment.CourseId = model.CourseId;
                        editenrollment.UserId = model.UserId;
                        editenrollment.UpdatedBy = userId;
                        editenrollment.UpdatedOn = HelperStatic.GetCurrentTimeStamp();
                        _dbContext.Enrollment.Update(editenrollment);
                        await _dbContext.SaveChangesAsync();
                        _response.Success = Constants.ResponseSuccess;
                        _response.Message = Constants.DataUpdate;
                        return _response;
                    }
                }
                _response.Message = Constants.NotFound.Replace("{data}", "Enrollment");
                _response.Success = Constants.ResponseFailure;
                return _response;

            }
        }

        public async Task<IResponse> EnrollUser(VW_EnrolledViewModel model)
        {
            //enroll user call if user time duration completed for any course and he/she not completed this course yet
            //then user send again request for course access.
            if (model.ApplyAgain == 1)
            {
                var enrolledUser = await _dbContext.Enrollment.Where(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.EnrolledStatus == "0").FirstOrDefaultAsync();
                if (enrolledUser != null)
                {
                    var tempuser = await _dbContext.TempEnrollment.Where(x => x.UserId == model.UserId && x.CourseId == model.CourseId).FirstOrDefaultAsync();
                    if (tempuser != null)
                    {
                        _response.Message = "You've already send request for this course access";
                        _response.Success = Constants.ResponseFailure;
                        return _response;
                    }
                    TempEnrollment tempenrollment = new()
                    {
                        TempEnrolledId = Guid.NewGuid(),
                        UserId = model.UserId,
                        CourseId = model.CourseId,
                        UserName = model.UserName,
                        ApprovalAgain = 1,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = HelperStatic.GetCurrentTimeStamp(),
                        UpdatedBy = model.UpdatedBy,
                        UpdatedOn = model.UpdatedOn,
                    };
                    await _dbContext.TempEnrollment.AddAsync(tempenrollment);
                    await _dbContext.SaveChangesAsync();
                    _response.Data = tempenrollment;
                    _response.Success = Constants.ResponseSuccess;
                    _response.Message = "Your request again sent for course access to Admin for Approval.";
                    return _response;
                }
                _response.Message = "You'r not enrolled for this course yet.";
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            var user = await _dbContext.Enrollment.Where(x => x.UserId == model.UserId && x.CourseId == model.CourseId && x.EnrolledStatus == "0").FirstOrDefaultAsync();
            if (user == null)
            {
                var tempuser = await _dbContext.TempEnrollment.Where(x => x.UserId == model.UserId && x.CourseId == model.CourseId).FirstOrDefaultAsync();
                if (tempuser != null)
                {
                    _response.Message = "You'r Alraedy Applied for this course";
                    _response.Success = Constants.ResponseFailure;
                    return _response;
                }
                TempEnrollment tempenrollment = new(model, HelperStatic.GetCurrentTimeStamp());
                await _dbContext.TempEnrollment.AddAsync(tempenrollment);
                await _dbContext.SaveChangesAsync();
                //for sending Email
                // var email = new MimeMessage();

                //     email.Sender = MailboxAddress.Parse("fatishiekh57@gmail.com");
                //     email.To.Add(MailboxAddress.Parse(model.Email));
                //     email.Subject = "You'r Enrolled" ;
                //     var builder = new BodyBuilder();

                //     builder.HtmlBody = $@"<html>
                //                         <body>
                //                         <p>Dear ,{model.Name}</p>
                //                         <p>Welcome. You're enrolled now for the course.Best of luck we hope you learn alot from it, we thank you for choosing us.</p>
                //                         <p>Don't Forget to share your Reviews and Recommendation.</p>
                //                         <p>Sincerely,<br>KHAZNA PK</br></p>
                //                         </body>
                //                         </html>";;
                //     email.Body = builder.ToMessageBody();
                //     using var smtp = new MailKit.Net.Smtp.SmtpClient();
                //     smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                //     smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                //     var flag= await smtp.SendAsync(email);
                //     smtp.Disconnect(true);
                _response.Data = tempenrollment;
                _response.Success = Constants.ResponseSuccess;
                _response.Message = "After Admin Approval you'r able to view the Course Content";
                return _response;
            }
            _response.Message = "You'r Alraedy Enrolled for this course";
            _response.Success = Constants.ResponseFailure;
            return _response;

        }

        public async Task<IResponse> GetAllByProc(List_EnrolledUser model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "UserName" : model.Sort;
            //LINQ
            var data = (from enrolledUser in _dbContext.Enrollment
                        join course in _dbContext.Course on enrolledUser.CourseId equals course.CourseId
                        join category in _dbContext.Category on course.CategoryId equals category.CategoryId
                        join createdBy in _dbContext.User on enrolledUser.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on enrolledUser.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                        (EF.Functions.ILike(enrolledUser.UserName, $"%{model.UserName}%") || String.IsNullOrEmpty(model.UserName)) &&
                          (category.CategoryId == model.CategoryId || model.CategoryId == null) &&
                          (enrolledUser.CourseId == model.CourseId || model.CourseId == null))
                        select new VW_Enrollment
                        {
                            CategoryName = category.CategoryName,
                            CourseName = course.Title,
                            UserName = enrolledUser.UserName,
                            EnrolledId = enrolledUser.EnrolledId
                            // CreatedBy = updateBy == null ? updatedUserr.DisplayName : updateBy.DisplayName,
                            // CreatedOn = (int)(Category.UpdatedOn == null ? Category.CreatedOn + diff : Category.UpdatedOn.Value + diff)
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
        public async Task<IResponse> GetAllBywaitingForApprovalUserList(List_UserApproval model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "UserName" : model.Sort;
            //LINQ
            var data = (from enrolledUser in _dbContext.TempEnrollment
                        join course in _dbContext.Course on enrolledUser.CourseId equals course.CourseId
                        join createdBy in _dbContext.User on enrolledUser.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on enrolledUser.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                        (EF.Functions.ILike(enrolledUser.UserName, $"%{model.UserName}%") || String.IsNullOrEmpty(model.UserName)) &&
                         (EF.Functions.ILike(course.Title, $"%{model.CourseName}%") || String.IsNullOrEmpty(model.CourseName)) &&
                          (enrolledUser.CourseId == model.CourseId || model.CourseId == null) && enrolledUser.ApprovalAgain == 0)
                        select new VW_Enrollment
                        {
                            CourseName = course.Title,
                            UserName = enrolledUser.UserName,
                            EnrolledId = enrolledUser.TempEnrolledId
                            // CreatedBy = updateBy == null ? updatedUserr.DisplayName : updateBy.DisplayName,
                            // CreatedOn = (int)(Category.UpdatedOn == null ? Category.CreatedOn + diff : Category.UpdatedOn.Value + diff)
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
        //All Request After Course Time Duration Completed and User Send Again Request For Approval
        public async Task<IResponse> GetAllAgainRequestsForCourseApproval(List_UserApproval model, int diff)
        {
            model.Sort = model.Sort == null || model.Sort == "" ? "UserName" : model.Sort;
            //LINQ
            var data = (from enrolledUser in _dbContext.TempEnrollment
                        join course in _dbContext.Course on enrolledUser.CourseId equals course.CourseId
                        join createdBy in _dbContext.User on enrolledUser.CreatedBy equals createdBy.UserId into updateUser
                        from updatedUserr in updateUser.DefaultIfEmpty()
                        join updatedUser in _dbContext.User on enrolledUser.UpdatedBy equals updatedUser.UserId into update
                        from updateBy in update.DefaultIfEmpty()
                        where (
                        (EF.Functions.ILike(enrolledUser.UserName, $"%{model.UserName}%") || String.IsNullOrEmpty(model.UserName)) &&
                         (EF.Functions.ILike(course.Title, $"%{model.CourseName}%") || String.IsNullOrEmpty(model.CourseName)) &&
                          (enrolledUser.CourseId == model.CourseId || model.CourseId == null) && enrolledUser.ApprovalAgain == 1)
                        select new VW_Enrollment
                        {
                            CourseName = course.Title,
                            UserName = enrolledUser.UserName,
                            EnrolledId = enrolledUser.TempEnrolledId
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
        public async Task<IResponse> ApproveUserRequest(Guid Id, Guid userId)
        {
            var usercourseapproval = await _dbContext.TempEnrollment.FirstOrDefaultAsync(x => x.TempEnrolledId == Id);
            if (usercourseapproval == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Training");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            DateTime currentDate = DateTime.UtcNow;
            var courseTimePariod = await _dbContext.Course.Where(x => x.CourseId == usercourseapproval.CourseId).FirstOrDefaultAsync();
            var enrolledUser = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.CourseId == usercourseapproval.CourseId && x.UserId == usercourseapproval.UserId && x.EnrolledStatus == "0");
            if (enrolledUser != null)
            {
                enrolledUser.TimeRestriction = currentDate.AddMonths(courseTimePariod.Duration);
                enrolledUser.Disable = 1;
                _dbContext.Enrollment.Update(enrolledUser);
                _dbContext.TempEnrollment.Remove(usercourseapproval);
                await _dbContext.SaveChangesAsync();
                _response.Success = Constants.ResponseSuccess;
                _response.Message = Constants.DataUpdate;
                return _response;
            }
            else
            {
                Enrollment enrollment = new()
                {
                    EnrolledId = Guid.NewGuid(),
                    CourseId = usercourseapproval.CourseId,
                    UserId = usercourseapproval.UserId,
                    CreatedOn = HelperStatic.GetCurrentTimeStamp(),
                    EnrolledStatus = "0",
                    CreatedBy = userId,
                    UserName = usercourseapproval.UserName,
                    Disable = 1,
                    TimeRestriction = currentDate.AddMonths(courseTimePariod.Duration)
                };
                await _dbContext.Enrollment.AddAsync(enrollment);
                _dbContext.TempEnrollment.Remove(usercourseapproval);
                await _dbContext.SaveChangesAsync();
                _response.Success = Constants.ResponseSuccess;
                return _response;
            }
        }
        public async Task<IResponse> GetAllUsers()
        {
            var users = await (from userdetail in _dbContext.UserDetail
                               join user in _dbContext.User on userdetail.UserId equals user.UserId
                               where (
                                userdetail.Role == "User")
                               select new User_List
                               {
                                   UserId = userdetail.UserId,
                                   UserName = user.Username
                               }).ToListAsync();
            if (users == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = users;
            _response.Success = Constants.ResponseSuccess;
            return _response;

        }

        public async Task<IResponse> GetLearningOverViewCount(Guid userId)
        {
            DashboardOverview count = new();
            var CompletedCourse = await _dbContext.Enrollment.Where(x => x.UserId == userId && x.EnrolledStatus == "1").ToListAsync();
            count.CompletedCoursesCount = CompletedCourse.Count;
            var OnGoingCourse = await _dbContext.Enrollment.Where(x => x.UserId == userId && x.EnrolledStatus == "0").ToListAsync();
            count.OnGoingCourseCount = OnGoingCourse.Count;
            var CertificateCount = await _dbContext.Enrollment.Where(x => x.UserId == userId && x.EnrolledStatus == "1").ToListAsync();
            count.CertificateCount = CertificateCount.Count;

            _response.Data = count;
            _response.Success = Constants.ResponseSuccess;
            _response.Message = "Count Available there";
            return _response;

        }
        public async Task<IResponse> GetAllCoursesByCategoryId(Guid Id)
        {
            var courses = await _dbContext.Course.Where(x => x.CategoryId == Id && x.Status == 1).Select(x => new { x.Title, x.CourseId }).ToListAsync();
            if (courses.Count <= 0)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = courses;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> updateEnrollmentDisableStatus(ActiveInactive model)
        {
            Enrollment enrollment = await _dbContext.Enrollment.FirstOrDefaultAsync(x => x.EnrolledId == model.Id);
            if (enrollment == null)
            {
                _response.Message = Constants.NotFound.Replace("{data}", "Enrollment");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            enrollment.Disable = model.Status;
            _dbContext.Enrollment.Update(enrollment);
            await _dbContext.SaveChangesAsync();
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
    }

}
