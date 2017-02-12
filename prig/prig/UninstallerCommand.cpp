/* 
 * File: UninstallerCommand.cpp
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

#ifndef PRIG_UNINSTALLERCOMMAND_H
#include <prig/UninstallerCommand.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

namespace prig { 

    namespace UninstallerCommandDetail {
        
        using boost::filesystem::path;
        using std::vector;
        using Urasandesu::Prig::PrigConfig;
        using Urasandesu::Prig::PrigPackageConfig;
        
        void FillPrigDllNames(path const &libPath, vector<wstring> &prigDllNames)
        {
            using boost::filesystem::recursive_directory_iterator;
            
            for (recursive_directory_iterator i(libPath), i_end; i != i_end; ++i)
            {
                if (!is_regular_file(i->status()))
                    continue;
                
                prigDllNames.push_back(i->path().filename().native());
            }
        }

        void RemoveSymLinkForPrigLib(PrigPackageConfig const &pkg, vector<wstring> const &prigDllNames)
        {
            using boost::system::error_code;

            if (!exists(pkg.Source))
                return;
            
            BOOST_FOREACH (auto const &prigDllName, prigDllNames)
            {
                auto symlink = pkg.Source / prigDllName;
                auto ec = error_code();
                remove(symlink, ec);
            }
        }

        void RemoveSymLinkForAdditionalDelegates(PrigPackageConfig const &pkg)
        {
            using boost::system::error_code;

            if (!exists(pkg.Source))
                return;
            
            BOOST_FOREACH (auto const &additionalDlgt, pkg.AdditionalDelegates)
            {
                auto symlink = pkg.Source / additionalDlgt.HintPath.filename();
                auto ec = error_code();
                remove(symlink, ec);
            }
        }

        int UninstallerCommandImpl::Execute()
        {
            using boost::wformat;
            using std::endl;
            using std::wcout;
            
            
            auto prigConfigPath = PrigConfig::GetConfigPath();
            
            auto config = PrigConfig();
            config.TryDeserializeFrom(prigConfigPath);
            
            auto pkgs = config.FindPackages(m_package);
            auto hasProcessed = false;
            auto prigDllNames = vector<wstring>();
            BOOST_FOREACH (auto const &pkg, pkgs)
            {
                if (!hasProcessed)
                {
                    hasProcessed = true;
                    
                    auto libPath = PrigConfig::GetLibPath();
                    FillPrigDllNames(libPath, prigDllNames);
                }
                
                RemoveSymLinkForPrigLib(pkg, prigDllNames);
                RemoveSymLinkForAdditionalDelegates(pkg);
                config.DeletePackage(pkg);
            }
            if (!hasProcessed)
            {
                wcout << wformat(L"The specified package:%|1$s| is not found. It might have been already uninstalled.") % m_package << endl;
                return 0;
            }
            
            config.TrySerializeTo(prigConfigPath);
            return 0;
        }

        
        
        void UninstallerCommandImpl::SetPackage(wstring const &package)
        {
            _ASSERTE(m_package.empty());
            m_package = package;
        }

    } // namespace UninstallerCommandDetail {

}   // namespace prig { 
