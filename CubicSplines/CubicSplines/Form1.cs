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

namespace CubicSplines
{
    public partial class Form1 : Form
    {
        /*  Значения аргумента и функции,  */
        Dictionary<double, double> x = new Dictionary<double, double>();

        /*  Конструктор формы,  */
        public Form1()
        {
            InitializeComponent();
            
        }

        /*     Действия при выборе пункта меню для чтения 
         *  значений аргумента и функции из файла,        */
        private void прочитатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Application.StartupPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                x = WorkWithInformation.ReadFile(
                          openFileDialog1.FileName);
                WorkWithInformation.WriteToDataGridView(
                           x, dataGridView1);
                Spline a = new Spline(x);
                a.OutToDataGridView(dataGridView2);
            }
        }

        /*     Действия при выборе пункта меню для 
         *  создания значений аргумента и функции
         *  как случайных чисел,                  */
        private void случайныеЗначенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x = WorkWithInformation.CreateDictionary(1, 10, 50);
            WorkWithInformation.WriteToDataGridView(
                        x, dataGridView1);
            Spline a = new Spline(x);
            a.OutToDataGridView(dataGridView2);
        }

        /*     Действия при выборе пункта меню для сохранения 
         *  значений аргумента и функции в файл,        */
        private void сохранитьЗначенияДляЗадачиВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK
                 && dataGridView1.RowCount>0)
            {
                WorkWithInformation.WriteToFile(
                     dataGridView1,saveFileDialog1.FileName);
            }
        }

        
    }
}
