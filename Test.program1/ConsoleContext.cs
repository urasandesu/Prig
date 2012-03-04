using System;
using System.IO;

namespace Test.program1
{
    public class ConsoleContext : IDisposable
    {
        bool m_disposed;
        TextWriter m_lastOut;

        public ConsoleContext()
        {
            m_lastOut = Console.Out;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    Console.SetOut(m_lastOut);
                }
            }
            m_disposed = true;
        }

        ~ConsoleContext()
        {
            Dispose(false);
        }
    }
}
