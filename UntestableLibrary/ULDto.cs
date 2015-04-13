/* 
 * File: ULDto.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UntestableLibrary
{
    class ULTableStatus
    {
        internal bool IsOpened = false;
        internal int RowsCount = 0;
    }

    public class ULColumn
    {
        public ULColumn(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public class ULColumns : IEnumerable
    {
        ULTableStatus m_status;
        List<ULColumn> m_columns = new List<ULColumn>();

        internal ULColumns(ULTableStatus status)
        {
            m_status = status;
        }

        public void Add(ULColumn column)
        {
            ValidateState(m_status);
            m_columns.Add(column);
        }

        public void Remove(ULColumn column)
        {
            ValidateState(m_status);
            m_columns.Remove(column);
        }

        public IEnumerator GetEnumerator()
        {
            return m_columns.GetEnumerator();
        }

        static void ValidateState(ULTableStatus status)
        {
            if (!status.IsOpened)
            {
                Console.WriteLine(string.Format("{0} property value is {1}.", ULNameHalper.GetName(() => status.IsOpened), status.IsOpened));
                Console.WriteLine(string.Format("By the way, look at {0} field. What do you think of it?", ULNameHalper.GetName(() => DateTime.MaxValue)));
                throw new InvalidOperationException("The column can not be modified because owner table has not been opened.");
            }

            if (0 < status.RowsCount)
            {
                Console.WriteLine(string.Format("{0}.", ULNameHalper.GetName(() => ULConfigurationManager.GetProperty("foo", "bar"))));
                Console.WriteLine(string.Format("When are you going to do it? If not {0}, then when?", ULNameHalper.GetName(() => DateTime.Now)));
                throw new ArgumentException("The column can not be modified because some rows already exist.");
            }
        }
    }

    public class ULTable
    {
        ULTableStatus m_status = new ULTableStatus();

        public ULTable(string tableName)
        {
            TableName = tableName;
            Columns = new ULColumns(m_status);
        }

        public string TableName { get; private set; }
        public ULColumns Columns { get; private set; }

        public void Open(string connectionString)
        {
            Thread.Sleep(5000); // simulate connecting DB and filling this schema
            
            m_status.IsOpened = true;
        }
    }
}
