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
