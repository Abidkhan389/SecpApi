
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Paradigm.Data.Model
{
    //Table where Course Content Added
    [Table("CourseContent", Schema = "Admin")]
    public class CourseContent : LogFields
    {
        [Key]
        public Guid ContentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public string ContentName { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        #nullable enable
        public string? Link { get; set; }
        #nullable disable
        public int Status { get; set; }
        public int Order { get; set; }
        #nullable enable
        public string? Attachments { get; set; }
        #nullable disable
        public CourseContent()
        {

        }

        public CourseContent(AddEditContent model, Guid UserId, int TimeStamp)
        {
            this.ContentId = Guid.NewGuid();
            this.ContentName = model.ContentName;
            this.CourseId = model.CourseId;
            this.LectureId = model.LectureId;
            this.Type = model.Type;
            this.Link = model.Link;
            this.Attachments = model.Attachments;
            this.Order = model.Order;
            this.Status = 1;
            this.Text = model.Text;
            this.CreatedOn = TimeStamp;
            this.CreatedBy = UserId;

        }
    }
    public class AddEditContent : LogFields
    {
        public Guid? ContentId { get; set; }
        public string ContentName { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int Order { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public string Attachments { get; set; }

    }
    public class VW_CourseForTraining
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
    }
    public class VW_LectureForCourse
    {
        public Guid LectureId { get; set; }
        public string LectureTitle { get; set; }
    }
    public class List_Content : TableParam
    {
        public string ContentName { get; set; }
        public int? Status { get; set; }
        #nullable enable
        public new int? Order { get; set; }
        public string? Type { get; set; }
        #nullable disable
        public Guid? CourseId { get; set; }
        public Guid? LectureId { get; set; }

    }
    public class VW_CourseContent : ListingLogFields
    {
        public Guid? ContentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public string CourseName { get; set; }
        public string LectureTitle { get; set; }
        public string ContentName { get; set; }
        public string Link { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }

    }
}