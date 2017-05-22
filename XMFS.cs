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
        static const string prompt = " $ ";
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
            mainDoc.Load(filename);
            workingElement = (XmlElement)mainDoc.DocumentElement.ChildNodes.Item(1);
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
            return true;
        }

        static bool DSD(XmlNode newElement)
        {
            if (changeElement(newElement))
            {
                workingString = workingString + newElement.LocalName;
                return true;
            }
            return false;
        }

        static bool DSU()
        {
            if (workingString = "/")
            {
                return false;
            }
            if (changeElement(workingElement.ParentNode))
            {
                do
                {
                    workingString = workingString.Remove(workingString.Length - 1);
                } while (!workingString.EndsWith("/"));
                return true;
            }
            return false;
        }
        #endregion
    }
}
