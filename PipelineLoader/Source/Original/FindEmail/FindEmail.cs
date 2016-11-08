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
        public string SSL { get; set; }
        public string immediateExcelExtract { get; set; }     
        public int port { get; set; }
        public string enabled { get; set; }
        public string domain { get; set; }

        public void GetEmail()
        {
            bool connected = false;

            Utils.WriteToLogStart(processName);

            if (Utils.GetKeyFromConfig("SkipSharepoint") != "True")
            {
                DocLibHelper dlh = new DocLibHelper();

                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "About to get sharepoint list");

                System.Xml.XmlNode emailDetails = dlh.GetList(Utils.GetKeyFromConfig("ApplicationConfigurationSite"), Utils.GetKeyFromConfig("EmailReaderList"));

                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Finished getting sharepoint list");

                // null return means we had a problem contacting sharepoint
                if (emailDetails != null)
                {
                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "About to read sharepoint list");

                    // copy the xml structure to a dataset
                    DataSet objDataSet = new DataSet();
                    XmlNodeReader ObjXmlreadrer = new XmlNodeReader(emailDetails);
                    objDataSet.ReadXml(ObjXmlreadrer);

                    foreach (DataRow row in objDataSet.Tables[1].Rows)
                    {
                        #region assign dataset to vars
                        title = row["ows_Title"].ToString();
                        emailServerType = row["ows_EmailServerType"].ToString();
                        serverAddress = row["ows_ServerAddress"].ToString();
                        emailAccount = row["ows_EmailAccount"].ToString();
                        emailPassword = row["ows_EmailPassword"].ToString();
                        destinationFolder = row["ows_DestinationFolder"].ToString();
                        SSL = row["ows_SSL"].ToString();
                        if (row["ows_Port"].ToString() != "")
                        {
                            port = int.Parse(row["ows_Port"].ToString());
                        }
                        immediateExcelExtract = row["ows_ImmediateExcelExtract"].ToString();
                        enabled = row["ows_Enabled"].ToString();
                        domain = row["ows_Domain"].ToString();
                        #endregion

                        
                        if (int.Parse(enabled) == 1)
                        {
                            Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Init Exchange mailbox");

                            // Argh, the class ExchangeClient does not extend ProtocolClient, like the others...
                            // So have to deal with with this type separately
                            if (emailServerType.ToUpper() == "EXCHANGE")
                            {
                                ExchangeClient exchangeClient = null;

                                exchangeClient = new ExchangeClient(serverAddress, emailAccount, emailPassword, "HE1");
                                readInbox(exchangeClient);
                            }
                            else
                            {
                                ProtocolClient client = null;

                                if (emailServerType.ToUpper() == "POP3")
                                {
                                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Init POP3 mailbox");
                                    client = new Pop3Client();
                                }
                                else if (emailServerType.ToUpper() == "IMAP")
                                {
                                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Init IMAP mailbox");
                                    client = new ImapClient();
                                }

                                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Connecting...");

                                connected = connect(client);

                                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Connected");

                                if (connected)
                                {
                                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Reading mailbox...");

                                    readInbox(client);

                                    Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Finished reading mailbox");

                                }
                            }
                        }
                    }
                }
                else
                {
                    Utils.WriteToLog(Utils.LogSeverity.Warning, processName, "Problem accessing SharePoint list");
                }
            }
            else
            {
                title = "Test";
                emailServerType = "IMAP";
                serverAddress = "imap.gmail.com";
                emailAccount = "windartdata@googlemail.com";
                emailPassword = "WindyMiller";
                destinationFolder = @"C:\PipelineData\0.EmailFolder";
                SSL = "Yes";
                port = 993;

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

                if (connected)
                {
                    readInbox(client);
                }
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
                Console.WriteLine(ex.ToString());
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
                writeFile.Close();

                Utils.WriteToLog(Utils.LogSeverity.Info, processName, "Downloaded file: " + destinationFolder + @"\" + attachedFile.Name);
                //if (immediateExcelExtract == "1" && (attachedFile.Name.ToLower().EndsWith("xls") || attachedFile.Name.ToLower().EndsWith("xlsx")))
                //{
                //    convertFiles.Add(destinationFolder + @"\" + attachedFile.Name, new clsFileList { FullFilePath = destinationFolder + @"\" + attachedFile.Name });
                //}
            }
            else
            {
                Utils.WriteToLog(Utils.LogSeverity.Warning, processName, "Ignored dupliacte file: " + destinationFolder + @"\" + attachedFile.Name);
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
