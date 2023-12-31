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
    public class UserProfileService : IUserProfileService
    {
        private readonly DbContextBase _dbContext;
        private readonly IResponse _response;
        public UserProfileService(DbContextBase dbContext, IResponse response)
        {
            this._dbContext = dbContext;
            this._response = response;
        }

        public async Task<IResponse> ResetPassword(ResetPassword model, AuditTrack audit, string passwordHash, string passswordSalt)
        {
            var user = await _dbContext.User.Where(x => x.UserId == model.UserId).FirstOrDefaultAsync();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passswordSalt;
            _dbContext.User.Update(user);
            await _dbContext.SaveChangesAsync();
            _response.Data = user;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

        public async Task<IResponse> UserProfile(Guid userId)
        {
            VM_UserProfile userProfile = new ();
            userProfile = await (from user in _dbContext.User
                                 join userdetails in _dbContext.UserDetail on user.UserId equals userdetails.UserId
                                 where (
                                    user.UserId == userId
                                 )
                                 select new VM_UserProfile
                                 {
                                     Name = userdetails.FirstName + " " + userdetails.LastName,
                                     CNIC = userdetails.CNIC,
                                     MobileNumber = user.MobileNumber,
                                     Address = userdetails.Address,
                                     UserName = user.Username,
                                     UserId = user.UserId
                                 }).FirstOrDefaultAsync();
            if (userProfile == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = userProfile;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }
        public async Task<IResponse> GetUserDetails(Guid userId)
        {

            var userDetails = await (from user in _dbContext.User
                                     join userdetails in _dbContext.UserDetail on user.UserId equals userdetails.UserId
                                     where (
                                        user.UserId == userId && userdetails.UserId == userId
                                     )
                                     select new EditProfile
                                     {
                                         FirstName = userdetails.FirstName,
                                         LastName = userdetails.LastName,
                                         CNIC = userdetails.CNIC,
                                         MobileNumber = user.MobileNumber,
                                         Address = userdetails.Address,
                                         Gender = userdetails.Gender,
                                         Occupation = userdetails.Occupation,
                                         HighestDegree = userdetails.HighestDegree,
                                         About = userdetails.About,
                                         University = userdetails.University,
                                         Major = userdetails.Major,
                                         DOB = userdetails.DOB,
                                         UserId = user.UserId
                                     }).FirstOrDefaultAsync();
            // var userDetails= await _dbContext.UserDetail.FirstOrDefaultAsync(x=>x.UserId == userId);

            if (userDetails == null)
            {
                _response.Message = ("No Record Exist");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }
            _response.Data = userDetails;
            _response.Success = Constants.ResponseSuccess;
            return _response;
        }

        public async Task<IResponse> GetUserCertificate(Guid UserId)
        {
            List<UserCertificates> userCertificates = new();
            userCertificates = await (from course in _dbContext.Course
                                      join enrollment in _dbContext.Enrollment on course.CourseId equals enrollment.CourseId
                                      join grade in _dbContext.UserGrade on course.CourseId equals grade.CourseId
                                      where (
                                       course.Status == 1 && enrollment.UserId == UserId && enrollment.EnrolledStatus == "1" && grade.UserId == UserId)
                                      select new UserCertificates
                                      {
                                          CourseId = course.CourseId,
                                          CourseTitle = course.Title,
                                          Icon = course.Icon,
                                          Date = grade.Date
                                      }).ToListAsync();
            var courseIds = userCertificates.Select(c => c.CourseId).ToList();
            var lectures = await _dbContext.Lecture
                                    .Where(x => courseIds.Contains(x.CourseId) && x.Status == 1)
                                    // .Select(x=> new {x.LectureTitle, x.CourseId, x.LectureId})
                                    .ToListAsync();

            foreach (var certificate in userCertificates)
            {
                certificate.LectureList = lectures.Where(l => l.CourseId == certificate.CourseId).ToList();
            }
            if (userCertificates == null || userCertificates.Count == 0)
            {
                _response.Message = ("No Certificates Yet");
                _response.Success = Constants.ResponseFailure;
                return _response;
            }

            _response.Success = Constants.ResponseSuccess;
            _response.Data = userCertificates;
            return _response;
        }


    }
}
