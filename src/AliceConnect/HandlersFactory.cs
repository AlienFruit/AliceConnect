using AlienFruit.CommandLine.Abstractions;
using AlienFruit.Core.Abstractions;

namespace AliceConnect
{
    public class HandlersFactory(IServiceProvider serviceProvider) : IHandlersFactory
    {
        private readonly IServiceProvider serviceProvider = serviceProvider;

        public Func<ICommand, Task> GetAsyncHandler(Type commandType)
        {
            var item = this.serviceProvider.GetService(typeof(IHandler<>).MakeGenericType(commandType)) as IHandler
                ?? throw new ArgumentNullException($"Cannot find handler for command {commandType}");
            return item.HandleAsync;
        }

        public Func<ICommand, Task<object>> GetAsyncHandlerWithResult(Type messageType)
        {
            return null;
        }

        public Action<ICommand> GetHandler(Type commandType)
        {
            return null;
        }

        public Func<ICommand, object> GetHandlerWithResult(Type commandType)
        {
            return null;
        }
    }
}
