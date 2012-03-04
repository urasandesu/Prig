// Weaver.h : CWeaver の宣言

#pragma once
#include "resource.h"       // メイン シンボル



#include "UrasandesuPrig_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "DCOM の完全サポートを含んでいない Windows Mobile プラットフォームのような Windows CE プラットフォームでは、単一スレッド COM オブジェクトは正しくサポートされていません。ATL が単一スレッド COM オブジェクトの作成をサポートすること、およびその単一スレッド COM オブジェクトの実装の使用を許可することを強制するには、_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA を定義してください。ご使用の rgs ファイルのスレッド モデルは 'Free' に設定されており、DCOM Windows CE 以外のプラットフォームでサポートされる唯一のスレッド モデルと設定されていました。"
#endif

using namespace ATL;


// CWeaver
#ifdef UNP
#error This .h temporarily reserves the word "UNP" that means "Urasandesu::CppAnonym::Profiling".
#else
#define UNP Urasandesu::CppAnonym::Profiling

class ATL_NO_VTABLE CWeaver :
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CWeaver, &CLSID_Weaver>,
    public UNP::ICorProfilerCallback2Impl<ICorProfilerCallback2>,
	public IDispatchImpl<IWeaver, &IID_IWeaver, &LIBID_UrasandesuPrigLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CWeaver()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_WEAVER)


BEGIN_COM_MAP(CWeaver)
	COM_INTERFACE_ENTRY(IWeaver)
	COM_INTERFACE_ENTRY(IDispatch)
    COM_INTERFACE_ENTRY(ICorProfilerCallback2)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

    HRESULT FinalConstruct();

    void FinalRelease();

protected:
    STDMETHOD(InitializeCore)(
        /* [in] */ IUnknown *pICorProfilerInfoUnk);

    STDMETHOD(ShutdownCore)(void);

    STDMETHOD(ModuleLoadStartedCore)( 
        /* [in] */ ModuleID moduleId);

    STDMETHOD(JITCompilationStartedCore)( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock);

private:
    boost::timer m_timer;

    ATL::CComPtr<ICorProfilerInfo2> m_pInfo;
    ATL::CComPtr<IMetaDataEmit2> m_pEmtMSCorLib;

    static std::wstring const MODULE_NAME_OF_MS_COR_LIB;
    static std::wstring const TYPE_NAME_OF_SYSTEM_DATE_TIME_GET_NOW;

    std::auto_ptr<BYTE> m_pDateTimeget_NowBody;
};

OBJECT_ENTRY_AUTO(__uuidof(Weaver), CWeaver)

#undef UNP
#endif
