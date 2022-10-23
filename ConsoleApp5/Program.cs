using ConsoleApp5.Entities;
using Microsoft.Win32;

String dir = String.Empty;
String content = String.Empty;
String option = String.Empty;
//
FileInfo fileInfo;
FileInfo fileItem;
string[] linesFile1;
string[] linesFile2;
//
Int32 equalFiles = 0;
Boolean exit = false;
//
List<FileIdentifier> fileEntries;
List<FileIdentifier> repeatFiles;

ConsoleProperties();

while (!exit)
{
    try
    {
        MenuOptions();

        int opcion = Convert.ToInt32(Console.ReadLine());

        switch (opcion)
        {
            case 1:
                Console.WriteLine("Directory cleaning /Downloads in process.");
                dir = GetDownloadFolderPath();
                fileEntries = new List<FileIdentifier>();
                GetFilesOfDirectory(fileEntries, dir);
                SeekAndDeleteDuplicateFiles();
                break;

            case 2:
                GetRouteOfDirectory();
                fileEntries = new List<FileIdentifier>();
                GetFilesOfDirectory(fileEntries, dir);
                SeekAndDeleteDuplicateFiles();
                break;

            case 3:
                Console.WriteLine("Exiting the application");
                exit = true;
                break;

            default:
                Console.WriteLine("Choose an option between 1 and 3");
                break;
        }

    }
    catch (FormatException e)
    {
        Console.WriteLine(e.Message);
    }
}

Console.ReadLine();


#region Methods

static void ConsoleProperties()
{
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Title = "n-f-d";
    Console.WriteLine("n-f-d (not a file duplicator)\n\n");
    Console.ResetColor();
    Console.WriteLine("Console application to delete duplicate files\n");
}

static void MenuOptions()
{
    Console.WriteLine("1. Clean /Download directory");
    Console.WriteLine("2. Enter path to the directory to be cleaned");
    Console.WriteLine("3. Exit");
    Console.WriteLine("Select one of the options");
}

string GetDownloadFolderPath()
{
    return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
}

void GetFilesOfDirectory(List<FileIdentifier> fileEntries, string dir)
{
    string[] entries = Directory.GetFiles(dir);

    for (int i = 0; i < entries.Length; i++)
    {
        fileEntries.Add(new FileIdentifier(i, entries[i], 0, entries[i], true));
    }
}

void GetRouteOfDirectory()
{
    Console.WriteLine("Enter the path to the directory to be scanned:");
    do
    {
        dir = Console.ReadLine();
        if (dir == "0")
        {
            return;
        }
    } while (dir == null || dir == String.Empty);
}

void SeekAndDeleteDuplicateFiles()
{
    for (int i = 0; i < fileEntries.Count; i++)
    {
        if (fileEntries[i].Exist)
        {
            repeatFiles = new List<FileIdentifier>();
            fileInfo = new FileInfo(fileEntries[i].FullName);
            linesFile1 = File.ReadAllLines(fileInfo.FullName);
            for (int j = 0; j < fileEntries.Count; j++)
            {
                if (fileEntries[j].Exist)
                {
                    fileItem = new FileInfo(fileEntries[j].FullName);
                    linesFile2 = File.ReadAllLines(fileItem.FullName);
                    // a file is considered equal to another if his size, extension and content is the same
                    if (fileInfo.Extension == fileItem.Extension && fileInfo.Length == fileItem.Length && EqualContent(linesFile1, linesFile2))
                    {
                        equalFiles++;
                        FileIdentifier newFileRepeat = new FileIdentifier(j, fileItem.Name, equalFiles, fileItem.FullName, true);
                        repeatFiles.Add(newFileRepeat);
                    }
                }
            }
            if (equalFiles > 1)
            {
                Console.WriteLine($"\nFile: {fileInfo.Name} found {equalFiles} times in the directory {dir}:");
                foreach (var item in repeatFiles)
                {
                    Console.WriteLine($"{item.Id}_ {item.Name}");
                }

                do
                {
                    Console.WriteLine($"\nEnter the number of the file you want to keep, the others will be deleted: ");
                    option = Console.ReadLine();
                } while (!repeatFiles.Select(x => x.Id).ToList().Contains(Convert.ToInt32(option)));

                foreach (var f in repeatFiles)
                {
                    if (f.Id != Convert.ToInt32(option))
                    {
                        File.Delete(f.FullName);
                        fileEntries[f.Id].Exist = false;
                    }
                }
            }
            equalFiles = 0;
        }
    }
    Console.WriteLine($"\nNo more duplicate files were found.\n");
}


bool EqualContent(string[] file1, string[] file2)
{
    if (file1.Length != file2.Length) return false;

    var fileLength = file1.Length;

    for (int i = 0; i < fileLength; i++)
    {
        var isEqual = file1[i].Equals(file2[i]);

        if (!isEqual) return false;
    }
    return true;
}

#endregion