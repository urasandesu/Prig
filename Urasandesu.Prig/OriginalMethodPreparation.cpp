/* 
 * File: OriginalMethodPreparation.cpp
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

#ifndef INDIRETIONINTERFACES_H
#include <InstanceGetters.h>
#endif

#ifndef ORIGINALMETHODPREPARATION_H
#include <OriginalMethodPreparation.h>
#endif

namespace OriginalMethodPreparationDetail {

    OriginalMethodPreparation::OriginalMethodPreparation() : 
        m_pInt32(nullptr), 
        m_pVoid(nullptr), 
        m_pTryPrigTarget(nullptr), 
        m_pPInvokedTarget(nullptr), 
        m_pNewPInvokeTarget(nullptr), 
        m_mdt(mdTokenNil)
    { }



    void OriginalMethodPreparation::FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;
        using std::wstring;
        using std::vector;

        auto timer = cpu_timer();

        _ASSERTE(prigData.m_pMSCorLibDll);
        auto const *pMSCorLibDll = prigData.m_pMSCorLibDll;
        auto const *pInt32 = pMSCorLibDll->GetType(L"System.Int32");
        auto const *pBoolean = pMSCorLibDll->GetType(L"System.Boolean");
        auto const *pVoid = pMSCorLibDll->GetType(L"System.Void");

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
        m_pVoid = pVoid;
        m_pTryPrigTarget = pTryPrigTarget;
        
        if (pTarget->GetAttribute() & MethodAttributes::MA_PINVOKE_IMPL)
        {
            auto *pNewPInvokeTarget = pTarget->CloneShell(L"Prig" + pTarget->GetName());
            
            pTarget->SetAttributes(pTarget->GetAttribute() & ~MethodAttributes::MA_PINVOKE_IMPL);
            pTarget->SetImplementationFlags(pTarget->GetMethodImplementationFlags() & ~MethodImplAttributes::MIA_PRESERVE_SIG);
            
            CPPANONYM_V_LOG1("Processing time to update method properties and define new P/Invoke: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            timer = cpu_timer();
            
            m_pPInvokedTarget = pTarget;
            m_pNewPInvokeTarget = pNewPInvokeTarget;
        }
    }



    void OriginalMethodPreparation::ResolveIndirectionPreparation(AssemblyGenerator *pAsmGen)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        auto timer = cpu_timer();

        m_pTryPrigTarget = pAsmGen->Resolve(m_pTryPrigTarget);

        auto mdt = mdTokenNil;
        mdt = m_pTryPrigTarget->GetToken();
        CPPANONYM_D_LOGW1(L"Resolved the Token for TryPrigTarget: 0x%|1$08X|", mdt);
        m_mdt = mdt;
        {
            auto const *pDeclaringType = m_pTryPrigTarget->GetDeclaringType();
            if (pDeclaringType && pDeclaringType->IsGenericType())
            {
                m_mdt = m_pTryPrigTarget->GetSourceMethod()->GetToken();
            }
            else if (m_pTryPrigTarget->IsGenericMethod())
            {
                auto const *pDeclaringMethod = m_pTryPrigTarget->GetDeclaringMethod();
                pDeclaringType = pDeclaringMethod->GetDeclaringType();
                if (pDeclaringType && pDeclaringType->IsGenericType())
                    m_mdt = pDeclaringMethod->GetSourceMethod()->GetToken();
                else
                    m_mdt = pDeclaringMethod->GetToken();
            }
        }
        CPPANONYM_D_LOGW1(L"Resolved the Indirectable Token for TryPrigTarget: 0x%|1$08X|", m_mdt);
        if (m_pPInvokedTarget)
        {
            mdt = m_pPInvokedTarget->GetToken();
            CPPANONYM_D_LOGW1(L"Resolved the Token for P/Invoked target: 0x%|1$08X|", mdt);
            mdt = m_pNewPInvokeTarget->GetToken();
            CPPANONYM_D_LOGW1(L"Resolved the Token for new P/Invoke target: 0x%|1$08X|", mdt);
        }

        CPPANONYM_V_LOG1("Processing time to resolve the metadata tokens for original method preparation: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
    }



    void EmitCalliCurrentAppDomainEmpty(MethodBodyGenerator *pNewBodyGen, ProcessProfiler *pProcProf, IType const *pInt32, IType const *pVoid)
    {
        using std::vector;
        
        auto paramTypes = vector<IType const *>();
        paramTypes.push_back(pVoid->MakePointerType());
        
#ifdef _M_IX86
        pNewBodyGen->Emit(OpCodes::Ldc_I4, reinterpret_cast<INT>(pProcProf));
        pNewBodyGen->Emit(OpCodes::Ldc_I4, reinterpret_cast<INT>(&InstanceGettersCurrentAppDomainEmpty));
#else
        pNewBodyGen->Emit(OpCodes::Ldc_I8, reinterpret_cast<LONGLONG>(pProcProf));
        pNewBodyGen->Emit(OpCodes::Ldc_I8, reinterpret_cast<LONGLONG>(&InstanceGettersCurrentAppDomainEmpty));
#endif
        pNewBodyGen->EmitCalli(OpCodes::Calli, CallingConventions::UNMANAGED_CC_STDCALL, pInt32, paramTypes);
    }



    SIZE_T OriginalMethodPreparation::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen, ProcessProfiler *pProcProf) const
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

        EmitCalliCurrentAppDomainEmpty(pNewBodyGen, pProcProf, m_pInt32, m_pVoid);
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
        
        if (m_pNewPInvokeTarget)
        {
            EmitIndirectParameters(pNewBodyGen, pMethodGen);
            pNewBodyGen->Emit(OpCodes::Call, m_pNewPInvokeTarget);
            pNewBodyGen->Emit(OpCodes::Ret);
        }

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

            auto offset = EmitIndirectMethodBody(pNewBodyGen, pMethodGen, pFuncProf->GetProcessProfiler());

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

}   // namespace OriginalMethodPreparationDetail {
