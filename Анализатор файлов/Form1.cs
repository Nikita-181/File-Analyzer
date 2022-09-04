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

using Microsoft.Win32;
using System.Runtime.InteropServices;


using System.Data.Sql;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Diagnostics;
using System.Reflection;

namespace Анализатор_файлов
{
    public partial class Form1 : Form
    {
        // Подключение библиотек WIN
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        List<string> devices = new List<string>(); // список подключенных USB накопителей
        List<string> USB_derectories = new List<string>(); // список папок(директорий) и подпапок USB накопителя
        List<string> format = new List<string>(); // список выделенных расширений
        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            //заполняем treeView
            treeView1.Nodes.Add("Архив");
            treeView1.Nodes.Add("Графика");
            treeView1.Nodes.Add("Презентация");
            treeView1.Nodes.Add("Документ");
            treeView1.Nodes[0].Nodes.Add(".rar");
            treeView1.Nodes[0].Nodes.Add(".zip");
            treeView1.Nodes[1].Nodes.Add(".jpeg(jpg)");
            treeView1.Nodes[1].Nodes.Add(".png");
            treeView1.Nodes[2].Nodes.Add(".pptx");
            treeView1.Nodes[3].Nodes.Add(".docx");
            treeView1.Nodes[3].Nodes.Add(".pdf");
            foreach (var dInfo in DriveInfo.GetDrives())
            {
                if (dInfo.IsReady && dInfo.DriveType == DriveType.Removable)
                {
                    listBox1.Items.Add(string.Format("{0} ({1})",
                          (string.IsNullOrEmpty(dInfo.VolumeLabel) ? "Съёмный диск" : dInfo.VolumeLabel),
                          dInfo.Name));
                    devices.Add(dInfo.Name); //добавляем имя устройства в лист
                }
            }
            Watcher_Start(); //запускаем слежку
        }
        public void Signature_analysis(string path) //Анализ файла
        {
            bool flag = false;

            Byte[] Nabor = System.IO.File.ReadAllBytes(path); // получаем набор байтов файла

            var fs = new FileStream(path, FileMode.Open);

            var signature = string.Join(" ", Nabor.Select(b => b.ToString("X2")));

            if (signature.Contains("FF D8 FF E0 00 10 4A 46 49 46 00 01") && format.Contains(".jpeg(jpg)")) //проверка на JPEG
            {
                flag = true;
            }

            if (signature.Contains("89 50 4E 47 0D 0A 1A 0A") && format.Contains(".png")) // проверка на PNG
            {
                flag = true;
            }

            if (signature.Contains("50 4B 03 04") && format.Contains(".docx")) // проверка на docx 
            {
                flag = true;
            }

            if ((signature.Contains("50 4B 03 04") || signature.Contains("50 4B 05 06") || signature.Contains("50 4B 07 08")) && format.Contains(".zip"))
            {
                flag = true;
            }
            //проверка на ZIP
            if (signature.Contains("50 4B 03 04") || signature.Contains("50 4B 05 06") || signature.Contains("50 4B 07 08"))
            {
                if (signature.Contains("6A 70 65 67") && format.Contains(".jpeg(jpg)"))
                {
                    flag = true;
                }
                if (signature.Contains("6A 70 67 0A") && format.Contains(".jpeg(jpg)"))
                {
                    flag = true;
                }
                if (signature.Contains("74 78 74 55") && format.Contains(".txt"))
                {
                    flag = true;
                }
                if (signature.Contains("7A 69 70") && format.Contains(".zip"))
                {
                    flag = true;
                }
                if (signature.Contains("70 64 66 A") && format.Contains(".pdf"))
                {
                    flag = true;
                }
                if (signature.Contains("70 6E 67 0A") && format.Contains(".png"))
                {
                    flag = true;
                }
                if (signature.Contains("2E 70 70 74 78") && format.Contains(".pptx"))
                {
                    flag = true;
                }
                if (signature.Contains("2E 64 6F 63 78") && format.Contains(".docx"))
                {
                    flag = true;
                }
                if (signature.Contains("6D 70 34 0A") && format.Contains(".mp4"))
                {
                    flag = true;
                }
            }
            if (signature.Contains("52 61 72 21 1A 07") && format.Contains(".rar")) //  RAR
            {
                flag = true;
            }
            if (signature.Contains("52 61 72 21 1A 07")) // проверка на RAR
            {
                if (signature.Contains("52 61 72 21 1A 7") && format.Contains(".rar"))
                {
                    flag = true;
                }
                if (signature.Contains("6A 70 65 67") && format.Contains(".jpeg(jpg)"))
                {
                    flag = true;
                }
                if (signature.Contains("6A 70 67 0A") && format.Contains(".jpeg(jpg)"))
                {
                    flag = true;
                }
                if (signature.Contains("74 78 74 0A") && format.Contains(".txt"))
                {
                    flag = true;
                }
                if (signature.Contains("7A 69 70") && format.Contains(".zip"))
                {
                    flag = true;
                }
                if (signature.Contains("70 64 66 0A") && format.Contains(".pdf"))
                {
                    flag = true;
                }
                if (signature.Contains("70 6E 67 0A") && format.Contains(".png"))
                {
                    flag = true;
                }
                if (signature.Contains("6D 70 34 0A") && format.Contains(".mp4"))
                {
                    flag = true;
                }
                if (signature.Contains("2E 70 70 74 78") && format.Contains(".pptx"))
                {
                    flag = true;
                }
                if (signature.Contains("2E 64 6F 63 78") && format.Contains(".docx"))
                {
                    flag = true;
                }
            }
            if (signature.Contains("25 50 44 46 2D") && format.Contains(".pdf")) //проверка на pdf
            {
                flag = true;
            }
            fs.Close();
            if (flag)
            {
                textBox1.Text += path + "\r\n";
                File.Delete(path);
            }
        }

