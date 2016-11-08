using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper;
using Aspose.Network;
using Aspose.Network.Pop3;
using Aspose.Network.Imap;
using Aspose.Network.Mail;
using System.Xml;
using System.Data;
using System.IO;
using Aspose.Network.Exchange;

namespace FindEmail
{
    class FindEmail
    {
        static string processName = "FindEmail";

        public string title { get; set; }
        public string emailServerType { get; set; }
        public string serverAddress { get; set; }
        public string emailAccount { get; set; }
        public string emailPassword { get; set; }
        public string destinationFolder { get; set; }
        public string archiveFolder { get; set; }
        public string SSL { get; set; }
        public string immediateExcelExtract { get; set; }     
        public int port { get; set; }
        public string enabled { get; set; }
        public string domain { get; set; }

        public void GetEmail()
        {
            bool connected = false;

            Utils.WriteToLogStart(processName);

            //Console.WriteLine("wrote to db log");
                title = "Test";
                emailServerType = "IMAP";
                serverAddress = "imap.gmail.com";
                emailAccount = "windartdata@googlemail.com";
                emailPassword = "WindyMiller%";
                destinationFolder = @"D:\PipelineData\0.EmailFolder";
                archiveFolder = @"D:\PipelineData\00.Preprocess";
                SSL = "Yes";
                port = 993;

                //Console.WriteLine("assigned variables");
                //Console.ReadLine();
                ProtocolClient client = null;

                if (emailServerType.ToUpper() == "POP3")
                {
                    client = new Pop3Client();
                }
                else if (emailServerType.ToUpper() == "IMAP")
                {
                    client = new ImapClient();
                }

                connected = connect(client);

                //Console.WriteLine("connected " + connected);
                //Console.ReadLine();

                if (connected)
                {
                    readInbox(client);
                }
            

            Utils.WriteToLogFinish(processName);
        }

        private void readInbox(ExchangeClient client)
        {
            Dictionary<string, clsFileList> convertFiles = new Dictionary<string, clsFileList>();
           

            try
            {
                //query mailbox
                ExchangeMailboxInfo mailbox = client.GetMailboxInfo();
                //list messages in the Inbox
                ExchangeMessageInfoCollection messages = client.ListMessages(mailbox.InboxUri, false);

                foreach (ExchangeMessageInfo info in messages)
                {
                    //save the message locally
                    //client.SaveMessage(info.UniqueUri, info.Subject + ".eml");
                    // create object of type MailMessage
                    MailMessage msg;

                    msg = client.FetchMessage(info.UniqueUri);


                    foreach (Attachment attachedFile in msg.Attachments)
                    {
                        extractAttachment(convertFiles, attachedFile);
                    }

                    

                    // Can't see a 'readed' flag on messages, info or msg
                    // so we'll have to delete emails after we have read them
                    //client.DeleteMessage(info.UniqueUri);

                }
            }
            catch (ExchangeException ex)
            {
               // Console.WriteLine(ex.ToString());
            }
        }

        private void readInbox(ProtocolClient client)
        {
            // Base clases have been sealed, so can't extend a generic 'readInbox' function
            if (client.GetType() == typeof(Pop3Client))
            {
                readInbox((Pop3Client)client);
            }
            else if (client.GetType() == typeof(ImapClient))
            {
                readInbox((ImapClient)client);
            }
        }

        private bool connect(ProtocolClient client)
        {
            if (client.GetType() == typeof(Pop3Client))
            {
                return connect((Pop3Client)client);
            } else if(client.GetType() == typeof(ImapClient))
            {
                return connect((ImapClient)client);
            }

            return false;
        }

        private void readInbox(Pop3Client client)
        {
            Dictionary<string, clsFileList> convertFiles = new Dictionary<string, clsFileList>();
           

            // get number of messages in the mailbox
            int messageCount = client.GetMessageCount();

            // iterate through the messages and retrieve one by one
            for (int i = 1; i <= messageCount; i++)
            {
                // create object of type MailMessage
                MailMessage msg;

                msg = client.FetchMessage(i);

                foreach (Attachment attachedFile in msg.Attachments)
                {
                    extractAttachment(convertFiles, attachedFile);
                }

                
            }
        }

