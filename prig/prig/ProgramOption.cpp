/* 
 * File: ProgramOption.cpp
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

#ifndef PRIG_COMMANDFACTORY_H
#include <prig/CommandFactory.h>
#endif

#ifndef PRIG_PROGRAMOPTION_H
#include <prig/ProgramOption.h>
#endif

namespace prig { 

    namespace ProgramOptionDetail {

        ProgramOptionImpl::ProgramOptionImpl(int argc, WCHAR* argv[]) : 
            m_args(ToVector(argc, argv))
        { }



        auto const STYLE = 
            cls::allow_short | 
            cls::short_allow_next | 
            cls::allow_long | 
            cls::long_allow_next | 
            cls::allow_guessing | 
            cls::allow_dash_for_short | 
            cls::allow_long_disguise | 
            cls::case_insensitive;
        
        
        
        Command ReparseToRunnerCommand(variables_map const &globalVm, wparsed_options const &globalParsed)
        {
            using boost::program_options::include_positional;

            auto runnerDesc = options_description
                (
                "RUNNER OPTIONS\n"
                "Specify options to start with Prig.\n"
                "\n"
                "==== EXAMPLE 1 ====\n"
                "CMD C:\\> prig run -process \"C:\\Program Files (x86)\\NUnit 2.6.3\\bin\\nunit-console-x86.exe\" -arguments \"Test.program1.dll /domain=None\"\n"
                "\n"
                "This command executes the process designated by -process option with the program options designated by -arguments option.\n"
                "\n");
            
            runnerDesc.add_options()(
                "help", 
                "Display help message.\n"
                "\n")
                (
                "process", 
                wvalue<wstring>()->required(), 
                "Process to execute. If its path contains any spaces, you shall surround with \"(double quotes).\n"
                "This option is mandatory.\n"
                "\n")
                (
                "arguments", 
                wvalue<wstring>(), 
                "Program options for process. If the option is plural, you shall separate each option with space and surround with \"(double quotes).\n"
                "\n");

            if (globalVm.count("help"))
                return CommandFactory::MakeHelpCommand(runnerDesc);

            auto opts = collect_unrecognized(globalParsed.options, include_positional);
            opts.erase(opts.begin());

            auto runnerVm = variables_map();
            auto runnerParsed = wcommand_line_parser(opts).options(runnerDesc).style(STYLE).run();
            store(runnerParsed, runnerVm);
            notify(runnerVm);

            auto process = runnerVm["process"].as<wstring>();
            auto arguments = runnerVm.count("arguments") ? runnerVm["arguments"].as<wstring>() : wstring();
            return CommandFactory::MakeRunnerCommand(process, arguments);
        }


        
        Command ReparseToStubberCommand(variables_map const &globalVm, wparsed_options const &globalParsed)
        {
            BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException());
        }


        
        Command ReparseToDisassemblerCommand(variables_map const &globalVm, wparsed_options const &globalParsed)
        {
            using boost::program_options::include_positional;

            auto dasmlrDesc = options_description
                (
                "DISASSEMBLER OPTIONS\n"
                "Specify options to disassemble with Prig.\n"
                "\n"
                "==== EXAMPLE 1 ====\n"
                "CMD C:\\> prig dasm -assembly \"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n"
                "\n"
                "This command disassembles the assembly designated by -assembly option.\n"
                "\n"
                "==== EXAMPLE 2 ====\n"
                "CMD C:\\> prig dasm -assemblyfrom \"C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\mscorlib.dll\"\n"
                "\n"
                "This command disassembles the assembly designated by -assemblyfrom option.\n"
                "\n");
            
            dasmlrDesc.add_options()(
                "help", 
                "Display help message.\n"
                "\n")
                (
                "assembly", 
                wvalue<wstring>(), 
                "Assembly Display Name to disassemble. If its name contains any spaces, you shall surround with \"(double quotes).\n"
                "Either this option or 'assemblyfrom' is mandatory.\n"
                "\n")
                (
                "assemblyfrom", 
                wvalue<wstring>(), 
                "Assembly Path to disassemble. If its path contains any spaces, you shall surround with \"(double quotes).\n"
                "Either this option or 'assembly' is mandatory.\n"
                "\n");

            if (globalVm.count("help"))
                return CommandFactory::MakeHelpCommand(dasmlrDesc);

            auto opts = collect_unrecognized(globalParsed.options, include_positional);
            opts.erase(opts.begin());

            auto dasmlrVm = variables_map();
            auto dasmlrParsed = wcommand_line_parser(opts).options(dasmlrDesc).style(STYLE).run();
            store(dasmlrParsed, dasmlrVm);
            notify(dasmlrVm);

            auto asmFullName = !dasmlrVm.count("assembly") ? wstring() : dasmlrVm["assembly"].as<wstring>();
            auto asmPath = !dasmlrVm.count("assemblyfrom") ? wstring() : dasmlrVm["assemblyfrom"].as<wstring>();
            if (asmFullName.empty() && asmPath.empty() || !asmFullName.empty() && !asmPath.empty())
                return CommandFactory::MakeHelpCommand(dasmlrDesc);
            
            return CommandFactory::MakeDisassemblerCommand(asmFullName, asmPath);
        }
        


        Command ProgramOptionImpl::Parse() const
        {
            using boost::program_options::positional_options_description;
            using boost::to_lower;

            auto globalDesc = options_description
                (
                "GLOBAL OPTIONS\n"
                "The below options are applied to all commands.\n"
                "\n");
            
            globalDesc.add_options()(
                "help", 
                "Display help message.\n"
                "\n")
                (
                "command", 
                wvalue<wstring>(), 
                "Command to execute. Currently supported commands are the followings:\n"
                "\n"
                "  * run\n"
                "  * dasm\n"
                "\n"
                "About each command usage, see each command's help.\n"
                "\n"
                "==== EXAMPLE 1 ====\n"
                "CMD C:\\> prig run -help\n"
                "\n"
                "This command displays Runner's command help.\n"
                "\n")
                (
                "subargs", 
                wvalue<vector<wstring> >(), 
                "Arguments for command.\n"
                "\n");

	        if (m_args.empty())
                return CommandFactory::MakeHelpCommand(globalDesc);

            auto globalPosDesc = positional_options_description();
            globalPosDesc.
                add("command", 1).
                add("subargs", -1);

            auto globalVm = variables_map();
            auto globalParsed = wcommand_line_parser(m_args).options(globalDesc).positional(globalPosDesc).allow_unregistered().style(STYLE).run();
            store(globalParsed, globalVm);
            notify(globalVm);

            if (globalVm.count("command"))
            {
                auto cmd = globalVm["command"].as<wstring>();
                to_lower(cmd);

                if (cmd == L"run")
                    return ReparseToRunnerCommand(globalVm, globalParsed);
                else if (cmd == L"stub")
                    return ReparseToStubberCommand(globalVm, globalParsed);
                else if (cmd == L"dasm")
                    return ReparseToDisassemblerCommand(globalVm, globalParsed);
            }

            return CommandFactory::MakeHelpCommand(globalDesc);
        }

        
        
        vector<wstring> ProgramOptionImpl::ToVector(int argc, WCHAR* argv[])
        {
            if (argc <= 1)
                return vector<wstring>();
            
            auto v = vector<wstring>();
            v.reserve(argc - 1);
            for (auto i = argv + 1, i_end = i + argc - 1; i != i_end; ++i)
                v.push_back(*i);
            
            return v;
        }

    }   // namespace ProgramOptionDetail {

    
    
    ProgramOption::ProgramOption(int argc, WCHAR* argv[]) : 
        base_type(argc, argv)
    { }

}   // namespace prig { 
