using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace untitledApplication
{
    class Program
    {
        static string filename;
        static XmlElement workingElement;
        static string workingString = "/";
        static XmlDocument mainDoc;
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
                        XmlElement folder = searchByID(input.Split(' ')[1]);
                        if(folder == null)
                        {
                            Console.WriteLine("Error: folder does not exist.");
                            break;
                        }
                        workingElement.RemoveChild(folder);
                        break;

                    case "CD":
                        if (input.Split(' ')[1] == "..")
                        {
                            DCU();
                            break;
                        }
                        XmlElement nWDF = searchByID(input.Split(' ')[1]);
                        if (nWDF == null)
                        {
                            Console.WriteLine("Error: folder does not exist.");
                            break;
                        }
                        DCD(nWDF);
                        break; 

                    default:
                        Console.WriteLine("Error: Unknown command: \"{0}\"", input);
                        break;
                }
            }
        }

        #region cd functions
        static bool changeElement(XmlNode newWorkingElement)
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
            if (changeElement(newElement))
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
            workingElement = workingElement.ParentNode;
            do
            {
                workingString = workingString.Remove(workingString.Length - 1);
            } while (!workingString.EndsWith("/"));
            return true;
        }
        #endregion

        static XmlElement searchByName(string elementName)
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

        static XmlElement searchByID(string elementID)
        {
            try
            {
                XmlElement cElm = (XmlElement)workingElement.ChildNodes.Item(Int32.Parse(elementID));
                elementID = cElm.GetAttribute("name");
            }
            catch (System.Exception)
            {
            }
            return searchByName(elementID);
        }
    }
}
