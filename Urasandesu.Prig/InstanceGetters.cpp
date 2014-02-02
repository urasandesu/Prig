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
#include "InstanceGetters.h"

typedef Urasandesu::CppAnonym::Collections::GlobalSafeDictionary<std::wstring, void const *> InstanceGetters;

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryAdd(LPCWSTR key, void const *pFuncPtr)
{
    auto &ing = InstanceGetters::GetInstance();
    auto result = ing.TryAdd(std::wstring(key), pFuncPtr);
    D_WCOUT3(L"InstanceGettersTryAdd(key: %|1$s|, pFuncPtr: 0x%|2$08X|): %|3$d|", key, pFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryGet(LPCWSTR key, void const **ppFuncPtr)
{
    _ASSERTE(ppFuncPtr != NULL);
    auto &ing = InstanceGetters::GetInstance();
    auto result = ing.TryGet(std::wstring(key), *ppFuncPtr);
    D_WCOUT3(L"InstanceGettersTryGet(key: %|1$s|, *ppFuncPtr: 0x%|2$08X|): %|3$d|", key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryRemove(LPCWSTR key, void const **ppFuncPtr)
{
    _ASSERTE(ppFuncPtr != NULL);
    auto &ing = InstanceGetters::GetInstance();
    auto result = ing.TryRemove(std::wstring(key), *ppFuncPtr);
    D_WCOUT3(L"InstanceGettersTryRemove(key: %|1$s|, *ppFuncPtr: 0x%|2$08X|): %|3$d|", key, *ppFuncPtr, result);
    return result;
}

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersClear()
{
    D_WCOUT(L"InstanceGettersClear()");
    auto &ing = InstanceGetters::GetInstance();
    ing.Clear();
}
