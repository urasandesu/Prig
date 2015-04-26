/* 
 * File: RunnerCommand.cpp
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

#ifndef PRIG_RUNNERCOMMAND_H
#include <prig/RunnerCommand.h>
#endif

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

namespace prig { 

    namespace RunnerCommandDetail {

        void PrepareEnvironment()
        {
            using Urasandesu::CppAnonym::Environment;
            using Urasandesu::Swathe::Profiling::ProfilingSpecialValues;

            auto enableProfiling = Environment::GetEnvironmentVariable(ProfilingSpecialValues::ENABLE_PROFILING_KEY);
            auto profiler = Environment::GetEnvironmentVariable(ProfilingSpecialValues::PROFILER_KEY);
            if (!enableProfiling.empty() && !profiler.empty())
                Environment::SetEnvironmentVariable(ProfilingSpecialValues::EXTERNAL_PROFILER_KEY, profiler);
            
            Environment::SetEnvironmentVariable(ProfilingSpecialValues::ENABLE_PROFILING_KEY, ProfilingSpecialValues::ENABLE_PROFILING_VALUE_ENABLED);
            Environment::SetEnvironmentVariable(ProfilingSpecialValues::PROFILER_KEY, L"{532C1F05-F8F3-4FBA-8724-699A31756ABD}");
        }



        int Run(wstring const &process, wstring const &arguments)
        {
            using Urasandesu::CppAnonym::Diagnostics::Process;

            return Process::StartAndWaitForExit(process, arguments);
        }



        int RunnerCommandImpl::Execute()
        {
            PrepareEnvironment();
            return Run(m_process, m_arguments);
        }

    
    
        void RunnerCommandImpl::SetProcess(wstring const &process)
        {
            _ASSERTE(m_process.empty());
            _ASSERTE(!process.empty());
            m_process = process;
        }



        void RunnerCommandImpl::SetArguments(wstring const &arguments)
        {
            m_arguments = arguments;
        }

    } // namespace RunnerCommandDetail {

}   // namespace prig { 
