/* 
 * File: StatusbarViewModel.cs
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



namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public class StatusbarViewModel
    {
        uint m_maximum;

        public void BeginProgress(uint maximum)
        {
            m_maximum = maximum;
            ReportProgress(string.Empty, 0u);
        }

        public void ReportProgress(string label, uint value)
        {
            var progressState = new ProgressState();
            progressState.IsInProgress = true;
            progressState.Label = label;
            progressState.Value = value;
            progressState.Maximum = m_maximum;
            ProgressState.Value = progressState;
        }

        public void EndProgress()
        {
            var progressState = new ProgressState();
            progressState.IsInProgress = false;
            progressState.Label = string.Empty;
            progressState.Value = 0u;
            progressState.Maximum = 0u;
            ProgressState.Value = progressState;
        }

        ApplicationProperty<ProgressState> m_progressState;
        public ApplicationProperty<ProgressState> ProgressState
        {
            get
            {
                if (m_progressState == null)
                    m_progressState = new ApplicationProperty<ProgressState>();
                return m_progressState;
            }
        }

        ApplicationProperty<string> m_text;
        public ApplicationProperty<string> Text
        {
            get
            {
                if (m_text == null)
                    m_text = new ApplicationProperty<string>();
                return m_text;
            }
        }
    }
}
