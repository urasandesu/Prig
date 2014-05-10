/* 
 * File: IMethodSigEqualTo.h
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
#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTO_H
#define URASANDESU_PRIG_IMETHODSIGEQUALTO_H

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

#ifndef URASANDESU_PRIG_IMETHODSIGEQUALTOFWD_H
#include <Urasandesu/Prig/IMethodSigEqualToFwd.h>
#endif

namespace Urasandesu { namespace Prig { 

    namespace IMethodSigEqualToDetail {

        using std::vector;
        using Urasandesu::CppAnonym::Traits::EqualityComparable;
        using Urasandesu::Swathe::Metadata::IMethod;
        using Urasandesu::Swathe::Metadata::IType;
        using Urasandesu::Swathe::Metadata::IParameter;

        struct IMethodSigEqualToImpl : 
            EqualityComparable<IMethod const *>
        {
            result_type operator()(param_type x, param_type y) const;
            static result_type ReturnTypeEqual(IType const *pRetTypeX, IType const *pRetTypeY);
            static result_type ParametersEqual(vector<IParameter const *> const &paramsX, vector<IParameter const *> const &paramsY);
            static result_type ParameterEqual(IParameter const *pParamX, IParameter const *pParamY);
        };

    }   // namespace IMethodSigEqualToDetail {

    struct IMethodSigEqualTo : 
        IMethodSigEqualToDetail::IMethodSigEqualToImpl
    {
    };
    
}}   // namespace Urasandesu { namespace Prig { 

#endif  // URASANDESU_PRIG_IMETHODSIGEQUALTO_H

