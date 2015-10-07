/* 
 * File: ChatRoom.cs
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


using System.Text;
using UntestableLibrary;

namespace program1.MyLibrary
{
    public class ChatRoom
    {
        public string Log { get; private set; }

        public void Start(ULTypeOfFeedback input, ULChatBot bot1, ULChatBot bot2)
        {
            var reply = default(string);
            var action = default(ULActions);
            var recommendation = default(ULTypeOfFeedback);
            var result = default(bool);

            var sb = new StringBuilder(Log);

            result = bot1.ReplyInformation(input, out reply, ref action, out recommendation);
            sb.AppendFormat("Reply: {0} Action: {1} return? {2}\r\n", reply, action, result);

            result = bot2.ReplyInformation(recommendation, out reply, ref action, out recommendation);
            sb.AppendFormat("Reply: {0} Action: {1} return? {2}\r\n", reply, action, result);

            Log = sb.ToString();
        }
    }
}
