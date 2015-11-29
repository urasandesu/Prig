/* 
 * File: DirectoryInfoModifyingBegun.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
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

namespace Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.System.IO
{
    public struct DirectoryInfoModifyingBegun : IDisposable
    {
        DirectoryInfo m_orgInfo;
        string m_bakPath;

        public DirectoryInfoModifyingBegun(DirectoryInfo orgInfo, string bakPath)
        {
            m_orgInfo = orgInfo;
            m_bakPath = bakPath;
        }
        public DirectoryInfo Info { get { return m_orgInfo; } }

        public void Dispose()
        {
            try
            {
                if (m_orgInfo.Exists)
                    m_orgInfo.Delete(true);
            }
            catch
            { }

            try
            {
                if (Directory.Exists(m_bakPath))
                    Directory.Move(m_bakPath, m_orgInfo.FullName);
            }
            catch
            { }
        }
    }
}
