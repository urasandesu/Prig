// Weaver.cpp : CWeaver ‚ÌŽÀ‘•

#include "stdafx.h"
#include "Weaver.h"

//#define OUTPUT_VERBOSE
//#define OUTPUT_DEBUG

#ifdef OUTPUT_VERBOSE
#define V_WCOUT(s) std::wcout << s << std::endl
#else
#define V_WCOUT(s)
#endif

#ifdef OUTPUT_VERBOSE
#define V_WCOUT1(fmt, arg) std::wcout << boost::wformat(fmt) % arg << std::endl
#else
#define V_WCOUT1(fmt, arg)
#endif

#ifdef OUTPUT_DEBUG
#define D_COUT1(fmt, arg) std::cout << boost::format(fmt) % arg << std::endl
#else
#define D_COUT1(fmt, arg) 
#endif

#ifdef OUTPUT_DEBUG
#define D_WCOUT1(fmt, arg) std::wcout << boost::wformat(fmt) % arg << std::endl
#else
#define D_WCOUT1(fmt, arg) 
#endif

std::wstring const CWeaver::MODULE_NAME_OF_MS_COR_LIB = L"mscorlib.dll";
std::wstring const CWeaver::TYPE_NAME_OF_SYSTEM_DATE_TIME_GET_NOW = L"System.DateTime.get_Now";

// CWeaver
HRESULT CWeaver::FinalConstruct()
{
    return S_OK;
}

void CWeaver::FinalRelease()
{
}

HRESULT CWeaver::InitializeCore(
    /* [in] */ IUnknown *pICorProfilerInfoUnk)
{
    using namespace std;
    using namespace Urasandesu::CppAnonym;
    HRESULT hr = E_FAIL;


    // Reset the timer.
    m_timer.restart();
    

    // Initialize the profiling API.
    hr = pICorProfilerInfoUnk->QueryInterface(IID_ICorProfilerInfo2, 
                                              reinterpret_cast<void**>(&m_pInfo));
    if (FAILED(hr)) 
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));


    // Set a value that specifies the types of events for which the profiler 
    // wants to receive notification from CLR.
    // NOTE: If you want to profile core APIs such as types in mscorlib, 
    //       you should set COR_PRF_USE_PROFILE_IMAGES.
    DWORD event_ = COR_PRF_MONITOR_MODULE_LOADS | 
                   COR_PRF_MONITOR_JIT_COMPILATION | 
                   COR_PRF_USE_PROFILE_IMAGES;
    hr = m_pInfo->SetEventMask(event_);
    if (FAILED(hr)) 
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

    return S_OK;
}

HRESULT CWeaver::ShutdownCore(void)
{ 
    using namespace std;
    using namespace boost;
    
    // Display the time elapsed.
    std::cout << boost::format("Time Elapsed: %|1$f|s") % m_timer.elapsed() << std::endl;

    return S_OK; 
}

