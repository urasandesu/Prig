/* 
 * File: IndirectionHolder.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */


using System;
using System.Collections.Generic;

namespace Urasandesu.Prig.Framework
{
    public class IndirectionHolder<TDelegate> : InstanceHolder<IndirectionHolder<TDelegate>>, IDisposable
    {
        IndirectionHolder() { }

        Dictionary<string, TDelegate> m_dict = new Dictionary<string, TDelegate>();

        public bool TryGet(IndirectionInfo info, out TDelegate method)
        {
            method = default(TDelegate);
            if (!InstanceGetters.IsEnabled())
                return false;

            lock (m_dict)
            {
                var key = info + "";
                if (!m_dict.ContainsKey(key))
                    return false;

                method = m_dict[key];
                return true;
            }
        }

        public bool TryRemove(IndirectionInfo info, out TDelegate method)
        {
            method = default(TDelegate);
            if (!InstanceGetters.IsEnabled())
                return false;

            lock (m_dict)
            {
                var key = info + "";
                if (!m_dict.ContainsKey(key))
                    return false;

                method = m_dict[key];
                m_dict.Remove(key);
                return true;
            }
        }

        public TDelegate AddOrUpdate(IndirectionInfo info, TDelegate method)
        {
            if (!InstanceGetters.IsEnabled())
                return method;

            lock (m_dict)
            {
                var key = info + "";
                m_dict[key] = method; 
                return method;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (m_dict)
            {
                if (disposing)
                {
                    m_dict.Clear(); // You must not set m_dict null, because this instance will be reused usually. 
                                    // Also the normal disposable pattern is not available.
                }
            }
        }

        ~IndirectionHolder()
        {
            Dispose(false);
        }
    }
}
