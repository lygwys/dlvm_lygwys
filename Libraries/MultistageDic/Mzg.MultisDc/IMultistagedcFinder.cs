using Mzg.Core.Context;
using Mzg.MultisDc.Domain;
using System;

namespace Mzg.MultisDc
{
    public interface IMultistagedcFinder
    {
        MultistageDc Find(Func<QueryDescriptor<MultistageDc>, QueryDescriptor<MultistageDc>> container);

        MultistageDc Find(string systemName, string className, string methodName);

        MultistageDc Find(string url);

        MultistageDc FindById(Guid id);
    }
}
