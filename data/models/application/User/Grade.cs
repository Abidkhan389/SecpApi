using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    [Table("UserGrade", Schema = "User")]
    public class UserGrade : LogFields
    {
        [Key]
        public Guid GradeId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string Grade { get; set; }
        public string Date { get; set; }
        public double Percentage { get; set; }

    }
    [Table("TempGrade", Schema = "User")]
    public class TempGrade : LogFields
    {
        [Key]
        public Guid TempGradeId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public string GradeTemp { get; set; }
        public double Percentage { get; set; }

    }
    public class QuizDetail
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public List<QuizAnswer> QuizAnswers { get; set; }
    }
    public class AssessmentDetail
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public List<AssessmentAns> AssessmentAns { get; set; }
    }
    public class AssessmentAns
    {
        public Guid AssessmentId { get; set; }
        public string SelectedOption { get; set; }
    }
    public class QuizAnswer
    {
        public Guid QuestionId { get; set; }
        public string SelectedOption { get; set; }

    }
    public class UserGradeDetails
    {
        public string CourseTitle { get; set; }
        public string CategoryName { get; set; }
        public double Percentage { get; set; }
        public string Date { get; set; }
        public string Grade { get; set; }
    }
    public class VM_UserReportViweModal
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string CNIC { get; set; }
        public List<CourseGradeReport> courseGradeReport { get; set; }
    }
    public class CourseGradeReport
    {
        public string CourseName { get; set; }
        public string CourseGrade { get; set; }
        public string CourseStatus { get; set; }
    }
    public class UserCertificates
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string Icon { get; set; }
        public string Date { get; set; }
        public List<Lecture> LectureList { get; set; }
    }

}
