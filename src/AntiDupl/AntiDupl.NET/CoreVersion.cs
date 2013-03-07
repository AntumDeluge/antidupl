﻿/*
* AntiDupl.NET Program.
*
* Copyright (c) 2002-2013 Yermalayeu Ihar.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy 
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
* copies of the Software, and to permit persons to whom the Software is 
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in 
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiDupl.NET
{
    public class CoreVersion
    {
        public UInt32 major;
        public UInt32 minor;
        public UInt32 release;

        public CoreVersion(ref CoreDll.adVersion adVersion)
        {
            major = adVersion.major;
            minor = adVersion.minor;
            release = adVersion.release;
        }
        
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(major.ToString());
            builder.Append(".");
            builder.Append(minor.ToString());
            builder.Append(".");
            builder.Append(release.ToString());
            return builder.ToString();
        }
    }
}
