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

#endif  // #ifndef INDIRETIONINTERFACES_H