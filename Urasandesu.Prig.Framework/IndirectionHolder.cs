using System.Collections.Generic;

namespace Urasandesu.Prig.Framework
{
    public class IndirectionHolder<TDelegate> : InstanceHolder<IndirectionHolder<TDelegate>>
    {
        IndirectionHolder() { }

        Dictionary<string, TDelegate> m_dict = new Dictionary<string, TDelegate>();

        public bool TryGet(IndirectionInfo info, out TDelegate method)
        {
            var key = info.AssemblyName + ", " + info.TypeFullName + ", " + info.MethodName;
            if (m_dict.ContainsKey(key))
            {
                method = m_dict[key];
                return true;
            }
            else
            {
                method = default(TDelegate);
                return false;
            }
        }

        public bool TryRemove(IndirectionInfo info, out TDelegate method)
        {
            var key = info.AssemblyName + ", " + info.TypeFullName + ", " + info.MethodName;
            if (m_dict.ContainsKey(key))
            {
                method = m_dict[key];
                m_dict.Remove(key);
                return true;
            }
            else
            {
                method = default(TDelegate);
                return false;
            }
        }

        public TDelegate AddOrUpdate(IndirectionInfo info, TDelegate method)
        {
            var key = info.AssemblyName + ", " + info.TypeFullName + ", " + info.MethodName;
            if (m_dict.ContainsKey(key))
            {
                m_dict.Add(key, method);
            }
            else
            {
                m_dict[key] = method;
            }
            return method;
        }
    }
}
