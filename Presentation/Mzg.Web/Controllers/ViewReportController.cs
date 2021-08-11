using Microsoft.AspNetCore.Mvc;
using Mzg.Core.Data;
using Mzg.Data;
using Mzg.Infrastructure.Utility;
using Mzg.Schema.Attribute;
using Mzg.Schema.Entity;
using Mzg.Sdk.Abstractions;
using Mzg.Web.Customize.Models;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mzg.Web.Controllers
{
    public class ViewReportController : WebControllerBase
    {
        private readonly DataRepositoryBase<dynamic> _repository;
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDbContext _dbContext;
        public ViewReportController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeFinder
            , IDbContext dbContext
            )
            : base(appContext)
        {
            _dbContext = dbContext;
            _entityFinder = entityService;
            _attributeFinder = attributeFinder;
            _repository = new DataRepositoryBase<dynamic>(_dbContext);
        }
        public QueryParameters Parameters { get; set; } = new QueryParameters();
        public IActionResult Index(Guid? id, string reportName, Guid attributeid1, Guid attributeid2, Guid attributeid3, Guid attributeid4, Guid attributeid5, Guid attributeid6, Guid attributeid7, Guid attributeid8, Guid attributeid9, Guid attributeid10, Guid entityid)
        {
            XtraReportModel model = new XtraReportModel();
            StringBuilder sqlString = new StringBuilder();
            model.id = id;
            model.reportName = reportName;
            if (!entityid.IsEmpty())
                model.EntityId = Guid.Parse(entityid.ToString());
            string entityName = entityid.IsEmpty() ? "" : _entityFinder.FindById(entityid).Name;

            /*********************第一个参数的值***********************************/
            string attributName = attributeid1.IsEmpty() ? "" : _attributeFinder.FindById(attributeid1).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p1 = value != null ? value.ToString() : "";
                }

            }

            /*********************第2个参数的值***********************************/
            attributName = attributeid2.IsEmpty() ? "" : _attributeFinder.FindById(attributeid2).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p2 = value != null ? value.ToString() : "";
                }

            }

            /*********************第3个参数的值***********************************/
            attributName = attributeid3.IsEmpty() ? "" : _attributeFinder.FindById(attributeid3).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p3 = value != null ? value.ToString() : "";
                }

            }

            /*********************第4个参数的值***********************************/
            attributName = attributeid4.IsEmpty() ? "" : _attributeFinder.FindById(attributeid4).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p4 = value != null ? value.ToString() : "";
                }

            }

            /*********************第5个参数的值***********************************/
            attributName = attributeid5.IsEmpty() ? "" : _attributeFinder.FindById(attributeid5).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p5 = value != null ? value.ToString() : "";
                }

            }


            /*********************第6个参数的值***********************************/
            attributName = attributeid6.IsEmpty() ? "" : _attributeFinder.FindById(attributeid6).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p6 = value != null ? value.ToString() : "";
                }

            }

            /*********************第7个参数的值***********************************/
            attributName = attributeid7.IsEmpty() ? "" : _attributeFinder.FindById(attributeid7).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p7 = value != null ? value.ToString() : "";
                }

            }

            /*********************第8个参数的值***********************************/
            attributName = attributeid8.IsEmpty() ? "" : _attributeFinder.FindById(attributeid8).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p8 = value != null ? value.ToString() : "";
                }

            }


            /*********************第9个参数的值***********************************/
            attributName = attributeid9.IsEmpty() ? "" : _attributeFinder.FindById(attributeid9).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p9 = value != null ? value.ToString() : "";
                }

            }


            /*********************第10个参数的值***********************************/
            attributName = attributeid10.IsEmpty() ? "" : _attributeFinder.FindById(attributeid10).Name;
            if (!attributName.Equals("") && !entityName.Equals(""))
            {
                Parameters.Args.Clear();
                Parameters.Args.Add(id);
                sqlString.AppendFormat("select " + attributName + " from " + entityName + " where " + entityName + "Id=@0");
                var attribValue = _repository.ExecuteQuery(sqlString.ToString(), Parameters.Args.ToArray());
                if (attribValue.NotEmpty())
                {
                    var value = (attribValue[0] as IDictionary<string, object>).Values.First();
                    model.p10 = value != null ? value.ToString() : "";
                }

            }

            return View(model);
        }
    }
}