//
//  Copyright (C) 2011 Robin Duerden (rduerden@gmail.com)
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
//
using System;
using System.Reflection;

namespace node.common.Exceptions
{
    public static class Preconditions
    {

        public static void IsNotNull( object value, string message, params string[] paramList )
        {
            IsNotNull<NodeApplicationException>( value, message, paramList );
        }

        public static void IsNotNull<T>( object value, string message, params string[] paramList ) where T : Exception
        {
            if( value != null ) return;

            throwException( typeof(T), message, paramList);
        }

        public static void IsNull( object value, string message, params string[] paramList )
        {
            IsNull<NodeApplicationException>( value, message, paramList );
        }

        public static void IsNull<T>( object value, string message, params string[] paramList ) where T : Exception
        {
            if( value != null ) return;

            throwException( typeof(T), message, paramList);
        }

        public static void IsFalse( bool value, string message, params string[] paramList )
        {
            IsTrue<NodeApplicationException>( !value, message, paramList );
        }

        public static void IsFalse<T>( bool value, string message, params string[] paramList ) where T : Exception
        {
            IsTrue<T>( !value, message, paramList );
        }

        public static void IsTrue( bool value, string message, params string[] paramList )
        {
            IsTrue<NodeApplicationException>( value, message, paramList );
        }

        public static void IsTrue<T>( bool value, string message, params string[] paramList ) where T : Exception
        {
            if( value ) return;

            throwException( typeof(T), message, paramList);
        }

        static void throwException (Type exceptionType, string message, string[] paramList)
        {
            ConstructorInfo constructorInfo = exceptionType.GetConstructor ( new[] { typeof(string) });

            Exception ex  = (Exception)constructorInfo.Invoke (new object[] { message });
            throw ex;
        }


    }
}

