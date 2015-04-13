/* 
 * File: IndirectionHolder.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Urasandesu.NAnonym;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.NAnonym.Mixins.System.Reflection.Emit;
using OpCodes = Urasandesu.NAnonym.Reflection.Emit.OpCodes;

namespace Urasandesu.Prig.Framework
{
    public class IndirectionHolder<TDelegate> : InstanceHolder<IndirectionHolder<TDelegate>> where TDelegate : class
    {
        static IndirectionHolder()
        {
            var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var method in typeof(IndirectionHolder<TDelegate>).GetMethods(all))
                RuntimeHelpers.PrepareMethod(method.MethodHandle, new[] { typeof(TDelegate).TypeHandle });
        }

        IndirectionHolder() { }

        Dictionary<string, TDelegate> m_dict = InstanceGetters.DisableProcessing().EnsureDisposalThen(_ => new Dictionary<string, TDelegate>());

        public bool TryGet(IndirectionInfo info, out TDelegate method)
        {
            method = default(TDelegate);
            if (InstanceGetters.IsDisabledProcessing())
                return false;

            lock (m_dict)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    var key = info + "";
                    if (!m_dict.ContainsKey(key))
                        return false;

                    method = m_dict[key];
                    return true;
                }
            }
        }

        public TDelegate GetOrDefault(IndirectionInfo info)
        {
            var method = default(TDelegate);
            TryGet(info, out method);
            return method;
        }

        public bool Remove(IndirectionInfo info)
        {
            var method = default(TDelegate);
            return TryRemove(info, out method);
        }

        public bool TryRemove(IndirectionInfo info, out TDelegate method)
        {
            method = default(TDelegate);

            lock (m_dict)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    var key = info + "";
                    if (!m_dict.ContainsKey(key))
                        return false;

                    method = m_dict[key];
                    m_dict.Remove(key);
                    return true;
                }
            }
        }

        public TDelegate AddOrUpdate(IndirectionInfo info, TDelegate method)
        {
            lock (m_dict)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    var key = info + "";
                    m_dict[key] = method;
                    return method;
                }
            }
        }

        protected internal override void Prepare()
        {
            {
                var method = default(TDelegate);
                AddOrUpdate(IndirectionInfo.Empty, method);
            }
            {
                var method = default(TDelegate);
                TryGet(IndirectionInfo.Empty, out method);
            }
            {
                var method = default(TDelegate);
                TryRemove(IndirectionInfo.Empty, out method);
            }
            {
                GetOrDefault(IndirectionInfo.Empty);
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (m_dict)
            {
                if (disposing)
                {
                    using (InstanceGetters.DisableProcessing())
                        m_dict.Clear();
                }
            }
        }
    }

    public class IndirectionHolderUntyped
    {
        readonly object m_holder;
        readonly Definitions m_definitions;
        Dictionary<string, Work> m_dict = InstanceGetters.DisableProcessing().EnsureDisposalThen(_ => new Dictionary<string, Work>());

        public IndirectionHolderUntyped(object holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            var holderInfo = holder.GetType();
            if (!holderInfo.IsGenericType ||
                holderInfo.IsGenericTypeDefinition ||
                holderInfo.GetGenericTypeDefinition() != typeof(IndirectionHolder<>))
            {
                var msg = string.Format("The parameter must be an instance of a generic type instance of \"{0}\" but its type is \"{1}\".",
                                        typeof(IndirectionHolder<>).FullName, holderInfo.GetType().FullName);
                throw new ArgumentException(msg, "holder");
            }

            m_holder = holder;
            IndirectionDelegate = holderInfo.GetGenericArguments()[0];
            m_definitions = new Definitions(holderInfo);
        }

        public Type IndirectionDelegate { get; private set; }

        class Definitions
        {
            readonly Type m_holderInfo;

            public Definitions(Type holderInfo)
            {
                m_holderInfo = holderInfo;
            }

            MethodInfo m_tryGetIndirectionInfoTDelegateRef;
            public MethodInfo TryGetIndirectionInfoTDelegateRef
            {
                get
                {
                    if (m_tryGetIndirectionInfoTDelegateRef == null)
                        m_tryGetIndirectionInfoTDelegateRef = m_holderInfo.GetMethod("TryGet");
                    return m_tryGetIndirectionInfoTDelegateRef;
                }
            }

            MethodInfo m_tryRemoveIndirectionInfoTDelegateRef;
            public MethodInfo TryRemoveIndirectionInfoTDelegateRef
            {
                get
                {
                    if (m_tryRemoveIndirectionInfoTDelegateRef == null)
                        m_tryRemoveIndirectionInfoTDelegateRef = m_holderInfo.GetMethod("TryRemove");
                    return m_tryRemoveIndirectionInfoTDelegateRef;
                }
            }

            MethodInfo m_addOrUpdateIndirectionInfoTDelegate;
            public MethodInfo AddOrUpdateIndirectionInfoTDelegate
            {
                get
                {
                    if (m_addOrUpdateIndirectionInfoTDelegate == null)
                        m_addOrUpdateIndirectionInfoTDelegate = m_holderInfo.GetMethod("AddOrUpdate");
                    return m_addOrUpdateIndirectionInfoTDelegate;
                }
            }
        }

        public bool TryGet(IndirectionInfo info, out Work method)
        {
            method = default(Work);
            if (InstanceGetters.IsDisabledProcessing())
                return false;

            lock (m_dict)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    if (!(bool)m_definitions.TryGetIndirectionInfoTDelegateRef.Invoke(m_holder, new object[] { info, null }))
                        return false;

                    var key = info + "";
                    if (!m_dict.ContainsKey(key))
                        return false;

                    method = m_dict[key];
                    return true;
                }
            }
        }

        public Work GetOrDefault(IndirectionInfo info)
        {
            var method = default(Work);
            TryGet(info, out method);
            return method;
        }

        public bool Remove(IndirectionInfo info)
        {
            var method = default(Work);
            return TryRemove(info, out method);
        }

        public bool TryRemove(IndirectionInfo info, out Work method)
        {
            method = default(Work);

            lock (m_dict)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    if (!(bool)m_definitions.TryRemoveIndirectionInfoTDelegateRef.Invoke(m_holder, new object[] { info, null }))
                        return false;

                    var key = info + "";
                    if (!m_dict.ContainsKey(key))
                        return false;

                    method = m_dict[key];
                    m_dict.Remove(key);
                    return true;
                }
            }
        }

        public Work AddOrUpdate(IndirectionInfo info, Work method)
        {
            lock (m_dict)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<BodyOfNonPublicMethod, Dictionary<string, Work>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<BodyOfNonPublicMethod>.Make(new Dictionary<string, Work>());

                using (InstanceGetters.DisableProcessing())
                {
                    var key = info + "";
                    holder.Source.Value[key] = method;

                    var wrapAndInvoke = CreateWrapAndInvokeMethod(IndirectionDelegate, key);
                    m_definitions.AddOrUpdateIndirectionInfoTDelegate.Invoke(m_holder, new object[] { info, wrapAndInvoke });

                    m_dict[key] = method;
                    return method;
                }
            }
        }

        class BodyOfNonPublicMethod { }

        static Delegate CreateWrapAndInvokeMethod(Type indDlgt, string key)
        {
            var @string = typeof(string);
            var work = typeof(Work);
            var dictionary = typeof(Dictionary<,>);
            var bodyOfNonPublicMethod = typeof(BodyOfNonPublicMethod);
            var taggedBag = typeof(TaggedBag<,>);
            var genericHolder = typeof(GenericHolder<>);
            var looseCrossDomainAccessor = typeof(LooseCrossDomainAccessor);
            var @object = typeof(object);
            var @bool = typeof(bool);

            var dictionaryOfStringOfWork = dictionary.MakeGenericType(@string, work);
            var taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork = taggedBag.MakeGenericType(bodyOfNonPublicMethod, dictionaryOfStringOfWork);
            var genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork = genericHolder.MakeGenericType(taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork);
            var objectArr = @object.MakeArrayType();

            var looseCrossDomainAccessor_GetOrRegister = looseCrossDomainAccessor.GetMethod("GetOrRegister");
            var genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork_get_Source = genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork.GetMethod("get_Source");
            var taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork_get_Value = taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork.GetMethod("get_Value");
            var dictionaryOfStringOfWork_get_Item = dictionaryOfStringOfWork.GetMethod("get_Item");
            var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
            var work_Invoke = work.GetMethod("Invoke");

            var looseCrossDomainAccessor_GetOrRegisterOfGenericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork = looseCrossDomainAccessor_GetOrRegister.MakeGenericMethod(genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork);

            var name = string.Format("Wrap_{0}_AndInvoke_{1}", key, indDlgt);
            var method = new DynamicMethod(name, indDlgt_Invoke.ReturnType, indDlgt_Invoke.GetParameters().Select(_ => _.ParameterType).ToArray(), true);
            var gen = method.GetILGenerator();

            var reservedLocals = new List<LocalBuilder>();
            var local0_holder = gen.DeclareLocal(genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork); reservedLocals.Add(local0_holder);
            var local1_source = gen.DeclareLocal(taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork); reservedLocals.Add(local1_source);
            var local2_body = gen.DeclareLocal(work); reservedLocals.Add(local2_body);
            var local3_args = gen.DeclareLocal(objectArr); reservedLocals.Add(local3_args);

            gen.Emit(OpCodes.Call, looseCrossDomainAccessor_GetOrRegisterOfGenericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Callvirt, genericHolderOfTaggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork_get_Source);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Ldloca_S, local1_source);
            gen.Emit(OpCodes.Call, taggedBagOfBodyOfNonPublicMethodOfDictionaryOfStringOfWork_get_Value);
            gen.Emit(OpCodes.Ldstr, key);
            gen.Emit(OpCodes.Callvirt, dictionaryOfStringOfWork_get_Item);
            gen.Emit(OpCodes.Stloc_2);

            var @params = indDlgt_Invoke.GetParameters();
            var reflocals = new LocalBuilder[0];
            var refParams = @params.Where(_ => _.ParameterType.IsByRef).ToArray();
            if (0 < refParams.Length)
            {
                reflocals = new LocalBuilder[refParams.Length];
                for (int i = 0; i < refParams.Length; i++)
                {
                    var localType = refParams[i].ParameterType.ElementIfByRef();
                    reflocals[i] = gen.DeclareLocal(localType);
                    if (localType.IsValueType)
                    {
                        gen.Emit(OpCodes.Ldloca_Opt, reflocals[i].LocalIndex);
                        gen.Emit(OpCodes.Initobj, localType);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Ldnull);
                        gen.Emit(OpCodes.Stloc_Opt, reflocals[i].LocalIndex);
                    }
                }
            }

            gen.Emit(OpCodes.Ldc_I4_Opt, @params.Length);
            gen.Emit(OpCodes.Newarr, @object);
            gen.Emit(OpCodes.Stloc_3);
            {
                var refParamsIndex = 0;
                for (int i = 0; i < @params.Length; i++)
                {
                    gen.Emit(OpCodes.Ldloc_3);
                    gen.Emit(OpCodes.Ldc_I4_Opt, i);
                    if (@params[i].ParameterType.IsByRef)
                        gen.Emit(OpCodes.Ldloc_Opt, reflocals[refParamsIndex++].LocalIndex);
                    else
                        gen.Emit(OpCodes.Ldarg_S, (byte)i);
                    gen.Emit(OpCodes.Box_Opt, @params[i].ParameterType.ElementIfByRef());
                    gen.Emit(OpCodes.Stelem_Ref);
                }
            }

            gen.Emit(OpCodes.Ldloc_2);
            gen.Emit(OpCodes.Ldloc_3);
            gen.Emit(OpCodes.Callvirt, work_Invoke);

            var local_result = default(LocalBuilder);
            if (indDlgt_Invoke.ReturnType == typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }
            else
            {
                gen.Emit(OpCodes.Unbox_Opt, indDlgt_Invoke.ReturnType);
                local_result = gen.DeclareLocal(indDlgt_Invoke.ReturnType);
                gen.Emit(OpCodes.Stloc_Opt, local_result.LocalIndex);
            }

            if (0 < refParams.Length)
            {
                for (int i = 0; i < refParams.Length; i++)
                {
                    gen.Emit(OpCodes.Ldarg_S, (byte)refParams[i].Position);
                    gen.Emit(OpCodes.Ldloc_3);
                    gen.Emit(OpCodes.Ldc_I4, refParams[i].Position);
                    gen.Emit(OpCodes.Ldelem_Ref);
                    gen.Emit(OpCodes.Unbox_Opt, refParams[i].ParameterType.GetElementType());
                    if (refParams[i].ParameterType.GetElementType().IsValueType)
                        gen.Emit(OpCodes.Stobj, refParams[i].ParameterType.GetElementType());
                    else
                        gen.Emit(OpCodes.Stind_Ref);
                }
            }

            if (local_result != null)
                gen.Emit(OpCodes.Ldloc_Opt, local_result.LocalIndex);

            gen.Emit(OpCodes.Ret);

            return method.CreateDelegate(indDlgt);
        }

        public static Type MakeGenericInstance(MethodBase target, Type delegateType, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (delegateType == null)
                throw new ArgumentNullException("delegateType");

            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
                throw new ArgumentException("The parameter must be a delegate type.", "delegateType");

            if (typeGenericArgs != null && 0 < typeGenericArgs.Length)
            {
                if (!delegateType.IsGenericType)
                    throw new ArgumentException("\"delegateType\" must be a generic type if the parameter is specified.", "typeGenericArgs");

                var declType = target.DeclaringType;
                if (!declType.IsGenericType)
                    throw new ArgumentException("The declaring type of \"target\" must be a generic type if the parameter is specified.", "typeGenericArgs");

                var typeGenericDefArgs = declType.GetGenericTypeDefinition().GetGenericArguments();

                var newGenericArgs = new List<Type>();
                foreach (var genericArg in delegateType.GetGenericArguments())
                {
                    var index = Array.IndexOf(typeGenericDefArgs, genericArg);
                    newGenericArgs.Add(index < 0 ? genericArg : typeGenericArgs[index]);
                }

                delegateType = delegateType.GetGenericTypeDefinition().MakeGenericType(newGenericArgs.ToArray());
            }

            if (methodGenericArgs != null && 0 < methodGenericArgs.Length)
            {
                if (!delegateType.IsGenericType)
                    throw new ArgumentException("\"delegateType\" must be a generic type if the parameter is specified.", "methodGenericArgs");

                var method = target as MethodInfo;
                if (method == null || !method.IsGenericMethod)
                    throw new ArgumentException("\"target\" must be a generic method if the parameter is specified.", "methodGenericArgs");

                var methodGenericDefArgs = method.GetGenericMethodDefinition().GetGenericArguments();

                var newGenericArgs = new List<Type>();
                foreach (var genericArg in delegateType.GetGenericArguments())
                {
                    var index = Array.IndexOf(methodGenericDefArgs, genericArg);
                    newGenericArgs.Add(index < 0 ? genericArg : methodGenericArgs[index]);
                }

                delegateType = delegateType.GetGenericTypeDefinition().MakeGenericType(newGenericArgs.ToArray());
            }

            return delegateType;
        }

        public static Type MakeTypedType(MethodBase target, Type delegateType, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var t = typeof(IndirectionHolder<>);
            return t.MakeGenericType(MakeGenericInstance(target, delegateType, typeGenericArgs, methodGenericArgs));
        }
    }
}
