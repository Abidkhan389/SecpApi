using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Paradigm.Contract.Interface;
using Paradigm.Data;
using Paradigm.Data.Model;
using Paradigm.Server.Interface;
using System.Globalization;
using Microsoft.VisualBasic;

namespace Paradigm.Server.Application
{
    public static class HelperStatic
    {
        private static Random random = new Random();
        private static  DateTime currentDate = DateTime.UtcNow;
        private static    DateTime twoMonthsAgo = currentDate.AddMonths(-8);
        public static int GetCurrentTimeStamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static string WhereBuilder(string search)
        {
            string extraWhere = "";
            if (!String.IsNullOrEmpty(search))
            {
                string currentSearch;
                string currentValue;
                string currentTable;
                var searchSplit = search.Split("+");
                foreach (var item in searchSplit)
                {
                    currentTable = item.Split("*")[0];
                    currentSearch = item.Split("*")[1];
                    currentValue = item.Split("*")[2];
                    if (currentSearch == "Status")
                    {
                        extraWhere = extraWhere + " " + currentTable + ".\"" + currentSearch + "\"" + " = " + currentValue + " and ";
                    }
                    else
                    {
                        extraWhere = extraWhere + " " + currentTable + ".\"" + currentSearch + "\"" + " ILIKE '%" + currentValue + "%'" + " and ";
                    }
                }
                extraWhere = extraWhere.TrimEnd(' ').TrimEnd('d').TrimEnd('n').TrimEnd('a');
            }
            return extraWhere;
        }
        public static DateTime TimestampToDate(int? timestamp)
        {
           // return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((Int32)timestamp).ToLocalTime().ToString("MMM dd, yyyy, hh:mm:ss tt", CultureInfo.InvariantCulture);
             return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(timestamp.Value)
            .ToLocalTime();
        }
        public static int CountDateforLastTwoMonths(IEnumerable<Enrollment> item)
        {
            int CertificateCount=0;
             foreach (var data in item)
            {
                // Assuming 'createdon' is a property of type int in your Enrollment class
                int? timestamp = data.CreatedOn; // Replace 'createdon' with the actual property name
                if (timestamp.HasValue)
                {
                    DateTime  courseDate = HelperStatic.TimestampToDate(timestamp);

                    if (courseDate >= twoMonthsAgo && courseDate <= currentDate)
                    {
                        CertificateCount++;
                    }
                }
            }
            return CertificateCount;
        }
         public static int CourseCountDateforLastTwoMonths(IEnumerable<Course> item)
        {
            int coursecount=0;
             foreach (var course in item)
            {
                // Assuming 'createdon' is a property of type int in your Enrollment class
                int? timestamp = course.CreatedOn; // Replace 'createdon' with the actual property name
                if (timestamp.HasValue)
                {
                    DateTime  courseDate = HelperStatic.TimestampToDate(timestamp);

                    if (courseDate >= twoMonthsAgo && courseDate <= currentDate)
                    {
                        coursecount++;
                    }
                }
            }
            return coursecount;
        }
        public static string QueryFinalize(TableParamModel model)
        {
            var where = !String.IsNullOrEmpty(model.Search) ? HelperStatic.WhereBuilder(model.Search) : null;
            string query = (!String.IsNullOrEmpty(model.Search) ? "WHERE " + where : "") +
            " ORDER BY \"" + model.Sort + "\" " + model.Order +
            " OFFSET(" + model.Start + ") ROWS " +
            " FETCH FIRST(" + model.Limit + ") ROW ONLY";
            return query;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetURL(Microsoft.AspNetCore.Http.IHeaderDictionary headers)
        {
            string orgn = headers[Constants.Referer].ToString();
            return orgn;
        }
        public static Guid GetAuditSessionIdFromClaims(ClaimsIdentity identity)
        {
            Guid sessionID = Guid.Empty;
            IEnumerable<Claim> claims = identity.Claims;
            var auditSession = claims.FirstOrDefault(e => e.Type == "AuditSession");
            if (auditSession != null)
            {
                sessionID = new Guid(auditSession.Value);
            }
            return sessionID;
        }
        public static Guid GetUserIdFromClaims(ClaimsIdentity identity)
        {
            Guid sessionID = Guid.Empty;
            IEnumerable<Claim> claims = identity.Claims;
            var auditSession = claims.FirstOrDefault(e => e.Type == "UserId");
            if (auditSession != null)
            {
                sessionID = new Guid(auditSession.Value);
            }
            return sessionID;
        }
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
    public class GeneralService : IGeneralService
    {
        private DbContextBase _dbContext;
        public GeneralService(DbContextBase dbContext)
        {
            _dbContext = dbContext;
        }
    }

}