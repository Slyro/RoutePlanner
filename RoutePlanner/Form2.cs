using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

namespace RoutePlanner
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.BackColor = Settings1.Default.NotEqualTerritoryColor;
            textBox2.BackColor = Settings1.Default.NotFoundColor;
            textBox3.BackColor = Settings1.Default.PlannedWithErrorColor;
            textBox4.BackColor = Settings1.Default.SelectedOrderColor;
            textBox5.BackColor = Settings1.Default.UnplannedErrorColor;
            textBox6.BackColor = Settings1.Default.PlannedFine;

            linkTextBox.Text = Settings1.Default.AKDLink;
            loginTextBox.Text = Settings1.Default.Login;
            passwordTextBox.Text = Settings1.Default.Password;
            RouteLabelRename();
            saveSettingsButton.Enabled = false;
        }
        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Настройки сохранены", "Сохранить...", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            saveButtonControl(false);
        }
        private void linkTextBox_TextChanged(object sender, EventArgs e)
        {
            saveButtonControl();
        }
        private void loginTextBox_TextChanged(object sender, EventArgs e)
        {
            saveButtonControl();
        }
        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            saveButtonControl();
        }
        private void saveButtonControl()
        {
            if (!saveSettingsButton.Enabled)
            {
                saveSettingsButton.Enabled = true;
            }
        }
        private void saveButtonControl(bool state) => saveSettingsButton.Enabled = state;
        private void closeSettingsButton_Click(object sender, EventArgs e)
        {
            if (saveSettingsButton.Enabled)
                if (MessageBox.Show("Сохранить изменения?", "Сохранение", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    SaveSettings();
            Close();
        }
        private dynamic ParseJsonData(string json) => JObject.Parse(json);
        private void RouteLabelRename()
        {
            if (!TerritoriesIsEmpty())
                label4.Text = "Маршрутов: " + ParseJsonData(Settings1.Default.Territories).total;
            else
                label4.Text = "Маршрутов: 0";

            label5.Text = "Идентификатор УКД: " + Settings1.Default.CenterID;
        }
        private void getTerritoriesButton_Click(object sender, EventArgs e)
        {
            if (TerritoriesIsEmpty())
            {
                try
                {
                    GetCenterIdAndTerrs();
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Драйвер не запущен" + Environment.NewLine + "Кнопку нужно нажать на загруженной странице ПОКД");
                    return;
                }
                saveButtonControl();
            }
            else
            {
                if (MessageBox.Show("Список маршрутов был получен ранее" + Environment.NewLine + "Получить маршруты заного?", "Получить маршруты...", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        GetCenterIdAndTerrs();
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Драйвер не запущен" + Environment.NewLine + "Кнопку нужно нажать на загруженной странице ПОКД");
                        return;
                    }
                    saveButtonControl();
                }
            }
            RouteLabelRename();
        }
        private bool TerritoriesIsEmpty() => string.IsNullOrEmpty(Settings1.Default.Territories);
        private void GetCenterIdAndTerrs()
        {
            AKDTools.CenterID = Settings1.Default.CenterID = AKDTools.GetCenterID();
            Settings1.Default.Territories = AKDTools.GetTerritories();
        }
        private void SaveSettings()
        {
            AKDTools.Link = Settings1.Default.AKDLink = linkTextBox.Text;
            AKDTools.Login = Settings1.Default.Login = loginTextBox.Text;
            AKDTools.Password = Settings1.Default.Password = passwordTextBox.Text;
            AKDTools.Territories = Settings1.Default.Territories;
            Settings1.Default.Save();
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.BackColor = Settings1.Default.NotEqualTerritoryColor = colorDialog1.Color;
                saveButtonControl();
            }
        }
        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.BackColor = Settings1.Default.NotFoundColor = colorDialog1.Color;
                saveButtonControl();
            }
        }
        private void textBox3_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.BackColor = Settings1.Default.PlannedWithErrorColor = colorDialog1.Color;
                saveButtonControl();
            }
        }
        private void textBox4_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.BackColor = Settings1.Default.SelectedOrderColor = colorDialog1.Color;
                saveButtonControl();
            }
        }
        private void textBox5_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox5.BackColor = Settings1.Default.UnplannedErrorColor = colorDialog1.Color;
                saveButtonControl();
            }
        }
        private void textBox6_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox6.BackColor = Settings1.Default.PlannedFine = colorDialog1.Color;
                saveButtonControl();
            }
        }
    }
}