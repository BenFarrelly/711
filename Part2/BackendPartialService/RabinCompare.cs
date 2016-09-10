using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BackendPartialService
{
    public static class RabinCompare
    {

        public static List<Chunk> Compare(string file1, string file2, ulong boundary)
        {
            //file2 server file
            //file1 cache file
            byte[] A = null;
            byte[] B = null;
            //using (StreamReader r = new StreamReader(file1))
            //{
            A = File.ReadAllBytes(file1);
            //}

            B = File.ReadAllBytes(file2);

          //  Stopwatch watch = new Stopwatch();
          //  watch.Start();
            var listA = Slice(A, boundary);
           // watch.Stop();
           // long timeA = watch.ElapsedMilliseconds;
           // watch.Reset();
           // watch.Start();
            var listB = Slice(B, boundary);
           // watch.Stop();
           /// long timeB// = watch.ElapsedMilliseconds;
            int i = 0;
            //Console.WriteLine(string.Format("Chunks in file '{0}'\t: {1}", new FileInfo(file1).Name, listA.Count));
            //Console.WriteLine(string.Format("Chunks in file '{0}'\t: {1}", new FileInfo(file2).Name, listB.Count));
            //Console.WriteLine("\n--------MATCHED CHUNKS--------\n");
            int matchCount = 0;
            //need to create list of byte[] and chunks
            List<Chunk> chunks = new List<Chunk>();
            foreach (byte[] s in listA)
            {
                if (ContainsSequence(listB, s) >= 0)
                {
                    //Console.WriteLine("{0:D4}<-->{1:D4}  (size:{2:D8} bytes)", i, ContainsSequence(listB, s), s.Length);
                    Chunk temp = new BackendPartialService.Chunk(listB[ContainsSequence(listB, s)], i);
                    chunks.Add(temp);
                    matchCount++;
                }
                else
                    //Console.WriteLine("{0:D4}    *     (size:{1:D8} bytes)", i, s.Length);
                i++;
            }

            return chunks;
          //Console.WriteLine("\n------------------------------\n");
          //Console.WriteLine("Matched blockes {0} (out of {1})", matchCount, listA.Count);
          //Console.WriteLine("Time used to process file '{0}'\t:{1:f3} (sec)", new FileInfo(file1).Name, timeA / 1000.0);
          //Console.WriteLine("Time used to process file '{0}'\t:{1:f3} (sec)", new FileInfo(file2).Name, timeB / 1000.0);

        }
        static List<byte[]> Slice(byte[] s, ulong boundary)
        {
            int z = 0;
            List<byte[]> ret = new List<byte[]>();
            ulong Q = 179424691; //A really large prime number.
            ulong D = 256;
            ulong pow = 1;
            int windowSize = 48;
            for (int k = 1; k < windowSize; k++)
                pow = (pow * D) % Q;
            ulong sig = 0;
            int lastIndex = 0;
            for (int i = 0; i < windowSize; i++)
                sig = (sig * D + s[i]) % Q;
            for (int j = 1; j <= s.Length - windowSize; j++)
            {
                sig = (sig + Q - pow * s[j - 1] % Q) % Q;
                var temp1 = s[j - 1];
                sig = (sig * D + s[j + windowSize - 1]) % Q;
                var temp2 = s[j + windowSize - 1];

                if ((sig & boundary) == 0)
                {
                    if (j + 1 - lastIndex >= 2048)
                    {
                        List<byte> byteList = new List<byte>();
                        int l = 0;
                        var index = j - lastIndex;
                        for (int x = lastIndex; x <= j; x++)
                        {
                            byteList.Add(s[x]);
                            l++;
                        }
                        var b = byteList.Count;
                        var str = System.Text.Encoding.UTF8.GetString(byteList.ToArray());
                        ret.Add(byteList.ToArray());
                        lastIndex = j + 1;
                        z++;
                    }
                }
                else if (j + 1 - lastIndex >= 65536)
                {
                    List<byte> temp = new List<byte>();
                    for (int i = lastIndex; i < j + 1 - lastIndex; i++)
                    {
                        temp.Add(s[i]);
                    }
                    ret.Add(temp.ToArray());
                    lastIndex = j + 1;

                }
            }
            if (lastIndex < s.Length - 1)
            {
                List<byte> temp = new List<byte>();
                for (int i = lastIndex; i < s.Length; i++)
                {
                    temp.Add(s[i]);
                }
                ret.Add(temp.ToArray());
            }
            Console.WriteLine("Number of iterations " + z);
            return ret;
        }
        static int ContainsSequence(List<byte[]> toSearch, byte[] toFind)
        {
            //MD5 hasher = MD5.Create();
            int index = -1;
            for (var i = 0; i < toSearch.Count; i++)
            {

                byte[] temp = toSearch[i];
                if (temp.Length == toFind.Length)
                {
                    // var allSame = true;
                    //for (var j = 0; j < toFind.Length; j++)
                    //{

                    if (ParallelSequenceEqual(temp, toFind))//(temp.SequenceEqual(toFind))
                    {
                        // allSame = true;

                        index = i;
                        return index;
                    }
                    //}
                }
            }
            return index;
        }
        static bool SequenceEqual(byte[] seq1, byte[] seq2)
        {
            bool AreEqual = true;
            for (int i = 0; i < seq1.Length; i++)
            {
                if (seq1[i].Equals(seq2[i]))
                {
                    continue;
                }
                else
                {
                    AreEqual = false;
                    return AreEqual;
                }

            }
            return AreEqual;
        }
        static bool ParallelSequenceEqual(byte[] seq1, byte[] seq2)
        {
            bool AreEqual = true;
            if (seq1.Length != seq2.Length)
            {
                return false;
            }
            Parallel.For(0, seq1.Length - 1,
                (i, loopstate) => {
                    if (!seq1[i].Equals(seq2[i]))
                    {
                        AreEqual = false;
                        loopstate.Stop();

                    }
                });


            return AreEqual;
        }
    }
}
