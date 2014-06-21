/* 
 * File: DisassemblerCommand.cpp
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

#ifndef PRIG_DISASSEMBLERCOMMAND_H
#include <prig/DisassemblerCommand.h>
#endif

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

namespace prig { 

    namespace DisassemblerCommandDetail {
        
        namespace AbstractCsvAssemblyNameExDetail {

            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Metadata;
            using std::wostream;

            class AbstractCsvAssemblyNameEx
            {
            public:
                AbstractCsvAssemblyNameEx(RuntimeHost const *pRuntime) : 
                    m_pRuntime(pRuntime), 
                    m_pDisp(nullptr)
                { }
            
                MetadataDispenser const *GetMetadataDispenser() const
                {
                    if (!m_pDisp)
                    {
                        auto const *pMetaInfo = m_pRuntime->GetInfo<MetadataInfo>();
                        m_pDisp = pMetaInfo->CreateDispenser();
                    }
                    _ASSERTE(m_pDisp);
                    return m_pDisp;
                }

            protected: 
                virtual IAssemblyPtrRange GetAssemblies() const = 0;

                RuntimeHost const *m_pRuntime;

            private:
                friend wostream &operator <<(wostream &os, AbstractCsvAssemblyNameEx const &asmNameEx)
                {
                    using std::endl;

                    os << L"Name,Version,Culture,PublicKeyToken,ProcessorArchitecture,FullName,ImageRuntimeVersion,Location" << endl;
                    auto asms = asmNameEx.GetAssemblies();
                    BOOST_FOREACH (auto const *pAsm, asms)
                    {
                        os << ToCsv(pAsm) << endl;
                    }

                    return os;
                }
                
                static wstring ToCsv(IAssembly const *pAsm)
                {
                    using Urasandesu::CppAnonym::Collections::SequenceToString;
                    using std::wostringstream;
                    
                    auto csv = wostringstream();
                    csv << pAsm->GetName();
                    csv << L"," << pAsm->GetVersion();
                    csv << L"," << pAsm->GetCultureName();
                    csv << L"," << (!pAsm->GetStrongNameKey() ? wstring(L"null") : SequenceToString(pAsm->GetStrongNameKey()->GetPublicKeyToken()));
                    csv << L"," << pAsm->GetProcessorArchitectures()[0];
                    csv << L",\"" << pAsm->GetFullName() << "\"";
                    csv << L"," << pAsm->GetImageRuntimeVersion();
                    csv << L",\"" << pAsm->GetLocation().native() << "\"";
                    
                    return csv.str();
                }

                mutable MetadataDispenser const *m_pDisp;
            };

        }   // namespace AbstractCsvAssemblyNameExDetail {

        using AbstractCsvAssemblyNameExDetail::AbstractCsvAssemblyNameEx;



        namespace CsvAssemblyNameExDetail {
            
            using namespace Urasandesu::CppAnonym::Utilities;
            using namespace Urasandesu::Swathe::Fusion;
            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Metadata;

            class CsvAssemblyNameEx : 
                public AbstractCsvAssemblyNameEx
            {
            public:
                CsvAssemblyNameEx(RuntimeHost const *pRuntime, wstring const &asmFullName) : 
                    AbstractCsvAssemblyNameEx(pRuntime), 
                    m_asmFullName(asmFullName)
                { }
            
            protected: 
                IAssemblyPtrRange GetAssemblies() const
                {
                    using boost::adaptors::transformed;
                    using std::function;

                    _ASSERTE(!m_asmFullName.empty());

                    auto const *pFuInfo = m_pRuntime->GetInfo<FusionInfo>();
                    m_pCondition = pFuInfo->NewAssemblyName(m_asmFullName, NewAssemblyNameFlags::NANF_CANOF_PARSE_DISPLAY_NAME);
                    m_pAsmNames = pFuInfo->EnumerateAssemblyName(m_pCondition, AssemblyCacheFlags::ACF_GAC);
                    
                    auto toAssembly = function<IAssembly const *(AutoPtr<AssemblyName const> const &)>();
                    toAssembly = [this](AutoPtr<AssemblyName const> const &pAsmName) { return GetMetadataDispenser()->GetAssembly(pAsmName->GetFullName()); };
                    return *m_pAsmNames | transformed(toAssembly);
                }

            private:
                mutable AutoPtr<AssemblyName> m_pCondition;
                mutable AutoPtr<AssemblyNameRange> m_pAsmNames;
                wstring m_asmFullName;
            };

        }   // namespace CsvAssemblyNameExDetail {

        using CsvAssemblyNameExDetail::CsvAssemblyNameEx;



        namespace CsvAssemblyNameExFromDetail {
            
            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Metadata;
            using std::vector;

            class CsvAssemblyNameExFrom : 
                public AbstractCsvAssemblyNameEx
            {
            public:
                CsvAssemblyNameExFrom(RuntimeHost const *pRuntime, path const &asmPath) : 
                    AbstractCsvAssemblyNameEx(pRuntime), 
                    m_asmPath(asmPath)
                { }
            
            protected: 
                IAssemblyPtrRange GetAssemblies() const
                {
                    _ASSERTE(!m_asmPath.empty());

                    m_asms = vector<IAssembly const *>();
                    m_asms.push_back(GetMetadataDispenser()->GetAssemblyFrom(m_asmPath));
                    return m_asms;
                }

            private:
                mutable vector<IAssembly const *> m_asms;
                path m_asmPath;
            };

        }   // namespace CsvAssemblyNameExFromDetail {

        using CsvAssemblyNameExFromDetail::CsvAssemblyNameExFrom;
        
        
        
        int DisassemblerCommandImpl::Execute()
        {
            using namespace Urasandesu::CppAnonym::Traits;
            using namespace Urasandesu::Swathe::Hosting;
            using boost::remove_reference;
            using std::wcout;
            using std::map;
            
            _ASSERTE(!m_asmFullName.empty() || !m_asmPath.empty());
            
            auto const *pHost = HostInfo::CreateHost();
            auto const &runtimes = pHost->GetRuntimes();
            
            typedef remove_reference<RemoveConst<decltype(runtimes)>::type>::type Runtimes;
            typedef Runtimes::key_type Key;
            typedef Runtimes::mapped_type Mapped;
            auto orderedRuntimes = map<Key, Mapped>(runtimes.begin(), runtimes.end());
            auto const *pLatestRuntime = (*orderedRuntimes.rbegin()).second;
            
            if (!m_asmFullName.empty())
                wcout << CsvAssemblyNameEx(pLatestRuntime, m_asmFullName);
            else
                wcout << CsvAssemblyNameExFrom(pLatestRuntime, m_asmPath);
            
            return 0;
        }

        
        
        void DisassemblerCommandImpl::SetAssembly(wstring const &asmFullName)
        {
            _ASSERTE(m_asmFullName.empty());
            m_asmFullName = asmFullName;
        }

        
        
        void DisassemblerCommandImpl::SetAssemblyFrom(path const &asmPath)
        {
            _ASSERTE(m_asmPath.empty());
            m_asmPath = asmPath;
        }

    } // namespace DisassemblerCommandDetail {

}   // namespace prig { 
