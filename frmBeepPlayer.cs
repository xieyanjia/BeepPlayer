using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

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

            // 播放旋律前，先鎖住單音按鈕
            SetNoteButtonsEnabled(false);

            // 播放期間先停用按鈕，避免使用者一直重複按
            btnPlayMelody.Enabled = false;
            cmbMelody.Enabled = false;

            try
            {
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
            }
            finally
            {
                // 播放完成後，不管有沒有錯誤，都恢復按鈕
                SetNoteButtonsEnabled(true);

                btnPlayMelody.Enabled = true;
                cmbMelody.Enabled = true;
            }
        }

        private void PlayHappyBirthday()
        {
            // 生日快樂歌簡化版
            int[] melody =
            {
        262, 262, 294, 262, 349, 330,
        262, 262, 294, 262, 392, 349,
        262, 262, 523, 440, 349, 330, 294,
        466, 466, 440, 349, 392, 349
    };

            int[] duration =
            {
        300, 300, 600, 600, 600, 900,
        300, 300, 600, 600, 600, 900,
        300, 300, 600, 600, 600, 600, 900,
        300, 300, 600, 600, 600, 900
    };

            for (int i = 0; i < melody.Length; i++)
            {
                Beep(melody[i], duration[i]);
                Thread.Sleep(80);
            }
        }

        private void PlayMario()
        {
            // 超級瑪利歐主題曲簡化版
            // 0 代表休止符，不發出聲音

            int[] melody =
            {
        659, 659, 0, 659,
        0, 523, 659, 0,
        784, 0, 392, 0,

        523, 0, 392, 0, 330, 0,
        440, 0, 494, 0, 466, 440,
        392, 659, 784, 880,
        698, 784, 0, 659,
        523, 587, 494, 0,

        523, 0, 392, 0, 330, 0,
        440, 0, 494, 0, 466, 440,
        392, 659, 784, 880,
        698, 784, 0, 659,
        523, 587, 494, 0,

        0, 784, 740, 698,
        622, 659, 0, 415,
        440, 523, 0, 440,
        523, 587, 0,

        0, 784, 740, 698,
        622, 659, 0, 1046,
        1046, 1046, 0
    };

            int[] duration =
            {
        150, 150, 150, 150,
        150, 150, 150, 150,
        300, 300, 300, 300,

        150, 150, 150, 150, 150, 150,
        150, 150, 150, 150, 150, 150,
        150, 150, 150, 150,
        150, 150, 150, 150,
        150, 150, 300, 150,

        150, 150, 150, 150, 150, 150,
        150, 150, 150, 150, 150, 150,
        150, 150, 150, 150,
        150, 150, 150, 150,
        150, 150, 300, 150,

        150, 150, 150, 150,
        150, 150, 150, 150,
        150, 150, 150, 150,
        150, 300, 150,

        150, 150, 150, 150,
        150, 150, 150, 150,
        150, 300, 300
    };

            for (int i = 0; i < melody.Length; i++)
            {
                if (melody[i] == 0)
                {
                    Thread.Sleep(duration[i]);
                }
                else
                {
                    Beep(melody[i], duration[i]);
                }

                Thread.Sleep(40);
            }
        }

        private void SetNoteButtonsEnabled(bool enabled)
        {
            btn1.Enabled = enabled;
            btn2.Enabled = enabled;
            btn3.Enabled = enabled;
            btn4.Enabled = enabled;
            btn5.Enabled = enabled;
            btn6.Enabled = enabled;
            btn7.Enabled = enabled;
            btn8.Enabled = enabled;
        }
    }
}
