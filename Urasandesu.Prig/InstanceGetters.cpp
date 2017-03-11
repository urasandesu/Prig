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

typedef boost::recursive_mutex RecursiveMutex;
typedef boost::lock_guard<RecursiveMutex> LockGuard;
RecursiveMutex g_lock;

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryAdd(AppDomainID appDomainId, LPCWSTR key, void const *pFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();
    
    auto _ = LockGuard(g_lock);
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    auto &adfc = AppDomainFunctionCollection::GetInstance();

    typedef AppDomainFunctionCollection::in_key_type InKey;
    typedef AppDomainFunctionCollection::in_value_type InValue;
    typedef AppDomainFunctionCollection::out_value_type OutValue;

    auto addValue = boost::make_shared<FunctionCollection>();
    auto updateValueFactory = std::function<void(InKey, InValue, OutValue)>();
    updateValueFactory = [](InKey inKey, InValue inValue, OutValue outValue) { outValue = inValue; };
    auto resultValue = adfc.AddOrUpdate(appDomainId, addValue, updateValueFactory);

    auto result = resultValue->TryAdd(std::wstring(key), pFuncPtr);

    CPPANONYM_D_LOGW4(L"InstanceGettersTryAdd(appDomainId: 0x%1%, key: %|2$s|, pFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, pFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryGet(AppDomainID appDomainId, LPCWSTR key, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    auto _ = LockGuard(g_lock);
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto &adfc = AppDomainFunctionCollection::GetInstance();

    typedef AppDomainFunctionCollection::in_key_type InKey;
    typedef AppDomainFunctionCollection::in_value_type InValue;
    typedef AppDomainFunctionCollection::out_value_type OutValue;

    auto addValue = boost::make_shared<FunctionCollection>();
    auto updateValueFactory = std::function<void(InKey, InValue, OutValue)>();
    updateValueFactory = [](InKey inKey, InValue inValue, OutValue outValue) { outValue = inValue; };
    auto resultValue = adfc.AddOrUpdate(appDomainId, addValue, updateValueFactory);

    auto result = resultValue->TryGet(std::wstring(key), *ppFuncPtr);

    CPPANONYM_D_LOGW4(L"InstanceGettersTryGet(appDomainId: 0x%1%, key: %|2$s|, *ppFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryRemove(AppDomainID appDomainId, LPCWSTR key, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    auto _ = LockGuard(g_lock);
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto &adfc = AppDomainFunctionCollection::GetInstance();

    typedef AppDomainFunctionCollection::in_key_type InKey;
    typedef AppDomainFunctionCollection::in_value_type InValue;
    typedef AppDomainFunctionCollection::out_value_type OutValue;

    auto addValue = boost::make_shared<FunctionCollection>();
    auto updateValueFactory = std::function<void(InKey, InValue, OutValue)>();
    updateValueFactory = [](InKey inKey, InValue inValue, OutValue outValue) { outValue = inValue; };
    auto resultValue = adfc.AddOrUpdate(appDomainId, addValue, updateValueFactory);

    auto result = resultValue->TryRemove(std::wstring(key), *ppFuncPtr);

    CPPANONYM_D_LOGW4(L"InstanceGettersTryRemove(appDomainId: 0x%1%, key: %|2$s|, *ppFuncPtr: 0x%3%): %|4$d|", reinterpret_cast<void *>(appDomainId), key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersGetOrAdd(AppDomainID appDomainId, LPCWSTR key, void const *pFuncPtr, void const **ppFuncPtr)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    auto _ = LockGuard(g_lock);
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    _ASSERTE(ppFuncPtr != NULL);
    auto &adfc = AppDomainFunctionCollection::GetInstance();

    typedef AppDomainFunctionCollection::in_key_type InKey;
    typedef AppDomainFunctionCollection::in_value_type InValue;
    typedef AppDomainFunctionCollection::out_value_type OutValue;

    auto addValue = boost::make_shared<FunctionCollection>();
    auto updateValueFactory = std::function<void(InKey, InValue, OutValue)>();
    updateValueFactory = [](InKey inKey, InValue inValue, OutValue outValue) { outValue = inValue; };
    auto resultValue = adfc.AddOrUpdate(appDomainId, addValue, updateValueFactory);

    auto result = resultValue->GetOrAdd(std::wstring(key), pFuncPtr, *ppFuncPtr);

    CPPANONYM_D_LOGW5(L"InstanceGettersGetOrAdd(appDomainId: 0x%1%, key: %|2$s|, pFuncPtr: 0x%3%, *ppFuncPtr: 0x%4%): %|5$d|", reinterpret_cast<void *>(appDomainId), key, pFuncPtr, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersClear(AppDomainID appDomainId)
{
    using Urasandesu::Prig::PrigConfig;

    CPPANONYM_LOG_FUNCTION();

    auto _ = LockGuard(g_lock);
    if (!PrigConfig::IsPrigAttached())
        appDomainId = 0;

    auto &adfc = AppDomainFunctionCollection::GetInstance();

    typedef AppDomainFunctionCollection::in_key_type InKey;
    typedef AppDomainFunctionCollection::in_value_type InValue;
    typedef AppDomainFunctionCollection::out_value_type OutValue;

    auto addValue = boost::make_shared<FunctionCollection>();
    auto updateValueFactory = std::function<void(InKey, InValue, OutValue)>();
    updateValueFactory = [](InKey inKey, InValue inValue, OutValue outValue) { outValue = inValue; };
    auto resultValue = adfc.AddOrUpdate(appDomainId, addValue, updateValueFactory);

    resultValue->Clear();

    CPPANONYM_D_LOGW1(L"InstanceGettersClear(appDomainId: 0x%1%)", reinterpret_cast<void *>(appDomainId));
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersEnterDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    try
    {
        g_lock.lock();

        auto &adfc = AppDomainFunctionCollection::GetInstance();
        adfc.EnterDisabledProcessing();
        CPPANONYM_D_LOGW(L"InstanceGettersEnterDisabledProcessing()");
    }
    catch (...)
    {
        g_lock.unlock();
    }
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersExitDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    auto result = BOOL();
    try
    {
        auto &adfc = AppDomainFunctionCollection::GetInstance();
        result = adfc.ExitDisabledProcessing();
    }
    catch (...)
    { }

    g_lock.unlock();

    CPPANONYM_D_LOGW1(L"InstanceGettersExitDisabledProcessing(): %|1$d|", result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersIsDisabledProcessing()
{
    CPPANONYM_LOG_FUNCTION();

    auto _ = LockGuard(g_lock);
    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = adfc.IsDisabledProcessing();
    CPPANONYM_D_LOGW1(L"InstanceGettersIsDisabledProcessing(): %|1$d|", result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersErrorWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_E_LOGW1(L"InstanceGettersErrorWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersWarningWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_W_LOGW1(L"InstanceGettersWarningWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersInfoWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_I_LOGW1(L"InstanceGettersInfoWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersVerboseWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_V_LOGW1(L"InstanceGettersVerboseWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersDebugWriteLine(LPCWSTR message)
{
    CPPANONYM_LOG_FUNCTION();

    CPPANONYM_D_LOGW1(L"InstanceGettersDebugWriteLine(message: %|1$s|)", message);
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersCurrentAppDomainEmpty(void *pProcProf)
{
    using namespace Urasandesu::Swathe::Profiling;
    
    CPPANONYM_LOG_FUNCTION();
    
    auto _ = LockGuard(g_lock);
    auto pDomainProf = static_cast<ProcessProfiler *>(pProcProf)->GetCurrentAppDomain();
    auto appDomainId = pDomainProf->GetID();
    
    auto const &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();   // NOTE: to determine whether empty, so "no data" is TRUE.
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (result = !adfc.TryGet(appDomainId, pFc))
        goto EXIT;
    
    result = pFc->Empty();
EXIT:
    CPPANONYM_D_LOGW2(L"InstanceGettersEmpty(pProcProf: 0x%1%): %|2$d|", pProcProf, result);
    return result;
}

BOOL InstanceGettersCurrentAppDomainUnload(AppDomainID appDomainId)
{
    CPPANONYM_LOG_FUNCTION();
    
    auto _ = LockGuard(g_lock);
    auto &adfc = AppDomainFunctionCollection::GetInstance();
    auto result = BOOL();
    auto pFc = boost::shared_ptr<FunctionCollection>();
    if (!(result = adfc.TryRemove(appDomainId, pFc)))
        goto EXIT;
    
EXIT:
    CPPANONYM_D_LOGW2(L"InstanceGettersUnload(appDomainId: 0x%1%): %|2$d|", reinterpret_cast<void *>(appDomainId), result);
    return result;
}
