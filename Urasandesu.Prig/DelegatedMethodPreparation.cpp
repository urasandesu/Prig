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
        m_pIndHolder1IndDlgtInst(nullptr), 
        m_pIndInfo(nullptr), 
        m_pIndDlgtInst(nullptr), 
        m_pException(nullptr), 
        m_pTarget(nullptr), 
        m_pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst(nullptr), 
        m_pIndInfo_set_AssemblyName(nullptr), 
        m_pIndInfo_set_Token(nullptr), 
        m_pIndHolder1IndDlgtInst_TryGet(nullptr), 
        m_pIndDlgtInst_Invoke(nullptr), 
        m_pLooseCrossDomainAccessor_IsInstanceOfIdentity(nullptr)
    { }



    void DelegatedMethodPreparation::FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;
        using std::vector;

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

}   // namespace DelegatedMethodPreparationDetail {
