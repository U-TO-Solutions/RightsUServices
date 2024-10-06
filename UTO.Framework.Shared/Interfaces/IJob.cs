using Autofac;

namespace UTO.Framework.Shared.Interfaces
{
    public interface IJob
    {
        void Initialize(IContainer container);
        void Execute();
        void CleanUp();
    }
}
