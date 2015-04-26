/* 
 * File: UninstallerCommand.h
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
#ifndef PRIG_UNINSTALLERCOMMAND_H
#define PRIG_UNINSTALLERCOMMAND_H

#ifndef PRIG_COMMANDFACTORYFWD_H
#include <prig/CommandFactoryFwd.h>
#endif

#ifndef PRIG_UNINSTALLERCOMMANDFWD_H
#include <prig/UninstallerCommandFwd.h>
#endif

namespace prig { 

    namespace UninstallerCommandDetail {

        using std::wstring;

        class UninstallerCommandImpl
        {
        public:
            int Execute();

        private:
            friend class CommandFactoryDetail::CommandFactoryImpl;

            void SetPackage(wstring const &package);

            wstring m_package;
        };

    }   // namespace UninstallerCommandDetail {

    struct UninstallerCommand : 
        UninstallerCommandDetail::UninstallerCommandImpl
    {
    };
    
}   // namespace prig { 

#endif  // PRIG_UNINSTALLERCOMMAND_H

