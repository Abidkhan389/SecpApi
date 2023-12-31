namespace Paradigm.Server
{
    using StructureMap;

    using Microsoft.AspNetCore.Http;

    using Microsoft.Extensions.Configuration;

    using Paradigm.Data;
    using Paradigm.Server.Filters;
    using Paradigm.Server.Resolver;
    using Paradigm.Server.Interface;
    using Paradigm.Contract.Interface;
    using Paradigm.Service.Localization;
    using Paradigm.Server.Application;

    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            var targets = new ApiExceptionFilterTargets()
            {
                {
                    typeof(Service.ServiceException), PayloadMessageType.Failure
                }
            };

            For<DbContextBase>().Use<DatabaseContext>();
            For<IResponse>().Use<Response>();
            For<ICountResponse>().Use<CountResponse>();
            For<IGeneralService>().Use<GeneralService>();
            For<IAuditService>().Use<AuditService>();
            For<IMenuService>().Use<MenuService>();
            For<IUserService>().Use<UserService>();
            For<IRoleService>().Use<RoleService>();
            For<IStaticDataService>().Use<StaticDataService>();

            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
            For<IDeviceProfiler>().Use<HttpDeviceProfiler>();
            For<IHttpContextAccessor>().Use<HttpContextAccessor>().Transient();
            For<IHttpServiceContextResolver>().Use<HttpServiceContextResolver>();
            For<IDomainContextResolver>().Use<HttpServiceContextResolver>();
            For<ILocalizationResolver>().Add<LocalizationResolver>().Singleton();
            For<ILocalizationService>().Add<LocalizationService>();
            For<ICategoryService>().Add<CategoryService>();
            For<IDashboardService>().Add<DashboardService>();
            For<ICourseService>().Add<CourseService>();
            For<ICourseContentService>().Add<CourseContentService>();
            For<IQuizService>().Add<QuizService>();
            For<ILetureService>().Add<LectureService>();
            For<IEnrollmentService>().Add<EnrollmentService>();
            For<IEnrolledCourseReport>().Add<EnrolledCourseReport>();
            For<IGradeService>().Add<GradeService>();
            For<IAssessmentService>().Add<AssessmentService>();
            For<IUserProfileService>().Add<UserProfileService>();
            For<ICompletedCoursesGradeReport>().Add<CompletedCoursesGradeeport>();
            For<IGradingCriteria>().Add<GradingCriteriaService>();


            For<CultureService>();
            For<ApiResultFilter>();
            For<ApiExceptionFilter>();
            For<IdentityMappingFilter>();
            For<HttpServiceContextFactory>();
            For<ApiExceptionFilterTargets>().Use(targets);
        }
    }
}