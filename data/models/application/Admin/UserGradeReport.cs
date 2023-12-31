

using System;

namespace Paradigm.Data.Model
{
    public class CompletedCourseGrade : TableParam
    {
        public string CourseTitle { get; set; }
        public string UserName { get; set; }
        public string Grade { get; set; }
    }
    public class VW_CompletedCourseGrade : ListingLogFields
    {
        public string UserName { get; set; }
        public string CourseTitle { get; set; }
        public string Grade { get; set; }
        public Guid GradeId { get; set; }
        public string Date { get; set; }
    }
}
