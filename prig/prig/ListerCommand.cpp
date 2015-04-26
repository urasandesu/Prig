/* 
 * File: ListerCommand.cpp
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

#ifndef PRIG_LISTERCOMMAND_H
#include <prig/ListerCommand.h>
#endif

namespace prig { 

    namespace ListerCommandDetail {
        
        int ListerCommandImpl::Execute()
        {
            std::wcout << L"filter: " << m_filter << std::endl;
            std::wcout << L"localonly: " << m_localonly << std::endl;
            BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException());
        }

        
        
        void ListerCommandImpl::SetFilter(wstring const &filter)
        {
            _ASSERTE(m_filter.empty());
            m_filter = filter;
        }

        
        
        void ListerCommandImpl::SetLocalonly(bool localonly)
        {
            m_localonly = localonly;
        }

    } // namespace ListerCommandDetail {

}   // namespace prig { 
