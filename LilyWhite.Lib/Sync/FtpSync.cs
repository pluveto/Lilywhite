using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using static LilyWhite.Lib.Util.Diff;

namespace LilyWhite.Lib.Sync
{
    public class FtpSync
    {
        public List<DiffItem> DiffFiles { get; private set; }
        public string Address { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string RemoteBaseDir { get; private set; }
        public string LocalBaseDir { get; private set; }

        public FtpSync(string addr, string remoteBaseDir, string localBaseDir, string username, string password, List<DiffItem> diffItems)
        {
            this.Address = addr;
            this.Username = username;
            this.Password = password;
            this.DiffFiles = diffItems;
            var sepr = new char[] { '/', '\\' };
            this.RemoteBaseDir = remoteBaseDir.TrimEnd(sepr);
            this.LocalBaseDir = localBaseDir.TrimEnd(sepr);
        }

        public void Run()
        {
            var connectionInfo = new ConnectionInfo(this.Address, this.Username, new PasswordAuthenticationMethod(this.Username, this.Password));
            sftp.Connect();

            // Upload File
            using (var sftp = new SftpClient(connectionInfo))
            {
                foreach (var diffItem in DiffFiles)
                {
                    switch (diffItem.Type)
                    {
                        case DiffType.Update:
                            break;
                        case DiffType.Delete:
                            break;
                        case DiffType.Create:
                            //sftp.ChangeDirectory("/MyFolder");
                            using (var uplfileStream = System.IO.File.OpenRead(diffItem.FileName))
                            {
                                var rel = Path.GetRelativePath(diffItem.FileName, this.LocalBaseDir);
                                if (!rel.EndsWith("\\"))
                                    rel += "\\";
                                sftp.UploadFile(uplfileStream, diffItem.FileName, true);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
            sftp.Disconnect();

            var request = WebRequest.Create($"ftp://{this.Address}") as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader("testfile.txt"))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }
    }
}
