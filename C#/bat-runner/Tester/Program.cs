using System.IO;

// ReSharper disable InconsistentNaming

namespace Tester {

internal static class Program {

    public static void Main() {
        const string pathFilePath = "../../file/test/path.txt";
        string path = readPath(pathFilePath);
        var args = new string[1];
        args[0] = path;
        BatRunner.Program.Main(args);
    }
    
    private static string readPath(string pathFilePath) {
        var fileStream = new FileStream(pathFilePath, FileMode.Open,
            FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        string content = streamReader.ReadToEnd().Trim();
        streamReader.Close();
        fileStream.Close();
        return content;
    }
}

}