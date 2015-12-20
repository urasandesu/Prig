/* 
 * File: ProfilerLocation.cs
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



using Microsoft.Win32;
using System;
using System.Text;

namespace Urasandesu.Prig.VSPackage
{
    class ProfilerLocation : IEquatable<ProfilerLocation>
    {
        public static readonly string InprocServer32Path = @"CLSID\{532C1F05-F8F3-4FBA-8724-699A31756ABD}\InprocServer32";
        public static readonly string ExpectedFileDescriptionFormat = "Prig Profiler {0} Type Library";

        public ProfilerLocation(RegistryView registryView, string pathOfInstalling)
        {
            RegistryView = registryView;
            PathOfInstalling = pathOfInstalling;
        }

        public RegistryView RegistryView { get; private set; }
        public string PathOfInstalling { get; private set; }

        public static string GetExpectedFileDescription(string ver)
        {
            return string.Format(ExpectedFileDescriptionFormat, ver);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ProfilerLocation [");
            sb.Append("RegistryView="); sb.Append(RegistryView);
            sb.Append(", "); sb.Append("PathOfInstalling="); sb.Append(PathOfInstalling);
            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<ProfilerLocation>)this).Equals(obj as ProfilerLocation);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= RegistryView.GetHashCode();
            hashCode ^= PathOfInstalling == null ? 0 : PathOfInstalling.GetHashCode();
            return hashCode;
        }

        public bool Equals(ProfilerLocation other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            return RegistryView == other.RegistryView && 
                   PathOfInstalling == other.PathOfInstalling;
        }

        public static bool operator ==(ProfilerLocation lhs, ProfilerLocation rhs)
        {
            return object.ReferenceEquals(lhs, null) ? false : lhs.Equals(rhs);
        }

        public static bool operator !=(ProfilerLocation lhs, ProfilerLocation rhs)
        {
            return !(lhs == rhs);
        }
    }
}
