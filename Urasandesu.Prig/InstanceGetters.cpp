/*
 * File: InstanceGetters.cpp
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


#include "StdAfx.h"

#ifndef INDIRETIONINTERFACES_H
#include <InstanceGetters.h>
#endif

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

typedef Urasandesu::CppAnonym::Collections::GlobalSafeDictionary<std::wstring, void const *> FunctionCollection;
typedef Urasandesu::CppAnonym::Collections::GlobalSafeDictionary<AppDomainID, boost::shared_ptr<FunctionCollection>> AppDomainFunctionCollection;

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryAdd(AppDomainID appDomainId, LPCWSTR key, void const *pFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();
    
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    auto &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    if (!adfc.IsDisabledProcessing())  // NOTE: we should skip if processing is disabled because "while (...)" will be an infinite loop.
    {
        auto pFc = boost::make_shared<FunctionCollection>();
        while (!adfc.GetOrAdd(appDomainId, pFc, pFc));

        result = pFc->TryAdd(std::wstring(key), pFuncPtr);
    }
    CPPANONYM_D_LOGW4(L"InstanceGettersTryAdd(appDomainId: 0x%1%, key: %|2$s|, pFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, pFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryGet(AppDomainID appDomainId, LPCWSTR key, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (!(result = adfc.TryGet(appDomainId, pFc)))
        goto EXIT;

    result = pFc->TryGet(std::wstring(key), *ppFuncPtr);
EXIT:
    CPPANONYM_D_LOGW4(L"InstanceGettersTryGet(appDomainId: 0x%1%, key: %|2$s|, *ppFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryRemove(AppDomainID appDomainId, LPCWSTR key, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (!(result = adfc.TryGet(appDomainId, pFc)))
        goto EXIT;

    result = pFc->TryRemove(std::wstring(key), *ppFuncPtr);
EXIT:
    CPPANONYM_D_LOGW4(L"InstanceGettersTryRemove(appDomainId: 0x%1%, key: %|2$s|, *ppFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersGetOrAdd(AppDomainID appDomainId, LPCWSTR key, void const *pFuncPtr, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    if (!adfc.IsDisabledProcessing())  // NOTE: we should skip if processing is disabled because "while (...)" will be an infinite loop.
    {
        auto pFc = boost::make_shared<FunctionCollection>();
        while (!adfc.GetOrAdd(appDomainId, pFc, pFc));

        result = pFc->GetOrAdd(std::wstring(key), pFuncPtr, *ppFuncPtr);
    }
    CPPANONYM_D_LOGW5(L"InstanceGettersGetOrAdd(appDomainId: 0x%1%, key: %|2$s|, pFuncPtr: 0x%3%, *ppFuncPtr: 0x%4%): %|5$d|", reinterpret_cast<void *>(appDomainId), key, pFuncPtr, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersClear(AppDomainID appDomainId)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (!(result = adfc.TryGet(appDomainId, pFc)))
        goto EXIT;
    
    pFc->Clear();
EXIT:
    CPPANONYM_D_LOGW1(L"InstanceGettersClear(appDomainId: 0x%1%)", reinterpret_cast<void *>(appDomainId));
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersEnterDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    auto &adfc = AppDomainFunctionCollection::GetInstance();
    adfc.EnterDisabledProcessing();
    CPPANONYM_D_LOGW(L"InstanceGettersEnterDisabledProcessing()");
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersExitDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    auto &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = adfc.ExitDisabledProcessing();
    CPPANONYM_D_LOGW1(L"InstanceGettersExitDisabledProcessing(): %|1$d|", result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersIsDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = adfc.IsDisabledProcessing();
    CPPANONYM_D_LOGW1(L"InstanceGettersIsDisabledProcessing(): %|1$d|", result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersDebugWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_D_LOGW1(L"InstanceGettersDebugWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersCurrentAppDomainEmpty(void *pProcProf)
{
    using namespace Urasandesu::Swathe::Profiling;
    
    auto pDomainProf = static_cast<ProcessProfiler *>(pProcProf)->GetCurrentAppDomain();
    auto appDomainId = pDomainProf->GetID();
    
    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (result = !adfc.TryGet(appDomainId, pFc))    // NOTE: to determine whether empty, so "no data" is TRUE.
        goto EXIT;
    
    result = pFc->Empty();
EXIT:
    CPPANONYM_D_LOGW2(L"InstanceGettersEmpty(pProcProf: 0x%1%): %|2$d|", pProcProf, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersCurrentAppDomainUnload(void *pProcProf)
{
    using namespace Urasandesu::Swathe::Profiling;
    
    auto pDomainProf = static_cast<ProcessProfiler *>(pProcProf)->GetCurrentAppDomain();
    auto appDomainId = pDomainProf->GetID();
    
    auto &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (!(result = adfc.TryRemove(appDomainId, pFc)))
        goto EXIT;
    
EXIT:
    CPPANONYM_D_LOGW2(L"InstanceGettersUnload(pProcProf: 0x%1%): %|2$d|", pProcProf, result);
    return result;
}
