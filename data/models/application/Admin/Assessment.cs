using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Paradigm.Data.Model
{
    [Table("Assessment", Schema = "Admin")]
    public class Assessment : LogFields
    {
        [Key]
        public Guid AssessmentId { get; set; }
        public string Question { get; set; }
        public string Options { get; set; }
        public Guid CourseId { get; set; }
        public int DifficultyLevel { get; set; }
        public int QuestionType { get; set; }
        public int Medium { get; set; }
        public int LanguageTypes { get; set; }
        public int Status { get; set; }
        #nullable enable
        public string? Explanation { get; set; }
        #nullable disable
        public Assessment()
        {

        }
        public Assessment(AddEditAssessment model, Guid userId, int timestamp)
        {
            this.AssessmentId = Guid.NewGuid();
            this.Question = model.Question;
            this.CourseId = model.CourseId;
            this.Options = JsonSerializer.Serialize(model.Options);
            this.CreatedBy = userId;
            this.CreatedOn = timestamp;
            this.Status = 1;
            this.QuestionType = model.QuestionType;
            this.Explanation = model.Explanation;
            this.DifficultyLevel = model.DifficultyLevel;
            this.Medium = Convert.ToInt32(model.Medium);
            this.LanguageTypes = model.LanguageTypes;
        }

    }
    public class List_Assessment : TableParam
    {
        public string Question { get; set; }
        public int? QuestionType { get; set; }
        public int? DifficultyType { get; set; }
        public Guid? CourseId { get; set; }
        public int? Status { get; set; }
    }
    public class VW_Assessment : ListingLogFields
    {
        public Guid? AssessmentId { get; set; }
        public string Question { get; set; }
        public string CourseName { get; set; }
        public int QuestionType { get; set; }
    }
    public class AddEditAssessment : LogFields
    {
        public Guid? AssessmentId { get; set; }
        public string Question { get; set; }
        public List<OptionsOfQuestions> Options { get; set; }
        public Guid CourseId { get; set; }
        public int Status { get; set; }
        #nullable enable
        public string? Explanation { get; set; }
        #nullable disable
        public int DifficultyLevel { get; set; }
        public int QuestionType { get; set; }
        public int Medium { get; set; }
        public int LanguageTypes { get; set; }

    }
    public class OptionsOfQuestions
    {
        public string OptionNo { get; set; }
        public string OptionTitle { get; set; }
        public bool IsAnswer { get; set; }
        public bool? IsAnswered { get; set; }
    }
    public class VW_AssessmentQuestions
    {
        public Guid? AssessmentId { get; set; }
        public string Question { get; set; }
        public string Explanation { get; set; }
        public List<OptionsOfQuestions> Options { get; set; }
        public List<AssessmentAnswers> AnswerArr { get; set; }
        public bool LinkId { get; set; }
        public int LanguageTypes { get; set; }
        public string CourseName { get; set; }
    }
    public class AssessmentAnswers
    {
        public string Answer { get; set; }
        public bool IsChecked { get; set; }
    }
}