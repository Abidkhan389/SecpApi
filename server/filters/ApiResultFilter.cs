namespace Paradigm.Server.Filters
{
    using System.Threading.Tasks;    

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc.Filters;    

    using Paradigm.Server.Model;

    public class ApiResultFilter : IAsyncResultFilter
    {
        public ApiResultFilter(ILoggerFactory logger) { }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            ObjectResult obj = context.Result as ObjectResult;

            if (obj != null)
            {
                var payload = new Payload<object>()
                {
                    Data = obj.Value,
                    Message = new PayloadMessage()
                    {
                        MessageType = PayloadMessageType.Success,
                        Title = context.ActionDescriptor.AttributeRouteInfo.Template,
                        Text = context.ActionDescriptor.DisplayName
                    }
                };

                obj.Value = payload;
            }

            await next();
        }
    }
}
