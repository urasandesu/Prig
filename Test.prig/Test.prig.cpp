/* 
 * File: Test.prig.cpp
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


// Test.prig.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#ifndef URASANDESU_SWATHE_H
#include <Urasandesu/Swathe.h>
#endif

// Test.prig.exe --gtest_filter=prig_Test.*
namespace {

    CPPANONYM_TEST(prig_Test, Test_01)
    {
        using Urasandesu::CppAnonym::Utilities::AutoPtr;

        using namespace Urasandesu::Swathe;
        using namespace Urasandesu::Swathe::Hosting;
        using namespace Urasandesu::Swathe::Metadata;
        using namespace Urasandesu::Swathe::StrongNaming;
        using boost::assign::operator +=;
        using boost::get;
        using std::vector;
        using std::wstring;
        using Urasandesu::CppAnonym::Utilities::Empty;

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
        auto *pMetaDisp = pMetaInfo->CreateDispenser();

        auto const *pMSCorLib = pMetaDisp->GetAssembly(L"mscorlib, Version=2.0.0.0, Culture=neutral, " 
                                                       L"PublicKeyToken=b77a5c561934e089, processorArchitecture=x86");

        auto const *pPrigFramework = pMetaDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, "
                                                            L"PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=x86");

        auto const *pMSCorLibDll = pMSCorLib->GetModule(L"CommonLanguageRuntimeLibrary");
        auto const *pPrigFrameworkDll = pPrigFramework->GetModule(L"Urasandesu.Prig.Framework.dll");

        auto const *pVoid = pMSCorLibDll->GetType(L"System.Void");
        auto const *pDateTime = pMSCorLibDll->GetType(L"System.DateTime");
        auto const *pUInt32 = pMSCorLibDll->GetType(L"System.UInt32");
        auto const *pString = pMSCorLibDll->GetType(L"System.String");
        auto const *pType = pMSCorLibDll->GetType(L"System.Type");
        auto const *pRuntimeTypeHandle = pMSCorLibDll->GetType(L"System.RuntimeTypeHandle");
        auto const *pAssembly = pMSCorLibDll->GetType(L"System.Reflection.Assembly");
        auto const *pIndirectableAttrType = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
        auto const *pIndirectionInfo = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndirectionHolder1 = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pIndirectionFunc1 = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionFunc`1");
        auto const *pLooseCrossDomainAccessor = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");

        auto const *pIndirectionFunc1DateTime = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pDateTime);
            pIndirectionFunc1DateTime = pIndirectionFunc1->MakeGenericType(genericArgs);
        }
        auto const *pIndirectionHolder1IndirectionFunc1DateTime = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndirectionFunc1DateTime);
            pIndirectionHolder1IndirectionFunc1DateTime = pIndirectionHolder1->MakeGenericType(genericArgs);
        }

        auto const *pIndirectionInfo_set_AssemblyName = pIndirectionInfo->GetMethod(L"set_AssemblyName");
        auto const *pIndirectionInfo_set_Token = pIndirectionInfo->GetMethod(L"set_Token");
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        auto const *pLooseCrossDomainAccessor_GetOrRegister = pLooseCrossDomainAccessor->GetMethod(L"GetOrRegister");
        auto const *pIndirectionHolder1IndirectionFunc1DateTime_TryRemove = pIndirectionHolder1IndirectionFunc1DateTime->GetMethod(L"TryRemove");
        auto const *pIndirectionHolder1IndirectionFunc1DateTime_AddOrUpdate = pIndirectionHolder1IndirectionFunc1DateTime->GetMethod(L"AddOrUpdate");
        auto const *pIndirectableAttr_ctor = static_cast<IMethod *>(nullptr);
        {
            auto paramTypes = vector<IType const *>();
            paramTypes.push_back(pUInt32);
            pIndirectableAttr_ctor = pIndirectableAttrType->GetConstructor(paramTypes);
        }

        auto const *pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndirectionHolder1IndirectionFunc1DateTime);
            pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime = pLooseCrossDomainAccessor_TryGet->MakeGenericMethod(genericArgs);
        }

        auto const *pLooseCrossDomainAccessor_GetOrRegisterIndirectionHolder1IndirectionFunc1DateTime = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndirectionHolder1IndirectionFunc1DateTime);
            pLooseCrossDomainAccessor_GetOrRegisterIndirectionHolder1IndirectionFunc1DateTime = pLooseCrossDomainAccessor_GetOrRegister->MakeGenericMethod(genericArgs);
        }




        auto const *pSnInfo = pRuntime->GetInfo<StrongNameInfo>();
        auto pSnKey = pSnInfo->NewStrongNameKey(L"..\\..\\..\\mscorlib.Prig.snk");

        auto *pMSCorLibPrigGen = pMetaDisp->DefineAssemblyWithPartialName(L"mscorlib.Prig");
        pMSCorLibPrigGen->SetStrongNameKey(pSnKey);

        auto *pIndirectableAttrGen = static_cast<ICustomAttribute *>(nullptr);
        {
            auto constructorArgs = vector<CustomAttributeArgument>();
            constructorArgs.push_back(0x060002D2u);
            pIndirectableAttrGen = pMSCorLibPrigGen->DefineCustomAttribute(pIndirectableAttr_ctor, constructorArgs);
        }

        auto *pMSCorLibPrigDllGen = pMSCorLibPrigGen->DefineModule(L"mscorlib.Prig.dll");

        auto *pPDateTimeGen = pMSCorLibPrigDllGen->DefineType(L"System.Prig.PDateTime",
                                                              TypeAttributes::TA_PUBLIC |
                                                              TypeAttributes::TA_ABSTRACT |
                                                              TypeAttributes::TA_ANSI_CLASS |
                                                              TypeAttributes::TA_SEALED |
                                                              TypeAttributes::TA_BEFORE_FIELD_INIT);

        auto *pNowGetGen = pPDateTimeGen->DefineNestedType(L"NowGet",
                                                           TypeAttributes::TA_ABSTRACT |
                                                           TypeAttributes::TA_ANSI_CLASS |
                                                           TypeAttributes::TA_SEALED |
                                                           TypeAttributes::TA_NESTED_PUBLIC |
                                                           TypeAttributes::TA_BEFORE_FIELD_INIT);

        auto set_BodyParams = vector<IType const *>();
        set_BodyParams.push_back(pIndirectionFunc1DateTime);
        auto *pNowGetGen_set_Body = pNowGetGen->DefineMethod(L"set_Body",
                                                             MethodAttributes::MA_PUBLIC |
                                                             MethodAttributes::MA_HIDE_BY_SIG |
                                                             MethodAttributes::MA_SPECIAL_NAME |
                                                             MethodAttributes::MA_STATIC,
                                                             CallingConventions::CC_STANDARD,
                                                             pVoid,
                                                             set_BodyParams);

        auto *pNowGetGen_Body = pNowGetGen->DefineProperty(L"Body",
                                                           PropertyAttributes::PA_NONE,
                                                           pIndirectionFunc1DateTime,
                                                           vector<IType const *>());
        pNowGetGen_Body->SetSetMethod(pNowGetGen_set_Body);

        {
            auto *pBodyGen = pNowGetGen_set_Body->DefineMethodBody();

            auto *pLocal0_info = pBodyGen->DefineLocal(pIndirectionInfo);
            auto *pLocal1_holder = pBodyGen->DefineLocal(pIndirectionHolder1IndirectionFunc1DateTime);
            auto *pLocal2_method = pBodyGen->DefineLocal(pIndirectionFunc1DateTime);
            auto *pLocal3_V_3 = pBodyGen->DefineLocal(pIndirectionHolder1IndirectionFunc1DateTime);

            auto label0 = pBodyGen->DefineLabel();
            auto label1 = pBodyGen->DefineLabel();

            pBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_info);
            pBodyGen->Emit(OpCodes::Initobj, pIndirectionInfo);
            pBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_info);
            pBodyGen->Emit(OpCodes::Ldstr, L"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            pBodyGen->Emit(OpCodes::Call, pIndirectionInfo_set_AssemblyName);
            pBodyGen->Emit(OpCodes::Ldloca_S, pLocal0_info);
            pBodyGen->Emit(OpCodes::Ldc_I4, 0x60002d2);
            pBodyGen->Emit(OpCodes::Call, pIndirectionInfo_set_Token);
            pBodyGen->Emit(OpCodes::Ldarg_0);
            pBodyGen->Emit(OpCodes::Brtrue_S, label0);

            pBodyGen->Emit(OpCodes::Ldnull);
            pBodyGen->Emit(OpCodes::Stloc_1);
            pBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_holder);
            pBodyGen->Emit(OpCodes::Call, pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime);
            pBodyGen->Emit(OpCodes::Brfalse_S, label1);

            pBodyGen->Emit(OpCodes::Ldnull);
            pBodyGen->Emit(OpCodes::Stloc_2);
            pBodyGen->Emit(OpCodes::Ldloc_1);
            pBodyGen->Emit(OpCodes::Ldloc_0);
            pBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_method);
            pBodyGen->Emit(OpCodes::Callvirt, pIndirectionHolder1IndirectionFunc1DateTime_TryRemove);
            pBodyGen->Emit(OpCodes::Pop);
            pBodyGen->Emit(OpCodes::Ret);

            pBodyGen->MarkLabel(label0);
            pBodyGen->Emit(OpCodes::Call, pLooseCrossDomainAccessor_GetOrRegisterIndirectionHolder1IndirectionFunc1DateTime);
            pBodyGen->Emit(OpCodes::Stloc_3);
            pBodyGen->Emit(OpCodes::Ldloc_3);
            pBodyGen->Emit(OpCodes::Ldloc_0);
            pBodyGen->Emit(OpCodes::Ldarg_0);
            pBodyGen->Emit(OpCodes::Callvirt, pIndirectionHolder1IndirectionFunc1DateTime_AddOrUpdate);
            pBodyGen->Emit(OpCodes::Pop);
            pBodyGen->MarkLabel(label1);
            pBodyGen->Emit(OpCodes::Ret);
        }

        pMSCorLibPrigGen->Save(PortableExecutableKinds::PEK_IL_ONLY, ImageFileMachine::IFM_I386);
    }



    CPPANONYM_TEST(prig_Test, Test_02)
    {
        using Urasandesu::CppAnonym::Utilities::AutoPtr;

        using namespace Urasandesu::Swathe;
        using namespace Urasandesu::Swathe::Hosting;
        using namespace Urasandesu::Swathe::Metadata;
        using namespace Urasandesu::Swathe::StrongNaming;
        using boost::assign::operator +=;
        using boost::get;
        using std::vector;
        using std::wstring;
        using Urasandesu::CppAnonym::Utilities::Empty;

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
        auto *pMetaDisp = pMetaInfo->CreateDispenser();

        auto const *pMSCorLib = pMetaDisp->GetAssembly(L"mscorlib, Version=2.0.0.0, Culture=neutral, " 
                                                       L"PublicKeyToken=b77a5c561934e089, processorArchitecture=x86");

        auto const *pPrigFramework = pMetaDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, "
                                                            L"PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=x86");

        auto const *pMSCorLibDll = pMSCorLib->GetModule(L"CommonLanguageRuntimeLibrary");
        auto const *pPrigFrameworkDll = pPrigFramework->GetModule(L"Urasandesu.Prig.Framework.dll");

        auto const *pDateTime = pMSCorLibDll->GetType(L"System.DateTime");
        auto const *pIndirectionInfo = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionInfo");
        auto const *pIndirectionHolder1 = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionHolder`1");
        auto const *pIndirectionFunc1 = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.IndirectionFunc`1");
        auto const *pLooseCrossDomainAccessor = pPrigFrameworkDll->GetType(L"Urasandesu.Prig.Framework.LooseCrossDomainAccessor");

        auto const *pIndirectionFunc1DateTime = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pDateTime);
            pIndirectionFunc1DateTime = pIndirectionFunc1->MakeGenericType(genericArgs);
        }
        auto const *pIndirectionHolder1IndirectionFunc1DateTime = static_cast<IType *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndirectionFunc1DateTime);
            pIndirectionHolder1IndirectionFunc1DateTime = pIndirectionHolder1->MakeGenericType(genericArgs);
        }

        auto const *pDateTime_get_UtcNow = pDateTime->GetMethod(L"get_UtcNow");
        auto const *pDateTime_ToLocalTime = pDateTime->GetMethod(L"ToLocalTime");
        auto const *pIndirectionInfo_set_AssemblyName = pIndirectionInfo->GetMethod(L"set_AssemblyName");
        auto const *pIndirectionInfo_set_Token = pIndirectionInfo->GetMethod(L"set_Token");
        auto const *pIndirectionFunc1DateTime_Invoke = pIndirectionFunc1DateTime->GetMethod(L"Invoke");
        auto const *pLooseCrossDomainAccessor_TryGet = pLooseCrossDomainAccessor->GetMethod(L"TryGet");
        auto const *pIndirectionHolder1IndirectionFunc1DateTime_TryGet = pIndirectionHolder1IndirectionFunc1DateTime->GetMethod(L"TryGet");

        auto const *pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime = static_cast<IMethod *>(nullptr);
        {
            auto genericArgs = vector<IType const *>();
            genericArgs.push_back(pIndirectionHolder1IndirectionFunc1DateTime);
            pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime = pLooseCrossDomainAccessor_TryGet->MakeGenericMethod(genericArgs);
        }




        auto const *pSnInfo = pRuntime->GetInfo<StrongNameInfo>();
        auto pSnKey = pSnInfo->NewStrongNameKey(L"..\\..\\..\\mscorlib.Prig.snk");

        auto *pMSCorLibIndGen = pMetaDisp->DefineAssemblyWithPartialName(L"mscorlib.Indirect");
        pMSCorLibIndGen->SetStrongNameKey(pSnKey);

        auto *pMSCorLibIndDllGen = pMSCorLibIndGen->DefineModule(L"mscorlib.Indirect.dll");

        auto *pIDateTimeGen = pMSCorLibIndDllGen->DefineType(L"System.Indirect.IDateTime",
                                                             TypeAttributes::TA_PUBLIC |
                                                             TypeAttributes::TA_ABSTRACT |
                                                             TypeAttributes::TA_ANSI_CLASS |
                                                             TypeAttributes::TA_SEALED |
                                                             TypeAttributes::TA_BEFORE_FIELD_INIT);

        auto *pIDateTimeGen_get_Now = pIDateTimeGen->DefineMethod(L"get_Now",
                                                                  MethodAttributes::MA_PUBLIC |
                                                                  MethodAttributes::MA_HIDE_BY_SIG |
                                                                  MethodAttributes::MA_SPECIAL_NAME |
                                                                  MethodAttributes::MA_STATIC,
                                                                  CallingConventions::CC_STANDARD,
                                                                  pDateTime,
                                                                  MetadataSpecialValues::EMPTY_TYPES);

        {
            auto *pNewBodyGen = pIDateTimeGen_get_Now->DefineMethodBody();

            auto *pLocal0 = pNewBodyGen->DefineLocal(pDateTime);
            auto *pLocal1_holder = pNewBodyGen->DefineLocal(pIndirectionHolder1IndirectionFunc1DateTime);
            auto *pLocal2_info = pNewBodyGen->DefineLocal(pIndirectionInfo);
            auto *pLocal3_get_Now = pNewBodyGen->DefineLocal(pIndirectionFunc1DateTime);

            auto label0 = pNewBodyGen->DefineLabel();

            pNewBodyGen->Emit(OpCodes::Ldnull);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal1_holder);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal1_holder);
            pNewBodyGen->Emit(OpCodes::Call, pLooseCrossDomainAccessor_TryGetIndirectionHolder1IndirectionFunc1DateTime);
            pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_info);
            pNewBodyGen->Emit(OpCodes::Initobj, pIndirectionInfo);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_info);
            pNewBodyGen->Emit(OpCodes::Ldstr, L"mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            pNewBodyGen->Emit(OpCodes::Call, pIndirectionInfo_set_AssemblyName);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal2_info);
            pNewBodyGen->Emit(OpCodes::Ldc_I4, 0x60002d2);
            pNewBodyGen->Emit(OpCodes::Call, pIndirectionInfo_set_Token);
            pNewBodyGen->Emit(OpCodes::Ldnull);
            pNewBodyGen->Emit(OpCodes::Stloc_S, pLocal3_get_Now);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal1_holder);
            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal2_info);
            pNewBodyGen->Emit(OpCodes::Ldloca_S, pLocal3_get_Now);
            pNewBodyGen->Emit(OpCodes::Callvirt, pIndirectionHolder1IndirectionFunc1DateTime_TryGet);
            pNewBodyGen->Emit(OpCodes::Brfalse_S, label0);

            pNewBodyGen->Emit(OpCodes::Ldloc_S, pLocal3_get_Now);
            pNewBodyGen->Emit(OpCodes::Callvirt, pIndirectionFunc1DateTime_Invoke);
            pNewBodyGen->Emit(OpCodes::Ret);

            pNewBodyGen->MarkLabel(label0);
            pNewBodyGen->Emit(OpCodes::Call, pDateTime_get_UtcNow);
            pNewBodyGen->Emit(OpCodes::Stloc_0);
            pNewBodyGen->Emit(OpCodes::Ldloca, static_cast<SHORT>(0x00));
            pNewBodyGen->Emit(OpCodes::Call, pDateTime_ToLocalTime);
            pNewBodyGen->Emit(OpCodes::Ret);
        }

        pMSCorLibIndGen->Save(PortableExecutableKinds::PEK_IL_ONLY, ImageFileMachine::IFM_I386);
    }



    CPPANONYM_TEST(prig_Test, Test_03)
    {
        using Urasandesu::CppAnonym::Utilities::AutoPtr;

        using namespace Urasandesu::Swathe;
        using namespace Urasandesu::Swathe::Hosting;
        using namespace Urasandesu::Swathe::Metadata;
        using namespace Urasandesu::Swathe::StrongNaming;
        using boost::assign::operator +=;
        using boost::get;
        using std::vector;
        using std::wstring;
        using Urasandesu::CppAnonym::Utilities::Empty;

        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
        auto *pMetaDisp = pMetaInfo->CreateDispenser();

        auto const *pMSCorLib = pMetaDisp->GetAssembly(L"mscorlib, Version=2.0.0.0, Culture=neutral, " 
                                                       L"PublicKeyToken=b77a5c561934e089, processorArchitecture=x86");

        auto const *pMSCorLibDll = pMSCorLib->GetModule(L"CommonLanguageRuntimeLibrary");


        auto const *pVoid = pMSCorLibDll->GetType(L"System.Void");
        auto const *pInt32 = pMSCorLibDll->GetType(L"System.Int32");
        auto const *pString = pMSCorLibDll->GetType(L"System.String");
        auto const *pStreamReader = pMSCorLibDll->GetType(L"System.IO.StreamReader");
        auto const *pChar = pMSCorLibDll->GetType(L"System.Char");
        auto const *pIOException = pMSCorLibDll->GetType(L"System.IO.IOException");
        auto const *pTextReader = pMSCorLibDll->GetType(L"System.IO.TextReader");
        auto const *pException = pMSCorLibDll->GetType(L"System.Exception");
        auto const *pConsole = pMSCorLibDll->GetType(L"System.Console");
        auto const *pObject = pMSCorLibDll->GetType(L"System.Object");

        auto const *pStreamReader_ctor_string = static_cast<IMethod *>(nullptr);
        {
            auto params = vector<IType const *>();
            params.push_back(pString);
            pStreamReader_ctor_string = pStreamReader->GetConstructor(params);
        }
        auto const *pTextReader_ReadBlock_CharArr_Int32_Int32 = static_cast<IMethod *>(nullptr);
        {
            auto params = vector<IType const *>();
            params.push_back(pChar->MakeArrayType());
            params.push_back(pInt32);
            params.push_back(pInt32);
            pTextReader_ReadBlock_CharArr_Int32_Int32 = pTextReader->GetMethod(L"ReadBlock", params);
        }
        auto const *pTextReader_Close = pTextReader->GetMethod(L"Close");
        auto const *pException_get_Message = pException->GetMethod(L"get_Message");
        auto const *pConsole_WriteLine_string_object_object = static_cast<IMethod *>(nullptr);
        {
            auto params = vector<IType const *>();
            params.push_back(pString);
            params.push_back(pObject);
            params.push_back(pObject);
            pConsole_WriteLine_string_object_object = pConsole->GetMethod(L"WriteLine", params);
        }



        auto const *pSnInfo = pRuntime->GetInfo<StrongNameInfo>();
        auto pSnKey = pSnInfo->NewStrongNameKey(L"..\\..\\..\\mscorlib.Prig.snk");

        auto *pMSCorLibEHGen = pMetaDisp->DefineAssemblyWithPartialName(L"mscorlib.EH");
        pMSCorLibEHGen->SetStrongNameKey(pSnKey);

        auto *pMSCorLibEHDllGen = pMSCorLibEHGen->DefineModule(L"mscorlib.EH.dll");

        auto *pEHClassGen = pMSCorLibEHDllGen->DefineType(L"mscorlib.Prig.EHClass",
                                                          TypeAttributes::TA_PUBLIC |
                                                          TypeAttributes::TA_AUTO_CLASS |
                                                          TypeAttributes::TA_ANSI_CLASS |
                                                          TypeAttributes::TA_BEFORE_FIELD_INIT);

        auto *pEHClassGen_ReadFile = static_cast<MethodGenerator *>(nullptr);
        {
            auto params = vector<IType const *>();
            params.push_back(pString);
            params.push_back(pInt32);
            pEHClassGen_ReadFile = pEHClassGen->DefineMethod(L"ReadFile",
                                                             MethodAttributes::MA_PUBLIC | 
                                                             MethodAttributes::MA_HIDE_BY_SIG, 
                                                             CallingConventions::CC_HAS_THIS, 
                                                             pVoid,
                                                             params);
        }

        {
            auto *pBodyGen = pEHClassGen_ReadFile->DefineMethodBody();

            auto *pLocal0_file = pBodyGen->DefineLocal(pStreamReader);
            auto *pLocal1_buffer = pBodyGen->DefineLocal(pChar->MakeArrayType());
            auto *pLocal2_e = pBodyGen->DefineLocal(pIOException);

            auto label0 = pBodyGen->DefineLabel();

            pBodyGen->Emit(OpCodes::Ldnull);
            pBodyGen->Emit(OpCodes::Stloc_0);
            pBodyGen->Emit(OpCodes::Ldc_I4_S, static_cast<BYTE>(10));
            pBodyGen->Emit(OpCodes::Newarr, pChar);
            pBodyGen->Emit(OpCodes::Stloc_1);

            pBodyGen->BeginExceptionBlock();
            {
                pBodyGen->Emit(OpCodes::Ldarg_1);
                pBodyGen->Emit(OpCodes::Newobj, pStreamReader_ctor_string);
                pBodyGen->Emit(OpCodes::Stloc_0);
                pBodyGen->Emit(OpCodes::Ldloc_0);
                pBodyGen->Emit(OpCodes::Ldloc_1);
                pBodyGen->Emit(OpCodes::Ldarg_2);
                pBodyGen->Emit(OpCodes::Ldloc_1);
                pBodyGen->Emit(OpCodes::Ldlen);
                pBodyGen->Emit(OpCodes::Conv_I4);
                pBodyGen->Emit(OpCodes::Callvirt, pTextReader_ReadBlock_CharArr_Int32_Int32);
                pBodyGen->Emit(OpCodes::Pop);
            }
            pBodyGen->BeginCatchBlock(pIOException);
            {
                pBodyGen->Emit(OpCodes::Stloc_2);
                pBodyGen->Emit(OpCodes::Ldstr, L"Error reading from {0}. Message = {1}");
                pBodyGen->Emit(OpCodes::Ldarg_1);
                pBodyGen->Emit(OpCodes::Ldloc_2);
                pBodyGen->Emit(OpCodes::Callvirt, pException_get_Message);
                pBodyGen->Emit(OpCodes::Call, pConsole_WriteLine_string_object_object);
            }
            pBodyGen->BeginFinallyBlock();
            {
                pBodyGen->Emit(OpCodes::Ldloc_0);
                pBodyGen->Emit(OpCodes::Brfalse_S, label0);
                pBodyGen->Emit(OpCodes::Ldloc_0);
                pBodyGen->Emit(OpCodes::Callvirt, pTextReader_Close);
                pBodyGen->MarkLabel(label0);
            }
            pBodyGen->EndExceptionBlock();

            pBodyGen->Emit(OpCodes::Ret);
        }

        pMSCorLibEHGen->Save(PortableExecutableKinds::PEK_IL_ONLY, ImageFileMachine::IFM_I386);
    }
    
    
    
    CPPANONYM_TEST(prig_Test, GetCustomAttributesTest_01)
    {
        using namespace Urasandesu::Swathe::Hosting;
        using namespace Urasandesu::Swathe::Metadata;

        using boost::begin;
        using boost::distance;
        using boost::get;
        using boost::filesystem::canonical;
        using boost::filesystem::current_path;
        using boost::filesystem::path;
        
        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
        auto *pMetaDisp = pMetaInfo->CreateDispenser();

        auto const *pPrigFrmwrk = pMetaDisp->GetAssembly(L"Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce");
        auto const *pPrigFrmwrkDll = pPrigFrmwrk->GetMainModule();
        auto const *pIndAttrInfo = pPrigFrmwrkDll->GetType(L"Urasandesu.Prig.Framework.IndirectableAttribute");
        
        auto currentDir = current_path().native();
        boost::replace_all(currentDir, L"Test.prig", L"mscorlib.Prig");
        auto mscorlibPrigPath = path(currentDir);
        mscorlibPrigPath /= L"mscorlib.Prig.dll";
        auto const *pMSCorLibPrig = pMetaDisp->GetAssemblyFrom(mscorlibPrigPath);
        auto indAttrs = pMSCorLibPrig->GetCustomAttributes(pIndAttrInfo);
        ASSERT_EQ(10, distance(indAttrs));
        auto const *pIndAttr = *begin(indAttrs);
        auto const &args = pIndAttr->GetConstructorArguments();
        ASSERT_EQ(1, args.size());
        auto mdmdTarget = get<UINT>(args[0]);
        ASSERT_EQ(0x0600023B, mdmdTarget);
    }
    
    
    
    CPPANONYM_TEST(prig_Test, GetExceptionClausesTest_01)
    {
        using namespace Urasandesu::Swathe::Hosting;
        using namespace Urasandesu::Swathe::Metadata;

        using boost::filesystem::canonical;
        using boost::filesystem::current_path;
        using boost::filesystem::path;
        using std::wstring;
        using Urasandesu::CppAnonym::Utilities::Empty;
        
        auto const *pHost = HostInfo::CreateHost();
        auto const *pRuntime = pHost->GetRuntime(L"v2.0.50727");
        auto const *pMetaInfo = pRuntime->GetInfo<MetadataInfo>();
        auto *pMetaDisp = pMetaInfo->CreateDispenser();
        
        auto currentDir = current_path().native();
        boost::replace_all(currentDir, L"Test.prig", L"mscorlib.Prig");
        auto mscorlibPrigPath = path(currentDir);
        mscorlibPrigPath /= L"mscorlib.Prig.dll";
        auto const *pMSCorLibPrig = pMetaDisp->GetAssemblyFrom(mscorlibPrigPath);
        auto const *pMSCorLibPrigDll = pMSCorLibPrig->GetMainModule();
        auto const *pEHClass = pMSCorLibPrigDll->GetType(L"mscorlib.Prig.EHClass");
        auto const *pEHClass_ReadFile = pEHClass->GetMethod(L"ReadFile");
        {
            auto const *pBody = pEHClass_ReadFile->GetMethodBody();
            auto insts = pBody->GetInstructions();
            ASSERT_EQ(46, insts.size());

            auto exClauses = pBody->GetExceptionClauses();
            ASSERT_EQ(2, exClauses.size());
            {
                auto i = 0ul;
                BOOST_FOREACH (auto const &exClause, exClauses)
                {
                    auto clauseKind = exClause.GetClauseKind();
                    auto tryStart = exClause.GetTryStart();
                    auto tryEnd = exClause.GetTryEnd();
                    auto clauseStart = exClause.GetClauseStart();
                    auto clauseEnd = exClause.GetClauseEnd();
                    
                    auto const &tryStartOpcode = (*tryStart)->GetOpCode();
                    auto const &tryStartOperand = (*tryStart)->GetOperand();
                    
                    auto const &tryEndOpcode = (*tryEnd)->GetOpCode();
                    auto const &tryEndOperand = (*tryEnd)->GetOperand();
                    
                    auto const &clauseStartOpcode = (*clauseStart)->GetOpCode();
                    auto const &clauseStartOperand = (*clauseStart)->GetOperand();
                    
                    auto const &clauseEndOpcode = (*clauseEnd)->GetOpCode();
                    auto const &clauseEndOperand = (*clauseEnd)->GetOperand();

                    switch (i++)
                    {
                        case 0:
                            {
                                ASSERT_TRUE(clauseKind == ClauseKinds::CK_CATCH);

                                ASSERT_EQ(&OpCodes::Nop, &tryStartOpcode);
                                ASSERT_TRUE(Empty(tryStartOperand));

                                ASSERT_EQ(&OpCodes::Stloc_2, &tryEndOpcode);
                                ASSERT_TRUE(Empty(tryEndOperand));

                                ASSERT_EQ(&OpCodes::Stloc_2, &clauseStartOpcode);
                                ASSERT_TRUE(Empty(clauseStartOperand));

                                ASSERT_EQ(&OpCodes::Nop, &clauseEndOpcode);
                                ASSERT_TRUE(Empty(clauseEndOperand));

                                ASSERT_EQ(wstring(L"System.IO.IOException"), exClause.GetExceptionType()->GetFullName());
                            }
                            break;
                        case 1:
                            {
                                ASSERT_TRUE(clauseKind == ClauseKinds::CK_FINALLY);

                                ASSERT_EQ(&OpCodes::Nop, &tryStartOpcode);
                                ASSERT_TRUE(Empty(tryStartOperand));

                                ASSERT_EQ(&OpCodes::Nop, &tryEndOpcode);
                                ASSERT_TRUE(Empty(tryEndOperand));

                                ASSERT_EQ(&OpCodes::Nop, &clauseStartOpcode);
                                ASSERT_TRUE(Empty(clauseStartOperand));

                                ASSERT_EQ(&OpCodes::Nop, &clauseEndOpcode);
                                ASSERT_TRUE(Empty(clauseEndOperand));
                            }
                            break;
                        default:
                            FAIL() << "We shouldn't get here!!";
                            break;
                    }
                }
            }
        }
    }
}