        protected override void WndProc(ref Message m) //отслеживание подключенных устройств
        {
            const int WM_DEVICECHANGE = 0x0219;

            const int ADD_DEVICE = 0x8000;

            const int REMOVE_DEVICE = 0x8004;

            if (m.Msg == WM_DEVICECHANGE)
            {
                switch ((int)m.WParam)
                {
                    case ADD_DEVICE: // Устройство подключено
                        {
                            MessageBox.Show("Подключен внешний носитель.");
                            foreach (var dInfo in DriveInfo.GetDrives())
                            {
                                if (dInfo.IsReady && dInfo.DriveType == DriveType.Removable && !listBox1.Items.Contains(string.Format("{0} ({1})", (string.IsNullOrEmpty(dInfo.VolumeLabel) ? "Съёмный диск" : dInfo.VolumeLabel), dInfo.Name)))
                                {
                                    listBox1.Items.Add(string.Format("{0} ({1})",
                                          (string.IsNullOrEmpty(dInfo.VolumeLabel) ? "Съёмный диск" : dInfo.VolumeLabel),
                                          dInfo.Name)); //Выводим устройство в список устройств
                                    devices.Add(dInfo.Name); //добавляем имя устройства в лист
                                }
                            }
                            Watcher_Start(); //запускаем слежку
                            break;
                        }
                    case REMOVE_DEVICE: //Устройство отключено
                        {
                            MessageBox.Show("Внешний носитель отключен.");
                            listBox1.Items.Clear(); // очищаем список для пересборки
                            devices.Clear(); // очищаем список для пересборки
                            foreach (var dInfo in DriveInfo.GetDrives())
                            {
                                if (dInfo.IsReady && dInfo.DriveType == DriveType.Removable)
                                {
                                    listBox1.Items.Add(string.Format("{0} ({1})",
                                          (string.IsNullOrEmpty(dInfo.VolumeLabel) ? "Съёмный диск" : dInfo.VolumeLabel),
                                          dInfo.Name));//Выводим устройство в список устройств
                                    devices.Add(dInfo.Name);//добавляем имя устройства в лист
                                }
                            }
                            break;
                        }
                }
            }
            base.WndProc(ref m); // Переопределение оконной процедуры
        }

