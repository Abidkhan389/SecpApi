using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Paradigm.Data.Model
{

    //Category Table for categories
    [Table("Category", Schema = "Admin")]
    public class Category : LogFields
    {

        [Key]
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
        public Category()
        {

        }

        public Category(AddEditCategory model)
        {
            this.CategoryId = Guid.NewGuid();
            this.CategoryName = model.CategoryName;
            this.CreatedBy = model.CreatedBy;
            this.CreatedOn = model.CreatedOn;
            this.Status = 1;
            this.CreatedOn = model.CreatedOn;
            this.CreatedBy = model.CreatedBy;
        }

    }

    public class AddEditCategory : LogFields
    {
        public string CategoryName { get; set; }
        public Guid? CategoryId { get; set; }
        public int Status { get; set; }

    }
    public class VW_Category : ListingLogFields
    {
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
    public class List_Categories : TableParam
    {
        public string CategoryName { get; set; }
        public int? Status { get; set; }
    }
     public class VW_CategoryForCoursed
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        
    }
    public class Count_Category
    {
        public string CategoryName {get; set;}
        public Guid CategoryId {get; set;}
        public int Count {get; set;}

    }
    
}