HRESULT CWeaver::ModuleLoadStartedCore( 
    /* [in] */ ModuleID moduleId)
{
    using namespace std;
    using namespace boost::filesystem;
    using namespace Urasandesu::CppAnonym;
    HRESULT hr = E_FAIL;
    

    // Convert ModuleID to the name.
    LPCBYTE pBaseLoadAddress = NULL;
    WCHAR modName[MAX_SYM_NAME] = { 0 };
    ULONG modNameSize = sizeof(modName);
    AssemblyID asmId = 0;
    
    hr = m_pInfo->GetModuleInfo(moduleId, &pBaseLoadAddress, 
                                modNameSize, &modNameSize, modName, &asmId);
    if (hr != CORPROF_E_DATAINCOMPLETE && FAILED(hr))
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
    

    // If target module is detected, the object implemented IMetaDataEmit is initialized 
    // to use the following process.
    path modPath(modName);

    V_WCOUT1(L"ModuleLoadStarted: %|1$s|", modName);
    if (modPath.filename().wstring() == MODULE_NAME_OF_MS_COR_LIB)
    {
        V_WCOUT(L"The target module is detected. Getting module meta data is started.");

        hr = m_pInfo->GetModuleMetaData(moduleId, ofRead | ofWrite, IID_IMetaDataEmit2, 
                                        reinterpret_cast<IUnknown **>(&m_pEmtMSCorLib));
        if (hr != CORPROF_E_DATAINCOMPLETE && FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
    }

    return S_OK;
}

HRESULT CWeaver::JITCompilationStartedCore( 
    /* [in] */ FunctionID functionId,
    /* [in] */ BOOL fIsSafeToBlock)
{
    namespace OpCodes = Urasandesu::CppAnonym::MetaData::OpCodes;
    using namespace std;
    using namespace boost;
    using namespace boost::filesystem;
    using namespace Urasandesu::CppAnonym;
    HRESULT hr = E_FAIL;
    

    // Convert FunctionID to MethodDef token.
    mdMethodDef mdmd = mdMethodDefNil;
    CComPtr<IMetaDataImport2> pImport;
    hr = m_pInfo->GetTokenAndMetaDataFromFunction(functionId, IID_IMetaDataImport2, 
                                                  reinterpret_cast<IUnknown **>(&pImport), 
                                                  &mdmd);
    if (FAILED(hr)) 
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
    

    // Get the properties of this method.
    mdTypeDef mdtd = mdTypeDefNil;
    WCHAR methodName[MAX_SYM_NAME] = { 0 };
    ULONG methodNameSize = sizeof(methodName);
    DWORD methodAttr = 0;
    PCCOR_SIGNATURE pMethodSig = NULL;
    ULONG methodSigSize = 0;
    ULONG methodRVA = 0;
    DWORD methodImplFlag = 0;
    hr = pImport->GetMethodProps(mdmd, &mdtd, methodName, methodNameSize, 
                                 &methodNameSize, &methodAttr, &pMethodSig, 
                                 &methodSigSize, &methodRVA, &methodImplFlag);
    if (FAILED(hr)) 
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
    

    // Get the properties of type that this method is declared.
    WCHAR typeName[MAX_SYM_NAME] = { 0 };
    ULONG typeNameSize = sizeof(typeName);
    DWORD typeAttr = 0;
    mdToken mdtTypeExtends = mdTokenNil;
    hr = pImport->GetTypeDefProps(mdtd, typeName, typeNameSize, &typeNameSize, 
                                  &typeAttr, &mdtTypeExtends);
    if (FAILED(hr)) 
        BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
    

    // If target type is detected, Replacing method body is started.
    wstring methodFullName(typeName);
    methodFullName += L".";
    methodFullName += methodName;

    V_WCOUT1(L"JITCompilationStarted: %|1$s|", methodFullName);

    if (methodFullName == TYPE_NAME_OF_SYSTEM_DATE_TIME_GET_NOW)
    {
        V_WCOUT(L"The target type is detected. Replacing method body is started.");
    

        // Prepare the 1st source assembly mscorlib.Prig that is delegated an actual process.
        // Get IMetaDataImport to manipulate the assembly.
        path frameworkPath(L".\\Urasandesu.Prig.Framework.dll");
        
        CComPtr<IMetaDataDispenserEx> pDispFramework;
        hr = ::CoCreateInstance(CLSID_CorMetaDataDispenser, NULL, CLSCTX_INPROC_SERVER, 
                                IID_IMetaDataDispenserEx, 
                                reinterpret_cast<void **>(&pDispFramework));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

        CComPtr<IMetaDataImport2> pImpFramework;
        hr = pDispFramework->OpenScope(frameworkPath.c_str(), ofRead, IID_IMetaDataImport2, 
                                      reinterpret_cast<IUnknown **>(&pImpFramework));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        


        // Get TypeDef records to add to TypeRef table.
        mdTypeDef mdtdLooseCrossDomainAccessor = mdTypeDefNil;
        hr = pImpFramework->FindTypeDefByName(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor", NULL, &mdtdLooseCrossDomainAccessor);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeDef for Urasandesu.Prig.Framework.LooseCrossDomainAccessor: 0x%|1$08X|", mdtdLooseCrossDomainAccessor);

        mdTypeDef mdtdIndirectionHolder1 = mdTypeDefNil;
        hr = pImpFramework->FindTypeDefByName(L"Urasandesu.Prig.Framework.IndirectionHolder`1", NULL, &mdtdIndirectionHolder1);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeDef for Urasandesu.Prig.Framework.IndirectionHolder`1: 0x%|1$08X|", mdtdIndirectionHolder1);

        mdTypeDef mdtdIndirectionInfo = mdTypeDefNil;
        hr = pImpFramework->FindTypeDefByName(L"Urasandesu.Prig.Framework.IndirectionInfo", NULL, &mdtdIndirectionInfo);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeDef for Urasandesu.Prig.Framework.IndirectionInfo: 0x%|1$08X|", mdtdIndirectionInfo);
        


        // Get MethodDef records to add to MethodRef table.
        mdMethodDef mdmdLooseCrossDomainAccessorTryGet = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_GENERIC,  // GENERIC
                1,                              // GenParamCount  
                1,                              // ParamCount: 1
                ELEMENT_TYPE_BOOLEAN,           // RetType: bool
                ELEMENT_TYPE_BYREF,             // Param: BYREF
                ELEMENT_TYPE_MVAR,              //        MVAR
                0                               //        Generic Parameter Index: 0
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpFramework->FindMethod(mdtdLooseCrossDomainAccessor, L"TryGet", 
                                              pSigBlob, sigBlobSize, &mdmdLooseCrossDomainAccessorTryGet);            
            
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for Urasandesu.Prig.Framework.LooseCrossDomainAccessor.TryGet<T>: 0x%|1$08X|", mdmdLooseCrossDomainAccessorTryGet);
        
        mdMethodDef mdmdIndirectionInfoset_AssemblyName = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_HASTHIS,  // HASTHIS
                1,                              // ParamCount: 1
                ELEMENT_TYPE_VOID,              // RetType: void
                ELEMENT_TYPE_STRING             // Param: string
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpFramework->FindMethod(mdtdIndirectionInfo, L"set_AssemblyName", 
                                              pSigBlob, sigBlobSize, &mdmdIndirectionInfoset_AssemblyName);            
            
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for Urasandesu.Prig.Framework.IndirectionInfo.set_AssemblyName: 0x%|1$08X|", mdmdIndirectionInfoset_AssemblyName);
        
        mdMethodDef mdmdIndirectionInfoset_TypeFullName = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_HASTHIS,  // HASTHIS
                1,                              // ParamCount: 1
                ELEMENT_TYPE_VOID,              // RetType: void
                ELEMENT_TYPE_STRING             // Param: string
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpFramework->FindMethod(mdtdIndirectionInfo, L"set_TypeFullName", 
                                              pSigBlob, sigBlobSize, &mdmdIndirectionInfoset_TypeFullName);            
            
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for Urasandesu.Prig.Framework.IndirectionInfo.set_TypeFullName: 0x%|1$08X|", mdmdIndirectionInfoset_TypeFullName);
        
        mdMethodDef mdmdIndirectionInfoset_MethodName = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_HASTHIS,  // HASTHIS
                1,                              // ParamCount: 1
                ELEMENT_TYPE_VOID,              // RetType: void
                ELEMENT_TYPE_STRING             // Param: string
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpFramework->FindMethod(mdtdIndirectionInfo, L"set_MethodName", 
                                              pSigBlob, sigBlobSize, &mdmdIndirectionInfoset_MethodName);            
            
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for Urasandesu.Prig.Framework.IndirectionInfo.set_MethodName: 0x%|1$08X|", mdmdIndirectionInfoset_MethodName);
        
        mdMethodDef mdmdIndirectionHolder1TryGet = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_HASTHIS,  // HASTHIS
                2,                              // ParamCount: 2
                ELEMENT_TYPE_BOOLEAN,           // RetType: bool
                ELEMENT_TYPE_VALUETYPE,         // Param: VALUETYPE
                0x10,                           //        TypeDef: 0x02000004(Urasandesu.Prig.Framework.IndirectionInfo)
                ELEMENT_TYPE_BYREF,             //        BYREF
                ELEMENT_TYPE_VAR,               //        VAR
                0                               //        Generic Parameter Index: 0
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpFramework->FindMethod(mdtdIndirectionHolder1, L"TryGet", 
                                              pSigBlob, sigBlobSize, &mdmdIndirectionHolder1TryGet);            
            
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for Urasandesu.Prig.Framework.IndirectionHolder1<T>.TryGet: 0x%|1$08X|", mdmdIndirectionHolder1TryGet);
        


        // Get MetaDataAssemblyImport and Assembly record to add to AssemblyRef table.
        CComPtr<IMetaDataAssemblyImport> pAsmImpFramework;
        hr = pImpFramework->QueryInterface(IID_IMetaDataAssemblyImport, 
                                       reinterpret_cast<void **>(&pAsmImpFramework));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

        mdAssembly mdaFramework = mdAssemblyNil;
        hr = pAsmImpFramework->GetAssemblyFromScope(&mdaFramework);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of Assembly for Urasandesu.Prig.Framework.dll: 0x%|1$08X|", mdaFramework);

        auto_ptr<PublicKeyBlob> pFrameworkPubKey;
        DWORD frameworkPubKeySize = 0;
        auto_ptr<WCHAR> frameworkAsmName;
        ASSEMBLYMETADATA amdFramework;
        ::ZeroMemory(&amdFramework, sizeof(ASSEMBLYMETADATA));
        DWORD frameworkAsmFlags = 0;
        {
            ULONG nameSize = 0;
            DWORD asmFlags = 0;
            hr = pAsmImpFramework->GetAssemblyProps(mdaFramework, NULL, NULL, NULL, NULL, 0, 
                                                       &nameSize, &amdFramework, 
                                                       &asmFlags);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            frameworkAsmFlags |= (asmFlags & ~afPublicKey);
            frameworkAsmName = auto_ptr<WCHAR>(new WCHAR[nameSize]);
            amdFramework.szLocale = amdFramework.cbLocale ? new WCHAR[amdFramework.cbLocale] : NULL;
            amdFramework.rOS = amdFramework.ulOS ? new OSINFO[amdFramework.ulOS] : NULL;
            amdFramework.rProcessor = amdFramework.ulProcessor ? new ULONG[amdFramework.ulProcessor] : NULL;

            void *pPubKey = NULL;
            hr = pAsmImpFramework->GetAssemblyProps(mdaFramework, 
                                                const_cast<const void**>(&pPubKey), 
                                                &frameworkPubKeySize, NULL, 
                                                frameworkAsmName.get(), nameSize, NULL, 
                                                &amdFramework, NULL);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            if (frameworkPubKeySize)
                if (!::StrongNameTokenFromPublicKey(reinterpret_cast<BYTE*>(pPubKey), 
                                                    frameworkPubKeySize, 
                                                    reinterpret_cast<BYTE**>(&pPubKey), 
                                                    &frameworkPubKeySize))
                    BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            pFrameworkPubKey = auto_ptr<PublicKeyBlob>(
                            reinterpret_cast<PublicKeyBlob*>(new BYTE[frameworkPubKeySize]));
            ::memcpy_s(pFrameworkPubKey.get(), frameworkPubKeySize, pPubKey, 
                       frameworkPubKeySize);

            if (frameworkPubKeySize)
                ::StrongNameFreeBuffer(reinterpret_cast<BYTE*>(pPubKey));
        }

        D_WCOUT1(L"Assembly Name: %|1$s|", frameworkAsmName.get());
        
        

        // Prepare the 2nd source assembly System.Core to reference delegation types such as Func<TResult>.
        // NOTE: If you want to access GAC, you can use Fusion API.
        path corSystemDirectoryPath;
        path fusionPath;
        {
            WCHAR buffer[MAX_PATH] = { 0 };
            DWORD length = 0;
            hr = ::GetCORSystemDirectory(buffer, MAX_PATH, &length);
            if (FAILED(hr)) 
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            corSystemDirectoryPath = buffer;
            fusionPath = buffer;
            fusionPath /= L"fusion.dll";
        }

        HMODULE hmodCorEE = ::LoadLibraryW(fusionPath.c_str());
        if (!hmodCorEE)
            BOOST_THROW_EXCEPTION(CppAnonymSystemException(::GetLastError()));
        BOOST_SCOPE_EXIT((hmodCorEE))
        {
            ::FreeLibrary(hmodCorEE);
        }
        BOOST_SCOPE_EXIT_END

        typedef HRESULT (__stdcall *CreateAsmCachePtr)(IAssemblyCache **ppAsmCache, DWORD dwReserved);

        CreateAsmCachePtr pfnCreateAsmCache = NULL;
        pfnCreateAsmCache = reinterpret_cast<CreateAsmCachePtr>(
                                        ::GetProcAddress(hmodCorEE, "CreateAssemblyCache"));
        if (!pfnCreateAsmCache)
            BOOST_THROW_EXCEPTION(CppAnonymSystemException(::GetLastError()));
        
        CComPtr<IAssemblyCache> pAsmCache;
        hr = pfnCreateAsmCache(&pAsmCache, 0);
        if (FAILED(hr)) 
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        
        
        // Get IMetaDataImport to manipulate the assembly.
        path systemCorePath;
        {
            WCHAR buffer[MAX_PATH] = { 0 };
            ASSEMBLY_INFO asmInfo;
            ::ZeroMemory(&asmInfo, sizeof(ASSEMBLY_INFO));
            asmInfo.cbAssemblyInfo = sizeof(ASSEMBLY_INFO);
            asmInfo.pszCurrentAssemblyPathBuf = buffer;
            asmInfo.cchBuf = MAX_PATH;
            hr = pAsmCache->QueryAssemblyInfo(0, L"System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL", &asmInfo);
            if (FAILED(hr)) 
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
            
            D_WCOUT1(L"System.Core is here: %|1$s|", buffer);
            systemCorePath = buffer;
        }

        CComPtr<IMetaDataDispenserEx> pDispSystemCore;
        hr = ::CoCreateInstance(CLSID_CorMetaDataDispenser, NULL, CLSCTX_INPROC_SERVER, 
                                IID_IMetaDataDispenserEx, 
                                reinterpret_cast<void **>(&pDispSystemCore));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

        CComPtr<IMetaDataImport2> pImpSystemCore;
        hr = pDispSystemCore->OpenScope(systemCorePath.c_str(), ofRead, 
                                        IID_IMetaDataImport2, 
                                        reinterpret_cast<IUnknown **>(&pImpSystemCore));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));



        // Get TypeDef records to add to TypeRef table.
        mdTypeDef mdtdFunc1 = mdTypeDefNil;
        hr = pImpSystemCore->FindTypeDefByName(L"System.Func`1", NULL, &mdtdFunc1);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeDef for System.Func<T>: 0x%|1$08X|", mdtdFunc1);



        // Get MethodDef records to add to MethodRef table.
        mdMethodDef mdmdFunc1Invoke = mdMethodDefNil;
        {
            COR_SIGNATURE pSigBlob[] = { 
                IMAGE_CEE_CS_CALLCONV_HASTHIS,  // HASTHIS 
                0,                              // ParamCount
                ELEMENT_TYPE_VAR,               // RetType: VAR
                0                               //          Generic Parameter Index: 0
            };
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = pImpSystemCore->FindMethod(mdtdFunc1, L"Invoke", 
                                            pSigBlob, sigBlobSize, 
                                            &mdmdFunc1Invoke);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodDef for System.Func<T>.Invoke: 0x%|1$08X|", mdmdFunc1Invoke);



        // Get MetaDataAssemblyImport and Assembly record to add to AssemblyRef table.
        CComPtr<IMetaDataAssemblyImport> pAsmImpSystemCore;
        hr = pImpSystemCore->QueryInterface(IID_IMetaDataAssemblyImport, 
                                          reinterpret_cast<void **>(&pAsmImpSystemCore));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

        mdAssembly mdaSystemCore = mdAssemblyNil;
        hr = pAsmImpSystemCore->GetAssemblyFromScope(&mdaSystemCore);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of Assembly for System.Core.dll: 0x%|1$08X|", mdaSystemCore);

        auto_ptr<PublicKeyBlob> pSystemCorePubKey;
        DWORD systemCorePubKeySize = 0;
        auto_ptr<WCHAR> systemCoreName;
        ASSEMBLYMETADATA amdSystemCore;
        ::ZeroMemory(&amdSystemCore, sizeof(ASSEMBLYMETADATA));
        DWORD systemCoreAsmFlags = 0;
        {
            ULONG nameSize = 0;
            DWORD asmFlags = 0;
            hr = pAsmImpSystemCore->GetAssemblyProps(mdaSystemCore, NULL, NULL, NULL, NULL, 0, 
                                                     &nameSize, &amdSystemCore, 
                                                     &asmFlags);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            systemCoreAsmFlags |= (asmFlags & ~afPublicKey);
            systemCoreName = auto_ptr<WCHAR>(new WCHAR[nameSize]);
            amdSystemCore.szLocale = amdSystemCore.cbLocale ? 
                                        new WCHAR[amdSystemCore.cbLocale] : NULL;
            amdSystemCore.rOS = amdSystemCore.ulOS ? new OSINFO[amdSystemCore.ulOS] : NULL;
            amdSystemCore.rProcessor = amdSystemCore.ulProcessor ? 
                                        new ULONG[amdSystemCore.ulProcessor] : NULL;

            void *pPubKey = NULL;
            hr = pAsmImpSystemCore->GetAssemblyProps(mdaSystemCore, 
                                                     const_cast<const void**>(&pPubKey), 
                                                     &systemCorePubKeySize, NULL, 
                                                     systemCoreName.get(), nameSize, NULL, 
                                                     &amdSystemCore, NULL);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            if (systemCorePubKeySize)
                if (!::StrongNameTokenFromPublicKey(reinterpret_cast<BYTE*>(pPubKey), 
                                                    systemCorePubKeySize, 
                                                    reinterpret_cast<BYTE**>(&pPubKey), 
                                                    &systemCorePubKeySize))
                    BOOST_THROW_EXCEPTION(CppAnonymCOMException(::StrongNameErrorInfo()));

            pSystemCorePubKey = auto_ptr<PublicKeyBlob>(
                            reinterpret_cast<PublicKeyBlob*>(new BYTE[systemCorePubKeySize]));
            ::memcpy_s(pSystemCorePubKey.get(), systemCorePubKeySize, pPubKey, 
                       systemCorePubKeySize);

            if (systemCorePubKeySize)
                ::StrongNameFreeBuffer(reinterpret_cast<BYTE*>(pPubKey));
        }

        D_WCOUT1(L"Assembly Name: %|1$s|", systemCoreName.get());



        // Prepare the target assembly.
        // Get IMetaDataAssemblyEmit to add to AssemblyRef table. 
        CComPtr<IMetaDataAssemblyEmit> pAsmEmtMSCorLib;
        hr = m_pEmtMSCorLib->QueryInterface(IID_IMetaDataAssemblyEmit, 
                                            reinterpret_cast<void **>(&pAsmEmtMSCorLib));
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));



        // Add Assembly records retrieved in above to AssemblyRef table.
        mdAssemblyRef mdarFramework = mdAssemblyRefNil;
        hr = pAsmEmtMSCorLib->DefineAssemblyRef(pFrameworkPubKey.get(), frameworkPubKeySize, 
                                                frameworkAsmName.get(), &amdFramework, NULL, 0, 
                                                frameworkAsmFlags, &mdarFramework);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of AssemblyRef for Urasandesu.Prig.Framework.dll: 0x%|1$08X|", mdarFramework);

        mdAssemblyRef mdarSystemCore = mdAssemblyRefNil;
        hr = pAsmEmtMSCorLib->DefineAssemblyRef(pSystemCorePubKey.get(), systemCorePubKeySize, 
                                                systemCoreName.get(), &amdSystemCore, 
                                                NULL, 0, 
                                                systemCoreAsmFlags, &mdarSystemCore);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of AssemblyRef for System.Core.dll: 0x%|1$08X|", mdarSystemCore);



        // Add to TypeRef table with the name.
        mdTypeRef mdtrFunc1 = mdTypeRefNil;
        hr = m_pEmtMSCorLib->DefineTypeRefByName(mdarSystemCore, L"System.Func`1", &mdtrFunc1);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeRef for System.Func`1: 0x%|1$08X|", mdtrFunc1);

        mdTypeRef mdtrLooseCrossDomainAccessor = mdTypeRefNil;
        hr = m_pEmtMSCorLib->DefineTypeRefByName(mdarFramework, L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor", &mdtrLooseCrossDomainAccessor);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeRef for Urasandesu.Prig.Framework.LooseCrossDomainAccessor: 0x%|1$08X|", mdtrLooseCrossDomainAccessor);

        mdTypeRef mdtrIndirectionHolder1 = mdTypeRefNil;
        hr = m_pEmtMSCorLib->DefineTypeRefByName(mdarFramework, L"Urasandesu.Prig.Framework.IndirectionHolder`1", &mdtrIndirectionHolder1);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeRef for Urasandesu.Prig.Framework.IndirectionHolder`1: 0x%|1$08X|", mdtrIndirectionHolder1);

        mdTypeRef mdtrIndirectionInfo = mdTypeRefNil;
        hr = m_pEmtMSCorLib->DefineTypeRefByName(mdarFramework, L"Urasandesu.Prig.Framework.IndirectionInfo", &mdtrIndirectionInfo);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of TypeRef for Urasandesu.Prig.Framework.IndirectionInfo: 0x%|1$08X|", mdtrIndirectionInfo);



        // Add TypeRef records retrieved in above to TypeSpec table.
        mdTypeSpec mdtsSystemFunc1DateTime = mdTypeSpecNil;
        {
            COR_SIGNATURE pSigBlob[] = {
                ELEMENT_TYPE_GENERICINST,       // TYPE: GENERICINST
                ELEMENT_TYPE_CLASS,             //       CLASS
                0x05,                           //       TypeRef: 0x01000001(System.Func`1)
                1,                              //       Generics Arguments Count: 1
                ELEMENT_TYPE_VALUETYPE,         //       VALUETYPE
                0x80,                           //       TypeDef: 0x02000032(System.DateTime)
                0xC8                            // 
            };                                  
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = m_pEmtMSCorLib->GetTokenFromTypeSpec(pSigBlob, sigBlobSize, 
                                                      &mdtsSystemFunc1DateTime);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of TypeSpec for System.Func<System.DateTime>: 0x%|1$08X|", mdtsSystemFunc1DateTime);

        mdTypeSpec mdtsIndirectionHolder1Func1DateTime = mdTypeSpecNil;
        {
            COR_SIGNATURE pSigBlob[] = {
                ELEMENT_TYPE_GENERICINST,       // TYPE: GENERICINST
                ELEMENT_TYPE_CLASS,             //       CLASS
                0x0D,                           //       TypeRef: 0x01000003(Urasandesu.Prig.Framework.IndirectionHolder`1)
                1,                              //       Generics Arguments Count: 1
                ELEMENT_TYPE_GENERICINST,       //       GENERICINST
                ELEMENT_TYPE_CLASS,             //       CLASS
                0x05,                           //       TypeRef: 0x01000001(System.Func`1)
                1,                              //       Generics Arguments Count: 1
                ELEMENT_TYPE_VALUETYPE,         //       VALUETYPE
                0x80,                           //       TypeDef: 0x02000032(System.DateTime)
                0xC8                            // 
            };                                  
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = m_pEmtMSCorLib->GetTokenFromTypeSpec(pSigBlob, sigBlobSize, 
                                                      &mdtsIndirectionHolder1Func1DateTime);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of TypeSpec for Urasandesu.Prig.Framework.IndirectionHolder<System.Func<System.DateTime>>: 0x%|1$08X|", mdtsIndirectionHolder1Func1DateTime);


        // Add MethodDef records retrieved in above to MemberRef table.
        mdMemberRef mdmrLooseCrossDomainAccessorTryGet = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpFramework, NULL, 0, pImpFramework, 
                                                mdmdLooseCrossDomainAccessorTryGet, 
                                                pAsmEmtMSCorLib, 
                                                mdtrLooseCrossDomainAccessor, 
                                                &mdmrLooseCrossDomainAccessorTryGet);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for Urasandesu.Prig.Framework.LooseCrossDomainAccessor.TryGet<T>: 0x%|1$08X|", mdmrLooseCrossDomainAccessorTryGet);

        mdMemberRef mdmrIndirectionInfoset_AssemblyName = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpFramework, NULL, 0, pImpFramework, 
                                                mdmdIndirectionInfoset_AssemblyName, 
                                                pAsmEmtMSCorLib, 
                                                mdtrIndirectionInfo, 
                                                &mdmrIndirectionInfoset_AssemblyName);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for Urasandesu.Prig.Framework.IndirectionInfo.set_AssemblyName: 0x%|1$08X|", mdmrIndirectionInfoset_AssemblyName);

        mdMemberRef mdmrIndirectionInfoset_TypeFullName = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpFramework, NULL, 0, pImpFramework, 
                                                mdmdIndirectionInfoset_TypeFullName, 
                                                pAsmEmtMSCorLib, 
                                                mdtrIndirectionInfo, 
                                                &mdmrIndirectionInfoset_TypeFullName);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for Urasandesu.Prig.Framework.IndirectionInfo.set_TypeFullName: 0x%|1$08X|", mdmrIndirectionInfoset_TypeFullName);

        mdMemberRef mdmrIndirectionInfoset_MethodName = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpFramework, NULL, 0, pImpFramework, 
                                                mdmdIndirectionInfoset_MethodName, 
                                                pAsmEmtMSCorLib, 
                                                mdtrIndirectionInfo, 
                                                &mdmrIndirectionInfoset_MethodName);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for Urasandesu.Prig.Framework.IndirectionInfo.set_MethodName: 0x%|1$08X|", mdmrIndirectionInfoset_MethodName);

        mdMemberRef mdmrIndirectionHolder1TryGet = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpFramework, NULL, 0, pImpFramework, 
                                                mdmdIndirectionHolder1TryGet, 
                                                pAsmEmtMSCorLib, 
                                                mdtsIndirectionHolder1Func1DateTime, 
                                                &mdmrIndirectionHolder1TryGet);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for Urasandesu.Prig.Framework.IndirectionHolder<System.Func<System.DateTime>>.TryGet: 0x%|1$08X|", mdmrIndirectionHolder1TryGet);
        
        mdMemberRef mdmrFunc1DateTimeInvoke = mdMemberRefNil;
        hr = m_pEmtMSCorLib->DefineImportMember(pAsmImpSystemCore, NULL, 0, pImpSystemCore, 
                                                mdmdFunc1Invoke, 
                                                pAsmEmtMSCorLib, 
                                                mdtsSystemFunc1DateTime, 
                                                &mdmrFunc1DateTimeInvoke);
        if (FAILED(hr))
            BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
        D_COUT1("Token of MemberRef for System.Func<System.DateTime>.Invoke: 0x%|1$08X|", mdmrFunc1DateTimeInvoke);


        // Add MemberRef records retrieved in above to MethodSpec table.
        mdMethodSpec mdmsLooseCrossDomainAccessorTryGetIndirectionHolder1Func1DateTime = mdMethodSpecNil;
        {
            COR_SIGNATURE pSigBlob[] = {
                IMAGE_CEE_CS_CALLCONV_GENERICINST,  // TYPE: GENERICINST
                1,                                  //       Generics Arguments Count: 1
                ELEMENT_TYPE_GENERICINST,           //       GENERICINST
                ELEMENT_TYPE_CLASS,                 //       CLASS
                0x0D,                               //       TypeRef: 0x01000003(Urasandesu.Prig.Framework.IndirectionHolder`1)
                1,                                  //       Generics Arguments Count: 1
                ELEMENT_TYPE_GENERICINST,           //       GENERICINST
                ELEMENT_TYPE_CLASS,                 //       CLASS
                0x05,                               //       TypeRef: 0x01000001(System.Func`1)
                1,                                  //       Generics Arguments Count: 1
                ELEMENT_TYPE_VALUETYPE,             //       VALUETYPE
                0x80,                               //       TypeDef: 0x02000032(System.DateTime)
                0xC8                                // 
            };                                  
            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = m_pEmtMSCorLib->DefineMethodSpec(mdmrLooseCrossDomainAccessorTryGet, 
                                                  pSigBlob, sigBlobSize, 
                                                  &mdmsLooseCrossDomainAccessorTryGetIndirectionHolder1Func1DateTime);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
        
        D_COUT1("Token of MethodSpec for Urasandesu.Prig.Framework.LooseCrossDomainAccessor.TryGet<Urasandesu.Prig.Framework.IndirectionHolder<System.Func<System.DateTime>>>: 0x%|1$08X|", mdmsLooseCrossDomainAccessorTryGetIndirectionHolder1Func1DateTime);


        
        // Copy the original method body to add behind indirection process.
        {
            ClassID classId = 0;
            ModuleID modId = 0;
            mdToken mdt = mdTokenNil;
            hr = m_pInfo->GetFunctionInfo(functionId, &classId, &modId, &mdt);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
            LPCBYTE pMethodBodyAll = NULL;
            ULONG methodBodyAllSize = 0;
            hr = m_pInfo->GetILFunctionBody(modId, mdt, &pMethodBodyAll, &methodBodyAllSize);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));

            m_pDateTimeget_NowBody = auto_ptr<BYTE>(new BYTE[methodBodyAllSize]);
            ::memcpy_s(m_pDateTimeget_NowBody.get(), methodBodyAllSize, pMethodBodyAll, 
                       methodBodyAllSize);
        }
