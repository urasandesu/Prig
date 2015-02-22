/* 
 * File: PackageLog.cs
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



using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public abstract class PackageLog
    {
        protected string Source { get; private set; }
        protected IVsActivityLog ActivityLog { get; private set; }

        protected PackageLog(string source, IVsActivityLog activityLog)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (activityLog == null)
                throw new ArgumentNullException("activityLog");

            Source = source;
            ActivityLog = activityLog;
        }

        public virtual void Info(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            ErrorHandler.ThrowOnFailure(ActivityLog.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, Source, description));
        }

        public virtual void Warn(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            ErrorHandler.ThrowOnFailure(ActivityLog.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING, Source, description));
        }

        public virtual void Error(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            ErrorHandler.ThrowOnFailure(ActivityLog.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, Source, description));
        }
    }
}
