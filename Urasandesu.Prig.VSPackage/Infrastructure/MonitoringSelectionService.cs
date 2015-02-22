/* 
 * File: MonitoringSelectionService.cs
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
// NOTE: This class was made based on the following files that are released under Apache License 2.0: 
// * VsUtility.cs  at bf0c24d - NuGet - CodePlex
// https://nuget.codeplex.com/SourceControl/latest#src/VisualStudio/VsUtility.cs
// 


using EnvDTE;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public class MonitoringSelectionService
    {
        readonly IVsMonitorSelection m_monitorSelection;

        public MonitoringSelectionService(IVsMonitorSelection monitorSelection)
        {
            if (monitorSelection == null)
                throw new ArgumentNullException("monitorSelection");

            m_monitorSelection = monitorSelection;
        }

        public virtual Project GetCurrentProject()
        {
            using (var selection = new Selection(m_monitorSelection))
            {
                if (selection.Hierarchy == null)
                    return null;

                var project = default(object);
                ErrorHandler.ThrowOnFailure(selection.Hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out project));
                return project as Project;
            }
        }

        public virtual T GetSelectedItem<T>() where T : class
        {
            using (var selection = new Selection(m_monitorSelection))
            {
                if (selection.Hierarchy == null)
                    return null;

                var item = default(object);
                ErrorHandler.ThrowOnFailure(selection.Hierarchy.GetProperty(selection.ItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out item));
                var result = item as T;
                if (result == null)
                    throw new InvalidCastException(string.Format("Selected item(type: '{0}') can't cast to the type '{1}'.", Information.TypeName(item), typeof(T).Name));
                return result;
            }
        }

        class Selection : IDisposable
        {
            IntPtr m_ppHier = IntPtr.Zero;
            IntPtr m_ppSC = IntPtr.Zero;

            public IVsHierarchy Hierarchy { get; private set; }

            uint m_itemId;
            public uint ItemId { get { return m_itemId; } }

            public Selection(IVsMonitorSelection monitorSelection)
            {
                if (monitorSelection == null)
                    throw new ArgumentNullException("monitorSelection");

                var ppMIS = default(IVsMultiItemSelect);
                ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(out m_ppHier, out m_itemId, out ppMIS, out m_ppSC));

                if (m_ppHier == IntPtr.Zero)
                    return;

                if (m_itemId == (uint)VSConstants.VSITEMID.Selection)
                    return;

                Hierarchy = (IVsHierarchy)Marshal.GetTypedObjectForIUnknown(m_ppHier, typeof(IVsHierarchy));
            }

            bool m_disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (m_disposed)
                    return;

                if (disposing)
                {
                    // Dispose managed resources here.
                }

                // Dispose unmanaged resources here.
                if (m_ppHier != IntPtr.Zero)
                {
                    Marshal.Release(m_ppHier);
                    m_ppHier = IntPtr.Zero;
                }

                if (m_ppSC != IntPtr.Zero)
                {
                    Marshal.Release(m_ppSC);
                    m_ppSC = IntPtr.Zero;
                }

                m_disposed = true;
            }

            ~Selection()
            {
                Dispose(false);
            }
        }
    }
}
