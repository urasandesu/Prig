/* 
 * File: Weaver.cpp
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
#include "Weaver.h"

namespace CWeaverDetail {

    namespace IMethodSigHashDetail {
        
        using Urasandesu::CppAnonym::Traits::HashComputable;
        using Urasandesu::CppAnonym::Collections::SequenceHashValue;

        struct IMethodSigHashImpl : 
            HashComputable<IMethod const *>
        {
            result_type operator()(param_type v) const
            {
                auto hash = [](IParameter const *pParam) { return HashValue(pParam); };
                auto hasRet = static_cast<INT>(v->GetReturnType()->GetKind() != TypeKinds::TK_VOID);
                return hasRet ^ SequenceHashValue(v->GetParameters(), hash);
            }

            static result_type HashValue(IParameter const *pParam)
            {
                auto dwattr = pParam->GetAttribute().Value();
                auto isByRef = static_cast<INT>(pParam->GetParameterType()->IsByRef());
                return dwattr ^ isByRef;
            }
        };

    }   // namespace IMethodSigHashDetail {

    struct IMethodSigHash : 
        IMethodSigHashDetail::IMethodSigHashImpl
    {
    };



    namespace IMethodSigEqualToDetail {

        using Urasandesu::CppAnonym::Traits::EqualityComparable;
        using Urasandesu::CppAnonym::Collections::SequenceEqual;

        struct IMethodSigEqualToImpl : 
            EqualityComparable<IMethod const *>
        {
            result_type operator()(param_type x, param_type y) const
            {
                auto equalTo = [](IParameter const *pParamX, IParameter const *pParamY) { return EqualTo(pParamX, pParamY); };
                auto hasRetX = x->GetReturnType()->GetKind() != TypeKinds::TK_VOID;
                auto hasRetY = y->GetReturnType()->GetKind() != TypeKinds::TK_VOID;
                return hasRetX == hasRetY && SequenceEqual(x->GetParameters(), y->GetParameters(), equalTo);
            }
            
            static result_type EqualTo(IParameter const *pParamX, IParameter const *pParamY)
            {
                auto dwattrX = pParamX->GetAttribute().Value();
                auto isByRefX = pParamX->GetParameterType()->IsByRef();
                auto dwattrY = pParamY->GetAttribute().Value();
                auto isByRefY = pParamY->GetParameterType()->IsByRef();
                return dwattrX == dwattrY && isByRefX == isByRefY;
            }
        };

    }   // namespace IMethodSigEqualToDetail {

    struct IMethodSigEqualTo : 
        IMethodSigEqualToDetail::IMethodSigEqualToImpl
    {
    };



    struct PrigData
    {
        PrigData() : 
            m_indirectablesInit(false)
        { }

        boost::filesystem::path m_modPrigPath;
        boost::unordered_map<mdToken, ICustomAttribute const *> m_indirectables;
        bool m_indirectablesInit;
        boost::unordered_map<IMethod const *, IType const *, IMethodSigHash, IMethodSigEqualTo> m_indDlgtCache;
    };



    CWeaverImpl::CWeaverImpl() : 
        m_pProfInfo(nullptr)
    { }

    
    
    STDMETHODIMP CWeaverImpl::InitializeCore( 
        /* [in] */ IUnknown *pICorProfilerInfoUnk)
    {
        D_WCOUT1(L"InitializeCore(IUnknown *: 0x%|1$08X|)", reinterpret_cast<SIZE_T>(pICorProfilerInfoUnk));

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        m_pProfInfo = pRuntime->GetInfo<ProfilingInfo>();
        
        auto pProcProf = m_pProfInfo->AttachToCurrentProcess(pICorProfilerInfoUnk);
        pProcProf.Persist();
        pProcProf->SetEventMask(ProfilerEvents::PE_MONITOR_APPDOMAIN_LOADS | 
                                ProfilerEvents::PE_MONITOR_MODULE_LOADS | 
                                ProfilerEvents::PE_MONITOR_JIT_COMPILATION | 
                                ProfilerEvents::PE_USE_PROFILE_IMAGES);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ShutdownCore()
    {
        D_WCOUT(L"ShutdownCore()");

        m_pProfInfo->DetachFromCurrentProcess();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        D_WCOUT1(L"AppDomainCreationStartedCore(AppDomainID: 0x%|1$08X|)", appDomainId);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        D_WCOUT2(L"AppDomainCreationFinishedCore(AppDomainID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", appDomainId, hrStatus);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pDomainProf = pProcProf->AttachToAppDomain(appDomainId);
        pDomainProf.Persist();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        D_WCOUT1(L"AppDomainShutdownStartedCore(AppDomainID: 0x%|1$08X|)", appDomainId);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromAppDomain(appDomainId);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        D_WCOUT2(L"AppDomainShutdownFinishedCore(AppDomainID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", appDomainId, hrStatus);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ModuleLoadStartedCore( 
        /* [in] */ ModuleID moduleId)
    {
        D_WCOUT1(L"ModuleLoadStartedCore(ModuleID: 0x%|1$08X|)", moduleId);

        return S_OK;
    }
        
    
    
    mdToken GetIndirectableToken(ICustomAttribute const *pIndirectableAttr)
    {
        using boost::get;

        auto const &args = pIndirectableAttr->GetConstructorArguments();
        _ASSERTE(args.size() == 1);
        return get<UINT>(args[0]);
    }



    STDMETHODIMP CWeaverImpl::ModuleLoadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        using boost::lexical_cast;
        using boost::filesystem::exists;
        using boost::filesystem::path;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;

        D_WCOUT2(L"ModuleLoadFinishedCore(ModuleID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", moduleId, hrStatus);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pModProf = pProcProf->AttachToModule(moduleId);
        auto modPath = path(pModProf->GetName());
        auto modPrigPath = path(modPath.stem().native() + L".Prig.dll");
        if (!exists(modPrigPath))
            return S_OK;
        
        D_WCOUT1(L"    Detour module: %|1$s| is found. Start to modify the module.", modPrigPath.native());
        pModProf.Persist();
        auto pAsmProf = pModProf->AttachToAssembly();
        pAsmProf.Persist();
        auto pDomainProf = pAsmProf->AttachToAppDomain();
        pDomainProf.Persist();

        auto asmId = lexical_cast<wstring>(pAsmProf->GetID());
        auto pData = pDomainProf->GetData(asmId);
        if (!pData)
        {
            auto *pPrigData = new PrigData();
            pData = AnyPtr(pPrigData);
            pPrigData->m_modPrigPath = modPrigPath;
            pDomainProf->SetData(asmId, pData);
        }

        auto *pAsmGen = pAsmProf->GetAssemblyGenerator();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ModuleUnloadStartedCore( 
        /* [in] */ ModuleID moduleId)
    {
        D_WCOUT1(L"ModuleUnloadStartedCore(ModuleID: 0x%|1$08X|)", moduleId);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromModule(moduleId);

        return S_OK;
    }


        
    STDMETHODIMP CWeaverImpl::ModuleUnloadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        D_WCOUT2(L"ModuleUnloadFinishedCore(ModuleID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", moduleId, hrStatus);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::JITCompilationStartedCore( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock)
    {
        using boost::lexical_cast;
        using boost::filesystem::path;
        using std::vector;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;
        
        D_WCOUT2(L"JITCompilationStartedCore(FunctionID: 0x%|1$08X|, BOOL: 0x%|2$08X|)", functionId, fIsSafeToBlock);
        
        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pFuncProf = pProcProf->AttachToFunction(functionId);
        auto pModProf = pFuncProf->AttachToModule();
        if (!pModProf.IsPersisted())
            return S_OK;
        
        D_WCOUT(L"    This method is candidate for the module that can detour.");
        
        auto pAsmProf = pModProf->AttachToAssembly();
        auto pDomainProf = pAsmProf->AttachToAppDomain();
        auto *pDisp = pDomainProf->GetMetadataDispenser();
        auto asmId = lexical_cast<wstring>(pAsmProf->GetID());
        auto pData = pDomainProf->GetData(asmId);
        _ASSERTE(pData);
        auto &prigData = *pData.Get<PrigData *>();
        _ASSERTE(!prigData.m_modPrigPath.empty());
        if (!prigData.m_indirectablesInit && pDisp->IsCOMMetaDataDispenserPrepared())
        {
            auto prigFrmwrkPath = canonical(path(L"Urasandesu.Prig.Framework.dll"));
            auto const *pPrigFrmwrk = pDisp->GetAssemblyFrom(prigFrmwrkPath);
            auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();
            auto const *pIndAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
            auto const *pPrigAsm = pDisp->GetAssemblyFrom(prigData.m_modPrigPath);
            auto indAttrs = pPrigAsm->GetCustomAttributes(pIndAttrType, false);
            BOOST_FOREACH (auto const *pIndAttr, indAttrs)
                prigData.m_indirectables[GetIndirectableToken(pIndAttr)] = pIndAttr;

#ifdef OUTPUT_DEBUG
            BOOST_FOREACH (auto const &pair, prigData.m_indirectables)
                D_WCOUT1(L"    Indirectable Token: 0x%|1$08X|", pair.first);
#endif

            prigData.m_indirectablesInit = true;
        }
        
        typedef decltype(prigData.m_indirectables) Indirectables;
        typedef Indirectables::iterator Iterator;
        
        auto const *pMethodGen = pFuncProf->GetMethodGenerator();
        auto mdt = pMethodGen->GetToken();
        D_WCOUT1(L"    Token: 0x%|1$08X|)", mdt);
        auto result = Iterator();
        if ((result = prigData.m_indirectables.find(mdt)) == prigData.m_indirectables.end())
            return S_OK;
        
        D_WCOUT(L"    This method is marked by IndirectableAttribute.");
        pFuncProf.Persist();
        
        auto pNewBodyProf = pFuncProf->NewFunctionBody();
        auto *pNewBodyGen = pNewBodyProf->GetMethodBodyGenerator();
        
        auto const *pBody = pMethodGen->GetMethodBody();
        BOOST_FOREACH (auto const *pLocal, pBody->GetLocals())
            pNewBodyGen->DefineLocal(pLocal->GetLocalType());

        auto offset = EmitNewMethodBody(pNewBodyGen, pDisp, pMethodGen, prigData);
        
        BOOST_FOREACH (auto const *pInst, pBody->GetInstructions())
            pNewBodyGen->Emit(pInst);
        
        BOOST_FOREACH (auto const &exClause, pBody->GetExceptionClauses())
            pNewBodyGen->DefineExceptionClause(exClause, offset);

        pFuncProf->SetFunctionBody(pNewBodyProf);
        
        pProcProf->DetachFromFunction(functionId);

        return S_OK;
    }



    SIZE_T CWeaverImpl::EmitNewMethodBody(MethodBodyGenerator *pNewBodyGen, MetadataDispenser const *pDisp, MethodGenerator const *pMethodGen, PrigData &prigData)
    {
        using boost::filesystem::path;
        using std::vector;

        auto prigFrmwrkPath = canonical(path(L"Urasandesu.Prig.Framework.dll"));
        auto const *pPrigFrmwrk = pDisp->GetAssemblyFrom(prigFrmwrkPath);
        auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();

        auto const *pIndInfo = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndHolder1 = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pIndDlgtAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionDelegateAttribute");
        auto const *pLooseCrossDomainAccessor = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");

        auto const *pIndDlgtInst = GetIndirectionDelegateInstance(pMethodGen, pPrigFrmwrkDll, pIndDlgtAttrType, prigData);
        auto const *pIndHolder1IndDlgtInst = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndDlgtInst);
            pIndHolder1IndDlgtInst = pIndHolder1->MakeGenericType(genericArgs);
        }

        auto const *pIndInfo_set_AssemblyName = pIndInfo->GetMethod(L"set_AssemblyName");
        auto const *pIndInfo_set_Token = pIndInfo->GetMethod(L"set_Token");
        auto const *pIndDlgtInst_Invoke = pIndDlgtInst->GetMethod(L"Invoke");
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        auto const *pIndHolder1IndDlgtInst_TryGet = pIndHolder1IndDlgtInst->GetMethod(L"TryGet");

        auto const *pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndHolder1IndDlgtInst);
            pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst = pLooseCrossDomainAccessor_TryGet->MakeGenericMethod(genericArgs);
        }

        auto *pLocal0_holder = pNewBodyGen->DefineLocal(pIndHolder1IndDlgtInst);
        auto *pLocal1_info = pNewBodyGen->DefineLocal(pIndInfo);
        auto *pLocal2_ind = pNewBodyGen->DefineLocal(pIndDlgtInst);

        auto label0 = pNewBodyGen->DefineLabel();

        pNewBodyGen->Emit(OpCodes::Ldnull);
        pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal0_holder);
        pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_holder);
        pNewBodyGen->Emit(OpCodes::Call, pLooseCrossDomainAccessor_TryGetIndHolderIndDlgtInst);
        pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

        pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
        pNewBodyGen->Emit(OpCodes::Initobj, pIndInfo);
        pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
        pNewBodyGen->Emit(OpCodes::Ldstr, pMethodGen->GetAssembly()->GetFullName());
        pNewBodyGen->Emit(OpCodes::Call, pIndInfo_set_AssemblyName);
        pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_info);
        pNewBodyGen->Emit(OpCodes::Ldc_I4, static_cast<INT>(pMethodGen->GetToken()));
        pNewBodyGen->Emit(OpCodes::Call, pIndInfo_set_Token);
        pNewBodyGen->Emit(OpCodes::Ldnull);
        pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal2_ind);
        pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal0_holder);
        pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal1_info);
        pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_ind);
        pNewBodyGen->Emit(OpCodes::Callvirt, pIndHolder1IndDlgtInst_TryGet);
        pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

        pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal2_ind);
        EmitParameters(pNewBodyGen, pMethodGen);
        pNewBodyGen->Emit(OpCodes::Callvirt, pIndDlgtInst_Invoke);
        pNewBodyGen->Emit(OpCodes::Ret);

        pNewBodyGen->MarkLabel(label0);
        
        return pNewBodyGen->GetInstructions().size();
    }


    
    void CWeaverImpl::EmitParameters(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen)
    {
        // TODO: インスタンスメソッドの場合はどうする？
        auto isStatic = pMethodGen->IsStatic();
        auto const &params = pMethodGen->GetParameters();
        BOOST_FOREACH (auto const &pParam, params)
        {
            auto position = pParam->GetPosition();
            if (isStatic)
                --position;
            
            switch (position)
            {
                case 0:
                    pNewBodyGen->Emit(OpCodes::Ldarg_0);
                    break;
                case 1:
                    pNewBodyGen->Emit(OpCodes::Ldarg_1);
                    break;
                case 2:
                    pNewBodyGen->Emit(OpCodes::Ldarg_2);
                    break;
                case 3:
                    pNewBodyGen->Emit(OpCodes::Ldarg_3);
                    break;
                default:
                    BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException());
            }
        }
    }



    IType const *CWeaverImpl::GetIndirectionDelegateInstance(IMethod const *pTarget, IModule const *pIndDll, IType const *pIndDlgtAttrType, PrigData &prigData) const
    {
        using boost::adaptors::filtered;
        using std::vector;
        using Urasandesu::CppAnonym::Collections::FindIf;
        using Urasandesu::CppAnonym::CppAnonymCOMException;
        
        // check whether the delegate is cached
        {
            auto result = prigData.m_indDlgtCache.find(pTarget);
            if (result != prigData.m_indDlgtCache.end())
                return (*result).second;
        }
        
        // find IndirectionDelegate which has same signature with the target method and cache it.
        {
            auto const &types = pIndDll->GetTypes();
            auto isIndDlgt = [pIndDlgtAttrType](IType const *pType) { return pType->IsDefined(pIndDlgtAttrType, false); };
            auto indDlgts = types | filtered(isIndDlgt);
            auto isTarget = [pTarget](IType const *pType) { return IMethodSigEqualTo()(pType->GetMethod(L"Invoke"), pTarget); };
            auto result = FindIf(indDlgts, isTarget);
            if (!result)
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(CLDB_E_RECORD_NOTFOUND));
            
            auto const *pIndDlgt = *result;
            auto genericArgs = vector<IType const *>();
            auto const &params = pTarget->GetParameters();
            BOOST_FOREACH (auto const &pParam, params)
            {
                auto const *pParamType = pParam->GetParameterType();
                genericArgs.push_back(pParamType->IsByRef() ? pParamType->GetDeclaringType() : pParamType);
            }
            if (pTarget->GetReturnType()->GetKind() != TypeKinds::TK_VOID)
                genericArgs.push_back(pTarget->GetReturnType());
            auto const *pIndDlgtInst = pIndDlgt->MakeGenericType(genericArgs);
            prigData.m_indDlgtCache[pTarget] = pIndDlgtInst;
            return pIndDlgtInst;
        }
    }

}   // namespace CWeaverDetail {



HRESULT CWeaver::FinalConstruct()
{
    D_WCOUT(L"CWeaver::FinalConstruct()");
    
    //::_CrtDbgBreak();
    
    return S_OK;
}

void CWeaver::FinalRelease()
{
}
