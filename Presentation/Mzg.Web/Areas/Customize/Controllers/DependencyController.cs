using Microsoft.AspNetCore.Mvc;
using Mzg.Dependency;
using Mzg.Dependency.Abstractions;
using Mzg.Solution;
using Mzg.Web.Customize.Controllers;
using Mzg.Web.Customize.Models;
using Mzg.Web.Framework.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Areas.Customize.Controllers
{
    /// <summary>
    /// 依赖关系控制器
    /// </summary>
    public class DependencyController : CustomizeBaseController
    {
        private readonly IDependencyLookupFactory _dependencyLookupFactory;

        public DependencyController(IWebAppContext appContext
            , ISolutionService solutionService
            , IDependencyLookupFactory dependencyLookupFactory
            ) : base(appContext, solutionService)
        {
            _dependencyLookupFactory = dependencyLookupFactory;
        }

        [Route("/error/dependentexception")]
        public IActionResult DependentComponents([FromBody]List<DependentDescriptor> dependents)
        {
            DependentComponentsModel model = new DependentComponentsModel
            {
                Items = dependents
            };
            return View(model);
        }

        [Description("查看依赖项")]
        public IActionResult CheckDependents(int requiredComponentType, Guid requiredId)
        {
            DependentComponentsModel model = new DependentComponentsModel
            {
                Items = _dependencyLookupFactory.GetDependents(requiredComponentType, requiredId)
            };
            return View("DependentComponents", model);
        }
    }
}