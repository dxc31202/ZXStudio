﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    internal static partial class Opcodes
    {
        public static DOpcode[] FDPrefix = new DOpcode[]
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
            new DOpcode (0x09,"ADD IY,BC",15,0,"",true),
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
            new DOpcode (0x19,"ADD IY,DE",15,0,"",true),
            new DOpcode (0x1A,null,0,0,"",false),
            new DOpcode (0x1B,null,0,0,"",false),
            new DOpcode (0x1C,null,0,0,"",false),
            new DOpcode (0x1D,null,0,0,"",false),
            new DOpcode (0x1E,null,0,0,"",false),
            new DOpcode (0x1F,null,0,0,"",false),
            new DOpcode (0x20,null,0,0,"",false),
            new DOpcode (0x21,"LD IY,$nn",14,0,"",true),
            new DOpcode (0x22,"LD ($nn),IY",20,0,"Indirect",true),
            new DOpcode (0x23,"INC IY",10,0,"",true),
            new DOpcode (0x24,"INC IYH",8,0,"",false),
            new DOpcode (0x25,"DEC IYH",8,0,"",false),
            new DOpcode (0x26,"LD IYH,$b",11,0,"",false),
            new DOpcode (0x27,null,0,0,"",false),
            new DOpcode (0x28,null,0,0,"",false),
            new DOpcode (0x29,"ADD IY,IY",15,0,"",true),
            new DOpcode (0x2A,"LD IY,($nn)",20,0,"Indirect",true),
            new DOpcode (0x2B,"DEC IY",10,0,"",true),
            new DOpcode (0x2C,"INC IYL",8,0,"",true),
            new DOpcode (0x2D,"DEC IYL",8,0,"",true),
            new DOpcode (0x2E,"LD IYL,$b",11,0,"",true),
            new DOpcode (0x2F,null,0,0,"",false),
            new DOpcode (0x30,null,0,0,"",false),
            new DOpcode (0x31,null,0,0,"",false),
            new DOpcode (0x32,null,0,0,"",false),
            new DOpcode (0x33,null,0,0,"",false),
            new DOpcode (0x34,"INC (IY$o)",23,0,"Indexed",true),
            new DOpcode (0x35,"DEC (IY$o)",23,0,"Indexed",true),
            new DOpcode (0x36,"LD (IY$o),$b",19,0,"Indexed",true),
            new DOpcode (0x37,null,0,0,"",false),
            new DOpcode (0x38,null,0,0,"",false),
            new DOpcode (0x39,"ADD IY,SP",15,0,"",true),
            new DOpcode (0x3A,null,0,0,"",false),
            new DOpcode (0x3B,null,0,0,"",false),
            new DOpcode (0x3C,null,0,0,"",false),
            new DOpcode (0x3D,null,0,0,"",false),
            new DOpcode (0x3E,null,0,0,"",false),
            new DOpcode (0x3F,null,0,0,"",false),
            new DOpcode (0x40,null,0,0,"",false),
            new DOpcode (0x41,null,0,0,"",false),
            new DOpcode (0x42,null,0,0,"",false),
            new DOpcode (0x43,null,0,0,"",false),
            new DOpcode (0x44,"LD B,IYH",8,0,"",false),
            new DOpcode (0x45,"LD B,IYL",8,0,"",false),
            new DOpcode (0x46,"LD B,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x47,null,0,0,"",false),
            new DOpcode (0x48,null,0,0,"",false),
            new DOpcode (0x49,null,0,0,"",false),
            new DOpcode (0x4A,null,0,0,"",false),
            new DOpcode (0x4B,null,0,0,"",false),
            new DOpcode (0x4C,"LD C,IYH",8,0,"",false),
            new DOpcode (0x4D,"LD C,IYL",8,0,"",false),
            new DOpcode (0x4E,"LD C,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x4F,null,0,0,"",false),
            new DOpcode (0x50,null,0,0,"",false),
            new DOpcode (0x51,null,0,0,"",false),
            new DOpcode (0x52,null,0,0,"",false),
            new DOpcode (0x53,null,0,0,"",false),
            new DOpcode (0x54,"LD D,IYH",8,0,"",false),
            new DOpcode (0x55,"LD D,IYL",8,0,"",false),
            new DOpcode (0x56,"LD D,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x57,null,0,0,"",false),
            new DOpcode (0x58,null,0,0,"",false),
            new DOpcode (0x59,null,0,0,"",false),
            new DOpcode (0x5A,null,0,0,"",false),
            new DOpcode (0x5B,null,0,0,"",false),
            new DOpcode (0x5C,"LD E,IYH",8,0,"",false),
            new DOpcode (0x5D,"LD E,IYL",8,0,"",false),
            new DOpcode (0x5E,"LD E,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x5F,null,0,0,"",false),
            new DOpcode (0x60,"LD IYH,B",8,0,"",true),
            new DOpcode (0x61,"LD IYH,C",8,0,"",true),
            new DOpcode (0x62,"LD IYH,D",8,0,"",true),
            new DOpcode (0x63,"LD IYH,E",8,0,"",true),
            new DOpcode (0x64,"LD IYH,IYH",8,0,"",false),
            new DOpcode (0x65,"LD IYH,IYL",8,0,"",false),
            new DOpcode (0x66,"LD H,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x67,"LD IYH,A",8,0,"",true),
            new DOpcode (0x68,"LD IYL,B",8,0,"",true),
            new DOpcode (0x69,"LD IYL,C",8,0,"",true),
            new DOpcode (0x6A,"LD IYL,D",8,0,"",true),
            new DOpcode (0x6B,"LD IYL,E",8,0,"",true),
            new DOpcode (0x6C,"LD IYL,IYH",8,0,"",false),
            new DOpcode (0x6D,"LD IYL,IYL",8,0,"",false),
            new DOpcode (0x6E,"LD L,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x6F,"LD IYL,A",8,0,"",true),
            new DOpcode (0x70,"LD (IY$o),B",19,0,"Indexed",true),
            new DOpcode (0x71,"LD (IY$o),C",19,0,"Indexed",true),
            new DOpcode (0x72,"LD (IY$o),D",19,0,"Indexed",true),
            new DOpcode (0x73,"LD (IY$o),E",19,0,"Indexed",true),
            new DOpcode (0x74,"LD (IY$o),H",19,0,"Indexed",true),
            new DOpcode (0x75,"LD (IY$o),L",19,0,"Indexed",true),
            new DOpcode (0x76,null,0,0,"",false),
            new DOpcode (0x77,"LD (IY$o),A",19,0,"Indexed",true),
            new DOpcode (0x78,null,0,0,"",false),
            new DOpcode (0x79,null,0,0,"",false),
            new DOpcode (0x7A,null,0,0,"",false),
            new DOpcode (0x7B,null,0,0,"",false),
            new DOpcode (0x7C,"LD A,IYH",8,0,"",false),
            new DOpcode (0x7D,"LD A,IYL",8,0,"",false),
            new DOpcode (0x7E,"LD A,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x7F,null,0,0,"",false),
            new DOpcode (0x80,null,0,0,"",false),
            new DOpcode (0x81,null,0,0,"",false),
            new DOpcode (0x82,null,0,0,"",false),
            new DOpcode (0x83,null,0,0,"",false),
            new DOpcode (0x84,"ADD A,IYH",8,0,"",false),
            new DOpcode (0x85,"ADD A,IYL",8,0,"",false),
            new DOpcode (0x86,"ADD A,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x87,null,0,0,"",false),
            new DOpcode (0x88,null,0,0,"",false),
            new DOpcode (0x89,null,0,0,"",false),
            new DOpcode (0x8A,null,0,0,"",false),
            new DOpcode (0x8B,null,0,0,"",false),
            new DOpcode (0x8C,"ADC A,IYH",8,0,"",false),
            new DOpcode (0x8D,"ADC A,IYL",8,0,"",false),
            new DOpcode (0x8E,"ADC A,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x8F,null,0,0,"",false),
            new DOpcode (0x90,null,0,0,"",false),
            new DOpcode (0x91,null,0,0,"",false),
            new DOpcode (0x92,null,0,0,"",false),
            new DOpcode (0x93,null,0,0,"",false),
            new DOpcode (0x94,"SUB IYH",8,0,"",false),
            new DOpcode (0x95,"SUB IYL",8,0,"",false),
            new DOpcode (0x96,"SUB (IY$o)",19,0,"Indexed",false),
            new DOpcode (0x97,null,0,0,"",false),
            new DOpcode (0x98,null,0,0,"",false),
            new DOpcode (0x99,null,0,0,"",false),
            new DOpcode (0x9A,null,0,0,"",false),
            new DOpcode (0x9B,null,0,0,"",false),
            new DOpcode (0x9C,"SBC A,IYH",8,0,"",false),
            new DOpcode (0x9D,"SBC A,IYL",8,0,"",false),
            new DOpcode (0x9E,"SBC A,(IY$o)",19,0,"Indexed",false),
            new DOpcode (0x9F,null,0,0,"",false),
            new DOpcode (0xA0,null,0,0,"",false),
            new DOpcode (0xA1,null,0,0,"",false),
            new DOpcode (0xA2,null,0,0,"",false),
            new DOpcode (0xA3,null,0,0,"",false),
            new DOpcode (0xA4,"AND IYH",8,0,"",false),
            new DOpcode (0xA5,"AND IYL",8,0,"",false),
            new DOpcode (0xA6,"AND (IY$o)",19,0,"Indexed",false),
            new DOpcode (0xA7,null,0,0,"",false),
            new DOpcode (0xA8,null,0,0,"",false),
            new DOpcode (0xA9,null,0,0,"",false),
            new DOpcode (0xAA,null,0,0,"",false),
            new DOpcode (0xAB,null,0,0,"",false),
            new DOpcode (0xAC,"XOR IYH",8,0,"",false),
            new DOpcode (0xAD,"XOR IYL",8,0,"",false),
            new DOpcode (0xAE,"XOR (IY$o)",19,0,"Indexed",false),
            new DOpcode (0xAF,null,0,0,"",false),
            new DOpcode (0xB0,null,0,0,"",false),
            new DOpcode (0xB1,null,0,0,"",false),
            new DOpcode (0xB2,null,0,0,"",false),
            new DOpcode (0xB3,null,0,0,"",false),
            new DOpcode (0xB4,"OR IYH",8,0,"",false),
            new DOpcode (0xB5,"OR IYL",8,0,"",false),
            new DOpcode (0xB6,"OR (IY$o)",19,0,"Indexed",false),
            new DOpcode (0xB7,null,0,0,"",false),
            new DOpcode (0xB8,null,0,0,"",false),
            new DOpcode (0xB9,null,0,0,"",false),
            new DOpcode (0xBA,null,0,0,"",false),
            new DOpcode (0xBB,null,0,0,"",false),
            new DOpcode (0xBC,"CP IYH",8,0,"",false),
            new DOpcode (0xBD,"CP IYL",8,0,"",false),
            new DOpcode (0xBE,"CP (IY$o)",19,0,"Indexed",false),
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
            new DOpcode (0xCB,"AfterCB",0,0,"",true),
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
            new DOpcode (0xDD,"AfterDD",4,0,"",true),
            new DOpcode (0xDE,null,0,0,"",false),
            new DOpcode (0xDF,null,0,0,"",false),
            new DOpcode (0xE0,null,0,0,"",false),
            new DOpcode (0xE1,"POP IY",14,0,"",true),
            new DOpcode (0xE2,null,0,0,"",false),
            new DOpcode (0xE3,"EX (SP),IY",23,0,"",true),
            new DOpcode (0xE4,null,0,0,"",false),
            new DOpcode (0xE5,"PUSH IY",15,0,"",true),
            new DOpcode (0xE6,null,0,0,"",false),
            new DOpcode (0xE7,null,0,0,"",false),
            new DOpcode (0xE8,null,0,0,"",false),
            new DOpcode (0xE9,"JP (IY)",8,0,"Branch Terminal",true),
            new DOpcode (0xEA,null,0,0,"",false),
            new DOpcode (0xEB,null,0,0,"",false),
            new DOpcode (0xEC,null,0,0,"",false),
            new DOpcode (0xED,"AfterED",4,0,"",true),
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
            new DOpcode (0xF9,"LD SP,IY",10,0,"",true),
            new DOpcode (0xFA,null,0,0,"",false),
            new DOpcode (0xFB,null,0,0,"",false),
            new DOpcode (0xFC,null,0,0,"",false),
            new DOpcode (0xFD,"AfterFD",4,0,"",true),
            new DOpcode (0xFE,null,0,0,"",false),
            new DOpcode (0xFF,"null",0,0,"",true)
        };
    }
}

