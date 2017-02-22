/* 
 * File: AppDomainId.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2017 Akira Sugiura
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
using System.Reflection;

namespace Urasandesu.Prig.Framework
{
    static class AppDomainId
    {
        static class Holder
        {
            static Holder()
            {
                using (InstanceGetters.DisableProcessing())
                {
                    // Using AppDomainID is an easy way to synchronize the stubbed method that is in same AppDomain with profiler side.
                    // In managed code, _dummyField (CLR v2.0.50727)/_pDomain (CLR v4.0.30319) field that belongs AppDomain class indicates that.
                    // See also this QA: [How to get current application domain id?](https://social.msdn.microsoft.com/Forums/en-US/2e2bc361-8a4f-4973-b4fc-3c79adbe0351/how-to-get-current-application-domain-id?forum=netfxtoolsdev)
                    var appDomainIdField = typeof(AppDomain).GetField("_dummyField", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (appDomainIdField == null)
                        appDomainIdField = typeof(AppDomain).GetField("_pDomain", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (appDomainIdField == null)
                        throw new NotSupportedException(string.Format("This runtime 'v{0}' is not supported.", typeof(object).Assembly.ImageRuntimeVersion));

                    Value = (IntPtr)appDomainIdField.GetValue(AppDomain.CurrentDomain);
                }
            }

            public readonly static IntPtr Value;
        }

        public static IntPtr Value { get { return Holder.Value; } }
    }
}
