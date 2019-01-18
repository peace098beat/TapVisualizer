using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TapVisualizer;

namespace TempChartTest
{
    public partial class Form1 : Form
    {
        private int BufferLength;
        private RecodingBuffer recbuffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Seriesの作成
            Series series = new Series("wave");

            //グラフのタイプを指定(今回は線)
            series.ChartType = SeriesChartType.Line;

            //作ったSeriesをchartコントロールに追加する
            chart1.Series.Add(series);

            BufferLength = 1024;

            recbuffer = new TapVisualizer.RecodingBuffer(BufferLength, 44100, 16, 1);

            recbuffer.Start(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            recbuffer.Stop();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            Series series = chart1.Series["wave"];

            series.Points.Clear();

            float[] b = recbuffer.Buffer;

            //グラフのデータを追加
            for (int i = 0; i < BufferLength; i++)
            {
                series.Points.AddXY(i, b[i]);
            }

            chart1.ChartAreas[0].AxisY.Maximum = 1;
            chart1.ChartAreas[0].AxisY.Minimum= -1;
        }
    }
}
