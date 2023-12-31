using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{
    //Course Table for Courses
    [Table("Course", Schema = "Admin")]
    public class Course : LogFields
    {
        [Key]
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public Guid CategoryId { get; set; }
        public int Status { get; set; }
        public int Duration { get; set; } //Course Duration in Months
        public Course()
        {

        }
        public Course(VW_Courseaddedit model, Guid userId, int timestamp)
        {
            this.CourseId = Guid.NewGuid();
            this.Title = model.Title;
            Description = model.Description;
            Icon = model.Icon;
            Value = model.Value;
            CategoryId = model.CategoryId;
            CreatedBy = userId;
            CreatedOn = timestamp;
            Duration = model.Duration;
            Status = 1;
        }
    }

    public class VW_Courseaddedit : LogFields
    {
        public Guid? CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Icon { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public int? Status { get; set; }
        public int Duration { get; set; } //Course Duration in Months
    }
    //View Model for Course List Against category
    public class CourseList
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        #nullable enable
        public string? CategoryName { get; set; }
        public string? Icon { get; set; }
        public string? Value { get; set; }
        #nullable disable

    }
    public class VW_Course : ListingLogFields
    {
        public Guid? CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public string CategoryName { get; set; }
        public List<Lecture> LectureList { get; set; }

    }
    public class List_Courses : TableParam
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public string CategoryName { get; set; }
        public int? Status { get; set; }
    }
    //View Model for view courses with lectures list for User screen
    public class EnrolledCourses
    {
        public Guid? CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public string CategoryName { get; set; }
        public int TimeRestriction { get; set; }
        public int Disable { get; set; }
    }
    public class VW_CourseForUser
    {
        public Guid? CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
        public string CategoryName { get; set; }
        public List<Lecture> LectureList { get; set; }
        public bool? AssessmentAttempt { get; set; }

    }
    //View Model for Course List for all categories for user
    public class VW_ListOfAllCourses
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string EnrolledStatus { get; set; }
        public string AppliedStatus { get; set; }
    }
}
