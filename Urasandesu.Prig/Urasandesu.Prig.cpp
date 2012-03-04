// Urasandesu.Prig.cpp : DLL エクスポートの実装です。


#include "stdafx.h"
#include "resource.h"
#include "UrasandesuPrig_i.h"
#include "dllmain.h"


// DLL を OLE によってアンロードできるようにするかどうかを指定します。
STDAPI DllCanUnloadNow(void)
{
			return _AtlModule.DllCanUnloadNow();
	}

// 要求された型のオブジェクトを作成するクラス ファクトリを返します。
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
		return _AtlModule.DllGetClassObject(rclsid, riid, ppv);
}

// DllRegisterServer - エントリをシステム レジストリに追加します。
STDAPI DllRegisterServer(void)
{
	// オブジェクト、タイプ ライブラリおよびタイプ ライブラリ内のすべてのインターフェイスを登録します
	HRESULT hr = _AtlModule.DllRegisterServer();
		return hr;
}

// DllUnregisterServer - エントリをレジストリから削除します。
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = _AtlModule.DllUnregisterServer();
		return hr;
}

// DllInstall - ユーザーおよびコンピューターごとのシステム レジストリ エントリを追加または削除します。
STDAPI DllInstall(BOOL bInstall, LPCWSTR pszCmdLine)
{
	HRESULT hr = E_FAIL;
	static const wchar_t szUserSwitch[] = L"user";

	if (pszCmdLine != NULL)
	{
		if (_wcsnicmp(pszCmdLine, szUserSwitch, _countof(szUserSwitch)) == 0)
		{
			ATL::AtlSetPerUserRegistration(true);
		}
	}

	if (bInstall)
	{	
		hr = DllRegisterServer();
		if (FAILED(hr))
		{
			DllUnregisterServer();
		}
	}
	else
	{
		hr = DllUnregisterServer();
	}

	return hr;
}


