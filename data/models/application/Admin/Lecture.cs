using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Paradigm.Data.Model
{
    //Lecture Table for Courses
    [Table("Lecture", Schema = "Admin")]
    public class Lecture : LogFields
    {
        [Key]
        public Guid LectureId { get; set; }
        public string LectureTitle { get; set; }
        public int LectureNumber { get; set; }
        public Guid CourseId { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public Lecture()
        {

        }
        public Lecture(AddEditLecture model, Guid UserId, int TimeStamp)
        {
            this.LectureId = Guid.NewGuid();
            this.LectureTitle = model.LectureTitle;
            this.LectureNumber = model.LectureNumber;
            this.Description = model.Description;
            this.Status = 1;
            this.CourseId = model.CourseId;
            this.CreatedOn = TimeStamp;
            this.CreatedBy = UserId;

        }
        public class AddEditLecture : LogFields
        {
            public Guid? LectureId { get; set; }
            public string LectureTitle { get; set; }
            public int LectureNumber { get; set; }
            public string Description { get; set; }
            public int Status { get; set; }
            public Guid CourseId { get; set; }

        }
        public class VW_Lecture : ListingLogFields
        {
            public Guid? LectureId { get; set; }
            public string LectureTitle { get; set; }
            public int LectureNumber { get; set; }
            public string Description { get; set; }
            public string CourseName { get; set; }
            public Guid CourseId { get; set; }

        }
        public class VW_LectureWithContent
        {
            public Guid? LectureId { get; set; }
            public string LectureTitle { get; set; }
            public int LectureNumber { get; set; }
            public string Description { get; set; }
            public string CourseName { get; set; }
            public Guid CourseId { get; set; }
            public bool isattempt {get;set;}
            public List<CourseContent> CourseContents {get; set;}

        }
        public class List_Lectures : TableParam
        {
            public string LectureTitle { get; set; }
            public int? Status { get; set; }
            public Guid? CourseId { get; set; }
        }
    }
}

