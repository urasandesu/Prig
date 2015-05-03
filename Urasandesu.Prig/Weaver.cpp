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
        using Urasandesu::CppAnonym::Xml::operator >>;
        using Urasandesu::CppAnonym::Xml::FromUTF8;
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

        m_pkgPath = path(Environment::GetEnvironmentVariable(L"URASANDESU_PRIG_PACKAGE_FOLDER"));
        auto toolsPath = m_pkgPath / L"tools";
        auto prigConfigPath = toolsPath / L"Prig.config";
        
        auto config = PrigConfig();
        FromUTF8(prigConfigPath) >> make_nvp("Config", config);
            
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
        auto targetIndDllPath = path(m_currentDir);
        targetIndDllPath /= GetIndirectionDllName(modName, pRuntime, pDomainProf, pAsmProf);
        CPPANONYM_D_LOGW1(L"Find detour module: %|1$s|.", targetIndDllPath.native());
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
            pPrigData->m_corVersion = pRuntime->GetCORVersion();
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
        using boost::timer::cpu_timer;
        using boost::timer::default_places;
        using std::regex_search;
        using std::wregex;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;
        using Urasandesu::Prig::IndirectionDelegates;
        
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

            BOOST_FOREACH (auto const &additionalDlgt, m_currentPkg.AdditionalDelegates)
            {
                prigData.m_indirectionDelegatesList.push_back(new IndirectionDelegates());
                auto &indDlgts = prigData.m_indirectionDelegatesList.back();
                
                auto patternStr = additionalDlgt.FullName;
                boost::replace_all(patternStr, L".", L"\\.");
                auto pattern = wregex(patternStr);
                BOOST_FOREACH (auto const &pAsm, pDisp->GetAssemblies())
                {
                    if (!regex_search(pAsm->GetFullName(), pattern))
                        continue;
                    
                    indDlgts.m_pIndirectionDelegatesAssembly = pAsm;
                    break;
                }
                
                if (!indDlgts.m_pIndirectionDelegatesAssembly)
                    indDlgts.m_pIndirectionDelegatesAssembly = pDisp->GetAssemblyFrom(additionalDlgt.HintPath);
            }
            
            auto const &corVersion = prigData.m_corVersion;
            auto libPath = m_pkgPath / (corVersion == L"v2.0.50727" ? L"lib\\net35" : L"lib\\net40");
            auto prigDelegatesNames = vector<wstring>(4);
            prigDelegatesNames[0] = L"Urasandesu.Prig.Delegates." + corVersion + L".v0.1.0.0.MSIL.dll";
            prigDelegatesNames[1] = L"Urasandesu.Prig.Delegates.0404." + corVersion + L".v0.1.0.0.MSIL.dll";
            prigDelegatesNames[2] = L"Urasandesu.Prig.Delegates.0804." + corVersion + L".v0.1.0.0.MSIL.dll";
            prigDelegatesNames[3] = L"Urasandesu.Prig.Delegates.1205." + corVersion + L".v0.1.0.0.MSIL.dll";
            BOOST_FOREACH (auto const &prigDelegatesName, prigDelegatesNames)
            {
                prigData.m_indirectionDelegatesList.push_back(new IndirectionDelegates());
                auto &indDlgts = prigData.m_indirectionDelegatesList.back();
                
                indDlgts.m_pIndirectionDelegatesAssembly = pDisp->GetAssemblyFrom(libPath / prigDelegatesName);
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

            auto offset = EmitIndirectMethodBody(pNewBodyGen, pDisp, pMethodGen, prigData);

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
        
            pProcProf->DetachFromFunction(functionId);
        }

        CPPANONYM_V_LOG2("Processing time to modify the method(Token: 0x%|1$08X|): %|2$s|.", mdt, timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));

        return S_OK;
    }



    SIZE_T CWeaverImpl::EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MetadataDispenser const *pDisp, MethodGenerator const *pMethodGen, PrigData &prigData)
    {
        using boost::timer::cpu_timer;
        using boost::timer::default_places;

        CPPANONYM_LOG_FUNCTION();

        auto timer = cpu_timer();

        auto mscorlibFullName = wstring();
        if (prigData.m_corVersion == L"v2.0.50727")
            mscorlibFullName = L"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        else if (prigData.m_corVersion == L"v4.0.30319")
            mscorlibFullName = L"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        _ASSERTE(!mscorlibFullName.empty());
        
        auto const *pMSCorLib = pDisp->GetAssembly(mscorlibFullName);
        auto const *pMSCorLibDll = pMSCorLib->GetMainModule();

        CPPANONYM_V_LOG1("Processing time to find mscorlib: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pException = pMSCorLibDll->GetType(L"System.Exception");
        auto const *pType = pMSCorLibDll->GetType(L"System.Type");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pType_GetTypeFromHandle = pType->GetMethod(L"GetTypeFromHandle");

        CPPANONYM_V_LOG1("Processing time to get BCL definitions 2: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pPrigFrmwrk = pDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce");
        auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();

        CPPANONYM_V_LOG1("Processing time to find Urasandesu.Prig.Framework: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndInfo = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndHolder1 = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pIndDlgtAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionDelegateAttribute");
        auto const *pLooseCrossDomainAccessor = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");
        auto const *pFlowControlException = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.FlowControlException");

        CPPANONYM_V_LOG1("Processing time to get indirection definitions 1: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        timer = cpu_timer();

        auto const *pIndDlgtInst = GetIndirectionDelegateInstance(pMethodGen, pIndDlgtAttrType, prigData);

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
        timer = cpu_timer();

        auto *pLocal0_holder = pNewBodyGen->DefineLocal(pIndHolder1IndDlgtInst);
        auto *pLocal1_info = pNewBodyGen->DefineLocal(pIndInfo);
        auto *pLocal2_ind = pNewBodyGen->DefineLocal(pIndDlgtInst);
        auto *pLocal3_e = pNewBodyGen->DefineLocal(pException);
        auto *pLocal4 = static_cast<LocalGenerator *>(nullptr);
        if (pMethodGen->GetReturnType()->GetKind() != TypeKinds::TK_VOID)
            pLocal4 = pNewBodyGen->DefineLocal(pMethodGen->GetReturnType());

        auto label0 = pNewBodyGen->DefineLabel();
        auto label1 = pNewBodyGen->DefineLabel();
        auto label2 = pNewBodyGen->DefineLabel();
        auto label3 = pNewBodyGen->DefineLabel();

        pNewBodyGen->BeginExceptionBlock();
        {
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
            if (pLocal4)
                pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal4);
            pNewBodyGen->Emit(OpCodes::Leave_S, label1);

            pNewBodyGen->MarkLabel(label0);
        }
        pNewBodyGen->BeginCatchBlock(pException);
        {
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal3_e);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal3_e);
            pNewBodyGen->Emit(OpCodes::Ldstr, L"Urasandesu.Prig.Framework.FlowControlException");
            pNewBodyGen->Emit(OpCodes::Call, pLooseCrossDomainAccessor_IsInstanceOfIdentity);
            pNewBodyGen->Emit(OpCodes::Brtrue_S, label2);
            
            pNewBodyGen->Emit(OpCodes::Rethrow);
            
            pNewBodyGen->MarkLabel(label2);
        }
        pNewBodyGen->EndExceptionBlock();
        
        pNewBodyGen->Emit(OpCodes::Br_S, label3);
        pNewBodyGen->MarkLabel(label1);
        if (pLocal4)
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal4);
        pNewBodyGen->Emit(OpCodes::Ret);
        pNewBodyGen->MarkLabel(label3);

        auto size = pNewBodyGen->GetInstructions().size();

        CPPANONYM_V_LOG1("Processing time to emit indirect method body details: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
        
        return size;
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

    IType const *CWeaverImpl::GetIndirectionDelegateInstance(IMethod const *pTarget, IType const *pIndDlgtAttrType, PrigData &prigData) const
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
        
        auto const *pIndDlgt = static_cast<IType *>(nullptr);
        // check whether the delegate is cached.
        {
            auto result = prigData.m_indDlgtCache.find(pTarget);
            if (result != prigData.m_indDlgtCache.end())
                pIndDlgt = (*result).second;
        }

        auto &dlgtsList = prigData.m_indirectionDelegatesList;
        BOOST_FOREACH (auto &pDlgts, dlgtsList)
        {
            CPPANONYM_V_LOG1("Processing time to check whether the delegate is cached: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
            timer = cpu_timer();

            // enumerate IndirectionDelegate and cache them.
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
            
            // find IndirectionDelegate which has same signature with the target method and cache it.
            {
                auto result = FindIf(pDlgts.m_indirectionDelegates, IndirectionDelegateFinder(pTarget));
                if (!result)
                    continue;
                
                pIndDlgt = *result;
                prigData.m_indDlgtCache[pTarget] = pIndDlgt;

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

                CPPANONYM_V_LOG1("Processing time to make Generic Type Instance of IndirectionDelegate: %|1$s|.", timer.format(default_places, "%ws wall, %us user + %ss system = %ts CPU (%p%)"));
                timer = cpu_timer();

                return pIndDlgtInst;
            }
        }
        
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(CLDB_E_RECORD_NOTFOUND));
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
    CPPANONYM_D_LOGW(L"CWeaver::FinalConstruct()");

    return S_OK;
}

void CWeaver::FinalRelease()
{
}
