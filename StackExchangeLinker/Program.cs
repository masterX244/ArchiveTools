using HtmlAgilityPack;
using System;
using System.IO;
using System.Xml;

namespace StackExchangeLinker
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                Console.WriteLine("Starting " + arg);
                if (!File.Exists(arg + ".txt"))
                {
                    XmlReader r = XmlReader.Create(arg);
                    Console.WriteLine("Really Starting " + arg);
                    using TextWriter tw = new StreamWriter(arg + ".txt"); //C# 8, selfdestructs at scope end
                    using TextWriter tw2 = new StreamWriter(arg + ".relative.txt"); //C# 8, selfdestructs at scope end
                    r.MoveToContent();
                    while (r.Read())
                    {
                        var body = r.GetAttribute("Body");
                        if (body != null)
                        {
                            var parseMsgBody = String.Format(@"<html><body>{0}</body></html>", body);
                            var doc = new HtmlDocument();
                            doc.LoadHtml(parseMsgBody);
                            var nodes = doc.DocumentNode.SelectNodes("//a");

                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    var attr = node.Attributes["href"];
                                    if(attr==null)
                                    {
                                        Console.WriteLine("DAFUQ?");
                                        Console.WriteLine(parseMsgBody);
                                        continue;
                                    }
                                    var link = attr.Value ?? "" + "\n";
                                    if (!link.StartsWith("/"))
                                    {
                                        tw.Write(link);
                                    }
                                }
                            }
                            nodes = doc.DocumentNode.SelectNodes("//img");

                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    var attr = node.Attributes["src"];
                                    if (attr == null)
                                    {
                                        Console.WriteLine("DAFUQ?");
                                        Console.WriteLine(parseMsgBody);
                                        continue;
                                    }
                                    var link = attr.Value + "\n";

                                    if (!link.StartsWith("/"))
                                    {
                                        tw.Write(link);
                                    }
                                    else
                                    {
                                        //throw new Exception("BLAH!, this shouldnt have happened at all. Not sure what fudgery is the reason");
                                        tw2.Write(link);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
    }
}
