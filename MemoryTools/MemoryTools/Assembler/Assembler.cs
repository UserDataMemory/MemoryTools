using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binarysharp.Assemblers.Fasm;

namespace MemoryTools.Assembler
{
    public static class Assembler
    {
        public static byte[] assemble(StringBuilder code, IntPtr address)
        {
            return assemble(code.ToString(),address);

        }
        public static byte[] assemble(string code, IntPtr address)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("use32");
            builder.AppendLine($"org 0x{address.ToInt32().ToString("X")}");
            builder.AppendLine(code);
            try
            {

                return FasmNet.Assemble(builder.ToString());
            }
            catch (FasmAssemblerException execption)
            {

                string[] lines = builder.ToString().Split('\n');
                throw new Exception($"An error occurred during FASM was assembling mnemonics. Error code: {(int)execption.ErrorCode} ({execption.ErrorCode}); Error line: {execption.ErrorLine}; Error offset: {execption.ErrorOffset}; Line: {lines[execption.ErrorLine - 1]}");
            }
        }
            public static byte[] jmp(IntPtr address,IntPtr AddressBase)
        {
            return assemble($"jmp {address}", AddressBase);
        }
 
    }
}
