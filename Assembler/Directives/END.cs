﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token END(Token token)
        {
            terminate = true;
            return null;
        }
    }
}
