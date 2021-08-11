namespace Mzg.Plugin.Abstractions
{
    public interface IEntityPlugin : IAbsPlugin
    {
        void Execute(PluginExecutionContext context);
    }
}