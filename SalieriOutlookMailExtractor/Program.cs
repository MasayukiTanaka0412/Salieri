using System;
using System.Reflection; // to use Missing.Value
using Outlook = Microsoft.Office.Interop.Outlook;
using System.IO;


namespace ConsoleApplication1
{
    public class Class1
    {
        static string mailboxName;
        static string folderName;
        static string outputDirectory;

        public static int Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Please specify mailboxName, FolderName, and OutputDirectory");
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
                return 1;
            }

            mailboxName = args[0];
            folderName = args[1];
            outputDirectory = args[2];
            try
            {
                Outlook.Application oApp = new Outlook.Application();

                // Get the MAPI namespace.
                Outlook.NameSpace oNS = oApp.GetNamespace("mapi");
                // Log on by using the default profile or existing session (no dialog box).
                oNS.Logon(Missing.Value, Missing.Value, false, true);

                //Get the Inbox folder.
                Outlook.MAPIFolder oInbox = oApp.Session.Folders[mailboxName].Folders[folderName];

                foreach (object item in oInbox.Items)
                {
                    try
                    {
                        Outlook.MailItem oMsg = (Outlook.MailItem)item;
                        Console.WriteLine(oMsg.Subject);
                        Console.WriteLine(oMsg.SenderName);
                        Console.WriteLine(oMsg.ReceivedTime);
                        Console.WriteLine(oMsg.Body);


                        string filename = oMsg.Subject + "_" + oMsg.ReceivedTime + ".txt";
                        filename = filename.Replace(@"\", "_");
                        filename = filename.Replace("/", "_");
                        filename = filename.Replace(":", "_");
                        filename = filename.Replace("*", "_");
                        filename = filename.Replace("?", "_");
                        filename = filename.Replace("\"", "_");
                        filename = filename.Replace("<", "_");
                        filename = filename.Replace(">", "_");
                        filename = filename.Replace(@"\", "_");

                        string content = oMsg.Body;

                        using (StreamWriter sw = new StreamWriter(Path.Combine(outputDirectory, filename)))
                        {
                            sw.WriteLine(content);
                        }

                        oMsg = null;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("{0} Exception caught: ", e);
                    }
                }

                //Log off.
                oNS.Logoff();

                //Explicitly release objects.
                oInbox = null;
                oNS = null;
                oApp = null;
            }

            //Error handler.
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught: ", e);
            }

            // Return value.
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            return 0;

        }

        
        
    }
}

