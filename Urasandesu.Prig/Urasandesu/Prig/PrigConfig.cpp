/* 
 * File: PrigConfig.cpp
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

#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#include <Urasandesu/Prig/PrigConfig.h>
#endif

namespace Urasandesu { namespace Prig { 

    namespace PrigConfigDetail {

        void PrigPackageConfig::AddOrUpdateAdditionalDelegates(vector<PrigAdditionalDelegateConfig> const &additionalDlgts)
        {
            using boost::range::find_if;
                
            BOOST_FOREACH (auto const &additionalDlgt, additionalDlgts)
            {
                auto result = find_if(AdditionalDelegates, [&additionalDlgt](PrigAdditionalDelegateConfig const &dlgt){ return dlgt.FullName == additionalDlgt.FullName; });
                if (result != AdditionalDelegates.end())
                    *result = additionalDlgt;
                else
                    AdditionalDelegates.push_back(additionalDlgt);
            }
        }



        wstring const PrigConfig::FILE_NAME = L"Prig.config";

        path const &PrigConfig::GetPackagePath()
        {
            using Urasandesu::CppAnonym::Environment;

            static auto pkgPath = path(Environment::GetEnvironmentVariable(L"URASANDESU_PRIG_PACKAGE_FOLDER"));
            return pkgPath;
        }
        
        path const &PrigConfig::GetToolsPath()
        {
            static auto toolsPath = GetPackagePath() / L"tools";
            return toolsPath;
        }
        
        path const &PrigConfig::GetLibPath()
        {
            static auto libPath = GetPackagePath() / L"lib";
            return libPath;
        }
        
        path const &PrigConfig::GetConfigPath()
        {
            static auto prigConfigPath = GetToolsPath() / PrigConfig::FILE_NAME;
            return prigConfigPath;
        }

        bool PrigConfig::TrySerializeFrom(path const &prigConfigPath)
        {
            using boost::serialization::make_nvp;
            using namespace Urasandesu::CppAnonym::IO;
            using namespace Urasandesu::CppAnonym::Xml;

            if (!exists(prigConfigPath))
                return false;
            
            FromUTF8(prigConfigPath) >> make_nvp("Config", *this);
            return true;
        }

        bool PrigConfig::TryDeserializeTo(path const &prigConfigPath) const
        {
            using boost::serialization::make_nvp;
            using namespace Urasandesu::CppAnonym::IO;
            using namespace Urasandesu::CppAnonym::Xml;

            ToUTF8(prigConfigPath) << make_nvp("Config", *this);
            return true;
        }

        optional<PrigPackageConfig> PrigConfig::FindPackage(path const &source) const
        {
            using Urasandesu::CppAnonym::Collections::FindIf;

            return FindIf(Packages, [&source](PrigPackageConfig const &pkg) { return equivalent(pkg.Source, source); });
        }

        vector<PrigPackageConfig> PrigConfig::FindPackages(wstring const &pkgName) const
        {
            using boost::adaptors::filtered;
            using boost::copy;
            using std::back_inserter;

            auto pkgs = vector<PrigPackageConfig>();
            if (pkgName == L"all")
            {
                pkgs = Packages;
            }
            else
            {
                auto isTarget = [&pkgName](PrigPackageConfig const &pkg) { return pkg.Name == pkgName; };
                copy(Packages | filtered(isTarget), back_inserter(pkgs));
            }

            return pkgs;
        }

        void PrigConfig::DeletePackage(PrigPackageConfig const &item)
        {
            auto isTarget = [&item](PrigPackageConfig const &pkg) { return boost::iequals(pkg.Source.native(), item.Source.native()); };
            auto result = boost::remove_if(Packages, isTarget);
            if (result != Packages.end())
                Packages.erase(result, Packages.end());
        }

    }   // namespace PrigConfigDetail {

}}   // namespace Urasandesu { namespace Prig { 
