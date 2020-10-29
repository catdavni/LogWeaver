using Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Diagnostics;
using System.Linq;

namespace Patcher
{
    class Program
    {
        static void Main(string[] arg)
        {
            var assemblyPath = arg[0] ?? string.Empty;
            Console.WriteLine($"{nameof(assemblyPath)} {assemblyPath}");

            using (var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters(ReadingMode.Immediate) { ReadWrite = true, InMemory = true }))
            {
                var types = assembly.Modules.SelectMany(m => m.Types.ToArray()).ToArray();
                var typesWithAttribute = types.Where(t => t.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(LoggingAttribute).FullName)).ToArray();

                var publicMethods = typesWithAttribute.SelectMany(t => t.Methods.Where(m => m.IsPublic && !m.IsConstructor)).ToArray();

                var traceMethodInfo = typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) });
                var traceMethodReference = assembly.MainModule.ImportReference(traceMethodInfo);

                foreach (var m in publicMethods)
                {
                    m.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, m.FullName));
                    m.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, traceMethodReference));
                }
                assembly.Write(assemblyPath);
            }
        }
    }
}
