/* 
 * File: stdafx.h
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


// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but are changed infrequently

#pragma once

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>


#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS	// some CString constructors will be explicit

#include <atlbase.h>
#include <atlstr.h>

#ifndef URASANDESU_SWATHE_SWATHEDEPENDSON_H
#include <Urasandesu/Swathe/SwatheDependsOn.h>
#endif

#include <boost/assign/std/list.hpp>
#include <boost/assign/std/vector.hpp>
#include <boost/detail/atomic_count.hpp>
#include <boost/lambda/bind.hpp>
#include <boost/lambda/lambda.hpp>
#include <boost/range/algorithm_ext.hpp>

#ifndef CPPANONYMTEST_H
#include "CppAnonymTest.h"
#endif

#ifndef ATOMICCOUNTER_H
#include "AtomicCounter.h"
#endif

#ifndef BASICCOUNTER_H
#include "BasicCounter.h"
#endif

#ifndef SURVIVALCOUNTER_H
#include "SurvivalCounter.h"
#endif

#ifndef COUNTERWITHVALUE1_H
#include "CounterWithValue1.h"
#endif

#ifndef ACTIONCOUNER_H
#include "ActionCouner.h"
#endif

#ifndef COUNTERWITHACTION1_H
#include "CounterWithAction1.h"
#endif
