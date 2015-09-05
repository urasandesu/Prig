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

#ifndef PRIGDATA_H
#include <PrigData.h>
#endif

#ifndef INDIRECTIONDELEGATES_H
#include <IndirectionDelegates.h>
#endif

#ifndef ORIGINALMETHODPREPARATION_H
#include <OriginalMethodPreparation.h>
#endif

#ifndef DELEGATEDMETHODPREPARATION_H
#include <DelegatedMethodPreparation.h>
#endif

namespace CWeaverDetail {

    CWeaverImpl::CWeaverImpl() : 
        m_pProfInfo(nullptr)
    { }

    
    
    void GetIndirectionDllPaths(wstring const &version, wstring const &currentDir, vector<path> &indDllPaths)
    {
        using boost::filesystem::directory_iterator;
        using boost::filesystem::is_regular_file;
        using std::regex_search;
        using std::wregex;

        auto patternStr = version + L".v\\d+.\\d+.\\d+.\\d+.((x86)|(AMD64)|(MSIL)).Prig.dll";
        boost::replace_all(patternStr, L".", L"\\.");
        auto pattern = wregex(patternStr);
        for (directory_iterator i(currentDir), i_end; i != i_end; ++i)
        {
            if (!is_regular_file(i->status()))
                continue;
            
            if (!regex_search(i->path().filename().native(), pattern))
                continue;
            
            indDllPaths.push_back(i->path());
        }
    }



    STDMETHODIMP CWeaverImpl::InitializeCore( 
        /* [in] */ IUnknown *pICorProfilerInfoUnk)
    {
        using ATL::CComQIPtr;
        using boost::filesystem::current_path;
        using boost::lexical_cast;
        using boost::bad_lexical_cast;
        using boost::serialization::make_nvp;
        using std::regex_search;
        using std::wregex;
        using Urasandesu::CppAnonym::Collections::FindIf;
        using Urasandesu::CppAnonym::CppAnonymInvalidOperationException;
        using Urasandesu::CppAnonym::Environment;
        using Urasandesu::Prig::PrigConfig;

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

        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"InitializeCore(IUnknown *: 0x%|1$X|)", reinterpret_cast<void *>(pICorProfilerInfoUnk));

        auto _ = guard_type(m_lock);

        auto version = wstring(L"v2.0.50727");
        if (CComQIPtr<ICorProfilerInfo3>(pICorProfilerInfoUnk))
            version = wstring(L"v4.0.30319");
        CPPANONYM_D_LOGW1(L"Runtime Version: %|1$s|", version);
        
        auto procPath = Environment::GetCurrentProcessPath();
        auto targetProcName = Environment::GetEnvironmentVariable(L"URASANDESU_PRIG_TARGET_PROCESS_NAME");
        CPPANONYM_D_LOGW1(L"Current Process Path: %|1$s|", procPath);
        CPPANONYM_D_LOGW1(L"Target Process Name: %|1$s|", targetProcName);
        if (!targetProcName.empty() && !regex_search(procPath.native(), wregex(targetProcName)))
            return S_OK;

        CPPANONYM_LOG_NAMED_SCOPE("if (regex_search(procPath, wregex(targetProcName)))");
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
        
        m_currentDir = Environment::GetEnvironmentVariable(L"URASANDESU_PRIG_CURRENT_DIRECTORY");
        if (m_currentDir.empty())
            m_currentDir = current_path().native();
        CPPANONYM_D_LOGW1(L"Current Directory: %|1$s|", m_currentDir);
        GetIndirectionDllPaths(version, m_currentDir, m_indDllPaths);
        if (CPPANONYM_D_LOG_ENABLED())
        {
            BOOST_FOREACH (auto const &indDllPath, m_indDllPaths)
                CPPANONYM_D_LOGW1(L"Indirection DLL: %|1$s|", indDllPath.native());
        }

        auto config = PrigConfig();
        config.TrySerializeFrom(PrigConfig::GetConfigPath());
            
        auto procDirPath = procPath.parent_path();
        auto result = FindIf(config.Packages, [&procDirPath](PrigPackageConfig const &pkg) { return equivalent(pkg.Source, procDirPath); });
        if (!result)
        {
            auto oss = std::wostringstream();
            oss << L"The package which has the source \"" << procDirPath << L"\" is not found.";
            BOOST_THROW_EXCEPTION(CppAnonymInvalidOperationException(oss.str()));
        }
        
