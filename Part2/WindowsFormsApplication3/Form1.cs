using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void refresh_Click(object sender, EventArgs e)
        {
            string path = "C:\\711\\Part1\\cache_log.txt";
            using (StreamReader s = new StreamReader(path))
            {
                string log = s.ReadToEnd();
                richTextBox1.Text = log;
                listBox1.DataSource = HelperMethods.GetAvailableFiles();
            }
        }

        private void clear_Click(object sender, EventArgs e)
        {
            string dir = "C:\\711\\Part1\\cache\\";
            string[] fileList = Directory.GetFiles(dir);
            foreach (string f in fileList)
            {
                File.Delete(f);
            }
            listBox1.DataSource = null;
        }

        private void listBox_Click(object sender, EventArgs e)
        {
            //This gives us the item name we want to download!
            //Extract the name and then send the request to http://localhost:8099/Service1.svc/DownloadFile/{filename}
            
            var item = listBox1.SelectedItem;
            string path = "C:\\711\\Part2\\cache\\" + item;
            //Set up the request
            if (File.Exists(path))
            {
                displayFile(path);
            }
        }
        private void displayFile(string filepath)
        {
            //This method displays the content of the file once it has been received,
            //with the header of the form being that of the filename
            string filename = Path.GetFileName(filepath);
            var content = File.ReadAllBytes(filepath);
            Form2 form2 = new Form2(content, filename);
            //before showing the result, write the file to disk
           
            //TODO: Change implementation to use: WriteAllBytes
            
          
           



        }
    }
}
