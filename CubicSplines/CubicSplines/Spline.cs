using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubicSplines
{
      /*     Класс для  построения  кубических  сплайнов и
       *  выполнения   интерполяции,               */
    class Spline
    {
        /*  Количество  точек  разбиения,  */
        int n = 0;
        /*  Количество  отрезков  разбиения,  */
        int n1 = 0;
        /*  Значения  аргумента, то есть  точки разбиения,  */
        double[] x = new double[0];
        /*  Значения  функции  в  точках  разбиния,     */
        double[] y = new double[0];
        /*  Длины  отрезков  разбиения,     */
        double[] h = new double[0];
        /*  Коэффициенты a кубических сплайнов,   */
        double[] a = new double[0];
        /*  Коэффициенты b кубических сплайнов,   */
        double[] b = new double[0];
        /*  Коэффициенты c кубических сплайнов,   */
        double[] c = new double[0];
        /*  Коэффициенты d кубических сплайнов,   */
        double[] d = new double[0];

          /*   Кнструктор класса Spline,   */
        public Spline(Dictionary<double, double> x1)
        {
            double a1=0;
            n = x1.Keys.Count;
            n1 = (n > 0) ? n - 1 : 0;
            x = new double[n];
            y = new double[n];
            h = new double[n1];
            a = new double[n1];
            b = new double[n1];
            c = new double[n1];
            d = new double[n1];
            for (int i = 0; i < n; i++)
            {
                a1=x1.Keys.ElementAt(i);
                x[i] = a1;
                y[i] = x1[a1];
            }
            for (int i = 0; i < n1; i++)
            {
                h[i] = x[i + 1] - x[i];
            }
            CreateCoefficients();
        }

        /* Строка для вывода i-го сплайна,   */
        public string SplineToString(int i)
        {
            string s = "";
            if (i>=0 &&  i < n1)
            {
                s = string.Format(
       "{1:0.###}+{2:0.###}(x-{0})+{3:0.###}(x-{0})^2+{4:0.###}(x-{0})^3,",
                    x[i], a[i], b[i], c[i], d[i]);
            }
            return s;
        }

        /* Значение i-го сплайна в точке,   */
        public double SplineValue (double a1, int i)
        {
            double f = 0;
            if (i >= 0 && i < n1)
            {
                f=a[i]+b[i]*(a1-x[i])+
                c[i]*Math.Pow(a1-x[i],2)+
                d[i]*Math.Pow(a1-x[i],3);
            }
            return f;
        }



        /*    Вывод информации 
         *  о коэффициентах сплайнов и самих сплайнов 
         *  в элемент управления DataGridView,           */  
        public void OutToDataGridView(DataGridView a1)
        {
            a1.ColumnCount = 7;
            a1.RowCount = n1;
            a1.Columns[0].HeaderText = "a";
            a1.Columns[1].HeaderText = "b";
            a1.Columns[2].HeaderText = "c";
            a1.Columns[3].HeaderText = "d";
            a1.Columns[4].HeaderText = "Сплайн fi";
            a1.Columns[5].HeaderText = "fi(xi)";
            a1.Columns[6].HeaderText = "fi(xi+1)";
            a1.Columns[4].Width = 500;
            for (int i = 0; i < n1; i++)
            {
                a1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                a1[0, i].Value = a[i];
                a1[1, i].Value = b[i];
                a1[2, i].Value = c[i];
                a1[3, i].Value = d[i];
                a1[4, i].Value = SplineToString(i);
                a1[5, i].Value = SplineValue(x[i], i);
                a1[6, i].Value = SplineValue(x[i + 1], i);
            }
        }

        /*  Создание коэффициентов кубических сплайнов,  */
        private void CreateCoefficients()
        {
            for (int i = 0; i < n1; i++)
            {
                a[i] = y[i];
            }
            int m = n1 - 1;
            double[] a1 = new double[m];
            double[] b1 = new double[m];
            double[] c1 = new double[m];
            double[] f = new double[m];
            double[] q = new double[m];
            for (int i = 0; i < m; i++)
            {
                a1[i] = h[i];
                b1[i] = 2 * (h[i] + h[i + 1]);
                c1[i] = h[i + 1];
                f[i] = 3 * ((y[i + 2] - y[i + 1]) / h[i + 1] - (y[i + 1] - y[i]) / h[i]);
            }
            a1[0] = 0;
            q = Solution(a1, b1, c1, f, m);
            c[0] = 0;
            for (int i = 0; i < m; i++)
            {
                c[i + 1] = q[i];
            }
            for (int i = 0; i < n1 - 1; i++)
            {
                b[i] = (y[i + 1] - y[i]) / h[i] -
                    h[i] * (c[i + 1] + 2 * c[i]) / 3;
                d[i] = (c[i + 1] - c[i]) / (3 * h[i]);
            }
               b[n-2] = (y[n-1] - y[n-2]) / h[n-2] -
                    2*h[n-2] * c[n-2] / 3;
               d[n - 2] = -c[n - 2] / (3 * h[n - 2]);
        }


        /*     Решение системы уравнений с трехдиагональной матрицей
         * методом прогонки,
         *    b - главная диагональ,
         *    a - массив элементов под главной диагональю,
         *    c - массив элементов над главной диагональю,
         *    f - столбец свободных членов,
         *    n - количество уравнений, 
         *      порядок матрицы системы уравнений,       */
        public static double[] Solution(
            double[] a, double[] b, double[] c,
            double[] f, int n)
        {
            double[] x = new double[n];
              /*  Прямой ход метода прогонки,   */
            c[0] = c[0] / b[0];
            f[0] = f[0] / b[0];
            b[0] = 1;
            for (int i = 1; i < n; i++)
            {
                b[i] = b[i] - a[i] * c[i - 1];
                f[i] = f[i] - a[i] * f[i - 1];
                a[i] = 0;
            }
              /*  Обратный ход метода прогонки,    */
            x[n - 1] = f[n - 1]/b[n-1];
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = f[i] - c[i] * x[i + 1];
            }
            return x;
        }

       
    }
}
