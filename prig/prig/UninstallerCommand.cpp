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

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

namespace prig { 

    namespace UninstallerCommandDetail {
        
        using boost::filesystem::path;
        using std::vector;
        using Urasandesu::Prig::PrigConfig;
        using Urasandesu::Prig::PrigPackageConfig;

        void RemoveSymLinksForPrigLib(PrigPackageConfig const &pkg, vector<path> const &libAllAsmPaths)
        {
            using boost::system::error_code;

            if (!exists(pkg.Source))
                return;
            
            BOOST_FOREACH (auto const &libAllAsmPath, libAllAsmPaths)
            {
                auto symlink = pkg.Source / libAllAsmPath.filename();
                auto ec = error_code();
                remove(symlink, ec);
            }
        }

        void RemoveSymLinksForAdditionalDelegates(PrigPackageConfig const &pkg)
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

        void UnregisterFromGAC(vector<path> const &asmPaths)
        {
            using namespace Urasandesu::CppAnonym::Traits;
            using namespace Urasandesu::Swathe::Hosting;
            using namespace Urasandesu::Swathe::Fusion;

            using boost::remove_reference;
            using std::map;

            auto const *pHost = HostInfo::CreateHost();
            auto const &runtimes = pHost->GetRuntimes();
            
            typedef remove_reference<RemoveConst<decltype(runtimes)>::type>::type Runtimes;
            typedef Runtimes::key_type Key;
            typedef Runtimes::mapped_type Mapped;
            auto orderedRuntimes = map<Key, Mapped>(runtimes.begin(), runtimes.end());
            auto const *pLatestRuntime = (*orderedRuntimes.rbegin()).second;
            auto const *pFuInfo = pLatestRuntime->GetInfo<FusionInfo>();
            auto pAsmCache = pFuInfo->NewAssemblyCache();
            BOOST_FOREACH (auto const &asmPath, asmPaths)
                pAsmCache->UninstallAssembly(asmPath.stem().native());
        }

        int UninstallerCommandImpl::Execute()
        {
            using boost::wformat;
            using std::endl;
            using std::wcout;
            using Urasandesu::CppAnonym::Environment;
            
            
            auto prigConfigPath = PrigConfig::GetConfigPath();
            
            auto config = PrigConfig();
            config.TryDeserializeFrom(prigConfigPath);
            
            auto pkgs = config.FindPackages(m_package);
            if (pkgs.empty())
            {
                wcout << wformat(L"The specified package:%|1$s| is not found. It might have been already uninstalled.") % m_package << endl;
                return 0;
            }

            if (!Environment::IsCurrentProcessRunAsAdministrator())
            {
                wcout << wformat(L"Uninstallation process has failed. To uninstall pacakge:%|1$s|, you have to run this command as Administrator.") % m_package << endl;
                return 1;
            }

            auto libAllAsmPaths = PrigConfig::GetLibAllAssemblyPaths();
            BOOST_FOREACH (auto const &pkg, pkgs)
            {
                RemoveSymLinksForPrigLib(pkg, libAllAsmPaths);
                RemoveSymLinksForAdditionalDelegates(pkg);
                config.DeletePackage(pkg);
            }

            if (config.Packages.empty())
                UnregisterFromGAC(PrigConfig::GetLibFrameworkAssemblyPaths());
            
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
