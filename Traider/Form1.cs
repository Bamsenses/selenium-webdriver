using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;



namespace Traider
{

    public partial class Form1 : Form
    {
        IWebDriver Browser;
        IWebElement Pips;
        double delta, cat, catmax1, catmin1, catmax2, catmin2, cat00, cat49;
        string realtime;
        int prof, posmX, posmY;
        bool trade, trade1, trade2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trade = false;
            prof = 0;
        }


//--------------------Запуск браузера с сайтом---------------------------------------------
        private void buttonStartBrowser_Click(object sender, EventArgs e)
        {
            Browser = new OpenQA.Selenium.Chrome.ChromeDriver();
            Browser.Manage().Window.Maximize();
            Browser.Navigate().GoToUrl("https://olymptrade.com");
        }
//-----------------------------------------------------------------------------------------


//--------------------Считываем положения мышки--------------------------------------------
        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.K && e.Control)
            {
                label20.Text = Convert.ToString(MousePosition);
                posmX = MousePosition.X;
                posmY = MousePosition.Y;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            panel1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }
//-----------------------------------------------------------------------------------------


//--------------------Нажатие на кнопку вверх----------------------------------------------
        private void ClickUp()
        {
            IWebElement UP = Browser.FindElement(By.CssSelector("div.-up"));
            UP.Click();
            System.Threading.Thread.Sleep(100);
        }
//-----------------------------------------------------------------------------------------

//--------------------Нажатие на кнопку вниз-----------------------------------------------
        private void ClickDown()
        {
            IWebElement DOWN = Browser.FindElement(By.CssSelector("div.-down"));
            DOWN.Click();
            System.Threading.Thread.Sleep(100);
        }
//-----------------------------------------------------------------------------------------


//--------------------Вписываем время сделки-----------------------------------------------
        private void TimeStavka(int time)
        {
            int s;
            string str;
            str = Convert.ToString(time);
            char[] summ = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                summ[i] = str[i];
            }

            Random rnd = new Random();
            IWebElement stavka = Browser.FindElement(By.CssSelector("input.timeinput__input_minutes"));

            stavka.Click();
            var act = new Actions(Browser);
            act.DoubleClick(stavka).Perform();
            for (int i = 0; i < str.Length; i++)
            {
                s = rnd.Next(250, 300);
                System.Threading.Thread.Sleep(s);
                string ss = Convert.ToString(summ[i]);
                stavka.SendKeys(ss);
            }
        }

        //-----------------------------------------------------------------------------------------


        //--------------------Вписываем в поле сумму сделки----------------------------------------

        private void Summa(int SummStavka)
        {
            int s;
            string str;
            str = Convert.ToString(SummStavka);
            char[] summ = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                summ[i] = str[i];
            }

            Random rnd = new Random();
            IWebElement stavka = Browser.FindElement(By.CssSelector("input[data-test=deal-amount-input]"));

            stavka.Click();
            var act = new Actions(Browser);
            act.Click(stavka).Perform();
            for (int i = 0; i < 6; i++)
            {
                act.SendKeys(OpenQA.Selenium.Keys.Backspace).Perform();
            }

            for (int i = 0; i < str.Length; i++)
            {
                s = rnd.Next(250, 300);
                System.Threading.Thread.Sleep(s);
                string ss = Convert.ToString(summ[i]);
                stavka.SendKeys(ss);
            }
        }

//-----------------------------------------------------------------------------------------





//--------------------Чтение времени и катеровки-------------------------------------------
        private void Time()
        {
            string txt;
            try
            {
                IWebElement time11 = Browser.FindElement(By.CssSelector("span.chart-status--time"));
                txt = time11.Text;
                string[] str = txt.Split(' ');
                label3.Text = str[0];
                label4.Text = str[1];
                realtime = str[1];
                IWebElement Pips = Browser.FindElement(By.CssSelector("text.pin_text"));
                Catirovka.Text = Pips.Text;
                cat = Convert.ToDouble(Pips.Text.Replace(".", ","));
            }
            catch { }

        }
