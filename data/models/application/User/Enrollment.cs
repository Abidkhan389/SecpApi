using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    [Table("Enrollment", Schema = "User")]
    public class Enrollment : LogFields
    {
        [Key]
        public Guid EnrolledId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string EnrolledStatus { get; set; }
        public string UserName { get; set; }
        public int Disable { get; set; }
        public DateTime TimeRestriction { get; set; }
        public Enrollment()
        {

        }
        public Enrollment(AddEditEnrollment model, Guid UserId, int TimeStamp, string UserName)
        {
            this.EnrolledId = Guid.NewGuid();
            this.UserName = UserName;
            this.UserId = model.UserId;
            this.EnrolledStatus = "0";
            this.CourseId = model.CourseId;
            this.CreatedOn = TimeStamp;
            this.CreatedBy = UserId;

        }
    }

    public class AddEditEnrollment : LogFields
    {
        public Guid? EnrolledId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid CategoryId { get; set; }
        public string EnrolledStatus { get; set; }


    }

    //View Model for Enroll User
    public class VW_EnrolledViewModel : LogFields
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string UserName { get; set; }
        public int ApplyAgain { get; set; }

    }
    public class DashboardOverview
    {
        public int CompletedCoursesCount { get; set; }
        public int OnGoingCourseCount { get; set; }
        public int CertificateCount { get; set; }

    }
    public class List_EnrolledUser : TableParam
    {
        public Guid? CategoryId { get; set; }
        public Guid? CourseId { get; set; }
        public string UserName { get; set; }

    }
    //For Table List to Show User Course Approval which Approved by Admin
    public class List_UserApproval : TableParam
    {
        public Guid? CourseId { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string CourseName { get; set; }

    }
    //View Model to get User List for admin Portal Erollment
    public class User_List
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

    }
    //View Model for Admin Portal Enrollment
    public class VW_Enrollment : ListingLogFields
    {
        public string CategoryName { get; set; }
        public string CourseName { get; set; }
        public string UserName { get; set; }
        public Guid EnrolledId { get; set; }
    }
    // view Model for Admin dashboard to get user count for each course
    public class CourseUserCount
    {
        public Guid CourseId { get; set; }
        public int EnrollmentCount { get; set; }
    }
    public class UserCourseCount
    {
        public Guid UserId { get; set; }
        public int EnrollmentCount { get; set; }
    }
    
    public class UserCourseCountWithDetails
    {
        public Guid UserId { get; set; }
        public int EnrollmentCount { get; set; }
        public string UserName { get; set; }
    }
     public class CoursePerUserCountWithDetails
    {
        public Guid CourseId { get; set; }
        public int EnrollmentCount { get; set; }
        public string CourseName { get; set; }
    }
    //View Model for Admin Dashboar
    public class AdminDashboard
    {
        public int LastTwoMonthsCourseCount { get; set; }
        public int LastTwoMonthsCompleteCourseCertificateCount { get; set; }
        public int LastTwoMonthsUserCtreateCount { get; set; }
        public int EnrolledUserCount { get; set; }
        public int CourseCount { get; set; }
        public string currentday {get;set;}
        public int CompletedCoursesCount { get; set; }
        public List<AdminDashboardCourses> AllCourses {get;set;}
        //public int DailyBasedEnrolledUserCount { get; set; }
       // public List<CourseUserCount> courseUserCount {get;set;}
        public List<UserCourseCountWithDetails> UserPerCourseCount { get; set; }
        public List<CoursePerUserCountWithDetails> CourseUserCountdetails { get; set; }
    }
    public class AdminDashboardCourses
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
    }


}