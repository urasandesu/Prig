/* 
 * File: IndirectionStub.cs
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
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Urasandesu.NAnonym.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Urasandesu.Prig.Framework.PilotStubberConfiguration
{
    public class IndirectionStub
    {
        string m_name;
        string m_alias;
        string m_xml;
        MethodBase m_target;

        internal IndirectionStub(string name, string alias, string xml, MethodBase target)
        {
            Debug.Assert(target != null);
            m_name = name;
            m_alias = alias;
            m_xml = xml;
            m_target = target;
        }

        public uint IndirectableToken { get { return (uint)m_target.MetadataToken; } }

        public string Name { get { return m_name; } }
        public string Alias { get { return m_alias; } }
        public string Xml { get { return m_xml; } }
        public MethodBase Target { get { return m_target; } }

        Type m_indDlgt;
        public Type IndirectionDelegate
        {
            get
            {
                if (m_indDlgt == null)
                    using (InstanceGetters.DisableProcessing())
                        m_indDlgt = GetIndirectionDelegateInstance(m_target, InstanceGetters.NewAdditionalDelegatesAssemblyRepository().FindAll());
                return m_indDlgt;
            }
        }

        class ExplicitThis : ParameterInfo
        {
            Type m_paramType;

            public override ParameterAttributes Attributes { get { return ParameterAttributes.None; } }
            public override Type ParameterType { get { return m_paramType; } }

            public void Set(Type paramType)
            {
                Debug.Assert(m_paramType == null);
                Debug.Assert(paramType != null);
                m_paramType = paramType;
            }
        }

        class MethodSigEqualityComparer : IEqualityComparer<MethodBase>
        {
            public bool Equals(MethodBase x, MethodBase y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(MethodBase obj)
            {
                throw new NotImplementedException();
            }

            public static bool ReturnTypeEquals(MethodBase x, MethodBase y)
            {
                var hasRetX = false;
                var _x = default(MethodInfo);
                hasRetX = (_x = x as MethodInfo) != null && _x.ReturnType != typeof(void);

                var hasRetY = false;
                var _y = default(MethodInfo);
                hasRetY = (_y = y as MethodInfo) != null && _y.ReturnType != typeof(void);

                return hasRetX == hasRetY;
            }

            public static bool ParametersEquals(IEnumerable<ParameterInfo> paramsX, IEnumerable<ParameterInfo> paramsY)
            {
                return paramsX.SequenceEqual(paramsY, new LambdaEqualityComparer<ParameterInfo>(ParameterEquals));
            }

            public static bool ParameterEquals(ParameterInfo paramX, ParameterInfo paramY)
            {
                var attrX = paramX.Attributes & (ParameterAttributes.In | ParameterAttributes.Out);
                var isByRefX = paramX.ParameterType.IsByRef;
                var attrY = paramY.Attributes & (ParameterAttributes.In | ParameterAttributes.Out);
                var isByRefY = paramY.ParameterType.IsByRef;
                return attrX == attrY && isByRefX == isByRefY;
            }
        }

        class IndirectionDelegateFinder
        {
            MethodBase m_target;

            public IndirectionDelegateFinder(MethodBase target)
            {
                m_target = target;
            }

            public bool Predicate(Type type)
            {
                Debug.Assert(type != null);
                var type_Invoke = type.GetMethod("Invoke");
                Debug.Assert(type_Invoke != null);
                var @params = new List<ParameterInfo>();
                var explicitThis = new ExplicitThis();
                if (m_target.IsStatic)
                {
                    @params.AddRange(m_target.GetParameters());
                }
                else
                {
                    var declaringType = m_target.DeclaringType;
                    if (declaringType.IsValueType)
                        declaringType = declaringType.MakeByRefType();
                    explicitThis.Set(declaringType);
                    @params.Add(explicitThis);
                    @params.AddRange(m_target.GetParameters());
                }
                return MethodSigEqualityComparer.ReturnTypeEquals(type_Invoke, m_target) &&
                       MethodSigEqualityComparer.ParametersEquals(type_Invoke.GetParameters(), @params);
            }
        }

        static Type GetIndirectionDelegateInstance(MethodBase target, IEnumerable<Assembly> indDlls)
        {
            Debug.Assert(target != null);

            var indDlgtAttrType = typeof(IndirectionDelegateAttribute);
            foreach (var indDll in indDlls)
            {
                var indDlgts = indDll.GetTypes().Where(_ => _.IsDefined(indDlgtAttrType, false));
                var indDlgt = indDlgts.FirstOrDefault(new IndirectionDelegateFinder(target).Predicate);
                if (indDlgt == null)
                    continue;

                if (!indDlgt.IsGenericType)
                {
                    return indDlgt;
                }
                else
                {
                    var genericArgs = new List<Type>();
                    if (!target.IsStatic)
                    {
                        var declaringType = target.DeclaringType;
                        if (declaringType.IsGenericType)
                            declaringType = MakeGenericExplicitThisType(declaringType);
                        genericArgs.Add(declaringType);
                    }
                    var @params = target.GetParameters();
                    foreach (var param in @params)
                    {
                        var paramType = param.ParameterType;
                        genericArgs.Add(paramType.IsByRef ? paramType.GetElementType() : paramType);
                    }
                    var _target = default(MethodInfo);
                    if ((_target = target as MethodInfo) != null && _target.ReturnType != typeof(void))
                        genericArgs.Add(_target.ReturnType);
                    var indDlgtInst = indDlgt.MakeGenericType(genericArgs.ToArray());
                    return indDlgtInst;
                }
            }

            throw new KeyNotFoundException(""); // TODO: 
        }

        static Type MakeGenericExplicitThisType(Type target)
        {
            var genericArgs = new List<Type>();
            foreach (var genericArg in target.GetGenericArguments())
                genericArgs.Add(genericArg);
            return target.MakeGenericType(genericArgs.ToArray());
        }
    }
}
