/* 
 * File: PrigData.cpp
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

#ifndef URASANDESU_PRIG_IMETHODSIGHASH_H
#include <Urasandesu/Prig/IMethodSigHash.h>
#endif

#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTO_H
#include <Urasandesu/Prig/IMethodSigEqualTo.h>
#endif

#ifndef INSTANCEGETTERS_H
#include <InstanceGetters.h>
#endif

namespace PrigDataDetail {

    void EmitIndirectParameters(MethodBodyGenerator *pNewBodyGen, IMethod const *pMethodGen, INT offset = 0)
    {
        using namespace Urasandesu::CppAnonym::Collections;

        auto isStatic = pMethodGen->IsStatic();
        auto staticOffset = !isStatic ? 0 : 1;
        auto const &params = pMethodGen->GetParameters();
        
        auto paramIndexes = RapidVector<ULONG>();
        if (!isStatic)
            paramIndexes.push_back(0 + offset);
        BOOST_FOREACH (auto const &pParam, params)
            paramIndexes.push_back(pParam->GetPosition() - staticOffset + offset);
        
        BOOST_FOREACH (auto paramIndex, paramIndexes)
        {
            if (paramIndex == 0)
                pNewBodyGen->Emit(OpCodes::Ldarg_0);
            else if (paramIndex == 1)
                pNewBodyGen->Emit(OpCodes::Ldarg_1);
            else if (paramIndex == 2)
                pNewBodyGen->Emit(OpCodes::Ldarg_2);
            else if (paramIndex == 3)
                pNewBodyGen->Emit(OpCodes::Ldarg_3);
            else if (4 <= paramIndex && paramIndex <= 255)
                pNewBodyGen->Emit(OpCodes::Ldarg_S, static_cast<BYTE>(paramIndex));
            else
                pNewBodyGen->Emit(OpCodes::Ldarg, static_cast<SHORT>(paramIndex));
        }
    }



    IType const *MakeGenericExplicitItsInstance(IType const *pTarget)
    {
        auto const *pAsm = pTarget->GetAssembly();
        
        auto genericParamPos = 0ul;
        auto genericArgs = vector<IType const *>();
        BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
            genericArgs.push_back(pAsm->GetGenericTypeParameter(genericParamPos++));
        
        return pTarget->MakeGenericType(genericArgs);
    }

    IMethod const *MakeGenericExplicitItsInstance(IMethod const *pTarget)
    {
        auto const *pAsm = pTarget->GetAssembly();
        
        auto genericParamPos = 0ul;
        auto genericArgs = vector<IType const *>();
        BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
            genericArgs.push_back(pAsm->GetGenericMethodParameter(genericParamPos++));
        
        return pTarget->MakeGenericMethod(genericArgs);
    }

    void OriginalMethodPreparation::FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        _ASSERTE(prigData.m_pMSCorLibDll);
        auto const *pMSCorLibDll = prigData.m_pMSCorLibDll;
        auto const *pInt32 = pMSCorLibDll->GetType(L"System.Int32");
        auto const *pBoolean = pMSCorLibDll->GetType(L"System.Boolean");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();
            
        auto const *pTryPrigTarget = static_cast<IMethod *>(nullptr);
        {
            auto *pDeclaringTypeGen = pTarget->GetDeclaringTypeGenerator();
            auto name = wstring(L"TryPrig") + pTarget->GetName();
            auto attr = MethodAttributes::MA_PRIVATE | MethodAttributes::MA_HIDE_BY_SIG | MethodAttributes::MA_STATIC;
            auto callingConvention = CallingConventions::CC_STANDARD;
            auto const *pRetType = pBoolean;
            auto paramTypes = vector<IType const *>();
            {
                if (pTarget->GetReturnType()->GetKind() != TypeKinds::TK_VOID)
                    paramTypes.push_back(pTarget->GetReturnType()->MakeByRefType());
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
                }
                auto const &params = pTarget->GetParameters();
                BOOST_FOREACH (auto const &pParam, params)
                {
                    auto const *pParamType = pParam->GetParameterType();
                    paramTypes.push_back(pParamType);
                }
            }
            auto *pTryPrigTargetGen = pDeclaringTypeGen->DefineMethod(name, attr, callingConvention, pRetType, paramTypes);
            if (pTarget->IsGenericMethod())
            {
                auto names = vector<wstring>();
                auto const &genericArgs = pTarget->GetGenericArguments();
                BOOST_FOREACH (auto const &pGenericArg, genericArgs)
                    names.push_back(pGenericArg->GetFullName());
                
                pTryPrigTargetGen->DefineGenericParameters(names);
            }
            if (pDeclaringTypeGen->IsGenericType())
                pTryPrigTargetGen = TypeGenerator::GetMethod(MakeGenericExplicitItsInstance(pDeclaringTypeGen), pTryPrigTargetGen);
            pTryPrigTarget = pTryPrigTargetGen;
            if (pTryPrigTarget->IsGenericMethod())
                pTryPrigTarget = MakeGenericExplicitItsInstance(pTryPrigTarget);
        }
            
        CPPANONYM_V_LOG1("Processing time to define the wrapper to suppress generic instantiation too early: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();
            
        m_pInt32 = pInt32;
        m_pTryPrigTarget = pTryPrigTarget;
    }

    void OriginalMethodPreparation::ResolveIndirectionPreparation(AssemblyGenerator *pAsmGen)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        m_pInt32 = pAsmGen->Resolve(m_pInt32);
        m_pTryPrigTarget = pAsmGen->Resolve(m_pTryPrigTarget);

        auto mdt = mdTokenNil;
        mdt = m_pInt32->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for Int32: 0x%|1$08X|", mdt);
        mdt = m_pTryPrigTarget->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for TryPrigTarget: 0x%|1$08X|", mdt);
        m_mdt = mdt;
        {
            auto const *pDeclaringType = m_pTryPrigTarget->GetDeclaringType();
            if (pDeclaringType && pDeclaringType->IsGenericType())
                m_mdt = m_pTryPrigTarget->GetSourceMethod()->GetToken();
            else if (m_pTryPrigTarget->IsGenericMethod())
                m_mdt = m_pTryPrigTarget->GetDeclaringMethod()->GetToken();
        }

        CPPANONYM_V_LOG1("Processing time to resolve the metadata tokens for original method preparation: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }

    SIZE_T OriginalMethodPreparation::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen) const
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        CPPANONYM_LOG_FUNCTION();

        auto timer = cpu_timer();

        auto const *pRetType = pMethodGen->GetReturnType();

        auto *pLocal0_result = static_cast<LocalGenerator *>(nullptr);
        if (pRetType->GetKind() != TypeKinds::TK_VOID)
            pLocal0_result = pNewBodyGen->DefineLocal(pRetType);

        auto label0 = pNewBodyGen->DefineLabel();

#ifdef _M_IX86
        auto funcPtr = reinterpret_cast<INT>(&InstanceGettersEmpty);
        pNewBodyGen->Emit(OpCodes::Ldc_I4, funcPtr);
#else
        auto funcPtr = reinterpret_cast<LONGLONG>(&InstanceGettersEmpty);
        pNewBodyGen->Emit(OpCodes::Ldc_I8, funcPtr);
#endif
        pNewBodyGen->EmitCalli(OpCodes::Calli, CallingConventions::UNMANAGED_CC_STDCALL, m_pInt32, MetadataSpecialValues::EMPTY_TYPES);
        pNewBodyGen->Emit(OpCodes::Brtrue_S, label0);
            
        if (pRetType->GetKind() != TypeKinds::TK_VOID)
        {
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_result);
            pNewBodyGen->Emit(OpCodes::Initobj, pRetType);
        }
        if (pRetType->GetKind() != TypeKinds::TK_VOID)
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_result);
        EmitIndirectParameters(pNewBodyGen, pMethodGen);
        pNewBodyGen->Emit(OpCodes::Call, m_pTryPrigTarget);
        pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);
            
        if (pRetType->GetKind() != TypeKinds::TK_VOID)
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal0_result);
        pNewBodyGen->Emit(OpCodes::Ret);
            
        pNewBodyGen->MarkLabel(label0);

        auto size = pNewBodyGen->GetInstructions().size();

        CPPANONYM_V_LOG1("Processing time to emit indirect method body details: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            
        return size;
    }
        
    void OriginalMethodPreparation::EmitMethodBody(MethodGenerator *pMethodGen, TempPtr<FunctionProfiler> &pFuncProf) const
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
            
            auto const *pBody = pMethodGen->GetMethodBody();
            BOOST_FOREACH (auto const *pLocal, pBody->GetLocals())
                pNewBodyGen->DefineLocal(pLocal->GetLocalType());

            CPPANONYM_V_LOG2("Processing time to define locals of the method(Token: 0x%|1$08X|): %|2$s|.", mdt, subTimer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            subTimer = cpu_timer();

            auto offset = EmitIndirectMethodBody(pNewBodyGen, pMethodGen);

            CPPANONYM_V_LOG2("Processing time to emit indirect method body of the method(Token: 0x%|1$08X|): %|2$s|.", mdt, subTimer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            subTimer = cpu_timer();
            
            BOOST_FOREACH (auto const *pInst, pBody->GetInstructions())
                pNewBodyGen->Emit(pInst);

            CPPANONYM_V_LOG2("Processing time to emit original method body of the method(Token: 0x%|1$08X|): %|2$s|.", mdt, subTimer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            subTimer = cpu_timer();
            
            BOOST_FOREACH (auto const &exClause, pBody->GetExceptionClauses())
                pNewBodyGen->DefineExceptionClause(exClause, offset);

            CPPANONYM_V_LOG2("Processing time to define exception clauses of the method(Token: 0x%|1$08X|): %|2$s|.", mdt, subTimer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));

            pFuncProf->SetFunctionBody(pNewBodyProf);
        }

        CPPANONYM_V_LOG2("Processing time to modify the method(Token: 0x%|1$08X|): %|2$s|.", mdt, timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }



    class ExplicitThis : 
        public IParameter
    {
    public:
        ExplicitThis() : 
            m_pParamType(nullptr)
        { }

        mdToken GetToken() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ULONG GetPosition() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        std::wstring const &GetName() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ParameterAttributes GetAttribute() const { return ParameterAttributes::PA_NONE; }
        IType const *GetParameterType() const { return m_pParamType; }
        Signature const &GetSignature() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IMethod const *GetMethod() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IProperty const *GetProperty() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ParameterProvider const &GetMember() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IAssembly const *GetAssembly() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IParameter const *GetSourceParameter() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        bool Equals(IParameter const *pParam) const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        size_t GetHashCode() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        void OutDebugInfo() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }

        void Set(IType const *pParamType)
        {
            _ASSERTE(!m_pParamType);
            _ASSERTE(pParamType);
            m_pParamType = pParamType;
        }

    private:
        IType const *m_pParamType;
    };

    class IndirectionDelegateFinder
    {
    public:
        IndirectionDelegateFinder(IMethod const *pTarget) : 
            m_pTarget(pTarget)
        { }

        bool operator ()(IType const *pType) const
        {
            using Urasandesu::Prig::IMethodSigEqualTo;

            auto pType_Invoke = pType->GetMethod(L"Invoke");
            _ASSERTE(pType_Invoke);
            auto params = vector<IParameter const *>();
            auto explicitThis = ExplicitThis();
            if (m_pTarget->IsStatic())
            {
                params = m_pTarget->GetParameters();
            }
            else
            {
                auto const *pDeclaringType = m_pTarget->GetDeclaringType();
                if (pDeclaringType->IsValueType())
                    pDeclaringType = pDeclaringType->MakeByRefType();
                explicitThis.Set(pDeclaringType);
                params.push_back(&explicitThis);
                params.insert(params.end(), m_pTarget->GetParameters().begin(), m_pTarget->GetParameters().end());
            }

            return IMethodSigEqualTo::ReturnTypeEqual(pType_Invoke->GetReturnType(), m_pTarget->GetReturnType()) && 
                    IMethodSigEqualTo::ParametersEqual(pType_Invoke->GetParameters(), params);
        }

    private:
        IMethod const *m_pTarget;
    };

    IType const *GetIndirectionDelegateInstance(IMethod const *pTarget, IType const *pIndDlgtAttrType, PrigData &prigData)
    {
        CPPANONYM_LOG_FUNCTION();

        using boost::adaptors::filtered;
        using boost::timer::cpu_timer;
        using boost::timer::default_places;
        using boost::copy;
        using std::back_inserter;
        using Urasandesu::CppAnonym::Collections::FindIf;
        using Urasandesu::CppAnonym::CppAnonymCOMException;

        auto timer = cpu_timer();
        
        auto &dlgtsList = prigData.m_indirectionDelegatesList;
        BOOST_FOREACH (auto &pDlgts, dlgtsList)
        {
            CPPANONYM_V_LOG1("Processing time to check whether the delegate is cached: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            timer = cpu_timer();

            // enumerate IndirectionDelegate and cache them if needed.
            if (!pDlgts.m_indirectionDelegatesInit)
            {
                CPPANONYM_LOG_NAMED_SCOPE("!pDlgts->m_indirectionDelegatesInit");
                auto types = pDlgts.m_pIndirectionDelegatesAssembly->GetTypes();

                CPPANONYM_V_LOG1("Processing time to enumerate IndirectionDelegate and cache them 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
                timer = cpu_timer();

                auto isIndDlgt = [pIndDlgtAttrType](IType const *pType) { return pType->IsDefined(pIndDlgtAttrType); };
                auto indDlgts = types | filtered(isIndDlgt);
                copy(indDlgts, back_inserter(pDlgts.m_indirectionDelegates));

                if (CPPANONYM_D_LOG_ENABLED())
                {
                    BOOST_FOREACH (auto const &pIndDlgt, pDlgts.m_indirectionDelegates)
                        CPPANONYM_D_LOGW1(L"IndirectionDelegate Token: 0x%|1$08X|", pIndDlgt->GetToken());
                }

                pDlgts.m_indirectionDelegatesInit = true;

                CPPANONYM_V_LOG1("Processing time to enumerate IndirectionDelegate and cache them 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
                timer = cpu_timer();
            }
            
            // find IndirectionDelegate which has same signature with the target method.
            auto const *pIndDlgt = static_cast<IType *>(nullptr);
            {
                auto result = FindIf(pDlgts.m_indirectionDelegates, IndirectionDelegateFinder(pTarget));
                if (!result)
                    continue;
                
                pIndDlgt = *result;

                CPPANONYM_V_LOG1("Processing time to find IndirectionDelegate which has same signature with the target method and cache it: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
                timer = cpu_timer();
            }

            // if the delegate is a generic type, make Generic Type Instance of IndirectionDelegate.
            if (!pIndDlgt->IsGenericType())
            {
                CPPANONYM_LOG_NAMED_SCOPE("!pIndDlgt->IsGenericType()");
                return pIndDlgt;
            }
            else
            {
                CPPANONYM_LOG_NAMED_SCOPE("pIndDlgt->IsGenericType()");
                auto genericArgs = vector<IType const *>();
                if (!pTarget->IsStatic())
                {
                    auto const *pDeclaringType = pTarget->GetDeclaringType();
                    if (pDeclaringType->IsGenericType())
                        pDeclaringType = MakeGenericExplicitItsInstance(pDeclaringType);
                    genericArgs.push_back(pDeclaringType);
                }
                auto const &params = pTarget->GetParameters();
                BOOST_FOREACH (auto const &pParam, params)
                {
                    auto const *pParamType = pParam->GetParameterType();
                    genericArgs.push_back(pParamType->IsByRef() ? pParamType->GetDeclaringType() : pParamType);
                }
                if (pTarget->GetReturnType()->GetKind() != TypeKinds::TK_VOID)
                    genericArgs.push_back(pTarget->GetReturnType());
                auto const *pIndDlgtInst = pIndDlgt->MakeGenericType(genericArgs);

                CPPANONYM_V_LOG1("Processing time to make Generic Type Instance of IndirectionDelegate: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
                timer = cpu_timer();

                return pIndDlgtInst;
            }
        }
        
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(CLDB_E_RECORD_NOTFOUND));
    }

    void DelegatedMethodPreparation::FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        _ASSERTE(prigData.m_pMSCorLibDll);
        auto const *pMSCorLibDll = prigData.m_pMSCorLibDll;
        auto const *pException = pMSCorLibDll->GetType(L"System.Exception");
        auto const *pType = pMSCorLibDll->GetType(L"System.Type");
        auto const *pInt32 = pMSCorLibDll->GetType(L"System.Int32");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pType_GetTypeFromHandle = pType->GetMethod(L"GetTypeFromHandle");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        CPPANONYM_V_LOG1("Processing time to find Urasandesu.Prig.Framework: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        _ASSERTE(prigData.m_pPrigFrameworkDll);
        auto const *pPrigFrmwrkDll = prigData.m_pPrigFrameworkDll;
        auto const *pIndInfo = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndHolder1 = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pIndDlgtAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionDelegateAttribute");
        auto const *pLooseCrossDomainAccessor = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");
        auto const *pFlowControlException = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.FlowControlException");

        CPPANONYM_V_LOG1("Processing time to get indirection definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndDlgtInst = GetIndirectionDelegateInstance(pTarget, pIndDlgtAttrType, prigData);

        CPPANONYM_V_LOG1("Processing time to get indirection delegate instance 1-1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndHolder1IndDlgtInst = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndDlgtInst);
            pIndHolder1IndDlgtInst = pIndHolder1->MakeGenericType(genericArgs);
        }

        CPPANONYM_V_LOG1("Processing time to get indirection delegate instance 1-2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndInfo_set_AssemblyName = pIndInfo->GetMethod(L"set_AssemblyName");
        auto const *pIndInfo_set_Token = pIndInfo->GetMethod(L"set_Token");
        auto const *pIndDlgtInst_Invoke = pIndDlgtInst->GetMethod(L"Invoke");
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        auto const *pLooseCrossDomainAccessor_IsInstanceOfIdentity = pLooseCrossDomainAccessor->GetMethod(L"IsInstanceOfIdentity");
        auto const *pIndHolder1IndDlgtInst_TryGet = pIndHolder1IndDlgtInst->GetMethod(L"TryGet");

        CPPANONYM_V_LOG1("Processing time to get indirection definitions 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndHolder1IndDlgtInst);
            pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = pLooseCrossDomainAccessor_TryGet->MakeGenericMethod(genericArgs);
        }

        CPPANONYM_V_LOG1("Processing time to get indirection delegate instance 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            
        m_pIndHolder1IndDlgtInst = pIndHolder1IndDlgtInst;
        m_pIndInfo = pIndInfo;
        m_pIndDlgtInst = pIndDlgtInst;
        m_pException = pException;
        m_pTarget = pTarget;
        m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst;
        m_pIndInfo_set_AssemblyName = pIndInfo_set_AssemblyName;
        m_pIndInfo_set_Token = pIndInfo_set_Token;
        m_pIndHolder1IndDlgtInst_TryGet = pIndHolder1IndDlgtInst_TryGet;
        m_pIndDlgtInst_Invoke = pIndDlgtInst_Invoke;
        m_pLooseCrossDomainAccessor_IsInstanceOfIdentity = pLooseCrossDomainAccessor_IsInstanceOfIdentity;
    }

    void DelegatedMethodPreparation::ResolveIndirectionPreparation(AssemblyGenerator *pAsmGen)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        m_pIndHolder1IndDlgtInst = pAsmGen->Resolve(m_pIndHolder1IndDlgtInst);
        m_pIndInfo = pAsmGen->Resolve(m_pIndInfo);
        m_pIndDlgtInst = pAsmGen->Resolve(m_pIndDlgtInst);
        m_pException = pAsmGen->Resolve(m_pException);
        m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = pAsmGen->Resolve(m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst);
        m_pIndInfo_set_AssemblyName = pAsmGen->Resolve(m_pIndInfo_set_AssemblyName);
        m_pIndInfo_set_Token = pAsmGen->Resolve(m_pIndInfo_set_Token);
        m_pIndHolder1IndDlgtInst_TryGet = pAsmGen->Resolve(m_pIndHolder1IndDlgtInst_TryGet);
        m_pIndDlgtInst_Invoke = pAsmGen->Resolve(m_pIndDlgtInst_Invoke);
        m_pLooseCrossDomainAccessor_IsInstanceOfIdentity = pAsmGen->Resolve(m_pLooseCrossDomainAccessor_IsInstanceOfIdentity);

        auto mdt = mdTokenNil;
        mdt = m_pIndHolder1IndDlgtInst->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionHolder<IndirectionDelegate>: 0x%|1$08X|", mdt);
        mdt = m_pIndInfo->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionInfo: 0x%|1$08X|", mdt);
        mdt = m_pIndDlgtInst->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionDelegate: 0x%|1$08X|", mdt);
        mdt = m_pException->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for Exception: 0x%|1$08X|", mdt);
        mdt = m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for LooseCrossDomainAccessor.TryGet<IndirectionHolder<IndirectionDelegate>>: 0x%|1$08X|", mdt);
        mdt = m_pIndInfo_set_AssemblyName->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionInfo.set_AssemblyName: 0x%|1$08X|", mdt);
        mdt = m_pIndInfo_set_Token->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionInfo.set_Token: 0x%|1$08X|", mdt);
        mdt = m_pIndHolder1IndDlgtInst_TryGet->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionHolder<IndirectionDelegate>.TryGet: 0x%|1$08X|", mdt);
        mdt = m_pIndDlgtInst_Invoke->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for IndirectionDelegate.Invoke: 0x%|1$08X|", mdt);
        mdt = m_pLooseCrossDomainAccessor_IsInstanceOfIdentity->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for LooseCrossDomainAccessor.IsInstanceOfIdentity: 0x%|1$08X|", mdt);

        CPPANONYM_V_LOG1("Processing time to resolve the metadata tokens for delegate method preparation: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }

    SIZE_T DelegatedMethodPreparation::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen) const
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        CPPANONYM_LOG_FUNCTION();

        auto timer = cpu_timer();

        auto const *pRetType = m_pTarget->GetReturnType();

        auto *pLocal0_holder = pNewBodyGen->DefineLocal(m_pIndHolder1IndDlgtInst);
        auto *pLocal1_info = pNewBodyGen->DefineLocal(m_pIndInfo);
        auto *pLocal2_ind = pNewBodyGen->DefineLocal(m_pIndDlgtInst);
        auto *pLocal3_e = pNewBodyGen->DefineLocal(m_pException);

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
            pNewBodyGen->Emit(OpCodes::Call, m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst);
            pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Initobj, m_pIndInfo);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldstr, m_pTarget->GetAssembly()->GetFullName());
            pNewBodyGen->Emit(OpCodes::Call, m_pIndInfo_set_AssemblyName);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldc_I4, static_cast<INT>(m_pTarget->GetToken()));
            pNewBodyGen->Emit(OpCodes::Call, m_pIndInfo_set_Token);
            pNewBodyGen->Emit(OpCodes::Ldnull);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal2_ind);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal0_holder);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal1_info);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_ind);
            pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndHolder1IndDlgtInst_TryGet);
            pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

            if (pRetType->GetKind() != TypeKinds::TK_VOID)
            {
                pNewBodyGen->Emit(OpCodes::Ldarg_0);
                    
                pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal2_ind);
                EmitIndirectParameters(pNewBodyGen, m_pTarget, 1);
                pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndDlgtInst_Invoke);
                    
                pNewBodyGen->Emit(OpCodes::Stobj, pRetType);
            }
            else
            {
                pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal2_ind);
                EmitIndirectParameters(pNewBodyGen, m_pTarget);
                pNewBodyGen->Emit(OpCodes::Callvirt, m_pIndDlgtInst_Invoke);
            }
            pNewBodyGen->Emit(OpCodes::Leave_S, label1);

            pNewBodyGen->MarkLabel(label0);
        }
        pNewBodyGen->BeginCatchBlock(m_pException);
        {
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal3_e);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal3_e);
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

}   // namespace PrigDataDetail {
