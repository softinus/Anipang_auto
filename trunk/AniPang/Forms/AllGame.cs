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
    public partial class frmAllGame : Form
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

        public frmAllGame()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorPaperLoad();
        }

        IntPtr m_hWndMain = new IntPtr();
        IntPtr m_hWndCanvas = new IntPtr();
        RECT m_rcLocation = new RECT();
        RECT m_rcSize = new RECT();

        Point m_ptStartDot = new Point(0, 0);
        Point m_ptLocation = new Point(17, 165);

        Bitmap bmp;
        ColorPapers m_pMainData;
        bool m_bStart = false;
        int[] m_pLimit = new int[4];


        private void ColorPaperLoad()
        {
            m_hWndMain = FindWindow(null, "BlueStacks App Player for Windows (beta-1)");
            m_hWndCanvas = FindWindowEx(m_hWndMain, 0, "BlueStacksApp", "_ctl.Window");

            GetWindowRect(m_hWndCanvas, ref m_rcLocation); // Top, Left
            GetClientRect(m_hWndCanvas, ref m_rcSize); // Bottom, Right

            //m_ptStartDot = new Point((m_rcLocation.left + 356 - 20), (m_rcLocation.top + 45 - 38));
            m_ptStartDot = new Point((m_rcLocation.left), (m_rcLocation.top));
        }

        
        private void AllGame_Load(object sender, EventArgs e)
        {
            this.HookedKeyboardNofity += new KeyboardHooker.HookedKeyboardUserEventHandler(CP_HookedKeyboardNofity);
            KeyboardHooker.Hook(HookedKeyboardNofity);

            //txt_interval.Text = timer1.Interval.ToString();
        }

        event KeyboardHooker.HookedKeyboardUserEventHandler HookedKeyboardNofity;
        private long CP_HookedKeyboardNofity(bool bIsKeyDown, bool bAlt, bool bCtrl, bool bShift, bool bWindowKey, int vkCode)
        {
            long lResult = 0;

            if (vkCode == (int)System.Windows.Forms.Keys.F1)
            {
                ColorPaperLoad();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetScreen();

            Point ptStart = new Point(0, 0);
            Point ptEnd = new Point(0, 0);

            AutoMouse(ptStart, ptEnd);
        }


        int m_iTimeStop = 0;
        bool m_bTimerStart = false;

        public void AutoMouse(Point ptStart, Point ptEnd)
        {
            Point ptCurrent = new Point(0, 0);
            GetCursorPos(ref ptCurrent);

            //if ((ptCurrent.X < m_pLimit[2] - 200) || (ptCurrent.X > m_pLimit[3] + 200) || (ptCurrent.Y < m_pLimit[0] - 200) || (ptCurrent.Y > m_pLimit[1] + 200))
            //{
            //    button3.Enabled = true;
            //    timer1.Stop();
            //}

            SetCursorPos(m_ptStartDot.X + 210, m_ptStartDot.Y + 360);
            mouse_event(WM_LBUTTONDOWN, 0, 0, 0, 0);

            switch (m_pMainData)
            {
                case ColorPapers.RED:
                    SetCursorPos(m_ptStartDot.X + 300, m_ptStartDot.Y+330);
                    break;
                case ColorPapers.BLUE:
                    SetCursorPos(m_ptStartDot.X + 50, m_ptStartDot.Y+330);
                    break;
                case ColorPapers.YELLOW:
                    SetCursorPos(m_ptStartDot.X+180, m_ptStartDot.Y + 460);
                    break;
                case ColorPapers.GREEN:
                    SetCursorPos(m_ptStartDot.X + 180, m_ptStartDot.Y+200);
                    break;
            }           

            mouse_event(WM_LBUTTONUP, 0, 0, 0, 0);
        }

        private void GetScreen()
        {
            Size sz = new Size(350, 600);
            Bitmap bt = new Bitmap(350, 600);

            m_pLimit[0] = m_ptStartDot.Y; m_pLimit[1] = (m_ptStartDot.Y + 600);
            m_pLimit[2] = m_ptStartDot.X; m_pLimit[3] = (m_ptStartDot.X + 350);

            Graphics g = Graphics.FromImage(bt);

            g.CopyFromScreen(m_ptStartDot.X, m_ptStartDot.Y, 0, 0, sz);

            MemoryStream ms = new MemoryStream();
            ms.Position = 0;

            // ms버퍼에 그림을 넣는다.
            bt.Save(ms, ImageFormat.Jpeg);

            Image img = Image.FromStream(ms);
            bmp = new Bitmap(img);
            ResultData(bmp, ref m_pMainData);
            bmp.Save("Test" + m_iTimeStop + ".bmp", ImageFormat.Bmp);
            m_bStart = true;
            this.panelCenter.Invalidate();
        }

        private void ResultData(Bitmap bmp, ref ColorPapers pMainData )
        {
            pMainData = new ColorPapers();
            pMainData = ColorPapers.UNKNOWN;

            Color crPixel= bmp.GetPixel(210,360);
            // 각 x:210, y:360 일 때의 RGB 값을 이용한다.
            // (RED)    -> 222, 69, 64
            // (BLUE)   -> 0, 184, 245
            // (YELLOW) -> 251,229,55
            // (GREEN)  -> 151,233,35

            Debug.WriteLine("R: " + crPixel.R + "G: " + crPixel.G + "B: " + crPixel.B);

            //if ((crPixel.R == 238) && (crPixel.G == 67) && (crPixel.B == 73))
            if (((crPixel.R <= 238) && (crPixel.R >= 227)) && ((crPixel.G <= 72) && (crPixel.G >= 67)) && ((crPixel.B <= 76) && (crPixel.B >= 69)))
            {
                pMainData = ColorPapers.RED;
            }
            else if (((crPixel.R <= 5) && (crPixel.R >= 0)) && ((crPixel.G <= 187) && (crPixel.G >= 180)) && ((crPixel.B <= 255) && (crPixel.B >= 250)))
            {
                pMainData = ColorPapers.BLUE;
            }
            else if (((crPixel.R <= 255) && (crPixel.R >= 240)) && ((crPixel.G <= 242) && (crPixel.G >= 230)) && ((crPixel.B <= 72) && (crPixel.B >= 55)))
            {
                pMainData = ColorPapers.YELLOW;
            }
            else if (((crPixel.R <= 164) && (crPixel.R >= 148)) && ((crPixel.G <= 238) && (crPixel.G >= 231)) && ((crPixel.B <= 51) && (crPixel.B >= 39)))
            {
                pMainData = ColorPapers.GREEN;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!m_bTimerStart)
            {
                timer1.Start();
                SetCursorPos(m_ptStartDot.X + 1, m_ptStartDot.Y + 1);
                m_bTimerStart = true;
            }
            else
            {
                timer1.Stop();
                m_bTimerStart = false;
            }
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
            // (RED)    -> 222, 69, 64
            // (BLUE)   -> 0, 184, 245
            // (YELLOW) -> 251,229,55
            // (GREEN)  -> 151,233,35

                Graphics g = e.Graphics;
                SolidBrush Brush1 = new SolidBrush(Color.FromArgb(222,69,64));
                SolidBrush Brush2 = new SolidBrush(Color.FromArgb(0,184,245));
                SolidBrush Brush3 = new SolidBrush(Color.FromArgb(251,229,55));
                SolidBrush Brush4 = new SolidBrush(Color.FromArgb(151,233,35));

                Font font = new Font("나눔고딕코딩", 11);
                SolidBrush Brush_font = new SolidBrush(Color.FromArgb(255, 0, 255));

                Color crPixel = bmp.GetPixel(210,360);
                String strColor = "R: " + crPixel.R.ToString() + "\nG: " + crPixel.G.ToString() + "\nB: " + crPixel.B.ToString();

                g.DrawString(strColor, font, Brush_font, 0, 0);

                switch(m_pMainData)
                {
                    case ColorPapers.RED:
                        g.FillRectangle(Brush1, new Rectangle(0,0,200,200));
                        g.DrawString("RED!", font, Brush_font, 50, 0);
                        break;
                    case ColorPapers.BLUE:
                        g.FillRectangle(Brush2, new Rectangle(0,0,200,200));
                        g.DrawString("BLUE!", font, Brush_font, 50, 0);
                        break;
                    case ColorPapers.YELLOW:
                        g.FillRectangle(Brush3, new Rectangle(0,0,200,200));
                        g.DrawString("YELLOW!", font, Brush_font, 50, 0);
                        break;
                    case ColorPapers.GREEN:
                        g.FillRectangle(Brush4, new Rectangle(0, 0, 200, 200));
                        g.DrawString("GREEN!", font, Brush_font, 50, 0);
                        break;
                    default:
                        g.DrawString("UNKNOWN!", font, Brush_font, 50, 0);
                        break;
                }
                
            }
        }
    }
}


