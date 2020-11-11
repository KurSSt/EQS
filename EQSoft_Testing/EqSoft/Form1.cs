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

        private Hotkeys.GlobalHotkey F1Key, F2Key, F3Key, F4Key, F5Key, F6Key, F7Key, F8Key, F9Key, PrintScreenKey;
        private Int32 F1Id = 1, F2Id = 2, F3Id = 3, F4Id = 4, F5Id = 5, F6Id = 6, F7Id = 7, F8Id = 8, F9Id = 9, PrintScreenId = 13;
        private EpreviousFilter previousFilter;
        private string printImagePath = Application.StartupPath;
        string optionsPath = Application.StartupPath + @"\options.txt";
        PictureBox[] screensPictureBox = new PictureBox[6];

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

        private void LoadOptions()
        {
            if (File.Exists(optionsPath))
            {
                String options = File.ReadAllText(optionsPath);
                if (options == null)
                    return;
                String[] data = options.Split('^');
                printImagePath = data[1];
            }
        }

        private void SaveOptions()
        {
            if(File.Exists(optionsPath))
            {
                string data = "1.Print Image Location = ^" + printImagePath;
                File.WriteAllText(optionsPath, data);
            }
            else
            {
                string data = "1.Print Image Location = ^" + printImagePath;
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

        }

        private void OnPrintScreen()
        {
            CaptureScreenNormal();
            RefreshImage();
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

        private void SetPreviousFilter()
        {
            switch(previousFilter)
            {
                case EpreviousFilter.deuteranomaly:
                    SetDeuteronormaly();
                    break;
                case EpreviousFilter.protanomaly:
                    SetProtanomaly();
                    break;
                case EpreviousFilter.protanopia:
                    SetProtanopia();
                    break;
                case EpreviousFilter.deuteranopia:
                    SetDeuteranopia();
                    break;
                case EpreviousFilter.tritanopia:
                    SetTritanopia();
                    break;
                case EpreviousFilter.tritanomaly:
                    SetTritanomaly();
                    break;
                case EpreviousFilter.achromatopsia:
                    SetAchromatopsia();
                    break;
                case EpreviousFilter.normal:
                    SetScreenDefault();
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
            SetPreviousFilter();
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
        private void SetDeuteronormaly()
        {
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
        private void SetProtanomaly()
        {
            SetScreenDefault();
            previousFilter = EpreviousFilter.protanomaly;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    0.5f,  0,   0,  0.0f,  0.0f,
                    0.5f,  1,   0,  0.0f,  0.0f,
                    0,  0,   1,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }


        private void SetProtanopia()
        {
            SetScreenDefault();
            previousFilter = EpreviousFilter.protanopia;
            float redScale = 0.2126f, greenScale = 0.7152f, blueScale = 0.0722f;
            var magEffectInvert = new NativeMethods.MAGCOLOREFFECT
            {
                transform = new[]
                {
                    0,  0,   0,  0.0f,  0.0f,
                    1,  1,   0,  0.0f,  0.0f,
                    0,  0,   1,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f,
                    0,  0,   0,  0.0f,  0.0f
                }
            };

            NativeMethods.MagInitialize();
            NativeMethods.MagSetFullscreenColorEffect(ref magEffectInvert);
        }

        private void SetDeuteranopia()
        {
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

        private void SetTritanopia()
        {
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

        private void SetTritanomaly()
        {
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

        private void SetScreenDefault()
        {
            previousFilter = EpreviousFilter.normal;
            NativeMethods.MagUninitialize();
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
            }
            base.WndProc(ref m);
        }

        private enum EpreviousFilter
        {
            normal, deuteranomaly, protanomaly, protanopia, deuteranopia, tritanopia, tritanomaly, achromatopsia
        }
        #endregion
    }
}