//-----------------------------------------------------------------------------------------



//--------------------Таймер вписывающий сумму сделки каждые 10 мин.-----------------------
        private void timer1_Tick(object sender, EventArgs e)
        {
            int s = Convert.ToInt32(maskedTextBox2.Text);
            Summa(s);
        }
//-----------------------------------------------------------------------------------------


        private void timerTry_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(10000);
           List<IWebElement> result = Browser.FindElements(By.CssSelector("td.user-deals-table__cell_status span.user-deals-table__cell-content span")).ToList();

            if (result[0].Text == "Прогноз не оправдался")
            {
                
                //if (prof == 2) trade = false;
               // else trade = true;
            }
            else
            {
                prof++;
                trade = true;
            }
            int s = Convert.ToInt32(maskedTextBox2.Text);
            label17.Text = Convert.ToString(prof);
            Summa(s);
            timer3.Start();
            timerTry.Stop();
        }


        private double CandelCat(string s, int p = 0)
        {
            double max, min;
            max = 0;
            min = 0;
            int idle = Convert.ToInt16(maskedTextBox1.Text);
            List<IWebElement> candlcat = Browser.FindElements(By.CssSelector("span.crosshair-vertical--value")).ToList();
            System.Threading.Thread.Sleep(200);
            Cursor.Position = new Point(Convert.ToInt32(posmX-p), Convert.ToInt32(posmY));
            System.Threading.Thread.Sleep(idle);
            Cursor.Position = new Point(Convert.ToInt32(posmX-p+2), Convert.ToInt32(posmY+10));

            max = Convert.ToDouble(candlcat[1].GetAttribute("innerHTML").Replace(".", ","));
            min = Convert.ToDouble(candlcat[2].GetAttribute("innerHTML").Replace(".", ","));


            if (s == "max") return max;
            else if (s == "min") return min;
            else return 0.0;
        }



//----------Таймер записывающий катировки в определенное время------------------------------
        private void timer3_Tick(object sender, EventArgs e)
        {

            Time();
            string[] str = realtime.Split(':');

             if (str[2] == "00")
            {
                catmax1 = CandelCat("max");
                label50.Text = Convert.ToString(catmax1);
                catmin1 = CandelCat("min");
                label54.Text = Convert.ToString(catmin1);

                catmax2 = CandelCat("max", 30);
                label55.Text = Convert.ToString(catmax2);
                catmin2 = CandelCat("min", 30);
                label56.Text = Convert.ToString(catmin2);

                if (trade)
                {
                    OpenStavka();
                }

            }

            label8.Text = Convert.ToString(trade);
        }
//-----------------------------------------------------------------------------------------


//----------Таймеры открывающие сделку в 50минут--------------------------------------------

        private void OpenStavka()
        {

            if (catmax1 == catmax2 && catmin1 == catmin2)
            {

            }
            else if (catmax1 == catmax2)
            {
                ClickDown();
                timer3.Stop();
                timerTry.Start();
            }
            else if (catmin1 == catmin2)
            {
                ClickUp();
                timer3.Stop();
                timerTry.Start();
            }
         }
//-----------------------------------------------------------------------------------------



//----------Старт торговли-------------------------------------------------------------
        private void buttonStart_Click(object sender, EventArgs e)
       {
            Time();
            timer3.Start();
            timer1.Start();
            trade = true;
            int s = Convert.ToInt32(maskedTextBox2.Text);
            Summa(s);

            List<IWebElement> timeandpips = Browser.FindElements(By.CssSelector("text.pin_text")).ToList();
            Pips = timeandpips[0];
            buttonStop.Enabled = true;
        }
//-----------------------------------------------------------------------------------------


//----------Остановка торговли-------------------------------------------------------------
        private void buttonStop_Click(object sender, EventArgs e)
       {
           timer3.Stop();
       }
//-----------------------------------------------------------------------------------------
    }
}
