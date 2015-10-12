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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Urasandesu.NAnonym;
using Urasandesu.NAnonym.Collections.Generic;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.NAnonym.Mixins.System.Reflection;
using Urasandesu.NAnonym.Mixins.System.Reflection.Emit;
using OpCodes = Urasandesu.NAnonym.Reflection.Emit.OpCodes;

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

        public string IndirectableAssemblyName { get { return m_target.Module.Assembly.FullName; } }
        public uint IndirectableToken { get { return (uint)m_target.MetadataToken; } }

        public string Name { get { return m_name; } }
        public string Alias { get { return m_alias; } }
        public string Xml { get { return m_xml; } }
        public MethodBase Target { get { return m_target; } }

        bool m_indDlgtInit;
        Type m_indDlgt;
        public Type IndirectionDelegate
        {
            get
            {
                if (!m_indDlgtInit)
                {
                    using (InstanceGetters.DisableProcessing())
                        m_indDlgt = GetIndirectionDelegateInstance(m_target, InstanceGetters.NewAdditionalDelegatesAssemblyRepository().FindAll());
                    m_indDlgtInit = true;
                }
                return m_indDlgt;
            }
        }

        Type[] m_sig;
        public Type[] Signature 
        {
            get
            {
                if (m_sig == null)
                    using (InstanceGetters.DisableProcessing())
                        m_sig = GetIndirectionDelegateSignature(m_target);
                return m_sig;
            }
        }

        class StorageToDefineRunnableTypes
        {
            public static Dictionary<string, Func<IndirectionBehaviors, Delegate>> ms_defaultBehaviorFactories = new Dictionary<string, Func<IndirectionBehaviors, Delegate>>();
            public static Dictionary<string, Func<Delegate, Dictionary<object, TargetSettingValue<Delegate>>, Delegate>> ms_behaviorSelectorFactories = new Dictionary<string, Func<Delegate, Dictionary<object, TargetSettingValue<Delegate>>, Delegate>>();
            public static Dictionary<MethodBase, Type> ms_indDlgtDefCache = new Dictionary<MethodBase, Type>();
            public static Dictionary<MethodBase, Type> ms_behaviorSelectorImplCache = new Dictionary<MethodBase, Type>();
            public static Dictionary<MethodBase, Type> ms_defaultBehaviorImplCache = new Dictionary<MethodBase, Type>();
            public static Dictionary<Assembly, ModuleBuilder> ms_modBldrCache = new Dictionary<Assembly, ModuleBuilder>();

            static ModuleBuilder NewTemporaryModuleBuilder()
            {
                var asmName = new AssemblyName(Guid.NewGuid().ToString("N"));
                var asmBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
                return asmBldr.DefineDynamicModule(asmName.Name + ".dll");
            }

            public static TemporaryModuleBuilderPrepared PrepareTemporaryModuleBuilder(Assembly asm, out ModuleBuilder modBldr)
            {
                lock (ms_modBldrCache)
                {
                    if (!ms_modBldrCache.ContainsKey(asm))
                        ms_modBldrCache.Add(asm, NewTemporaryModuleBuilder());
                    modBldr = ms_modBldrCache[asm];
                }

                Monitor.Enter(modBldr);
                return new TemporaryModuleBuilderPrepared(modBldr);
            }

            public struct TemporaryModuleBuilderPrepared : IDisposable
            {
                ModuleBuilder m_modBldr;

                public TemporaryModuleBuilderPrepared(ModuleBuilder modBldr)
                {
                    Debug.Assert(modBldr != null);
                    m_modBldr = modBldr;
                }

                public void Dispose()
                {
                    Monitor.Exit(m_modBldr);
                }
            }
        }

        public Delegate CreateDelegateOfDefaultBehavior(IndirectionBehaviors defaultBehavior, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            using (InstanceGetters.DisableProcessing())
            {
                var info = new IndirectionInfo();
                info.AssemblyName = Target.Module.Assembly.FullName;
                info.Token = (uint)Target.MetadataToken;
                info.SetInstantiation(Target, Signature, typeGenericArgs, methodGenericArgs);

                var key = info + "";
                var defaultBehaviorFactories = StorageToDefineRunnableTypes.ms_defaultBehaviorFactories;
                lock (defaultBehaviorFactories)
                {
                    if (!defaultBehaviorFactories.ContainsKey(key))
                        defaultBehaviorFactories.Add(key, DefineDefaultBehaviorFactory(m_target, typeGenericArgs, methodGenericArgs));
                    return defaultBehaviorFactories[key](defaultBehavior);
                }
            }
        }

        public Work CreateWorkOfDefaultBehavior(IndirectionBehaviors defaultBehavior, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            using (InstanceGetters.DisableProcessing())
            {
                var indDlgt = GetIndirectionDelegate(typeGenericArgs, methodGenericArgs);
                var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
                var @params = indDlgt_Invoke.GetParameters();

                return new Work(args =>
                {
                    if (args == null)
                        throw new ArgumentNullException("args");

                    if (defaultBehavior == IndirectionBehaviors.DefaultValue)
                    {
                        if (args.Length != @params.Length)
                        {
                            var msg = string.Format("This default behavior is initialized as {0} parameter(s) method but it is called as {1} parameter(s) method.",
                                                    @params.Length, args.Length);
                            throw new ArgumentException(msg, "args");
                        }

                        for (int i = 0; i < @params.Length; i++)
                        {
                            if (@params[i].ParameterType.IsByRef)
                            {
                                var paramType = @params[i].ParameterType.GetElementType();
                                args[i] = paramType.GetDefaultValue();
                            }
                        }

                        return indDlgt_Invoke.ReturnType.GetDefaultValue();
                    }

                    if (defaultBehavior == IndirectionBehaviors.Fallthrough)
                    {
                        using (InstanceGetters.DisableProcessing())
                            throw new FallthroughException();
                    }

                    using (InstanceGetters.DisableProcessing())
                        throw new NotImplementedException();
                });
            }
        }

        public Delegate CreateDelegateExecutingDefaultOr(Delegate defaultBehavior, Dictionary<object, TargetSettingValue<Delegate>> indirections, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            using (InstanceGetters.DisableProcessing())
            {
                if (Target.IsStatic)
                    throw new InvalidOperationException("This method can only be called if the target method is an instance member.");

                var info = new IndirectionInfo();
                info.AssemblyName = Target.Module.Assembly.FullName;
                info.Token = (uint)Target.MetadataToken;
                info.SetInstantiation(Target, Signature, typeGenericArgs, methodGenericArgs);

                var key = info + "";
                var behaviorSelectorFactories = StorageToDefineRunnableTypes.ms_behaviorSelectorFactories;
                lock (behaviorSelectorFactories)
                {
                    if (!behaviorSelectorFactories.ContainsKey(key))
                        behaviorSelectorFactories.Add(key, DefineBehaviorSelectorFactory(m_target, typeGenericArgs, methodGenericArgs));
                    return behaviorSelectorFactories[key](defaultBehavior, indirections);
                }
            }
        }

        public Work CreateWorkExecutingDefaultOr(Work defaultBehavior, Dictionary<object, TargetSettingValue<Work>> indirections, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            using (InstanceGetters.DisableProcessing())
            {
                if (Target.IsStatic)
                    throw new InvalidOperationException("This method can only be called if the target method is an instance member.");


                var indDlgt = GetIndirectionDelegate(typeGenericArgs, methodGenericArgs);
                var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
                var @params = indDlgt_Invoke.GetParameters();
                if (@params.Length < 1)
                    throw new ArgumentException("The delegate type must have one or more parameters.", "indDlgt");

                return new Work(args =>
                {
                    if (args == null)
                        throw new ArgumentNullException("args");

                    if (args.Length != @params.Length)
                    {
                        var msg = string.Format("This default behavior is initialized as {0} parameter(s) method but it is called as {1} parameter(s) method.",
                                                @params.Length, args.Length);
                        throw new ArgumentException(msg, "args");
                    }

                    if (object.ReferenceEquals(args[0], null) || !indirections.ContainsKey(args[0]))
                    {
                        var result = defaultBehavior(args);
                        return result;
                    }
                    else
                    {
                        var result = indirections[args[0]].Indirection(args);
                        return result;
                    }
                });
            }
        }

        public Type GetDeclaringTypeInstance(Type[] typeGenericArgs)
        {
            var declaringType = Target.DeclaringType;
            return !declaringType.IsGenericType ? declaringType : declaringType.MakeGenericType(typeGenericArgs);
        }

        static Func<IndirectionBehaviors, Delegate> DefineDefaultBehaviorFactory(MethodBase target, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var indDlgt = GetIndirectionDelegate(target, typeGenericArgs, methodGenericArgs);
            var defaultBehaviorImpl = GetDefaultBehaviorImplementation(target, typeGenericArgs, methodGenericArgs);

            var defaultBehaviorImpl_defaultBehavior = defaultBehaviorImpl.GetFields().First();

            var indDlgt_ctor = indDlgt.GetConstructors().First();
            var defaultBehaviorImpl_ctor = defaultBehaviorImpl.GetConstructors().First();
            var defaultBehaviorImpl_Invoke = defaultBehaviorImpl.GetMethods().First();

            var factory = new DynamicMethod("Factory", typeof(Delegate), new[] { typeof(IndirectionBehaviors) }, typeof(InstanceGetters).Module, true);
            var gen = factory.GetILGenerator();
            
            var local0_impl = gen.DeclareLocal(defaultBehaviorImpl);

            gen.Emit(OpCodes.Newobj, defaultBehaviorImpl_ctor);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stfld, defaultBehaviorImpl_defaultBehavior);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldftn, defaultBehaviorImpl_Invoke);
            gen.Emit(OpCodes.Newobj, indDlgt_ctor);
            gen.Emit(OpCodes.Ret);

            return (Func<IndirectionBehaviors, Delegate>)factory.CreateDelegate(typeof(Func<IndirectionBehaviors, Delegate>));
        }

        public Type GetIndirectionDelegate(Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            return GetIndirectionDelegate(Target, typeGenericArgs, methodGenericArgs);
        }

        static Type GetIndirectionDelegateDefinition(MethodBase target)
        {
            var indDlgtDefCache = StorageToDefineRunnableTypes.ms_indDlgtDefCache;
            lock (indDlgtDefCache)
            {
                if (!indDlgtDefCache.ContainsKey(target))
                    indDlgtDefCache.Add(target, DefineIndirectionDelegate(target));

                return indDlgtDefCache[target];
            }
        }

        static Type GetIndirectionDelegate(MethodBase target, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var indDlgtDef = GetIndirectionDelegateDefinition(target);
            return !indDlgtDef.IsGenericType ? indDlgtDef : indDlgtDef.MakeGenericType(typeGenericArgs.Concat(methodGenericArgs).ToArray());
        }

        static Type DefineIndirectionDelegate(MethodBase target)
        {
            var modBldr = default(ModuleBuilder);
            using (StorageToDefineRunnableTypes.PrepareTemporaryModuleBuilder(target.Module.Assembly, out modBldr))
            {
                var @object = typeof(object);
                var multicastDelegate = typeof(MulticastDelegate);
                var intPtr = typeof(IntPtr);

                var declaringType = target.DeclaringType;

                var typeGenericArgBldrs = new List<Type>();
                var methodGenericArgBldrs = new List<Type>();
                var indDlgtBldr = default(TypeBuilder);
                {
                    var name = MakeGeneratedDelegateUniqueName("IndirectionDelegate", target, declaringType);
                    var attr = TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NotPublic;
                    var baseType = multicastDelegate;
                    indDlgtBldr = modBldr.DefineType(name, attr, baseType);
                    DefineGeneratedDelegateGenericParameters(indDlgtBldr, target, declaringType, ref typeGenericArgBldrs, ref methodGenericArgBldrs);
                }

                {
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
                    var callingConvention = CallingConventions.HasThis;
                    var paramTypes = new[] { @object, intPtr };
                    var indDlgtBldr_ctor = indDlgtBldr.DefineConstructor(attr, callingConvention, paramTypes);
                    var implAttr = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    indDlgtBldr_ctor.SetImplementationFlags(implAttr);
                }

                {
                    var name = "Invoke";
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
                    var callingConvention = CallingConventions.HasThis;
                    var paramTypes = MakeGeneratedDelegateParameterTypes(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var retType = MakeGeneratedDelegateReturnType(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var indDlgtBldr_Invoke = indDlgtBldr.DefineMethod(name, attr, callingConvention, retType, paramTypes);
                    var implAttr = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                    indDlgtBldr_Invoke.SetImplementationFlags(implAttr);
                }

                return indDlgtBldr.CreateType();
            }
        }

        static Func<Delegate, Dictionary<object, TargetSettingValue<Delegate>>, Delegate> DefineBehaviorSelectorFactory(MethodBase target, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var indDlgtDef = GetIndirectionDelegateDefinition(target);
            var behaviorSelectorImpl = GetBehaviorSelectorImplementation(indDlgtDef, target, typeGenericArgs, methodGenericArgs);
            var indDlgt = !indDlgtDef.IsGenericType ? indDlgtDef : indDlgtDef.MakeGenericType(typeGenericArgs.Concat(methodGenericArgs).ToArray());

            var behaviorSelectorImpl_defaultBehavior = behaviorSelectorImpl.GetField("defaultBehavior");
            var behaviorSelectorImpl_indirections = behaviorSelectorImpl.GetField("indirections");

            var indDlgt_ctor = indDlgt.GetConstructors().First();
            var behaviorSelectorImpl_ctor = behaviorSelectorImpl.GetConstructors().First();
            var behaviorSelectorImpl_Invoke = behaviorSelectorImpl.GetMethods().First();

            var factory = new DynamicMethod("Factory", typeof(Delegate), new[] { typeof(Delegate), typeof(Dictionary<object, TargetSettingValue<Delegate>>) }, typeof(InstanceGetters).Module, true);
            var gen = factory.GetILGenerator();

            var local0_impl = gen.DeclareLocal(behaviorSelectorImpl);

            gen.Emit(OpCodes.Newobj, behaviorSelectorImpl_ctor);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stfld, behaviorSelectorImpl_defaultBehavior);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, behaviorSelectorImpl_indirections);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldftn, behaviorSelectorImpl_Invoke);
            gen.Emit(OpCodes.Newobj, indDlgt_ctor);
            gen.Emit(OpCodes.Ret);

            return (Func<Delegate, Dictionary<object, TargetSettingValue<Delegate>>, Delegate>)factory.CreateDelegate(typeof(Func<Delegate, Dictionary<object, TargetSettingValue<Delegate>>, Delegate>));
        }

        static Type GetBehaviorSelectorImplementation(Type indDlgtDef, MethodBase target, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var behaviorSelectorImplCache = StorageToDefineRunnableTypes.ms_behaviorSelectorImplCache;
            if (!behaviorSelectorImplCache.ContainsKey(target))
                behaviorSelectorImplCache.Add(target, DefineBehaviorSelectorImplementation(indDlgtDef, target));

            var behaviorSelectorImpl = behaviorSelectorImplCache[target];
            return !behaviorSelectorImpl.IsGenericType ? behaviorSelectorImpl : behaviorSelectorImpl.MakeGenericType(typeGenericArgs.Concat(methodGenericArgs).ToArray());
        }

        static Type DefineBehaviorSelectorImplementation(Type indDlgtDef, MethodBase target)
        {
            var modBldr = default(ModuleBuilder);
            using (StorageToDefineRunnableTypes.PrepareTemporaryModuleBuilder(target.Module.Assembly, out modBldr))
            {
                var @object = typeof(object);
                var dictionary = typeof(Dictionary<,>);
                var targetSettingValue = typeof(TargetSettingValue<>);
                var @delegate = typeof(Delegate);
                var looseCrossDomainAccessor = typeof(LooseCrossDomainAccessor);

                var targetSettingValueOfDelegate = targetSettingValue.MakeGenericType(@delegate);
                var dictionaryOfObjectOfTargetSettingValueOfDelegate = dictionary.MakeGenericType(@object, targetSettingValueOfDelegate);

                var object_ctor = @object.GetConstructors().First();
                var object_ReferenceEquals = @object.GetMethod("ReferenceEquals");
                var dictionaryOfObjectOfTargetSettingValueOfDelegate_ContainsKey = dictionaryOfObjectOfTargetSettingValueOfDelegate.GetMethod("ContainsKey");
                var dictionaryOfObjectOfTargetSettingValueOfDelegate_get_Item = dictionaryOfObjectOfTargetSettingValueOfDelegate.GetMethod("get_Item");
                var targetSettingValueOfDelegate_get_Indirection = targetSettingValueOfDelegate.GetMethod("get_Indirection");
                var looseCrossDomainAccessor_SafelyCast = looseCrossDomainAccessor.GetMethod("SafelyCast");

                var declaringType = target.DeclaringType;

                var typeGenericArgBldrs = new List<Type>();
                var methodGenericArgBldrs = new List<Type>();
                var behaviorSelectorImplBldr = default(TypeBuilder);
                {
                    var @namespace = Path.GetFileNameWithoutExtension(modBldr.ScopeName);
                    var name = MakeGeneratedDelegateUniqueName("BehaviorSelectorImplementation", target, declaringType);
                    var attr = TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NotPublic;
                    behaviorSelectorImplBldr = modBldr.DefineType(@namespace + "." + name, attr);
                    DefineGeneratedDelegateGenericParameters(behaviorSelectorImplBldr, target, declaringType, ref typeGenericArgBldrs, ref methodGenericArgBldrs);
                }
                var indDlgt = !indDlgtDef.IsGenericType ? indDlgtDef : indDlgtDef.MakeGenericType(typeGenericArgBldrs.Concat(methodGenericArgBldrs).ToArray());

                var indDlgtDef_Invoke = indDlgtDef.GetMethod("Invoke");
                var indDlgt_Invoke = !indDlgtDef.IsGenericType ? indDlgtDef_Invoke : TypeBuilder.GetMethod(indDlgt, indDlgtDef_Invoke);

                var looseCrossDomainAccessor_SafelyCastOfIndDlgt = looseCrossDomainAccessor_SafelyCast.MakeGenericMethod(indDlgtDef);

                var behaviorSelectorImplBldr_defaultBehavior = default(FieldBuilder);
                {
                    var name = "defaultBehavior";
                    var type = @delegate;
                    var attr = FieldAttributes.Public;
                    behaviorSelectorImplBldr_defaultBehavior = behaviorSelectorImplBldr.DefineField(name, type, attr);
                }

                var behaviorSelectorImplBldr_indirections = default(FieldBuilder);
                {
                    var name = "indirections";
                    var type = dictionaryOfObjectOfTargetSettingValueOfDelegate;
                    var attr = FieldAttributes.Public;
                    behaviorSelectorImplBldr_indirections = behaviorSelectorImplBldr.DefineField(name, type, attr);
                }

                {
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
                    var callingConvention = CallingConventions.HasThis;
                    var behaviorSelectorImplBldr_ctor = behaviorSelectorImplBldr.DefineConstructor(attr, callingConvention, Type.EmptyTypes);
                    var gen = behaviorSelectorImplBldr_ctor.GetILGenerator();

                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Call, object_ctor);
                    gen.Emit(OpCodes.Ret);
                }

                {
                    var name = "Invoke";
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig;
                    var callingConvention = CallingConventions.HasThis;
                    var paramTypes = MakeGeneratedDelegateParameterTypes(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var retType = MakeGeneratedDelegateReturnType(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var behaviorSelectorImplBldr_Invoke = behaviorSelectorImplBldr.DefineMethod(name, attr, callingConvention, retType, paramTypes);
                    var gen = behaviorSelectorImplBldr_Invoke.GetILGenerator();

                    var local0_this = gen.DeclareLocal(paramTypes[0]);
                    var local1_targetSettingValueOfDelegate = gen.DeclareLocal(targetSettingValueOfDelegate);
                    var local_refs = new LocalBuilder[paramTypes.Length - 1];
                    const int OffsetForThis = 2;    // [0] and [1] indicate the following: [0]: implicit this, [1]: explicit this.
                    for (int i = 1; i < paramTypes.Length; i++)
                    {
                        if (!paramTypes[i].IsByRef)
                            continue;

                        local_refs[i - 1] = gen.DeclareLocal(paramTypes[i].GetElementType());
                    }
                    var local_ret = default(LocalBuilder);
                    if (retType != typeof(void))
                        local_ret = gen.DeclareLocal(retType);

                    var label0 = gen.DefineLabel();
                    var label1 = gen.DefineLabel();

                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Ldloca_S, local0_this);
                    gen.Emit(OpCodes.Initobj, local0_this.LocalType);
                    gen.Emit(OpCodes.Ldloc_S, local0_this);
                    gen.Emit(OpCodes.Call, object_ReferenceEquals);
                    gen.Emit(OpCodes.Brtrue, label0);

                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, behaviorSelectorImplBldr_indirections);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Callvirt, dictionaryOfObjectOfTargetSettingValueOfDelegate_ContainsKey);
                    gen.Emit(OpCodes.Brtrue, label1);

                    gen.MarkLabel(label0);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            continue;

                        gen.Emit(OpCodes.Ldloca, local_refs[i]);
                        gen.Emit(OpCodes.Initobj, local_refs[i].LocalType);
                    }
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, behaviorSelectorImplBldr_defaultBehavior);
                    gen.Emit(OpCodes.Call, looseCrossDomainAccessor_SafelyCastOfIndDlgt);
                    gen.Emit(OpCodes.Ldarg_1);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            gen.Emit(OpCodes.Ldarg_Opt, i + OffsetForThis);
                        else
                            gen.Emit(OpCodes.Ldloca, local_refs[i]);
                    }
                    gen.Emit(OpCodes.Callvirt, indDlgt_Invoke);

                    if (local_ret != null)
                        gen.Emit(OpCodes.Stloc, local_ret);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            continue;

                        gen.Emit(OpCodes.Ldarg_Opt, i + OffsetForThis);
                        gen.Emit(OpCodes.Ldloc, local_refs[i]);
                        gen.Emit(OpCodes.Stobj, local_refs[i].LocalType);
                    }
                    if (local_ret != null)
                        gen.Emit(OpCodes.Ldloc, local_ret);
                    gen.Emit(OpCodes.Ret);

                    gen.MarkLabel(label1);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            continue;

                        gen.Emit(OpCodes.Ldloca, local_refs[i]);
                        gen.Emit(OpCodes.Initobj, local_refs[i].LocalType);
                    }
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, behaviorSelectorImplBldr_indirections);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Callvirt, dictionaryOfObjectOfTargetSettingValueOfDelegate_get_Item);
                    gen.Emit(OpCodes.Stloc_S, local1_targetSettingValueOfDelegate);
                    gen.Emit(OpCodes.Ldloca_S, local1_targetSettingValueOfDelegate);
                    gen.Emit(OpCodes.Call, targetSettingValueOfDelegate_get_Indirection);
                    gen.Emit(OpCodes.Call, looseCrossDomainAccessor_SafelyCastOfIndDlgt);
                    gen.Emit(OpCodes.Ldarg_1);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            gen.Emit(OpCodes.Ldarg_Opt, i + OffsetForThis);
                        else
                            gen.Emit(OpCodes.Ldloca, local_refs[i]);
                    }
                    gen.Emit(OpCodes.Callvirt, indDlgt_Invoke);

                    if (local_ret != null)
                        gen.Emit(OpCodes.Stloc, local_ret);
                    for (int i = 0; i < local_refs.Length; i++)
                    {
                        if (local_refs[i] == null)
                            continue;

                        gen.Emit(OpCodes.Ldarg_Opt, i + OffsetForThis);
                        gen.Emit(OpCodes.Ldloc, local_refs[i]);
                        gen.Emit(OpCodes.Stobj, local_refs[i].LocalType);
                    }
                    if (local_ret != null)
                        gen.Emit(OpCodes.Ldloc, local_ret);
                    gen.Emit(OpCodes.Ret);
                }

                return behaviorSelectorImplBldr.CreateType();
            }
        }

        static Type GetDefaultBehaviorImplementation(MethodBase target, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var defaultBehaviorImplCache = StorageToDefineRunnableTypes.ms_defaultBehaviorImplCache;
            if (!defaultBehaviorImplCache.ContainsKey(target))
                defaultBehaviorImplCache.Add(target, DefineDefaultBehaviorImplementation(target));

            var defaultBehaviorImpl = defaultBehaviorImplCache[target];
            return !defaultBehaviorImpl.IsGenericType ? defaultBehaviorImpl : defaultBehaviorImpl.MakeGenericType(typeGenericArgs.Concat(methodGenericArgs).ToArray());
        }

        static Type DefineDefaultBehaviorImplementation(MethodBase target)
        {
            var modBldr = default(ModuleBuilder);
            using (StorageToDefineRunnableTypes.PrepareTemporaryModuleBuilder(target.Module.Assembly, out modBldr))
            {
                var @object = typeof(object);
                var indirectionBehaviors = typeof(IndirectionBehaviors);
                var instanceGetters = typeof(InstanceGetters);
                var instanceGettersProcessingDisabled = typeof(InstanceGetters.InstanceGettersProcessingDisabled);
                var fallthroughException = typeof(FallthroughException);
                var iDisposable = typeof(IDisposable);
                var notImplementedException = typeof(NotImplementedException);

                var object_ctor = @object.GetConstructors().First();
                var instanceGetters_DisableProcessing = instanceGetters.GetMethod("DisableProcessing");
                var fallthroughException_ctor = fallthroughException.GetConstructor(Type.EmptyTypes);
                var iDisposable_Dispose = iDisposable.GetMethod("Dispose");
                var notImplementedException_ctor = notImplementedException.GetConstructor(Type.EmptyTypes);

                var declaringType = target.DeclaringType;

                var typeGenericArgBldrs = new List<Type>();
                var methodGenericArgBldrs = new List<Type>();
                var defaultBehaviorImplBldr = default(TypeBuilder);
                {
                    var @namespace = Path.GetFileNameWithoutExtension(modBldr.ScopeName);
                    var name = MakeGeneratedDelegateUniqueName("DefaultBehaviorImplementation", target, declaringType);
                    var attr = TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NotPublic;
                    defaultBehaviorImplBldr = modBldr.DefineType(@namespace + "." + name, attr);
                    DefineGeneratedDelegateGenericParameters(defaultBehaviorImplBldr, target, declaringType, ref typeGenericArgBldrs, ref methodGenericArgBldrs);
                }

                var defaultBehaviorImplBldr_defaultBehavior = default(FieldBuilder);
                {
                    var name = "defaultBehavior";
                    var type = indirectionBehaviors;
                    var attr = FieldAttributes.Public;
                    defaultBehaviorImplBldr_defaultBehavior = defaultBehaviorImplBldr.DefineField(name, type, attr);
                }

                {
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
                    var callingConvention = CallingConventions.HasThis;
                    var defaultBehaviorImplBldr_ctor = defaultBehaviorImplBldr.DefineConstructor(attr, callingConvention, Type.EmptyTypes);
                    var gen = defaultBehaviorImplBldr_ctor.GetILGenerator();

                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Call, object_ctor);
                    gen.Emit(OpCodes.Ret);
                }

                {
                    var name = "Invoke";
                    var attr = MethodAttributes.Public | MethodAttributes.HideBySig;
                    var callingConvention = CallingConventions.HasThis;
                    var paramTypes = MakeGeneratedDelegateParameterTypes(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var retType = MakeGeneratedDelegateReturnType(target, typeGenericArgBldrs, methodGenericArgBldrs);
                    var defaultBehaviorImplBldr_Invoke = defaultBehaviorImplBldr.DefineMethod(name, attr, callingConvention, retType, paramTypes);
                    var gen = defaultBehaviorImplBldr_Invoke.GetILGenerator();

                    var local0 = gen.DeclareLocal(instanceGettersProcessingDisabled);
                    var local1 = gen.DeclareLocal(instanceGettersProcessingDisabled);
                    var local2_result = default(LocalBuilder);
                    if (retType != typeof(void))
                        local2_result = gen.DeclareLocal(retType);

                    var label0 = gen.DefineLabel();
                    var label1 = gen.DefineLabel();

                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, defaultBehaviorImplBldr_defaultBehavior);
                    gen.Emit(OpCodes.Ldc_I4, (int)IndirectionBehaviors.DefaultValue);
                    gen.Emit(OpCodes.Bne_Un_S, label0);

                    for (int i = 0; i < paramTypes.Length; i++)
                    {
                        if (!paramTypes[i].IsByRef)
                            continue;

                        gen.Emit(OpCodes.Ldarg_Opt, i + 1);
                        gen.Emit(OpCodes.Initobj, paramTypes[i].GetElementType());
                    }
                    if (local2_result != null)
                    {
                        gen.Emit(OpCodes.Ldloca_S, local2_result);
                        gen.Emit(OpCodes.Initobj, retType);
                        gen.Emit(OpCodes.Ldloc_S, local2_result);
                    }
                    gen.Emit(OpCodes.Ret);

                    gen.MarkLabel(label0);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, defaultBehaviorImplBldr_defaultBehavior);
                    gen.Emit(OpCodes.Brtrue_S, label1);

                    gen.Emit(OpCodes.Call, instanceGetters_DisableProcessing);
                    gen.Emit(OpCodes.Stloc_0);
                    gen.BeginExceptionBlock();
                    {
                        gen.Emit(OpCodes.Newobj, fallthroughException_ctor);
                        gen.Emit(OpCodes.Throw);
                    }
                    gen.BeginFinallyBlock();
                    {
                        gen.Emit(OpCodes.Ldloca_S, local0);
                        gen.Emit(OpCodes.Constrained, instanceGettersProcessingDisabled);
                        gen.Emit(OpCodes.Callvirt, iDisposable_Dispose);
                    }
                    gen.EndExceptionBlock();

                    gen.MarkLabel(label1);
                    gen.Emit(OpCodes.Call, instanceGetters_DisableProcessing);
                    gen.Emit(OpCodes.Stloc_1);
                    gen.BeginExceptionBlock();
                    {
                        gen.Emit(OpCodes.Newobj, notImplementedException_ctor);
                        gen.Emit(OpCodes.Throw);
                    }
                    gen.BeginFinallyBlock();
                    {
                        gen.Emit(OpCodes.Ldloca_S, local1);
                        gen.Emit(OpCodes.Constrained, instanceGettersProcessingDisabled);
                        gen.Emit(OpCodes.Callvirt, iDisposable_Dispose);
                    }
                    gen.EndExceptionBlock();

                    if (local2_result != null)
                    {
                        gen.Emit(OpCodes.Ldloca_S, local2_result);
                        gen.Emit(OpCodes.Initobj, retType);
                        gen.Emit(OpCodes.Ldloc_S, local2_result);
                    }
                    gen.Emit(OpCodes.Ret);
                }

                return defaultBehaviorImplBldr.CreateType();
            }
        }

        static string MakeGeneratedDelegateUniqueName(string prefix, MethodBase target, Type declaringType)
        {
            var token = target.MetadataToken;
            var targetName = target.Name;
            var genericSuffix = string.Empty;
            {
                var genericArgsSize = 0;
                if (declaringType.IsGenericType)
                    genericArgsSize += declaringType.GetGenericArguments().Length;
                if (target.IsGenericMethod)
                    genericArgsSize += target.GetGenericArguments().Length;
                if (0 < genericArgsSize)
                    genericSuffix = string.Format("`{0}", genericArgsSize);
            }
            return string.Format("{0}{1}For{2}{3}", prefix, token & 0x00FFFFFF, targetName, genericSuffix);
        }

        static void DefineGeneratedDelegateGenericParameters(TypeBuilder typeBldr, MethodBase target, Type declaringType, ref List<Type> typeGenericArgBldrs, ref List<Type> methodGenericArgBldrs)
        {
            var names = new List<string>();
            var typeGenericArgs = !declaringType.IsGenericType ? Type.EmptyTypes : declaringType.GetGenericArguments();
            foreach (var genericArg in typeGenericArgs)
                names.Add(genericArg.Name);

            var methodGenericArgs = !target.IsGenericMethod ? Type.EmptyTypes : target.GetGenericArguments();
            foreach (var genericArg in methodGenericArgs)
                names.Add(genericArg.Name);

            if (0 < names.Count)
            {
                var genericArgBldrs = typeBldr.DefineGenericParameters(names.ToArray());
                typeGenericArgBldrs.AddRange(genericArgBldrs.Take(typeGenericArgs.Length).Cast<Type>());
                methodGenericArgBldrs.AddRange(genericArgBldrs.Skip(typeGenericArgs.Length).Cast<Type>());
            }
        }

        static Type[] MakeGeneratedDelegateParameterTypes(MethodBase target, IEnumerable<Type> typeGenericArgBldrs, IEnumerable<Type> methodGenericArgBldrs)
        {
            var paramTypes = new List<Type>();
            if (!target.IsStatic)
            {
                var declaringType_ = target.DeclaringType;
                var isGenericType = declaringType_.IsGenericType;
                var isValueType = declaringType_.IsValueType;
                if (isGenericType)
                    declaringType_ = ReplaceGenericParameter(declaringType_, typeGenericArgBldrs, methodGenericArgBldrs);
                if (isValueType)
                    declaringType_ = declaringType_.MakeByRefType();
                paramTypes.Add(declaringType_);
            }
            var @params = target.GetParameters();
            foreach (var param in @params)
            {
                var paramType = param.ParameterType;
                paramType = !ContainsGenericParameter(paramType) ? paramType : ReplaceGenericParameter(paramType, typeGenericArgBldrs, methodGenericArgBldrs);
                paramTypes.Add(paramType);
            }
            return paramTypes.ToArray();
        }

        static Type MakeGeneratedDelegateReturnType(MethodBase target, IEnumerable<Type> typeGenericArgBldrs, IEnumerable<Type> methodGenericArgBldrs)
        {
            var retType = target.GetReturnTypeIfAvailable();
            return !ContainsGenericParameter(retType) ? retType : ReplaceGenericParameter(retType, typeGenericArgBldrs, methodGenericArgBldrs);
        }

        static bool ContainsGenericParameter(Type type)
        {
            if (type.HasElementType)
            {
                return ContainsGenericParameter(type.GetElementType());
            }
            else if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericArgs = type.GetGenericArguments();
                foreach (var genericArg in genericArgs)
                    if (ContainsGenericParameter(genericArg))
                        return true;
                return ContainsGenericParameter(type.GetGenericTypeDefinition());
            }
            else if (type.IsGenericParameter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static Type ReplaceGenericParameter(Type type, IEnumerable<Type> typeGenericArgBldrs, IEnumerable<Type> methodGenericArgBldrs)
        {
            if (type.IsPointer)
            {
                return ReplaceGenericParameter(type.GetElementType(), typeGenericArgBldrs, methodGenericArgBldrs).MakePointerType();
            }
            else if (type.IsByRef)
            {
                return ReplaceGenericParameter(type.GetElementType(), typeGenericArgBldrs, methodGenericArgBldrs).MakeByRefType();
            }
            else if (type.IsArray)
            {
                var rank = type.GetArrayRank();
                if (rank == 1)
                    return ReplaceGenericParameter(type.GetElementType(), typeGenericArgBldrs, methodGenericArgBldrs).MakeArrayType();
                else
                    return ReplaceGenericParameter(type.GetElementType(), typeGenericArgBldrs, methodGenericArgBldrs).MakeArrayType(rank);
            }
            else if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericArgs = type.GetGenericArguments();
                var genericArgs_ = new List<Type>(genericArgs.Length);
                foreach (var genericArg in genericArgs)
                    genericArgs_.Add(ReplaceGenericParameter(genericArg, typeGenericArgBldrs, methodGenericArgBldrs));
                return ReplaceGenericParameter(type.GetGenericTypeDefinition(), typeGenericArgBldrs, methodGenericArgBldrs).MakeGenericType(genericArgs_.ToArray());
            }
            else if (type.IsGenericParameter && type.DeclaringType != null && type.DeclaringMethod == null)
            {
                Debug.Assert(typeGenericArgBldrs.Any());
                return typeGenericArgBldrs.ElementAt(type.GenericParameterPosition);
            }
            else if (type.IsGenericParameter && type.DeclaringType != null && type.DeclaringMethod != null)
            {
                Debug.Assert(methodGenericArgBldrs.Any());
                return methodGenericArgBldrs.ElementAt(type.GenericParameterPosition);
            }
            else
            {
                return type;
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
                return ReturnTypeEquals(x, y) && ParametersEquals(x, y);
            }

            public int GetHashCode(MethodBase obj)
            {
                var hashCode = 0;
                hashCode ^= GetReturnTypeHashCode(obj);
                hashCode ^= GetParametersHashCode(obj);
                return hashCode;
            }

            public static int GetReturnTypeHashCode(MethodBase mb)
            {
                var hasRet = false;
                var mi = default(MethodInfo);
                hasRet = (mi = mb as MethodInfo) != null && mi.ReturnType != typeof(void);

                return hasRet.GetHashCode();
            }

            public static int GetParametersHashCode(MethodBase mb)
            {
                return GetParametersHashCode(mb.GetParameters());
            }

            public static int GetParametersHashCode(IEnumerable<ParameterInfo> @params)
            {
                return @params.Aggregate(0, (result, next) => result ^ GetParameterHashCode(next));
            }

            public static int GetParameterHashCode(ParameterInfo param)
            {
                var attr = param.Attributes & ParameterAttributes.Out;
                var isByRef = param.ParameterType.IsByRef;
                return attr.GetHashCode() ^ isByRef.GetHashCode();
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

            public static bool ParametersEquals(MethodBase x, MethodBase y)
            {
                return ParametersEquals(x.GetParameters(), y.GetParameters());
            }

            public static bool ParametersEquals(IEnumerable<ParameterInfo> paramsX, IEnumerable<ParameterInfo> paramsY)
            {
                return paramsX.SequenceEqual(paramsY, new LambdaEqualityComparer<ParameterInfo>(ParameterEquals));
            }

            public static bool ParameterEquals(ParameterInfo paramX, ParameterInfo paramY)
            {
                var attrX = paramX.Attributes & ParameterAttributes.Out;
                var isByRefX = paramX.ParameterType.IsByRef;
                var attrY = paramY.Attributes & ParameterAttributes.Out;
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

        class StorageToFindIndirectionDelegate
        {
            static readonly Dictionary<MethodBase, Type> ms_indDlgtCache = new Dictionary<MethodBase, Type>(new MethodSigEqualityComparer());

            public static IndirectionDelegateCachePrepared PrepareIndirectionDelegateCache(IEnumerable<Assembly> indDlls, out Dictionary<MethodBase, Type> indDlgtCache)
            {
                try
                {
                    Monitor.Enter(ms_indDlgtCache);

                    if (ms_indDlgtCache.Count == 0)
                    {
                        var indDlgtAttrType = typeof(IndirectionDelegateAttribute);
                        var indDlgts = indDlls.SelectMany(_ => _.GetTypes()).Where(_ => _.IsDefined(indDlgtAttrType, false));
                        foreach (var indDlgt in indDlgts)
                        {
                            var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
                            if (!ms_indDlgtCache.ContainsKey(indDlgt_Invoke))
                                ms_indDlgtCache.Add(indDlgt_Invoke, indDlgt);
                        }
                    }

                    indDlgtCache = ms_indDlgtCache;
                    return new IndirectionDelegateCachePrepared(indDlgtCache);
                }
                catch
                {
                    Monitor.Exit(ms_indDlgtCache);
                    throw;
                }
            }

            public struct IndirectionDelegateCachePrepared : IDisposable
            {
                readonly Dictionary<MethodBase, Type> m_indDlgtCache;

                public IndirectionDelegateCachePrepared(Dictionary<MethodBase, Type> indDlgtCache)
                {
                    Debug.Assert(indDlgtCache != null);
                    m_indDlgtCache = indDlgtCache;
                }

                public void Dispose()
                {
                    Monitor.Exit(m_indDlgtCache);
                }
            }
        }

        class DelegateSignatureInfo : MethodInfo
        {
            readonly MethodBase m_target;
            readonly ParameterInfo[] m_params;
            readonly Type m_retType;

            public DelegateSignatureInfo(MethodBase target)
            {
                m_target = target;

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

                m_params = @params.ToArray();
                m_retType = m_target.GetReturnTypeIfAvailable();
            }

            public override MethodInfo GetBaseDefinition() { throw new NotImplementedException(); }
            public override ICustomAttributeProvider ReturnTypeCustomAttributes { get { throw new NotImplementedException(); } }
            public override MethodAttributes Attributes { get { throw new NotImplementedException(); } }
            public override MethodImplAttributes GetMethodImplementationFlags() { throw new NotImplementedException(); }
            public override ParameterInfo[] GetParameters() { return m_params; }
            public override Type ReturnType { get { return m_retType; } }
            public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) { throw new NotImplementedException(); }
            public override RuntimeMethodHandle MethodHandle { get { throw new NotImplementedException(); } }
            public override Type DeclaringType { get { throw new NotImplementedException(); } }
            public override object[] GetCustomAttributes(Type attributeType, bool inherit) { throw new NotImplementedException(); }
            public override object[] GetCustomAttributes(bool inherit) { throw new NotImplementedException(); }
            public override bool IsDefined(Type attributeType, bool inherit) { throw new NotImplementedException(); }
            public override string Name { get { throw new NotImplementedException(); } }
            public override Type ReflectedType { get { throw new NotImplementedException(); } }
        }

        static Type GetIndirectionDelegateInstance(MethodBase target, IEnumerable<Assembly> indDlls)
        {
            Debug.Assert(target != null);

            var indDlgtCache = default(Dictionary<MethodBase, Type>);
            using (StorageToFindIndirectionDelegate.PrepareIndirectionDelegateCache(indDlls, out indDlgtCache))
            {
                var sigInfo = new DelegateSignatureInfo(target);
                if (!indDlgtCache.ContainsKey(sigInfo))
                    return null;

                var indDlgt = indDlgtCache[sigInfo];
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
        }

        static Type[] GetIndirectionDelegateSignature(MethodBase target)
        {
            Debug.Assert(target != null);

            var sig = new List<Type>();

            if (!target.IsStatic)
            {
                var declaringType = target.DeclaringType;
                var isGenericType = declaringType.IsGenericType;
                var isValueType = declaringType.IsValueType;
                if (isGenericType)
                    declaringType = MakeGenericExplicitThisType(declaringType);
                if (isValueType)
                    declaringType = declaringType.MakeByRefType();
                sig.Add(declaringType);
            }

            var @params = target.GetParameters();
            sig.AddRange(@params.Select(_ => _.ParameterType));

            sig.Add(target.GetReturnTypeIfAvailable());

            return sig.ToArray();
        }

        class StorageToDefineReflectionOnlyTypes
        {
            public static Dictionary<Type, Type[]> ms_tempGenericArgsCache = new Dictionary<Type, Type[]>();

            static ModuleBuilder NewTemporaryModuleBuilder()
            {
                var asmName = new AssemblyName(Guid.NewGuid().ToString("N"));
                var asmBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.ReflectionOnly);
                return asmBldr.DefineDynamicModule(asmName.Name + ".dll");
            }
            public static ModuleBuilder ms_modBldr = NewTemporaryModuleBuilder();
        }

        static Type MakeGenericExplicitThisType(Type target)
        {
            // We couldn't make the generic type instance that is instantiated by its generic parameter from the managed code.
            // For example, `typeof(Nullable<>).MakeGenericType(typeof(Nullable<>).GetGenericArguments()[0])` is treated same as `Nullable<>`!
            // Therefore, we have to generate the generic parameter that has the constraints same as it to express such generic type instance.
            var tempGenericArgsCache = StorageToDefineReflectionOnlyTypes.ms_tempGenericArgsCache;
            lock (tempGenericArgsCache)
            {
                if (!tempGenericArgsCache.ContainsKey(target))
                    tempGenericArgsCache.Add(target, DefineTemporaryGenericArguments(target));

                return target.MakeGenericType(tempGenericArgsCache[target]);
            }
        }

        static Type[] DefineTemporaryGenericArguments(Type target)
        {
            var modBldr = StorageToDefineReflectionOnlyTypes.ms_modBldr;
            lock (modBldr)
            {
                var typeName = Path.GetFileNameWithoutExtension(modBldr.ScopeName) + "." + target.FullName;
                var typeBldr = modBldr.DefineType(typeName);
                var genericParams = target.GetGenericArguments();
                var names = genericParams.Select(_ => _.Name).ToArray();
                var genericParamBldrs = typeBldr.DefineGenericParameters(names);
                for (int i = 0; i < genericParams.Length; i++)
                {
                    var genericParam = genericParams[i];
                    var genericParamBldr = genericParamBldrs[i];

                    genericParamBldr.SetGenericParameterAttributes(genericParam.GenericParameterAttributes);

                    var baseTypeConstraint = default(Type);
                    var interfaceConstraints = new List<Type>();
                    foreach (var genericParamConstraint in genericParam.GetGenericParameterConstraints())
                    {
                        if (genericParamConstraint.IsInterface)
                        {
                            interfaceConstraints.Add(genericParamConstraint);
                        }
                        else
                        {
                            Debug.Assert(baseTypeConstraint == null, "`baseTypeConstraint` should be initialized only once.");
                            baseTypeConstraint = genericParamConstraint;
                        }
                    }
                    genericParamBldr.SetBaseTypeConstraint(baseTypeConstraint);
                    genericParamBldr.SetInterfaceConstraints(interfaceConstraints.ToArray());
                }

                return typeBldr.CreateType().GetGenericArguments();
            }
        }
    }
}
