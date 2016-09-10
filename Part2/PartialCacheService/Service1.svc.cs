using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Windows.Forms;

namespace PartialCacheService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public Stream DownloadFile(string filename)
        {
            //MessageBox.Show("MAKING IT TO THE START");
            string path = "C:\\711\\Part2\\cache\\" + filename;
            //Send the file to the server to receive the file chunks.
            FileInfo f = new FileInfo(path);
            if (File.Exists(path))
            {
                //var cache_file = File.ReadAllBytes(path);

                WebRequest request = WebRequest.Create(
                     "http://localhost:8181/Service1.svc/GetFileChunks/" + filename);
                request.Method = "POST";
                request.ContentType = "text/plain";
                request.ContentLength = f.Length;
                //request.ContentLength = cache_file.Length;
                using (Stream s = request.GetRequestStream())
                {
                    FileStream fileStream = new FileStream(
                        path, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[1024];
                    
                    int bytesRead = 0;
                    while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        s.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();
                    //s.Write(cache_file, 0, cache_file.Length);
                    
                }
                //Get response from server
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                DataContractJsonSerializer serializer =
                    new DataContractJsonSerializer(typeof(List<Chunk>));
                List<Chunk> chunks = (List<Chunk>)serializer.ReadObject(responseStream);
                //  StreamReader readStream = new StreamReader(responseStream);
                // var result = readStream.ReadToEnd();
                //Now get chunks and build up file.
                List<byte[]> chunked_file = RabinCompare.Slice(File.ReadAllBytes(path), 0x1FFF);
                //Got the chunks, now rebuild the file. Get the bytes together and write.

                if (chunks != null)
                {
                    //TODO: Change implementation to use WriteAllBytes
                    //System.IO.File.WriteAllText(path, result);
                    //Use Rabin chunker to know which bytes to use?
                    //Then recreate entire byte[]?
                    if (File.Exists(path))
                    {
                        using (StreamWriter sw = File.AppendText("C:\\711\\Part2\\cache_log.txt"))
                        {
                            sw.WriteLine("Response: downloaded file:" + filename);
                        }
                    }
                    return File.OpenRead(path);
                }
            }
            else // TODO implement normal file download
            {
                return null;
            }
            return null;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost host = new WebServiceHost(typeof(Service1), new Uri("http://localhost:8282/"));
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(Service1), new WebHttpBinding(), "");
            try
            {
                host.Open();
                Console.WriteLine("Service is running:");
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();
                host.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("An exception occured: {0}", e.Message);
                host.Abort();
            }
        }
    }
}
