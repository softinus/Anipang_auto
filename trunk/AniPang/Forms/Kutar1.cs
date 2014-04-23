using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Hooks;
using System.Diagnostics;

namespace AniPang
{
    public partial class frmKutar1 : Form
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd01, int hWnd02, string lpsz01, string lpsz02);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        private const int WM_LBUTTONDOWN = 0x0002;
        private const int WM_LBUTTONUP = 0x0004;
        private const int WM_LBUTTONDBLCLK = 0x203;

        int m_iTimeNumbering = 0;   // 현재 시간 [4/23/2014 Mark]
        int m_iTimeLastTouch = 0;   // 마지막 터치한 시간. [4/23/2014 Mark]
        bool m_bTimerStart = false;
        
        IntPtr m_hWndMain = new IntPtr();
        IntPtr m_hWndCanvas = new IntPtr();
        RECT m_rcLocation = new RECT();
        RECT m_rcSize = new RECT();

        Point m_ptStartDot = new Point(0, 0);
        //Point m_ptLocation = new Point(17, 165);

        Bitmap bmp;
        KutarStat m_pMainData;  // 쿠타 상태
        bool m_bStart = false;
        //int[] m_pLimit = new int[4];

        public frmKutar1()
        {
            InitializeComponent();
        }

        private void frmKutar1_Load(object sender, EventArgs e)
        {
            this.HookedKeyboardNofity += new KeyboardHooker.HookedKeyboardUserEventHandler(CP_HookedKeyboardNofity);
            KeyboardHooker.Hook(HookedKeyboardNofity);
        }

        event KeyboardHooker.HookedKeyboardUserEventHandler HookedKeyboardNofity;
        private long CP_HookedKeyboardNofity(bool bIsKeyDown, bool bAlt, bool bCtrl, bool bShift, bool bWindowKey, int vkCode)
        {
            long lResult = 0;

            if (vkCode == (int)System.Windows.Forms.Keys.F1)
            {
                KutarLoad();
                //MessageBox.Show("F1");
            }

            if (vkCode == (int)System.Windows.Forms.Keys.F2)
            {
                //if (!m_bTimerStart)
                {
                    //MessageBox.Show("F2");
                    timer1.Start();
                    SetCursorPos(m_ptStartDot.X + 1, m_ptStartDot.Y + 1);
                    m_bTimerStart = true;
                }
            }
            if (vkCode == (int)System.Windows.Forms.Keys.F3)
            {
                //if (m_bTimerStart)
                {
                    //MessageBox.Show("F3");
                    timer1.Stop();
                    m_bTimerStart = false;
                }
            }

            return lResult;
        }
        
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            m_iTimeNumbering += timer1.Interval;

            GetScreen();

            Point ptStart = new Point(0, 0);
            AutoMouse(ptStart);
        }


        // 쿠타 화면을 로드하고 [4/23/2014 Mark]
        private void KutarLoad()
        {
            m_hWndMain = FindWindow(null, "BlueStacks App Player for Windows (beta-1)");
            m_hWndCanvas = FindWindowEx(m_hWndMain, 0, "BlueStacksApp", "_ctl.Window");

            GetWindowRect(m_hWndCanvas, ref m_rcLocation); // Top, Left
            GetClientRect(m_hWndCanvas, ref m_rcSize); // Bottom, Right

            timer1.Interval = Int32.Parse(txt_interval.Text);

            //m_ptStartDot = new Point((m_rcLocation.left + 356 - 20), (m_rcLocation.top + 45 - 38));
            m_ptStartDot = new Point((m_rcLocation.left), (m_rcLocation.top));
        }

        // screen capture 하고 [4/23/2014 Mark]
        private void GetScreen()
        {
            // blue stack 해상도가 w:400, h:320 일 때 기준. [4/23/2014 Mark]
            //Size sz = new Size(400, 320);
            //Bitmap bt = new Bitmap(400, 320);
            Size sz = new Size(60, 30);
            Bitmap bt = new Bitmap(60, 30);

            //m_pLimit[0] = m_ptStartDot.Y; m_pLimit[1] = (m_ptStartDot.Y + 600);
            //m_pLimit[2] = m_ptStartDot.X; m_pLimit[3] = (m_ptStartDot.X + 350);

            Graphics g = Graphics.FromImage(bt);

            g.CopyFromScreen(m_ptStartDot.X+240, m_ptStartDot.Y+200, 0, 0, sz);

            MemoryStream ms = new MemoryStream();
            ms.Position = 0;

            // ms버퍼에 그림을 넣는다.
            bt.Save(ms, ImageFormat.Jpeg);

            Image img = Image.FromStream(ms);
            bmp = new Bitmap(img);
            ResultData(bmp, ref m_pMainData);
            bmp.Save("Test" + m_iTimeNumbering + ".bmp", ImageFormat.Bmp);
            //bmp.Save("Test" + "000" + ".bmp", ImageFormat.Bmp);
            m_bStart = true;
            this.panelCenter.Invalidate();
        }

        // 이미지 픽셀에 따라 상태를 판단하고, [4/23/2014 Mark]
        private void ResultData(Bitmap bmp, ref KutarStat pMainData)
        {
            pMainData = new KutarStat();
            pMainData = KutarStat.UP;

            // blue stack 해상도가 w:400, h:320 일 때 기준. [4/23/2014 Mark]
            //Color crPixel = bmp.GetPixel(264, 218);
            Color crPixel1 = bmp.GetPixel(20, 10);
            Color crPixel2 = bmp.GetPixel(25, 20);
            Color crPixel3 = bmp.GetPixel(30, 15); 

            //Debug.WriteLine("R: " + crPixel1.R + "G: " + crPixel1.G + "B: " + crPixel1.B);

            // 올라가서도 거품 없어질 때까지 시간이 좀 있다. [4/23/2014 Mark]
            if ((m_iTimeNumbering - m_iTimeLastTouch) > 450)
            {
                //if (crPixel.R == 255 && crPixel.G == 255 && crPixel.B == 255)
                if (crPixel1.R > 230 && crPixel1.G > 230 && crPixel1.B > 230)
                {
                    if (crPixel2.R > 230 && crPixel2.G > 230 && crPixel2.B > 230)
                    {
                        if (crPixel3.R > 230 && crPixel3.G > 230 && crPixel3.B > 230)
                        {
                            lstLog.Items.Add("거품으로 판단됨!");
                            pMainData = KutarStat.DOWN;
                        }
                    }                    
                }
            }
            else
            {
                lstLog.Items.Add("대기 중..");
            }
            
        }

        // 마우스를 인풋해준다. [4/23/2014 Mark]
        public void AutoMouse(Point ptStart)
        {
            //Point ptCurrent = new Point(0, 0);
            //GetCursorPos(ref ptCurrent);

            if (m_pMainData == KutarStat.DOWN)
            {
                SetCursorPos(m_ptStartDot.X + 200, m_ptStartDot.Y + 150);
                mouse_event(WM_LBUTTONDOWN, 0, 0, 0, 0);
                mouse_event(WM_LBUTTONUP, 0, 0, 0, 0);
                m_iTimeLastTouch = m_iTimeNumbering;    // 마지막으로 터치한 시각 저장 [4/23/2014 Mark]
                lstLog.Items.Add("=====터치!=====");
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }
            else if (m_pMainData == KutarStat.UP)
            {
                lstLog.Items.Add("아직 올라가 있음!");
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
             KutarLoad();                    
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
            SetCursorPos(m_ptStartDot.X + 1, m_ptStartDot.Y + 1);
            m_bTimerStart = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_bStart = false;
            timer1.Stop();
        }

        private void panelCenter_Paint(object sender, PaintEventArgs e)
        {
            if (m_bStart)
            {
                Graphics g = e.Graphics;

                Font font = new Font("나눔고딕코딩", 11);
                SolidBrush Brush_font = new SolidBrush(Color.FromArgb(255, 0, 255));

                //Color crPixel = bmp.GetPixel(264, 218); 
                Color crPixel = bmp.GetPixel(30, 15); 
                String strColor = "R: " + crPixel.R.ToString() + "\nG: " + crPixel.G.ToString() + "\nB: " + crPixel.B.ToString();
                String strStatus = m_pMainData.ToString();

                g.DrawString(strColor, font, Brush_font, 0, 0);
                g.DrawString(strStatus, font, Brush_font, 0, 150);
            }
        }



    }
}



