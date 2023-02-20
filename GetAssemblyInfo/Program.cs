using GetAssemblyInfo;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("You must specify a filename as an argument");
            Environment.ExitCode = 1;
            return;
        }

        var filePath = args[0];
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            Console.WriteLine("File '{0}' doesn't exist.", filePath);
            Environment.ExitCode = 1;
            return;
        }

        if (args.Length > 1 && args[1] == "--full")
        {
            Console.Write(AssemblyInfo.GetFullAssemblyInfoFromFilePath(filePath));
        }
        else
        {
            Console.Write(AssemblyInfo.GetCompactAssemblyInfoFromFilePath(filePath));
        }
    }
}
