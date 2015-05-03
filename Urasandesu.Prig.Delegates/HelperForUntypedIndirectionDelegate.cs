/* 
 * File: HelperForUntypedIndirectionDelegate.cs
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
using System.Collections.Generic;
using Urasandesu.NAnonym;
using Urasandesu.Prig.Framework;

namespace Urasandesu.Prig.Delegates
{
    public static class HelperForUntypedIndirectionDelegate
    {
        public static Work CreateDelegateOfDefaultBehavior(Type indDlgt, IndirectionBehaviors defaultBehavior)
        {
            if (indDlgt == null)
                throw new ArgumentNullException("indDlgt");

            if (!indDlgt.IsSubclassOf(typeof(MulticastDelegate)))
                throw new ArgumentException("The parameter must be a delegate type.", "indDlgt");

            var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
            var @params = indDlgt_Invoke.GetParameters();

            return new Work(args =>
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                if (defaultBehavior == IndirectionBehaviors.DefaultValue)
                {
                    if (args.Length != @params.Length)
                    {
                        var msg = string.Format("This default behavior is initialized as {0} parameter(s) method but it is called as {1} parameter(s) method.",
                                                @params.Length, args.Length);
                        throw new ArgumentException(msg, "args");
                    }

                    for (int i = 0; i < @params.Length; i++)
                    {
                        if (@params[i].ParameterType.IsByRef)
                        {
                            var paramType = @params[i].ParameterType.GetElementType();
                            args[i] = GetDefaultValue(paramType);
                        }
                    }

                    return GetDefaultValue(indDlgt_Invoke.ReturnType);
                }

                if (defaultBehavior == IndirectionBehaviors.Fallthrough)
                {
                    using (InstanceGetters.DisableProcessing())
                        throw new FallthroughException();
                }

                using (InstanceGetters.DisableProcessing())
                    throw new NotImplementedException();
            });
        }

        static object GetDefaultValue(Type paramType)
        {
            if (paramType == typeof(void))
                return null;
            else if (paramType.IsValueType)
                return Activator.CreateInstance(paramType);
            else
                return null;
        }

        public static Work CreateDelegateExecutingDefaultOrUntypedDelegate(Type indDlgt, Work defaultBehavior, Dictionary<object, TargetSettingValue<Work>> indirections)
        {
            if (indDlgt == null)
                throw new ArgumentNullException("indDlgt");

            if (!indDlgt.IsSubclassOf(typeof(MulticastDelegate)))
                throw new ArgumentException("The parameter must be a delegate type.", "indDlgt");

            var indDlgt_Invoke = indDlgt.GetMethod("Invoke");
            var @params = indDlgt_Invoke.GetParameters();
            if (@params.Length < 1)
                throw new ArgumentException("The delegate type must have one or more parameters.", "indDlgt");

            return new Work(args =>
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                if (args.Length != @params.Length)
                {
                    var msg = string.Format("This default behavior is initialized as {0} parameter(s) method but it is called as {1} parameter(s) method.",
                                            @params.Length, args.Length);
                    throw new ArgumentException(msg, "args");
                }

                if (EqualityComparer<object>.Default.Equals(args[0], default(object)) || !indirections.ContainsKey(args[0]))
                {
                    var result = defaultBehavior(args);
                    return result;
                }
                else
                {
                    var result = indirections[args[0]].Indirection(args);
                    return result;
                }
            });
        }
    }
}
