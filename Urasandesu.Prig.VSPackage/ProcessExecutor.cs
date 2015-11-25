/* 
 * File: ProcessExecutor.cs
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



using System;
using System.Diagnostics;

namespace Urasandesu.Prig.VSPackage
{
    class ProcessExecutor : IProcessExecutor
    {
        public TResult StartProcessWithoutShell<TResult>(string fileName, string arguments, Func<Process, TResult> resultSelector)
        {
            var info = NewProcessStartInfoWithoutShell(fileName, arguments);
            using (var p = Process.Start(info))
            {
                var result = resultSelector(p);
                p.WaitForExit();
                ValidateProcessExitCode(p);
                return result;
            }
        }

        static ProcessStartInfo NewProcessStartInfoWithoutShell(string fileName, string arguments)
        {
            var info = new ProcessStartInfo();
            info.FileName = fileName;
            info.Arguments = arguments;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            return info;
        }

        static void ValidateProcessExitCode(Process p)
        {
            if (p.ExitCode == 0)
                return;

            var msg = string.Format("The process '{0}' with arguments '{1}' exited with unsuccessful status. Code:'{2}', Message:'{3}', Error:'{4}'",
                                    p.StartInfo.FileName,
                                    p.StartInfo.Arguments,
                                    p.ExitCode,
                                    p.StandardOutput.ReadToEnd(),
                                    p.StandardError.ReadToEnd());
            throw new InvalidOperationException(msg);
        }
    }
}
