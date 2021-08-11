using System.ComponentModel;

namespace Mzg.Event.Abstractions
{
    [Description("对象删除前事件")]
    public class ObjectDeletingEvent<T> : IEvent
    {
        public ObjectDeletingEvent(T @object)
        {
            Object = @object;
        }

        public T Object { get; set; }
    }
}