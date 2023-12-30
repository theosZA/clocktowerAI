using System.Reflection;

namespace Clocktower
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
            if (targetMethod == null)
            {
                return null;
            }

            bool isAsyncMethod = typeof(Task).IsAssignableFrom(targetMethod.ReturnType);
            if (isAsyncMethod)
            {
                var tasks = new List<Task>();
                foreach (var item in items ?? Array.Empty<T>())
                {
                    if (targetMethod.Invoke(item, args) is Task task)
                    {
                        tasks.Add(task);
                    }
                }
                return tasks.Count > 0 ? Task.WhenAll(tasks) : null;
            }

            foreach (var item in items ?? Array.Empty<T>())
            {
                targetMethod?.Invoke(item, args);
            }
            return null;
        }

        private IReadOnlyCollection<T>? items;
    }
}
