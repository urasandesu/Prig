/* 
 * File: InstanceGettersTest.cpp
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

#ifndef INSTANCEGETTERS_H
#include <InstanceGetters.h>
#endif

namespace {

    void Hoge()
    {
    }

    // Test.Urasandesu.Prig.exe --gtest_filter=InstanceGettersTestSuite.*
    class InstanceGettersTestSuite : public testing::Test
    {
    protected:
        virtual void SetUp() 
        {
            InstanceGettersClear();
        }
        
        virtual void TearDown()
        {
            InstanceGettersClear();
        }
    };

    TEST_F(InstanceGettersTestSuite, InstanceGettersTestRecordExists01)
    {
        using namespace std;
        using namespace boost;

        typedef void (*HogePtr)();

        LPCWSTR key = L"Urasandesu.Prig.Framework.IndirectionHolder`1<System.Func`1<System.DateTime>>, Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6724069a628e8cb0";

        {
            HogePtr pfnHoge = Hoge;
            void (*pfnHoge2)() = NULL;
            ASSERT_TRUE(InstanceGettersTryAdd(key, pfnHoge) == TRUE);
        }

        {
            void const *pFuncPtr = NULL;
            ASSERT_TRUE(InstanceGettersTryGet(key, &pFuncPtr) == TRUE);
            ASSERT_EQ(static_cast<HogePtr>(Hoge), pFuncPtr);
        }
    }


    TEST_F(InstanceGettersTestSuite, InstanceGettersTestRecordNotExists01)
    {
        using namespace std;
        using namespace boost;

        typedef void (*HogePtr)();

        {
            LPCWSTR key = L"Urasandesu.Prig.Framework.IndirectionHolder`1<System.Func`1<System.DateTime>>, Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6724069a628e8cb0";
            HogePtr pfnHoge = Hoge;
            ASSERT_TRUE(InstanceGettersTryAdd(key, pfnHoge) == TRUE);
        }

        {
            LPCWSTR key = L"Urasandesu.Prig.Framework.IndirectionHolder`1<System.Func`1<System.String>>, Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6724069a628e8cb0";
            void const *pFuncPtr = NULL;
            ASSERT_TRUE(InstanceGettersTryGet(key, &pFuncPtr) == FALSE);
            ASSERT_EQ(static_cast<HogePtr>(NULL), pFuncPtr);
        }
    }
}
