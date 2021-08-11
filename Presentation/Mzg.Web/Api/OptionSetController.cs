﻿using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Context;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Abstractions;
using Mzg.Schema.OptionSet;
using Mzg.Solution.Abstractions;
using Mzg.Web.Api.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.ComponentModel;
using System.Linq;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 选项集接口
    /// </summary>
    [Route("{org}/api/schema/optionset")]
    public class OptionSetController : ApiControllerBase
    {
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IOptionSetDetailFinder _optionSetDetailFinder;

        public OptionSetController(IWebAppContext appContext
            , IOptionSetFinder optionSetFinder
            , IOptionSetDetailFinder optionSetDetailFinder) : base(appContext)
        {
            _optionSetFinder = optionSetFinder;
            _optionSetDetailFinder = optionSetDetailFinder;
        }

        [Description("查询选项集")]
        [HttpGet]
        public IActionResult Get(bool isPublic)
        {
            var result = _optionSetFinder.Query(n => n.Where(f => f.IsPublic == isPublic));
            return JOk(result);
        }

        [Description("查询选项集选项")]
        [HttpGet("getitems/{optionsetid}")]
        public IActionResult GetItems(Guid optionsetid)
        {
            var result = _optionSetDetailFinder.Query(n => n.Where(f => f.OptionSetId == optionsetid));
            return JOk(result);
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _optionSetFinder.QueryPaged(x => x.Where(f => f.IsPublic == true).Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.OptionSetId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = OptionSetDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }
    }
}