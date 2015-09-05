/* 
 * File: IndirectionPreparation.cpp
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

#ifndef PRIGDATA_H
#include <PrigData.h>
#endif

#ifndef INDIRECTIONDELEGATES_H
#include <IndirectionDelegates.h>
#endif

#ifndef INDIRECTIONPREPARATION_H
#include <IndirectionPreparation.h>
#endif

namespace IndirectionPreparationDetail {

    IndirectionPreparation::~IndirectionPreparation() 
    { }



    void EmitIndirectParameters(MethodBodyGenerator *pNewBodyGen, IMethod const *pMethodGen, INT offset)
    {
        using namespace Urasandesu::CppAnonym::Collections;

        auto isStatic = pMethodGen->IsStatic();
        auto staticOffset = !isStatic ? 0 : 1;
        auto const &params = pMethodGen->GetParameters();
        
        auto paramIndexes = RapidVector<ULONG>();
        if (!isStatic)
            paramIndexes.push_back(0 + offset);
        BOOST_FOREACH (auto const &pParam, params)
            paramIndexes.push_back(pParam->GetPosition() - staticOffset + offset);
        
        BOOST_FOREACH (auto paramIndex, paramIndexes)
        {
            if (paramIndex == 0)
                pNewBodyGen->Emit(OpCodes::Ldarg_0);
            else if (paramIndex == 1)
                pNewBodyGen->Emit(OpCodes::Ldarg_1);
            else if (paramIndex == 2)
                pNewBodyGen->Emit(OpCodes::Ldarg_2);
            else if (paramIndex == 3)
                pNewBodyGen->Emit(OpCodes::Ldarg_3);
            else if (4 <= paramIndex && paramIndex <= 255)
                pNewBodyGen->Emit(OpCodes::Ldarg_S, static_cast<BYTE>(paramIndex));
            else
                pNewBodyGen->Emit(OpCodes::Ldarg, static_cast<SHORT>(paramIndex));
        }
    }



    IType const *MakeGenericExplicitItsInstance(IType const *pTarget)
    {
        using std::vector;

        auto const *pAsm = pTarget->GetAssembly();
        
        auto genericParamPos = 0ul;
        auto genericArgs = vector<IType const *>();
        BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
            genericArgs.push_back(pAsm->GetGenericTypeParameter(genericParamPos++));
        
        return pTarget->MakeGenericType(genericArgs);
    }



    IMethod const *MakeGenericExplicitItsInstance(IMethod const *pTarget)
    {
        using std::vector;

        auto const *pAsm = pTarget->GetAssembly();
        
        auto genericParamPos = 0ul;
        auto genericArgs = vector<IType const *>();
        BOOST_FOREACH (auto const &_, pTarget->GetGenericArguments())
            genericArgs.push_back(pAsm->GetGenericMethodParameter(genericParamPos++));
        
        return pTarget->MakeGenericMethod(genericArgs);
    }



    class ExplicitThis : 
        public IParameter
    {
    public:
        ExplicitThis() : 
            m_pParamType(nullptr)
        { }

        mdToken GetToken() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ULONG GetPosition() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        std::wstring const &GetName() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ParameterAttributes GetAttribute() const { return ParameterAttributes::PA_NONE; }
        IType const *GetParameterType() const { return m_pParamType; }
        Signature const &GetSignature() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IMethod const *GetMethod() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IProperty const *GetProperty() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        ParameterProvider const &GetMember() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IAssembly const *GetAssembly() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        IParameter const *GetSourceParameter() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        bool Equals(IParameter const *pParam) const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        size_t GetHashCode() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }
        void OutDebugInfo() const { BOOST_THROW_EXCEPTION(Urasandesu::CppAnonym::CppAnonymNotImplementedException()); }

        void Set(IType const *pParamType)
        {
            _ASSERTE(!m_pParamType);
            _ASSERTE(pParamType);
            m_pParamType = pParamType;
        }

    private:
        IType const *m_pParamType;
    };



    class IndirectionDelegateFinder
    {
    public:
        IndirectionDelegateFinder(IMethod const *pTarget) : 
            m_pTarget(pTarget)
        { }

        bool operator ()(IType const *pType) const
        {
            using Urasandesu::Prig::IMethodSigEqualTo;
            using std::vector;

            auto pType_Invoke = pType->GetMethod(L"Invoke");
            _ASSERTE(pType_Invoke);
            auto params = vector<IParameter const *>();
            auto explicitThis = ExplicitThis();
            if (m_pTarget->IsStatic())
            {
                params = m_pTarget->GetParameters();
            }
            else
            {
                auto const *pDeclaringType = m_pTarget->GetDeclaringType();
                if (pDeclaringType->IsValueType())
                    pDeclaringType = pDeclaringType->MakeByRefType();
                explicitThis.Set(pDeclaringType);
                params.push_back(&explicitThis);
                params.insert(params.end(), m_pTarget->GetParameters().begin(), m_pTarget->GetParameters().end());
            }

            return IMethodSigEqualTo::ReturnTypeEqual(pType_Invoke->GetReturnType(), m_pTarget->GetReturnType()) && 
                    IMethodSigEqualTo::ParametersEqual(pType_Invoke->GetParameters(), params);
        }

    private:
        IMethod const *m_pTarget;
    };

}   // namespace IndirectionPreparationDetail {
