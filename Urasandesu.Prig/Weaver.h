/* 
 * File: Weaver.h
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


#pragma once

#include "resource.h"

#include "UrasandesuPrig_i.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef URASANDESU_PRIG_PRIGDATAFWD_H
#include <Urasandesu/Prig/PrigDataFwd.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

namespace CWeaverDetail {

    using namespace Urasandesu::CppAnonym::Utilities;
    using namespace Urasandesu::Swathe;
    using namespace Urasandesu::Swathe::Hosting;
    using namespace Urasandesu::Swathe::Metadata;
    using namespace Urasandesu::Swathe::Profiling;
    using boost::filesystem::path;
    using boost::unordered_map;
    using boost::unordered_set;
    using std::vector;
    using std::wstring;
    using Urasandesu::Prig::PrigData;
    using Urasandesu::Prig::PrigPackageConfig;

    class CWeaverImpl : 
        public ICorProfilerCallback5Impl<ICorProfilerCallback5>
    {
    public: 
        CWeaverImpl();

    protected:
        STDMETHOD(InitializeCore)( 
            /* [in] */ IUnknown *pICorProfilerInfoUnk);

        STDMETHOD(ShutdownCore)();

        STDMETHOD(AppDomainCreationStartedCore)( 
            /* [in] */ AppDomainID appDomainId);

        STDMETHOD(AppDomainCreationFinishedCore)( 
            /* [in] */ AppDomainID appDomainId,
            /* [in] */ HRESULT hrStatus);

        STDMETHOD(AppDomainShutdownStartedCore)( 
            /* [in] */ AppDomainID appDomainId);

        STDMETHOD(AppDomainShutdownFinishedCore)( 
            /* [in] */ AppDomainID appDomainId,
            /* [in] */ HRESULT hrStatus);

        STDMETHOD(ModuleLoadStartedCore)( 
            /* [in] */ ModuleID moduleId);
        
        STDMETHOD(ModuleLoadFinishedCore)( 
            /* [in] */ ModuleID moduleId,
            /* [in] */ HRESULT hrStatus);

        STDMETHOD(ModuleUnloadStartedCore)( 
            /* [in] */ ModuleID moduleId);
        
        STDMETHOD(ModuleUnloadFinishedCore)( 
            /* [in] */ ModuleID moduleId,
            /* [in] */ HRESULT hrStatus);

        STDMETHOD(JITCompilationStartedCore)( 
            /* [in] */ FunctionID functionId,
            /* [in] */ BOOL fIsSafeToBlock);
    
    private:
        SIZE_T EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MetadataDispenser const *pDisp, MethodGenerator const *pMethodGen, PrigData &prigData);
        void EmitIndirectParameters(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen);
        IType const *GetIndirectionDelegateInstance(IMethod const *pTarget, IType const *pIndDlgtAttrType, PrigData &prigData) const;
        IType const *MakeGenericExplicitThisType(IType const *pTarget) const;

        typedef boost::lock_guard<boost::mutex> guard_type;
        wstring m_currentDir;
        path m_pkgPath;
        PrigPackageConfig m_currentPkg;
        vector<path> m_indDllPaths;
        ProfilingInfo *m_pProfInfo;
        boost::mutex m_lock;
    };

}   // namespace CWeaverDetail {

struct ATL_NO_VTABLE CWeaver :
	ATL::CComObjectRootEx<ATL::CComMultiThreadModel>,
	ATL::CComCoClass<CWeaver, &CLSID_Weaver>,
    CWeaverDetail::CWeaverImpl,
	ATL::IDispatchImpl<IWeaver, &IID_IWeaver, &LIBID_UrasandesuPrigLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
	CWeaver()
	{ }

DECLARE_REGISTRY_RESOURCEID(IDR_WEAVER)

BEGIN_COM_MAP(CWeaver)
	COM_INTERFACE_ENTRY(IWeaver)
	COM_INTERFACE_ENTRY(IDispatch)
    COM_INTERFACE_ENTRY(ICorProfilerCallback)
    COM_INTERFACE_ENTRY(ICorProfilerCallback2)
    COM_INTERFACE_ENTRY(ICorProfilerCallback3)
    COM_INTERFACE_ENTRY(ICorProfilerCallback4)
    COM_INTERFACE_ENTRY(ICorProfilerCallback5)
END_COM_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

    HRESULT FinalConstruct();

    void FinalRelease();
};

OBJECT_ENTRY_AUTO(__uuidof(Weaver), CWeaver)
