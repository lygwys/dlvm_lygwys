using Mzg.Core.Context;
using Mzg.MultisDc.Domain;
using System;
using System.Collections.Generic;

namespace Mzg.MultisDc
{
    public interface IMultistagedcTreeBuilder
    {
        string Build(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container, bool nameLower = true);

        List<dynamic> Build(List<MultistageDc> privilegeList, Guid parentId);

        List<MultistageDc> GetTreePath(string url);

        List<MultistageDc> GetTreePath(string areaName, string className, string methodName);
    }
}