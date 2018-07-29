using System;
[Flags]
internal enum OpcodeProperty
{
    None = 0x000,
    Branch = 0x001,
    Call = 0x002,
    Conditional = 0x004,
    Port = 0x008,
    Indirect = 0x010,
    Restart = 0x020,
    Return = 0x040,
    Terminal = 0x080,
    Indexed = 0x100,
}
