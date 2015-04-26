/* 
 * File: CommandFactory.cpp
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

#ifndef PRIG_HELPCOMMAND_H
#include <prig/HelpCommand.h>
#endif

#ifndef PRIG_RUNNERCOMMAND_H
#include <prig/RunnerCommand.h>
#endif

#ifndef PRIG_STUBBERCOMMAND_H
#include <prig/StubberCommand.h>
#endif

#ifndef PRIG_DISASSEMBLERCOMMAND_H
#include <prig/DisassemblerCommand.h>
#endif

#ifndef PRIG_INSTALLERCOMMAND_H
#include <prig/InstallerCommand.h>
#endif

#ifndef PRIG_LISTERCOMMAND_H
#include <prig/ListerCommand.h>
#endif

#ifndef PRIG_UPDATERCOMMAND_H
#include <prig/UpdaterCommand.h>
#endif

#ifndef PRIG_UNINSTALLERCOMMAND_H
#include <prig/UninstallerCommand.h>
#endif

#ifndef PRIG_COMMANDFACTORY_H
#include <prig/CommandFactory.h>
#endif

namespace prig { 

    namespace CommandFactoryDetail {

        shared_ptr<HelpCommand> CommandFactoryImpl::MakeHelpCommand(options_description const &desc)
        {
            using boost::make_shared;

            auto pCommand = make_shared<HelpCommand>();
            pCommand->SetHelp(desc);
            return pCommand;
        }



        shared_ptr<RunnerCommand> CommandFactoryImpl::MakeRunnerCommand(wstring const &process, wstring const &arguments)
        {
            using boost::make_shared;

            auto pCommand = make_shared<RunnerCommand>();
            pCommand->SetProcess(process);
            pCommand->SetArguments(arguments);
            return pCommand;
        }
        
        
        
        shared_ptr<DisassemblerCommand> CommandFactoryImpl::MakeDisassemblerCommand(wstring const &asmFullName, wstring const &asmPath)
        {
            using boost::make_shared;

            auto pCommand = make_shared<DisassemblerCommand>();
            pCommand->SetAssembly(asmFullName);
            pCommand->SetAssemblyFrom(asmPath);
            return pCommand;
        }
        
        
        
        shared_ptr<InstallerCommand> CommandFactoryImpl::MakeInstallerCommand(wstring const &package, wstring const &source)
        {
            using boost::make_shared;

            auto pCommand = make_shared<InstallerCommand>();
            pCommand->SetPackage(package);
            pCommand->SetSource(source);
            return pCommand;
        }
        
        
        
        shared_ptr<ListerCommand> CommandFactoryImpl::MakeListerCommand(wstring const &filter, bool localonly)
        {
            using boost::make_shared;

            auto pCommand = make_shared<ListerCommand>();
            pCommand->SetFilter(filter);
            pCommand->SetLocalonly(localonly);
            return pCommand;
        }
        
        
        
        shared_ptr<UpdaterCommand> CommandFactoryImpl::MakeUpdaterCommand(wstring const &package, wstring const &delegate_)
        {
            using boost::make_shared;

            auto pCommand = make_shared<UpdaterCommand>();
            pCommand->SetPackage(package);
            pCommand->SetDelegate(delegate_);
            return pCommand;
        }
        
        
        
        shared_ptr<UninstallerCommand> CommandFactoryImpl::MakeUninstallerCommand(wstring const &package)
        {
            using boost::make_shared;

            auto pCommand = make_shared<UninstallerCommand>();
            pCommand->SetPackage(package);
            return pCommand;
        }

    }   // namespace CommandFactoryDetail {

}   // namespace prig { 
