using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;
using System.Timers;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static String apiKey = Properties.Settings.Default.apiKey;

        public static String refID = "68022";

        public static String username;
        public static String balance;
        public static String cpuTimeLeft;
        public static String cpuTimeLeftPercent;

        public Form1()
        {
            InitializeComponent();
            startApp();
        }
        private void startApp()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var testKey = webClient.DownloadString("https://1broker.com/api/v2/user/details.php?token=" + apiKey);

                var jtestKey = JsonConvert.DeserializeObject<dynamic>(testKey);

                if (jtestKey.error == true)
                {
                    MessageBox.Show("Error: "+ jtestKey.error_code);
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                    comboBox1.Enabled = false;
                }
                else
                {
                    radioButton1.Enabled = true;
                    radioButton2.Enabled = true;
                    comboBox1.Enabled = true;

                    openPositions();
                    openOrders();

                    setUserInfo();
                    label1.Text = cpuTimeLeftPercent + "%";
                    labelBalance.Text = balance + " BTC";
                    labelUsername.Text = username;

                    SetTimer();

                    textBoxApiKey.Text = Properties.Settings.Default.apiKey;

                    if (radioButton3.Checked == true)
                    {
                        textBox2.Enabled = false;
                    }
                    else
                    {
                        textBox2.Enabled = true;
                    }
                }
            }
        }

        private void setUserInfo()
        {
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                    var userDetail = webClient.DownloadString("https://1broker.com/api/v2/user/details.php?token=" + apiKey);
                    var cpuTime = webClient.DownloadString("https://1broker.com/api/v2/quota/status.php");

                    var jUserDetail = JsonConvert.DeserializeObject<dynamic>(userDetail);
                    var jcpuTime = JsonConvert.DeserializeObject<dynamic>(cpuTime);

                    balance = jUserDetail.response.balance.ToString();
                    username = jUserDetail.response.username.ToString();
                    cpuTimeLeft = jcpuTime.response.cpu_time_left.ToString();
                    cpuTimeLeftPercent = jcpuTime.response.cpu_time_left_percentage.ToString();
                }
            }
            catch
            {
                setUserInfo();
            }

        }
        public void openPositions()
        {
            do
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    try
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                    catch (Exception) { }
                }
            } while (dataGridView1.Rows.Count > 1);

            dataGridView1.AllowUserToAddRows = true;

            dataGridView1.DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = SystemColors.MenuBar;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeRows = false;

            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.RowHeadersVisible = false;

            using (var webClient = new System.Net.WebClient())
            {
            var openPos = webClient.DownloadString("https://1broker.com/api/v2/position/open.php?token="+apiKey);

            var jopenPos = JsonConvert.DeserializeObject<dynamic>(openPos);


                if (jopenPos.response.Count<1)
             {
                    dataGridView1.AllowUserToAddRows = false;
             }

            for (int s = 0; s < jopenPos.response.Count; s++)
            {
                var openPosPrice = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token="+apiKey+"&symbols="+ jopenPos.response[s].symbol);

                var jopenPosPrice = JsonConvert.DeserializeObject<dynamic>(openPosPrice);

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[s].Clone();
                row.Cells[0].Value = jopenPos.response[s].symbol.ToString();
                row.Cells[1].Value = jopenPos.response[s].margin.ToString() + " BTC";
                row.Cells[2].Value = jopenPos.response[s].leverage.ToString();
                row.Cells[4].Value = jopenPos.response[s].profit_loss_percent.ToString()+"%";
                row.Cells[3].Value = jopenPos.response[s].entry_price.ToString();
                row.Cells[6].Value = jopenPos.response[s].stop_loss.ToString();
                row.Cells[7].Value = jopenPos.response[s].take_profit.ToString();

                    if (jopenPos.response[s].direction.ToString().ToUpper() == "LONG")
                    {
                        row.Cells[5].Value = jopenPosPrice.response[0].bid;
                    }
                    else
                    {
                        row.Cells[5].Value = jopenPosPrice.response[0].ask;
                    }
                

                row.Cells[8].Value = jopenPos.response[s].direction.ToString().ToUpper();
                row.Cells[9].Value = "Close";
                dataGridView1.Rows.Add(row);

                if (s == (jopenPos.response.Count-1))
                {
                    dataGridView1.AllowUserToAddRows = false;
                }
            }
          }


    }
        public void openOrders()
        {
            do
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    try
                    {
                        dataGridView2.Rows.Remove(row);
                    }
                    catch (Exception) { }
                }
            } while (dataGridView2.Rows.Count > 1);

            dataGridView2.AllowUserToAddRows = true;

            dataGridView2.DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView2.DefaultCellStyle.BackColor = SystemColors.MenuBar;
            dataGridView2.DefaultCellStyle.ForeColor = Color.Black;

            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView2.AllowUserToResizeRows = false;

            dataGridView2.AllowUserToResizeColumns = false;
            dataGridView2.RowHeadersVisible = false;

            using (var webClient = new System.Net.WebClient())
            {
                var openOrd = webClient.DownloadString("https://1broker.com/api/v2/order/open.php?token=" + apiKey);

                var jopenOrd = JsonConvert.DeserializeObject<dynamic>(openOrd);

                if (jopenOrd.response.Count < 1)
                {
                    dataGridView2.AllowUserToAddRows = false;
                }

                for (int s = 0; s < jopenOrd.response.Count; s++)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[s].Clone();
                    row.Cells[0].Value = jopenOrd.response[s].symbol.ToString();
                    row.Cells[1].Value = jopenOrd.response[s].margin.ToString() + " BTC";
                    row.Cells[2].Value = jopenOrd.response[s].leverage.ToString();
                    row.Cells[3].Value = jopenOrd.response[s].direction.ToString().ToUpper();
                    row.Cells[4].Value = "Cancel";
                    dataGridView2.Rows.Add(row);

                    if (s == (jopenOrd.response.Count - 1))
                    {
                        dataGridView2.AllowUserToAddRows = false;
                        GC.Collect();
                    }
                }
            }
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
            }
            else
            {
                notifyIcon1.Visible = false;
            }
        }

        private void schließenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "0.00")
            {
                textBox2.Text = "";
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0.00")
            {
                textBox1.Text = "";
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text != "")
            {
                comboBox2.Enabled = true;
                comboBox2.Items.Clear();
                if (comboBox1.Text == "Forex")
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var symbols = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token="+apiKey+"&category=forex");

                        var jsymbols = JsonConvert.DeserializeObject<dynamic>(symbols);

                        for (int i= 0; i < jsymbols.response.Count; i++)
                            {
                            comboBox2.Items.Add(jsymbols.response[i].name);
                        };

                    }
                }
                if (comboBox1.Text == "Stock")
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var symbols = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token=" + apiKey + "&category=stock");

                        var jsymbols = JsonConvert.DeserializeObject<dynamic>(symbols);

                        for (int i = 0; i < jsymbols.response.Count; i++)
                        {
                            comboBox2.Items.Add(jsymbols.response[i].name);
                        };

                    }
                }
                if (comboBox1.Text == "Index")
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var symbols = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token=" + apiKey + "&category=index");

                        var jsymbols = JsonConvert.DeserializeObject<dynamic>(symbols);

                        for (int i = 0; i < jsymbols.response.Count; i++)
                        {
                            comboBox2.Items.Add(jsymbols.response[i].name);
                        };

                    }
                }
                if (comboBox1.Text == "Commodity")
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var symbols = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token=" + apiKey + "&category=COMMODITY");

                        var jsymbols = JsonConvert.DeserializeObject<dynamic>(symbols);

                        for (int i = 0; i < jsymbols.response.Count; i++)
                        {
                            comboBox2.Items.Add(jsymbols.response[i].name);
                        };

                    }
                }
            }
        }
        //NOT USED
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox1.Text == "-")
            {
                textBox1.Text = "";
            }
            using (var webClient = new System.Net.WebClient())
            {
                //USD=>BTC
                var btcKurs = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token=" + apiKey + "&symbols=BTCUSD");

                var jbtcKurs = JsonConvert.DeserializeObject<dynamic>(btcKurs);

                float btckurs = jbtcKurs.response[0].ask;
                if (textBox1.Text == "")
                {
                    float x = 0;
                    float v = x / btckurs;
                    //textBox2.Text = v.ToString();
                }
                else
                {
                    float x = float.Parse(textBox1.Text, CultureInfo.InvariantCulture);
                    float v = x / btckurs;
                    //textBox2.Text = v.ToString();
                }
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            //BTC=>USD
            if (textBox2.Text == "-")
            {
                textBox2.Text = "";
            }
            using (var webClient = new System.Net.WebClient())
            {
                var btcKurs = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token=" + apiKey + "&symbols=BTCUSD");

                var jbtcKurs = JsonConvert.DeserializeObject<dynamic>(btcKurs);

                float btckurs = jbtcKurs.response[0].ask;
                if (textBox2.Text == "")
                {
                    float x = 0;
                    float v = x * btckurs;

                    //textBox1.Text = v.ToString();
                }
                else
                {
                    float x = float.Parse(textBox2.Text, CultureInfo.InvariantCulture);
                    float v = x * btckurs;
                    //textBox1.Text = v.ToString();
                }
            }
        }
        //NOT USED end


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button3.Text = "CREATE LONG";
            button3.Enabled = true;
            button3.BackColor = Color.FromArgb(69, 191, 85);
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            button3.Text = "CREATE SHORT";
            button3.Enabled = true;
            button3.BackColor = Color.FromArgb(255, 61, 46);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text != "")
            {
                // Text
                string text = ((Control)sender).Text;

            // Is Negative Number?
            if (e.KeyChar == '-' && text.Length == 0)
            {
                e.Handled = false;
                return;
            }

            // Is Float Number?
            if (e.KeyChar == '.' && text.Length > 0 && !text.Contains("."))
            {
                e.Handled = false;
                return;
            }

            // Is Digit?
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox2.Text != "")
            { 
                // Text
                string text = ((Control)sender).Text;

            // Is Negative Number?
            if (e.KeyChar == '-' && text.Length == 0)
            {
                e.Handled = false;
                return;
            }

            // Is Float Number?
            if (e.KeyChar == '.' && text.Length > 0 && !text.Contains("."))
            {
                e.Handled = false;
                return;
            }

            // Is Digit?
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox3.Text != "")
            {   
            // Text
            string text = ((Control)sender).Text;

            // Is Negative Number?
            if (e.KeyChar == '-' && text.Length == 0)
            {
                e.Handled = false;
                return;
            }

            // Is Float Number?
            if (e.KeyChar == '.' && text.Length > 0 && !text.Contains("."))
            {
                e.Handled = false;
                return;
            }

            // Is Digit?
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
            } 
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox3.Text != "")
            {
                if (textBox3.Text == "-")
                {
                    textBox3.Text = "1";
                }
            }
            else
            {
                //textBox3.Text = "1";
            }
        }
        public static int categorySymbolNumber;

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "")
            {
                MessageBox.Show("Kein Symbol ausgewählt !");
            }
            else
            {
                if (textBox1.Text == "" || textBox1.Text == "0" || textBox1.Text == "0.00")
                {
                    MessageBox.Show("Keine Anzahl gegeben !");
                }
                else
                {
                    //VALIDATET
                    using (var webClient = new System.Net.WebClient())
                    {
                        string orderDirection;
                        string limitBuy;
                        string takeProfit;
                        string stopLoss;

                        var btcKurs = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token=" + apiKey + "&symbols=BTCUSD");

                        var jbtcKurs = JsonConvert.DeserializeObject<dynamic>(btcKurs);

                        float btckurs = jbtcKurs.response[0].ask;

                        float x = float.Parse(textBox1.Text, CultureInfo.InvariantCulture);
                        //float orderMargin = x / btckurs;
                        string orderMargin = textBox1.Text;

                        var category = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token=" + apiKey + "&category=" + comboBox1.Text);
                        var jcategory = JsonConvert.DeserializeObject<dynamic>(category);

                        if (radioButton1.Checked == true)
                        {
                            orderDirection = "long";
                        }
                        else
                        {
                            orderDirection = "short";
                        }
                        if (radioButton3.Checked == true)
                        {
                            limitBuy = "market";
                        }
                        else
                        {
                            limitBuy = "limit&order_type_parameter=" + textBox2.Text;
                        }

                        int textBox5Int = 0;
                        Int32.TryParse(textBox5.Text, out textBox5Int);
                        if (textBox5Int > 0)
                        {
                            takeProfit = "&take_profit=" + textBox5.Text;
                        }
                        else
                        {
                            takeProfit = "";
                        }
                        int textBox4Int = 0;
                        Int32.TryParse(textBox4.Text, out textBox4Int);
                        if (textBox4Int > 0)
                        {
                            stopLoss = "&stop_loss=" + textBox5.Text;
                        }
                        else
                        {
                            stopLoss = "";
                        }

                        var order = webClient.DownloadString("https://1broker.com/api/v2/order/create.php?token=" + apiKey + "&symbol=" + jcategory.response[categorySymbolNumber].symbol + "&margin=" + orderMargin + "&direction=" + orderDirection + "&leverage=" + textBox3.Text + "&order_type=" + limitBuy + "&referral_id=" + refID + takeProfit + stopLoss);
                            var jorder = JsonConvert.DeserializeObject<dynamic>(order);

                            if (jorder.error == true)
                            {
                                MessageBox.Show("Error: "+jorder.error_code);

                                setUserInfo();
                                label1.Text = cpuTimeLeftPercent + "%";
                                labelBalance.Text = balance + " BTC";

                                openPositions();
                                openOrders();
                            }
                            else
                            {
                                setUserInfo();
                                label1.Text = cpuTimeLeftPercent + "%";
                                labelBalance.Text = balance + " BTC";

                                openPositions();
                                openOrders();
                            }
   
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["apiKey"] = textBoxApiKey.Text;
            Properties.Settings.Default.Save();

            startApp();

            MessageBox.Show("Restart the program for the settings to take effect");
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                textBox2.Enabled = false;
            }
            else
            {
                textBox2.Enabled = true;
            }
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text != "")
            {
                using (var webClient = new System.Net.WebClient())
                {
                    var category = webClient.DownloadString("https://1broker.com/api/v2/market/list.php?token=" + apiKey + "&category=" + comboBox1.Text);

                    var jcategory = JsonConvert.DeserializeObject<dynamic>(category);

                    for (int i = 0; i < jcategory.response.Count; i++)
                    {
                        if (jcategory.response[i].name == comboBox2.Text)
                        {
                            categorySymbolNumber = i;

                            var symbolPrice = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token="+apiKey+"&symbols="+ jcategory.response[categorySymbolNumber].symbol);

                            var jsymbolPrice = JsonConvert.DeserializeObject<dynamic>(symbolPrice);

                            textBox2.Text = jsymbolPrice.response[0].ask;
                        }
                    }
                }
            }
        }

        public System.Timers.Timer aTimer;
        public void SetTimer()
        {
            aTimer = new System.Timers.Timer(2000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
            //aTimer.AutoReset = true;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            setUserInfo();

            label1.Invoke(new Action(() => label1.Text = cpuTimeLeftPercent + "%"));
            labelBalance.Invoke(new Action(() => labelBalance.Text = balance + " BTC"));

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Invoke(new Action(() => dataGridView1.Rows.RemoveAt(i)));
            }

            dataGridView1.DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = SystemColors.MenuBar;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AllowUserToResizeRows = false;

            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.RowHeadersVisible = false;

            using (var webClient = new System.Net.WebClient())
            {
                var openPos = webClient.DownloadString("https://1broker.com/api/v2/position/open.php?token=" + apiKey);

                var jopenPos = JsonConvert.DeserializeObject<dynamic>(openPos);

                if (jopenPos.response.Count < 1)
                {
                    dataGridView1.Invoke(new Action(() => dataGridView1.AllowUserToAddRows = false));
                }
                else
                {

                    for (int s = 0; s < jopenPos.response.Count; s++)
                    {
                        var openPosPrice = webClient.DownloadString("https://1broker.com/api/v2/market/quotes.php?token=" + apiKey + "&symbols=" + jopenPos.response[s].symbol);

                        var jopenPosPrice = JsonConvert.DeserializeObject<dynamic>(openPosPrice);

                        dataGridView1.Invoke(new Action(() => dataGridView1.AllowUserToAddRows = true));

                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[s].Clone();
                        row.Cells[0].Value = jopenPos.response[s].symbol.ToString();
                        row.Cells[1].Value = jopenPos.response[s].margin.ToString() + " BTC";
                        row.Cells[2].Value = jopenPos.response[s].leverage.ToString();
                        row.Cells[4].Value = jopenPos.response[s].profit_loss_percent.ToString() + "%";
                        row.Cells[3].Value = jopenPos.response[s].entry_price.ToString();
                        row.Cells[6].Value = jopenPos.response[s].stop_loss.ToString();
                        row.Cells[7].Value = jopenPos.response[s].take_profit.ToString();

                        if (jopenPos.response[s].direction.ToString().ToUpper() == "LONG")
                        {
                            row.Cells[5].Value = jopenPosPrice.response[0].bid;
                        }
                        else
                        {
                            row.Cells[5].Value = jopenPosPrice.response[0].ask;
                        }


                        row.Cells[8].Value = jopenPos.response[s].direction.ToString().ToUpper();
                        row.Cells[9].Value = "Close";

                        dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Add(row)));

                        if (s == (jopenPos.response.Count - 1))
                        {
                            dataGridView1.Invoke(new Action(() => dataGridView1.AllowUserToAddRows = false));
                        }
                    }
                }
            }


                for (int i=0;i < dataGridView2.Rows.Count; i++)
                {
                dataGridView2.Invoke(new Action(() => dataGridView2.Rows.RemoveAt(i)));
            }

                GC.Collect();

                dataGridView2.DefaultCellStyle.SelectionBackColor = SystemColors.MenuBar;
                dataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
                dataGridView2.DefaultCellStyle.BackColor = SystemColors.MenuBar;
                dataGridView2.DefaultCellStyle.ForeColor = Color.Black;

                dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGridView2.AllowUserToResizeRows = false;

                dataGridView2.AllowUserToResizeColumns = false;
                dataGridView2.RowHeadersVisible = false;

                using (var webClient = new System.Net.WebClient())
                {
                    var openOrd = webClient.DownloadString("https://1broker.com/api/v2/order/open.php?token=" + apiKey);

                    var jopenOrd = JsonConvert.DeserializeObject<dynamic>(openOrd);

                    if (jopenOrd.response.Count < 1)
                    {
                    dataGridView2.Invoke(new Action(() => dataGridView2.AllowUserToAddRows = false));
                }
                else
                {
                    for (int s = 0; s < jopenOrd.response.Count; s++)
                    {
                        dataGridView2.Invoke(new Action(() => dataGridView2.AllowUserToAddRows = true));

                        DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[s].Clone();
                        row.Cells[0].Value = jopenOrd.response[s].symbol.ToString();
                        row.Cells[1].Value = jopenOrd.response[s].margin.ToString() + " BTC";
                        row.Cells[2].Value = jopenOrd.response[s].leverage.ToString();
                        row.Cells[3].Value = jopenOrd.response[s].direction.ToString().ToUpper();
                        row.Cells[4].Value = "Cancel";


                        dataGridView2.Invoke(new Action(() => dataGridView2.Rows.Add(row)));

                        if (s == (jopenOrd.response.Count - 1))
                        {
                            dataGridView2.Invoke(new Action(() => dataGridView2.AllowUserToAddRows = false));
                            SetTimer();
                        }
                    }
                }
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentCell != null)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 4)
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var openOrd = webClient.DownloadString("https://1broker.com/api/v2/order/open.php?token=" + apiKey);

                        var jopenOrd = JsonConvert.DeserializeObject<dynamic>(openOrd);

                        var orderidClose = jopenOrd.response[dataGridView2.CurrentCell.RowIndex].order_id;

                        var closeOrd = webClient.DownloadString("https://1broker.com/api/v2/order/cancel.php?token=" + apiKey + "&order_id=" + orderidClose);

                        openPositions();
                        openOrders();

                        setUserInfo();
                        label1.Text = cpuTimeLeftPercent + "%";
                        labelBalance.Text = balance + " BTC";

                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                if (dataGridView1.CurrentCell.ColumnIndex == 9)
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        var openPos = webClient.DownloadString("https://1broker.com/api/v2/position/open.php?token=" + apiKey);

                        var jopenPos = JsonConvert.DeserializeObject<dynamic>(openPos);

                        var positionidClose = jopenPos.response[dataGridView1.CurrentCell.RowIndex].position_id;

                        var closeOrd = webClient.DownloadString("https://1broker.com/api/v2/position/close.php?token=" + apiKey + "&position_id=" + positionidClose);

                        openPositions();
                        openOrders();

                        setUserInfo();
                        label1.Text = cpuTimeLeftPercent + "%";
                        labelBalance.Text = balance + " BTC";

                    }
                }
            }
        }
    }
}
