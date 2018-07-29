using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token LD(Token token)
        {
            int n = 0;
            int m = 0;
            if ((token = NextToken(token)) == null) return token;
            RegisterType i = GetRegisterType(token);
            #region target can't be a number
            if (i == RegisterType.Unknown)
            {
                LogError(currentFileName, token,"Unknown Register, expecting register/pair or memory location");
                return token;
            }
            #endregion target can't be a number
            #region get target
            #region target is a memory location
            if (i == RegisterType.BRACKET)
            {
                if ((token = NextToken(token)) == null) return token;
                i = GetRegisterType(token);
                if (i != RegisterType.Unknown)
                {
                    if (i == RegisterType.IX || i == RegisterType.IY)
                    {
                        if ((token = NextToken(token)) == null) return token;
                        if (token.TokenType != TokenType.CloseBracket)
                        {
                            n = GetValue(ref token);
                            if ((token = NextToken(token)) == null) return token;

                        }
                    }
                    else
                    {
                        if (token.TokenType != TokenType.RegisterPair)
                        {
                            LogError(currentFileName, token,"Invalid target register");
                            return token;
                        }
                        if ((token = NextToken(token)) == null) return token;
                    }

                }
                else
                {
                    if (token.TokenType == TokenType.Symbol)
                    {
                        n = GetValue(ref token);
                        if ((token = NextToken(token)) == null) return token;
                    }
                    else
                    {
                        LogError(currentFileName, token,"Invalid target");
                        return token;
                    }
                }
                i += 100;
                if (token.Value != ")")
                {
                    LogError(currentFileName, token,"Expecting Closing Bracket");
                    if ((token = NextToken(token)) == null) return token;
                }
            }
            #endregion target is a memory location
            #region target is a register 
            else
            {
                // Nothing needs to be done
            }
            #endregion target is a register 
            #endregion get target
            #region move past the comma
            if ((token = NextToken(token)) == null) return token;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            #endregion move past the comma
            #region get source
            RegisterType j = GetRegisterType(token);
            #region source is a number?
            if (j == RegisterType.Unknown)
            {
                if (token.TokenType == TokenType.Symbol)
                {
                    m = GetValue(ref token);
                    token = token.Next;
                }
                else
                {
                    LogError(currentFileName, token,"Invalid target");
                    return token;
                }
            }
            #endregion source is a number?
            #region source is a memory location
            else if (j == RegisterType.BRACKET)
            {
                if ((token = NextToken(token)) == null) return token;
                j = GetRegisterType(token);
                if (j != RegisterType.Unknown)
                {
                    if (j == RegisterType.IX || j == RegisterType.IY)
                    {
                        if ((token = NextToken(token)) == null) return token;
                        if (token.IsSymbol || token.IsEquation)
                        {
                            n = GetValue(ref token);
                            if ((token = NextToken(token)) == null) return token;
                        }
                    }
                    else
                    {
                        if (token.TokenType != TokenType.RegisterPair)
                        {
                            LogError(currentFileName, token,"Invalid source register");
                            return token;
                        }
                        if ((token = NextToken(token)) == null) return token;
                    }
                }
                else
                {
                    if (token.TokenType == TokenType.Symbol)
                    {
                        m = GetValue(ref token);
                        if ((token = NextToken(token)) == null) return token;
                    }
                    else
                    {
                        LogError(currentFileName, token,"Invalid source");
                        return token;
                    }
                }
                j += 100;
                if (token.Value != ")")
                {
                    LogError(currentFileName, token,"Expecting Closing Bracket");
                }
            }
            #endregion source a memory location
            #region source is a register 
            else
            {
                // Nothing needs to be done
            }
            #endregion source is a register 
            #endregion get source
            #region target and source are both memory locations
            if ((int)i >= 99 && (int)j >= 99)
            {
                LogError(currentFileName, token,"Make your mind up! You can't access memory from source AND target, DUH!!");
                return token;
            }
            if (!EOL(token)) return token;
            #endregion target and source are both memory locations
            #region byte operations
            if (((i < RegisterType.BC || (int)i >= 99) ) && ((j < RegisterType.BC || (int)j >= 99) ))
            {
                #region ld r,n ld r,r ld r,xxh ld r,xxl
                if (i == 100 + RegisterType.HL) i = RegisterType.BRACKET;
                if (j == 100 + RegisterType.HL) j = RegisterType.BRACKET;
                if ((i >= RegisterType.B && i <= RegisterType.A))
                {
                    RegisterType j1 = j;
                    if ((int)j > 99) j1 = j - 100;
                    switch (j1)
                    {
                        case RegisterType.Unknown:
                            StoreOpcode((int)Codes.LD_B_N | ((int)i << 3));
                            StoreByte(m);
                            return token;
                        case RegisterType.IX:
                            StoreIXOpcodeDisplacement((byte)Codes.LD_B_xHL | ((int)i << 3), n);
                            return token;
                        case RegisterType.IY:
                            StoreIYOpcodeDisplacement((byte)Codes.LD_B_xHL | ((int)i << 3), n);
                            return token;
                        case RegisterType.XL:
                        case RegisterType.XH:
                            if (i == RegisterType.H || i == RegisterType.L || i == RegisterType.BRACKET)
                                break;
                            StoreIXOpcode(((int)Codes.LD_B_B | ((int)i << 3)) + ((int)j - (int)RegisterType.XH + (int)RegisterType.H));
                            return token;
                        case RegisterType.YL:
                        case RegisterType.YH:
                            if (i == RegisterType.H || i == RegisterType.L || i == RegisterType.BRACKET)
                                break;
                            StoreIYOpcode(((int)Codes.LD_B_B | ((int)i << 3)) + ((int)j - (int)RegisterType.YH + (int)RegisterType.H));
                            return token;
                        case RegisterType.B:
                        case RegisterType.C:
                        case RegisterType.D:
                        case RegisterType.E:
                        case RegisterType.H:
                        case RegisterType.L:
                        case RegisterType.BRACKET:
                        case RegisterType.A:
                            StoreOpcode((int)Codes.LD_B_B + ((int)i << 3) + (int)j);
                            return token;
                    }
                }
                #endregion ld r,n ld r,r ld r,xxh ld r,xxl
                #region ld xxh,n ld xxh,r ld xxl,n ld xxl,n
                if (i >= RegisterType.XH && i <= RegisterType.YL)
                {
                    if (i <= RegisterType.XL)
                    {
                        switch (j)
                        {
                            case RegisterType.Unknown:
                                StoreIXOpcode((int)Codes.LD_B_N + ((int)i - (int)RegisterType.XH + (int)RegisterType.H) << 3);
                                StoreByte(m);
                                return token;
                            case RegisterType.XH:
                            case RegisterType.XL:
                                StoreIXOpcode((int)Codes.LD_B_B + (((int)i - (int)RegisterType.XH + (int)RegisterType.H) << 3) + ((int)j - (int)RegisterType.XH + (int)RegisterType.H));
                                return token;
                            case RegisterType.B:
                            case RegisterType.C:
                            case RegisterType.D:
                            case RegisterType.E:
                            case RegisterType.A:
                                StoreIXOpcode((int)Codes.LD_B_B + (((int)i - (int)RegisterType.XH + (int)RegisterType.H) * 8) + (int)j);
                                return token;
                        }
                    }
                    else
                        switch (j)
                        {
                            case RegisterType.Unknown:
                                StoreIYOpcode((int)Codes.LD_B_N + ((int)i - (int)RegisterType.YH + (int)RegisterType.H) << 3);
                                StoreByte(m);
                                return token;
                            case RegisterType.YH:
                            case RegisterType.YL:
                                StoreIYOpcode((int)Codes.LD_B_B + (((int)i - (int)RegisterType.YH + (int)RegisterType.H) << 3) + ((int)j - (int)RegisterType.YH + (int)RegisterType.H));
                                return token;
                            case RegisterType.B:
                            case RegisterType.C:
                            case RegisterType.D:
                            case RegisterType.E:
                            case RegisterType.A:
                                StoreIYOpcode((int)Codes.LD_B_B + (((int)i - (int)RegisterType.YH + (int)RegisterType.H) * 8) + (int)j);
                                return token;
                        }
                }
                #endregion ld xxh,n ld xxh,r ld xxl,n ld xxl,n
                #region ld (nn),a ld (rr),a ld i,a ld r,a
                if (i == RegisterType.A)
                {
                    RegisterType j1 = j;
                    if ((int)j >= 99) j1 = j - 100;
                    switch (j1)
                    {
                        case RegisterType.Unknown:
                            StoreOpcode(Codes.LD_A_xNN);
                            StoreWord(m);
                            return token;
                        case RegisterType.BC:
                            StoreOpcode(Codes.LD_A_xBC);
                            return token;
                        case RegisterType.DE:
                            StoreOpcode(Codes.LD_A_xDE);
                            return token;
                        case RegisterType.I:
                            StoreEDOpcode(CodesED.LD_A_I);
                            return token;
                        case RegisterType.R:
                            StoreEDOpcode(CodesED.LD_A_R);
                            return token;
                    }
                }
                #endregion ld (nn),a ld (rr),a ld i,a ld r,a
                #region ld a,(nn) ld a,(rr) ld a,i ld a,r
                if (j == RegisterType.A)
                {
                    RegisterType i1 = i;
                    if ((int)i >= 99) i1 = i - 100;
                    switch (i1)
                    {
                        case RegisterType.Unknown:
                            StoreOpcode(Codes.LD_xNN_A);
                            StoreWord(n);
                            return token;
                        case RegisterType.BC:
                            StoreOpcode(Codes.LD_xBC_A);
                            return token;
                        case RegisterType.DE:
                            StoreOpcode(Codes.LD_xDE_A);
                            return token;
                        case RegisterType.I:
                            StoreEDOpcode(CodesED.LD_I_A);
                            return token;
                        case RegisterType.R:
                            StoreEDOpcode(CodesED.LD_R_A);
                            return token;
                    }
                }
                #endregion ld a,(nn) ld a,(rr) ld a,i ld a,r
                #region ld (xx+dis),reg and ld (xx+dis),n
                if (i == 100 + RegisterType.IX)
                {
                    if (j == RegisterType.Unknown)    
                    {
                        StoreIXOpcodeDisplacement((int)Codes.LD_xHL_N, n);
                        StoreByte(m);
                        return token;
                    }
                    else if (j >= RegisterType.B && j <= RegisterType.A) 
                    {
                        StoreIXOpcodeDisplacement((int)Codes.LD_xHL_B + (int)j, n);
                        return token;
                    }
                }
                if (i == 100 + RegisterType.IY)
                {
                    if (j == RegisterType.Unknown)
                    {
                        StoreIYOpcodeDisplacement((int)Codes.LD_xHL_N, n);
                        StoreByte(m);
                        return token;
                    }
                    else if (j >= RegisterType.B && j <= RegisterType.A) 
                    {
                        StoreIYOpcodeDisplacement((int)Codes.LD_xHL_B + (int)j, n);
                        return token;
                    }
                }
                #endregion ld (xx+dis),reg and ld (xx+dis),n
            }
            #endregion byte operations
            #region word operations
            #region Pseudo opcodes
            #region ld (hl),rr
            if (i == 100 + RegisterType.HL)
            {
                if (j ==RegisterType.BC)
                {
                    StoreOpcode(Codes.LD_xHL_C);
                    StoreOpcode(Codes.INC_HL);
                    StoreOpcode(Codes.LD_xHL_B);
                    StoreOpcode(Codes.DEC_HL);
                    return token;
                }
                if (j ==RegisterType.DE)
                {
                    StoreOpcode(Codes.LD_xHL_E);
                    StoreOpcode(Codes.INC_HL);
                    StoreOpcode(Codes.LD_xHL_D);
                    StoreOpcode(Codes.DEC_HL);
                    return token;
                }
            }
            #endregion ld (hl),rr
            #region ld rr,(hl)
            if (j == 100 + RegisterType.HL)      
            {
                if (i == RegisterType.BC)
                {
                    StoreOpcode(Codes.LD_C_xHL); StoreOpcode(Codes.INC_HL);
                    StoreOpcode(Codes.LD_B_xHL); StoreOpcode(Codes.DEC_HL);
                    return token;
                }
                if (i == RegisterType.DE)
                {
                    StoreOpcode(Codes.LD_E_xHL); StoreOpcode(Codes.INC_HL);
                    StoreOpcode(Codes.LD_D_xHL); StoreOpcode(Codes.DEC_HL);
                    return token;
                }
            }
            #endregion ld rr,(hl)
            #region ld rr,rr
            if (i >= RegisterType.BC && i < RegisterType.SP && j >= RegisterType.BC && j < RegisterType.SP)
            {
                n = (i - RegisterType.BC) * 8 + (j - RegisterType.BC);
                StoreOpcode((int)Codes.LD_B_B + n * 2);        // high byte
                StoreOpcode((int)Codes.LD_C_C + n * 2);        // low byte
                return token;
            }
            #endregion ld rr,rr
            #endregion Pseudo opcodes

            #endregion word operations

            if (i == RegisterType.IX)
            {
                i = RegisterType.HL;
                StoreOpcode(Codes.PFX_IX);
            }
            else if (i == RegisterType.IY)
            {
                i = RegisterType.HL;
                StoreOpcode(Codes.PFX_IY);
            }
            else if (j == RegisterType.IX)
            {
                j = RegisterType.HL;
                StoreOpcode(Codes.PFX_IX);
            }
            else if (j == RegisterType.IY)
            {
                j = RegisterType.HL;
                StoreOpcode(Codes.PFX_IY);
            }

            if ((int)i == 100 - 1)       // (NN)
            {
                switch (j)
                {
                    case RegisterType.BC: StoreEDOpcode(CodesED.LD_xNN_BC); StoreWord(n); return token;
                    case RegisterType.DE: StoreEDOpcode(CodesED.LD_xNN_DE); StoreWord(n); return token;
                    case RegisterType.HL: StoreOpcode(Codes.LD_xNN_HL); StoreWord(n); return token;
                    case RegisterType.SP: StoreEDOpcode(CodesED.LD_xNN_SP); StoreWord(n); return token;
                    default:
                        LogError(currentFileName, token,"Expecting Register Pair, BC, DE, HL or SP");
                        return token;
                }
            }

            if ((int)j == 100 - 1)       // (NN)
            {
                switch (i)
                {
                    case RegisterType.HL: StoreOpcode(Codes.LD_HL_xNN); StoreWord(m); return token;
                    case RegisterType.BC: StoreEDOpcode(CodesED.LD_BC_xNN); StoreWord(m); return token;
                    case RegisterType.DE: StoreEDOpcode(CodesED.LD_DE_xNN); StoreWord(m); return token;
                    case RegisterType.SP: StoreEDOpcode(CodesED.LD_SP_xNN); StoreWord(m); return token;
                    default:
                        LogError(currentFileName, token,"Expecting Register Pair, BC, DE, HL or SP");
                        return token;
                }
            }

            if (j == RegisterType.Unknown)            // NN
            {
                switch (i)
                {
                    case RegisterType.HL: StoreOpcode(Codes.LD_HL_NN); StoreWord(m); return token;
                    case RegisterType.BC: StoreOpcode(Codes.LD_BC_NN); StoreWord(m); return token;
                    case RegisterType.DE: StoreOpcode(Codes.LD_DE_NN); StoreWord(m); return token;
                    case RegisterType.SP: StoreOpcode(Codes.LD_SP_NN); StoreWord(m); return token;
                    default:
                        LogError(currentFileName, token,"Expecting Register Pair, BC, DE, HL or SP");
                        return token;
                }
                
            }

            if (i == RegisterType.SP && j == RegisterType.HL) { StoreOpcode(Codes.LD_SP_HL); return token; }

            LogError(currentFileName, null, "Invalid LD options");
            return token;
        }

    }
}
