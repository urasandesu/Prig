// dllmain.h : モジュール クラスの宣言

class CUrasandesuPrigModule : public ATL::CAtlDllModuleT< CUrasandesuPrigModule >
{
public :
	DECLARE_LIBID(LIBID_UrasandesuPrigLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_URASANDESUPRIG, "{484F25F7-5212-434D-A09B-3D3484CDEE2A}")
};

extern class CUrasandesuPrigModule _AtlModule;
