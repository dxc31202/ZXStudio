﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    internal static partial class Opcodes
    {
        public static DOpcode[] EDPrefix = new DOpcode[]
        {
            new DOpcode (0x00,null,0,0,"",false),
            new DOpcode (0x01,null,0,0,"",false),
            new DOpcode (0x02,null,0,0,"",false),
            new DOpcode (0x03,null,0,0,"",false),
            new DOpcode (0x04,null,0,0,"",false),
            new DOpcode (0x05,null,0,0,"",false),
            new DOpcode (0x06,null,0,0,"",false),
            new DOpcode (0x07,null,0,0,"",false),
            new DOpcode (0x08,null,0,0,"",false),
            new DOpcode (0x09,null,0,0,"",false),
            new DOpcode (0x0A,null,0,0,"",false),
            new DOpcode (0x0B,null,0,0,"",false),
            new DOpcode (0x0C,null,0,0,"",false),
            new DOpcode (0x0D,null,0,0,"",false),
            new DOpcode (0x0E,null,0,0,"",false),
            new DOpcode (0x0F,null,0,0,"",false),
            new DOpcode (0x10,null,0,0,"",false),
            new DOpcode (0x11,null,0,0,"",false),
            new DOpcode (0x12,null,0,0,"",false),
            new DOpcode (0x13,null,0,0,"",false),
            new DOpcode (0x14,null,0,0,"",false),
            new DOpcode (0x15,null,0,0,"",false),
            new DOpcode (0x16,null,0,0,"",false),
            new DOpcode (0x17,null,0,0,"",false),
            new DOpcode (0x18,null,0,0,"",false),
            new DOpcode (0x19,null,0,0,"",false),
            new DOpcode (0x1A,null,0,0,"",false),
            new DOpcode (0x1B,null,0,0,"",false),
            new DOpcode (0x1C,null,0,0,"",false),
            new DOpcode (0x1D,null,0,0,"",false),
            new DOpcode (0x1E,null,0,0,"",false),
            new DOpcode (0x1F,null,0,0,"",false),
            new DOpcode (0x20,null,0,0,"",false),
            new DOpcode (0x21,null,0,0,"",false),
            new DOpcode (0x22,null,0,0,"",false),
            new DOpcode (0x23,null,0,0,"",false),
            new DOpcode (0x24,null,0,0,"",false),
            new DOpcode (0x25,null,0,0,"",false),
            new DOpcode (0x26,null,0,0,"",false),
            new DOpcode (0x27,null,0,0,"",false),
            new DOpcode (0x28,null,0,0,"",false),
            new DOpcode (0x29,null,0,0,"",false),
            new DOpcode (0x2A,null,0,0,"",false),
            new DOpcode (0x2B,null,0,0,"",false),
            new DOpcode (0x2C,null,0,0,"",false),
            new DOpcode (0x2D,null,0,0,"",false),
            new DOpcode (0x2E,null,0,0,"",false),
            new DOpcode (0x2F,null,0,0,"",false),
            new DOpcode (0x30,null,0,0,"",false),
            new DOpcode (0x31,null,0,0,"",false),
            new DOpcode (0x32,null,0,0,"",false),
            new DOpcode (0x33,null,0,0,"",false),
            new DOpcode (0x34,null,0,0,"",false),
            new DOpcode (0x35,null,0,0,"",false),
            new DOpcode (0x36,null,0,0,"",false),
            new DOpcode (0x37,null,0,0,"",false),
            new DOpcode (0x38,null,0,0,"",false),
            new DOpcode (0x39,null,0,0,"",false),
            new DOpcode (0x3A,null,0,0,"",false),
            new DOpcode (0x3B,null,0,0,"",false),
            new DOpcode (0x3C,null,0,0,"",false),
            new DOpcode (0x3D,null,0,0,"",false),
            new DOpcode (0x3E,null,0,0,"",false),
            new DOpcode (0x3F,null,0,0,"",false),
            new DOpcode (0x40,"IN B,(C)",12,0,"",true),
            new DOpcode (0x41,"OUT (C),B",12,0,"",true),
            new DOpcode (0x42,"SBC HL,BC",15,0,"",true),
            new DOpcode (0x43,"LD ($nn),BC",20,0,"",true),
            new DOpcode (0x44,"NEG",8,0,"",true),
            new DOpcode (0x45,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x46,"IM 0",8,0,"",true),
            new DOpcode (0x47,"LD I,A",9,0,"",true),
            new DOpcode (0x48,"IN C,(C)",12,0,"",true),
            new DOpcode (0x49,"OUT (C),C",12,0,"",true),
            new DOpcode (0x4A,"ADC HL,BC",15,0,"",true),
            new DOpcode (0x4B,"LD BC,($nn)",20,0,"",true),
            new DOpcode (0x4C,"NEG",8,0,"",true),
            new DOpcode (0x4D,"RETI",14,0,"Return Terminal",true),
            new DOpcode (0x4E,"IM 0",8,0,"",true),
            new DOpcode (0x4F,"LD_R_A",9,0,"",true),
            new DOpcode (0x50,"IN D,(C)",12,0,"",true),
            new DOpcode (0x51,"OUT (C),D",12,0,"",true),
            new DOpcode (0x52,"SBC HL,DE",15,0,"",true),
            new DOpcode (0x53,"LD ($nn),DE",20,0,"",true),
            new DOpcode (0x54,"NEG",8,0,"",true),
            new DOpcode (0x55,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x56,"IM 1",8,0,"",true),
            new DOpcode (0x57,"LD A,I",9,0,"",true),
            new DOpcode (0x58,"IN E,(C)",12,0,"",true),
            new DOpcode (0x59,"OUT (C),E",12,0,"",true),
            new DOpcode (0x5A,"ADC HL,DE",15,0,"",true),
            new DOpcode (0x5B,"LD DE,($nn)",20,0,"",true),
            new DOpcode (0x5C,"NEG",8,0,"",true),
            new DOpcode (0x5D,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x5E,"IM 2",8,0,"",true),
            new DOpcode (0x5F,"LD A,R",9,0,"",true),
            new DOpcode (0x60,"IN H,(C)",12,0,"",true),
            new DOpcode (0x61,"OUT (C),H",12,0,"",true),
            new DOpcode (0x62,"SBC HL,HL",15,0,"",true),
            new DOpcode (0x63,"LD ($nn),HL",20,0,"",true),
            new DOpcode (0x64,"NEG",8,0,"",true),
            new DOpcode (0x65,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x66,"IM 0",8,0,"",true),
            new DOpcode (0x67,"RRD",18,0,"",true),
            new DOpcode (0x68,"IN L,(C)",12,0,"",true),
            new DOpcode (0x69,"OUT (C),L",12,0,"",true),
            new DOpcode (0x6A,"ADC HL,HL",15,0,"",true),
            new DOpcode (0x6B,"LD HL,($nn)",20,0,"",true),
            new DOpcode (0x6C,"NEG",8,0,"",true),
            new DOpcode (0x6D,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x6E,"IM 0",8,0,"",true),
            new DOpcode (0x6F,"RLD",18,0,"",true),
            new DOpcode (0x70,"IN (C)",12,0,"",true),
            new DOpcode (0x71,"OUT (C),0",12,0,"",true),
            new DOpcode (0x72,"SBC HL,SP",15,0,"",true),
            new DOpcode (0x73,"LD ($nn),SP",20,0,"",true),
            new DOpcode (0x74,"NEG",8,0,"",true),
            new DOpcode (0x75,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x76,"IM 1",8,0,"",true),
            new DOpcode (0x77,null,0,0,"",false),
            new DOpcode (0x78,"IN A,(C)",12,0,"",true),
            new DOpcode (0x79,"OUT (C),A",12,0,"",true),
            new DOpcode (0x7A,"ADC HL,SP",15,0,"",true),
            new DOpcode (0x7B,"LD SP,($nn)",20,0,"",true),
            new DOpcode (0x7C,"NEG",8,0,"",true),
            new DOpcode (0x7D,"RETN",14,0,"Return Terminal",true),
            new DOpcode (0x7E,"IM 2",8,0,"",true),
            new DOpcode (0x7F,null,0,0,"",false),
            new DOpcode (0x80,null,0,0,"",false),
            new DOpcode (0x81,null,0,0,"",false),
            new DOpcode (0x82,null,0,0,"",false),
            new DOpcode (0x83,null,0,0,"",false),
            new DOpcode (0x84,null,0,0,"",false),
            new DOpcode (0x85,null,0,0,"",false),
            new DOpcode (0x86,null,0,0,"",false),
            new DOpcode (0x87,null,0,0,"",false),
            new DOpcode (0x88,null,0,0,"",false),
            new DOpcode (0x89,null,0,0,"",false),
            new DOpcode (0x8A,null,0,0,"",false),
            new DOpcode (0x8B,null,0,0,"",false),
            new DOpcode (0x8C,null,0,0,"",false),
            new DOpcode (0x8D,null,0,0,"",false),
            new DOpcode (0x8E,null,0,0,"",false),
            new DOpcode (0x8F,null,0,0,"",false),
            new DOpcode (0x90,null,0,0,"",false),
            new DOpcode (0x91,null,0,0,"",false),
            new DOpcode (0x92,null,0,0,"",false),
            new DOpcode (0x93,null,0,0,"",false),
            new DOpcode (0x94,null,0,0,"",false),
            new DOpcode (0x95,null,0,0,"",false),
            new DOpcode (0x96,null,0,0,"",false),
            new DOpcode (0x97,null,0,0,"",false),
            new DOpcode (0x98,null,0,0,"",false),
            new DOpcode (0x99,null,0,0,"",false),
            new DOpcode (0x9A,null,0,0,"",false),
            new DOpcode (0x9B,null,0,0,"",false),
            new DOpcode (0x9C,null,0,0,"",false),
            new DOpcode (0x9D,null,0,0,"",false),
            new DOpcode (0x9E,null,0,0,"",false),
            new DOpcode (0x9F,null,0,0,"",false),
            new DOpcode (0xA0,"LDI",16,0,"",true),
            new DOpcode (0xA1,"CPI",16,0,"",true),
            new DOpcode (0xA2,"INI",16,0,"",true),
            new DOpcode (0xA3,"OUTI",16,0,"",true),
            new DOpcode (0xA4,null,0,0,"",false),
            new DOpcode (0xA5,null,0,0,"",false),
            new DOpcode (0xA6,null,0,0,"",false),
            new DOpcode (0xA7,null,0,0,"",false),
            new DOpcode (0xA8,"LDD",16,0,"",true),
            new DOpcode (0xA9,"CPD",16,0,"",true),
            new DOpcode (0xAA,"IND",16,0,"",true),
            new DOpcode (0xAB,"OUTD",16,0,"",true),
            new DOpcode (0xAC,null,0,0,"",false),
            new DOpcode (0xAD,null,0,0,"",false),
            new DOpcode (0xAE,null,0,0,"",false),
            new DOpcode (0xAF,null,0,0,"",false),
            new DOpcode (0xB0,"LDIR",16,21,"",true),
            new DOpcode (0xB1,"CPIR",16,21,"",true),
            new DOpcode (0xB2,"INIR",16,21,"",true),
            new DOpcode (0xB3,"OTIR",16,21,"",true),
            new DOpcode (0xB4,null,0,0,"",false),
            new DOpcode (0xB5,null,0,0,"",false),
            new DOpcode (0xB6,null,0,0,"",false),
            new DOpcode (0xB7,null,0,0,"",false),
            new DOpcode (0xB8,"LDDR",16,21,"",true),
            new DOpcode (0xB9,"CPDR",16,21,"",true),
            new DOpcode (0xBA,"INDR",16,21,"",true),
            new DOpcode (0xBB,"OTDR",16,21,"",true),
            new DOpcode (0xBC,null,0,0,"",false),
            new DOpcode (0xBD,null,0,0,"",false),
            new DOpcode (0xBE,null,0,0,"",false),
            new DOpcode (0xBF,null,0,0,"",false),
            new DOpcode (0xC0,null,0,0,"",false),
            new DOpcode (0xC1,null,0,0,"",false),
            new DOpcode (0xC2,null,0,0,"",false),
            new DOpcode (0xC3,null,0,0,"",false),
            new DOpcode (0xC4,null,0,0,"",false),
            new DOpcode (0xC5,null,0,0,"",false),
            new DOpcode (0xC6,null,0,0,"",false),
            new DOpcode (0xC7,null,0,0,"",false),
            new DOpcode (0xC8,null,0,0,"",false),
            new DOpcode (0xC9,null,0,0,"",false),
            new DOpcode (0xCA,null,0,0,"",false),
            new DOpcode (0xCB,null,0,0,"",false),
            new DOpcode (0xCC,null,0,0,"",false),
            new DOpcode (0xCD,null,0,0,"",false),
            new DOpcode (0xCE,null,0,0,"",false),
            new DOpcode (0xCF,null,0,0,"",false),
            new DOpcode (0xD0,null,0,0,"",false),
            new DOpcode (0xD1,null,0,0,"",false),
            new DOpcode (0xD2,null,0,0,"",false),
            new DOpcode (0xD3,null,0,0,"",false),
            new DOpcode (0xD4,null,0,0,"",false),
            new DOpcode (0xD5,null,0,0,"",false),
            new DOpcode (0xD6,null,0,0,"",false),
            new DOpcode (0xD7,null,0,0,"",false),
            new DOpcode (0xD8,null,0,0,"",false),
            new DOpcode (0xD9,null,0,0,"",false),
            new DOpcode (0xDA,null,0,0,"",false),
            new DOpcode (0xDB,null,0,0,"",false),
            new DOpcode (0xDC,null,0,0,"",false),
            new DOpcode (0xDD,null,0,0,"",false),
            new DOpcode (0xDE,null,0,0,"",false),
            new DOpcode (0xDF,null,0,0,"",false),
            new DOpcode (0xE0,null,0,0,"",false),
            new DOpcode (0xE1,null,0,0,"",false),
            new DOpcode (0xE2,null,0,0,"",false),
            new DOpcode (0xE3,null,0,0,"",false),
            new DOpcode (0xE4,null,0,0,"",false),
            new DOpcode (0xE5,null,0,0,"",false),
            new DOpcode (0xE6,null,0,0,"",false),
            new DOpcode (0xE7,null,0,0,"",false),
            new DOpcode (0xE8,null,0,0,"",false),
            new DOpcode (0xE9,null,0,0,"",false),
            new DOpcode (0xEA,null,0,0,"",false),
            new DOpcode (0xEB,null,0,0,"",false),
            new DOpcode (0xEC,null,0,0,"",false),
            new DOpcode (0xED,null,0,0,"",false),
            new DOpcode (0xEE,null,0,0,"",false),
            new DOpcode (0xEF,null,0,0,"",false),
            new DOpcode (0xF0,null,0,0,"",false),
            new DOpcode (0xF1,null,0,0,"",false),
            new DOpcode (0xF2,null,0,0,"",false),
            new DOpcode (0xF3,null,0,0,"",false),
            new DOpcode (0xF4,null,0,0,"",false),
            new DOpcode (0xF5,null,0,0,"",false),
            new DOpcode (0xF6,null,0,0,"",false),
            new DOpcode (0xF7,null,0,0,"",false),
            new DOpcode (0xF8,null,0,0,"",false),
            new DOpcode (0xF9,null,0,0,"",false),
            new DOpcode (0xFA,null,0,0,"",false),
            new DOpcode (0xFB,null,0,0,"",false),
            new DOpcode (0xFC,null,0,0,"",false),
            new DOpcode (0xFD,null,0,0,"",false),
            new DOpcode (0xFE,null,0,0,"",false),
            new DOpcode (0xFF,"null",0,0,"",true)
        };
    }
}

