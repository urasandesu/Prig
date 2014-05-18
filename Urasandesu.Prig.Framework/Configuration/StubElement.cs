/* 
 * File: StubElement.cs
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



using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Urasandesu.Prig.Framework.Configuration
{
    public class StubElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("alias")]
        public string Alias
        {
            get { return (string)base["alias"]; }
        }

        public MethodBase Target { get; private set; }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            switch (elementName)
            {
                case "RuntimeMethodInfo":
                case "RuntimeConstructorInfo":
                    Target = DeserializeMethodBase(reader);
                    return true;
                default:
                    return base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
        }

        static MethodBase DeserializeMethodBase(XmlReader reader)
        {
            var sb = new StringBuilder();
            sb.Append(reader.ReadOuterXml());

            var ndcs = new NetDataContractSerializer();
            using (var sr = new StringReader(sb.ToString()))
            using (var xr = new XmlTextReader(sr))
            {
                return (MethodBase)ndcs.ReadObject(xr);
            }
        }
    }
}
