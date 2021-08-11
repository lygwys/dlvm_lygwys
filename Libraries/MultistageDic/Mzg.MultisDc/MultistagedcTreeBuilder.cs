using Mzg.Core.Context;
using Mzg.Infrastructure.Utility;
using Mzg.MultisDc.Domain;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Mzg.MultisDc
{
    /// <summary>
    /// 菜单树结构
    /// </summary>
    public class MultistagedcTreeBuilder : IMultistagedcTreeBuilder
    {
        private readonly IMultistagedcService _MultistagedcService;

        public MultistagedcTreeBuilder(IMultistagedcService MultistagedcService)
        {
            _MultistagedcService = MultistagedcService;
        }

        /// <summary>
        /// 获取点击页面的路径信息
        /// 如点击开发平台再点流程再点编辑后生成一个list分别为[0]开发平台[1]流程[2]编辑业务流程
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<MultistageDc> GetTreePath(string url)
        {
            if (url.IsNotEmpty())
            {
                Predicate<MultistageDc> filter = (n => n.Url.IsCaseInsensitiveEqual(url));
                return GetTreePathCore(filter);
            }
            return null;
        }

        public List<MultistageDc> GetTreePath(string areaName, string className, string methodName)
        {
            if (className.IsNotEmpty() && methodName.IsNotEmpty())
            {
                Predicate<MultistageDc> filter = (n => n.ClassName.IsCaseInsensitiveEqual(className) && n.MethodName.IsCaseInsensitiveEqual(methodName));
                if (areaName.IsNotEmpty())
                {
                    filter += (x => x.SystemName.IsCaseInsensitiveEqual(areaName));
                }
                return GetTreePathCore(filter);
            }
            return null;
        }

        private List<MultistageDc> GetTreePathCore(Predicate<MultistageDc> filter)
        {
            List<MultistageDc> result = new List<MultistageDc>();
            var all = _MultistagedcService.AllMultistagedcs;
            if (all == null)
            {
                return result;
            }
            var current = all.Find(filter);
            if (null != current)
            {
                var flag = current.Level > 1;
                result.Add(current);
                Guid parentid = current.ParentMultistagedcId;
                while (flag)
                {
                    var parent = all.Find(n => n.MultistagedcId == parentid);
                    if (parent != null)
                    {
                        result.Add(parent);
                        parentid = parent.ParentMultistagedcId;
                        if (parent.Level <= 1)
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                result.Reverse();
            }

            return result;
        }

        #region json相关

        public string Build(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container, bool nameLower = true)
        {
            List<MultistageDc> list = _MultistagedcService.Query(container);

            List<dynamic> dlist = Build(list, Guid.Empty);
            dynamic contact = new ExpandoObject();
            contact.label = "root";
            contact.id = Guid.Empty;
            contact.children = dlist;

            List<dynamic> results = new List<dynamic>();
            results.Add(contact);

            var json = results.SerializeToJson(nameLower);
            return json;
        }

        public List<dynamic> Build(List<MultistageDc> MultistagedcList, Guid parentId)
        {
            List<dynamic> dynamicList = new List<dynamic>();
            List<MultistageDc> childList = MultistagedcList.Where(n => n.ParentMultistagedcId == parentId).OrderBy(n => n.DisplayOrder).ToList();
            if (childList != null && childList.Count > 0)
            {
                List<dynamic> ddList = new List<dynamic>();
                dynamic contact = new ExpandoObject();
                foreach (var item in childList)
                {
                    contact = new ExpandoObject();
                    contact.label = item.DisplayName;
                    contact.id = item.MultistagedcId;
                    contact.url = item.Url;
                    contact.smallicon = item.SmallIcon;
                    contact.opentarget = item.OpenTarget;
                    if (MultistagedcList.Find(n => n.ParentMultistagedcId == item.MultistagedcId) != null)
                    {
                        ddList = Build(MultistagedcList, item.MultistagedcId);
                        if (ddList.Count > 0)
                        {
                            contact.children = ddList;
                            ddList = new List<dynamic>();
                        }
                    }
                    dynamicList.Add(contact);
                }
            }
            return dynamicList;
        }

        #endregion json相关
    }
}