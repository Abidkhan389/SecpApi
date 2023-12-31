
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Paradigm.Data.Model
{
    [Table("staticdata")]
    public class StaticData
    {
        public StaticData()
        {

        }
        public StaticData(AddEditStaticData addEdit)
        {
            this.StaticDataId = Guid.NewGuid();
            this.StaticDataParentId = addEdit.StaticDataParentId;
            this.ValueMember = addEdit.ValueMember;
            this.Status = 1;
        }
        [Key]
        public Guid StaticDataId { get; set; }
        public Guid StaticDataParentId { get; set; }
        public string ValueMember { get; set; }
        public int Status { get; set; }
    }
    [Table("staticdataparent")]
    public class StaticDataParent
    {
        [Key]
        public Guid StaticDataParentId { get; set; }
        public string StaticDataParentName { get; set; }
        public int Status { get; set; }
    }
    public class AddEditStaticData
    {
        public Guid? StaticDataId { get; set; }
        public Guid StaticDataParentId { get; set; }
        public string ValueMember { get; set; }
    }
    public class VW_StaticData
    {
        public int TotalCount { get; set; }
        public long SerialNo { get; set; }
        [Key]
        public Guid StaticDataId { get; set; }
        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }
        public int Status { get; set; }
    }
    public class VW_StaticDataParent
    {
        public int TotalCount { get; set; }
        public long SerialNo { get; set; }
        [Key]
        public Guid StaticDataParentId { get; set; }
        public string StaticDataParentName { get; set; }
        public int Status { get; set; }
    }
    public class AddEditStaticDataParent
    {
        public Guid? StaticDataParentId { get; set; }
        public string StaticDataParentName { get; set; }
    }
    public class MultipleStaticData
    {
        public string Parent { get; set; }
        public List<IdText> Data { get; set; }
    }
}