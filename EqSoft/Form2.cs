using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Policy;

namespace EqSoft
{
    public partial class Form2 : Form
    {
        public FQS previousForm;
        public string optionsPath;
        public string printImagePath;
        public Color RedCustomColorValue = Color.Red;
        public Color GreenCustomColorValue = Color.Green;
        public Color BlueCustomColorValue = Color.Blue;
        public bool automaticPreview;

        public Form2(FQS previousForm, string optionPath, string printImagePath)
        {
            this.previousForm = previousForm;
            this.optionsPath = optionPath;
            this.printImagePath = printImagePath;
            InitializeComponent();
            previousForm.LoadOptions();
            SetPictureColor();
        }

        private void SetPictureColor()
        {
            pictureBox1.BackColor = previousForm.RedCustomColorValue;
            pictureBox2.BackColor = previousForm.GreenCustomColorValue;
            pictureBox3.BackColor = previousForm.BlueCustomColorValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetRedColor();
        }

        private void SetRedColor()
        {
            if (colorDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                pictureBox1.BackColor = colorDialog1.Color;
                previousForm.RedCustomColorValue = colorDialog1.Color;
                previousForm.SaveOptions();
                if (automaticPreview)
                    previousForm.SetCustomScreen();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetGreenColor();
        }

        private void SetGreenColor()
        {
            if (colorDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                pictureBox2.BackColor = colorDialog1.Color;
                previousForm.GreenCustomColorValue = colorDialog1.Color;
                previousForm.SaveOptions();
                if (automaticPreview)
                    previousForm.SetCustomScreen();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetBlueColor();
        }

        private void SetBlueColor()
        {
            if (colorDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                pictureBox3.BackColor = colorDialog1.Color;
                previousForm.BlueCustomColorValue = colorDialog1.Color;
                previousForm.SaveOptions();
                if (automaticPreview)
                    previousForm.SetCustomScreen();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            previousForm.LoadOptions();
            this.Hide();
            previousForm.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                previousForm.SetCustomScreen();
                automaticPreview = true;
            }
            else
            {
                previousForm.SetScreenDefault();
                automaticPreview = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SetRedColor();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SetGreenColor();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            SetBlueColor();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            previousForm.Show();
        }
    }
}
