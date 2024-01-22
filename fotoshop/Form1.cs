﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.IO.Ports;
using System.Threading;
using System.Drawing.Imaging;

namespace fotoshop
{
    public partial class Form1 : Form
    {
        private BitovaMapa btm;
        List <BitovaMapa> oldBtms = new List<BitovaMapa>();
        private int positionInOld = 0;
        BitovaMapa[] prehravatBtm = new BitovaMapa[4];
        public OpenFileDialog ofd = new OpenFileDialog(); //používám jeden file dialog na celej kód
        public Form1()
        {
            btm = new BitovaMapa();
            InitializeComponent();
            btm.drawBitmap(new Point(0, 0), this);
            ofd.Title = "Otevřít"; ofd.Filter = "jpg obrázky (*.jpg)|*.jpg|Jpeg obrázky (*jpeg)|*.jpeg|png obrázky (*.png)|*.png";
        }
        private void soubor_zobrazit_foto_Click(object sender, EventArgs e)
        {
            btm.drawBitmap(new Point(0, 0), this);
        }
        private void soubor_otevrit_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                oldBtms.Insert(positionInOld, btm.copy());
                BitovaMapa Import = new BitovaMapa(ofd.FileName);
                btm.bitmap = Import.bitmap;
                btm.drawBitmap(new Point(0, 0), this);
            }
        }
        private void soubor_zobrazit_4_Click(object sender, EventArgs e)
        {
            btm.drawFour();
        }
        private void soubor_zobrazit_prehravat_Click(object sender, EventArgs e)
        {
            string ee = Environment.CurrentDirectory;
            prehravatBtm[0] = new BitovaMapa(ee + @"/hasagi.jpg"); prehravatBtm[1] = new BitovaMapa(ee + @"/neon.jpg"); prehravatBtm[2] = new BitovaMapa(ee + @"/bb.jpg"); prehravatBtm[3] = new BitovaMapa(ee + @"/neco.jpg");

            timer1.Interval = 2000;
            timer1.Tick += Timer1_Tick;
            timer1.Enabled = !timer1.Enabled;
            prehravat_i = 0;
            přehrávatFotoToolStripMenuItem.Text = (timer1.Enabled)? "Vypnout přehrávání": "Přehrávat foto";
        }
        int prehravat_i = 0;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (prehravat_i == 4)
            {
                prehravat_i = 0;
            }
            prehravatBtm[prehravat_i].size = this.Size;
            prehravatBtm[prehravat_i].drawBitmap(new Point(0, 0), this);
            prehravat_i++;

        }

        private void soubor_zpet_Click(object sender, EventArgs e)
        {
            if (oldBtms.Count == 0) { return; }
            this.Text = "Fotošop - navrácení úpravy";
            btm = oldBtms[positionInOld];
            positionInOld++;
            btm.drawBitmap(new Point(0, 0), this);
            this.Text = "Fotošop";
        }
        private void soubor_zobrazit_fotoovelikosti_Click(object sender, EventArgs e)
        {

        }
        #region Úpravy
        private void upravy_cernobili_cernobila_Click(object sender, EventArgs e)
        {
            oldBtms.Insert(positionInOld, btm.copy());//vysvětlim naživo xd moc komplikovaný
            Text = "Fotošop - načítání filtru";
            Bitmap editedBmp = btm.bitmap;
            Color c;
            int rgb;
            for (int y = 0; y < editedBmp.Height; y++)
            {
                for (int x = 0; x < editedBmp.Width; x++)
                {
                    c = editedBmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    editedBmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            btm.bitmap = editedBmp;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
        private void upravy_cernobili_bilacerna_Click(object sender, EventArgs e)
        {
            oldBtms.Insert(positionInOld, btm.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap editedBmp = btm.bitmap;
            for (int y = 0; y < editedBmp.Height; y++)
                for (int x = 0; x < editedBmp.Width; x++)
                {
                    if (btm.svetelnost(editedBmp.GetPixel(x, y)) > 160)
                    {
                        editedBmp.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        editedBmp.SetPixel(x, y, Color.Black);
                    }
                }
            btm.bitmap = editedBmp;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
        private void upravy_cernobili_petodstinu_Click(object sender, EventArgs e)
        {
            oldBtms.Insert(positionInOld, btm.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap editedBmp = btm.bitmap;
            for (int y = 0; y < editedBmp.Height; y++)
                for (int x = 0; x < editedBmp.Width; x++)
                {
                    if (btm.svetelnost(editedBmp.GetPixel(x, y)) < 51)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                    }
                    else if (btm.svetelnost(editedBmp.GetPixel(x, y)) < 102)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, 51, 51, 51));
                    }
                    else if (btm.svetelnost(editedBmp.GetPixel(x, y)) < 153)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, 102, 102, 102));
                    }
                    else if (btm.svetelnost(editedBmp.GetPixel(x, y)) < 204)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, 153, 153, 153));
                    }
                    else
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, 204, 204, 204));
                    }
                }
            btm.bitmap = editedBmp;
            btm.drawBitmap(new Point(0,0), this);
            Text = "Fotošop";
        }
        private void upravy_odstinybarvy_5odstinu1barvy_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            oldBtms.Insert(positionInOld, btm.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap editedBmp = btm.bitmap;
            for (int y = 0; y < editedBmp.Height; y++)
                for (int x = 0; x < editedBmp.Width; x++)
                {
                    Color pixel = editedBmp.GetPixel(x, y);
                    Color clr = cd.Color;
                    if (btm.svetelnost(pixel) < 51)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(clr.R * 0.4), Convert.ToInt32(clr.G * 0.4), Convert.ToInt32(clr.B * 0.4)));
                    }
                    else if (btm.svetelnost(pixel) < 102)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(clr.R * 0.5), Convert.ToInt32(clr.G * 0.5), Convert.ToInt32(clr.B * 0.5)));
                    }
                    else if (btm.svetelnost(pixel) < 153)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(clr.R * 0.7), Convert.ToInt32(clr.G * 0.7), Convert.ToInt32(clr.B * 0.7)));
                    }
                    else if (btm.svetelnost(pixel) < 204)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(clr.R * 0.9), Convert.ToInt32(clr.G * 0.9), Convert.ToInt32(clr.B * 0.9)));
                    }
                    else
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, clr.R, clr.G, clr.B));
                    }
                }
            btm.bitmap = editedBmp;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
        private void upravy_odstinybarvy_5barev_Click(object sender, EventArgs e)
        {
            ColorDialog[] cd = new ColorDialog[5];//tohle je taková monstrozita
            cd[0] = new ColorDialog();//ale přepisovat to je moc práce
            if (cd[0].ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            cd[1] = new ColorDialog();
            if (cd[1].ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            cd[2] = new ColorDialog();
            if (cd[2].ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            cd[3] = new ColorDialog();
            if (cd[3].ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            cd[4] = new ColorDialog();
            if (cd[4].ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání barvy"); return; }

            oldBtms.Insert(positionInOld, btm.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap editedBmp = btm.bitmap;
            for (int y = 0; y < editedBmp.Height; y++)
                for (int x = 0; x < editedBmp.Width; x++)
                {
                    Color pixel = editedBmp.GetPixel(x, y);
                    Color clr = cd[0].Color;
                    if (btm.svetelnost(pixel) < 51)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(cd[0].Color.R), Convert.ToInt32(cd[0].Color.G), Convert.ToInt32(cd[0].Color.B)));
                    }
                    else if (btm.svetelnost(pixel) < 102)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(cd[1].Color.R), Convert.ToInt32(cd[1].Color.G), Convert.ToInt32(cd[1].Color.B)));
                    }
                    else if (btm.svetelnost(pixel) < 153)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(cd[2].Color.R), Convert.ToInt32(cd[2].Color.G), Convert.ToInt32(cd[2].Color.B)));
                    }
                    else if (btm.svetelnost(pixel) < 204)
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, Convert.ToInt32(cd[3].Color.R), Convert.ToInt32(cd[3].Color.G), Convert.ToInt32(cd[3].Color.B)));
                    }
                    else
                    {
                        editedBmp.SetPixel(x, y, Color.FromArgb(255, cd[4].Color.R, cd[4].Color.G, cd[4].Color.B));
                    }
                }
            btm.bitmap = editedBmp;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
        #region Reliéf
        private void upravy_relief_zhorazleva_Click(object sender, EventArgs e)
        {
            reliefFilter(btm, -4, -2);
        }
        private void upravy_relief_zhorazprava_Click(object sender, EventArgs e)
        {
            reliefFilter(btm, 4, -2);
        }
        private void upravy_relief_zdolazprava_Click(object sender, EventArgs e)
        {
            reliefFilter(btm, 4, 2);
        }
        private void upravy_relief_zdolazleva_Click(object sender, EventArgs e)
        {
            reliefFilter(btm, -4, 2);
        }
        private void upravy_relief_custom_Click(object sender, EventArgs e)
        {
            Input_Relief input_Relief = new Input_Relief(0);
            if (input_Relief.ShowDialog() != DialogResult.OK) { return; }
            reliefFilter(btm, input_Relief.SmerX, input_Relief.SmerY);
        }
        //udělá reliéf podle nastavení smerX a smerY, pak rovnou vykreslí (taky zaznamenává změny pro mojí zpět funkci)
        private BitovaMapa reliefFilter(BitovaMapa bmp, int smerX, int smerY) //smerX a Y od šmída byli -4 a -2 (jeho zhora zleva)
        {
            oldBtms.Insert(positionInOld, bmp.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap bit1 = new Bitmap(bmp.copy().bitmap);
            Bitmap bit2 = new Bitmap(bmp.copy().bitmap);//výsledek se bude zobrazovat tady
            Color pixel1;
            Color pixel2;
            int svetlost;
            for (int i = 0; i < bit1.Width; i++)
            {
                for (int j = 0; j < bit1.Height; j++)
                {
                    //pokud jsme na okraji bitmapy tak se okamžitě nastaví šedá a jde se na další pixel
                    if (i + smerX >= bit1.Width || i+smerX <= 0 || j+smerY >= bit1.Height || j+smerY<=0)
                    {
                        bit2.SetPixel(i, j, Color.FromArgb(127,127,127)); continue;
                    }
                    pixel1 = bit1.GetPixel(i, j);
                    pixel2 = bit1.GetPixel(i + smerX, j + smerY);
                    svetlost = (150 + bmp.svetelnost(pixel2) - bmp.svetelnost(pixel1));
                    if (svetlost < 0)
                    {
                        svetlost = 0;
                    }else if (svetlost > 255)
                    {
                        svetlost = 255;
                    }
                    bit2.SetPixel(i, j, Color.FromArgb(svetlost, svetlost, svetlost));
                }
                if (i%50 == 0)
                {
                    bmp.bitmap = bit2;
                    bmp.drawBitmap(new Point(0, 0), this);
                }
            }
            bmp.bitmap = bit2;
            bmp.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
            return bmp;
        }
        #endregion
        #endregion
        private void zpet_Click(object sender, EventArgs e)
        {
            if (oldBtms.Count == 0 || positionInOld >= oldBtms.Count) { return; }
            Text = "Fotošop - navrácení úpravy";
            btm = oldBtms[positionInOld];
            positionInOld++;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            btm.drawBitmap(new Point(0, 0), this);
        }

        Form form2 = new Form();
        BitovaMapa Import = new BitovaMapa();
        BitovaMapa Import2 = new BitovaMapa();
        private void soubor_zobrazit_2overlay_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání obrázku"); return; } //pokud exit file dialogu není OK tak se vypne celá funkce
            Import = new BitovaMapa(ofd.FileName);//používám jenom jeden file dialog takže nejdřív nastavím výsledek na první a pak zjístím druhou
            if (ofd.ShowDialog() != DialogResult.OK) { MessageBox.Show("Nastala chyba v načítání obrázku"); return; }
            Import2 = new BitovaMapa(ofd.FileName);

            //zakládání vedlejšího okna
            form2 = new Form();
            form2.Show();//zobrazení formu
            form2.Size = Import.size;//velikost formu se nastaví na velikost prvního obrázku
            Import = SetImageOpacity(Import, 0.5F); //nastaví bitmapě vlastnost ImageAttributes na 50% transparence
            Import2 = SetImageOpacity(Import2, 0.5F); //ty image attributes toho umí víc ale já tomu nerozumim
            Import.drawBitmap(new Point(0, 0), form2, true);
            Import2.drawBitmap(new Point(0, 0), form2, true);
            //při změnění velikosti formu ze znovu vykreslí bitmapy na větší prostor
            form2.SizeChanged += new EventHandler(Form2_SizeChanged);
        }
        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            Import.drawBitmap(new Point(0, 0), form2);
            Import2.drawBitmap(new Point(0, 0), form2, true);
        }

        //ztopený někde ze stack overflowu (proto komenty v aj)
        private BitovaMapa SetImageOpacity(BitovaMapa image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.bitmap);

                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity  
                matrix.Matrix33 = opacity;

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                image.imageAttributes = attributes;
                
                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            oldBtms.Insert(positionInOld, btm.copy());
            Text = "Fotošop - načítání filtru";
            Bitmap bit1 = new Bitmap(btm.copy().bitmap);
            Bitmap bit2 = new Bitmap(btm.copy().bitmap);//výsledek se bude zobrazovat tady
            Color pixel1;
            Color pixel2;
            Color pixel3;
            Color pixel4;
            Color pixel5;
            int svetlost;
            for (int i = 0; i < bit1.Width; i++)
            {
                for (int j = 0; j < bit1.Height; j++)
                {
                    //pokud jsme na okraji bitmapy tak se okamžitě nastaví bílá a jde se na další pixel
                    if (i + 2 >= bit1.Width || i - 2 <= 0 || j + 2 >= bit1.Height || j - 2 <= 0)
                    {
                        bit2.SetPixel(i, j, Color.FromArgb(0, 0, 0)); continue;
                    }
                    pixel1 = bit1.GetPixel(i, j);
                    pixel2 = bit1.GetPixel(i + 2, j + 2);
                    pixel3 = bit1.GetPixel(i - 2, j - 2);
                    pixel4 = bit1.GetPixel(i - 2, j + 2);
                    pixel5 = bit1.GetPixel(i + 2, j - 2);
                    int pixel1svetlo = btm.svetelnost(pixel1);
                    if (((btm.svetelnost(pixel2) - pixel1svetlo) < -50 || ((btm.svetelnost(pixel3) - pixel1svetlo) < -50) || ((btm.svetelnost(pixel4) - pixel1svetlo) < -50) || ((btm.svetelnost(pixel5) - pixel1svetlo) < -50)))
                    {
                        svetlost = 0;
                    }
                    else
                    {
                        svetlost = 255;
                    }
                    bit2.SetPixel(i, j, Color.FromArgb(svetlost, svetlost, svetlost));
                }
                if (i % 50 == 0)
                {
                    btm.bitmap = bit2;
                    btm.drawBitmap(new Point(0, 0), this);
                }
            }
            btm.bitmap = bit2;
            btm.drawBitmap(new Point(0, 0), this);
            Text = "Fotošop";
        }
    }
}
