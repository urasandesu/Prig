/* 
 * File: PrigPackageResources.cs
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



using System.Globalization;
using System.Resources;

namespace Urasandesu.Prig.VSPackage
{
    class PrigPackageResources 
    {        
        static ResourceManager m_resourceManager;        

        public PrigPackageResources()
        { }
        
        public static ResourceManager ResourceManager 
        {
            get 
            {
                if (object.ReferenceEquals(m_resourceManager, null))
                    m_resourceManager = new ResourceManager("Urasandesu.Prig.VSPackage.PrigPackageResources", typeof(PrigPackageResources).Assembly);
                return m_resourceManager;
            }
        }

        public static CultureInfo Culture { get; set; }

        public static string GetString(string name)
        {
            return ResourceManager.GetString(name, Culture);
        }
    }
}