#ifdef OUTPUT_DEBUG
        {
            cout << "Original Method Body: ";
            COR_ILMETHOD *pILMethod = reinterpret_cast<COR_ILMETHOD *>(m_pDateTimeget_NowBody.get());
            unsigned headerSize = 0;
            BYTE *pCode = NULL;
            unsigned codeSize = 0;
            if (pILMethod->Fat.IsFat())
            {
                headerSize = sizeof(COR_ILMETHOD_FAT);
                pCode = pILMethod->Fat.GetCode();
                codeSize = pILMethod->Fat.GetCodeSize();
            }
            else
            {
                headerSize = sizeof(COR_ILMETHOD_TINY);
                pCode = pILMethod->Tiny.GetCode();
                codeSize = pILMethod->Tiny.GetCodeSize();
            }

            for (BYTE *i = pCode, *i_end = i + codeSize; i != i_end; ++i)
                cout << format("%|1$02X| ") % static_cast<INT>(*i);

            cout << endl;
        }
#endif
        
        
        
        // Create StandAloneSig records.
        mdSignature mdsDateTimeget_NowLocals = mdSignatureNil;
        {
            COR_SIGNATURE pSigBlob[] = {
                IMAGE_CEE_CS_CALLCONV_LOCAL_SIG,// LOCAL_SIG   
                0x04,                           // Count: 4
                ELEMENT_TYPE_VALUETYPE,         // Type[0]: VALUETYPE
                0x80,                           //          TypeDef: 0x02000032(System.DateTime)
                0xC8,
                ELEMENT_TYPE_GENERICINST,       // Type[1]: GENERICINST
                ELEMENT_TYPE_CLASS,             //          CLASS
                0x0D,                           //          TypeRef: 0x01000003(Urasandesu.Prig.Framework.IndirectionHolder`1)
                1,                              //          Generics Arguments Count: 1
                ELEMENT_TYPE_GENERICINST,       //          GENERICINST
                ELEMENT_TYPE_CLASS,             //          CLASS
                0x05,                           //          TypeRef: 0x01000001(System.Func`1)
                1,                              //          Generics Arguments Count: 1
                ELEMENT_TYPE_VALUETYPE,         //          VALUETYPE
                0x80,                           //          TypeDef: 0x02000032(System.DateTime)
                0xC8,                           // 
                ELEMENT_TYPE_VALUETYPE,         // Type[2]: VALUETYPE
                0x11,                           //          TypeRef: 0x01000004(Urasandesu.Prig.Framework.IndirectionInfo)
                ELEMENT_TYPE_GENERICINST,       // TYPE[3]: GENERICINST
                ELEMENT_TYPE_CLASS,             //          CLASS
                0x05,                           //          TypeRef: 0x01000001(System.Func`1)
                1,                              //          Generics Arguments Count: 1
                ELEMENT_TYPE_VALUETYPE,         //          VALUETYPE
                0x80,                           //          TypeDef: 0x02000032(System.DateTime)
                0xC8                            // 
            };                                  

            ULONG sigBlobSize = sizeof(pSigBlob) / sizeof(COR_SIGNATURE);
            hr = m_pEmtMSCorLib->GetTokenFromSig(pSigBlob, sigBlobSize, 
                                                   &mdsDateTimeget_NowLocals);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
            
        D_COUT1("Token of StandAloneSig for System.DateTime.get_Now Locals: 0x%|1$08X|", mdsDateTimeget_NowLocals);
        
        
        
        // Create inline strings to #US heap.
        mdString mdsAssemblyName = mdStringNil;
        {
            wstring s(L"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            hr = m_pEmtMSCorLib->DefineUserString(s.c_str(), s.size(), &mdsAssemblyName);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
            
        D_COUT1("Token of the inline string for \"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\": 0x%|1$08X|", mdsAssemblyName);

        mdString mdsTypeFullName = mdStringNil;
        {
            wstring s(L"System.DateTime");
            hr = m_pEmtMSCorLib->DefineUserString(s.c_str(), s.size(), &mdsTypeFullName);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
            
        D_COUT1("Token of the inline string for \"System.DateTime\": 0x%|1$08X|", mdsTypeFullName);

        mdString mdsMethodName = mdStringNil;
        {
            wstring s(L"get_Now");
            hr = m_pEmtMSCorLib->DefineUserString(s.c_str(), s.size(), &mdsMethodName);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
            
        D_COUT1("Token of the inline string for \"get_Now\": 0x%|1$08X|", mdsMethodName);




        // Get IMethodMalloc to allocate new method body area.
        {
            ClassID classId = 0;
            ModuleID modId = 0;
            mdToken mdt = mdTokenNil;
            hr = m_pInfo->GetFunctionInfo(functionId, &classId, &modId, &mdt);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        
            CComPtr<IMethodMalloc> pMethodMalloc;
            hr = m_pInfo->GetILFunctionBodyAllocator(modId, &pMethodMalloc);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));



            // Emit the new method body to replace to a delegated process.
            SimpleBlob mbDateTimeget_Body;
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDNULL].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_STLOC_1].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x01);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALL].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmsLooseCrossDomainAccessorTryGetIndirectionHolder1Func1DateTime);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_BRFALSE_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x40);

            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x02);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_INITOBJ].byte1);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_INITOBJ].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdtrIndirectionInfo);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x02);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDSTR].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdsAssemblyName);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALL].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmrIndirectionInfoset_AssemblyName);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x02);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDSTR].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdsTypeFullName);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALL].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmrIndirectionInfoset_TypeFullName);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x02);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDSTR].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdsMethodName);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALL].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmrIndirectionInfoset_MethodName);

            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDNULL].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_STLOC_3].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOC_1].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOC_2].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOCA_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x03);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALLVIRT].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmrIndirectionHolder1TryGet);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_BRFALSE_S].byte2);
            mbDateTimeget_Body.Put<BYTE>(0x07);

            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_LDLOC_3].byte2);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_CALLVIRT].byte2);
            mbDateTimeget_Body.Put<DWORD>(mdmrFunc1DateTimeInvoke);
            mbDateTimeget_Body.Put<BYTE>(OpCodes::Encodings[OpCodes::CEE_RET].byte2);

            // Add the original method body behind indirection process.
            {
                COR_ILMETHOD *pILMethod = reinterpret_cast<COR_ILMETHOD *>(m_pDateTimeget_NowBody.get());
                unsigned headerSize = 0;
                BYTE *pCode = NULL;
                unsigned codeSize = 0;
                if (pILMethod->Fat.IsFat())
                {
                    headerSize = sizeof(COR_ILMETHOD_FAT);
                    pCode = pILMethod->Fat.GetCode();
                    codeSize = pILMethod->Fat.GetCodeSize();
                }
                else
                {
                    headerSize = sizeof(COR_ILMETHOD_TINY);
                    pCode = pILMethod->Tiny.GetCode();
                    codeSize = pILMethod->Tiny.GetCodeSize();
                }

                for (BYTE *i = pCode, *i_end = i + codeSize; i != i_end; ++i)
                    mbDateTimeget_Body.Put<BYTE>(*i);        
            }



            BYTE *pNewILFunctionBody = NULL;
            {
                COR_ILMETHOD_FAT fatHeader;
                ::ZeroMemory(&fatHeader, sizeof(COR_ILMETHOD_FAT));
                fatHeader.SetMaxStack(3);
                fatHeader.SetCodeSize(mbDateTimeget_Body.Size());
                fatHeader.SetLocalVarSigTok(mdsDateTimeget_NowLocals);
                fatHeader.SetFlags(CorILMethod_InitLocals);
                
                unsigned headerSize = COR_ILMETHOD::Size(&fatHeader, false);
                unsigned totalSize = headerSize + mbDateTimeget_Body.Size();

                pNewILFunctionBody = reinterpret_cast<BYTE*>(pMethodMalloc->Alloc(totalSize));
                BYTE *pBuffer = pNewILFunctionBody;

                pBuffer += COR_ILMETHOD::Emit(headerSize, &fatHeader, false, pBuffer);
                ::memcpy_s(pBuffer, totalSize - headerSize, mbDateTimeget_Body.Ptr(), 
                           mbDateTimeget_Body.Size());
#ifdef OUTPUT_DEBUG
                {
                    cout << "New Method Body: ";
                    for (BYTE *i = mbDateTimeget_Body.Ptr(), *i_end = i + mbDateTimeget_Body.Size(); i != i_end; ++i)
                        cout << format("%|1$02X| ") % static_cast<INT>(*i);

                    cout << endl;
                }
#endif
            }



            // Set new method body!!
            hr = m_pInfo->SetILFunctionBody(modId, mdt, pNewILFunctionBody);
            if (FAILED(hr))
                BOOST_THROW_EXCEPTION(CppAnonymCOMException(hr));
        }
    }


    return S_OK;
}
