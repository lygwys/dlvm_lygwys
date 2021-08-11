using Mzg.Core.Context;

namespace Mzg.Data.Provider
{
    public class UpdateContextBuilder
    {
        public static UpdateContext<T> Build<T>() where T : class
        {
            return new UpdateContext<T>(new ExpressionParser((entityType, name) => { return PocoHelper.FormatColumn(entityType, name); }));
        }
    }
}