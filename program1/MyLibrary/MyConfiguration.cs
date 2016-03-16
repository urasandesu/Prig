/* 
 * File: MyConfiguration.cs
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
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace program1.MyLibrary
{
    public class MyConfiguration
    {
        public string Assembly { get; set; }
        public string AssemblyFrom { get; set; }
        public string Settings { get; set; }

        public MySection LoadFromSettings()
        {
            var asmInfo = GetAssemblyInfo();

            var fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Settings;
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var section = (MySection)config.GetSection("my");
            ValidateSection(asmInfo, section);

            return section;
        }

        public MySection LoadFromAppConfig()
        {
            var asmInfo = GetAssemblyInfo();

            var section = (MySection)ConfigurationManager.GetSection("my");
            ValidateSection(asmInfo, section);

            return section;
        }

        Assembly GetAssemblyInfo()
        {
            var asmInfo = default(Assembly);
            if (!string.IsNullOrEmpty(Assembly))
                asmInfo = System.Reflection.Assembly.Load(Assembly);
            else if (!string.IsNullOrEmpty(AssemblyFrom))
                asmInfo = System.Reflection.Assembly.LoadFrom(AssemblyFrom);
            if (asmInfo == null)
                throw new InvalidOperationException("The parameter 'Assembly' or 'AssemblyFrom' is mandatory.");
            return asmInfo;
        }

        static void ValidateSection(Assembly asmInfo, MySection section)
        {
            var unintendedSettings = section.MyElements.OfType<MyElement>().Where(_ => _.Target.Module.Assembly != asmInfo).ToArray();
            if (0 < unintendedSettings.Length)
                throw new InvalidOperationException(
                    string.Format("Now the indirection settings for \"{0}\" is being analysed, but the settings for \"{1}\" was detected.",
                                  asmInfo.FullName, unintendedSettings[0].Target.Module.Assembly.FullName));
        }
    }
}
