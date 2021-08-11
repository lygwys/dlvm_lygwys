﻿using Mzg.Business.DataAnalyse;
using Mzg.Business.DataAnalyse.Domain;
using Mzg.Form.Domain;
using Mzg.Sdk.Abstractions.Query;
using System;
using System.Collections.Generic;

namespace Mzg.Web.Models
{
    public class RenderChartModel
    {
        public Guid QueryId { get; set; }

        public Guid ChartId { get; set; }

        public string EntityName { get; set; }
        public bool ShowTitle { get; set; }
        public Chart ChartEntity { get; set; }

        public ChartDescriptor Chart { get; set; }
        public ChartDataDescriptor ChartData { get; set; }
        public List<Schema.Domain.Attribute> Attributes { get; set; }
        public List<dynamic> DataSource { get; set; }
        public FilterExpression Filter { get; set; }
        public List<string> Groups { get; set; }
    }

    public class RenderDashBoardModel
    {
        public Guid Id { get; set; }
        public SystemForm FormEntity { get; set; }

        public bool EnabledSelector { get; set; }

        public List<SystemForm> FormList { get; set; }
    }
}