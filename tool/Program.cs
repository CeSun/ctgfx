using CppAst;

var outputPath = "../../../../../src";

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
    Console.WriteLine($"Generate file: {files[i]}");

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
    else
    {

        var rp = Path.GetRelativePath(includeDir.FullName + "/tgfx", files[i]);

        var outputFilePath = outputPath + "/" + rp;

        var f = Path.GetFileName(outputFilePath);
        var ext = Path.GetExtension(outputFilePath);
        if (Directory.Exists(outputFilePath.Replace(f, "")) == false)
        {
            Directory.CreateDirectory(f);
        }
        using (var sw = new StreamWriter(outputFilePath.Replace(ext, ".cpp")))
        {
            sw.WriteLine("// hello");

            sw.WriteLine($"#include <tgfx/{rp.Replace("\\", "/")}>");

            foreach (var @namespace in cppCompilation.Namespaces)
            {
                foreach (var @class in @namespace.Classes)
                {
                    if (@class.SourceFile != files[i])
                        continue;
                    Console.WriteLine($"\tGenerate class:  {@class.FullName}");
                    foreach (var method in @class.Functions)
                    {
                        if (method.Visibility != CppVisibility.Public)
                            continue;

                        sw.WriteLine($"// {@namespace.Name}{@class.Name}_{method.Name}()");
                        Console.WriteLine($"\t\tGenerate method for {method.Name}");
                    }

                    foreach (var constructor in @class.Constructors)
                    {
                        if (constructor.Visibility != CppVisibility.Public)
                            continue;

                        sw.WriteLine($"// {@namespace.Name}Create{@class.Name}()");
                    }
                }
            }
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