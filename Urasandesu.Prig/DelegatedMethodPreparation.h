/* 
 * File: DelegatedMethodPreparation.h
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
#ifndef DELEGATEDMETHODPREPARATION_H
#define DELEGATEDMETHODPREPARATION_H

#ifndef INDIRECTIONPREPARATION_H
#include <IndirectionPreparation.h>
#endif

#ifndef DELEGATEDMETHODPREPARATIONFWD_H
#include <DelegatedMethodPreparationFwd.h>
#endif

namespace DelegatedMethodPreparationDetail {

    using namespace Urasandesu::CppAnonym::Utilities;
    using namespace Urasandesu::Swathe::Metadata;
    using namespace Urasandesu::Swathe::Profiling;
    using std::vector;

    struct DelegatedMethodPreparation : 
        IndirectionPreparation
    {
        DelegatedMethodPreparation();

        void FillIndirectionPreparation(MetadataDispenser const *pDisp, MethodGenerator *pTarget, PrigData &prigData);
        void ResolveIndirectionPreparation(AssemblyGenerator *pAsmGen);
        void EmitInstantiation(MethodBodyGenerator *pNewBodyGen, ILocal const *pLocal5_0000) const;
        SIZE_T EmitIndirectMethodBody(MethodBodyGenerator *pNewBodyGen, MethodGenerator const *pMethodGen) const;
        void EmitMethodBody(MethodGenerator *pMethodGen, TempPtr<FunctionProfiler> &pFuncProf) const;

        IType const *m_pIndHolder1Delegate;
        IType const *m_pIndInfo;
        IType const *m_pIndDlgt;
        TypeGenerator const *m_pIndDlgtGen;
        IType const *m_pDelegate;
        IType const *m_pException;
        IType const *m_pString;
        IType const *m_pStringArr;
        IMethod const *m_pTarget;
        IMethod const *m_pLooseCrossDomainAccessor_TryGetIndHolder1Delegate;
        IMethod const *m_pLooseCrossDomainAccessor_SafelyCastIndDlgt;
        IMethod const *m_pIndInfo_set_AssemblyName;
        IMethod const *m_pIndInfo_set_Token;
        IMethod const *m_pIndInfo_set_Instantiation;
        IMethod const *m_pIndHolder1Delegate_TryGet;
        IMethod const *m_pIndDlgt_Invoke;
        IMethod const *m_pLooseCrossDomainAccessor_IsInstanceOfIdentity;
        IMethod const *m_pType_GetTypeFromHandle;
        IMethod const *m_pObject_ToString;
        vector<IType const *> m_instantiation;
    };

}   // namespace DelegatedMethodPreparationDetail {

#endif  // DELEGATEDMETHODPREPARATION_H

