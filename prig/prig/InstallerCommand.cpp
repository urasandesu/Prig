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

namespace prig { 

    namespace InstallerCommandDetail {
        
        using Urasandesu::Prig::PrigConfig;
        using Urasandesu::Prig::PrigPackageConfig;

        bool CreateSymLinkForPrigLib(path const &libPath, path const &source)
        {
            using boost::filesystem::create_symlink;
            using boost::filesystem::exists;
            using boost::filesystem::recursive_directory_iterator;
            using boost::filesystem::remove;
            using boost::system::error_code;

            if (!exists(source))
                return false;
            
            for (recursive_directory_iterator i(libPath), i_end; i != i_end; ++i)
            {
                if (!is_regular_file(i->status()))
                    continue;
                
                auto symlink = source / i->path().filename();
                auto ec = error_code();
                remove(symlink, ec);
                create_symlink(i->path(), symlink);
            }

            return true;
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

            auto libPath = PrigConfig::GetLibPath();
            if (!CreateSymLinkForPrigLib(libPath, m_source))
            {
                wcout << wformat(L"The specified source:%|1$s| is invalid.") % m_source << endl;
                return 1;
            }
            
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
