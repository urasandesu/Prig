/* 
 * File: ListerCommand.cpp
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

#ifndef PRIG_LISTERCOMMAND_H
#include <prig/ListerCommand.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

namespace prig { 

    namespace ListerCommandDetail {
        
        int ListerCommandImpl::Execute()
        {
            using namespace Urasandesu::CppAnonym::Json;
            using boost::property_tree::wptree;
            using std::endl;
            using std::regex_constants::icase;
            using std::wregex;
            using std::wcout;
            using Urasandesu::Prig::PrigConfig;
            
            
            auto pattern = wregex(m_filter, icase);
            
            auto prigConfigPath = PrigConfig::GetConfigPath();
            
            auto config = PrigConfig();
            config.TryDeserializeFrom(prigConfigPath);
            
            auto pt = wptree();
            auto pkgspt = wptree();
            BOOST_FOREACH (auto const &pkg, config.Packages)
            {
                if (!m_filter.empty() && 
                    !regex_search(pkg.Name, pattern) &&
                    !regex_search(pkg.Source.native(), pattern))
                    continue;
                
                auto pkgpt = wptree();
                pkgpt.put(L"Name", pkg.Name);
                pkgpt.put(L"Source", pkg.Source.native());
                auto additionalDlgtspt = wptree();
                BOOST_FOREACH (auto const &additionalDlgt, pkg.AdditionalDelegates)
                {
                    auto additionalDlgtpt = wptree();
                    additionalDlgtpt.put(L"FullName", additionalDlgt.FullName);
                    additionalDlgtpt.put(L"HintPath", additionalDlgt.HintPath.native());
                    additionalDlgtspt.push_back(make_pair(L"", additionalDlgtpt));
                }
                pkgpt.add_child(L"AdditionalDelegates", additionalDlgtspt);
                pkgspt.push_back(make_pair(L"", pkgpt));
            }
            pt.add_child(L"Packages", pkgspt); 
            
            wcout << pt << endl;
            
            return 0;
        }

        
        
        void ListerCommandImpl::SetFilter(wstring const &filter)
        {
            _ASSERTE(m_filter.empty());
            m_filter = filter;
        }

        
        
        void ListerCommandImpl::SetLocalonly(bool localonly)
        {
            m_localonly = localonly;
        }

    } // namespace ListerCommandDetail {

}   // namespace prig { 
