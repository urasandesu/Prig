/* 
 * File: ULWebClient.cs
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



using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace UntestableLibrary
{
    public class ULWebClient
    {
        public event AsyncCompletedEventHandler DownloadFileCompleted;

        public void DownloadFileAsync(Uri address)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(5000); // simulate heavy network traffic

                    var fileName = Path.GetTempFileName();
                    using (var sw = new StreamWriter(fileName))
                        sw.WriteLine(Guid.NewGuid());
                    OnDownloadFileCompleted(null, false, fileName);
                }
                catch (Exception e)
                {
                    OnDownloadFileCompleted(e, true, null);
                }
            });
            thread.Start();
        }

        void OnDownloadFileCompleted(Exception error, bool cancelled, params object[] userState)
        {
            var handler = DownloadFileCompleted;
            if (handler == null)
                return;

            handler(this, new AsyncCompletedEventArgs(error, cancelled, userState));
        }
    }
}
