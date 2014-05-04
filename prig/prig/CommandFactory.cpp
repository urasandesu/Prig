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

    }   // namespace CommandFactoryDetail {

}   // namespace prig { 
