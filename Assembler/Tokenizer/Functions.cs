using System;
using System.Collections.Generic;
using System.Text;

public static class Functions
{
    public enum BaseType
    {
        Binary = 2,
        Base4 = 4,
        Octal = 8,
        Decimal = 10,
        Hex = 16,
    }

    public static int BaseToDecimal(string value, BaseType numberBase)
    {
        int returnValue = 0;
        int multiplier = 1;
        value = value.ToUpper();
        for (int i = value.Length - 1; i >= 0; i--)
        {
            int part = value[i] ^ 0x30;
            if (part > 0x09)
                part -= 0x67;
            returnValue += (multiplier * part);
            multiplier *= (int)numberBase;
        }
        return returnValue;
    }

    public static string DecimalToBase(int value, BaseType numberBase)
    {
        StringBuilder returnValue = new StringBuilder();
        while (value > 0)
        {
            int part = value & ((int)numberBase - 1);
            if (part > 0x9)
                part += 0x7;
            value /= (int)numberBase;
            returnValue.Insert(0, (char)(part + 0x30));
        }
        if (numberBase == BaseType.Binary)
            return ("00000000" + returnValue.ToString()).Substring(("00000000" + returnValue).Length - 8, 8);
        else
            return returnValue.ToString();
    }
}
