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

namespace BackendPartialService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : BackendPartialServiceContract
    {
      
        public List<string> GetAvailableFilenames()
        {
            string path = "C:\\711\\Part2\\files";
            if (Directory.Exists(path))
            {
                string[] availableFiles = Directory.GetFiles(path);
                List<string> filesList = new List<string>();
                foreach (string s in availableFiles)
                {
                    filesList.Add(Path.GetFileName(s));
                }
                availableFiles.ToList();
                return filesList;
            }
            else
            {
                Console.WriteLine("Could not find " + path);
                return null;
            }



        }
    

        public Stream GetFileChunks(string filename, Stream s)
        {
            //This service receives the file from the cache
            //Then compares this file against the server's version
            //Then sends the missing chunks to the cache, which are then injected.
            int length = 0;
            string path = "C:\\711\\Part2\\from_cache\\" + filename;
            if (Directory.Exists("C:\\711\\Part2\\from_cache"))
            {
                using (FileStream writer = new FileStream(path, FileMode.Create))
                {
                    int readCount;
                    var buffer = new byte[8192];
                    while ((readCount = s.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        writer.Write(buffer, 0, readCount);
                        length += readCount;
                    }
                }
                //File is written, now compare!
                //Need to get List<Chunk> from RabinCompare.
                List<Chunk> chunks = RabinCompare.Compare("C:\\711\\Part2\\from_cache\\" + filename,
                    "C:\\711\\Part2\\files\\" + filename, 0x1FFF); //Error coming from here!
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Chunk>));
                MemoryStream stream = new MemoryStream();
                ser.WriteObject(stream, chunks);
                //Now send 

                return stream;
            }
            else 
            {
                return null;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost host = new WebServiceHost(typeof(BackendPartialServiceContract), new Uri("http://localhost:8181/"));
            ServiceEndpoint ep = host.AddServiceEndpoint(typeof(BackendPartialServiceContract), new WebHttpBinding(), "");
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
