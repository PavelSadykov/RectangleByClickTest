
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RectangleByClickTest
{
    public partial class Form1 : Form
    {
        private bool isGreen = true;//флаг на цвет
        private bool isDragging = false;//флаг на перемещение
        private Figure selectedFigure = null;
        private Point lastMousePos;

        // Graphics g;
        List<Figure> rectangles_lst = new List<Figure>();
        public Form1()
        {
            InitializeComponent();
            initCanvas();
        }
        private void initCanvas()
        {
            this.Size = new Size(900, 900);
        }
        private void addCell(Point point, Brush brush)
        {
            //this.Paint += Form1_Paint;
            Rectangle alarm = new Rectangle(0, 0, 135, 135);
            if (alarm.Contains(point))
            {
                return;
            }
            //  g = CreateGraphics();
            int height = 90;
            int width = 90;
            Rectangle rectCell = new Rectangle(
                point.X - (width / 2), point.Y - (height / 2)
                , width, height);

            foreach(var existingRect in rectangles_lst)
    {
                if (existingRect.rectangle.IntersectsWith(rectCell))
                {
                    
                    return;
                }
            }

            Brush cellBrush = isGreen ? Brushes.DarkGreen : Brushes.Red;
            Figure figure = new Figure(rectCell, cellBrush);


            rectangles_lst.Add(figure);
            Invalidate();
            isGreen = !isGreen; // Инвертируем цвет для следующей ячейки
        }
      
        private void Form1_Resize(object sender, EventArgs e)
        {
            string mess = string.Empty;
            int counter = 0;
            foreach (var item in rectangles_lst)
            {
                mess += $"№{++counter} {item.rectangle.X} : " +
                    $"{item.rectangle.Y} " + "\n";
            }
            MessageBox.Show($"{mess}");
        }

        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            foreach (var item in rectangles_lst)
            {
                e.Graphics.FillRectangle(item.brush, item.rectangle);
            }
        }

        private void Form1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            addCell(e.Location, Brushes.DarkGreen);
            
              foreach (var item in rectangles_lst)
              {
                  if (item.rectangle.Contains(e.Location))
                  {
                      RemoveSelectedCell();
                      break;
                  }
              }
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            foreach (var item in rectangles_lst)
            {
                if (item.rectangle.Contains(e.Location))
                {
                    selectedFigure = item;
                    isDragging = true;
                    lastMousePos = e.Location;
                    break;
                }
            }
            
        }

        private void Form1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (isDragging && selectedFigure != null)
            {
                int dx = e.Location.X - lastMousePos.X;
                int dy = e.Location.Y - lastMousePos.Y;

                Rectangle newRectangle = new Rectangle(
                selectedFigure.rectangle.X + dx,
                selectedFigure.rectangle.Y + dy,
                selectedFigure.rectangle.Width,
                selectedFigure.rectangle.Height);

                selectedFigure.rectangle = newRectangle;

                lastMousePos = e.Location;
                Invalidate();
            }
        }

        private void Form1_MouseUp_1(object sender, MouseEventArgs e)
        {
            isDragging = false;
            selectedFigure = null;
        }
        
        private void RemoveSelectedCell()
        {
            if (selectedFigure != null)
            {
                rectangles_lst.Remove(selectedFigure);
                selectedFigure = null;
                isDragging = false;
                Invalidate();
            }
        }
       

        private void btnDelete_Click(object sender, EventArgs e)
        {
          
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files|*.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in rectangles_lst)
                    {
                        string colorHex = ColorTranslator.ToHtml((item.brush as SolidBrush).Color);
                        writer.WriteLine($"{item.rectangle.X},{item.rectangle.Y},{item.rectangle.Width},{item.rectangle.Height},{colorHex}");
                    }
                }

                MessageBox.Show("Текущее состояние успешно сохранено в файл.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
