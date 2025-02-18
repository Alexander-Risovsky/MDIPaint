using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MDIPaint
{
    public partial class MainForm : Form
    {
        public static Color CurColor { get; set; }
        public static float CurSize { get; set; }

        public static bool isZalivka { get; set; }

        public static int PolygonSides = 3;

        public static Tool CurTool { get; set; }



        public MainForm()
        {
            InitializeComponent();
            CurColor = Color.Black;
            CurSize = 1;
            CurTool = Tool.Pen;
            isZalivka = false;

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = new DocumentForm();
            doc.MdiParent = this;
            сохранитьToolStripMenuItem.Enabled = true;
            сохранитьКакToolStripMenuItem.Enabled = true;

            doc.Show();

        }
        public void UpdateMenu()
        {
            bool isActive = Application.OpenForms.Cast<Form>().Any(f => f is DocumentForm);
            сохранитьToolStripMenuItem.Enabled = isActive;
            сохранитьКакToolStripMenuItem.Enabled = isActive;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new AboutForm();
            frm.ShowDialog();
        }

        private void красныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                MessageBox.Show("Сначала поменяйте инструмент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                CurColor = Color.Red;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                MessageBox.Show("Сначала поменяйте инструмент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                CurColor = Color.Blue;

        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                MessageBox.Show("Сначала поменяйте инструмент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                CurColor = Color.Green;
        }

        private void другиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                MessageBox.Show("Сначала поменяйте инструмент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    CurColor = dlg.Color;
                }
            }


        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            int newSize = 1;
            if (string.IsNullOrWhiteSpace(toolStripTextBox1.Text))
            {
                CurSize = newSize;
                return;
            }
            if (int.TryParse(toolStripTextBox1.Text, out newSize) && int.Parse(toolStripTextBox1.Text) > 0)
            {
                // Успешное преобразование, обновляем размер
                CurSize = newSize;
            }
            else
            {
                // Обработка некорректного ввода (опционально)
                MessageBox.Show("Введите корректное числовое значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                toolStripTextBox1.Text = CurSize.ToString(); // Сбрасываем текст в текущее значение
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var activeDocumentForm = ActiveMdiChild as DocumentForm;
            if (activeDocumentForm.isSaved)
            {
                Bitmap bmp = activeDocumentForm.GetBitmap();
                bmp.Save(activeDocumentForm.SavedPath);
            }
            else
            {
                ToolSave(activeDocumentForm);
            }
        }
        public void ToolSave(DocumentForm doc)
        {
            Bitmap bmp = doc.GetBitmap();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpg)|*.jpg";
            ImageFormat[] ff = { ImageFormat.Bmp, ImageFormat.Jpeg };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                bmp.Save(dlg.FileName, ff[dlg.FilterIndex - 1]);
                doc.SavedPath = dlg.FileName;
                doc.isSaved = true;
            }
        }
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var activeDocumentForm = ActiveMdiChild as DocumentForm;
            ToolSave(activeDocumentForm);
        }



        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            сохранитьКакToolStripMenuItem.Enabled = true;
            сохранитьToolStripMenuItem.Enabled = true;
        }

        private void каскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                LayoutMdi(MdiLayout.Cascade);
            else
                MessageBox.Show("Нет достпуных форм", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void сверхуToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ActiveMdiChild != null)
                LayoutMdi(MdiLayout.TileVertical);
            else
                MessageBox.Show("Нет достпуных форм", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void слеваНаправоToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ActiveMdiChild != null)
                LayoutMdi(MdiLayout.TileHorizontal);
            else
                MessageBox.Show("Нет достпуных форм", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void упорядочитьЗначкиToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (ActiveMdiChild != null)
                LayoutMdi(MdiLayout.ArrangeIcons);
            else
                MessageBox.Show("Нет достпуных форм", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg|Все файлы ()*.*|*.*";
            dlg.Title = "Открыть изображение";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var doc = new DocumentForm(dlg.FileName);
                doc.Show();
                doc.MdiParent = this;
            }

        }

        private void линияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Line;
            SetTool(CurTool);
            ActiveZalivka();
        }

        private void окружностьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Circle;
            SetTool(CurTool);
            ActiveZalivka();

        }

        private void прямоугольникToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Rectangle;
            SetTool(CurTool);
            ActiveZalivka();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Pen;
            SetTool(CurTool);
            ActiveZalivka();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CurTool = Tool.Lastik;
            CurColor = Color.White;
            SetTool(CurTool);
            ActiveZalivka();
        }
        public void ActiveZalivka()
        {
            bool isTool = CurTool == Tool.Circle | CurTool == Tool.Rectangle | CurTool == Tool.Polygon;
            toolStripDropDownButton3.Enabled = isTool;
        }

        private void SetTool(Tool tool)
        {
            MainForm.CurTool = tool;

            switch (tool)
            {
                case Tool.Pen:
                    this.Cursor = new Cursor("pen.cur");
                    break;
                case Tool.Lastik:
                    this.Cursor = new Cursor("eraser.cur");
                    break;
                case Tool.Line:
                case Tool.Circle:
                case Tool.Rectangle:
                case Tool.Polygon:
                    this.Cursor = Cursors.Cross;
                    break;
                case Tool.Bucket:
                    this.Cursor = new Cursor("Bucket.cur");
                    break;
                case Tool.Text:
                    this.Cursor = Cursors.IBeam;
                    break;
                default:
                    this.Cursor = Cursors.Default;
                    break;
            }
        }



        private void безЗаливкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isZalivka = false;
        }

        private void сплошнаяЗаливкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool != Tool.Lastik)
                isZalivka = true;
            else
                MessageBox.Show("Сначала поменяйте инструмент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CurTool = Tool.Bucket;
            SetTool(CurTool);
        }

        private void многоугольникToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Polygon;
            SetTool(CurTool);
            ActiveZalivka();
        }


        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolStripTextBox2.Text))
            {
                PolygonSides = 3;
                return;
            }
            if (int.TryParse(toolStripTextBox2.Text, out PolygonSides) && int.Parse(toolStripTextBox2.Text) > 0)
            {
                return;
            }
            else
            {
                // Обработка некорректного ввода (опционально)
                MessageBox.Show("Введите корректное числовое значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                toolStripTextBox2.Text = string.Empty; // Сбрасываем текст в текущее значение
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (CurTool == Tool.Lastik)
                CurColor = Color.Black;
            CurTool = Tool.Text;
            SetTool(CurTool);
            ActiveZalivka();
        }
    }
}
