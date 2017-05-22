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
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: No filename specified.");
                return;
            }
            filename = args[0];
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            XmlElement XEXMFS = doc.CreateElement("XMFS");
            doc.AppendChild(XEXMFS);
            XmlElement XEConfig = doc.CreateElement("configuration");
            XEXMFS.AppendChild(XEConfig);
            XmlElement XEConfigHome = doc.CreateElement("home");
            XEConfig.AppendChild(XEConfigHome);
            XmlText XTConfigHomeCDATA = doc.CreateTextNode("/home");
            XEConfigHome.AppendChild(XTConfigHomeCDATA);
            XmlElement XEFilesystem = doc.CreateElement("filesystem");
            XEXMFS.AppendChild(XEFilesystem);
            doc.Save(filename);
        }
    }
}
