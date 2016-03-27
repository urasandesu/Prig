/* 
 * File: PEnvironmentMixin.cs
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
using System;
using System.Prig;
using Urasandesu.Moq.Prig;
using Urasandesu.Moq.Prig.Mixins.Urasandesu.Prig.Framework;

namespace Test.program1.TestUtilities.Mixins.System.Prig
{
    public static class PEnvironmentMixin
    {
        public static MockStorage AutoBodyBy(MockStorage ms)
        {
            { var m = PEnvironment.GetCommandLineArgs().BodyBy(ms); m.Setup(_ => _()).Returns(new[] { Guid.NewGuid().ToString() }); ms.Set(PEnvironment.GetCommandLineArgs, m); }
            { var m = PEnvironment.CurrentDirectoryGet().BodyBy(ms); m.Setup(_ => _()).Returns(Guid.NewGuid().ToString()); ms.Set(PEnvironment.CurrentDirectoryGet, m); }
            return ms;
        }
    }
}
