/* 
 * File: PrigData.h
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
#ifndef URASANDESU_PRIG_PRIGDATA_H
#define URASANDESU_PRIG_PRIGDATA_H

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef URASANDESU_PRIG_IMETHODSIGHASH_H
#include <Urasandesu/Prig/IMethodSigHash.h>
#endif

#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTO_H
#include <Urasandesu/Prig/IMethodSigEqualTo.h>
#endif

#ifndef URASANDESU_PRIG_PRIGDATAFWD_H
#include <Urasandesu/Prig/PrigDataFwd.h>
#endif

namespace Urasandesu { namespace Prig { 

    namespace PrigDataDetail {

        using boost::filesystem::path;
        using boost::noncopyable;
        using boost::unordered_map;
        using boost::ptr_vector;
        using std::vector;
        using std::wstring;
        using Urasandesu::Swathe::Metadata::ICustomAttribute;
        using Urasandesu::Swathe::Metadata::IAssembly;
        using Urasandesu::Swathe::Metadata::IMethod;
        using Urasandesu::Swathe::Metadata::IType;
        
        struct IndirectionDelegates : 
            noncopyable
        {
            IndirectionDelegates() : 
                m_pIndirectionDelegatesAssembly(nullptr), 
                m_indirectionDelegatesInit(false)
            { }

            IAssembly const *m_pIndirectionDelegatesAssembly;
            vector<IType const *> m_indirectionDelegates;
            bool m_indirectionDelegatesInit;
        };

        struct PrigData : 
            noncopyable
        {
            PrigData() : 
                m_indirectablesInit(false)
            { }

            wstring m_corVersion;
            path m_indDllPath;
            unordered_map<mdToken, ICustomAttribute const *> m_indirectables;
            bool m_indirectablesInit;
            ptr_vector<IndirectionDelegates> m_indirectionDelegatesList;
            unordered_map<IMethod const *, IType const *, IMethodSigHash, IMethodSigEqualTo> m_indDlgtCache;
        };

    }   // namespace PrigDataDetail {
    
}}   // namespace Urasandesu { namespace Prig { 

#endif  // URASANDESU_PRIG_PRIGDATA_H

