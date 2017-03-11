/* 
 * File: InstallerCommand.cpp
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

#ifndef PRIG_INSTALLERCOMMAND_H
#include <prig/InstallerCommand.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

namespace prig { 

    namespace InstallerCommandDetail {
        
        using std::vector;
        using Urasandesu::Prig::PrigConfig;
        using Urasandesu::Prig::PrigPackageConfig;

        void RemoveSymLinks(vector<path> const &asmPaths, path const &source)
        {
            using boost::system::error_code;

            BOOST_FOREACH (auto const &asmPath, asmPaths)
            {
                auto symlink = source / asmPath.filename();
                auto ec = error_code();
                remove(symlink, ec);
            }
        }

        void CreateSymLinks(vector<path> const &asmPaths, path const &source)
        {
            BOOST_FOREACH (auto const &asmPath, asmPaths)
            {
                auto symlink = source / asmPath.filename();
                create_symlink(asmPath, symlink);
            }
        }

        void RegisterToGAC(vector<path> const &asmPaths)
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
                pAsmCache->InstallAssembly(AssemblyCacheInstallFlags::ACIF_REFRESH, asmPath);
        }
        
        int InstallerCommandImpl::Execute()
        {
            using boost::wformat;
            using std::endl;
            using std::wcout;
            using Urasandesu::CppAnonym::Environment;
            
            
            auto prigConfigPath = PrigConfig::GetConfigPath();
            
            auto config = PrigConfig();
            config.TryDeserializeFrom(prigConfigPath);
            
            auto result = config.FindPackage(m_source);
            if (result)
            {
                wcout << wformat(L"The specified source:%|1$s| has already installed.") % m_source << endl;
                return 0;
            }
            
            if (!Environment::IsCurrentProcessRunAsAdministrator())
            {
                wcout << wformat(L"Installation process has failed. To install source:%|1$s|, you have to run this command as Administrator.") % m_source << endl;
                return 1;
            }

            if (!exists(m_source))
            {
                wcout << wformat(L"The specified source:%|1$s| is invalid.") % m_source << endl;
                return 1;
            }

            RemoveSymLinks(PrigConfig::GetLibAllAssemblyPaths(), m_source);
            CreateSymLinks(PrigConfig::GetLibNonFrameworkAssemblyPaths(), m_source);
            if (config.Packages.empty())
                RegisterToGAC(PrigConfig::GetLibFrameworkAssemblyPaths());
            
            auto package = PrigPackageConfig();
            package.Name = m_package;
            package.Source = m_source;
            config.Packages.push_back(package);
            config.TrySerializeTo(prigConfigPath);
            return 0;
        }

        
        
        void InstallerCommandImpl::SetPackage(wstring const &package)
        {
            _ASSERTE(m_package.empty());
            m_package = package;
        }

        
        
        void InstallerCommandImpl::SetSource(path const &source)
        {
            _ASSERTE(m_source.empty());
            m_source = source;
        }

    } // namespace InstallerCommandDetail {

}   // namespace prig { 
