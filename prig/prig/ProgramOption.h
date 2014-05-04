/* 
 * File: ProgramOption.h
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
#ifndef PRIG_PROGRAMOPTION_H
#define PRIG_PROGRAMOPTION_H

#ifndef PRIG_PROGRAMOPTIONFWD_H
#include <prig/ProgramOptionFwd.h>
#endif

namespace prig { 

    namespace ProgramOptionDetail {

        namespace cls = boost::program_options::command_line_style;
        using boost::program_options::variables_map;
        using boost::program_options::wparsed_options;
        using boost::program_options::options_description;
        using boost::program_options::wcommand_line_parser;
        using boost::program_options::wvalue;
        using std::wstring;
        using boost::noncopyable;
        using boost::shared_ptr;
        using std::vector;
        using std::wstring;

        class ProgramOptionImpl : 
            noncopyable
        {
        public: 
            ProgramOptionImpl(int argc, WCHAR* argv[]);

            Command Parse() const;

        private:
            static vector<wstring> ToVector(int argc, WCHAR* argv[]);

            vector<wstring> m_args;
        };

    }   // namespace ProgramOptionDetail {

    struct ProgramOption : 
        ProgramOptionDetail::ProgramOptionImpl
    {
        typedef ProgramOptionDetail::ProgramOptionImpl base_type;

        ProgramOption(int argc, WCHAR* argv[]);
    };
    
}   // namespace prig { 

#endif  // PRIG_PROGRAMOPTION_H