        m_currentPkg = *result;
        
        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ShutdownCore()
    {
        CPPANONYM_LOG_FUNCTION();
        CPPANONYM_D_LOGW(L"ShutdownCore()");

        auto _ = guard_type(m_lock);
        if (!m_pProfInfo)
            return S_OK;
        
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
        if (!m_pProfInfo)
            return S_OK;

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
        if (!m_pProfInfo)
            return S_OK;

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



    wstring GetIndirectionDllName(wstring const &modName, RuntimeHost const *pRuntime, AssemblyGenerator const *pAsmGen)
    {
        using std::wostringstream;

        auto targetIndDllName = wostringstream();
        targetIndDllName << modName;
        targetIndDllName << L"." << pRuntime->GetCORVersion();
        targetIndDllName << L".v" << pAsmGen->GetVersion();
        targetIndDllName << L"." << pAsmGen->GetProcessorArchitectures()[0];
        targetIndDllName << L".Prig.dll";
        
        return targetIndDllName.str();
    }

    void FillIndirectionPreparationList(MetadataDispenser const *pDisp, AssemblyGenerator *pAsmGen, PrigData &prigData)
    {
        using Urasandesu::Prig::PrigConfig;

        auto mscorlibFullName = wstring();
        if (prigData.m_corVersion == L"v2.0.50727")
            mscorlibFullName = L"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        else if (prigData.m_corVersion == L"v4.0.30319")
            mscorlibFullName = L"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        _ASSERTE(!mscorlibFullName.empty());
        auto const *pMSCorLib = pDisp->GetAssembly(mscorlibFullName);
        prigData.m_pMSCorLibDll = pMSCorLib->GetMainModule();
        
        auto const *pPrigFrmwrk = pDisp->GetAssemblyFrom(PrigConfig::GetLibPath() / L"Urasandesu.Prig.Framework.dll");
        auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();
        prigData.m_pPrigFrameworkDll = pPrigFrmwrkDll;

        auto const *pIndAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
        auto const *pIndAsm = pDisp->GetAssemblyFrom(prigData.m_indDllPath);
        auto indAttrs = pIndAsm->GetCustomAttributes(pIndAttrType);
        BOOST_FOREACH (auto const *pIndAttr, indAttrs)
        {
            auto mdt = GetIndirectableToken(pIndAttr);
            auto *pTarget = pAsmGen->GetMethodGenerator(mdt);

            auto &orgPrep = prigData.NewPreparation<OriginalMethodPreparation>(mdt);
            orgPrep.FillIndirectionPreparation(pDisp, pTarget, prigData);
            orgPrep.ResolveIndirectionPreparation(pAsmGen);
            
            auto &dlgtPrep = prigData.NewPreparation<DelegatedMethodPreparation>(orgPrep.m_mdt);
            dlgtPrep.FillIndirectionPreparation(pDisp, pTarget, prigData);
            dlgtPrep.ResolveIndirectionPreparation(pAsmGen);
        }

        if (CPPANONYM_D_LOG_ENABLED())
        {
            BOOST_FOREACH (auto const &pair, prigData.m_indirectables)
                CPPANONYM_D_LOGW1(L"Indirectable Token: 0x%|1$08X|", pair.first);
        }
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
        using Urasandesu::Prig::PrigConfig;

        CPPANONYM_D_LOGW2(L"ModuleLoadFinishedCore(ModuleID: 0x%|1$X|, HRESULT: 0x%|2$08X|)", reinterpret_cast<void *>(moduleId), hrStatus);

        auto _ = guard_type(m_lock);
        if (!m_pProfInfo)
            return S_OK;

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pModProf = pProcProf->AttachToModule(moduleId);
        CPPANONYM_D_LOGW1(L"Current Path: %|1$s|", current_path().native());
        auto modPath = path(pModProf->GetName());
        auto modName = modPath.stem().native();
        CPPANONYM_D_LOGW1(L"Loading Module: %|1$s|", modName);
        if (modName.empty())
            return S_OK;
        
        
        CPPANONYM_LOG_NAMED_SCOPE("if (!modName.empty())");
        auto candidateIndDllPaths = unordered_set<path, Hash<path>, EqualTo<path> >();
        auto isCandidate = [&](path const &p) { return p.filename().native().find(modName) != wstring::npos; };
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
        auto *pDisp = pDomainProf->GetMetadataDispenser();
        auto *pAsmGen = pAsmProf->GetAssemblyGenerator(pDisp);
        auto targetIndDllPath = path(m_currentDir);
        targetIndDllPath /= GetIndirectionDllName(modName, pRuntime, pAsmGen);
        CPPANONYM_D_LOGW1(L"Find detour module: %|1$s|.", targetIndDllPath.native());
        if (candidateIndDllPaths.find(targetIndDllPath) == candidateIndDllPaths.end())
            return S_OK;
        
        
        CPPANONYM_LOG_NAMED_SCOPE("if (candidateIndDllPaths.find(targetIndDllPath) != candidateIndDllPaths.end())");
        CPPANONYM_D_LOGW1(L"Detour module: %|1$s| is found. Start to modify the module.", targetIndDllPath.native());
        pModProf.Persist();
        pAsmProf.Persist();
        pDomainProf.Persist();

        auto &asmResolver = pDisp->GetAssemblyResolver();
        asmResolver.AddSearchDirectory(m_currentDir);

        auto asmId = lexical_cast<wstring>(pAsmProf->GetID());
        auto pData = pDomainProf->GetData(asmId);
        if (!pData)
        {
            auto *pPrigData = new PrigData();
            pData = AnyPtr(pPrigData);
            auto &prigData = *pPrigData;
            
            prigData.m_corVersion = pRuntime->GetCORVersion();
            prigData.m_indDllPath = targetIndDllPath;
            FillIndirectionPreparationList(pDisp, pAsmGen, prigData);
            
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
        if (!m_pProfInfo)
            return S_OK;

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
        
        CPPANONYM_D_LOGW2(L"JITCompilationStartedCore(FunctionID: 0x%|1$X|, BOOL: 0x%|2$08X|)", reinterpret_cast<void *>(functionId), fIsSafeToBlock);
        
        auto _ = guard_type(m_lock);
        if (!m_pProfInfo)
            return S_OK;

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
        
        auto const &indPrep = prigData.m_indirectionPreparationList[(*result).second];
        indPrep.EmitMethodBody(pMethodGen, pFuncProf);
        
        pProcProf->DetachFromFunction(functionId);

        return S_OK;
    }

}   // namespace CWeaverDetail {



HRESULT CWeaver::FinalConstruct()
{
    CPPANONYM_D_LOGW(L"CWeaver::FinalConstruct()");

    return S_OK;
}

void CWeaver::FinalRelease()
{
}
