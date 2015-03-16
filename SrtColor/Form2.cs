using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SrtColor
{
    public partial class Form2 : Form
    {
        Form1 f1;
        public Form2(Form1 f)
        {
            InitializeComponent();
            f1 = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ora=maskedTextBox1.Text;
            if (ora.Length == 0)
                ora = "00";
            else if (ora.Length == 1)
                ora = "0" + ora;
            string minuti=maskedTextBox2.Text.Trim();
            if (minuti.Length == 0)
                minuti = "00";
            else if (minuti.Length == 1)
                minuti = "0" + minuti;
            string secondi = maskedTextBox3.Text.Trim();
            if (secondi.Length == 0)
                secondi = "00";
            else if (secondi.Length == 1)
                secondi = "0" + secondi;
            string millisecondi = maskedTextBox4.Text.Trim();
            if (millisecondi.Length == 0)
                millisecondi = "000";
            else if (millisecondi.Length == 1)
                millisecondi = "00" + millisecondi.Trim();
            else if (millisecondi.Length == 2)
                millisecondi = "0" + millisecondi;
            string tempo=ora+":"+minuti+":"+secondi+","+millisecondi;
            TimeSpan ts = TimeSpan.ParseExact(tempo, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
            double totlaMilliseconds = ts.TotalMilliseconds;
            f1.millisecondi = totlaMilliseconds;
            this.Close();
        }

        private void maskedTextBox3_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
    }
}
