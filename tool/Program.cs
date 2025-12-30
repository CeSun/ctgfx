using CppAst;

var options = new CppParserOptions() { AdditionalArguments = { "-std=c++17", "-DNOMINMAX", "-D_USE_MATH_DEFINES" } };

options.IncludeFolders.Add("../../../../../third_party/tgfx/include");
options.IncludeFolders.Add("../../../../../third_party/tgfx/src");
options.IncludeFolders.Add("../../../../../third_party/tgfx/vendor/angle/include");

var includeDir = new DirectoryInfo("../../../../../third_party/tgfx/include");


List<string> files = new List<string>();

processDirectory(includeDir);

int start = 0;

for (int i = start; i < files.Count; i++)
{
    var cppCompilation = CppParser.ParseFile(files[i], options);
    if (cppCompilation.HasErrors)
    {
        Console.WriteLine("index: " + i);
        Console.WriteLine(files[i]);
        foreach (var error in cppCompilation.Diagnostics.Messages)
        {
            Console.WriteLine(error);
        }
    }
}


void processDirectory(DirectoryInfo directoryInfo)
{
    foreach (var file in directoryInfo.GetFiles())
    {
        if (file.FullName.Contains("cgl"))
            continue;
        if (file.FullName.Contains("eagl"))
            continue;
        if (file.FullName.Contains("qt"))
            continue;
        if (file.FullName.Contains("webgl"))
            continue;
        if (file.FullName.Contains("android"))
            continue;
        if (file.FullName.Contains("apple"))
            continue;
        if (file.FullName.Contains("ohos"))
            continue;
        if (file.FullName.Contains("web"))
            continue;
        if (file.FullName.Contains("BytesKey.h"))
            continue;
        files.Add(file.FullName);
    }
    foreach (var dir in directoryInfo.GetDirectories())
    {
        processDirectory(dir);
    }
}