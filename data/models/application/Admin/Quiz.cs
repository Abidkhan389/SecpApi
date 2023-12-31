using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Paradigm.Data.Model
{
    [Table("Quiz", Schema = "Admin")]
    public class Quiz : LogFields
    {
        [Key]
        public Guid QuestionId { get; set; }
        public string Question { get; set; }
        public string Options { get; set; }
        public Guid CourseId { get; set; }
        public Guid LectureId { get; set; }
        public int TrainingType { get; set; }
        public int DifficultyLevel { get; set; }
        public int Type { get; set; }
        public int Medium { get; set; }
        public int LanguageTypes { get; set; }
        public int Status { get; set; }
        #nullable enable
        public string? Explanation { get; set; }
        #nullable disable
        public Quiz()
        {

        }
        public Quiz(VW_Quizaddedit model, Guid userId, int timestamp)
        {
            this.QuestionId = Guid.NewGuid();
            this.Question = model.Question;
            this.CourseId = model.CourseId;
            this.Options = JsonSerializer.Serialize(model.Options);
            this.CreatedBy = userId;
            this.CreatedOn = timestamp;
            this.Status = 1;
            this.Type = model.Type;
            this.Explanation = model.Explanation;
            this.TrainingType = 2;
            this.DifficultyLevel = model.DifficultyLevel;
            this.LectureId = model.LectureId;
            this.Medium = Convert.ToInt32(model.Medium);
            this.LanguageTypes = model.LanguageTypes;
        }
    }

    public class List_Quiz : TableParam
    {
        public string Question { get; set; }
        public int? QuestionType { get; set; }
        public int? DifficultyType { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? LectureId { get; set; }
        public int? Status { get; set; }
    }
    public class VW_quiz : ListingLogFields
    {
        public Guid? QuestionId { get; set; }
        public string Question { get; set; }
        public string CourseName { get; set; }
        public string LectureName { get; set; }
        public int Type { get; set; }
        //public string Lecturename {get;set;}
    }
    public class VW_Quizaddedit : LogFields
    {
        public Guid? QuestionId { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Question { get; set; }
        public List<QuestionOptions> Options { get; set; }
        public Guid CourseId { get; set; }
        public int Status { get; set; }
        [Required]
        #nullable enable
        public string? Explanation { get; set; }
        #nullable disable
        [Required]
        public int DifficultyLevel { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public Guid LectureId { get; set; }
        [Required]
        public int Medium { get; set; }
        [Required]
        public int LanguageTypes { get; set; }


    }
    public class QuestionOptions
    {
        public string OptionNo { get; set; }
        public string OptionTitle { get; set; }
        public bool IsAnswer { get; set; }
        public bool? IsAnswered { get; set; }
    }
    //View Model of question for user side
    public class VW_Questions
    {
        public Guid? QuestionId { get; set; }
        public string Question { get; set; }
        public string Explanation { get; set; }
        public List<QuestionOptions> Options { get; set; }
        public List<Answers> AnswerArr { get; set; }
        public bool LinkId { get; set; }
        public int LanguageTypes { get; set; }
        public string CourseName { get; set; }
        public string LectureName { get; set; }
        public int LectureNumber { get; set; }


    }
    public class Answers
    {
        public string Answer { get; set; }
        public bool IsChecked { get; set; }
    }
}
