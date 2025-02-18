using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class DocumentForm : Form
    {
        int oldX;
        int oldY;
        private Bitmap bitmap;
        private Bitmap bmpTemp;

        public event EventHandler DocumentFormClosed;
        public bool isSaved = false;
        public string SavedPath { get; set; }
        public static int countForms = 0;
        public static bool isChanged = false;

        private float zoomFactor = 1.0f;
        private Point zoomCenter;

        public DocumentForm()
        {
            InitializeComponent();
            countForms++;
            bitmap = CreateBimap();
            bmpTemp = bitmap;

            // Добавляем обработчик события MouseWheel
            this.MouseWheel += new MouseEventHandler(DocumentForm_MouseWheel);
        }

        public DocumentForm(string FileName)
        {
            InitializeComponent();
            countForms++;
            bitmap = CreateBimap(FileName);
            bmpTemp = bitmap;

            // Добавляем обработчик события MouseWheel
            this.MouseWheel += new MouseEventHandler(DocumentForm_MouseWheel);
        }

        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        public Bitmap CreateBimap()
        {
            Bitmap bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }
            return bmp;
        }

        public Bitmap CreateBimap(string FileName)
        {
            Bitmap bmp = new Bitmap(Image.FromFile(FileName));
            return bmp;
        }

        private void DocumentForm_Load(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void DocumentForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Пересчитываем координаты мыши с учетом масштаба
                oldX = (int)((e.X - zoomCenter.X) / zoomFactor + zoomCenter.X);
                oldY = (int)((e.Y - zoomCenter.Y) / zoomFactor + zoomCenter.Y);
            }
        }

        private void DocumentForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isChanged = true;
                var pen = new Pen(MainForm.CurColor, MainForm.CurSize);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Пересчитываем координаты мыши с учетом масштаба
                int adjustedX = (int)((e.X - zoomCenter.X) / zoomFactor + zoomCenter.X);
                int adjustedY = (int)((e.Y - zoomCenter.Y) / zoomFactor + zoomCenter.Y);

                switch (MainForm.CurTool)
                {
                    case Tool.Pen:
                        {
                            var g = Graphics.FromImage(bitmap);
                            g.DrawLine(pen, oldX, oldY, adjustedX, adjustedY);
                            oldX = adjustedX;
                            oldY = adjustedY;
                            bmpTemp = bitmap;
                            Invalidate();
                            break;
                        }
                    case Tool.Line:
                        {
                            bmpTemp = (Bitmap)bitmap.Clone();
                            var g = Graphics.FromImage(bmpTemp);
                            g.DrawLine(pen, oldX, oldY, adjustedX, adjustedY);
                            Invalidate();
                            break;
                        }
                    case Tool.Rectangle:
                        {
                            bmpTemp = (Bitmap)bitmap.Clone();
                            var g = Graphics.FromImage(bmpTemp);
                            if (MainForm.isZalivka)
                                g.FillRectangle(new SolidBrush(MainForm.CurColor), new Rectangle(oldX, oldY, adjustedX - oldX, adjustedY - oldY));
                            else
                                g.DrawRectangle(pen, new Rectangle(oldX, oldY, adjustedX - oldX, adjustedY - oldY));
                            Invalidate();
                            break;
                        }
                    case Tool.Circle:
                        {
                            bmpTemp = (Bitmap)bitmap.Clone();
                            var g = Graphics.FromImage(bmpTemp);
                            if (MainForm.isZalivka)
                                g.FillEllipse(new SolidBrush(MainForm.CurColor), new Rectangle(oldX, oldY, adjustedX - oldX, adjustedY - oldY));
                            else
                                g.DrawEllipse(pen, new Rectangle(oldX, oldY, adjustedX - oldX, adjustedY - oldY));
                            Invalidate();
                            break;
                        }
                    case Tool.Lastik:
                        {
                            var g = Graphics.FromImage(bmpTemp);
                            g.FillEllipse(new SolidBrush(MainForm.CurColor), adjustedX - MainForm.CurSize / 2, adjustedY - MainForm.CurSize / 2, MainForm.CurSize, MainForm.CurSize);
                            oldX = adjustedX;
                            oldY = adjustedY;
                            bmpTemp = bitmap;
                            Invalidate();
                            break;
                        }
                    case Tool.Bucket:
                        {
                            Color targetColor = bitmap.GetPixel(adjustedX, adjustedY);
                            Color replacementColor = MainForm.CurColor;

                            if (targetColor != replacementColor)
                            {
                                FloodFill(adjustedX, adjustedY, targetColor, replacementColor);
                                bmpTemp = bitmap;
                                Invalidate();
                            }
                            break;
                        }
                    case Tool.Polygon:
                        {
                            bmpTemp = (Bitmap)bitmap.Clone();
                            var g = Graphics.FromImage(bmpTemp);
                            DrawPolygon(g, MainForm.PolygonSides, oldX, oldY, adjustedX, adjustedY, pen, MainForm.isZalivka);

                            Invalidate();
                            break;
                        }
                    case Tool.Text:
                        {
                            bmpTemp = (Bitmap)bitmap.Clone();
                            var g = Graphics.FromImage(bmpTemp);

                            var dlg = new Dialog();
                            dlg.ShowDialog();
                            string inputText = dlg.inText;
                            g.DrawString(inputText, new Font("Arial", 12), new SolidBrush(MainForm.CurColor), adjustedX, adjustedY);
                            bitmap = bmpTemp;
                            Invalidate();
                            break;
                        }
                }
            }
        }
        private void DrawPolygon(Graphics g, int sides, int x1, int y1, int x2, int y2, Pen pen, bool isZalivka)
        {
            double radius = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            double angle = Math.Atan2(y2 - y1, x2 - x1);
            PointF[] points = new PointF[sides];

            for (int i = 0; i < sides; i++)
            {
                double theta = angle + 2 * Math.PI * i / sides;
                points[i] = new PointF(
                    (float)(x1 + radius * Math.Cos(theta)),
                    (float)(y1 + radius * Math.Sin(theta))
                );
            }
            if (isZalivka)
                g.FillPolygon(new SolidBrush(MainForm.CurColor), points);
            else
                g.DrawPolygon(pen, points);

        }
        private void FloodFill(int x, int y, Color targetColor, Color replacementColor)
        {
            try
            {
                if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height)
                    return;

                if (bitmap.GetPixel(x, y) != targetColor)
                    return;

                if (bitmap.GetPixel(x, y) == replacementColor)
                    return;

                bitmap.SetPixel(x, y, replacementColor);

                FloodFill(x + 1, y, targetColor, replacementColor);
                FloodFill(x - 1, y, targetColor, replacementColor);
                FloodFill(x, y + 1, targetColor, replacementColor);
                FloodFill(x, y - 1, targetColor, replacementColor);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmap != null)
            {
                Graphics g = e.Graphics;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;


                int newWidth = (int)(bitmap.Width * zoomFactor);
                int newHeight = (int)(bitmap.Height * zoomFactor);

                int x = zoomCenter.X - (int)(zoomCenter.X * zoomFactor);
                int y = zoomCenter.Y - (int)(zoomCenter.Y * zoomFactor);

                g.DrawImage(bmpTemp, new Rectangle(x, y, newWidth, newHeight));
            }
        }

        private void DocumentForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomIn(e.Location);
            }
            else
            {
                ZoomOut(e.Location);
            }
        }

        public void ZoomIn(Point center)
        {
            zoomCenter = center;
            zoomFactor *= 1.1f;
            Invalidate();
        }

        public void ZoomOut(Point center)
        {
            zoomCenter = center;
            zoomFactor /= 1.1f;
            Invalidate();
        }

        private void DocumentForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var MF = MdiParent as MainForm;
            MF.UpdateMenu();
        }

        private void DocumentForm_SizeChanged(object sender, EventArgs e)
        {
            if (bitmap != null)
            {
                Bitmap newBmp = new Bitmap(ClientSize.Width, ClientSize.Height);
                Graphics g = Graphics.FromImage(newBmp);
                g.Clear(Color.White);
                g.DrawImage(bitmap, 0, 0);
                bitmap = newBmp;
                Invalidate();
            }
        }

        private void DocumentForm_MouseUp(object sender, MouseEventArgs e)
        {
            switch (MainForm.CurTool)
            {
                case Tool.Circle:
                case Tool.Rectangle:
                case Tool.Line:
                case Tool.Polygon:
                case Tool.Bucket:
                case Tool.Text:
                    bitmap = bmpTemp;
                    Invalidate();
                    break;
            }
        }

        private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var MF = MdiParent as MainForm;
            MF.UpdateMenu();
            if (isChanged)
            {
                DialogResult result = MessageBox.Show("Сохранить файл?", "Подтверждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    MF.ToolSave(this);
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
                else if (result == DialogResult.No)
                    return;
            }
        }
    }
}