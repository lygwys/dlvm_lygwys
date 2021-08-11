namespace Mzg.Event.Abstractions
{
    /// <summary>
    /// 事件接收接口
    /// 所有的事件类定义时都对应此接口，利用此接口参数说明是哪个事件中可以使用此事件类的服务并实现了
    /// HandLeEvent，可对应多个，根据参数的不同执行不同的事件
    /// 例如：UpdateEntityProcessState.cs是此接口的实现并定义了哪个事件中可以使用
    /// xmg
    /// 20200616
    /// </summary>
    /// <typeparam name="T">在哪个事件中可以使用</typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventMessage">事件</param>
        void HandleEvent(T eventMessage);
    }
}