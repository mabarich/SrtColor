using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SrtColor
{
    public partial class Form1 : Form
    {
        ArrayList contatori = new ArrayList();
        ArrayList tempi = new ArrayList();
        ArrayList testi = new ArrayList();
        ArrayList finef = new ArrayList();
        string directory="", colore="";
        public double millisecondi=0;
        int filecount = 0;
        bool add = false;
        bool edit = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void leggi()
        {
            string curFile = "Settings.txt";
            bool exists = File.Exists(curFile);
            if (exists)
            {
                StreamReader sr = new StreamReader(curFile, Encoding.GetEncoding(1252));
                directory = sr.ReadLine();
                colore = sr.ReadLine();
                sr.Close();
            }
            textBox1.Text = colore;
            openFileDialog1.Filter = "Srt Files | *.srt";
            openFileDialog1.DefaultExt = "srt";
            if (!exists)
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                openFileDialog1.InitialDirectory = directory;
            openFileDialog1.ShowDialog();
            String line, lastline = "";
            string name = openFileDialog1.FileName;
            if (!add)
                this.Text = Path.GetFileName(name);
            else
                this.Text = this.Text + "+" + Path.GetFileName(name);
            try
            {
                StreamReader sr = new StreamReader(name, Encoding.GetEncoding(1252));
                directory = Path.GetDirectoryName(name);
                int cont = 1;
                line = sr.ReadLine();
                while (line != null)
                {
                    if (cont == 1)
                    {
                        try
                        {
                            if (line == "")
                            {
                                contatori.Add((Convert.ToInt32(contatori[contatori.Count - 1])) + 1);
                                cont++;
                            }
                            else if (contatori.Count == 0 || (Convert.ToInt32(line) == ((Convert.ToInt32(contatori[contatori.Count - 1])) + 1)))
                            {
                                contatori.Add(line);
                                cont++;
                            }
                            filecount++;
                        }
                        catch (Exception)
                        {
                            if (!line.Contains(" --> "))
                            {
                                cont = 4;
                                if (!line.StartsWith("-"))
                                    testi[testi.Count - 1] += " " + line;
                                else
                                    testi[testi.Count - 1] += "\n" + line;
                            }
                            else
                            {
                                cont = 3;
                                contatori.Add((Convert.ToInt32(contatori[contatori.Count - 1])) + 1);
                                tempi.Add(line);
                            }
                        }
                    }
                    else if (cont == 2)
                    {
                        if (!line.Contains(" --> "))
                        {
                            MessageBox.Show("Timing mancante alla riga " + contatori[contatori.Count - 1] + ".", "Timing mancante", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            throw new Exception();
                        }
                        else
                        {
                            tempi.Add(line);
                            cont++;
                        }
                    }
                    else if (cont > 2)
                    {
                        if (line == "")
                        {
                            if (cont == 3)
                                testi.Add("");
                            cont = 1;
                        }
                        else
                        {
                            if (cont == 3)
                                testi.Add(line);
                            else
                            {
                                if (!line.Contains(" --> "))
                                {
                                    if (!line.StartsWith("-"))
                                        testi[testi.Count - 1] += " " + line;
                                    else
                                        testi[testi.Count - 1] += "\n" + line;
                                }
                                else
                                {
                                    if (testi[testi.Count - 1] != lastline)
                                        testi[testi.Count - 1] = testi[testi.Count - 1].ToString().Replace(" " + lastline, "");
                                    else
                                        testi[testi.Count - 1] = "";
                                    contatori.Add(lastline);
                                    tempi.Add(line);
                                    cont = 2;
                                }
                            }
                            testi[testi.Count - 1] = testi[testi.Count - 1].ToString().Replace("  ", " ");
                            cont++;
                        }
                    }
                    lastline = line;
                    line = sr.ReadLine();
                }
                finef.Add(filecount - 1);
                sr.Close();
            }
            catch (Exception)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            contatori = new ArrayList();
            tempi = new ArrayList();
            testi = new ArrayList();
            finef = new ArrayList();
            dataGridView1.Visible = false;
            dataGridView1.RowCount = 1;
            button6.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            millisecondi=0;
            filecount = 0;
            add = false;
            edit = true;
            leggi();
        }

        private void colora(String codice)
        {
            for (int i = 0; i < testi.Count; i++)
            {
                if (testi[i].ToString()!="")
                {
                    if (!testi[i].ToString().Contains("<font color="))
                        testi[i] = "<font color=\""+codice+"\">"+testi[i];
                    else 
                    {
                        int pos = testi[i].ToString().IndexOf(">");
                        int lunghezza=testi[i].ToString().Length;
                        string nuovo="<font color=\""+codice+"\">";
                        testi[i] = nuovo + testi[i].ToString().Substring(pos + 1, lunghezza - pos - 1);
                    }
                }
            }
        }

        private void sostituisci()
        {
            for (int i = 0; i < testi.Count; i++)
            {
                testi[i] = testi[i].ToString().Replace("  ", " ");
                testi[i] = testi[i].ToString().Replace("[", "");
                testi[i] = testi[i].ToString().Replace("]", "");
                testi[i] = testi[i].ToString().Replace("<b>", "");
                testi[i] = testi[i].ToString().Replace("</b>", "");
                testi[i] = testi[i].ToString().Replace("<i>", "");
                testi[i] = testi[i].ToString().Replace("</i>", "");
                testi[i] = testi[i].ToString().Replace("a'", "à");
                testi[i] = testi[i].ToString().Replace("i'", "ì");
                testi[i] = testi[i].ToString().Replace("o'", "ò");
                testi[i] = testi[i].ToString().Replace("pò", "po'");
                testi[i] = testi[i].ToString().Replace("u'", "ù");
                testi[i] = testi[i].ToString().Replace("che'", "ché");
                testi[i] = testi[i].ToString().Replace("ne'", "né");
                testi[i] = testi[i].ToString().Replace("e'", "è");
                testi[i] = testi[i].ToString().Replace("E'", "È");
                testi[i] = testi[i].ToString().Replace("A'", "À");
                testi[i] = testi[i].ToString().Replace("U'", "Ù");
                testi[i] = testi[i].ToString().Replace("I'", "Ì");
                testi[i] = testi[i].ToString().Replace("Suiton...", "Suiton:");
                testi[i] = testi[i].ToString().Replace("Suiton:", "Suiton:");
                testi[i] = testi[i].ToString().Replace("Katon...", "Katon:");
                testi[i] = testi[i].ToString().Replace("Katon:", "Katon:");
                testi[i] = testi[i].ToString().Replace("Doton...", "Doton:");
                testi[i] = testi[i].ToString().Replace("Doton:", "Doton:");
                testi[i] = testi[i].ToString().Replace("Fuuton...", "Fuuton:");
                testi[i] = testi[i].ToString().Replace("Fuuton:", "Fuuton:");
                testi[i] = testi[i].ToString().Replace("Futon...", "Futon:");
                testi[i] = testi[i].ToString().Replace("Futon:", "Futon:");
                testi[i] = testi[i].ToString().Replace("Raiton...", "Raiton:");
                testi[i] = testi[i].ToString().Replace("Raiton:", "Raiton:");
                testi[i] = testi[i].ToString().Replace("Mokuton...", "Mokuton:");
                testi[i] = testi[i].ToString().Replace("Mokuton:", "Mokuton:");
                testi[i] = testi[i].ToString().Replace("Enton...", "Enton:");
                testi[i] = testi[i].ToString().Replace("Enton:", "Enton:");
                testi[i] = testi[i].ToString().Replace("ninja", "Ninja");
                testi[i] = testi[i].ToString().Replace("genjutsu", "Genjutsu");
                testi[i] = testi[i].ToString().Replace("taijutsu", "Taijutsu");
                testi[i] = testi[i].ToString().Replace("ninjutsu", "Ninjutsu");
                testi[i] = testi[i].ToString().Replace("gli shinobi", "i Ninja");
                testi[i] = testi[i].ToString().Replace("Gli shinobi", "I Ninja");
                testi[i] = testi[i].ToString().Replace("lo shinobi", "il Ninja");
                testi[i] = testi[i].ToString().Replace("Lo shinobi", "Il Ninja");
                testi[i] = testi[i].ToString().Replace("uno shinobi", "un Ninja");
                testi[i] = testi[i].ToString().Replace("Uno shinobi", "Un Ninja");
                testi[i] = testi[i].ToString().Replace("shinobi", "Ninja");
                testi[i] = testi[i].ToString().Replace("ANBU", "Anbu");
                testi[i] = testi[i].ToString().Replace("genin", "Genin");
                testi[i] = testi[i].ToString().Replace("chunin", "Chuunin");
                testi[i] = testi[i].ToString().Replace("chuunin", "Chuunin");
                testi[i] = testi[i].ToString().Replace("jonin", "Jonin");
                testi[i] = testi[i].ToString().Replace("nukenin", "Nukenin");
                testi[i] = testi[i].ToString().Replace("mukenin", "Nukenin");
                testi[i] = testi[i].ToString().Replace("Monocoda", "Ichibi");
                testi[i] = testi[i].ToString().Replace("Due-Code", "Nibi");
                testi[i] = testi[i].ToString().Replace("Tre-Code", "Sanbi");
                testi[i] = testi[i].ToString().Replace("Quattro-Code", "Yonbi");
                testi[i] = testi[i].ToString().Replace("Cinque-Code", "Gobi");
                testi[i] = testi[i].ToString().Replace("Sei-Code", "Rokubi");
                testi[i] = testi[i].ToString().Replace("Sette-Code", "Nanabi");
                testi[i] = testi[i].ToString().Replace("Otto-Code", "Hachibi");
                testi[i] = testi[i].ToString().Replace("Nove-Code", "Kyuubi");
                testi[i] = testi[i].ToString().Replace("Dieci-Code", "Juubi");
                testi[i] = testi[i].ToString().Replace("dal villaggio della Foglia", "da Konoha");
                testi[i] = testi[i].ToString().Replace("del villaggio della Foglia", "di Konoha");
                testi[i] = testi[i].ToString().Replace("Dal villaggio della Foglia", "Da Konoha");
                testi[i] = testi[i].ToString().Replace("al villaggio della Foglia", "a Konoha");
                testi[i] = testi[i].ToString().Replace("Al villaggio della Foglia", "A Konoha");
                testi[i] = testi[i].ToString().Replace("dal Villaggio della Foglia", "da Konoha");
                testi[i] = testi[i].ToString().Replace("del Villaggio della Foglia", "di Konoha");
                testi[i] = testi[i].ToString().Replace("Dal Villaggio della Foglia", "Da Konoha");
                testi[i] = testi[i].ToString().Replace("al Villaggio della Foglia", "a Konoha");
                testi[i] = testi[i].ToString().Replace("Al Villaggio della Foglia", "A Konoha");
                testi[i] = testi[i].ToString().Replace("dalla Foglia", "da Konoha");
                testi[i] = testi[i].ToString().Replace("della Foglia", "di Konoha");
                testi[i] = testi[i].ToString().Replace("Dalla Foglia", "Da Konoha");
                testi[i] = testi[i].ToString().Replace("alla Foglia", "a Konoha");
                testi[i] = testi[i].ToString().Replace("Alla Foglia", "A Konoha");
                testi[i] = testi[i].ToString().Replace("La Foglia", "Konoha");
                testi[i] = testi[i].ToString().Replace("la Foglia", "Konoha");
                testi[i] = testi[i].ToString().Replace("dal villaggio della Sabbia", "da Suna");
                testi[i] = testi[i].ToString().Replace("del villaggio della Sabbia", "di Suna");
                testi[i] = testi[i].ToString().Replace("Dal villaggio della Sabbia", "Da Suna");
                testi[i] = testi[i].ToString().Replace("al villaggio della Sabbia", "a Suna");
                testi[i] = testi[i].ToString().Replace("Al villaggio della Sabbia", "A Suna");
                testi[i] = testi[i].ToString().Replace("dal Villaggio della Sabbia", "da Suna");
                testi[i] = testi[i].ToString().Replace("del Villaggio della Sabbia", "di Suna");
                testi[i] = testi[i].ToString().Replace("Dal Villaggio della Sabbia", "Da Suna");
                testi[i] = testi[i].ToString().Replace("al Villaggio della Sabbia", "a Suna");
                testi[i] = testi[i].ToString().Replace("Al Villaggio della Sabbia", "A Suna");
                testi[i] = testi[i].ToString().Replace("dalla Sabbia", "da Suna");
                testi[i] = testi[i].ToString().Replace("della Sabbia", "di Suna");
                testi[i] = testi[i].ToString().Replace("Dalla Sabbia", "Da Suna");
                testi[i] = testi[i].ToString().Replace("alla Sabbia", "a Suna");
                testi[i] = testi[i].ToString().Replace("Alla Sabbia", "A Suna");
                testi[i] = testi[i].ToString().Replace("La Sabbia", "Suna");
                testi[i] = testi[i].ToString().Replace("la Sabbia", "Suna");
                testi[i] = testi[i].ToString().Replace("dal villaggio della Roccia", "da Iwa");
                testi[i] = testi[i].ToString().Replace("del villaggio della Roccia", "di Iwa");
                testi[i] = testi[i].ToString().Replace("Dal villaggio della Roccia", "Da Iwa");
                testi[i] = testi[i].ToString().Replace("al villaggio della Roccia", "a Iwa");
                testi[i] = testi[i].ToString().Replace("Al villaggio della Roccia", "A Iwa");
                testi[i] = testi[i].ToString().Replace("dal Villaggio della Roccia", "da Iwa");
                testi[i] = testi[i].ToString().Replace("del Villaggio della Roccia", "di Iwa");
                testi[i] = testi[i].ToString().Replace("Dal Villaggio della Roccia", "Da Iwa");
                testi[i] = testi[i].ToString().Replace("al Villaggio della Roccia", "a Iwa");
                testi[i] = testi[i].ToString().Replace("Al Villaggio della Roccia", "A Iwa");
                testi[i] = testi[i].ToString().Replace("dalla Roccia", "da Iwa");
                testi[i] = testi[i].ToString().Replace("della Roccia", "di Iwa");
                testi[i] = testi[i].ToString().Replace("Dalla Roccia", "Da Iwa");
                testi[i] = testi[i].ToString().Replace("alla Roccia", "a Iwa");
                testi[i] = testi[i].ToString().Replace("Alla Roccia", "A Iwa");
                testi[i] = testi[i].ToString().Replace("La Roccia", "Iwa");
                testi[i] = testi[i].ToString().Replace("la Roccia", "Iwa");
                testi[i] = testi[i].ToString().Replace("dal villaggio della Nuvola", "da Kumo");
                testi[i] = testi[i].ToString().Replace("del villaggio della Nuvola", "di Kumo");
                testi[i] = testi[i].ToString().Replace("Dal villaggio della Nuvola", "Da Kumo");
                testi[i] = testi[i].ToString().Replace("al villaggio della Nuvola", "a Kumo");
                testi[i] = testi[i].ToString().Replace("Al villaggio della Nuvola", "A Kumo");
                testi[i] = testi[i].ToString().Replace("dal Villaggio della Nuvola", "da Kumo");
                testi[i] = testi[i].ToString().Replace("del Villaggio della Nuvola", "di Kumo");
                testi[i] = testi[i].ToString().Replace("Dal Villaggio della Nuvola", "Da Kumo");
                testi[i] = testi[i].ToString().Replace("al Villaggio della Nuvola", "a Kumo");
                testi[i] = testi[i].ToString().Replace("Al Villaggio della Nuvola", "A Kumo");
                testi[i] = testi[i].ToString().Replace("dalla Nuvola", "da Kiri");
                testi[i] = testi[i].ToString().Replace("della Nuvola", "di Kumo");
                testi[i] = testi[i].ToString().Replace("Dalla Nuvola", "Da Kumo");
                testi[i] = testi[i].ToString().Replace("alla Nuvola", "a Kumo");
                testi[i] = testi[i].ToString().Replace("Alla Nuvola", "A Kumo");
                testi[i] = testi[i].ToString().Replace("La Nuvola", "Kumo");
                testi[i] = testi[i].ToString().Replace("la Nuvola", "Kumo");
                testi[i] = testi[i].ToString().Replace("dal villaggio della Nebbia", "da Kiri");
                testi[i] = testi[i].ToString().Replace("del villaggio della Nebbia", "di Kiri");
                testi[i] = testi[i].ToString().Replace("Dal villaggio della Nebbia", "Da Kiri");
                testi[i] = testi[i].ToString().Replace("al villaggio della Nebbia", "a Kiri");
                testi[i] = testi[i].ToString().Replace("Al villaggio della Nebbia", "A Kiri");
                testi[i] = testi[i].ToString().Replace("dal Villaggio della Nebbia", "da Kiri");
                testi[i] = testi[i].ToString().Replace("del Villaggio della Nebbia", "di Kiri");
                testi[i] = testi[i].ToString().Replace("Dal Villaggio della Nebbia", "Da Kiri");
                testi[i] = testi[i].ToString().Replace("al Villaggio della Nebbia", "a Kiri");
                testi[i] = testi[i].ToString().Replace("Al Villaggio della Nebbia", "A Kiri");
                testi[i] = testi[i].ToString().Replace("dalla Nebbia", "da Kiri");
                testi[i] = testi[i].ToString().Replace("della Nebbia", "di Kiri");
                testi[i] = testi[i].ToString().Replace("Dalla Nebbia", "Da Kiri");
                testi[i] = testi[i].ToString().Replace("alla Nebbia", "a Kiri");
                testi[i] = testi[i].ToString().Replace("Alla Nebbia", "A Kiri");
                testi[i] = testi[i].ToString().Replace("La Nebbia", "Kiri");
                testi[i] = testi[i].ToString().Replace("la Nebbia", "Kiri");
                testi[i] = testi[i].ToString().Replace("Quel jutsu", "Quella tecnica");
                testi[i] = testi[i].ToString().Replace("quel jutsu", "quella tecnica");
                testi[i] = testi[i].ToString().Replace("Quei jutsu", "Quelle tecniche");
                testi[i] = testi[i].ToString().Replace("quei jutsu", "quelle tecniche");
                testi[i] = testi[i].ToString().Replace("Dei jutsu", "Delle tecniche");
                testi[i] = testi[i].ToString().Replace("Dai jutsu", "Dalle tecniche");
                testi[i] = testi[i].ToString().Replace("dei jutsu", "delle tecniche");
                testi[i] = testi[i].ToString().Replace("dai jutsu", "dalle tecniche");
                testi[i] = testi[i].ToString().Replace("Sui jutsu", "Sulle tecniche");
                testi[i] = testi[i].ToString().Replace("sui jutsu", "sulle tecniche");
                testi[i] = testi[i].ToString().Replace("di jutsu", "di tecnica");
                testi[i] = testi[i].ToString().Replace("Di jutsu", "Di tecniche");
                testi[i] = testi[i].ToString().Replace("I jutsu", "Le tecniche");
                testi[i] = testi[i].ToString().Replace("i jutsu", "le tecniche");
                testi[i] = testi[i].ToString().Replace("Un jutsu", "Una tecnica");
                testi[i] = testi[i].ToString().Replace("un jutsu", "una tecnica");
                testi[i] = testi[i].ToString().Replace("Il jutsu", "La tecnica");
                testi[i] = testi[i].ToString().Replace("il jutsu", "la tecnica");
                testi[i] = testi[i].ToString().Replace("chakra", "Chakra");
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter s = new StreamWriter("Settings.txt", false, Encoding.GetEncoding(1252));
            s.WriteLine(directory);
            if (textBox1.Text.Length > 2)
                s.Write(colore.Substring(1,colore.Length-1));
            s.Close();
            saveFileDialog1.Filter = "Srt Files | *.srt";
            saveFileDialog1.DefaultExt = "srt";
            saveFileDialog1.InitialDirectory = directory;
            saveFileDialog1.ShowDialog();
            string name = saveFileDialog1.FileName;
            try
            {
                StreamWriter sw = new StreamWriter(name, false, Encoding.GetEncoding(1252));

                for (int i = 0; i < contatori.Count; i++)
                {
                    sw.WriteLine(contatori[i]);
                    sw.WriteLine(tempi[i]);
                    sw.WriteLine(testi[i]);
                    if (testi[i].ToString() != "")
                        sw.WriteLine("");
                }
                sw.Close();
            }
            catch (Exception)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sostituisci();
            aggiorna();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            colore = "#" + textBox1.Text.Trim() ;
            colora(colore);
            aggiorna();
        }

        private void aggiorna()
        {
            if(dataGridView1.Visible)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                for (int i = 0; i < contatori.Count; i++)
                {
                    if (i < dataGridView1.RowCount-1)
                    {
                        dataGridView1.Rows[i].Cells[0].Value = tempi[i].ToString();
                        dataGridView1.Rows[i].Cells[1].Value = testi[i].ToString();
                    }
                    else
                    {
                        dataGridView1.Rows.Add(tempi[i].ToString(), testi[i].ToString());
                    }
                }
                for (int i=0; i<finef.Count;i++)
                    dataGridView1.Rows[Convert.ToInt32(finef[i])].DefaultCellStyle.BackColor = Color.Yellow;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (edit)
            {
                dataGridView1.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                button8.Visible = true;
                button9.Visible = true;
                button10.Visible = true;
                dataGridView1.ColumnCount = 2;
                dataGridView1.Columns[0].Name = "Time";
                dataGridView1.Columns[0].Width = dataGridView1.Width * 12 / 100;
                dataGridView1.Columns[0].Frozen = true;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[1].Name = "Text";
                dataGridView1.Columns[1].Width = dataGridView1.Width * 88 / 100;
                dataGridView1.Columns[1].Frozen = true;
                for (int i = 0; i < contatori.Count; i++)
                {
                    dataGridView1.Rows.Add(tempi[i].ToString(), testi[i].ToString());
                }
                for (int i = 0; i < finef.Count; i++)
                    dataGridView1.Rows[Convert.ToInt32(finef[i])].DefaultCellStyle.BackColor = Color.Yellow;
                edit = false;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if(col==0)
                tempi[row] = dataGridView1.Rows[row].Cells[0].Value;
            else if (col==1)
                testi[row] = dataGridView1.Rows[row].Cells[1].Value;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                for (int i=0; i<dataGridView1.SelectedRows.Count; i++)
                {
                    int riga = dataGridView1.SelectedRows[i].Index;
                    for (int j = riga + 1; j < contatori.Count; j++)
                    {
                        contatori[j] = Convert.ToInt32(contatori[j]) - 1;
                    }
                    contatori.RemoveAt(riga);
                    tempi.RemoveAt(riga);
                    testi.RemoveAt(riga);
                    for (int k = 0; k < finef.Count; k++)
                    {
                        if (riga <= Convert.ToInt32(finef[k]))
                        {
                            finef[k] = Convert.ToInt32(finef[k]) - 1;
                        }
                    }
                    filecount--;
                }
                aggiorna();
                dataGridView1.ClearSelection();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int riga = dataGridView1.SelectedRows[0].Index;
                dataGridView1.Rows.Insert(riga,1);
                contatori.Insert(riga, ""+(riga));
                tempi.Insert(riga, tempi[riga]);
                string colore = "";
                if (textBox1.Text != "")
                    colore = "<font color=\"#" + textBox1.Text+"\">";
                testi.Insert(riga, colore+"----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                for (int i=riga; i<contatori.Count; i++)
                {
                    contatori[i] = Convert.ToInt32(contatori[i])+1;
                }
                for (int i=0; i<finef.Count; i++)
                {
                    if (riga <= Convert.ToInt32(finef[i]))
                    {
                        finef[i] = Convert.ToInt32(finef[i]) + 1;
                    }
                }
                filecount++;
                aggiorna();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Form2 f2 = new Form2(this);
                f2.ShowDialog();
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    int riga = dataGridView1.SelectedRows[i].Index;
                    string tempo = tempi[riga].ToString();
                    string inizio = "";
                    string fine = "";
                    char[] caratteri=tempo.ToCharArray();
                    int j=0;
                    inizio += caratteri[j];
                    while (caratteri[j] != ' ' && caratteri[j + 1] != '-' && caratteri[j + 2] != '-' && caratteri[j + 3] != '>')
                    {
                        j++;
                        inizio += caratteri[j];
                    }
                    j=j + 6;
                    for (int k = j; k < caratteri.Length; k++)
                    {
                        fine += caratteri[k];
                    }
                    TimeSpan ts = TimeSpan.ParseExact(inizio,@"hh\:mm\:ss\,fff",CultureInfo.InvariantCulture);
                    double totlaMilliseconds = ts.TotalMilliseconds - millisecondi;
                    TimeSpan ts2 = TimeSpan.ParseExact(fine, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    double totlaMilliseconds2 = ts2.TotalMilliseconds - millisecondi;
                    if (totlaMilliseconds >= 0 && totlaMilliseconds2 >= 0)
                    {
                        TimeSpan t0 = TimeSpan.FromMilliseconds(totlaMilliseconds);
                        string answer = t0.ToString(@"hh\:mm\:ss\,fff");
                        TimeSpan t2 = TimeSpan.FromMilliseconds(totlaMilliseconds2);
                        string answer2 = t2.ToString(@"hh\:mm\:ss\,fff");
                        tempi[riga] = answer + " --> " + answer2;
                    }
                    aggiorna();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                Form2 f2 = new Form2(this);
                f2.ShowDialog();
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    int riga = dataGridView1.SelectedRows[i].Index;
                    string tempo = tempi[riga].ToString();
                    string inizio = "";
                    string fine = "";
                    char[] caratteri = tempo.ToCharArray();
                    int j = 0;
                    inizio += caratteri[j];
                    while (caratteri[j] != ' ' && caratteri[j + 1] != '-' && caratteri[j + 2] != '-' && caratteri[j + 3] != '>')
                    {
                        j++;
                        inizio += caratteri[j];
                    }
                    j = j + 6;
                    for (int k = j; k < caratteri.Length; k++)
                    {
                        fine += caratteri[k];
                    }
                    TimeSpan ts = TimeSpan.ParseExact(inizio, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    double totlaMilliseconds = ts.TotalMilliseconds + millisecondi;
                    TimeSpan ts2 = TimeSpan.ParseExact(fine, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    double totlaMilliseconds2 = ts2.TotalMilliseconds + millisecondi;
                    TimeSpan t0 = TimeSpan.FromMilliseconds(totlaMilliseconds);
                    string answer = t0.ToString(@"hh\:mm\:ss\,fff");
                    TimeSpan t2 = TimeSpan.FromMilliseconds(totlaMilliseconds2);
                    string answer2 = t2.ToString(@"hh\:mm\:ss\,fff");
                    tempi[riga] = answer + " --> " + answer2;
                    aggiorna();
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            add = true;
            leggi();
            add = false;
            aggiorna();
        }
    }
}