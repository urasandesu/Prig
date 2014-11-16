/* 
 * File: IMethodSigEqualTo.cpp
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

#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTO_H
#include <Urasandesu/Prig/IMethodSigEqualTo.h>
#endif

namespace Urasandesu { namespace Prig { 

    namespace IMethodSigEqualToDetail {

        IMethodSigEqualToImpl::result_type IMethodSigEqualToImpl::operator()(param_type x, param_type y) const
        {
            auto isStaticX = x->IsStatic();
            auto isStaticY = y->IsStatic();
            return isStaticX == isStaticY && ReturnTypeEqual(x->GetReturnType(), y->GetReturnType()) && ParametersEqual(x->GetParameters(), y->GetParameters());
        }



        IMethodSigEqualToImpl::result_type IMethodSigEqualToImpl::ReturnTypeEqual(IType const *pRetTypeX, IType const *pRetTypeY)
        {
            using Urasandesu::Swathe::Metadata::TypeKinds;

            auto hasRetX = pRetTypeX->GetKind() != TypeKinds::TK_VOID;
            auto hasRetY = pRetTypeY->GetKind() != TypeKinds::TK_VOID;
            return hasRetX == hasRetY;
        }



        IMethodSigEqualToImpl::result_type IMethodSigEqualToImpl::ParametersEqual(vector<IParameter const *> const &paramsX, vector<IParameter const *> const &paramsY)
        {
            using Urasandesu::CppAnonym::Collections::SequenceEqual;

            auto equalTo = [](IParameter const *pParamX, IParameter const *pParamY) { return ParameterEqual(pParamX, pParamY); };
            return SequenceEqual(paramsX, paramsY, equalTo);
        }



        IMethodSigEqualToImpl::result_type IMethodSigEqualToImpl::ParameterEqual(IParameter const *pParamX, IParameter const *pParamY)
        {
            using Urasandesu::Swathe::Metadata::ParameterAttributes;

            auto isEqual = true;

            if (isEqual)
            {
                auto dwattrX = pParamX->GetAttribute() & (ParameterAttributes::PA_IN | ParameterAttributes::PA_OUT);
                auto dwattrY = pParamY->GetAttribute() & (ParameterAttributes::PA_IN | ParameterAttributes::PA_OUT);
                isEqual &= dwattrX == dwattrY;
            }
            
            if (isEqual)
                isEqual &= pParamX->GetParameterType()->IsByRef() == pParamY->GetParameterType()->IsByRef();
            
            return isEqual;
        }

    }   // namespace IMethodSigEqualToDetail {
    
}}   // namespace Urasandesu { namespace Prig { 