     private void extractAttachment(Dictionary<string, clsFileList> convertFiles, Attachment attachedFile)
        {
            // Don't add files that have been sent twice
            if (!convertFiles.ContainsKey(destinationFolder + @"\" + attachedFile.Name))
            {

                FileStream writeFile = new FileStream(destinationFolder + @"\" + attachedFile.Name, FileMode.Create); 
                attachedFile.SaveRawContent(writeFile);
               
               
               FileStream writeFileToArchive = new FileStream(archiveFolder + @"\" + attachedFile.Name, FileMode.Create);
               attachedFile.SaveRawContent(writeFileToArchive); 
                
               writeFile.Close();
               writeFileToArchive.Close();
                
                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Downloaded file: " + destinationFolder + @"\" + attachedFile.Name);
                //if (immediateExcelExtract == "1" && (attachedFile.Name.ToLower().EndsWith("xls") || attachedFile.Name.ToLower().EndsWith("xlsx")))
                //{
                //    convertFiles.Add(destinationFolder + @"\" + attachedFile.Name, new clsFileList { FullFilePath = destinationFolder + @"\" + attachedFile.Name });
                //}
            }
            else
            {
                Utils.WriteToLog(Utils.LogSeverity.Warning, processName, "Ignored duplcate file: " + destinationFolder + @"\" + attachedFile.Name);
            }
        }

        private void readInbox(ImapClient client)
        {
            Dictionary<string, clsFileList> convertFiles = new Dictionary<string, clsFileList>();
           
            // select the inbox folder
            client.SelectFolder(ImapFolderInfo.InBox);

            // get the message info collection              
            ImapMessageInfoCollection list = client.ListMessages();

            // iterate through the messages and retrieve one by one
            // TO DO: this example code might be able to be rewritten as a foreach(ImapMessageInfo...
            // will still need to do the client.FetchMessage though.
            for (int i = 1; i <= list.Count; i++)
            {
                // create object of type MailMessage
                MailMessage msg;

                Utils.WriteToLog(Utils.LogSeverity.Info, processName, String.Format("Reading message number {0} of {1}", i, list.Count));

                msg = client.FetchMessage(i);

                if (!list[i-1].Readed)
                {
                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Message not read");

                    foreach (Attachment attachedFile in msg.Attachments)
                    {
                        Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Extracting attachement");

                        extractAttachment(convertFiles, attachedFile);

                        Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Finished extracting attachement");

                        //FileStream writeFile = new FileStream(destinationFolder + @"\" + attachedFile.Name, FileMode.Create);
                        //attachedFile.SaveRawContent(writeFile);
                        //writeFile.Close();

                        //Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Downloaded file: " + destinationFolder + @"\" + attachedFile.Name);
                        //if (immediateExcelExtract == "1" && (attachedFile.Name.ToLower().EndsWith("xls") || attachedFile.Name.ToLower().EndsWith("xlsx")))
                        //{
                        //    convertFiles.Add(destinationFolder + @"\" + attachedFile.Name, new clsFileList { FullFilePath = destinationFolder + @"\" + attachedFile.Name });
                        //}
                    }

                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Immediate extraction, if required");

                    

                    //if (immediateExcelExtract == "1")
                    //{
                    //    // In immediate mode we save the contents of each workbook
                    //    // to the current folder
                    //    // Finally delete all the original xls files
                    //    cf.ConvertExcelToCSV(convertFiles, destinationFolder, true);
                    //}

                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Changing email status");

                    client.ChangeMessageFlags(i, Aspose.Network.Imap.MessageFlags.Readed);

                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Finished changing email status");
                }
            }
        }

        private void createIfDoesntExist(string p)
        {
            if (!System.IO.Directory.Exists(p))
            {
                System.IO.Directory.CreateDirectory(p);
            }
        }

        private bool connect(Pop3Client client)
        {
            bool success = false;

            // basic setings (required)
            client.Host = serverAddress; 
            client.Username = emailAccount;
            client.Password = emailPassword;
            client.Port = port; 

            if (SSL=="Yes")
            {
                // Settings required for SSL enabled Pop3 servers
                client.SecurityMode = Pop3SslSecurityMode.Implicit;
                client.EnableSsl = true;
            }

            try
            {
                client.Connect();
                client.Login();
                success = true;
            }
            catch (Exception e)
            {
                Utils.WriteToLog(Utils.LogSeverity.Error, processName, "The following error was generated whilst trying to connect (POP3): " + e.InnerException + "\n" + e.Message + "\n" + e.StackTrace);
            }

            return success;
        }

        private bool connect(ImapClient client)
        {
            bool success = false;
            // basic setings (required)
            client.Host = serverAddress; 
            client.Username = emailAccount;  
            client.Password = emailPassword;
            client.Port = port; 

            if (SSL == "Yes")
            {
                // Settings required for SSL enabled Imap servers
                client.SecurityMode = ImapSslSecurityMode.Implicit;
                client.EnableSsl = true;
            }

            try
            {
                client.Connect();
                client.Login();
                success = true;
            }
            catch(Exception e)
            {
                Utils.WriteToLog(Utils.LogSeverity.Error, processName, "The following error was generated whilst trying to connect: (IMAP)" + e.InnerException + "\n" + e.Message + "\n" + e.StackTrace);
            }

            return success;
        }

    }


}
