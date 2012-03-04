using System;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PDateTime
    {
        public static class NowGet
        {
            public static Func<DateTime> Body
            {
                set
                {
                    var t = typeof(DateTime);
                    var info = new IndirectionInfo();
                    info.AssemblyName = t.Assembly.FullName;
                    info.TypeFullName = t.FullName;
                    info.MethodName = "get_Now";
                    if (value == null)
                    {
                        var holder = default(IndirectionHolder<Func<DateTime>>);
                        if (LooseCrossDomainAccessor.TryGet(out holder))
                        {
                            var method = default(Func<DateTime>);
                            holder.TryRemove(info, out method);
                        }
                    }
                    else
                    {
                        var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<Func<DateTime>>>();
                        holder.AddOrUpdate(info, value);
                    }
                }
            }
        }
    }
}
