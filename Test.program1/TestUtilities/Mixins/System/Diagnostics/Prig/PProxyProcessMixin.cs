/* 
 * File: PProxyProcessMixin.cs
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


using Moq;
using System.Diagnostics;
using System.Diagnostics.Prig;
using Urasandesu.Moq.Prig;
using Urasandesu.Moq.Prig.Mixins.Urasandesu.Prig.Framework;

namespace Test.program1.TestUtilities.Mixins.System.Diagnostics.Prig
{
    public static class PProxyProcessMixin
    {
        public static MockStorage AutoBodyBy(this PProxyProcess proc, MockStorage ms)
        {
            { var m = proc.StartInfoGet().BodyBy(ms); m.Setup(_ => _(It.IsAny<Process>())).Returns(new ProcessStartInfo()); ms.Set(proc.StartInfoGet, m); }
            { var m = proc.MainModuleGet().BodyBy(ms); m.Setup(_ => _(It.IsAny<Process>())).Returns(new PProxyProcessModule()); ms.Set(proc.MainModuleGet, m); }
            { var m = proc.CloseMainWindow().BodyBy(ms); m.Setup(_ => _(It.IsAny<Process>())).Returns(true); ms.Set(proc.CloseMainWindow, m); }
            return ms;
        }
    }
}