using System;
using System.Globalization;

namespace Paradigm.Data.Model
{
    public class TableParamModel
    {
        public int Start { get; set; }
        public int? Limit { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public string Search { get; set; }
    }
    public class ActiveInactive
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }
    public class ActiveInactiveBool
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
    }
    public class ActiveInactiveRole
    {
        public string Id { get; set; }
        public bool Status { get; set; }
    }
    public class Education
    {
        public string Degree { get; set; }
        public int PassingYear { get; set; }
        public string Institution { get; set; }
        public string Specialization { get; set; }
    }
    public class Experience
    {
        public string Organization { get; set; }
        public string Designation { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public string Description { get; set; }
    }
    public class IdText
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
    public class LogFields
    {
        public Guid? CreatedBy { get; set; }
        public int? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public int? UpdatedOn { get; set; }
    }
    public class ListingLogFields
    {
        public int TotalCount { get; set; }
        public long SerialNo { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedOn { get; set; }
        public string CreatedDate
        {
            get
            {
                Int32 dateTime = Convert.ToInt32(CreatedOn);
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(dateTime);
                return dtDateTime.ToString("MMM dd, yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
        }
    }
     public class TableParam
    {
        public int Start { get; set; }
        public int Limit { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public int LimitEx
        {
            get
            {
                return this.Limit == 0 ? 10 : this.Limit;
            }
        }
        public string SortEx
        {
            get
            {
                string ret = this.Sort;
                if (String.IsNullOrEmpty(ret) || String.IsNullOrEmpty(ret))
                {
                    ret = "CreatedOn";
                }
                ret = ret == "createdDate" || ret == "serialNo" ? "CreatedOn" : ret;
                ret = $"{ret[0].ToString().ToUpper()}{ret.Substring(1)}";
                return ret;
            }
        }
        public string OrderEx
        {
            get
            {
                return String.IsNullOrEmpty(this.Order) || String.IsNullOrEmpty(this.Sort) ? "desc" : this.Order;
            }
        }
    }

   
}