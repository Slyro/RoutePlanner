using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RoutePlanner
{
    public partial class Form1 : Form
    {
        int NextIndex;
        readonly string SheetName = "Курьеры";
        const int WaitTime = 20;
        int MaxOrdersPerTime = 15;
        int SelectedOrdersCount;
        readonly int OLExpansion = 2;
        int id1, id2;

        #region Функциональные объекты
        Form2 SettingsForm;
        Dictionary<string, int> Couriers = new Dictionary<string, int>();
        readonly Dictionary<int, string> Territories = new Dictionary<int, string>();
        List<string>[] OrderList;
        public int CourierOrdersCount => OrderList[listBox1.SelectedIndex].Count;
        public static string Url { get => AKDTools.Link; set => AKDTools.Link = value; }
        public Form2 SettingsForm1 { get => SettingsForm; set => SettingsForm = value; }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        public void LoadSettings()
        {
            AKDTools.Link = Settings1.Default.AKDLink;
            AKDTools.Login = Settings1.Default.Login;
            AKDTools.Password = Settings1.Default.Password;
            AKDTools.Territories = Settings1.Default.Territories;
            AKDTools.CenterID = Settings1.Default.CenterID;
            if (!string.IsNullOrEmpty(AKDTools.Territories))
            {
                ParseJsonData(AKDTools.Territories);
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                id1 = 0;
                id2 = 0;
                listBox1.Items.Clear();
            }

            using (ExcelManager ExcelManager = new ExcelManager())
            {
                if (ExcelManager.OpenExcelFile())
                {
                    listBox1.Items.Clear();
                    Couriers.Clear();
                    if (ExcelManager.FindWorksheet(SheetName))
                    {
                        Couriers = ExcelManager.GetCouriers(Couriers);
                        OrderList = new List<string>[Couriers.Count + OLExpansion];
                        for (int i = 0; i < OrderList.Length - OLExpansion; i++)
                        {
                            OrderList[i] = ExcelManager.GetOrders(Couriers.ElementAt(i).Value);
                        }
                    }
                    else
                    {
                        return;
                    }
                    driverButton.Enabled = true;
                    FillPostListBox();
                }
            }
        }
        private void DriverButton_Click(object sender, EventArgs e)
        {
            BackColor = Color.Red;
            //Запуск браузера.
            try
            {
                DriverManager.CreateDriverAndOpenUrl(Url, TimeSpan.FromSeconds(WaitTime));
            }
            catch (Exception)
            {
                MessageBox.Show("Нет ссылки на страницу ПО КД.");
                return;
            }
            //Вход по логину и паролю
            try
            {
                DriverManager.FindElementByName("login").SendKeys(AKDTools.Login);
                DriverManager.FindElementByName("password").SendKeys(AKDTools.Password);
                DriverManager.FindElementById("mx.app.page.Login.enter").Click();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Не удалось выполнить вход в ПО курьерской доставки.");
                if (DriverManager.IsRunning())
                {
                    DriverManager.Quit();
                }
            }

            BackColor = Color.FromKnownColor(KnownColor.Control);
            button1.Enabled = DriverManager.IsRunning();
            listBox1.Enabled = true;
        }
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridByOrders();
            if (DriverManager.IsRunning())
            {
                NewCourier();
                EnableSelectionButton();
            }
        }
        private void Startbutton_Click(object sender, EventArgs e)
        {
            Planning(MaxOrdersPerTime, NextIndex);
            ButtonRename();
            restartButton.Enabled = true;
        }
        private void RestartButton_Click(object sender, EventArgs e)
        {
            NewCourier();
            ButtonRename();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingsForm1.Dispose();
            DriverManager.Quit();
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox1.Checked;
            label1.Visible = checkBox1.Checked;
            if (!checkBox1.Checked)
            {
                numericUpDown1.Value = 15M;
            }
        }
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            MaxOrdersPerTime = (int)numericUpDown1.Value;
        }
        private void NewCourier()
        {
            if (AKDTools.HaveSelectedOrder)
            {
                AKDTools.Renew();
            }
            NextIndex = 0;
            SelectedOrdersCount = 0;
            ButtonRename();
            restartButton.Enabled = false;
            DeleteBackColor();
        }
        private void EnableSelectionButton()
        {
            if (!startbutton.Enabled)
            {
                if (DriverManager.IsRunning())
                {
                    startbutton.Enabled = true;
                }
            }
        }
        private void DeleteBackColor()
        {
            foreach (DataGridViewRow Rows in dataGridView1.Rows)
            {
                Rows.Cells[0].Style.BackColor = Color.White;
            }
        }
        private void FillDataGridByOrders()
        {
            dataGridView1.Columns.Clear();
            if (listBox1.SelectedIndex < OrderList.Length - OLExpansion)
            {
                try
                {
                    dataGridView1.Columns.Add(Couriers.ElementAt(listBox1.SelectedIndex).Key, Couriers.ElementAt(listBox1.SelectedIndex).Key);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                dataGridView1.Columns.Add(listBox1.SelectedItem.ToString(), listBox1.SelectedItem.ToString());
            }
            for (int i = 0; i < OrderList[listBox1.SelectedIndex].Count; i++)
            {
                dataGridView1.Rows.Add(OrderList[listBox1.SelectedIndex][i]);
            }
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        private void FillPostListBox()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < OrderList.Length; i++)
            {
                if (i < OrderList.Length - OLExpansion)
                {
                    listBox1.Items.Add($"{Couriers.ElementAt(i).Key} - {OrderList[i].Count}");
                }
                else
                {
                    if (OrderList[i] != null)
                    {
                        if (i == id1)
                            listBox1.Items.Add($"Незапланированно - {OrderList[i].Count}");
                        if (i == id2)
                            listBox1.Items.Add($"Запланировано - {OrderList[i].Count}");
                    }
                }
            }
        }
        private void ButtonRename()
        {
            startbutton.Text = NextIndex > 0 ? "Продолжить..." : "Выделение почты";
        }
                                                        //Поиск и выделение объектов на карте согласно выбраным территориям
        private void Planning(int maxOrders, int index) //TODO: Нужен ли тут async/await?
        {
            AKDTools.ScriptInject();
            AKDTools.JQLoaderWait();
            int[] zones = new int[listBox2.SelectedIndices.Count];
            for (int i = 0; i < listBox2.SelectedIndices.Count; i++)
            {
                zones[i] = Territories.ElementAt(listBox2.SelectedIndices[i]).Key;
            }
            if (zones.Length == 0)
            {
                MessageBox.Show("Нужно выбрать хотя бы одну территори.");
                return;
            }
            for (int i = index, j = 0; j < maxOrders && (i + j) < CourierOrdersCount; j++)
            {
                switch (AKDTools.SelectOrder(OrderList[listBox1.SelectedIndex][NextIndex], zones))
                {
                    case 0:// Нет совпадения территорий
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.NotEqualTerritoryColor;
                        break;
                    case 1:// Отправление найдено и выделено
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.SelectedOrderColor;
                        SelectedOrdersCount++;
                        break;
                    case 2:// Уже запланировано в рейс
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.PlannedFine;
                        break;
                    case 3:// Не найдено совсем.
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.NotFoundColor;
                        break;
                    case 4:// С Ошибкой.
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.UnplannedErrorColor;
                        break;
                    case 5:// В рейсе с ошибкой
                        dataGridView1.Rows[i + j].Cells[0].Style.BackColor = Settings1.Default.PlannedWithErrorColor;
                        break;
                }
                if (NextIndex <= CourierOrdersCount)
                {
                    NextIndex++;
                }
            }
        }
        private void ParseJsonData(string jsonTerritory)
        {
            dynamic data = JObject.Parse(jsonTerritory);
            foreach (var item in data.rows)
            {
                Territories.Add(Convert.ToInt32(item.id.Value), item.name.Value);
            }
            foreach (var item in Territories)
            {
                listBox2.Items.Add(item.Value);
            }
        }
        private void Button1_Click_1(object sender, EventArgs e)
        {
            GetOrders(ref id1, AKDTools.GetOrders.All);
        }
        private void GetOrders(ref int position,AKDTools.GetOrders type)
        {
            try
            {
                if (position == 0)
                {
                    for (int i = 0; i < OrderList.Length; i++)
                    {
                        if (OrderList[i] == null)
                        {
                            position = i;
                            OrderList[position] = AKDTools.GetOrderList(type);
                            break;
                        }
                    }
                }
                else
                {
                    OrderList[position] = AKDTools.GetOrderList(type);
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Драйвер на запущен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FillPostListBox();
        }
        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingsForm1.IsDisposed)
                SettingsForm1 = new Form2();
            SettingsForm1.ShowDialog();
        }
        private void ApplyNewSettinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Territories.Clear();
            listBox2.Items.Clear();
            LoadSettings();
        }
        private void GetPlannedordersButton_Click(object sender, EventArgs e)
        {
            GetOrders(ref id2, AKDTools.GetOrders.Planned);
        }
        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DriverManager.Quit();
            SettingsForm1.Dispose();
            Environment.Exit(0);
        }
        private void AuthourNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Валерий Степанской 2017 - 2019\n\n\nПрограмма создана на основе проектов:\n\nseleniumHQ\nJson.Net\nEPPlus", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.icon;
            readbookbutton.Enabled = true;
            driverButton.Enabled = false;
            startbutton.Enabled = false;
            restartButton.Enabled = false;
            LoadSettings();
            SettingsForm1 = new Form2();

            toolTip1.SetToolTip(button1, "Загрузить список незапланированных заказов");
            toolTip1.SetToolTip(getPlannedordersButton, "Загрузить список запланированых заказов из всех рейсов");
        }
    }
}