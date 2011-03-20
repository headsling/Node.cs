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
using Manos.Http;
using Manos.Collections;
namespace Manos
{
    public delegate void BodyDataHandler( HttpEntity entity, ByteBuffer data, int pos, int len );

    public class HttpPassthroughBodyHandler : IHttpBodyHandler
    {
        private BodyDataHandler handler;

        public HttpPassthroughBodyHandler( BodyDataHandler handler )
        {
            this.handler = handler;
        }

        public void HandleData (HttpEntity entity, ByteBuffer data, int pos, int len)
        {
            handler( entity, data, pos, len );
        }

        public void Finish (HttpEntity entity)
        {
            handler( entity, null, -1, -1 );
            handler = null;
        }

    }
}

