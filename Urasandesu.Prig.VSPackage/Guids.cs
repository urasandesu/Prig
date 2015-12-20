/* 
 * File: Guids.cs
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



// Guids.cs
// MUST match guids.h
using System;

namespace Urasandesu.Prig.VSPackage
{
    static class GuidList
    {
        public const string PrigPackageString = "0a06101d-8de3-40c4-b083-c5c16ca227ae";
        public const string MainMenuString = "349577aa-b891-4271-bc7a-2121ccb68ee5";
        public const string MainMenuGroupString = "1ee497a1-b076-46b0-8808-64deb408c423";
        public const string RegistrationMenuGroupString = "b635358f-b28c-4dac-b31a-7dccddfe7b0a";
        public const string AddPrigAssemblyForMSCorLibGroupString = "b99f3bdd-fee9-42ac-a943-303620e0ce53";
        public const string AddPrigAssemblyGroupString = "7c0c2ffb-c3a5-45d0-9887-026566d68825";
        public const string EditPrigIndirectionSettingsGroupString = "15fa994b-e3c6-4f6a-acdb-f311d1ebfa65";


        public static readonly Guid MainMenu = new Guid(MainMenuString);
        public static readonly Guid MainMenuGroup = new Guid(MainMenuGroupString);
        public static readonly Guid RegistrationMenuGroup = new Guid(RegistrationMenuGroupString);
        public static readonly Guid AddPrigAssemblyForMSCorLibGroup = new Guid(AddPrigAssemblyForMSCorLibGroupString);
        public static readonly Guid AddPrigAssemblyGroup = new Guid(AddPrigAssemblyGroupString);
        public static readonly Guid EditPrigIndirectionSettingsGroup = new Guid(EditPrigIndirectionSettingsGroupString);
    };
}