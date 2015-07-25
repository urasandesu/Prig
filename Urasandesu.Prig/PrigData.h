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
#ifndef PRIGDATA_H
#define PRIGDATA_H

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef INDIRECTIONDELEGATESFWD_H
#include <IndirectionDelegatesFwd.h>
#endif

#ifndef INDIRECTIONPREPARATION_H
#include <IndirectionPreparation.h>
#endif

#ifndef PRIGDATAFWD_H
#include <PrigDataFwd.h>
#endif

namespace PrigDataDetail {

    using namespace Urasandesu::Swathe::Metadata;
    using boost::filesystem::path;
    using boost::noncopyable;
    using boost::unordered_map;
    using boost::ptr_vector;
    using std::wstring;

    struct PrigData : 
        noncopyable
    {
        PrigData();

        template<class T>
        T &NewPreparation(mdToken mdt)
        {
            auto *p = new T();
            m_indirectionPreparationList.push_back(p);
            m_indirectables[mdt] = m_indirectionPreparationList.size() - 1;
            return *p;
        }

        wstring m_corVersion;
        path m_indDllPath;
        unordered_map<mdToken, SIZE_T> m_indirectables;
        bool m_indirectablesInit;
        ptr_vector<IndirectionDelegates> m_indirectionDelegatesList;
        ptr_vector<IndirectionPreparation> m_indirectionPreparationList;
        IModule const *m_pMSCorLibDll;
        IModule const *m_pPrigFrameworkDll;
    };

}   // namespace PrigDataDetail {

#endif  // PRIGDATA_H

