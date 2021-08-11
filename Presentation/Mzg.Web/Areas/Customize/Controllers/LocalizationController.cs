using Microsoft.AspNetCore.Mvc;
using Mzg.Localization.Abstractions;
using Mzg.Solution;
using Mzg.Web.Framework.Context;
using System.ComponentModel;

namespace Mzg.Web.Customize.Controllers
{
    /// <summary>
    /// 平台多语言标签
    /// </summary>
    public class LocalizationController : CustomizeBaseController
    {
        private readonly ILocalizedTextProvider _localizedTextProvider;

        public LocalizationController(IWebAppContext appContext
            , ISolutionService solutionService
            , ILocalizedTextProvider localizedTextProvider)
            : base(appContext, solutionService)
        {
            _localizedTextProvider = localizedTextProvider;
        }

        [Description("多语言显示标签")]
        public IActionResult Index()
        {
            return View();
        }
    }
}