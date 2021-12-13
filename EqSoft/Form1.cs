using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hotkeys;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
//https://pl.wikipedia.org/wiki/Plik:Color_blindness.png
//https://arxiv.org/ftp/arxiv/papers/1502/1502.03723.pdf
// uper link work perfectly with deuteranomaly


namespace EqSoft
{
    public partial class FQS : Form
    {

        #region Private Variables

        private Hotkeys.GlobalHotkey F1Key, F2Key, F3Key, F4Key, F5Key, F6Key, F7Key, F8Key, F9Key, PrintScreenKey, PlusKey, MinusKey;
        private Int32 F1Id = 1, F2Id = 2, F3Id = 3, F4Id = 4, F5Id = 5, F6Id = 6, F7Id = 7, F8Id = 8, F9Id = 9, PrintScreenId = 13, PlusId = 10, MinusId = 11;
        private EpreviousFilter previousFilter;
        private string printImagePath = Application.StartupPath;
        string optionsPath = Application.StartupPath + @"\options.txt";
        public Color RedCustomColorValue, GreenCustomColorValue, BlueCustomColorValue;
        PictureBox[] screensPictureBox = new PictureBox[6];
        Form2 customFilter;
        int waveLenghtSeverity = 0;



        private Dictionary<int, float[]> protanomalyMatrixDictionary = new Dictionary<int, float[]>
        #region
        {
            {
                0,
                new[]
                {
                    1,  0,   0,  0.0f,  0.0f,
                    0,  1,   0,  0.0f,  0.0f,
                    0,  0,   1,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f
                }
            },
            {
                2,
                new[]
                {
                    0.856167f, 0.029342f,  -0.002880f, 0.0f,  0.0f,
                    0.182038f, 0.955115f,  -0.001563f, 0.0f,  0.0f,
                     -0.038205f, 0.015544f,  1.004443f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                4,
                new[]
                {
                    0.734766f,  0.051840f,  -0.004928f,   0.0f,  0.0f,
                    0.334872f,  0.919198f,  -0.004209f,  0.0f,  0.0f,
                    -0.069637f,  0.028963f,  1.009137f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                6,
                new[]
                {
                    0.630323f, 0.069181f,   -0.006308f, 0.0f,  0.0f,
                    0.465641f,  0.890046f,  -0.007724f, 0.0f,  0.0f,
                    -0.095964f,  0.040773f,  1.014032f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                8,
                new[]
                {
                    0.539009f,  0.082546f,  -0.007136f,  0.0f,  0.0f,
                    0.579343f,  0.866121f,  -0.011959f,  0.0f,  0.0f,
                    -0.118352f,  0.051332f,  1.019095f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                10,
                new[]
                {
                    0.458064f,  0.092785f,  -0.007494f,  0.0f,  0.0f,
                    0.679578f,  0.846313f,  -0.016807f,  0.0f,  0.0f,
                    -0.137642f,  0.060902f,  1.024301f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                12,
                new[]
                {
                    0.385450f,  0.100526f,  -0.007442f,  0.0f,  0.0f,
                    0.769005f,  0.829802f,  -0.022190f,  0.0f,  0.0f,
                    -0.154455f,  0.069673f,  1.029632f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                14,
                new[]
                {
                    0.319627f,  0.106241f,  -0.007025f,  0.0f,  0.0f,
                    0.849633f,  0.815969f,  -0.028051f,  0.0f,  0.0f,
                    -0.169261f,  0.077790f,  1.035076f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                16,
                new[]
                {
                    0.259411f,  0.110296f,  -0.006276f,  0.0f,  0.0f,
                    0.923008f,  0.804340f,  -0.034346f,  0.0f,  0.0f,
                   -0.182420f,  0.085364f,  1.040622f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                18,
                new[]
                {
                    0.203876f,  0.112975f,  -0.005222f,  0.0f,  0.0f,
                    0.990338f,  0.794542f,  -0.041043f,  0.0f,  0.0f,
                    -0.194214f,  0.092483f,  1.046265f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },
            {
                20,
                new[]
                {
                    0.152286f,  0.114503f,  -0.003882f,  0.0f,  0.0f,
                    1.052583f,  0.786281f,  -0.048116f,  0.0f,  0.0f,
                    -0.204868f,  0.099216f,  1.051998f, 0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f,
                     0.0f,       0.0f,        0.0f,      0.0f,  0.0f
                }
            },

        };
        #endregion


        #endregion

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
                return;
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                                           Color.Black,
                                                           Color.DeepSkyBlue,
                                                           90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        public FQS()
        {
            InitializeComponent();
            LoadOptions();
            RefreshImage();
        }

        public void LoadOptions()
        {
            if (File.Exists(optionsPath))
            {
                String options = File.ReadAllText(optionsPath);
                if (options == null)
                    return;
                String[] data = options.Split('^');
                printImagePath = data[1];

                if (data.Length > 3)
                    RedCustomColorValue = ColorTranslator.FromHtml(data[3]);
                else
                    RedCustomColorValue = Color.Red;

                if (data.Length > 5)
                    GreenCustomColorValue = ColorTranslator.FromHtml(data[5]);
                else
                    GreenCustomColorValue = Color.Green;

                if (data.Length > 7)
                    BlueCustomColorValue = ColorTranslator.FromHtml(data[7]);
                else
                    BlueCustomColorValue = Color.Blue;
            }
        }

        public void SaveOptions()
        {
            if (File.Exists(optionsPath))
            {
                string dataImagePath = "1.Print Image Location = ^" + printImagePath;
                string dataRedColor = "^2. Red Color Custom Value = ^" + ColorTranslator.ToHtml(RedCustomColorValue);
                string dataGreenColor = "^3. Green Color Custom Value = ^" + ColorTranslator.ToHtml(GreenCustomColorValue);
                string dataBlueColor = "^4. Blue Color Custom Value = ^" + ColorTranslator.ToHtml(BlueCustomColorValue);
                string data = dataImagePath + dataRedColor + dataGreenColor + dataBlueColor;
                File.WriteAllText(optionsPath, data);
            }
            else
            {
                string dataImagePath = "1.Print Image Location = ^" + printImagePath;
                string dataRedColor = "^2. Red Color Custom Value = ^" + ColorTranslator.ToHtml(RedCustomColorValue);
                string dataGreenColor = "^3. Green Color Custom Value = ^" + ColorTranslator.ToHtml(GreenCustomColorValue);
                string dataBlueColor = "^4. Blue Color Custom Value = ^" + ColorTranslator.ToHtml(BlueCustomColorValue);
                string data = dataImagePath + dataRedColor + dataGreenColor + dataBlueColor;
                File.Create(optionsPath).Close();
                File.WriteAllText(optionsPath, data);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeHotkeys();
            ToggleHotkeys(true);
        }


        private void OnF1Action()
        {
            SetDeuteronormaly();
        }

        private void OnF2Action()
        {
            SetProtanomaly();
        }

        public void OnF3Action()
        {
            SetProtanopia();
        }

        private void OnF4Action()
        {
            SetDeuteranopia();
        }

        private void OnF5Action()
        {
            SetTritanopia();
        }

        private void OnF6Action()
        {
            SetTritanomaly();
        }

        private void OnF7Action()
        {
            SetAchromatopsia();
        }

        private void OnF8Action()
        {
            SetScreenDefault();
        }

        private void OnF9Action()
        {
            SetCustomScreen();
        }

        private void OnPrintScreen()
        {
            CaptureScreenNormal();
            RefreshImage();
        }

        private void OnPlus()
        {
            waveLenghtSeverity += 2;
            if (waveLenghtSeverity >= 20)
                waveLenghtSeverity = 20;

            SetPreviousFilter(false);
        }

        private void OnMinus()
        {
            waveLenghtSeverity -= 2;
            if (waveLenghtSeverity <= 0)
                waveLenghtSeverity = 0;

            SetPreviousFilter(false);
        }

        private void SetAchromatopsia()
        {
            SetScreenDefault();
            previousFilter = EpreviousFilter.achromatopsia;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[] 
                {
                    redScale,   redScale,   redScale,   0.0f,  0.0f,
                    greenScale, greenScale, greenScale, 0.0f,  0.0f,
                    blueScale,  blueScale,  blueScale,  0.0f,  0.0f,
                    0.0f,       0.0f,       0.0f,       1.0f,  0.0f,
                    0.0f,       0.0f,       0.0f,       0.0f,  1.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        private void SetPreviousFilter(bool resetSeverity = true)
        {
            switch(previousFilter)
            {
                case EpreviousFilter.deuteranomaly:
                    SetDeuteronormaly(resetSeverity);
                    break;
                case EpreviousFilter.protanomaly:
                    SetProtanomaly(resetSeverity);
                    break;
                case EpreviousFilter.protanopia:
                    SetProtanopia(resetSeverity);
                    break;
                case EpreviousFilter.deuteranopia:
                    SetDeuteranopia(resetSeverity);
                    break;
                case EpreviousFilter.tritanopia:
                    SetTritanopia(resetSeverity);
                    break;
                case EpreviousFilter.tritanomaly:
                    SetTritanomaly(resetSeverity);
                    break;
                case EpreviousFilter.achromatopsia:
                    SetAchromatopsia();
                    break;
                case EpreviousFilter.normal:
                    SetScreenDefault();
                    break;
                case EpreviousFilter.custom:
                    SetCustomScreen();
                    break;
            }
        }

        private void FQS_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RefreshImage()
        {
            screensPictureBox = new PictureBox[6] { pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24 };
            if (Directory.Exists(printImagePath) == false)
                return;
            string[] files = Directory.GetFiles(printImagePath, ".");
            List<string> imageFiles = new List<string>();
            foreach (string filename in files)
            {
                if (Regex.IsMatch(filename, @".jpg|.png|.gif$"))
                    imageFiles.Add(filename);
            }
            foreach (PictureBox picture in screensPictureBox)
            {
                picture.Image = null;
                picture.BackColor = Color.FromArgb(50, Color.Black);
            }

            for (int i = 0; i < imageFiles.Count; i++)
            {
                if (i > 5)
                    break;
                screensPictureBox[i].SizeMode = PictureBoxSizeMode.StretchImage;
                screensPictureBox[i].Load(imageFiles[i]);
            }
        }

        private void setScreenshotPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = printImagePath;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    printImagePath = fbd.SelectedPath.ToString();
                    SaveOptions();
                    RefreshImage();
                }
            }
        }

        private void openScreenshotFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(printImagePath);
            System.Diagnostics.Process.Start(printImagePath);
        }

        string firstImageName;
        string secongImageName;

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(printImagePath);
        }

        private void patronsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {
            OpenCustomFilterForm();
        }

        private void OpenCustomFilterForm()
        {
            customFilter = new Form2(this, optionsPath, printImagePath);
            this.Hide();
            customFilter.ShowDialog();
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            OpenCustomFilterForm();
        }

        private void Screen_Click(object sender, EventArgs e)
        {
            OpenCustomFilterForm();
        }

        private void FQS_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {

        }

        private void setCustomFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCustomFilterForm();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        string firstImagePath;
        string secondImagePath;
        private void CaptureScreenNormal()
        {
            String processName = System.AppDomain.CurrentDomain.FriendlyName;
            for(int i = 0; i < 500; i++)
            {
                firstImageName = previousFilter.ToString() + " filter " + i + "01.jpg";
                firstImageName.Replace('.', '~');
                if (!File.Exists(printImagePath + @"/" + firstImageName))
                {
                    secongImageName = previousFilter.ToString() + " filter " + i + "02.jpg";
                    break;
                }
            }
            Graphics myGraphics = this.CreateGraphics();
            Size bitmapSize = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Bitmap memoryImage = new Bitmap(bitmapSize.Width, bitmapSize.Height, myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(0, 0, 0, 0, bitmapSize);
            Bitmap ResizedBitmap = new Bitmap(memoryImage, bitmapSize.Width/2, bitmapSize.Height/2);
            firstImagePath = printImagePath + @"\" + firstImageName;
            secondImagePath = printImagePath + @"\" + secongImageName;
            ResizedBitmap.Save(firstImagePath);
            CaptureScreenUpdated();
        }

        private void CaptureScreenUpdated()
        {
            NativeMethods.MagUninitialize();
            Graphics myGraphics = this.CreateGraphics();
            Size bitmapSize = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Bitmap memoryImage = new Bitmap(bitmapSize.Width, bitmapSize.Height, myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(0, 0, 0, 0, bitmapSize);
            Bitmap ResizedBitmap = new Bitmap(memoryImage, bitmapSize.Width / 2, bitmapSize.Height/2);

            ResizedBitmap.Save(secondImagePath);
            SetPreviousFilter(false);
            MergeTwoImages();
        } 
        
        private void MergeTwoImages()
        {
            Image firstImage = Image.FromFile(firstImagePath);
            Image secondImage = Image.FromFile(secondImagePath);
            Bitmap mergedBitmap = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width /2, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Image mergedImage = (Image)mergedBitmap;

            Graphics myGraphics = Graphics.FromImage(mergedImage);

            myGraphics.Clear(Color.White);
            myGraphics.DrawImage(firstImage, new Point(0, 0));
            myGraphics.DrawImage(secondImage, new Point(0, firstImage.Height));

            myGraphics.Dispose();
            firstImage.Dispose();
            secondImage.Dispose();
            mergedImage.Save(firstImagePath);
            File.Delete(secondImagePath);
        }

        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

        //https://pl.wikipedia.org/wiki/Deuteranomalia
        private void SetDeuteronormaly(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 10;

            SetScreenDefault();
            previousFilter = EpreviousFilter.deuteranomaly;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f, multiplicator = 1;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    1,    0.5f,    0,  0,  0,
                    0,    0.5f,    0,  0,  0,
                    0,    0,    1,  0,  0,
                    0,    0,    0,  0,  0,
                    0,    0,    0,  0,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        //https://pl.wikipedia.org/wiki/Protanopia
        private void SetProtanomaly(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 10;

            SetScreenDefault();
            previousFilter = EpreviousFilter.protanomaly;

            float[] protanomalyMatrix = null;
            protanomalyMatrixDictionary.TryGetValue(waveLenghtSeverity, out protanomalyMatrix);

            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = protanomalyMatrix
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }


        private void SetProtanopia(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 20;

            SetScreenDefault();
            previousFilter = EpreviousFilter.protanopia;

            float[] protanomalyMatrix = null;
            protanomalyMatrixDictionary.TryGetValue(waveLenghtSeverity, out protanomalyMatrix);

            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = protanomalyMatrix
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        private void SetDeuteranopia(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 20;

            SetScreenDefault();
            previousFilter = EpreviousFilter.deuteranopia;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    1,         1,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       1,      0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        private void SetTritanopia(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 20;

            SetScreenDefault();
            previousFilter = EpreviousFilter.tritanopia;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    1,         0,       0,      0,  0.0f,
                    0,         1,       1,      0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        private void SetTritanomaly(bool resetSeverity = true)
        {
            if (resetSeverity)
                waveLenghtSeverity = 10;

            SetScreenDefault();
            previousFilter = EpreviousFilter.tritanomaly;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    1,         0,       0,      0,  0.0f,
                    0,         1,       0.5f,   0,  0.0f,
                    0,         0,       0.5f,   0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        public void SetScreenDefault()
        {
            previousFilter = EpreviousFilter.normal;
            NativeMethods.MagUninitialize();
        }

        public void SetCustomScreen()
        {
            SetScreenDefault();
            previousFilter = EpreviousFilter.custom;
            Color redColor = RedCustomColorValue;
            Color greenColor = GreenCustomColorValue;
            Color BlueColor = BlueCustomColorValue;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    redColor.R/255f,         redColor.G/255f,      redColor.B/255f,     0,  0.0f,
                    greenColor.R/255f,       greenColor.G/255f,    greenColor.B/255f,   0,  0.0f,
                    BlueColor.R/255f,        BlueColor.G/255f,     BlueColor.B/255f,    0,  0.0f,
                    0,         0,       0,      0,  0.0f,
                    0,         0,       0,      0,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        #region INPUT
        private void InitializeHotkeys()
        {
            F1Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F1, this);
            F2Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F2, this);
            F3Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F3, this);
            F4Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F4, this);
            F5Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F5, this);
            F6Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F6, this);
            F7Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F7, this);
            F8Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F8, this);
            F9Key = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.F9, this);
            PrintScreenKey = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.PrintScreen, this);
            PlusKey = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.Oemplus, this);
            MinusKey = new Hotkeys.GlobalHotkey(Constants.NOMOD, Keys.OemMinus, this);
        }

        private void ToggleHotkeys(bool enabled)
        {
            if (enabled)
            {
                F1Key.Register(F1Id);
                F3Key.Register(F3Id);
                F2Key.Register(F2Id);
                F4Key.Register(F4Id);
                F5Key.Register(F5Id);
                F6Key.Register(F6Id);
                F7Key.Register(F7Id);
                F8Key.Register(F8Id);
                F9Key.Register(F9Id);
                PrintScreenKey.Register(PrintScreenId);
                PlusKey.Register(PlusId);
                MinusKey.Register(MinusId);
            }
            else
            {
                F1Key.Unregiser(F1Id);
                F3Key.Unregiser(F3Id);
                F2Key.Unregiser(F2Id);
                F4Key.Unregiser(F4Id);
                F5Key.Unregiser(F5Id);
                F7Key.Unregiser(F7Id);
                F8Key.Unregiser(F8Id);
                F9Key.Unregiser(F9Id);
                PrintScreenKey.Unregiser(PrintScreenId);
                PlusKey.Unregiser(PlusId);
                MinusKey.Unregiser(MinusId);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                int id = m.WParam.ToInt32();

                if (id == F1Id)
                    OnF1Action();
                else if (id == F2Id)
                    OnF2Action();
                else if (id == F3Id)
                    OnF3Action();
                else if (id == F4Id)
                    OnF4Action();
                else if (id == F5Id)
                    OnF5Action();
                else if (id == F6Id)
                    OnF6Action();
                else if (id == F7Id)
                    OnF7Action();
                else if (id == F8Id)
                    OnF8Action();
                else if (id == F9Id)
                    OnF9Action();
                else if (id == PrintScreenId)
                    OnPrintScreen();
                else if (id == PlusId)
                    OnPlus();
                else if (id == MinusId)
                    OnMinus();

            }
            base.WndProc(ref m);
        }

        private enum EpreviousFilter
        {
            normal, deuteranomaly, protanomaly, protanopia, deuteranopia, tritanopia, tritanomaly, achromatopsia, custom
        }
        #endregion
    }
}
