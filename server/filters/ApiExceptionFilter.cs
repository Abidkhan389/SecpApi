namespace Paradigm.Server.Filters
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Paradigm.Data;
    using Paradigm.Data.Model;
    using Paradigm.Server.Application;
    using Paradigm.Server.Model;

    public class ApiExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> logger;
        private readonly ApiExceptionFilterTargets targets;
        private readonly DbContextBase _context;

        public ApiExceptionFilter(ApiExceptionFilterTargets targets, ILogger<ApiExceptionFilter> logger, DbContextBase context)
        {
            this.logger = logger;
            this.targets = targets;
            this._context = context;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();

            PayloadMessageType messageType = PayloadMessageType.Error;

            if (this.targets.Keys.Contains(exceptionType))
            {
                messageType = this.targets[exceptionType];

                this.logger.LogWarning(context.Exception, $"Targetted exception of type {context.Exception.GetType().FullName} was converted to api payload with status {messageType}");
            }
            else
            {
                this.logger.LogWarning(context.Exception, $"Untargetted exception of type {context.Exception.GetType().FullName} was converted to api payload with status {messageType}");
            }

            var payload = new Payload<object>()
            {
                Data = context.Exception.StackTrace,
                Message = new PayloadMessage()
                {
                    MessageType = messageType,
                    Text = context.Exception.Message
                }
            };

            //Save Exception
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            var exception = new AppException();
            exception.Message = JsonSerializer.Serialize(payload, options);
            exception.DateTime = HelperStatic.GetCurrentTimeStamp();
            _context.AppException.Add(exception);
            _context.SaveChanges();


            context.Exception = null;
            context.Result = new JsonResult(payload);

            return Task.FromResult<bool>(true);
        }
    }
}
