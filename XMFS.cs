using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace XMFS
{
    public class Program
    {
        static string filename;
        public static XmlElement workingElement;
        static string workingString = "/";
        public static XmlDocument mainDoc;
        const string PROMPT = " $ ";
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: no input file specified.");
                return;
            }
            filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine("Error: Unable to access file {0}.", filename);
                return;
            }
            mainDoc = new XmlDocument();
            mainDoc.Load(filename);
            workingElement = (XmlElement)mainDoc.DocumentElement.ChildNodes.Item(1);
            DCD(SearchByName("home"));
            while (true)
            {
                Console.Write(workingString+PROMPT);
                string input = Console.ReadLine();
                if (input.Split(' ').First().ToUpper() == "QUIT" || input.Split(' ').First().ToUpper() == "LOGOUT" || input.Split(' ').First().ToUpper() == "EXIT")
                {
                    mainDoc.Save(filename);
                    return;
                }
                switch (input.Split(' ').First().ToUpper())
                {
                    case "LS":
                        for (int e = 0; e < workingElement.ChildNodes.Count; e++)
                        {
                            XmlElement cElm = (XmlElement)workingElement.ChildNodes.Item(e);
                            if (cElm.LocalName == "folder")
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            Console.Write("{0}:{1}\t", e, cElm.GetAttribute("name"));
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.Write("\n");
                        break;

                    case "MKDIR":
                        string dirName = input.Split(' ')[1];
                        bool isIntName = true;
                        try
                        {
                            Int32.Parse(dirName);
                        }
                        catch (System.Exception)
                        {
                            isIntName = false;
                        }
                        if (isIntName)
                        {
                            Console.WriteLine("Error: names cannot be integers.");
                            break;
                        }
                        XmlElement newFolder = mainDoc.CreateElement("folder");
                        newFolder.SetAttribute("name", dirName);
                        workingElement.AppendChild(newFolder);
                        break;

                    case "RMDIR":
                        XmlElement folder = SearchByID(input.Split(' ')[1]);
                        if(folder == null)
                        {
                            Console.WriteLine("Error: folder does not exist.");
                            break;
                        }
                        workingElement.RemoveChild(folder);
                        break;

                    case "RM":
                        XmlElement file = SearchByID(input.Split(' ')[1]);
                        if(file == null)
                        {
                            Console.WriteLine("Error: file does not exist.");
                            break;
                        }
                        workingElement.RemoveChild(file);
                        break;

                    case "CD":
                        if (input.Split(' ')[1] == "..")
                        {
                            DCU();
                            break;
                        }
                        XmlElement nWDF = SearchByID(input.Split(' ')[1]);
                        if (nWDF == null)
                        {
                            Console.WriteLine("Error: folder does not exist.");
                            break;
                        }
                        DCD(nWDF);
                        break; 

                    case "ED":
                        TextEditor.Run(input.Split(' ')[1]);
                        break;

                    default:
                        Console.WriteLine("Error: Unknown command: \"{0}\"", input);
                        break;
                }
            }
        }

        #region cd functions
        static bool ChangeElement(XmlNode newWorkingElement)
        {
            try
            {
                workingElement = (XmlElement)newWorkingElement;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error: Invalid node type.");
                return false;
            }
            if (workingElement.LocalName == "folder")
            {
                return true;
            }
            Console.WriteLine("Error: Not a folder.");
            return false;
        }

        static bool DCD(XmlNode newElement)
        {
            if (ChangeElement(newElement))
            {
                XmlElement cElm = (XmlElement)newElement;
                workingString = workingString + cElm.GetAttribute("name") + "/";
                return true;
            }
            return false;
        }

        static bool DCU()
        {
            if (workingString == "/")
            {
                return false;
            }
            workingElement = (XmlElement)workingElement.ParentNode;
            do
            {
                workingString = workingString.Remove(workingString.Length - 1);
            } while (!workingString.EndsWith("/"));
            return true;
        }
        #endregion

        public static XmlElement SearchByName(string elementName)
        {
            for (int e = 0; e < workingElement.ChildNodes.Count; e++)
            {
                XmlElement cElm = (XmlElement)workingElement.ChildNodes.Item(e);
                if (cElm.GetAttribute("name") == elementName)
                {
                    return cElm;
                }
            }
            return null;
        }

        public static XmlElement SearchByID(string elementID)
        {
            try
            {
                XmlElement cElm = (XmlElement)workingElement.ChildNodes.Item(Int32.Parse(elementID));
                elementID = cElm.GetAttribute("name");
            }
            catch (System.Exception)
            {
            }
            return SearchByName(elementID);
        }
    }

    class TextEditor
    {
        static string[] lines;
        static int lineCount;
        public static void Run(string filename)
        {
            XmlElement file = Program.SearchByID(filename);
            if (file == null)
            {
                CreateFile(filename);
            }
            else
            {
                LoadFile(file.GetAttribute("name"), file.InnerText);
            }
            return;
        }

        static void CreateFile(string filename)
        {
            try
            {
                Int32.Parse(filename);
            }
            catch (System.Exception)
            {
                XmlElement newFile = Program.mainDoc.CreateElement("file");
                newFile.SetAttribute("name", filename);
                newFile.SetAttribute("exe", "false");
                Program.workingElement.AppendChild(newFile);
                LoadFile(filename, null);
                return;
            }
            Console.WriteLine("Error: filenames cannot be integers.");
            return;
        }

        static void LoadFile(string filename, string fileText)
        {
            XmlElement file = Program.SearchByName(filename);
            string[] rawLines = fileText.Split('\n');
            lines = new string[rawLines.Length - 2];
            Console.Write("Loading file...");
            for (int l = 1; l < rawLines.Length-1; l++)
            {
                lines[l-1] = rawLines[l].Trim();
            }
            Console.WriteLine("Done\nTotal lines in file: {0}", lines.Length);
            lineCount = lines.Length;
            int currentLine = 1;
            int lineStep;
            /* COMMANDS:
             * l: go to line
             * a: add line
             * d: delete line
             * r: replace line
             * w: writeOut
             * q: quit
             */
             bool running = true;
             string input;
             bool dirtyCopy = false;
             bool lineExists;
             while (running)
             {
                if (currentLine > lineCount)
                {
                    lineExists = false;
                }
                else
                {
                    lineExists = true;
                }
                Console.Write("[{0}] {1} ", currentLine, lineExists ? "-" : "*");
                char KP = (char)Console.Read();
                Console.Write("\n");
                switch (Char.ToUpper(KP))
                {
                    case 'Q':
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Error: Invalid command.");
                        break;
                }
             }
            return;
        }
    }
}
