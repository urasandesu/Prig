/* 
 * File: PrigConfig.cs
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



using System.Xml.Serialization;

namespace Urasandesu.Prig.Framework
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "Config")]
    public class PrigConfig
    {
        PrigPackageConfigs m_packages;
        [XmlElementAttribute("Packages", IsNullable = false)]
        public PrigPackageConfigs Packages 
        {
            get
            {
                if (m_packages == null)
                    m_packages = new PrigPackageConfigs();
                return m_packages;
            }
            set { m_packages = value; }
        }
        [XmlAttributeAttribute]
        public byte class_id { get; set; }
        [XmlAttributeAttribute]
        public byte tracking_level { get; set; }
        [XmlAttributeAttribute]
        public byte version { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class PrigPackageConfigs
    {
        public byte count { get; set; }
        public byte item_version { get; set; }
        PrigPackageConfig[] m_item;
        [XmlElementAttribute("item", IsNullable = false)]
        public PrigPackageConfig[] item 
        {
            get
            {
                if (m_item == null)
                    m_item = new PrigPackageConfig[0];
                return m_item;
            }
            set { m_item = value; }
        }
        [XmlAttributeAttribute]
        public byte class_id { get; set; }
        [XmlAttributeAttribute]
        public byte tracking_level { get; set; }
        [XmlAttributeAttribute]
        public byte version { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class PrigPackageConfig
    {
        public string Name { get; set; }
        PrigPathConfig m_source;
        [XmlElementAttribute("Source", IsNullable = false)]
        public PrigPathConfig Source 
        {
            get
            {
                if (m_source == null)
                    m_source = new PrigPathConfig();
                return m_source;
            }
            set { m_source = value; }
        }
        PrigAdditionalDelegateConfigs m_additionalDelegates;
        [XmlElementAttribute("AdditionalDelegates", IsNullable = false)]
        public PrigAdditionalDelegateConfigs AdditionalDelegates 
        {
            get
            {
                if (m_additionalDelegates == null)
                    m_additionalDelegates = new PrigAdditionalDelegateConfigs();
                return m_additionalDelegates;
            }
            set { m_additionalDelegates = value; }
        }
        [XmlAttributeAttribute]
        public byte class_id { get; set; }
        [XmlIgnoreAttribute]
        public bool class_idSpecified { get; set; }
        [XmlAttributeAttribute]
        public byte tracking_level { get; set; }
        [XmlIgnoreAttribute]
        public bool tracking_levelSpecified { get; set; }
        [XmlAttributeAttribute]
        public byte version { get; set; }
        [XmlIgnoreAttribute]
        public bool versionSpecified { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class PrigAdditionalDelegateConfigs
    {
        public byte count { get; set; }
        public byte item_version { get; set; }
        PrigAdditionalDelegateConfig[] m_item;
        [XmlElementAttribute("item", IsNullable = false)]
        public PrigAdditionalDelegateConfig[] item 
        {
            get
            {
                if (m_item == null)
                    m_item = new PrigAdditionalDelegateConfig[0];
                return m_item;
            }
            set { m_item = value; }
        }
        [XmlAttributeAttribute]
        public byte class_id { get; set; }
        [XmlIgnoreAttribute]
        public bool class_idSpecified { get; set; }
        [XmlAttributeAttribute]
        public byte tracking_level { get; set; }
        [XmlIgnoreAttribute]
        public bool tracking_levelSpecified { get; set; }
        [XmlAttributeAttribute]
        public byte version { get; set; }
        [XmlIgnoreAttribute]
        public bool versionSpecified { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class PrigAdditionalDelegateConfig
    {
        public string FullName { get; set; }
        PrigPathConfig m_hintPath;
        [XmlElementAttribute("HintPath", IsNullable = false)]
        public PrigPathConfig HintPath 
        {
            get
            {
                if (m_hintPath == null)
                    m_hintPath = new PrigPathConfig();
                return m_hintPath;
            }
            set { m_hintPath = value; }
        }
        [XmlAttributeAttribute]
        public byte class_id { get; set; }
        [XmlAttributeAttribute]
        public byte tracking_level { get; set; }
        [XmlAttributeAttribute]
        public byte version { get; set; }
    }

    [XmlTypeAttribute(AnonymousType = true)]
    public class PrigPathConfig
    {
        string m_native;
        [XmlElementAttribute("Native", IsNullable = false)]
        public string Native 
        {
            get
            {
                if (string.IsNullOrEmpty(m_native))
                    m_native = "*";
                return m_native;
            }
            set { m_native = value; }
        }
    }
}
