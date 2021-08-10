using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ZedGraph;
using System.Drawing;

namespace demo
{
    public partial class Form1 : Form
    {
        SerialPort serialPort = new SerialPort();

        GraphPane myPane = new GraphPane();

        // poing pair lists
        PointPairList listPointsOne = new PointPairList();

        // line item
        LineItem myCurveOne;

        int time = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeZedchart();
        }

        private void InitializeZedchart()
        {
            myPane = zedGraphControl1.GraphPane;

            // set a title
            myPane.Title.Text = "Nhiệt độ theo thời gian";

            // set X and Y axis titles
            myPane.XAxis.Title.Text = "Time, Seconds";
            myPane.YAxis.Title.Text = "Angle, Deg";

            // set lineitem to list of points
            myCurveOne = myPane.AddCurve("Nhiệt độ", listPointsOne, Color.Red, SymbolType.Circle);
            // ---------------------

            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;

            myCurveOne.Line.Width = 3;
        }

        public async Task Sendata(string Temp, string url = "https://localhost:44312")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url + "/Home/SetTemp?Temperature=" + Temp);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            catch
            {

            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.BaudRate = 9600;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    serialPort.Parity = Parity.None;
                    serialPort.PortName = comboBox1.Text;
                    serialPort.Open();
                    serialPort.DataReceived += serialPort_dataReceived;
                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                }
            }
            catch { }

        }

        private async void serialPort_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(1000);
            string txt = "";
            try
            {
                txt = serialPort.ReadExisting();
                if (txt != "")
                {
                    await Sendata(txt.Substring(0, txt.IndexOf("\n")));
                }
            }
            catch
            {
            }
            finally
            {
                MethodInvoker m = () =>
                {
                    double temp = 0;
                    temp = Convert.ToDouble(txt.Substring(0, txt.IndexOf("\n")));
                    listPointsOne.Add(new PointPair(time, temp));
                    time++;
                    // draw
                    myPane.XAxis.Scale.Max = time + 5;
                    myPane.AxisChange();
                    zedGraphControl1.Refresh();
                    rtLog.Text = txt + rtLog.Text;
                };
                if (InvokeRequired)
                {
                    BeginInvoke(m); // I need to pass txt string in some way here.
                }
                else
                {
                    Invoke(m); // I need to pass txt string in some way here.
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.DataReceived -= serialPort_dataReceived;
                serialPort.Close();
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
        }

        private void txtCOM_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] port = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(port);
        }

        private void rtLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
