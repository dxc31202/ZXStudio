using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token ADD(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            RegisterType registerType = GetRegisterType(token);
            if (registerType < RegisterType.F || registerType == RegisterType.Unknown)
                return RegMemOrByte(token, registerType, (int)Codes.ADD_B);

            if (registerType != RegisterType.HL && registerType != RegisterType.IX && registerType != RegisterType.IY)
            {
                LogError(currentFileName, token,"Expecting HL, IX or IY");
                return token.Next;
            }
            if ((token = NextToken(token)) == null) return token;
            if (CheckComma(token, false)) if ((token = NextToken(token)) == null) return token;
            if (registerType == RegisterType.IX) StoreOpcode(0xDD);
            if (registerType == RegisterType.IY) StoreOpcode(0xFD);

            switch (token.Value.ToUpper())
            {
                case "BC": StoreOpcode(Codes.ADD_HL_BC); return token.Next;
                case "DE": StoreOpcode(Codes.ADD_HL_DE); return token.Next;
                case "SP": StoreOpcode(Codes.ADD_HL_SP); return token.Next;
                case "HL": StoreOpcode(Codes.ADD_HL_HL); return token.Next;
                case "IX": StoreOpcode(Codes.ADD_HL_HL); return token.Next;
                case "IY": StoreOpcode(Codes.ADD_HL_HL); return token.Next;
            }
            LogError(currentFileName, token,"Expecting BC, DE, " + registerType.ToString() + " or SP");
            return token.Next;
        }
    }
}
