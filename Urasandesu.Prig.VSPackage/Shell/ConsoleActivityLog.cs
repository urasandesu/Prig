/* 
 * File: ConsoleActivityLog.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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

namespace Urasandesu.Prig.VSPackage.Shell
{
    class ConsoleActivityLog
    {
        Logger m_logger;
        public ConsoleActivityLog()
        {
            var strLoggingEnabled = Environment.GetEnvironmentVariable("URASANDESU_VSIX_LOGGING_ENABLED");
            var loggingEnabled = 0;
            if (int.TryParse(strLoggingEnabled, out loggingEnabled) && 0 != loggingEnabled)
                m_logger = new DefaultLog();
            else
                m_logger = new EmptyLog();
        }

        public void Info(string description)
        {
            m_logger.Info(description);
        }

        public void Error(string description)
        {
            m_logger.Error(description);
        }

        abstract class Logger
        {
            public abstract void Info(string description);
            public abstract void Error(string description);
        }

        class DefaultLog : Logger
        {
            public override void Info(string description)
            {
                Console.Out.WriteLine(description);
            }

            public override void Error(string description)
            {
                Console.Error.WriteLine(description);
            }
        }

        class EmptyLog : Logger
        {
            public override void Info(string description)
            { }

            public override void Error(string description)
            { }
        }
    }
}
