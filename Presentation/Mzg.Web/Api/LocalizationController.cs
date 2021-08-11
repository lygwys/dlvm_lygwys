﻿using Microsoft.AspNetCore.Mvc;
using Mzg.Infrastructure.Utility;
using Mzg.Localization;
using Mzg.Localization.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 本地化标签接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class LocalizationController : ApiControllerBase
    {
        private readonly ILocalizedTextProvider _localizedTextProvider;
        private readonly ILanguageService _languageService;

        public LocalizationController(IWebAppContext appContext
            , ILanguageService languageService
            , ILocalizedTextProvider localizedTextProvider)
            : base(appContext)
        {
            _languageService = languageService;
            _localizedTextProvider = localizedTextProvider;
        }

        [Description("多语言显示标签")]
        [HttpGet("Labels")]
        public IActionResult Labels(LanguageCode language)
        {
            var result = _localizedTextProvider.Labels.Where(x => x.Language == language).OrderBy(x => x.Name);
            return JOk(result);
        }

        [Description("语言选择列表")]
        [HttpGet("Languages")]
        public IActionResult Languages()
        {
            return JOk(_languageService.FindAll());
        }

        [Description("更新多语言显示标签")]
        [HttpPost]
        public IActionResult Post([FromBody]UpdateLocalizedTextModel model)
        {
            if (model.Labels.NotEmpty())
            {
                _localizedTextProvider.Save(model.Language, model.Labels);
                return UpdateSuccess();
            }
            return UpdateFailure(GetModelErrors());
        }
    }
}