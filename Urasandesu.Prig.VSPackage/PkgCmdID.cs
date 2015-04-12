/* 
 * File: PkgCmdID.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
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



// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace Urasandesu.Prig.VSPackage
{
    static class PkgCmdIDList
    {
        public const uint MainMenu = 0x1001;

        public const uint MainMenuGroup = 0x1101;
        public const uint EnableTestAdapterCommand = 0x1102;
        public const uint DisableTestAdapterCommand = 0x1103;

        public const uint AddPrigAssemblyForMSCorLibGroup = 0x1011;
        public const uint AddPrigAssemblyForMSCorLibCommand = 0x1012;

        public const uint AddPrigAssemblyGroup = 0x1021;
        public const uint AddPrigAssemblyCommand = 0x1022;

        public const uint EditPrigIndirectionSettingsGroup = 0x1031;
        public const uint EditPrigIndirectionSettingsCommand = 0x1032;
        public const uint RemovePrigAssemblyCommand = 0x1033;

    };
}