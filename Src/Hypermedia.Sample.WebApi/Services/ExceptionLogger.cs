using System;
using System.Web.Http.ExceptionHandling;

namespace Hypermedia.Sample.WebApi.Services
{
    public sealed class ExceptionLogger : System.Web.Http.ExceptionHandling.ExceptionLogger
    {
        /// <summary>
        /// When overridden in a derived class, logs the exception synchronously.
        /// </summary>
        /// <param name="context">The exception logger context.</param>
        public override void Log(ExceptionLoggerContext context)
        {
            var aggregateException = context.Exception as AggregateException;

            if (aggregateException != null)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    // TODO
                }

                return;
            }

            // TODO
        }
    }
}
