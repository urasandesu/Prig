/* 
 * File: PrigConfig.h
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
#ifndef URASANDESU_PRIG_PRIGCONFIG_H
#define URASANDESU_PRIG_PRIGCONFIG_H

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef URASANDESU_PRIG_PRIGCONFIGFWD_H
#include <Urasandesu/Prig/PrigConfigFwd.h>
#endif

#include <boost/serialization/vector.hpp>

namespace boost { namespace serialization {

    template<class Archive>
    void serialize(Archive &ar, boost::filesystem::path &p, unsigned int version)
    {
        split_free(ar, p, version);
    }

    template<class Archive>
    void save(Archive &ar, boost::filesystem::path const &p, unsigned int version)
    {
        ar & make_nvp("Native", p.native());
    }

    template<class Archive>
    void load(Archive &ar, boost::filesystem::path &p, unsigned int version)
    {
        auto v = std::wstring();
        ar & make_nvp("Native", v);
        p = v;
    }

}}  // namespace boost { namespace serialization {

namespace Urasandesu { namespace Prig { 

    namespace PrigConfigDetail {

        using boost::filesystem::path;
        using std::vector;
        using std::wstring;

        class PrigAdditionalDelegateConfig
        {
        public: 
            wstring FullName;
            path HintPath;

        private:
            friend class boost::serialization::access;
            template <class Archive>
            void serialize(Archive& archive, unsigned int version)
            {
                archive & BOOST_SERIALIZATION_NVP(FullName);
                archive & BOOST_SERIALIZATION_NVP(HintPath);
            }
        };

        class PrigPackageConfig
        {
        public: 
            wstring Name;
            path Source;
            vector<PrigAdditionalDelegateConfig> AdditionalDelegates;

        private:
            friend class boost::serialization::access;
            template <class Archive>
            void serialize(Archive& archive, unsigned int version)
            {
                archive & BOOST_SERIALIZATION_NVP(Name);
                archive & BOOST_SERIALIZATION_NVP(Source);
                archive & BOOST_SERIALIZATION_NVP(AdditionalDelegates);
            }
        };

        class PrigConfig
        {
        public: 
            vector<PrigPackageConfig> Packages;

        private:
            friend class boost::serialization::access;
            template <class Archive>
            void serialize(Archive& archive, unsigned int version)
            {
                archive & BOOST_SERIALIZATION_NVP(Packages);
            }
        };

    }   // namespace PrigConfigDetail {
    
}}   // namespace Urasandesu { namespace Prig { 

BOOST_CLASS_VERSION(Urasandesu::Prig::PrigAdditionalDelegateConfig, 0);
BOOST_CLASS_VERSION(Urasandesu::Prig::PrigPackageConfig, 0);
BOOST_CLASS_VERSION(Urasandesu::Prig::PrigConfig, 0);

#endif  // URASANDESU_PRIG_PRIGCONFIG_H