        public void Watcher_Start() //отслеживание 
        {
            foreach (string dev in devices) //активируем отслеживание через путь к каталогам устройств(счет по устройствам)
            {
                DirectoryInfo dir = new DirectoryInfo(@dev);
                USB_derectories.Add(@dev);//записываем главную директорию - путь к USB накопителю
                foreach (var item in dir.GetDirectories()) // получаем все папки и подпапки
                {
                    //берем католог, который не System Volume Information и добываем в нем все папки и подпапки
                    if (item.Name != "System Volume Information")
                    {
                        USB_derectories.Add(item.FullName);
                        USB_derectories.AddRange(Directory.GetDirectories(item.FullName, "*", SearchOption.AllDirectories)); // получаем все папки и подпапки с полным путем

                    }
                }
                foreach (var folders in USB_derectories)
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(folders);//@"F:\");
                    watcher.EnableRaisingEvents = true;
                    watcher.SynchronizingObject = this;
                    watcher.Created += new FileSystemEventHandler(Watcher_Created);// отслеживаем создание файла
                }
                USB_derectories.Clear();
            }
        }
        void Watcher_Created(object sender, FileSystemEventArgs e) //реакция на созданный файл
        {
            if (System.IO.File.Exists(e.FullPath)) //проверяем объект на пренадлежность к файлу
            {
                long s1 = GetFileSize(e.FullPath); // берем размер папки
                System.Threading.Thread.Sleep(1000); // задержка для изменения рамера пир копировании
                long s2 = GetFileSize(e.FullPath); // 2й раз берем размер папки
                while (s1 - s2 != 0) // ждем пока размер папки станет неизменным
                {
                    s1 = GetFileSize(e.FullPath);
                    System.Threading.Thread.Sleep(1000);
                    s2 = GetFileSize(e.FullPath);
                }
                Signature_analysis(e.FullPath);
            }
            else //если это папка 
            {
                long s1 = GetFolderSize(e.FullPath); // берем размер папки
                System.Threading.Thread.Sleep(1000); // задержка для изменения рамера пир копировании
                long s2 = GetFolderSize(e.FullPath); // 2й раз берем размер папки
                while (s1 - s2 != 0) // ждем пока размер папки станет неизменным
                {
                    s1 = GetFolderSize(e.FullPath);
                    System.Threading.Thread.Sleep(1000);
                    s2 = GetFolderSize(e.FullPath);
                }
                List<string> files = new List<string>(); // список файлов в папке
                files.AddRange(Directory.GetFiles(e.FullPath, "*", SearchOption.AllDirectories)); // получаем абсолютно все файлы из все папок и подпапок
                foreach (var file in files)
                {
                    Signature_analysis(file);
                }
                Watcher_Start(); //перенастраиваем слежку
            }
        }
        private static long GetFolderSize(string path) //рамер папки
        {
            try // пробуем взять рамер папки
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return di.EnumerateFiles(".", SearchOption.AllDirectories).Sum(n => n.Length);
            }
            catch // если папка занята другим процессом
            {
                return GetFolderSize(path);
            }
        }
        private static long GetFileSize(string path) //рамер файла
        {
            try // пробуем взять рамер файла
            {
                long size = new System.IO.FileInfo(path).Length;
                return size;
            }
            catch // если файл занята другим процессом
            {
                return GetFileSize(path);
            }
        }
        bool ignore = false; //"игнорирование" события AfterCheck
        bool ignore2 = false; //"игнорирование" для общей отметки
        private void CheckChildren(TreeNode rootNode, bool isChecked) //выделяем все подузлы
        {
            ignore = true;
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                if (node.Checked != isChecked)
                {
                    node.Checked = isChecked;
                }
                if (!format.Contains(node.Text) && isChecked == true)//избегаем дублеров из-за срабатывания события при ручном выделении
                {
                    format.Add(node.Text);
                }
                if (isChecked == false)
                {
                    format.Remove(node.Text);
                }
            }
        }
        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Checked) //если был выделен перед снятием выделения
            {
                if (!e.Node.FullPath.Contains("\\"))//если это родительский узел
                {
                    ignore = true;
                    ignore2 = true;
                    //проверяем все ли дочерние элементы имеют тот же статус, что и родитель. Если не имеют, то игнорируем
                    bool checked_status = true;
                    string[] g = (e.Node.FullPath).Split('\\'); //находим имя родительского узела
                    TreeNode[] main = treeView1.Nodes.Find("", false); //находим все родительские узлы
                    foreach (TreeNode items in main)
                    {
                        if (items.Text == g[0]) //если совпало имя
                        {
                            foreach (TreeNode child in items.Nodes) // перебор подузлов
                            {
                                if (child.Checked != items.Checked) checked_status = false; //если не отмечен, то false
                            }
                            if (checked_status == true) // Если все статусы совпадают, то обращаемся ко всем
                            {
                                e.Node.BackColor = Color.White;
                                CheckChildren(e.Node, false); //отправляем на выделение подузлов
                            }
                        }
                    }
                    ignore = false;
                    ignore2 = false;
                }
                else
                {
                    if (ignore2 == false) // если beforecheck произошел не во время отметки всех дочерних узлов
                    {
                        ignore = true;

                        bool checked_status = false;
                        string[] g = (e.Node.FullPath).Split('\\'); //находим имя родительского узела
                        TreeNode[] main = treeView1.Nodes.Find("", false); //находим все родительские узлы
                        foreach (TreeNode items in main)
                        {
                            if (items.Text == g[0]) //если совпало имя
                            {
                                foreach (TreeNode child in items.Nodes) // перебор подузлов
                                {
                                    if (child.Checked != false && child.Text != e.Node.Text) checked_status = true; //если не отмечен, то false
                                }
                                if (checked_status != true && format.Contains(e.Node.Text))
                                {
                                    format.Remove(e.Node.Text); //добавляем в список отмеченных(в ручном режиме)
                                    items.BackColor = Color.White;
                                    CheckChildren(e.Node, false); //отправляем на выделение подузлов
                                }
                                else
                                {
                                    format.Remove(e.Node.Text); //добавляем в список отмеченных(в ручном режиме)
                                    CheckChildren(e.Node, false); //отправляем на выделение подузлов
                                }
                            }
                        }
                        ignore = false;
                    }
                }
            }
            else
            {
                if (!e.Node.FullPath.Contains("\\"))//если это родительский узел
                {
                    e.Node.BackColor = Color.LightGreen;
                    CheckChildren(e.Node, true); //отправляем на выделение подузлов
                }
                else
                {
                    string[] g = (e.Node.FullPath).Split('\\'); //находим имя родительского узела
                    TreeNode[] main = treeView1.Nodes.Find("", false); //находим все родительские узлы
                    foreach (TreeNode items in main)
                    {
                        if (items.Text == g[0]) //если совпало имя
                        {
                            items.BackColor = Color.LightGreen; //меняем фоновый цвет
                        }
                    }
                    format.Add(e.Node.Text); //добавляем в список отмеченных(в ручном режиме)
                    CheckChildren(e.Node, true); //отправляем на выделение подузлов(нужно при нажатии на родителя, по событию заходит сюда)
                }
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked == false && e.Node.FullPath.Contains("\\") && ignore == false) // если это дочерний элемент и его состояние false
            {
                bool checked_status = false;
                string[] g = (e.Node.FullPath).Split('\\'); //находим имя родительского узела
                TreeNode[] main = treeView1.Nodes.Find("", false); //находим все родительские узлы
                foreach (TreeNode items in main)
                {
                    if (items.Text == g[0]) //если совпало имя
                    {
                        foreach (TreeNode child in items.Nodes) // перебор подузлов
                        {
                            if (child.Checked == items.Checked && child.Text == e.Node.Text) checked_status = true; //если не отмечен, то false
                        }
                        if (checked_status != true) items.Checked = false; //если статус остался true(все подузлы отмечены), то отмечаем родительский узел
                    }
                }
            }
        }

        private void dragAndDrop_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Byte[] Nabor = System.IO.File.ReadAllBytes(dialog.FileName); // получаем набор байтов файла
                    GetHexAndDump(Nabor, dialog.SafeFileName);
                }
            }
            Single_analysis();
        }
        string File_signature;
        List<char> Win1251StNabor = new List<char>();
        public void GetHexAndDump(Byte[] Nabor, string FileName) // Получаем HEX и Windows 1251 набор файла и выподим данный в соотвествующий datagrid
        {
            Win1251StNabor.Clear();
            string[] StNabor = new string[Nabor.Length]; //массив байтов в string с высогим регистром в HEX
            string[] Buf = new string[Nabor.Length];//массив для особых символов файлов в архиве
            for (int i = 0; i <= (Nabor.Length - 1); i++) //обмент м-ду массивами
            {
                StNabor[i] = Convert.ToString(Nabor[i], 16).ToUpper();
                if (StNabor[i].Length == 1) StNabor[i] = "0" + StNabor[i];
            }
            Win1251StNabor.AddRange(Encoding.GetEncoding(1251).GetString(Nabor));
            label7.Visible = true;
            label7.Text = FileName;

            File_signature = string.Join(" ", Nabor.Select(b => b.ToString("X2")));
            int columns = 10;
            dataGridView1.RowCount = (StNabor.Length / columns) + 1;
            dataGridView1.ColumnCount = columns;
            dataGridView2.RowCount = (StNabor.Length / columns) + 1;
            dataGridView2.ColumnCount = columns;
            int c = 0;
            int r = 0;
            for (int col = 0; col < columns; col++) // нумерация столбцов
            {
                dataGridView1.Columns[col].HeaderCell.Value = (col + 1).ToString();
                dataGridView2.Columns[col].HeaderCell.Value = (col + 1).ToString();
            }
            for (int j = 0; j < StNabor.Length; j++)
            {
                dataGridView1.Rows[r].Cells[c].Value = StNabor[j];
                char ggg = Win1251StNabor[j];
                dataGridView2.Rows[r].Cells[c].Value = Win1251StNabor[j];

                c += 1;
                if (c == columns)
                {
                    r += 1;
                    c = 0;
                }
            }

            int Width1 = ((dataGridView1.Width) / (dataGridView1.Columns.Count + 1));
            int Width2 = ((dataGridView2.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth) / dataGridView2.Columns.Count);
            //dataGridView1.RowHeadersWidth = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders; //.AutoResizeColumns();
            //dataGridView1.RowHeadersBorderStyle.
            for (int i = 0; i < dataGridView1.RowCount; i++) //назначаем рамеры ячейкам
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1.Columns[j].Width = Width1;
                    dataGridView2.Columns[j].Width = Width2;
                }
            }
        }

        private void dataGridView2_Scroll(object sender, ScrollEventArgs e) // синхронизируем прокрутку(предварительно оставив прокрутку только у второго datagrid)
        {
            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView2.FirstDisplayedScrollingRowIndex;
        }
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            this.dataGridView2.FirstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;
        }

        bool DG1_Focus = false; // состояние фокуса datagrid1
        bool DG2_Focus = false; // состояние фокуса datagrid2

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) // Реагируем на изменение выделения
        {
            if (DG1_Focus == true) // если манипуляции ведуться на этом datagrid
            {
                if (dataGridView2.Columns.Count != 0) // избегаем ошибок при старте или открытии нового файла
                {
                    dataGridView2.ClearSelection(); // очищаем все выделения на противоположном datagrid
                    foreach (var i in dataGridView1.SelectedCells) //добываем координаты ячейки
                    {
                        string[] cell = (Convert.ToString(i.ToString())).Split(',');
                        string X = "";
                        string Y = "";
                        foreach (var x in cell[0])
                        {
                            if (Char.IsNumber(x)) X += x;
                        }
                        foreach (var y in cell[1])
                        {
                            if (Char.IsNumber(y)) Y += y;
                        }
                        dataGridView2.Rows[Convert.ToInt32(Y)].Cells[Convert.ToInt32(X)].Selected = true; //делаем аналогичное выделение на противоположном datagrid
                    }
                }
            }
        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (DG2_Focus == true) // если манипуляции ведуться на этом datagrid
            {
                if (dataGridView2.Columns.Count != 0)
                {
                    dataGridView1.ClearSelection(); // очищаем все выделения
                    foreach (var i in dataGridView2.SelectedCells)
                    {
                        string[] cell = (Convert.ToString(i.ToString())).Split(',');
                        string X = "";
                        string Y = "";
                        foreach (var x in cell[0])
                        {
                            if (Char.IsNumber(x)) X += x;
                        }
                        foreach (var y in cell[1])
                        {
                            if (Char.IsNumber(y)) Y += y;
                        }
                        dataGridView1.Rows[Convert.ToInt32(Y)].Cells[Convert.ToInt32(X)].Selected = true;
                    }
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e) //самостоятельно отслеживаем фокус по клику на datagrid
        {
            DG1_Focus = true;
            DG2_Focus = false;
        }

        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)//самостоятельно отслеживаем фокус по клику на datagrid
        {
            DG1_Focus = false;
            DG2_Focus = true;
        }

        private void dragAndDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dragAndDrop.Text = "Отпустите мышь.";
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dragAndDrop_DragLeave(object sender, EventArgs e)
        {
            dragAndDrop.Text = "Нажмите, чтобы выбрать файл," + "\n" + "или перетащите его в эту область";
        }

        private void dragAndDrop_DragDrop(object sender, DragEventArgs e)
        {
            dragAndDrop.Text = "Нажмите, чтобы выбрать файл," + "\n" + "или перетащите его в эту область";
            string[] Path = (string[])e.Data.GetData(DataFormats.FileDrop);
            Byte[] Nabor = System.IO.File.ReadAllBytes(Path[0]); // получаем набор байтов файла
            string[] FileName = Path[0].Split('\\');
            GetHexAndDump(Nabor, FileName[FileName.Length - 1]);
            Single_analysis();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e) // нумерация строк datagridview с выравниванием текста по центру
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();
            var centerformat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerformat);

        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Checked = false;
            }
        }
        List<int> RowHeadersNumber = new List<int>(); //сроки на которых начинается икомая сигнатура или набор символов
        private void searchButton_Click(object sender, EventArgs e)
        {
            for (int row = 0; row < dataGridView1.RowCount; row++) //очищаем фон
            {
                for (int cell = 0; cell < dataGridView1.ColumnCount; cell++)
                {
                    dataGridView1.Rows[row].Cells[cell].Style.BackColor = Color.White;
                    dataGridView2.Rows[row].Cells[cell].Style.BackColor = Color.White;
                }
            }

            dataGridView3.Rows.Clear();

            if (checkBox1.Checked) //HEX
            {
                RowHeadersNumber.Clear();
                string[] text_array = textBox2.Text.Split(' '); // искомый набор без пробелов
                var source = File_signature.Split(' '); //набор сигнатур без пробелов
                var result = Searc_in_HEX(text_array, source);
                dataGridView3.RowCount = result.Item2; //строим таблицу результатов
                dataGridView3.ColumnCount = text_array.Length;
                int c = 0;
                int r = 0;
                for (int j = 0; j < source.Length; j++)
                {
                    if (result.Item1[j] == 1)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.Green; // красим ячейки с совпадением
                        dataGridView2.Rows[r].Cells[c].Style.BackColor = Color.Green;
                    }
                    c += 1;
                    if (c == 10)
                    {
                        r += 1;
                        c = 0;
                    }
                }

                for (int row = 0; row < dataGridView3.RowCount; row++) // заполняем таблицу результатов
                {
                    for (int cell = 0; cell < dataGridView3.ColumnCount; cell++)
                    {
                        dataGridView3.Rows[row].Cells[cell].Value = text_array[cell];
                        dataGridView3.Rows[row].HeaderCell.Value = RowHeadersNumber[row];
                    }
                }
            }
            else //Win1251StNabor(текст)
            {
                int found_count = 0; // кол-во найденных наборов
                RowHeadersNumber.Clear();
                string[] text_array = textBox2.Text.Split(' '); // искомый набор без пробелов
                var source = Win1251StNabor; //набор сигнатур без пробелов
                int[] Selected_mass = new int[source.Count]; //массив совпадений
                int i = 0; // индекс первого вхождения искомого набора в источнике
                while (i <= source.Count - text_array.Length)
                {
                    // ищем искомый набор в источнике
                    bool f = true;
                    for (int ii = i; ii <= source.Count - text_array.Length; ii++)
                    {
                        f = true;
                        for (int jj = 0; jj < text_array.Length; jj++)
                        {
                            f &= source[ii + jj] == Convert.ToChar(text_array[jj]);
                            if (!f) break;
                        }
                        if (f)
                        {
                            i = ii;
                            break;
                        }
                    }

                    if (f == false) break; // если не находим, то прекращаем поиск
                    for (int k = i; k < i + text_array.Length; k++) //запоминаем позиции в источнике
                    {
                        Selected_mass[k] = 1;
                    }
                    RowHeadersNumber.Add(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(i + 1) / Convert.ToDecimal(10)))); // запоминаем номер строки
                    i += text_array.Length; // сдвигаем начало поиска следующего совпадения
                    found_count += 1;
                }
                dataGridView3.RowCount = found_count; //строим таблицу результатов
                dataGridView3.ColumnCount = text_array.Length;
                int c = 0;
                int r = 0;
                for (int j = 0; j < source.Count; j++)
                {
                    if (Selected_mass[j] == 1)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.Green; // красим ячейки с совпадением
                        dataGridView2.Rows[r].Cells[c].Style.BackColor = Color.Green;
                    }
                    c += 1;
                    if (c == 10)
                    {
                        r += 1;
                        c = 0;
                    }
                }

                for (int row = 0; row < dataGridView3.RowCount; row++) // заполняем таблицу результатов
                {
                    for (int cell = 0; cell < dataGridView3.ColumnCount; cell++)
                    {
                        dataGridView3.Rows[row].Cells[cell].Value = text_array[cell];
                        dataGridView3.Rows[row].HeaderCell.Value = RowHeadersNumber[row];
                    }
                }
            }
        }
        public (int[], int) Searc_in_HEX(string[] text_array, string[] source) // поиск в HEX
        {
            int[] Selected_mass = new int[source.Length]; //массив совпадений
            int i = 0; // индекс первого вхождения искомого набора в источнике
            int found_count = 0; // кол-во найденных наборов
            while (i <= source.Length - text_array.Length)
            {
                // ищем искомый набор в источнике
                bool f = true;
                for (int ii = i; ii <= source.Length - text_array.Length; ii++)
                {
                    f = true;
                    for (int jj = 0; jj < text_array.Length; jj++)
                    {
                        f &= source[ii + jj] == text_array[jj];
                        if (!f) break;
                    }
                    if (f)
                    {
                        i = ii;
                        break;
                    }
                }

                if (f == false) break; // если не находим, то прекращаем поиск
                for (int k = i; k < i + text_array.Length; k++) //запоминаем позиции в источнике
                {
                    Selected_mass[k] = 1;
                }
                RowHeadersNumber.Add(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(i + 1) / Convert.ToDecimal(10)))); // запоминаем номер строки
                i += text_array.Length; // сдвигаем начало поиска следующего совпадения
                found_count += 1;
            }
            return (Selected_mass, found_count);
        }

        private void dataGridView3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (RowHeadersNumber[e.RowIndex]).ToString();
            var centerformat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerformat);
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[Convert.ToInt32(dataGridView3.Rows[e.RowIndex].HeaderCell.Value.ToString()) - 1].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].HeaderCell.Value) - 1;
        }

        private void Analysis_button_Click(object sender, EventArgs e)
        {
            Single_analysis();
        }
        public void Single_analysis()
        {
            for (int row = 0; row < dataGridView1.RowCount; row++) //очищаем фон
            {
                for (int cell = 0; cell < dataGridView1.ColumnCount; cell++)
                {
                    dataGridView1.Rows[row].Cells[cell].Style.BackColor = Color.White;
                    dataGridView2.Rows[row].Cells[cell].Style.BackColor = Color.White;
                }
            }
            dataGridView3.Rows.Clear();


            dataGridView3.RowCount = 1; //строим таблицу результатов
            dataGridView3.ColumnCount = 2;
            var source = File_signature.Split(' '); //набор сигнатур без пробелов
            string[] Signatures = { "FF D8 FF E0 00 10 4A 46 49 46 00 01", "89 50 4E 47 0D 0A 1A 0A",
                "50 4B 03 04", "50 4B 03 04", "50 4B 05 06", "50 4B 07 08","6A 70 65 67","6A 70 67 0A",
                "74 78 74 55","7A 69 70","70 64 66 A","70 6E 67 0A","2E 70 70 74 78","2E 64 6F 63 78",
                "6D 70 34 0A","52 61 72 21 1A 07","25 50 44 46 2D" };


            string[] Formats = { ".jpeg(jpg)", ".png", ".docx", ".zip", ".zip", ".zip", ".jpeg(jpg)",
                ".jpeg(jpg)", ".txt", ".zip", ".pdf", ".png",".pptx",".docx",".mp4",".rar",".pdf" };
            foreach (var sig in Signatures)
            {
                var text_array = sig.Split(' ');
                var result = Searc_in_HEX(text_array, source);
                int c = 0;
                int r = 0;
                for (int j = 0; j < source.Length; j++)
                {
                    if (result.Item1[j] == 1)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.Green; // красим ячейки с совпадением
                        dataGridView2.Rows[r].Cells[c].Style.BackColor = Color.Green;
                    }
                    c += 1;
                    if (c == 10)
                    {
                        r += 1;
                        c = 0;
                    }
                }
                for (int i = 0; i < result.Item2; i++)
                {
                    if (dataGridView3.Rows[0].Cells[0].Value == null)
                    {
                        dataGridView3.Rows[0].Cells[0].Value = sig;
                        dataGridView3.Rows[0].Cells[1].Value = Formats[Array.IndexOf(Signatures, sig)];
                        int ggg = RowHeadersNumber[dataGridView3.Rows.Count - 1];
                    }
                    else
                    {
                        DataGridViewRow row = (DataGridViewRow)dataGridView3.Rows[0].Clone();
                        row.Cells[0].Value = sig;
                        row.Cells[1].Value = Formats[Array.IndexOf(Signatures, sig)];
                        dataGridView3.Rows.Add(row);
                        int ggg = RowHeadersNumber[dataGridView3.Rows.Count - 1];
                    }
                }
            }
            for (int row = 0; row < dataGridView3.Rows.Count; row++)
            {
                dataGridView3.Rows[row].HeaderCell.Value = RowHeadersNumber[row];
            }

        }
    }
}
