/* 
 * File: UpdaterCommand.cpp
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

#ifndef PRIG_UPDATERCOMMAND_H
#include <prig/UpdaterCommand.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

namespace prig { 

    namespace UpdaterCommandDetail {
        
        using boost::filesystem::path;
        using std::vector;
        using Urasandesu::Prig::PrigConfig;
        using Urasandesu::Prig::PrigPackageConfig;
        using Urasandesu::Prig::PrigAdditionalDelegateConfig;

        bool FillDelegatePaths(wstring const &delegate_, vector<path> &delegatePaths)
        {
            using boost::is_any_of;
            using boost::split;
            
            auto strs = vector<wstring>();
            split(strs, delegate_, is_any_of(L";"));
            BOOST_FOREACH (auto const &str, strs)
            {
                if (str.empty())
                    continue;
                
                delegatePaths.push_back(str);
                if (!exists(delegatePaths.back()))
                    return false;
            }
            
            return true;
        }

        void FillAdditionalDelegates(vector<path> const &delegatePaths, vector<PrigAdditionalDelegateConfig> &additionalDlgts)
        {
            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Metadata;

            auto const *pHost = HostInfo::CreateHost();
            auto const *pRuntime = pHost->GetRuntime(L"v4.0.30319");    // Specifying the fixed version is enough because we want to just get the display name of the assembly.
            auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
            auto const *pDisp = pMetaInfo->CreateDispenser();
            BOOST_FOREACH (auto const &delegatePath, delegatePaths)
            {
                auto additionalDlgt = PrigAdditionalDelegateConfig();
                additionalDlgt.FullName = pDisp->GetAssemblyFrom(delegatePath)->GetFullName();
                additionalDlgt.HintPath = delegatePath;
                additionalDlgts.push_back(additionalDlgt);
            }
        }

        void CreateSymLinkForAdditionalDelegates(PrigPackageConfig const &pkg, vector<PrigAdditionalDelegateConfig> const &additionalDlgts)
        {
            using boost::system::error_code;

            BOOST_FOREACH (auto const &additionalDlgt, additionalDlgts)
            {
                auto symlink = pkg.Source / additionalDlgt.HintPath.filename();
                auto ec = error_code();
                remove(symlink, ec);
                create_symlink(additionalDlgt.HintPath, symlink);
            }
        }

        int UpdaterCommandImpl::Execute()
        {
            using boost::wformat;
            using std::endl;
            using std::wcout;
            using Urasandesu::CppAnonym::CppAnonymNotSupportedException;
            
            if (!m_delegate.empty())
            {
                auto delegatePaths = vector<path>();
                if (!FillDelegatePaths(m_delegate, delegatePaths))
                {
                    wcout << wformat(L"The specified delegate:\"%|1$s|\" is invalid. It contains the path %|2$s| that doesn't exist.") % m_delegate % delegatePaths.back() << endl;
                    return 1;
                }
                
                auto additionalDlgts = vector<PrigAdditionalDelegateConfig>();
                FillAdditionalDelegates(delegatePaths, additionalDlgts);
                
                auto prigConfigPath = PrigConfig::GetConfigPath();
                
                auto config = PrigConfig();
                config.TrySerializeFrom(prigConfigPath);
                
                auto hasProcessed = false;
                BOOST_FOREACH (auto &pkg, config.Packages)
                {
                    if (!hasProcessed)
                        hasProcessed = true;
                    
                    CreateSymLinkForAdditionalDelegates(pkg, additionalDlgts);
                    pkg.AddOrUpdateAdditionalDelegates(additionalDlgts);
                }
                if (!hasProcessed)
                {
                    wcout << wformat(L"The specified package:%|1$s| is not found. It might have been already uninstalled.") % m_package << endl;
                    return 1;
                }
                
                config.TryDeserializeTo(prigConfigPath);
                return 0;
            }
            
            BOOST_THROW_EXCEPTION(CppAnonymNotSupportedException(L"We shouldn't get here."));
        }

        
        
        void UpdaterCommandImpl::SetPackage(wstring const &package)
        {
            _ASSERTE(m_package.empty());
            m_package = package;
        }

        
        
        void UpdaterCommandImpl::SetDelegate(wstring const &delegate_)
        {
            _ASSERTE(m_delegate.empty());
            m_delegate = delegate_;
        }

    } // namespace UpdaterCommandDetail {

}   // namespace prig { 
