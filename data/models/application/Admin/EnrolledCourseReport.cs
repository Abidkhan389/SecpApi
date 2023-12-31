using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    public class EnrolledCourse : TableParam
    {
        public string UserName { get; set; }
        public string CNIC { get; set; }
        public string CourseTitle { get; set; }
        public string EnrolledStatus { get; set; }
    }
    public class EnrolledCourseCount : TableParam
    {
        public string CourseTitle { get; set; }
    }
    public class VW_EnrolledCourseReport : ListingLogFields
    {
        public string UserName { get; set; }
        public Guid? EnrollmentId { get; set; }

        public string CourseTitle { get; set; }
        public string CNIC { get; set; }
        public string MobileNumber { get; set; }
        public string EnrolledStatus { get; set; }
        public int TimeRestriction { get; set; }
    }
    public class CourseConunt : ListingLogFields
    {
        public string CourseName { get; set; }
        public int Count { get; set; }
    }
}
