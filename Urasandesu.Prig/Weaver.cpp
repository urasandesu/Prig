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
                auto isStatic = static_cast<INT>(v->IsStatic());
                return hasRet ^ SequenceHashValue(v->GetParameters(), hash) ^ isStatic;
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

        using std::vector;
        using Urasandesu::CppAnonym::Traits::EqualityComparable;
        using Urasandesu::CppAnonym::Collections::SequenceEqual;

        struct IMethodSigEqualToImpl : 
            EqualityComparable<IMethod const *>
        {
            result_type operator()(param_type x, param_type y) const
            {
                auto isStaticX = x->IsStatic();
                auto isStaticY = y->IsStatic();
                return isStaticX == isStaticY && ReturnTypeEqual(x->GetReturnType(), y->GetReturnType()) && ParametersEqual(x->GetParameters(), y->GetParameters());
            }
            
            static result_type ReturnTypeEqual(IType const *pRetTypeX, IType const *pRetTypeY)
            {
                auto hasRetX = pRetTypeX->GetKind() != TypeKinds::TK_VOID;
                auto hasRetY = pRetTypeY->GetKind() != TypeKinds::TK_VOID;
                return hasRetX == hasRetY;
            }
            
            static result_type ParametersEqual(vector<IParameter const *> const &paramsX, vector<IParameter const *> const &paramsY)
            {
                auto equalTo = [](IParameter const *pParamX, IParameter const *pParamY) { return ParameterEqual(pParamX, pParamY); };
                return SequenceEqual(paramsX, paramsY, equalTo);
            }
            
            static result_type ParameterEqual(IParameter const *pParamX, IParameter const *pParamY)
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
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"InitializeCore(IUnknown *: 0x%|1$08X|)", reinterpret_cast<SIZE_T>(pICorProfilerInfoUnk));

        auto _ = guard_type(m_lock);

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        m_pProfInfo = pRuntime->GetInfo<ProfilingInfo>();
        
        auto pProcProf = m_pProfInfo->AttachToCurrentProcess(pICorProfilerInfoUnk);
        pProcProf.Persist();
        pProcProf->SetEventMask(ProfilerEvents::PE_MONITOR_APPDOMAIN_LOADS | 
                                ProfilerEvents::PE_MONITOR_MODULE_LOADS | 
                                ProfilerEvents::PE_MONITOR_JIT_COMPILATION | 
                                ProfilerEvents::PE_DISABLE_INLINING | 
                                ProfilerEvents::PE_DISABLE_OPTIMIZATIONS | 
                                ProfilerEvents::PE_USE_PROFILE_IMAGES);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ShutdownCore()
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW(L"ShutdownCore()");

        auto _ = guard_type(m_lock);
        
        m_pProfInfo->DetachFromCurrentProcess();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"AppDomainCreationStartedCore(AppDomainID: 0x%|1$08X|)", appDomainId);

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainCreationFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"AppDomainCreationFinishedCore(AppDomainID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", appDomainId, hrStatus);

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pDomainProf = pProcProf->AttachToAppDomain(appDomainId);
        pDomainProf.Persist();

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownStartedCore( 
        /* [in] */ AppDomainID appDomainId)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"AppDomainShutdownStartedCore(AppDomainID: 0x%|1$08X|)", appDomainId);

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromAppDomain(appDomainId);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::AppDomainShutdownFinishedCore( 
        /* [in] */ AppDomainID appDomainId,
        /* [in] */ HRESULT hrStatus)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"AppDomainShutdownFinishedCore(AppDomainID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", appDomainId, hrStatus);

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::ModuleLoadStartedCore( 
        /* [in] */ ModuleID moduleId)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"ModuleLoadStartedCore(ModuleID: 0x%|1$08X|)", moduleId);

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



    STDMETHODIMP CWeaverImpl::ModuleLoadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        BOOST_LOG_FUNCTION();

        using boost::lexical_cast;
        using boost::log::current_scope;
        using boost::filesystem::current_path;
        using boost::filesystem::exists;
        using boost::filesystem::path;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;

        CPPANONYM_D_LOGW2(L"ModuleLoadFinishedCore(ModuleID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", moduleId, hrStatus);

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pModProf = pProcProf->AttachToModule(moduleId);
        CPPANONYM_D_LOGW1(L"Current Path: %|1$s|", current_path().native());
        auto modPath = path(pModProf->GetName());
        auto modPrigPath = path(modPath.stem().native() + L".Prig.dll");
        if (!exists(modPrigPath))
            return S_OK;
        
        BOOST_LOG_NAMED_SCOPE("if (exists(modPrigPath))");
        CPPANONYM_D_LOGW1(L"Detour module: %|1$s| is found. Start to modify the module.", modPrigPath.native());
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
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW1(L"ModuleUnloadStartedCore(ModuleID: 0x%|1$08X|)", moduleId);   // TODO: この辺実装中。。。

        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        pProcProf->DetachFromModule(moduleId);

        return S_OK;
    }


        
    STDMETHODIMP CWeaverImpl::ModuleUnloadFinishedCore( 
        /* [in] */ ModuleID moduleId,
        /* [in] */ HRESULT hrStatus)
    {
        BOOST_LOG_FUNCTION();
        CPPANONYM_D_LOGW2(L"ModuleUnloadFinishedCore(ModuleID: 0x%|1$08X|, HRESULT: 0x%|2$08X|)", moduleId, hrStatus);

        auto _ = guard_type(m_lock);

        return S_OK;
    }



    STDMETHODIMP CWeaverImpl::JITCompilationStartedCore( 
        /* [in] */ FunctionID functionId,
        /* [in] */ BOOL fIsSafeToBlock)
    {
        BOOST_LOG_FUNCTION();

        using boost::lexical_cast;
        using boost::filesystem::path;
        using std::vector;
        using Urasandesu::CppAnonym::Utilities::AnyPtr;
        
        CPPANONYM_D_LOGW2(L"JITCompilationStartedCore(FunctionID: 0x%|1$08X|, BOOL: 0x%|2$08X|)", functionId, fIsSafeToBlock);
        
        auto _ = guard_type(m_lock);

        auto *pProcProf = m_pProfInfo->GetCurrentProcessProfiler();
        auto pFuncProf = pProcProf->AttachToFunction(functionId);
        auto pModProf = pFuncProf->AttachToModule();
        if (!pModProf.IsPersisted())
            return S_OK;
        
        BOOST_LOG_NAMED_SCOPE("pModProf.IsPersisted()");
        CPPANONYM_D_LOGW(L"This method is candidate for the module that can detour.");
        
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
            BOOST_LOG_NAMED_SCOPE("!prigData.m_indirectablesInit && pDisp->IsCOMMetaDataDispenserPrepared()");
            auto const *pPrigFrmwrk = pDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce");
            auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();
            auto const *pIndAttrType = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
            auto const *pPrigAsm = pDisp->GetAssemblyFrom(prigData.m_modPrigPath);
            auto indAttrs = pPrigAsm->GetCustomAttributes(pIndAttrType);
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
        
        auto const *pMethodGen = pFuncProf->GetMethodGenerator();
        auto mdt = pMethodGen->GetToken();
        CPPANONYM_D_LOGW1(L"Token: 0x%|1$08X|", mdt);
        auto result = Iterator();
        if ((result = prigData.m_indirectables.find(mdt)) == prigData.m_indirectables.end())
            return S_OK;
        
        BOOST_LOG_NAMED_SCOPE("(result = prigData.m_indirectables.find(mdt)) != prigData.m_indirectables.end()");
        CPPANONYM_D_LOGW(L"This method is marked by IndirectableAttribute.");
        pFuncProf.Persist();
        
        auto pNewBodyProf = pFuncProf->NewFunctionBody();
        auto *pNewBodyGen = pNewBodyProf->GetMethodBodyGenerator();
        
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
        BOOST_LOG_FUNCTION();

        using boost::filesystem::path;
        using std::vector;

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
        {
            pNewBodyGen->Emit(OpCodes::Ldarg_0);
            auto const *pDeclaringType = pMethodGen->GetDeclaringType();
            if (pDeclaringType->IsValueType())
                pNewBodyGen->Emit(OpCodes::Ldobj, pDeclaringType);
        }
    
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
        ULONG GetHashCode() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
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
            using std::vector;

            auto pType_Invoke = pType->GetMethod(L"Invoke");
            auto params = vector<IParameter const *>();
            auto explicitThis = ExplicitThis();
            if (m_pTarget->IsStatic())
            {
                params = m_pTarget->GetParameters();
            }
            else
            {
                explicitThis.Set(m_pTarget->GetDeclaringType());
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
        using std::vector;
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
            genericArgs.push_back(pTarget->GetDeclaringType());
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

}   // namespace CWeaverDetail {



HRESULT CWeaver::FinalConstruct()
{
    CPPANONYM_D_LOGW(L"CWeaver::FinalConstruct()");
    
    //::_CrtDbgBreak();
    
    return S_OK;
}

void CWeaver::FinalRelease()
{
}
