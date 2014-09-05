/* 
 * File: IndirectionInfo.cs
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



using System;
using System.Runtime.Serialization;

namespace Urasandesu.Prig.Framework
{
    [Serializable]
    public struct IndirectionInfo : ISerializable, IEquatable<IndirectionInfo>
    {
        public static readonly IndirectionInfo Default = new IndirectionInfo() { AssemblyName = "DefaultIndirectionAssembly", Token = 0x06FFFFFF };
        public static readonly IndirectionInfo Empty = new IndirectionInfo() { AssemblyName = "EmptyIndirectionAssembly", Token = 0x06000000 };

        IndirectionInfo(SerializationInfo info, StreamingContext context)
            : this()
        {
            AssemblyName = (string)info.GetValue("AssemblyName", typeof(string));
            Token = (uint)info.GetValue("Token", typeof(uint));
        }


        public string AssemblyName { get; set; }
        public uint Token { get; set; }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AssemblyName", AssemblyName, typeof(string));
            info.AddValue("Token", Token, typeof(uint));
        }

        public override string ToString()
        {
            return "IndirectionInfo [AssemblyName=" + AssemblyName + ", Token=" + Token + "]";
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<IndirectionInfo>)this).Equals(obj as IndirectionInfo?);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            hashCode ^= AssemblyName == null ? 0 : AssemblyName.GetHashCode();
            hashCode ^= Token.GetHashCode();
            return hashCode;
        }

        public bool Equals(IndirectionInfo other)
        {
            return AssemblyName == other.AssemblyName && Token == other.Token;
        }

        public static bool operator ==(IndirectionInfo lhs, IndirectionInfo rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(IndirectionInfo lhs, IndirectionInfo rhs)
        {
            return !(lhs == rhs);
        }
    }
}
