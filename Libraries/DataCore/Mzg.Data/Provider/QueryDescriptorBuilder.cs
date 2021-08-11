using Mzg.Core.Context;

namespace Mzg.Data.Provider
{
    public class QueryDescriptorBuilder
    {
        public static QueryDescriptor<T> Build<T>() where T : class
        {
            return new QueryDescriptor<T>(new ExpressionParser((entityType, name) => { return PocoHelper.FormatColumn(entityType, name); }));
        }
    }
}