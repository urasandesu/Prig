/* 
 * File: DelegatedMethodPreparation.cpp
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



#include "stdafx.h"

#ifndef PRIGDATA_H
#include <PrigData.h>
#endif

#ifndef DELEGATEDMETHODPREPARATION_H
#include <DelegatedMethodPreparation.h>
#endif

namespace DelegatedMethodPreparationDetail {

    DelegatedMethodPreparation::DelegatedMethodPreparation() : 
        m_pIndHolder1Delegate(nullptr), 
        m_pIndInfo(nullptr), 
        m_pIndDlgt(nullptr), 
        m_pIndDlgtGen(nullptr), 
        m_pDelegate(nullptr), 
        m_pException(nullptr), 
        m_pString(nullptr), 
        m_pStringArr(nullptr), 
        m_pTarget(nullptr), 
        m_pLooseCrossDomainAccessor_TryGetIndHolder1Delegate(nullptr), 
        m_pLooseCrossDomainAccessor_SafelyCastIndDlgt(nullptr), 
        m_pIndInfo_set_AssemblyName(nullptr), 
        m_pIndInfo_set_Token(nullptr), 
        m_pIndInfo_set_Instantiation(nullptr), 
        m_pIndHolder1Delegate_TryGet(nullptr), 
        m_pIndDlgt_Invoke(nullptr), 
        m_pLooseCrossDomainAccessor_IsInstanceOfIdentity(nullptr), 
        m_pType_GetTypeFromHandle(nullptr), 
        m_pObject_ToString(nullptr)
    { }



    bool ContainsGenericParameter(IType const *pType)
    {
        auto kind = pType->GetKind();
        switch (kind.Value())
        {
            case TypeKinds::TK_PTR:
            case TypeKinds::TK_BYREF:
            case TypeKinds::TK_SZARRAY:
            case TypeKinds::TK_ARRAY:
                return ContainsGenericParameter(pType->GetDeclaringType());
            case TypeKinds::TK_GENERICINST:
                {
                    auto const &genericArgs = pType->GetGenericArguments();
                    BOOST_FOREACH (auto const &pGenericArg, genericArgs)
                        if (ContainsGenericParameter(pGenericArg))
                            return true;
                    return ContainsGenericParameter(pType->GetDeclaringType());
                }
            case TypeKinds::TK_VAR:
            case TypeKinds::TK_MVAR:
                return true;
            default:
                return false;
        }
    }



    IType const *ReplaceGenericParameter(IType const *pType, vector<TypeGenerator *> const &typeGenericArgGens, vector<TypeGenerator *> const &methodGenericArgGens)
    {
        auto kind = pType->GetKind();
        switch (kind.Value())
        {
            case TypeKinds::TK_PTR:
                return ReplaceGenericParameter(pType->GetDeclaringType(), typeGenericArgGens, methodGenericArgGens)->MakePointerType();
            case TypeKinds::TK_BYREF:
                return ReplaceGenericParameter(pType->GetDeclaringType(), typeGenericArgGens, methodGenericArgGens)->MakeByRefType();
            case TypeKinds::TK_SZARRAY:
                return ReplaceGenericParameter(pType->GetDeclaringType(), typeGenericArgGens, methodGenericArgGens)->MakeArrayType();
            case TypeKinds::TK_ARRAY:
                return ReplaceGenericParameter(pType->GetDeclaringType(), typeGenericArgGens, methodGenericArgGens)->MakeArrayType(pType->GetDimensions());
            case TypeKinds::TK_GENERICINST:
                {
                    auto const &genericArgs = pType->GetGenericArguments();
                    auto genericArgs_ = vector<IType const *>();
                    genericArgs_.reserve(genericArgs.size());
                    BOOST_FOREACH (auto const &pGenericArg, genericArgs)
                        genericArgs_.push_back(ReplaceGenericParameter(pGenericArg, typeGenericArgGens, methodGenericArgGens));
                    return ReplaceGenericParameter(pType->GetDeclaringType(), typeGenericArgGens, methodGenericArgGens)->MakeGenericType(genericArgs_);
                }
            case TypeKinds::TK_VAR:
                _ASSERTE(!typeGenericArgGens.empty());
                return typeGenericArgGens[pType->GetGenericParameterPosition()];
            case TypeKinds::TK_MVAR:
                _ASSERTE(!methodGenericArgGens.empty());
                return methodGenericArgGens[pType->GetGenericParameterPosition()];
            default:
                return pType;
        }
    }

    void DefineIndirectionDelegate(MethodGenerator *pTarget, PrigData &prigData, TypeGenerator *&pIndDlgtGen, MethodGenerator *&pIndDlgtGen_Invoke, vector<IType const *> &instantiation)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;
        using std::wstring;

        auto timer = cpu_timer();

        _ASSERTE(prigData.m_pMSCorLibDll);
        auto const *pMSCorLibDll = prigData.m_pMSCorLibDll;
        auto const *pObject = pMSCorLibDll->GetType(L"System.Object");
        auto const *pMulticastDelegate = pMSCorLibDll->GetType(L"System.MulticastDelegate");
        auto const *pIntPtr = pMSCorLibDll->GetType(L"System.IntPtr");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();
            
        auto *pDeclaringTypeGen = pTarget->GetDeclaringTypeGenerator();
        
        auto typeGenericArgGens = vector<TypeGenerator *>();
        auto methodGenericArgGens = vector<TypeGenerator *>();
        {
            auto mdTarget = pTarget->GetToken();
            auto const &targetName = pTarget->GetName();
            auto genericSuffix = wstring();
            {
                auto genericArgsSize = size_t();
                if (pDeclaringTypeGen->IsGenericType())
                    genericArgsSize += pDeclaringTypeGen->GetGenericArguments().size();
                if (pTarget->IsGenericMethod())
                    genericArgsSize += pTarget->GetGenericArguments().size();
                if (0 < genericArgsSize)
                    genericSuffix = boost::str(boost::wformat(L"`%|1$d|") % genericArgsSize);
            }
            auto name = boost::str(boost::wformat(L"IndirectionDelegate%|1$d|For%|2$s|%|3$s|") % RidFromToken(mdTarget) % targetName % genericSuffix);
            auto attr = TypeAttributes::TA_ANSI_CLASS | TypeAttributes::TA_SEALED | TypeAttributes::TA_NESTED_PRIVATE;
            auto const *pBaseType = pMulticastDelegate;
            pIndDlgtGen = pDeclaringTypeGen->DefineNestedType(name, attr, pBaseType);
            
            auto names = vector<wstring>();
            {
                auto const &typeGenericArgs = pDeclaringTypeGen->GetGenericArguments();
                BOOST_FOREACH (auto const &pGenericArg, typeGenericArgs)
                    names.push_back(pGenericArg->GetFullName());

                auto const &methodGenericArgs = pTarget->GetGenericArguments();
                BOOST_FOREACH (auto const &pGenericArg, methodGenericArgs)
                    names.push_back(pGenericArg->GetFullName());

                auto genericArgGens = vector<TypeGenerator *>();
                pIndDlgtGen->DefineGenericParameters(names, genericArgGens);
                typeGenericArgGens.assign(genericArgGens.begin(), genericArgGens.begin() + typeGenericArgs.size());
                methodGenericArgGens.assign(genericArgGens.begin() + typeGenericArgs.size(), genericArgGens.end());
            }
        }

        {
            auto attr = MethodAttributes::MA_PUBLIC | MethodAttributes::MA_HIDE_BY_SIG | MethodAttributes::MA_SPECIAL_NAME | MethodAttributes::MA_RT_SPECIAL_NAME;
            auto callingConvention = CallingConventions::CC_HAS_THIS;
            auto paramTypes = vector<IType const *>();
            paramTypes.push_back(pObject);
            paramTypes.push_back(pIntPtr);
            auto *pIndDlgtGen_ctor = pIndDlgtGen->DefineConstructor(attr, callingConvention, paramTypes);
            
            auto implAttr = MethodImplAttributes::MIA_RUNTIME | MethodImplAttributes::MIA_MANAGED;
            pIndDlgtGen_ctor->SetImplementationFlags(implAttr);
        }

        {
            auto name = wstring(L"Invoke");
            auto attr = MethodAttributes::MA_PUBLIC | MethodAttributes::MA_HIDE_BY_SIG | MethodAttributes::MA_NEW_SLOT | MethodAttributes::MA_VIRTUAL;
            auto callingConvention = CallingConventions::CC_HAS_THIS;
            auto paramTypes = vector<IType const *>();
            if (!pTarget->IsStatic())
            {
                auto const *pDeclaringType = pTarget->GetDeclaringType();
                auto isGenericType = pDeclaringType->IsGenericType();
                auto isValueType = pDeclaringType->IsValueType();
                if (isGenericType)
                    pDeclaringType = MakeGenericExplicitItsInstance(pDeclaringType);
                if (isValueType)
                    pDeclaringType = pDeclaringType->MakeByRefType();
                paramTypes.push_back(pDeclaringType);
                instantiation.push_back(pDeclaringType);
            }
            auto const &params = pTarget->GetParameters();
            BOOST_FOREACH (auto const &pParam, params)
            {
                auto const *pParamType = pParam->GetParameterType();
                instantiation.push_back(pParamType);
                pParamType = !ContainsGenericParameter(pParamType) ? pParamType : ReplaceGenericParameter(pParamType, typeGenericArgGens, methodGenericArgGens);
                paramTypes.push_back(pParamType);
            }
            auto const *pRetType = pTarget->GetReturnType();
            instantiation.push_back(pRetType);
            pRetType = !ContainsGenericParameter(pRetType) ? pRetType : ReplaceGenericParameter(pRetType, typeGenericArgGens, methodGenericArgGens);
            pIndDlgtGen_Invoke = pIndDlgtGen->DefineMethod(name, attr, callingConvention, pRetType, paramTypes);
            
            auto implAttr = MethodImplAttributes::MIA_RUNTIME | MethodImplAttributes::MIA_MANAGED;
            pIndDlgtGen_Invoke->SetImplementationFlags(implAttr);
        }

        CPPANONYM_V_LOG1("Processing time to make IndirectionDelegate: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();
    }

    IType const *MakeGenericTypeIfNecessary(IType const *pType, IMethod const *pTarget)
    {
        auto const *pDeclaringType = pTarget->GetDeclaringType();
        
        auto genericArgs = vector<IType const *>();
        if (pDeclaringType->IsGenericType())
        {
            auto const *pAsm = pDeclaringType->GetAssembly();
            
            auto genericParamPos = 0ul;
            BOOST_FOREACH (auto const &_, pDeclaringType->GetGenericArguments())
                genericArgs.push_back(pAsm->GetGenericTypeParameter(genericParamPos++));
        }
        if (pTarget->IsGenericMethod())
        {
            auto const *pAsm = pTarget->GetAssembly();
            
            auto genericParamPos = 0ul;
            BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
                genericArgs.push_back(pAsm->GetGenericMethodParameter(genericParamPos++));
        }
        
        return genericArgs.empty() ? pType : pType->MakeGenericType(genericArgs);
    }

    void DelegatedMethodPreparation::FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        _ASSERTE(prigData.m_pMSCorLibDll);
        auto const *pMSCorLibDll = prigData.m_pMSCorLibDll;
        auto const *pObject = pMSCorLibDll->GetType(L"System.Object");
        auto const *pException = pMSCorLibDll->GetType(L"System.Exception");
        auto const *pType = pMSCorLibDll->GetType(L"System.Type");
        auto const *pInt32 = pMSCorLibDll->GetType(L"System.Int32");
        auto const *pDelegate = pMSCorLibDll->GetType(L"System.Delegate");
        auto const *pString = pMSCorLibDll->GetType(L"System.String");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pType_GetTypeFromHandle = pType->GetMethod(L"GetTypeFromHandle");
        auto const *pObject_ToString = pObject->GetMethod(L"ToString");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        _ASSERTE(prigData.m_pPrigFrameworkDll);
        auto const *pPrigFrmwrkDll = prigData.m_pPrigFrameworkDll;
        auto const *pIndInfo = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndHolder1 = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pLooseCrossDomainAccessor = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");
        auto const *pFlowControlException = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.FlowControlException");

        CPPANONYM_V_LOG1("Processing time to get indirection definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto *pIndDlgtGen = static_cast<TypeGenerator *>(nullptr);
        auto *pIndDlgtGen_Invoke = static_cast<MethodGenerator *>(nullptr);
        auto instantiation = vector<IType const *>();
        DefineIndirectionDelegate(pTarget, prigData, pIndDlgtGen, pIndDlgtGen_Invoke, instantiation);

        CPPANONYM_V_LOG1("Processing time to define indirection delegate: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pStringArr = pString->MakeArrayType();
        
        auto const *pIndHolder1Delegate = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pDelegate);
            pIndHolder1Delegate = pIndHolder1->MakeGenericType(genericArgs);
        }
        
        auto const *pIndDlgt = MakeGenericTypeIfNecessary(pIndDlgtGen, pTarget);

        CPPANONYM_V_LOG1("Processing time to get indirection delegate instance: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndInfo_set_AssemblyName = pIndInfo->GetMethod(L"set_AssemblyName");
        auto const *pIndInfo_set_Token = pIndInfo->GetMethod(L"set_Token");
        auto const *pIndInfo_set_Instantiation = pIndInfo->GetMethod(L"set_Instantiation");
        auto const *pIndDlgt_Invoke = !pIndDlgt->IsGenericType() ? pIndDlgtGen_Invoke : TypeGenerator::GetMethod(pIndDlgt, pIndDlgtGen_Invoke);
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        auto const *pLooseCrossDomainAccessor_SafelyCast = pLooseCrossDomainAccessor->GetMethod(L"SafelyCast");
        auto const *pLooseCrossDomainAccessor_IsInstanceOfIdentity = pLooseCrossDomainAccessor->GetMethod(L"IsInstanceOfIdentity");
        auto const *pIndHolder1Delegate_TryGet = pIndHolder1Delegate->GetMethod(L"TryGet");

        CPPANONYM_V_LOG1("Processing time to get indirection definitions 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pLooseCrossDomainAccessor_TryGetIndHolder1Delegate = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndHolder1Delegate);
            pLooseCrossDomainAccessor_TryGetIndHolder1Delegate = pLooseCrossDomainAccessor_TryGet->MakeGenericMethod(genericArgs);
        }
        
        auto const *pLooseCrossDomainAccessor_SafelyCastIndDlgt = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndDlgt);
            pLooseCrossDomainAccessor_SafelyCastIndDlgt = pLooseCrossDomainAccessor_SafelyCast->MakeGenericMethod(genericArgs);
        }

        CPPANONYM_V_LOG1("Processing time to get indirection delegate instance 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            
        m_pIndHolder1Delegate = pIndHolder1Delegate;
        m_pIndInfo = pIndInfo;
        m_pIndDlgt = pIndDlgt;
        m_pIndDlgtGen = pIndDlgtGen;
        m_pDelegate = pDelegate;
        m_pException = pException;
        m_pString = pString;
        m_pStringArr = pStringArr;
        m_pTarget = pTarget;
        m_pLooseCrossDomainAccessor_TryGetIndHolder1Delegate = pLooseCrossDomainAccessor_TryGetIndHolder1Delegate;
        m_pLooseCrossDomainAccessor_SafelyCastIndDlgt = pLooseCrossDomainAccessor_SafelyCastIndDlgt;
        m_pIndInfo_set_AssemblyName = pIndInfo_set_AssemblyName;
        m_pIndInfo_set_Token = pIndInfo_set_Token;
        m_pIndInfo_set_Instantiation = pIndInfo_set_Instantiation;
        m_pIndHolder1Delegate_TryGet = pIndHolder1Delegate_TryGet;
        m_pIndDlgt_Invoke = pIndDlgt_Invoke;
        m_pLooseCrossDomainAccessor_IsInstanceOfIdentity = pLooseCrossDomainAccessor_IsInstanceOfIdentity;
        m_pType_GetTypeFromHandle = pType_GetTypeFromHandle;
        m_pObject_ToString = pObject_ToString;
        m_instantiation = instantiation;
    }



    void DelegatedMethodPreparation::ResolveIndirectionPreparation(AssemblyGenerator *pAsmGen)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        auto mdt = mdTokenNil;
        mdt = m_pIndDlgtGen->CreateType()->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionDelegate: 0x%|1$08X|", mdt);
        
        // For efficient performance, other tokens will be resolved at `JITCompilationStarted`.
        // In the design that is claimed by MSDN, we have to define any metadata excluding `LocalVarSig` until `ModuleLoadFinished`. It's under investigation, but it seems no problem for now.

        CPPANONYM_V_LOG1("Processing time to resolve the metadata tokens for indirection method preparation: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }



    void DelegatedMethodPreparation::EmitInstantiation(MethodBodyGenerator *pNewBodyGen, ILocal const *pLocal5_0000) const
    {
        pNewBodyGen->Emit(OpCodes::Ldc_I4, static_cast<INT>(m_instantiation.size()));
        pNewBodyGen->Emit(OpCodes::Newarr, m_pString);
        pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal5_0000);
        for (auto i = 0u; i < m_instantiation.size(); ++i)
        {
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal5_0000);
            pNewBodyGen->Emit(OpCodes::Ldc_I4, static_cast<INT>(i));
            pNewBodyGen->Emit(OpCodes::Ldtoken, m_instantiation[i]);
            pNewBodyGen->Emit(OpCodes::Call, m_pType_GetTypeFromHandle);
            pNewBodyGen->Emit(OpCodes::Callvirt, m_pObject_ToString);
            pNewBodyGen->Emit(OpCodes::Stelem_Ref);
        }
    }

    SIZE_T DelegatedMethodPreparation::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen) const
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        CPPANONYM_LOG_FUNCTION();

        auto timer = cpu_timer();

        auto const *pRetType = m_pTarget->GetReturnType();

        auto *pLocal0_holder = pNewBodyGen->DefineLocal(m_pIndHolder1Delegate);
        auto *pLocal1_info = pNewBodyGen->DefineLocal(m_pIndInfo);
        auto *pLocal2_dlgt = pNewBodyGen->DefineLocal(m_pDelegate);
        auto *pLocal3_ind = pNewBodyGen->DefineLocal(m_pIndDlgt);
        auto *pLocal4_e = pNewBodyGen->DefineLocal(m_pException);
        auto *pLocal5_0000 = pNewBodyGen->DefineLocal(m_pStringArr);

        auto label0 = pNewBodyGen->DefineLabel();
        auto label1 = pNewBodyGen->DefineLabel();
        auto label2 = pNewBodyGen->DefineLabel();
        auto label3 = pNewBodyGen->DefineLabel();
            
        if (pRetType->GetKind() != TypeKinds::TK_VOID)
        {
            pNewBodyGen->Emit(OpCodes::Ldarg_0);
            pNewBodyGen->Emit(OpCodes::Initobj, pRetType);
        }
        pNewBodyGen->BeginExceptionBlock();
        {
            pNewBodyGen->Emit(OpCodes::Ldnull);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal0_holder);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_holder);
            pNewBodyGen->Emit(OpCodes::Call, m_pLooseCrossDomainAccessor_TryGetIndHolder1Delegate);
            pNewBodyGen->Emit(OpCodes::Brfalse, label0);

            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Initobj, m_pIndInfo);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldstr, m_pTarget->GetAssembly()->GetFullName());
            pNewBodyGen->Emit(OpCodes::Call, m_pIndInfo_set_AssemblyName);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldc_I4, static_cast<INT>(m_pTarget->GetToken()));
            pNewBodyGen->Emit(OpCodes::Call, m_pIndInfo_set_Token);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            EmitInstantiation(pNewBodyGen, pLocal5_0000);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal5_0000);
            pNewBodyGen->Emit(OpCodes::Call, m_pIndInfo_set_Instantiation);
            pNewBodyGen->Emit(OpCodes::Ldnull);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal2_dlgt);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal0_holder);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_dlgt);
            pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndHolder1Delegate_TryGet);
            pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal2_dlgt);
            pNewBodyGen->Emit(OpCodes::Call, m_pLooseCrossDomainAccessor_SafelyCastIndDlgt);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal3_ind);

            if (pRetType->GetKind() != TypeKinds::TK_VOID)
            {
                pNewBodyGen->Emit(OpCodes::Ldarg_0);
                    
                pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal3_ind);
                EmitIndirectParameters(pNewBodyGen, m_pTarget, 1);
                pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndDlgt_Invoke);
                    
                pNewBodyGen->Emit(OpCodes::Stobj, pRetType);
            }
            else
            {
                pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal3_ind);
                EmitIndirectParameters(pNewBodyGen, m_pTarget);
                pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndDlgt_Invoke);
            }
            pNewBodyGen->Emit(OpCodes::Leave_S, label1);

            pNewBodyGen->MarkLabel(label0);
        }
        pNewBodyGen->BeginCatchBlock(m_pException);
        {
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal4_e);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal4_e);
            pNewBodyGen->Emit(OpCodes::Ldstr, L"Urasandesu.Prig.Framework.FlowControlException");
            pNewBodyGen->Emit(OpCodes::Call, m_pLooseCrossDomainAccessor_IsInstanceOfIdentity);
            pNewBodyGen->Emit(OpCodes::Brtrue_S, label2);
                
            pNewBodyGen->Emit(OpCodes::Rethrow);
                
            pNewBodyGen->MarkLabel(label2);
        }
        pNewBodyGen->EndExceptionBlock();
            
        pNewBodyGen->Emit(OpCodes::Br_S, label3);
        pNewBodyGen->MarkLabel(label1);
        pNewBodyGen->Emit(OpCodes::Ldc_I4_1);
        pNewBodyGen->Emit(OpCodes::Ret);
        pNewBodyGen->MarkLabel(label3);
        pNewBodyGen->Emit(OpCodes::Ldc_I4_0);
        pNewBodyGen->Emit(OpCodes::Ret);

        auto size = pNewBodyGen->GetInstructions().size();

        CPPANONYM_V_LOG1("Processing time to emit indirect method body details: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            
        return size;
    }



    void DelegatedMethodPreparation::EmitMethodBody(MethodGenerator *pMethodGen, TempPtr<FunctionProfiler> &pFuncProf) const
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto mdt = pMethodGen->GetToken();
            
        auto timer = cpu_timer();
        {
            CPPANONYM_LOG_NAMED_SCOPE("Modifying method");

            auto subTimer = cpu_timer();
                
            auto pNewBodyProf = pFuncProf->NewFunctionBody();
            auto *pNewBodyGen = pNewBodyProf->GetMethodBodyGenerator(pMethodGen);

            EmitIndirectMethodBody(pNewBodyGen, pMethodGen);

            CPPANONYM_V_LOG2("Processing time to emit indirect method body of the method(Token: 0x%|1$08X|): %|2$s|.", mdt, subTimer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));

            pFuncProf->SetFunctionBody(pNewBodyProf);
        }

        CPPANONYM_V_LOG2("Processing time to modify the method(Token: 0x%|1$08X|): %|2$s|.", mdt, timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }

}   // namespace DelegatedMethodPreparationDetail {
