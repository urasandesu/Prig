/* 
 * File: prig.cpp
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


// prig.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#ifndef PRIG_PROGRAMOPTION_H
#include <prig/ProgramOption.h>
#endif

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

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

struct ExecuteCommandVisitor : 
    boost::static_visitor<int>
{
    template<class T>
    int operator ()(T const &pCommand) const
    {
        return pCommand->Execute();
    }
};

int wmain(int argc, WCHAR* argv[])
{
    using namespace prig;
    using boost::lexical_cast;
    using boost::bad_lexical_cast;
    using Urasandesu::CppAnonym::Environment;

#ifdef _DEBUG
    auto dbgBreak = 0ul;
    try
    {
        auto strDbgBreak = Environment::GetEnvironmentVariable("URASANDESU_PRIG_DEBUGGING_BREAK");
        dbgBreak = lexical_cast<DWORD>(strDbgBreak);
    }
    catch(bad_lexical_cast const &)
    {
        dbgBreak = -1;
    }
    if (dbgBreak == 0)
        ::_CrtDbgBreak();
    else if (dbgBreak != -1)
        ::Sleep(dbgBreak);
#endif

    try
    {
        auto option = ProgramOption(argc, argv);
        auto command = option.Parse();
        return boost::apply_visitor(ExecuteCommandVisitor(), command);
    }
    catch (...)
    {
        std::cerr << boost::diagnostic_information(boost::current_exception()) << std::endl;
        return 1;
    }
}

