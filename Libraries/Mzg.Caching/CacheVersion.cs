using System.Collections.Generic;

namespace Mzg.Caching
{
    public class CacheVersion<T>
    {
        public int Version { get; set; }
        public List<T> Items { get; set; }
    }
}