using System.Text;
using ImageMagick;

namespace SearchInPDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SetText(string text)
        {
            textBox1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (var fileName in openFileDialog1.FileNames)
                    {
                        listView1.Items.Add(fileName);
                        using (MagickImage image = new MagickImage())
                        {
                            var readSettings = new MagickReadSettings();
                            readSettings.Density = new Density(300);
                            readSettings.Compression = CompressionMethod.NoCompression;

                            image.Read(fileName,readSettings);
                            image.Sharpen(Channels.Black);
                            image.Density = new Density(300);
                            image.Quality = 100;
                            image.Format = MagickFormat.Png;
                            image.Write("./testImg.png");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }
    }
}