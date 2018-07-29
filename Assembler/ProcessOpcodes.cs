using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ProcessOpcode(Token token)
        {
            switch (token.Value.ToUpper())
            {
                case "EI": StoreOpcode(Codes.EI); break;
                case "DI": StoreOpcode(Codes.DI); break;
                case "SCF": StoreOpcode(Codes.SCF); break;
                case "CCF": StoreOpcode(Codes.CCF); break;
                case "CPL": StoreOpcode(Codes.CPL); break;
                case "DAA": StoreOpcode(Codes.DAA); break;
                case "RRA": StoreOpcode(Codes.RRA); break;
                case "RLA": StoreOpcode(Codes.RLA); break;
                case "NOP": StoreOpcode(Codes.NOP); break;
                case "EXX": StoreOpcode(Codes.EXX); break;
                case "RLCA": StoreOpcode(Codes.RLCA); break;
                case "RRCA": StoreOpcode(Codes.RRCA); break;
                case "HALT": StoreOpcode(Codes.HALT); break;

                case "NEG": StoreEDOpcode(CodesED.NEG); break;
                case "RRD": StoreEDOpcode(CodesED.RRD); break;
                case "RLD": StoreEDOpcode(CodesED.RLD); break;
                case "LDI": StoreEDOpcode(CodesED.LDI); break;
                case "CPI": StoreEDOpcode(CodesED.CPI); break;
                case "INI": StoreEDOpcode(CodesED.INI); break;
                case "LDD": StoreEDOpcode(CodesED.LDD); break;
                case "CPD": StoreEDOpcode(CodesED.CPD); break;
                case "IND": StoreEDOpcode(CodesED.IND); break;
                case "OUTI": StoreEDOpcode(CodesED.OUTI); break;
                case "OUTD": StoreEDOpcode(CodesED.OUTD); break;
                case "LDIR": StoreEDOpcode(CodesED.LDIR); break;
                case "CPIR": StoreEDOpcode(CodesED.CPIR); break;
                case "INIR": StoreEDOpcode(CodesED.INIR); break;
                case "OTIR": StoreEDOpcode(CodesED.OTIR); break;
                case "LDDR": StoreEDOpcode(CodesED.LDDR); break;
                case "CPDR": StoreEDOpcode(CodesED.CPDR); break;
                case "INDR": StoreEDOpcode(CodesED.INDR); break;
                case "OTDR": StoreEDOpcode(CodesED.OTDR); break;
                case "RETI": StoreEDOpcode(CodesED.RETI); break;
                case "RETN": StoreEDOpcode(CodesED.RETN); break;

                case "ADC": token = ADC(token); break;
                case "ADD": token = ADD(token); break;
                case "AND": token = AND(token); break;
                case "OR": token = OR(token); break;
                case "XOR": token = XOR(token); break;
                case "CP": token = CP(token); break;
                case "SUB": token = SUB(token); break;
                case "SBC": token = SBC(token); break;
                case "LD": token = LD(token); break;
                case "JP": token = JP(token); break;
                case "CALL": token = CALL(token); break;
                case "IM": token = IM(token); break;
                case "EX": token = EX(token); break;
                case "JR": token = JR(token); break;
                case "IN": token = IN(token); break;
                case "OUT": token = OUT(token); break;


                case "RLC": token = ROTATESHIFT(token, 0x00); break;
                case "RRC": token = ROTATESHIFT(token, 0x08); break;
                case "RL": token = ROTATESHIFT(token, 0x10); break;
                case "RR": token = ROTATESHIFT(token, 0x18); break;
                case "SLA": token = ROTATESHIFT(token, 0x20); break;
                case "SRA": token = ROTATESHIFT(token, 0x28); break;
                case "SLL": token = ROTATESHIFT(token, 0x30); break;
                case "SRL": token = ROTATESHIFT(token, 0x38); break;

                case "BIT": token = BITSETRES(token, 0x40); break;
                case "RES": token = BITSETRES(token, 0x80); break;
                case "SET": token = BITSETRES(token, 0xC0); break;

                case "INC": token = INCDEC(token, 0x00); break;
                case "DEC": token = INCDEC(token, 0x08); break;

                case "RET": token = RET(token); break;

                case "PUSH": token = PUSHPOP(token, 0x04); break;
                case "POP": token = PUSHPOP(token, 0x00); break;

                case "DJNZ": token = DJNZ(token); break;

                case "RST": token = RST(token); break;
            }
            EOL(token);

            return token;
        }
    }
}
