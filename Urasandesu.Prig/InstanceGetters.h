/* 
 * File: InstanceGetters.h
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

#ifndef INDIRETIONINTERFACES_H
#define INDIRETIONINTERFACES_H

#ifdef URASANDESU_PRIG_EXPORTS
#define URASANDESU_PRIG_API __declspec(dllexport)
#else
#define URASANDESU_PRIG_API __declspec(dllimport)
#endif

EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryAdd(LPCWSTR key, void const *pFuncPtr);
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryGet(LPCWSTR key, void const **ppFuncPtr);
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersTryRemove(LPCWSTR key, void const **ppFuncPtr);
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersClear();
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(VOID) InstanceGettersEnterDisabledProcessing();
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersExitDisabledProcessing();
EXTERN_C URASANDESU_PRIG_API STDMETHODIMP_(BOOL) InstanceGettersIsDisabledProcessing();

#endif  // #ifndef INDIRETIONINTERFACES_H
