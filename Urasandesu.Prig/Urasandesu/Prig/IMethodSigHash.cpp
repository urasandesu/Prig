/* 
 * File: IMethodSigHash.cpp
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

#ifndef URASANDESU_PRIG_IMETHODSIGHASH_H
#include <Urasandesu/Prig/IMethodSigHash.h>
#endif

namespace Urasandesu { namespace Prig { 

    namespace IMethodSigHashDetail {

        IMethodSigHashImpl::result_type IMethodSigHashImpl::operator()(param_type v) const
        {
            using Urasandesu::CppAnonym::Collections::SequenceHashValue;
            using Urasandesu::Swathe::Metadata::TypeKinds;

            auto hash = [](IParameter const *pParam) { return HashValue(pParam); };
            auto hasRet = static_cast<INT>(v->GetReturnType()->GetKind() != TypeKinds::TK_VOID);
            auto isStatic = static_cast<INT>(v->IsStatic());
            return hasRet ^ SequenceHashValue(v->GetParameters(), hash) ^ isStatic;
        }



        IMethodSigHashImpl::result_type IMethodSigHashImpl::HashValue(IParameter const *pParam)
        {
            using Urasandesu::Swathe::Metadata::ParameterAttributes;

            auto dwattr = pParam->GetAttribute() & (ParameterAttributes::PA_IN | ParameterAttributes::PA_OUT);
            auto isByRef = static_cast<INT>(pParam->GetParameterType()->IsByRef());
            return dwattr ^ isByRef;
        }

    }   // namespace IMethodSigHashDetail {
    
}}   // namespace Urasandesu { namespace Prig { 
