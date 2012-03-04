
namespace Urasandesu.Prig.Framework
{
    public class GenericHolder<T> : InstanceHolder<GenericHolder<T>>
    {
        GenericHolder() { }
        public T Source { get; set; }
    }
}
