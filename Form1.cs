using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Practika
{
    public partial class Form1 : Form
    {
        Ploter ploter;
        char[] num_with_x = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'x', 'e' };
        int err = 0;
        bool flag1 = false, flag2 = false, flag3 = true;
        private bool isDown;
        private Point lastPoint;
        private Point currPoint;

        public Form1()
        {
            InitializeComponent();
            ploter = new Ploter(30, 30, new Point(Plotter.Width/2, Plotter.Height/2));
            Plotter.MouseWheel += Plotter_MouseWheel;
        }

        private void Plotter_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (ploter.XPixelsPerUnit < 110) ploter.XPixelsPerUnit *= 2;
                if (ploter.YPixelsPerUnit < 110) ploter.YPixelsPerUnit *= 2;
            }
            else
            {
                if (ploter.XPixelsPerUnit > 8) ploter.XPixelsPerUnit /= 2;
                if (ploter.YPixelsPerUnit > 8) ploter.YPixelsPerUnit /= 2;
            }
            Plotter.Invalidate();
        }
        private void Plotter_MouseDown(object sender, MouseEventArgs e){ isDown = true; lastPoint = e.Location; }
        private void Plotter_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                currPoint = e.Location;
                int deltaX = currPoint.X - lastPoint.X;
                int deltaY = currPoint.Y - lastPoint.Y;
                ploter.Center = new Point(ploter.Center.X + deltaX, ploter.Center.Y + deltaY);
                lastPoint = currPoint;
                Plotter.Invalidate();
            }
        }
        private void Plotter_MouseUp(object sender, MouseEventArgs e) { isDown = false; }

        private void Plotter_Paint(object sender, PaintEventArgs e)
        {
            ploter.DrawGridandAxes(e.Graphics, Plotter.Width, Plotter.Height);
            ploter.DrawLines(e.Graphics);
        }
        private void list_funcs_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ploter.Lines[e.Item.Index].Show = e.Item.Checked;
            Plotter.Invalidate();
        }
        private void list_funcs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var lv = sender as ListView;
                var item = lv.HitTest(e.Location).Item;
                if (item == null) MessageBox.Show("Vous n'avez sélectionné(e) aucune icône.");
                else
                {
                    Table_znach.Rows.Clear();
                    Table_znach.Columns.Clear();
                    for (int i = 0; i < ploter.Lines[lv.HitTest(e.Location).Item.Index].Points.Count; i++) 
                        Table_znach.Columns.Add("", ploter.Lines[lv.HitTest(e.Location).Item.Index].Points[i].X.ToString());
                    for(int i = 0; i < ploter.Lines[lv.HitTest(e.Location).Item.Index].Points.Count; i++)
                        Table_znach.Rows[0].Cells[i].Value = ploter.Lines[lv.HitTest(e.Location).Item.Index].Points[i].Y.ToString();
                }
            }
        }
        static bool BracketCheck(string s)
        {
            Stack<char> st = new Stack<char>();
            foreach (var x in s)
            {
                if (x == '(') st.Push(x);

                if (x == ')')
                {
                    if (st.Count == 0) return false;
                    else st.Pop();
                }
            }
            if (st.Count != 0) return false;
            return true;
        }

        static bool CheckZnach(char ch)
        {
            char[] znach = { '+', '-', '(', '*', '/' , ',', '^'};
            foreach (char str in znach) if (str == ch) return true;
            return false;
        }

        static bool CheckNum(char ch)
        {
            char[] num = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            foreach (char str in num) if (str == ch) return true;
            return false;
        }

        void Clear()
        {
            Table_znach.Rows.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 1;
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 2;
        }

        private void button_opening_bracket_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "" && flag1 == false && entry_field.Text[entry_field.Text.Length - 1] != 'g')  entry_field.Text = entry_field.Text + "*";
            entry_field.Text = entry_field.Text + '(';
            if (flag1 == true && entry_field.Text[entry_field.Text.Length - 2] != 'g') flag1 = false;
        }

        private void button_closing_bracket_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (BracketCheck(entry_field.Text))
                {
                    string message = "Ошибка ввода: У вас нет открывающей скобки";
                    MessageBox.Show(message);
                    flag2 = true;
                }
                else
                {
                    foreach (char ch in num_with_x)
                    {
                        if (ch == entry_field.Text[entry_field.Text.Length - 1])
                        {
                            flag2 = true;
                            entry_field.Text = entry_field.Text + ')';
                            break;
                        }
                        else flag2 = false;
                    }
                    if (entry_field.Text[entry_field.Text.Length - 1].ToString() == "𝝅")
                    {
                        flag2 = true;
                        entry_field.Text = entry_field.Text + ')';
                    }
                    
                }
            }
            else flag2 = false;
            if (flag2 == false)
            {
                if (entry_field.Text[entry_field.Text.Length - 1] == ')') entry_field.Text = entry_field.Text + ')';
                else
                {
                    string message = "Ошибка ввода: ')' может стоять только после переменной или цифры";
                    MessageBox.Show(message);
                }
            }
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 3;
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            list_funcs.Items.Clear();
            ploter.Lines.Clear();
            entry_field.Text = "";
            textBox_right_interval.Text = "";
            textBo_left_interval.Text = "";
            Plotter.Invalidate();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            entry_field.Text = entry_field.Text + "+";
        }

        private void button_division_Click(object sender, EventArgs e)
        {
            entry_field.Text = entry_field.Text + "/";
        }

        private void button_multiplication_Click(object sender, EventArgs e)
        {
            entry_field.Text = entry_field.Text + "*";
        }

        private void button_sub_Click(object sender, EventArgs e)
        {
            entry_field.Text = entry_field.Text + "-";
        }

        private void button_sinus_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "sin(";
        }

        private void button_cosinus_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "cos(";
        }

        private void button_tangens_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "tg(";
        }

        private void button_cotanges_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "ctg(";
        }

        private void button_dot_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (CheckNum(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + ",";
            }
        }

        private void button_degree_Click(object sender, EventArgs e)
        {
            entry_field.Text = entry_field.Text + "^";
        }

        private void button_root_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "sqrt(";
        }

        private void button_log_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "log(";
        }

        private void button_factorial_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "ln(";
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (entry_field.Text[entry_field.Text.Length - 1] == '(')
                {
                    for (int i = 0; i < num_with_x.Length; i++) {
                        if (entry_field.Text[entry_field.Text.Length - 2] == num_with_x[i]) flag1 = true;
                    }

                }
                entry_field.Text = entry_field.Text.Remove((entry_field.Text.Length - 1), 1);
            }
        }

        private void button_4_Click_1(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 4;
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 5;
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 6;
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 7;
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 8;
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 9;
        }

        private void button_0_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckNum(entry_field.Text[entry_field.Text.Length - 1]) && !CheckZnach(entry_field.Text[entry_field.Text.Length - 1]) &&
                    (entry_field.Text[entry_field.Text.Length - 1] != 'g' && entry_field.Text[entry_field.Text.Length - 2] != 'l')) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + 0;
        }

        private void button_x_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "x";
        }

        private void sinh_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "sinh(";
        }

        private void cosh_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "cosh(";
        }

        private void tgh_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "tgh(";
        }

        private void ctgh_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "ctgh(";

        }

        private void arcsin_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "asin(";
        }

        private void arccos_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "acos(";
        }

        private void arctangens_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "atg(";
        }

        private void arcctg_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "actg(";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "lg";
            flag1 = true;

        }
        private void button_abs_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "abs(";
        }

        private void button_e_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "e";
        }

        private void button_pi_Click(object sender, EventArgs e)
        {
            if (entry_field.Text != "")
            {
                if (!CheckZnach(entry_field.Text[entry_field.Text.Length - 1])) entry_field.Text = entry_field.Text + "*";
            }
            entry_field.Text = entry_field.Text + "𝝅";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            double[] param = new double[3];
            for (int i = 0; i < 3; i++) param[i] = 0;
            string message = "";
            if (entry_field.Text == "") err = 1;
            else if (textBox_right_interval.Text == "" && textBo_left_interval.Text == "") err = 2;
            else if (double.Parse(textBo_left_interval.Text) > double.Parse(textBox_right_interval.Text)) err = 3;
            else if (!BracketCheck(entry_field.Text)) err = 4;
            else if (!Expression.IsExpression(entry_field.Text)) err = 5;
            else err = 0;
            for (int i = 0; i < list_funcs.Items.Count; i++)
            {
                if (list_funcs.Items[i].SubItems[0].Text == entry_field.Text) err = 6;
            }
            switch (err)
            {
                case 1: message = "Введите уравнение"; break;
                case 2: message = "Введите интервал"; break;
                case 3: message = "Левый интервал не может быть больше правого"; break;
                case 4: message = "Есть незакрытая скобка"; break;
                case 5: message = "Ошибка в формуле"; break;
                case 6: message = "Данная формула уже введена"; break;
                case 0:
                    Expression expression = new Expression(entry_field.Text, null);
                    Random random = new Random();
                    Color ranCol = Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                    Line line = new Line(ranCol);
                    for (decimal j = 1, i = decimal.Parse(textBo_left_interval.Text); i <= decimal.Parse(textBox_right_interval.Text); i += 0.05m,j++)
                    {
                        param[0] = (double)i;
                        double[] result = expression.CalculateValue(param);
                        if (result[2] == 1) { message = "Нельзя делить на ноль"; Clear(); flag3 = false; break; }
                        else if (result[2] == 2) { message = "Аргумент корня должен быть неотрицательным"; Clear(); flag3 = false; break; }
                        else if (result[2] == 3) { message = "Аргумент тангенса не должен быть равен 𝝅/2+𝝅n"; Clear(); flag3 = false; break; }
                        else if (result[2] == 4) { message = "Аргумент котангенса не должен быть равен 𝝅n"; Clear(); flag3 = false; break; }
                        else if (result[2] == 5) { message = "Аргумент арксинуса и аркосинуса должен находиться в интервале [-1; 1]"; Clear(); flag3 = false; break; }
                        else if (result[2] == 6) { message = "Аргумент логарифма должен быть положительным"; Clear(); flag3 = false; break; }
                        else if (result[2] == 7) { message = "Основание логарифма должно быть положительным и не равным 1"; Clear(); flag3 = false; break; }
                        else
                        {
                            flag3 = true;
                            PointF point = new PointF((float)i, (float)result[1]);
                            line.Points.Add(point);
                            message = "Функция добавлена успешно";
                        }
                    }
                    if (flag3)
                    {
                        ListViewItem item = new ListViewItem(entry_field.Text)
                        {
                            Checked = true
                        };
                        item.SubItems.Add("[" + textBo_left_interval.Text + ";" + textBox_right_interval.Text + "]");
                        item.SubItems.Add("");
                        item.SubItems[2].BackColor = ranCol;
                        item.UseItemStyleForSubItems = false;
                        ploter.Lines.Add(line);
                        list_funcs.Items.Add(item);
                        Plotter.Invalidate();
                        entry_field.Text = "";
                        textBox_right_interval.Text = "";
                        textBo_left_interval.Text = "";
                    }
                    break;
            }
            if (flag3 == false) MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else MessageBox.Show(message);
        }
    }
}
