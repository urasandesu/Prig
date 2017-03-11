/* 
 * File: PackageViewModel.cs
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

namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public abstract class PackageViewModel : ApplicationViewModel
    {
        public void SetWaitCursor()
        {
            IsWaitCursorEnabled.Value = true;
        }

        ApplicationProperty<bool> m_isWaitCursorEnabled;
        public ApplicationProperty<bool> IsWaitCursorEnabled
        {
            get
            {
                if (m_isWaitCursorEnabled == null)
                    m_isWaitCursorEnabled = new ApplicationProperty<bool>();
                return m_isWaitCursorEnabled;
            }
        }

        public VSConstants.MessageBoxResult ShowMessageBox(string messageBoxText)
        {
            return ShowMessageBox(messageBoxText, OLEMSGBUTTON.OLEMSGBUTTON_OK);
        }

        public VSConstants.MessageBoxResult ShowMessageBox(string messageBoxText, OLEMSGBUTTON button)
        {
            return ShowMessageBox(messageBoxText, button, OLEMSGICON.OLEMSGICON_NOICON);
        }

        public VSConstants.MessageBoxResult ShowMessageBox(string messageBoxText, OLEMSGBUTTON button, OLEMSGICON icon)
        {
            return ShowMessageBox(messageBoxText, button, icon, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public VSConstants.MessageBoxResult ShowMessageBox(string messageBoxText, OLEMSGBUTTON button, OLEMSGICON icon, OLEMSGDEFBUTTON defaultButton)
        {
            var parameter = new MessageBoxParameter();
            parameter.Text = messageBoxText;
            parameter.Title = string.Empty; // This is NOT window title. It is the constant "Microsoft Visual Studio".
            parameter.Button = button;
            parameter.Icon = icon;
            parameter.DefaultButton = defaultButton;
            MessageBoxParameter.Value = parameter;
            return parameter.Result;
        }

        ApplicationProperty<MessageBoxParameter> m_messageBoxParameter;
        public ApplicationProperty<MessageBoxParameter> MessageBoxParameter
        {
            get
            {
                if (m_messageBoxParameter == null)
                    m_messageBoxParameter = new ApplicationProperty<MessageBoxParameter>();
                return m_messageBoxParameter;
            }
        }

        StatusbarViewModel m_statusbar;
        public StatusbarViewModel Statusbar
        {
            get
            {
                if (m_statusbar == null)
                    m_statusbar = new StatusbarViewModel();
                return m_statusbar;
            }
        }
    }
}
