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
        
        namespace CsvAssemblyNameExDetail {
            
            using Urasandesu::CppAnonym::Utilities::AutoPtr;
            using Urasandesu::Swathe::Fusion::AssemblyNameRange;
            using Urasandesu::Swathe::Metadata::IAssembly;
            using Urasandesu::Swathe::Metadata::MetadataDispenser;
            using std::wostream;

            class CsvAssemblyNameEx
            {
            public:
                CsvAssemblyNameEx(AutoPtr<AssemblyNameRange> const &pAsmNames, MetadataDispenser const *pDisp) : 
                    m_pAsmNames(pAsmNames), 
                    m_pDisp(pDisp)
                { }
            
            private:
                friend wostream &operator <<(wostream &os, CsvAssemblyNameEx &asmNameEx)
                {
                    using std::endl;

                    os << L"Name,Version,Culture,PublicKeyToken,ProcessorArchitecture,FullName,ImageRuntimeVersion" << endl;
                    BOOST_FOREACH (auto const &pAsmName, *asmNameEx.m_pAsmNames)
                    {
                        auto const *pAsm = asmNameEx.m_pDisp->GetAssembly(pAsmName->GetFullName());
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
                    
                    return csv.str();
                }

                AutoPtr<AssemblyNameRange> m_pAsmNames;
                MetadataDispenser const *m_pDisp;
            };

        }

        using CsvAssemblyNameExDetail::CsvAssemblyNameEx;
        
        
        
        int DisassemblerCommandImpl::Execute()
        {
            using namespace Urasandesu::CppAnonym::Traits;
            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Fusion;
            using namespace Urasandesu::Swathe::Metadata;
            using boost::remove_reference;
            using std::wcout;
            using std::map;
            
            auto const *pHost = HostInfo::CreateHost();
            auto const &runtimes = pHost->GetRuntimes();
            
            typedef remove_reference<RemoveConst<decltype(runtimes)>::type>::type Runtimes;
            typedef Runtimes::key_type Key;
            typedef Runtimes::mapped_type Mapped;
            auto orderedRuntimes = map<Key, Mapped>(runtimes.begin(), runtimes.end());
            auto const *pLatestRuntime = (*orderedRuntimes.rbegin()).second;
            
            auto const *pFuInfo = pLatestRuntime->GetInfo<FusionInfo>();
            auto pCondition = pFuInfo->NewAssemblyName(m_asmFullName, NewAssemblyNameFlags::NANF_CANOF_PARSE_DISPLAY_NAME);
            auto pAsmNames = pFuInfo->EnumerateAssemblyName(pCondition, AssemblyCacheFlags::ACF_GAC);
            
            auto const *pMetaInfo = pLatestRuntime->GetInfo<MetadataInfo>();
            auto *pDisp = pMetaInfo->CreateDispenser();
            
            wcout << CsvAssemblyNameEx(pAsmNames, pDisp);
            
            return 0;
        }

        
        
        void DisassemblerCommandImpl::SetAssembly(wstring const &asmFullName)
        {
            _ASSERTE(m_asmFullName.empty());
            _ASSERTE(!asmFullName.empty());
            m_asmFullName = asmFullName;
        }

    } // namespace DisassemblerCommandDetail {

}   // namespace prig { 
