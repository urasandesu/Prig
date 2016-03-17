/* 
 * File: ULChatBot.cs
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



using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace UntestableLibrary
{
    public enum ULActions
    {
        Unknown,
        Discard,
        ForwardToManagement,
        ForwardToDeveloper
    }

    public enum ULTypeOfFeedback
    {
        Complaint,
        Praise,
        Suggestion,
        Incomprehensible
    }

    public class ULChatBot
    {
        public ULTypeOfFeedback Recommendation { get; private set; }

        public bool ReplyInformation(ULTypeOfFeedback input, out string reply, ref ULActions action, out ULTypeOfFeedback recommendation)
        {
            Thread.Sleep(10000); // simulate connecting an external service and filling repltyText by using the data that are retrieved from some APIs
            var returnReply = false;
            var replyText = "Your feedback has been forwarded to the product manager.";

            reply = string.Empty;
            switch (input)
            {
                case ULTypeOfFeedback.Complaint:
                case ULTypeOfFeedback.Praise:
                    action = ULActions.ForwardToManagement;
                    reply = "Thank you. " + replyText;
                    returnReply = true;
                    break;
                case ULTypeOfFeedback.Suggestion:
                    action = ULActions.ForwardToDeveloper;
                    reply = replyText;
                    returnReply = true;
                    break;
                case ULTypeOfFeedback.Incomprehensible:
                default:
                    action = ULActions.Discard;
                    returnReply = false;
                    break;
            }
            Recommendation = recommendation = (ULTypeOfFeedback)(((int)input + 1) % 4);
            return returnReply;
        }
    }
}
