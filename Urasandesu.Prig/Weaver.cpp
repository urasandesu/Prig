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

#ifndef URASANDESU_PRIG_IMETHODSIGHASH_H
#include <Urasandesu/Prig/IMethodSigHash.h>
#endif

#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTO_H
#include <Urasandesu/Prig/IMethodSigEqualTo.h>
#endif

#ifndef URASANDESU_PRIG_PRIGDATA_H
#include <Urasandesu/Prig/PrigData.h>
#endif

namespace CWeaverDetail {

    CWeaverImpl::CWeaverImpl() : 
        m_pProfInfo(nullptr)
    { }

    
    
    STDMETHODIMP CWeaverImpl::InitializeCore( 
        /* [in] */ IUnknown *pICorProfilerInfoUnk)
    {
        using ATL::CComQIPtr;
        using boost::filesystem::current_path;
        using boost::filesystem::directory_iterator;
        using boost::filesystem::is_regular_file;
        using std::regex_search;
        using std::wregex;

        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"InitializeCore(IUnknown *: 0x%|1$X|)", reinterpret_cast<void *>(pICorProfilerInfoUnk));

        auto _ = guard_type(m_lock);

        auto version = wstring(L"v2.0.50727");
        if (CComQIPtr<ICorProfilerInfo3>(pICorProfilerInfoUnk))
            version = wstring(L"v4.0.30319");
        CPPANONYM_D_LOGW1(L"Runtime Version: %|1$s|", version);

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(version);
        m_pProfInfo = pRuntime->GetInfo<ProfilingInfo>();
        
        auto pProcProf = m_pProfInfo->AttachToCurrentProcess(pICorProfilerInfoUnk);
        pProcProf.Persist();
        pProcProf->SetEventMask(ProfilerEvents::PE_MONITOR_APPDOMAIN_LOADS | 
                                ProfilerEvents::PE_MONITOR_MODULE_LOADS | 
                                ProfilerEvents::PE_MONITOR_JIT_COMPILATION | 
                                ProfilerEvents::PE_MONITOR_ENTERLEAVE | 
                                ProfilerEvents::PE_DISABLE_INLINING | 
                                ProfilerEvents::PE_DISABLE_OPTIMIZATIONS | 
                                ProfilerEvents::PE_USE_PROFILE_IMAGES | 
                                ProfilerEvents::PE_DISABLE_TRANSPARENCY_CHECKS_UNDER_FULL_TRUST);
        
        auto currentDir = current_path().native();
        auto patternStr = version + L".v\\d+.\\d+.\\d+.\\d+.((x86)|(AMD64)|(MSIL)).Prig.dll";
        boost::replace_all(patternStr, L".", L"\\.");
        auto pattern = wregex(patternStr);
        for (directory_iterator i(currentDir), i_end; i != i_end; ++i)
        {
            if (!is_regular_file(i->status()))
                continue;
            
            if (!regex_search(i->path().filename().native(), pattern))
                continue;
            
            m_indDllPaths.push_back(i->path().filename());
        }
        if (CPPANONYM_D_LOG_ENABLED())
        {
            BOOST_FOREACH (auto const &indDllPath, m_indDllPaths)
                CPPANONYM_D_LOGW1(L"Indirection DLL: %|1$s|", indDllPath.native());
        }
        
        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ShutdownCore()
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW(L"ShutdownCore()");

        auto _ = guard_type(m_lock);
        
