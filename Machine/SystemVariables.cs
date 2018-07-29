using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXStudio
{
    static class SystemVariables
    {
        public const int KSTATE = 23552;	// N8	Used in reading the keyboard.
        public const int LASTK = 23560;	// Nl	Stores newly pressed key.
        public const int REPDEL = 23561;	// 1	Time (in 50ths of a second in 60ths of a second in N. America) that a key must be held down before it repeats. This starts off at 35, but you can POKE in other values.
        public const int REPPER = 23562;	// 1	Delay (in 50ths of a second in 60ths of a second in N. America) between successive repeats of a key held down: initially 5.
        public const int DEFADD = 23563;	// N2	Address of arguments of user defined function if one is being evaluated; otherwise 0.
        public const int KDATA = 23565;	// Nl	Stores 2nd byte of colour controls entered from keyboard .
        public const int TVDATA = 23566;	// N2	Stores bytes of coiour, AT and TAB controls going to television.
        public const int STRMS = 23568;	// X38	Addresses of channels attached to streams.
        public const int CHARS = 23606;	// 2	256 less than address of character set (which starts with space and carries on to the copyright symbol). Normally in ROM, but you can set up your own in RAM and make CHARS point to it.
        public const int RASP = 23608;	// 1	Length of warning buzz.
        public const int PIP = 23609;	// 1	Length of keyboard click.
        public const int ERRNR = 23610;	// 1	1 less than the report code. Starts off at 255 (for 1) so PEEK 23610 gives 255.
        public const int FLAGS = 23611;	// X1	Various flags to control the BASIC system.
        public const int TVFLAG = 23612;	// X1	Flags associated with the television.
        public const int ERRSP = 23613;	// X2	Address of item on machine stack to be used as error return.
        public const int LISTSP = 23615;	// N2	Address of return address from automatic listing.
        public const int MODE = 23617;	// N1	Specifies K, L, C. E or G cursor.
        public const int NEWPPC = 23618;	// 2	Line to be jumped to.
        public const int NSPPC = 23620;	// 1	Statement number in line to be jumped to. Poking first NEWPPC and then NSPPC forces a jump to a specified statement in a line.
        public const int PPC = 23621;	// 2	Line number of statement currently being executed.
        public const int SUBPPC = 23623;	// 1	Number within line of statement being executed.
        public const int BORDCR = 23624;	// 1	Border colour * 8; also contains the attributes normally used for the lower half of the screen.
        public const int EPPC = 23625;	// 2	Number of current line (with program cursor).
        public const int VARS = 23627;	// X2	Address of variables.
        public const int DEST = 23629;	// N2	Address of variable in assignment.
        public const int CHANS = 23631;	// X2	Address of channel data.
        public const int CURCHL = 23633;	// X2	Address of information currently being used for input and output.
        public const int PROG = 23635;	// X2	Address of BASIC program.
        public const int NXTLIN = 23637;	// X2	Address of next line in program.
        public const int DATADD = 23639;	// X2	Address of terminator of last DATA item.
        public const int ELINE = 23641;	// X2	Address of command being typed in.
        public const int KCUR = 23643;	// 2	Address of cursor.
        public const int CHADD = 23645;	// X2	Address of the next character to be interpreted: the character after the argument of PEEK, or the NEWLINE at the end of a POKE statement.
        public const int XPTR = 23647;	// 2	Address of the character after the ? marker.
        public const int WORKSP = 23649;	// X2	Address of temporary work space.
        public const int STKBOT = 23651;	// X2	Address of bottom of calculator stack.
        public const int STKEND = 23653;	// X2	Address of start of spare space.
        public const int BREG = 23655;	// N1	Calculator's b register.
        public const int MEM = 23656;	// N2	Address of area used for calculator's memory. (Usually MEMBOT, but not always.)
        public const int FLAGS2 = 23658;	// 1	More flags.
        public const int DFSZ = 23659;	// X1	The number of lines (including one blank line) in the lower part of the screen.
        public const int STOP = 23660;	// 2	The number of the top program line in automatic listings.
        public const int OLDPPC = 23662;	// 2	Line number to which CONTINUE jumps.
        public const int OSPCC = 23664;	// 1	Number within line of statement to which CONTINUE jumps.
        public const int FLAGX = 23665;	// N1	Various flags.
        public const int STRLEN = 23666;	// N2	Length of string type destination in assignment.
        public const int TADDR = 23668;	// N2	Address of next item in syntax table (very unlikely to be useful).
        public const int SEED = 23670;	// 2	The seed for RND. This is the variable that is set by RANDOMIZE.
        public const int FRAMES = 23672;	// 3	3 byte (least significant first), frame counter. Incremented every 20ms. See Chapter 18.
        public const int UDG = 23675;	// 2	Address of 1st user defined graphic You can change this for instance to save space by having fewer user defined graphics.
        public const int COORDSX = 23677;	// 1	x-coordinate of last point plotted.
        public const int COORDSY = 23678;	// 1	y-coordinate of last point plotted.
        public const int PPOSN = 23679;	// 1	33 column number of printer position
        public const int PRCC = 23680;	// X2	Full address of next position for LPRINT to print at (in ZX printer buffer). Legal values 5B00 - 5B1F. [Not used in 128K mode or when certain peripherals are attached]
        public const int ECHOE = 23682;	// 2	33 column number and 24 line number (in lower half) of end of input buffer.
        public const int DFCC = 23684;	// 2	Address in display file of PRINT position.
        public const int DFCCL = 23686;	// 2	Like DF CC for lower part of screen.
        public const int SPOSN = 23688;	// X1	33 column number for PRINT position
        public const int SPOSN1 = 23689;	// X1	24 line number for PRINT position.
        public const int SPOSNL = 23690;	// X2	Like S POSN for lower part
        public const int SCRCT = 23692;	// 1	Counts scrolls: it is always 1 more than the number of scrolls that will be done before stopping with scroll? If you keep poking this with a number bigger than 1 (say 255), the screen will scroll on and on without asking you.
        public const int ATTRP = 23693;	// 1	Permanent current colours, etc (as set up by colour statements).
        public const int MASKP = 23694;	// 1	Used for transparent colours, etc. Any bit that is 1 shows that the corresponding attribute bit is taken not from ATTR P, but from what is already on the screen.
        public const int ATTRT = 23695;	// N1	Temporary current colours, etc (as set up by colour items).
        public const int MASKT = 23696;	// N1	Like MASK P, but temporary.
        public const int PFLAG = 23697;	// 1	More flags.
        public const int MEMBOT = 23698;	// N30	Calculator's memory area; used to store numbers that cannot conveniently be put on the calculator stack.
        public const int NMIADD = 23728;	// 2	This is the address of a user supplied NMI address which is read by the standard ROM when a peripheral activates the NMI. Probably intentionally disabled so that the effect is to perform a reset if both locations hold zero, but do nothing if the locations hold a non-zero value. Interface 1's with serial number greater than 87315 will initialize these locations to 0 and 80 to allow the RS232 "T" channel to use a variable line width. 23728 is the current print position and 23729 the width - default 80.
        public const int RAMTOP = 23730;	// 2	Address of last byte of BASIC system area.
        public const int PRAMT = 23732;	// 2	Address of last byte of physical RAM.

    }
}
