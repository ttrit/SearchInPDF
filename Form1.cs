using ImageMagick;
using IronOcr;

namespace SearchInPDF
{
    public partial class Form1 : Form
    {
        private List<string> filePaths;
        private List<string> searchStrings;
        private Dictionary<string, string> mappingFileAndContent;
        private Dictionary<string, List<string>> missingFileAndContent;

        public Form1()
        {
            InitializeComponent();
            filePaths = new List<string>();
            searchStrings = new List<string>();
            mappingFileAndContent = new Dictionary<string,string>();
            missingFileAndContent = new Dictionary<string, List<string>>();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            searchStrings.AddRange(textBox2.Text.Split(","));
            ReadPdfContent();

            foreach (var file in mappingFileAndContent)
            {
                var missingString = new List<string>();
                foreach (var searchString in searchStrings)
                {
                    if (!file.Value.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    {
                        missingString.Add(searchString);
                    }
                }

                if (missingString.Any())
                {
                    missingFileAndContent.Add(file.Key, missingString);
                }
            }

            foreach (var missingContent in missingFileAndContent)
            {
                listView1.Items.Add(Path.GetFileName(missingContent.Key));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePaths.AddRange(openFileDialog1.FileNames);
            }
        }

        private void ReadPdfContent()
        {
            try
            {
                foreach (var fileName in filePaths)
                {
                    using (MagickImage image = new MagickImage())
                    {
                        var readSettings = new MagickReadSettings();
                        readSettings.Density = new Density(300);
                        readSettings.Compression = CompressionMethod.NoCompression;

                        image.Read(fileName, readSettings);
                        image.Sharpen(Channels.Black);
                        image.Density = new Density(300);
                        image.Quality = 100;
                        image.Format = MagickFormat.Png;
                        image.Write("./temp.png");
                    }

                    // convert image to text
                    var Ocr = new IronTesseract();
                    Ocr.Language = OcrLanguage.English;
                    using (var input = new OcrInput())
                    {
                        input.AddImage(@"./temp.png");
                        var result = Ocr.Read(input);
                        mappingFileAndContent.Add(fileName, result.Text);
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