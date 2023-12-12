using System.Reflection;

namespace Clocktower.Observer
{
    internal class ProxyCollection<T> : DispatchProxy where T : class
    {
        public static T CreateProxy(IEnumerable<T> items)
        {
            var proxy = Create<T, ProxyCollection<T>>();
            if (proxy is ProxyCollection<T> collection)
            {
                collection.items = items.ToList();
            }
            return proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            foreach (var item in items ?? Array.Empty<T>()) 
            {
                targetMethod?.Invoke(item, args);
            }
            return null;
        }

        private IReadOnlyCollection<T>? items;
    }
}
