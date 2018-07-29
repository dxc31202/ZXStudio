using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    partial class Assembler
    {
        static Token SBC(Token token)
        {
            if ((token = NextToken(token)) == null) return token;
            RegisterType registerType = GetRegisterType(token);
            if (registerType < RegisterType.F || registerType == RegisterType.Unknown)
                return RegMemOrByte(token, registerType, (int)Codes.SBC_B);

            if (registerType != RegisterType.HL && registerType != RegisterType.IX && registerType != RegisterType.IY)
            {
                LogError(currentFileName, token,"Expecting HL, IX or IY");
                return token.Next;
            }
            return HLRegSP(token, registerType, (int)CodesED.SBC_HL_BC);
        }
    }
}
