using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day07;

[SolverMap(7)]
public class Day07 : Solver
{
    public struct FileDescriptor
    {
        public string Filename;
        public string[] Path;
        public int Size;
    }

    public object PartA()
    {
        var folderMap = ParseFileStructure().FolderSizes;
        return folderMap.Where(kv => kv.Value <= 100000).Sum(kv => kv.Value);
    }

    public object PartB()
    {
        var folderMap = ParseFileStructure().FolderSizes;
        var freeSpace = 70000000 - folderMap["/"];
        var deleteThreshold = 30000000 - freeSpace;
        return folderMap.OrderBy(kv => kv.Value).First(kv => kv.Value >= deleteThreshold).Value;
    }

    public (Dictionary<string, int> FolderSizes, FileDescriptor[] FileDescriptors) ParseFileStructure()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay07.txt");
        var filePattern = @"(\d*) (.*)";

        var files = new List<FileDescriptor>();
        var currentFolder = new Stack<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("$ cd"))
            {
                var newFolder = line.Substring(5);
                if (newFolder == "..")
                {
                    currentFolder.Pop();
                }
                else
                {
                    currentFolder.Push(newFolder);
                }

                continue;
            }

            // ignorer les lignes intermediaires qui ne sont pas des fichiers
            if (line.StartsWith("$") || line.StartsWith("dir"))
            {
                continue;
            }

            var matches = Regex.Matches(line, filePattern);
            foreach (Match match in matches)
            {
                var path = currentFolder.Reverse().ToArray();
                var descriptor = new FileDescriptor
                {
                    Size = int.Parse(match.Groups[1].Value),
                    Filename = match.Groups[2].Value,
                    Path = path
                };
                files.Add(descriptor);
            }
        }

        // reconstruire l'aborescence a partir des fichiers
        var reverseMap = new Dictionary<string, int>();
        foreach (var file in files)
        {
            for (int i = 0; i < file.Path.Length; i++)
            {
                var subpath = "/" + string.Join('/', file.Path.Take(i + 1).Skip(1));
                if (!reverseMap.ContainsKey(subpath))
                {
                    reverseMap[subpath] = 0;
                }

                reverseMap[subpath] += file.Size;
            }
        }

        return (reverseMap, files.ToArray());
    }

}