        m_pProfInfo->DetachFromCurrentProcess();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"AppDomainCreationStartedCore(AppDomainID: 0x%|1$X|)", reinterpret_cast<void *>(appDomainId));

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"AppDomainCreationFinishedCore(AppDomainID: 0x%|1$X|, HRESULT: 0x%|2$08X|)", reinterpret_cast<void *>(appDomainId), hrStatus);

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pDomainProf = pProcProf->AttachToAppDomain(appDomainId);
        pDomainProf.Persist();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"AppDomainShutdownStartedCore(AppDomainID: 0x%|1$X|)", reinterpret_cast<void *>(appDomainId));

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromAppDomain(appDomainId);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"AppDomainShutdownFinishedCore(AppDomainID: 0x%|1$X|, HRESULT: 0x%|2$08X|)", reinterpret_cast<void *>(appDomainId), hrStatus);

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ModuleLoadStartedCore( 
        /* [in] */ ModuleID moduleId)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"ModuleLoadStartedCore(ModuleID: 0x%|1$X|)", reinterpret_cast<void *>(moduleId));

        auto _ = guard_type(m_lock);

        return S_OK;
    }
        
    
    
    mdToken GetIndirectableToken(ICustomAttribute const *pIndirectableAttr)
    {
        using boost::get;

        auto const &args = pIndirectableAttr->GetConstructorArguments();
        _ASSERTE(args.size() == 1);
        return get<UINT>(args[0]);
    }



    wstring GetIndirectionDllName(wstring const &modName, RuntimeHost const *pRuntime, TempPtr<AppDomainProfiler> &pDomainProf, TempPtr<AssemblyProfiler> &pAsmProf)
    {
        using std::wostringstream;
        using Urasandesu::CppAnonym::CppAnonymNotSupportedException;

        auto *pDisp = pDomainProf->GetMetadataDispenser();
        auto const *pAsmGen = pAsmProf->GetAssemblyGenerator(pDisp);
        auto targetIndDllName = wostringstream();
        targetIndDllName << modName;
        targetIndDllName << L"." << pRuntime->GetCORVersion();
        targetIndDllName << L".v" << pAsmGen->GetVersion();
        targetIndDllName << L"." << pAsmGen->GetProcessorArchitectures()[0];
        targetIndDllName << L".Prig.dll";
        
        return targetIndDllName.str();
    }

    STDMETHODIMP CWeaverImpl::ModuleLoadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        CPPANONYM_LOG_FUNCTION();

        using boost::adaptors::filtered;
        using boost::lexical_cast;
        using boost::log::current_scope;
        using boost::filesystem::current_path;
        using boost::range::for_each;
        using std::wostringstream;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;

        CPPANONYM_D_LOGW2(L"ModuleLoadFinishedCore(ModuleID: 0x%|1$X|, HRESULT: 0x%|2$08X|)", reinterpret_cast<void *>(moduleId), hrStatus);

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pModProf = pProcProf->AttachToModule(moduleId);
        CPPANONYM_D_LOGW1(L"Current Path: %|1$s|", current_path().native());
        auto modPath = path(pModProf->GetName());
        auto modName = modPath.stem().native();
        
        auto candidateIndDllPaths = unordered_set<path, Hash<path>, EqualTo<path> >();
        auto isCandidate = [&](path const &p) { return p.native().find(modName) != wstring::npos; };
        for_each(m_indDllPaths | filtered(isCandidate), [&](path const &p) { candidateIndDllPaths.insert(p); });
        if (candidateIndDllPaths.empty())
            return S_OK;
        
        
        CPPANONYM_LOG_NAMED_SCOPE("if (!candidateIndDllPaths.empty())");
        if (CPPANONYM_D_LOG_ENABLED())
        {
            auto oss = wostringstream();
            oss << L"Candidate modules:";
            BOOST_FOREACH (auto const &candidateIndDllPath, candidateIndDllPaths)
                oss << boost::wformat(L" \"%|1$s|\"") % candidateIndDllPath.native();
            CPPANONYM_D_LOGW(oss.str());
        }

        auto const *pRuntime = m_pProfInfo->GetRuntime();
        auto pAsmProf = pModProf->AttachToAssembly();
        auto pDomainProf = pAsmProf->AttachToAppDomain();
        auto targetIndDllPath = path(GetIndirectionDllName(modName, pRuntime, pDomainProf, pAsmProf));
        if (candidateIndDllPaths.find(targetIndDllPath) == candidateIndDllPaths.end())
            return S_OK;
        
        
        CPPANONYM_LOG_NAMED_SCOPE("if (candidateIndDllPaths.find(targetIndDllPath) != candidateIndDllPaths.end())");
        CPPANONYM_D_LOGW1(L"Detour module: %|1$s| is found. Start to modify the module.", targetIndDllPath.native());
        pModProf.Persist();
        pAsmProf.Persist();
        pDomainProf.Persist();

        auto asmId = lexical_cast<wstring>(pAsmProf->GetID());
        auto pData = pDomainProf->GetData(asmId);
        if (!pData)
        {
            auto *pPrigData = new PrigData();
            pData = AnyPtr(pPrigData);
            pPrigData->m_indDllPath = targetIndDllPath;
            pDomainProf->SetData(asmId, pData);
        }

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ModuleUnloadStartedCore( 
        /* [in] */ ModuleID moduleId)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"ModuleUnloadStartedCore(ModuleID: 0x%|1$X|)", reinterpret_cast<void *>(moduleId));

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromModule(moduleId);

        return S_OK;
    }


        
    STDMETHODIMP CWeaverImpl::ModuleUnloadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"ModuleUnloadFinishedCore(ModuleID: 0x%|1$X|, HRESULT: 0x%|2$08X|)", reinterpret_cast<void *>(moduleId), hrStatus);

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::JITCompilationStartedCore( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock)
    {
        CPPANONYM_LOG_FUNCTION();

        using boost::lexical_cast;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;
        
        CPPANONYM_D_LOGW2(L"JITCompilationStartedCore(FunctionID: 0x%|1$X|, BOOL: 0x%|2$08X|)", reinterpret_cast<void *>(functionId), fIsSafeToBlock);
        
        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pFuncProf = pProcProf->AttachToFunction(functionId);
        auto pModProf = pFuncProf->AttachToModule();
        if (!pModProf.IsPersisted())
            return S_OK;
        
        CPPANONYM_LOG_NAMED_SCOPE("pModProf.IsPersisted()");
        CPPANONYM_D_LOGW(L"This method is candidate for the module that can detour.");
        
        auto pAsmProf = pModProf->AttachToAssembly();
        auto pDomainProf = pAsmProf->AttachToAppDomain();
        auto *pDisp = pDomainProf->GetMetadataDispenser();
        auto asmId = lexical_cast<wstring>(pAsmProf->GetID());
        auto pData = pDomainProf->GetData(asmId);
        _ASSERTE(pData);
        auto &prigData = *pData.Get<PrigData *>();
        _ASSERTE(!prigData.m_indDllPath.empty());
        if (!prigData.m_indirectablesInit)
        {
            CPPANONYM_LOG_NAMED_SCOPE("!prigData.m_indirectablesInit");
            auto const *pPrigFrmwrk = pDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce");
            auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();
            auto const *pIndAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
            auto const *pIndAsm = pDisp->GetAssemblyFrom(prigData.m_indDllPath);
            auto indAttrs = pIndAsm->GetCustomAttributes(pIndAttrType);
            BOOST_FOREACH (auto const *pIndAttr, indAttrs)
                prigData.m_indirectables[GetIndirectableToken(pIndAttr)] = pIndAttr;

            if (CPPANONYM_D_LOG_ENABLED())
            {
                BOOST_FOREACH (auto const &pair, prigData.m_indirectables)
                    CPPANONYM_D_LOGW1(L"Indirectable Token: 0x%|1$08X|", pair.first);
            }

            prigData.m_indirectablesInit = true;
        }
        
        typedef decltype(prigData.m_indirectables) Indirectables;
        typedef Indirectables::iterator Iterator;
        
        auto *pAsmGen = pAsmProf->GetAssemblyGenerator(pDisp);
        auto *pMethodGen = pFuncProf->GetMethodGenerator(pAsmGen);
        auto mdt = pMethodGen->GetToken();
        CPPANONYM_D_LOGW1(L"Token: 0x%|1$08X|", mdt);
        auto result = Iterator();
        if ((result = prigData.m_indirectables.find(mdt)) == prigData.m_indirectables.end())
            return S_OK;
        
        CPPANONYM_LOG_NAMED_SCOPE("(result = prigData.m_indirectables.find(mdt)) != prigData.m_indirectables.end()");
        CPPANONYM_D_LOGW(L"This method is marked by IndirectableAttribute.");
        pFuncProf.Persist();
        
        auto pNewBodyProf = pFuncProf->NewFunctionBody();
        auto *pNewBodyGen = pNewBodyProf->GetMethodBodyGenerator(pMethodGen);
        
        auto const *pBody = pMethodGen->GetMethodBody();
        BOOST_FOREACH (auto const *pLocal, pBody->GetLocals())
            pNewBodyGen->DefineLocal(pLocal->GetLocalType());

        auto offset = EmitIndirectMethodBody(pNewBodyGen, pDisp, pMethodGen, prigData);
        
        BOOST_FOREACH (auto const *pInst, pBody->GetInstructions())
            pNewBodyGen->Emit(pInst);
        
        BOOST_FOREACH (auto const &exClause, pBody->GetExceptionClauses())
            pNewBodyGen->DefineExceptionClause(exClause, offset);

        pFuncProf->SetFunctionBody(pNewBodyProf);
        
        pProcProf->DetachFromFunction(functionId);

        return S_OK;
    }



    SIZE_T CWeaverImpl::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MetadataDispenser const *pDisp, MethodGenerator const *pMethodGen, PrigData &prigData)
    {
        CPPANONYM_LOG_FUNCTION();

        auto const *pPrigFrmwrk = pDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce");
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
        _ASSERTE(pIndInfo_set_AssemblyName);
        auto const *pIndInfo_set_Token = pIndInfo->GetMethod(L"set_Token");
        _ASSERTE(pIndInfo_set_Token);
        auto const *pIndDlgtInst_Invoke = pIndDlgtInst->GetMethod(L"Invoke");
        _ASSERTE(pIndDlgtInst_Invoke);
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        _ASSERTE(pLooseCrossDomainAccessor_TryGet);
        auto const *pIndHolder1IndDlgtInst_TryGet = pIndHolder1IndDlgtInst->GetMethod(L"TryGet");
        _ASSERTE(pIndHolder1IndDlgtInst_TryGet);

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
        EmitIndirectParameters(pNewBodyGen, pMethodGen);
        pNewBodyGen->Emit(OpCodes::Callvirt, pIndDlgtInst_Invoke);
        pNewBodyGen->Emit(OpCodes::Ret);

        pNewBodyGen->MarkLabel(label0);
        
        return pNewBodyGen->GetInstructions().size();
    }


    
    void CWeaverImpl::EmitIndirectParameters(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen)
    {
        auto isStatic = pMethodGen->IsStatic();
        auto offset = isStatic ? 1 : 0;
        if (!isStatic)
            pNewBodyGen->Emit(OpCodes::Ldarg_0);
    
        auto const &params = pMethodGen->GetParameters();
        BOOST_FOREACH (auto const &pParam, params)
        {
            auto position = pParam->GetPosition() - offset;
        
            if (position == 0)
                pNewBodyGen->Emit(OpCodes::Ldarg_0);
            else if (position == 1)
                pNewBodyGen->Emit(OpCodes::Ldarg_1);
            else if (position == 2)
                pNewBodyGen->Emit(OpCodes::Ldarg_2);
            else if (position == 3)
                pNewBodyGen->Emit(OpCodes::Ldarg_3);
            else if (4 <= position && position <= 255)
                pNewBodyGen->Emit(OpCodes::Ldarg_S, static_cast<BYTE>(position));
            else
                pNewBodyGen->Emit(OpCodes::Ldarg, static_cast<SHORT>(position));
        }
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

    IType const *CWeaverImpl::GetIndirectionDelegateInstance(IMethod const *pTarget, IModule const *pIndDll, IType const *pIndDlgtAttrType, PrigData &prigData) const
    {
        using boost::adaptors::filtered;
        using Urasandesu::CppAnonym::Collections::FindIf;
        using Urasandesu::CppAnonym::CppAnonymCOMException;
        
        auto const *pIndDlgt = static_cast<IType *>(nullptr);
        // check whether the delegate is cached.
        {
            auto result = prigData.m_indDlgtCache.find(pTarget);
            if (result != prigData.m_indDlgtCache.end())
                pIndDlgt = (*result).second;
        }
        
        // find IndirectionDelegate which has same signature with the target method and cache it.
        {
            auto types = pIndDll->GetTypes();
            auto isIndDlgt = [pIndDlgtAttrType](IType const *pType) { return pType->IsDefined(pIndDlgtAttrType); };
            auto indDlgts = types | filtered(isIndDlgt);
            auto result = FindIf(indDlgts, IndirectionDelegateFinder(pTarget));
            if (!result)
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(CLDB_E_RECORD_NOTFOUND));
            
            pIndDlgt = *result;
            prigData.m_indDlgtCache[pTarget] = pIndDlgt;
        }

        // make Generic Type Instance of IndirectionDelegate.
        auto genericArgs = vector<IType const *>();
        if (!pTarget->IsStatic())
        {
            auto const *pDeclaringType = pTarget->GetDeclaringType();
            if (pDeclaringType->IsGenericType())
                pDeclaringType = MakeGenericExplicitThisType(pDeclaringType);
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
        return pIndDlgtInst;
    }



    IType const *CWeaverImpl::MakeGenericExplicitThisType(IType const *pTarget) const
    {
        auto const *pAsm = pTarget->GetAssembly();
        
        auto genericParamPos = 0ul;
        auto genericArgs = vector<IType const *>();
        BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
            genericArgs.push_back(pAsm->GetGenericTypeParameter(genericParamPos++));
        
        return pTarget->MakeGenericType(genericArgs);
    }

}   // namespace CWeaverDetail {



HRESULT CWeaver::FinalConstruct()
{
    using boost::lexical_cast;
    using boost::bad_lexical_cast;
    using Urasandesu::CppAnonym::Environment;

    CPPANONYM_D_LOGW(L"CWeaver::FinalConstruct()");

#ifdef _DEBUG
    auto dbgBreak = 0ul;
    try
    {
        auto strDbgBreak = Environment::GetEnvironmentVariable("URASANDESU_PRIG_DEBUGGING_BREAK");
        dbgBreak = lexical_cast<DWORD>(strDbgBreak);
    }
    catch(bad_lexical_cast const &)
    {
        dbgBreak = -1;
    }
    if (dbgBreak == 0)
        ::_CrtDbgBreak();
    else if (dbgBreak != -1)
        ::Sleep(dbgBreak);
#endif
    
    return S_OK;
}

void CWeaver::FinalRelease()
{
}
