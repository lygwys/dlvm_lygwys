using Microsoft.AspNetCore.Http;
using Mzg.Dependency.Abstractions;
using Mzg.Infrastructure;
using Mzg.Infrastructure.Utility;
using System;

namespace Mzg.Dependency
{
    public class XmsDependencyExceptionHandler : IExceptionHandler<XmsDependencyException>
    {
        public void Handle(HttpContext context, Exception exception)
        {
            context.Request.Body = (exception as XmsDependencyException).Dependents.SerializeToJson().ToStream();
            context.Request.Path = new PathString("/error/dependentexception");
        }
    }
}