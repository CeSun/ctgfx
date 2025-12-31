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

                    foreach (var constructor in @class.Constructors)
                    {
                        if (constructor.Visibility != CppVisibility.Public)
                            continue;
                        if (constructor.Parameters.Any(parameter =>
                        {
                            if (parameter.Type.TypeKind == CppTypeKind.Unexposed)
                                return true;
                            return false;
                        }
                        ))
                            continue;
                        {
                            var (paramterTypeList, paramterList) = GetParameterList(constructor.Parameters);
                            sw.WriteLine();
                            sw.WriteLine($"void* {@namespace.Name}Create{@class.Name}{(constructor.Parameters.Count == 0 ? "" : constructor.Parameters.Count + "")}({paramterTypeList})");
                            sw.WriteLine("{");
                            sw.WriteLine($"\t return new {@namespace.Name}::{@class.Name}({paramterList});");
                            sw.WriteLine("}");
                            sw.WriteLine();
                        }
                    }

                    {
                        sw.WriteLine();
                        sw.WriteLine($"void {@namespace.Name}Destory{@class.Name}(void* obj)");
                        sw.WriteLine("{");
                        sw.WriteLine($"\tdelete ({@namespace.Name}::{@class.Name}*)obj;");
                        sw.WriteLine("}");
                        sw.WriteLine();
                    }

                    foreach (var method in @class.Functions)
                    {
                        if (method.Visibility != CppVisibility.Public)
                            continue;
                        if (method.IsStatic == true)
                            continue;
                        if (method.Parameters.Any(parameter =>
                        {
                            if (parameter.Type.TypeKind == CppTypeKind.Unexposed)
                                return true;
                            return false;
                        }
                        ))
                            continue;
                        Console.WriteLine($"\t\tGenerate method for {method.Name}");

                        var (paramterTypeList, paramterList) = GetParameterList(method.Parameters);
                        sw.WriteLine();
                        sw.WriteLine($"{method.ReturnType.FullName} {@namespace.Name}{@class.Name}_{method.Name}(void* obj{(paramterTypeList == "" ? "" : ", " + paramterTypeList)})");
                        sw.WriteLine("{");
                        if (method.ReturnType.FullName == "void")
                        {
                            sw.WriteLine($"\t (({@namespace.Name}::{@class.Name}*)obj)->{method.Name}({paramterList});");
                        }
                        else
                        {
                            sw.WriteLine($"\t return (({@namespace.Name}::{@class.Name}*)obj)->{method.Name}({paramterList});");
                        }
                        sw.WriteLine("}");
                        sw.WriteLine();

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

(string, string) GetParameterList(CppContainerList<CppParameter> parameters)
{
    string paramterTypeList = "";
    string paramterList = "";
    foreach (var paramter in parameters)
    {
        if (paramterTypeList != "")
            paramterTypeList += ", ";
        if (paramterList != "")
            paramterList += ", ";
        switch (paramter.Type.TypeKind)
        {
            case CppTypeKind.Enum:
            case CppTypeKind.Primitive:
                paramterTypeList += paramter.Type.FullName + " " + paramter.Name;
                break;
            case CppTypeKind.Pointer:
                paramterTypeList += "void* " + paramter.Name;
                break;
            case CppTypeKind.Reference:
                paramterTypeList += paramter.Type.FullName.Replace(" const&", " const*") + " " + paramter.Name;
                break;
            default:
                Console.WriteLine(paramter.Type.TypeKind + ": " + paramter.Type.FullName);
                paramterTypeList += paramter.Type.FullName + " " + paramter.Name;
                break;

        }
        paramterList += "(" + paramter.Type.FullName + ") " + paramter.Name;
    }

    return (paramterTypeList, paramterList);

}