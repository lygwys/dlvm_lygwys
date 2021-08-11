using Microsoft.AspNetCore.Mvc;
using Mzg.Schema.Attribute;
using Mzg.Schema.Extensions;
using Mzg.Sdk.Client;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Infrastructure;
using Mzg.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mzg.Web.Api
{
    /// <summary>
    /// 实体数据合并控制器
    /// </summary>
    [Route("{org}/api/data/merge")]
    public class DataMergeController : ApiControllerBase
    {
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataMerger _dataMerger;

        public DataMergeController(IWebAppContext appContext
            , IAttributeFinder attributeFinder
            , IDataMerger dataMerger)
            : base(appContext)
        {
            _attributeFinder = attributeFinder;
            _dataMerger = dataMerger;
        }

        [Description("合并记录")]
        [HttpPost]
        public IActionResult Post([FromForm]MergeModel model)
        {
            if (!Arguments.HasValue(model.EntityId, model.RecordId1, model.RecordId2))
            {
                return JError(T["parameter_error"]);
            }
            var attributes = _attributeFinder.FindByEntityId(model.EntityId);
            Dictionary<string, Guid> attributeMaps = new Dictionary<string, Guid>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var attr in attributes)
            {
                if (!Request.Form.ContainsKey(attr.Name))
                {
                    continue;
                }

                if (attr.IsSystemControl())
                {
                    continue;
                }

                var mainAttr = Request.Form[attr.Name].ToString();
                attributeMaps.Add(attr.Name, Guid.Parse(mainAttr));
            }
            _dataMerger.Merge(model.EntityId, model.MainRecordId, model.MainRecordId.Equals(model.RecordId1) ? model.RecordId2 : model.RecordId1, attributeMaps);

            return SaveSuccess();
        }
    }
}