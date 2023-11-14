using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;

namespace GetAssemblyInfo;

public static class AssemblyInfo
{
    internal static string GetCompactAssemblyInfoFromFilePath(string filePath)
    {
        var sb = new StringBuilder();
        var assemblyName = AssemblyName.GetAssemblyName(filePath);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.Name), assemblyName.Name, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.Version), assemblyName.Version, Environment.NewLine);
        try
        {
            var asm = AssemblyDefinition.ReadAssembly(filePath);
            sb.AppendFormat("ModuleVersionId: {0}{1}", asm.MainModule.Mvid, Environment.NewLine);
        }
        catch (Exception ex)
        {
            sb.AppendFormat("{0}{1}", ex, Environment.NewLine);
        }
        return sb.ToString();
    }

    internal static string GetFullAssemblyInfoFromFilePath(string filePath)
    {
        var sb = new StringBuilder();
        var assemblyName = AssemblyName.GetAssemblyName(filePath);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.Name), assemblyName.Name, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.Version), assemblyName.Version, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.Flags), assemblyName.Flags, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.CultureName), assemblyName.CultureName, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.FullName), assemblyName.FullName, Environment.NewLine);
        sb.AppendFormat("{0}: {1}{2}", nameof(assemblyName.ContentType), assemblyName.ContentType, Environment.NewLine);
        var processedAttributes = new List<string>();
        try
        {
            var asm = AssemblyDefinition.ReadAssembly(filePath);
            sb.AppendFormat("ModuleVersionId: {0}{1}", asm.MainModule.Mvid, Environment.NewLine);
            foreach (var attr in asm.MainModule.GetCustomAttributes())
            {
                try
                {
                    if (processedAttributes.Contains(attr.AttributeType.FullName))
                    {
                        continue;
                    }

                    processedAttributes.Add(attr.AttributeType.FullName);

                    if (attr.HasProperties)
                    {
                        foreach (var prop in attr.Properties)
                        {
                            sb.AppendFormat("{0}.{1}: {2}{3}", attr.AttributeType.Name, prop.Name, prop.Argument.Value,
                                Environment.NewLine);
                        }
                    }

                    if (attr.HasConstructorArguments)
                    {
                        var arguments = new List<string>();
                        for (var i = 0; i < attr.ConstructorArguments.Count; i++)
                        {
                            var ctrArg = attr.Constructor.Parameters[i];
                            if (string.IsNullOrEmpty(ctrArg.Name))
                            {
                                arguments.Add(attr.ConstructorArguments[i].Value?.ToString() ?? "(null)");
                            }
                            else
                            {
                                arguments.Add(ctrArg.Name + ": " +
                                              (attr.ConstructorArguments[i].Value?.ToString() ?? "(null)"));
                            }
                        }

                        sb.AppendFormat("{0}: {1}{2}", attr.AttributeType.Name, string.Join(", ", arguments),
                            Environment.NewLine);
                    }
                }
                catch
                {
                    //
                }
            }
        }
        catch (Exception ex)
        {
            sb.AppendFormat("{0}{1}", ex, Environment.NewLine);
        }

        return sb.ToString();
    }

    [UnmanagedCallersOnly(EntryPoint = "get_info")]
    internal static IntPtr GetInfo(IntPtr pFilepath)
    {
        if (Marshal.PtrToStringAnsi(pFilepath) is { } filepath)
        {
            var result = GetCompactAssemblyInfoFromFilePath(filepath);
            return Marshal.StringToHGlobalAnsi(result);
        }

        return IntPtr.Zero;
    }
}