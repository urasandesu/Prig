/* 
 * File: ConsoleHost.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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
using System.IO;
using System.Reflection;

namespace prig_vsix
{
    class ConsoleHost
    {
        static readonly Assembly ms_prig = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prig.dll"));
        static readonly Type ms_consoleHost = ms_prig.GetType("Urasandesu.Prig.VSPackage.Shell.ConsoleHost");
        static readonly MethodInfo ms_registerPrig = ms_consoleHost.GetMethod("RegisterPrig");
        static readonly MethodInfo ms_unregisterPrig = ms_consoleHost.GetMethod("UnregisterPrig");

        public static int RegisterPrig()
        {
            return (int)ms_registerPrig.Invoke(null, null);
        }

        public static int UnregisterPrig()
        {
            return (int)ms_unregisterPrig.Invoke(null, null);
        }
    }
}
