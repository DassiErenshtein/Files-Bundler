using System.Collections.Generic;
using System.CommandLine;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
//רשימת שפות תכנות שרק מהן אפשר להשתמש בקובץ
string[] langs =["java", "css", "html","ts","js","c","scss","node","sql", "csproj","cs","json"];
//פקודות להוספה לפרויקט
var rootCommand = new RootCommand("Root command for file bunder CLI");
var bundleCommand = new Command("bundle", "bundle code files to a single file");
var rspCommand = new Command("create-rsp", "answer questions") ;
//BUNDLEאופציות של ה
var outputOption = new Option<FileInfo>("--output", "File path and name") { IsRequired = true};
var languageOption = new Option<List<string>>("--language", "Which languages do you want to connect")
{
    IsRequired = true,
    AllowMultipleArgumentsPerToken = true
};
var noteOption = new Option<bool>("--note", "To write a source or not") { };
var sortOption = new Option<bool>("--sort", "To sort the files according to alphabetic sort-false or languages-true") { };
var deleteOption = new Option<bool>("--delete", "Remove empty lines") { };
var autherOption = new Option<string>("--auther", "To write the name of the auther") { };
//הוספת אליאסים
outputOption.AddAlias("-o");
languageOption.AddAlias("-l");
noteOption.AddAlias("-n");
sortOption.AddAlias("-s");
deleteOption.AddAlias("-d");
autherOption.AddAlias("-a");
//הוספת האופשנים לפקודה
bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(deleteOption);
bundleCommand.AddOption(autherOption);
//בעת ריצה, הוא יקבל את כל הארגומנטים ויפעל לפיהם לקיבוץ הכל לקובץ טקסט אחד.
bundleCommand.SetHandler((FileInfo output, List<string> languages, bool note,bool sort,bool delete,string auther) =>
{
    var newFile = File.Create("C");
    try
    {
        //יוצר קובץ ושומר אותו בתוך משתנה
        newFile = File.Create(output.FullName); 
        Console.WriteLine("file create");
    }
    catch
    {
        Console.WriteLine("error, try again");
    }
        List<string> contens1 = new List<string>();
        if (languages.Contains("all")) {
            foreach (string file in Directory.GetFiles(output.FullName.Substring(0, output.FullName.LastIndexOf(@"\")), "*.*", SearchOption.AllDirectories))
            {
                var folders = file.Split("\\");
                if (!folders.Contains("bin")&&!folders.Contains("Debug"))
                if (langs.Contains(Path.GetFileName(file).Substring(Path.GetFileName(file).LastIndexOf(".") + 1)) == true)
                {
                        contens1.Add(file);
                }
            }
        }
        else
            foreach (var language in languages)
            {
                if(langs.Contains(language))
                    foreach (string file in Directory.GetFiles(output.FullName.Substring(0, output.FullName.LastIndexOf(@"\")), "*." + language, SearchOption.AllDirectories))
                    {
                        var folders = file.Split("\\");
                        if (!folders.Contains("bin") && !folders.Contains("Debug"))
                        {
                            contens1.Add(file);
                            Console.WriteLine(true);
                        }
                    }
                else
                    Console.WriteLine(language+ " is not a programming language");
            }
        
        if (sort&&sort==true)
            contens1.Sort((x,y) => Path.GetFileName(x).Substring(Path.GetFileName(x).IndexOf(".")+1)
            .CompareTo(Path.GetFileName(y).Substring(Path.GetFileName(y).IndexOf(".")+1)));
        string contens = "";
        foreach(var file in contens1)
        {
            if (note && note==true)
                contens += file + "\n" + Path.GetFileName(file)+"\n\n";
            string c1 = File.ReadAllText(file);
            if (delete&&delete==true)
                c1=Regex.Replace(c1, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
            contens += c1;
        }

        using (StreamWriter writer = new StreamWriter(newFile))
        {
            writer.WriteLine(contens);
        }
}, outputOption, languageOption,noteOption,sortOption, deleteOption,autherOption);
rspCommand.SetHandler(() =>
{
    Console.WriteLine("What is your path of the file? (if you want this path, write just the name) ");
    FileInfo output = new FileInfo(Console.ReadLine());
    string end = Path.GetFileName(output.FullName).Substring(Path.GetFileName(output.FullName).IndexOf(".") + 1);
    while (!end.Equals("txt"))
    {
        Console.WriteLine("the file needs to end with .txt,What is your path of the file?");
        output = new FileInfo(Console.ReadLine());
        end= Path.GetFileName(output.FullName).Substring(Path.GetFileName(output.FullName).IndexOf(".") + 1);
    }
    Console.WriteLine(Directory.GetParent(output.FullName).Exists);
    while (!Directory.GetParent(output.FullName).Exists)
    {
        Console.WriteLine("the path is invalid, What is your path of the file?");
        output = new FileInfo(Console.ReadLine());
        while (!Path.GetFileName(output.FullName).Substring(Path.GetFileName(output.FullName).IndexOf(".") + 1).Equals("txt"))
        {
            Console.WriteLine("the file needs to end with .txt,What is your path of the file?");
            output = new FileInfo(Console.ReadLine());
        }
    }
    Console.WriteLine("Which languages do you want to use?");
    string[] languages1 = Console.ReadLine().Split(" ");
    List<string> languages = languages1.ToList();
    bool a = false;
    if (!languages.Contains("all"))
    foreach (string language in languages)
    {
        if (!langs.Contains(language))
        {
            Console.WriteLine(language + "is not a programming language");
            a = true;
        }
    }
    while (a==true) {
        a = false;
        Console.WriteLine("insert again the languages");
        languages1 = Console.ReadLine().Split(" ");
        languages = languages1.ToList();
        if (!languages.Contains("all"))
        {
            foreach (string language in languages)
            {
                if (!langs.Contains(language))
                {
                    Console.WriteLine(language + "is not a programming language");
                    a = true;
                }
            }
        }
    }
    Console.WriteLine("Do you want to write path and title of all the files? (Y/N)");
    string note1 = Console.ReadLine();
    bool note = false;
    if (note1.Equals("Y"))
        note = true;
    else while (!note1.Equals("N")&&note==false)
    {
        Console.WriteLine("invalid inputed ,do you want to sort according to file type? (Y/N)");
        note1 = Console.ReadLine();
        note = false;
        if (note1.Equals("Y"))
            note = true;
    }
    Console.WriteLine("do you want to sort according to file type? (Y/N)");
    string sort1 = Console.ReadLine();
    bool sort = false;
    if (sort1.Equals("Y"))
        sort = true;
    else while (!sort1.Equals("N")&&sort==false)
    {
        Console.WriteLine("invalid inputed ,do you want to sort according to file type? (Y/N)");
        sort1 = Console.ReadLine();
        sort = false;
        if (sort1.Equals("Y"))
            sort = true;
    }
    Console.WriteLine("Do you want to delete empty lines from your code? (Y/N)");
    string delete1 = Console.ReadLine();
    bool delete = false;
    if (delete1.Equals("Y"))
        delete = true;
    else while (!delete1.Equals("N")&&delete==false)
    {
        Console.WriteLine("invalid inputed ,do you want to delete empty lines? (Y/N)");
        delete1 = Console.ReadLine();
        delete = false;
        if (delete1.Equals("Y"))
            delete = true;
    }
    Console.WriteLine("write author name");
    string author =""+ Console.ReadLine();
    string str = " bundle "+"-o "+output+" -l "+String.Join(" ",languages)+" -n " +note+" -s "+sort+" -d "+delete+" -a "+author;
    File.WriteAllText(output.FullName.Substring(0, output.FullName.LastIndexOf(@"\")+1)+"bundle.rsp",str);
});

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(rspCommand);
rootCommand.InvokeAsync(args);
