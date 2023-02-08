using System.Reflection;
using System.Text;
using Mono.Cecil;

if (args?.Length < 1)
{
    Console.WriteLine("You must specify a filename as an argument");
    Environment.ExitCode = 1;
    return;
}

var filePath = args?[0];
if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
{
    Console.WriteLine("File '{0}' doesn't exist.", filePath);
    Environment.ExitCode = 1;
    return;
}

var assemblyName = AssemblyName.GetAssemblyName(filePath);
Console.WriteLine("{0}: {1}", nameof(assemblyName.Name), assemblyName.Name);
Console.WriteLine("{0}: {1}", nameof(assemblyName.Version), assemblyName.Version);
Console.WriteLine("{0}: {1}", nameof(assemblyName.Flags), assemblyName.Flags);
Console.WriteLine("{0}: {1}", nameof(assemblyName.CultureName), assemblyName.CultureName);
Console.WriteLine("{0}: {1}", nameof(assemblyName.FullName), assemblyName.FullName);
Console.WriteLine("{0}: {1}", nameof(assemblyName.ContentType), assemblyName.ContentType);
var processedAttributes = new List<string>();
try
{
    var asm = AssemblyDefinition.ReadAssembly(filePath);
    Console.WriteLine("ModuleVersionId: {0}", asm.MainModule.Mvid);
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
                    Console.WriteLine("{0}.{1}: {2}", attr.AttributeType.Name, prop.Name, prop.Argument.Value);
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
                        arguments.Add( ctrArg.Name + ": " + (attr.ConstructorArguments[i].Value?.ToString() ?? "(null)"));
                    }
                }
                
                Console.WriteLine("{0}: {1}", attr.AttributeType.Name, string.Join(", ", arguments));
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
    Console.WriteLine(ex);
}
