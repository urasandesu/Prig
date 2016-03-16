/* 
 * File: MyElement.cs
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


using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace program1.MyLibrary
{
    public class MyElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("alias", IsRequired = true)]
        public string Alias
        {
            get { return (string)base["alias"]; }
        }

        public MethodBase Target { get; private set; }
        public string Xml { get; private set; }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            switch (elementName)
            {
                case "RuntimeMethodInfo":
                case "RuntimeConstructorInfo":
                    var pair = DeserializeMethodBase(reader);
                    Xml = pair.Key;
                    Target = pair.Value;
                    return true;
                default:
                    return base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
        }

        static KeyValuePair<string, MethodBase> DeserializeMethodBase(XmlReader reader)
        {
            var sb = new StringBuilder();
            sb.Append(reader.ReadOuterXml());

            var s = sb.ToString();
            var ndcs = new NetDataContractSerializer();
            using (var sr = new StringReader(s))
            using (var xr = new XmlTextReader(sr))
            {
                return new KeyValuePair<string, MethodBase>(s, (MethodBase)ndcs.ReadObject(xr));
            }
        }
    }
}
