using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BeepPlayer
{   
    public partial class frmBeepPlayer : Form
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);

        // 設定音符對應的頻率( C4, D4, E4, F4, G4, A4, B4, C5 )
        int[] freq = { 523, 587, 659, 698, 784, 880, 988, 1046 };

        int initWidth = 0;
        int initHeight = 0;

        Dictionary<string, Rect> initControl = new Dictionary<string, Rect>();


        public frmBeepPlayer()
        {
            InitializeComponent();
            InitializeButton();

            InitializeMelody();
            btnPlayMelody.Click += btnPlayMelody_Click;
        }

        private void InitializeButton()
        {
            // 讓btn1~btn8共用同一個事件處理函式
            btn2.Click += btn1_Click;
            btn3.Click += btn1_Click;
            btn4.Click += btn1_Click;
            btn5.Click += btn1_Click;
            btn6.Click += btn1_Click;
            btn7.Click += btn1_Click;
            btn8.Click += btn1_Click;
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            Beep(freq[btn.TabIndex], 300);
            btn.Enabled = true;
        }

        private void frmBeepPlayer_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("歡迎使用 Beep Player！\n\n請按下按鈕來播放對應的音符。");

            // 儲存初始的視窗大小
            this.initWidth = this.palMain.Width;
            this.initHeight = this.palMain.Height;

            // 儲存初始的控制項位置和大小
            foreach (Control ctl in this.palMain.Controls)
            {
                this.initControl.Add(ctl.Name, new Rect(ctl.Left, ctl.Top,
                ctl.Width, ctl.Height));
            }
        }


        /// <summary>
        /// 當視窗大小改變時，根據新的視窗大小調整控制項的位置和大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBeepPlayer_SizeChanged(object sender, EventArgs e)
        {
            // 紀錄目前視窗的大小
            double width = this.palMain.Width;
            double height = this.palMain.Height;

            // 計算寬度和高度的縮放比例
            double iRatioWith = width / this.initWidth;
            double iRatioHeight = height / this.initHeight;

            // 根據縮放比例調整控制項的位置和大小
            foreach (Control ctl in this.palMain.Controls)
            {
                ctl.Left = (int)(initControl[ctl.Name].Left * iRatioWith);
                ctl.Top = (int)(initControl[ctl.Name].Top * iRatioHeight);
                ctl.Width = (int)(initControl[ctl.Name].Width * iRatioWith);
                ctl.Height = (int)(initControl[ctl.Name].Height *
                iRatioHeight);
            }
        }

        private void frmBeepPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("確定要關閉應用程式嗎？", "關閉確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // 取消關閉
            }
        }

        private void InitializeMelody()
        {
            // 清空下拉式選單，避免重複加入
            cmbMelody.Items.Clear();

            // 加入內建旋律
            cmbMelody.Items.Add("生日快樂歌");
            cmbMelody.Items.Add("超級瑪利歐");

            // 預設選第一首
            cmbMelody.SelectedIndex = 0;

            // 限制使用者只能從選單選，不讓使用者自己打字
            cmbMelody.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private async void btnPlayMelody_Click(object sender, EventArgs e)
        {
            string melody = cmbMelody.SelectedItem.ToString();

            // 播放期間先停用按鈕，避免使用者一直重複按
            btnPlayMelody.Enabled = false;
            cmbMelody.Enabled = false;

            await Task.Run(() =>
            {
                if (melody == "生日快樂歌")
                {
                    PlayHappyBirthday();
                }
                else if (melody == "超級瑪利歐")
                {
                    PlayMario();
                }
            });

            // 播放結束後恢復按鈕
            btnPlayMelody.Enabled = true;
            cmbMelody.Enabled = true;
        }

 
    }
}
