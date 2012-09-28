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

namespace AniPang
{
    public partial class Form1 : Form
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

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AniPangLoad();
        }

        IntPtr m_hWndMain = new IntPtr();
        IntPtr m_hWndCanvas = new IntPtr();
        RECT m_rcLocation = new RECT();
        RECT m_rcSize = new RECT();

        Point m_ptStartDot = new Point(0, 0);
        Point m_ptLocation = new Point(17, 165);

        int[,] m_pMainData = null;
        bool m_bStart = false;
        int[] m_pLimit = new int[4];

        private void AniPangLoad()
        {
            m_hWndMain = FindWindow(null, "BlueStacks App Player for Windows (beta-1)");
            m_hWndCanvas = FindWindowEx(m_hWndMain, 0, "BlueStacksApp", "_ctl.Window");

            GetWindowRect(m_hWndCanvas, ref m_rcLocation); // Top, Left
            GetClientRect(m_hWndCanvas, ref m_rcSize); // Bottom, Right

            m_ptStartDot = new Point((m_rcLocation.left + 356 - 20), (m_rcLocation.top + 45 - 38));
        }

        public void AutoMouse(Point ptStart, Point ptEnd)
        {
            Point ptCurrent = new Point(0, 0);
            GetCursorPos(ref ptCurrent);

            if ((ptCurrent.X < m_pLimit[2] - 200) || (ptCurrent.X > m_pLimit[3] + 200) || (ptCurrent.Y < m_pLimit[0] - 200) || (ptCurrent.Y > m_pLimit[1] + 200))
            {
                button3.Enabled = true;
                timer1.Stop();
            }

            int iX = m_ptStartDot.X + (m_ptLocation.X + ((ptStart.Y) * 50));
            int iY = m_ptStartDot.Y + (m_ptLocation.Y + ((ptStart.X) * 50));

            SetCursorPos(iX, iY);
            mouse_event(WM_LBUTTONDOWN, 0, 0, 0, 0);
            
            iX = m_ptStartDot.X + (m_ptLocation.X + ((ptEnd.Y) * 50));
            iY = m_ptStartDot.Y + (m_ptLocation.Y + ((ptEnd.X) * 50));

            SetCursorPos(iX, iY);

            mouse_event(WM_LBUTTONUP, 0, 0, 0, 0);
        }

        private void GetScreen()
        {
            Size sz = new Size(331, 565);
            Bitmap bt = new Bitmap(331, 565);

            m_pLimit[0] = m_ptStartDot.Y; m_pLimit[1] = (m_ptStartDot.Y + 565);
            m_pLimit[2] = m_ptStartDot.X; m_pLimit[3] = (m_ptStartDot.X + 331);

            Graphics g = Graphics.FromImage(bt);

            g.CopyFromScreen(m_ptStartDot.X, m_ptStartDot.Y, 0, 0, sz);

            MemoryStream ms = new MemoryStream();
            ms.Position = 0;

            // ms버퍼에 그림을 넣는다.
            bt.Save(ms, ImageFormat.Jpeg);

            Image img = Image.FromStream(ms);
            Bitmap bmp = new Bitmap(img);
            ResultData(bmp, ref m_pMainData);
            bmp.Save("Test" + m_iTimeStop + ".bmp", ImageFormat.Bmp);
            m_bStart = true;
            this.panelCenter.Invalidate();
        }

        private void ResultData(Bitmap bmp, ref int[,] pMainData)
        {
            pMainData = new int[11, 11];//1토끼 2괭이 3때지 4곰탱 5원슝 6쥐색 7병알 -1뭐냐

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    pMainData[i, j] = -444;
                }
            }

            for (int i = 2; i < 9; i++)
            {
                for (int j = 2; j < 9; j++)
                {
                    Color crPixel = bmp.GetPixel(m_ptLocation.X + ((i - 2) * 50), m_ptLocation.Y + ((j - 2) * 50));

                    if (((crPixel.R <= 255) && (crPixel.R >= 210)) && ((crPixel.G <= 255) && (crPixel.G >= 210)) && ((crPixel.B <= 255) && (crPixel.B >= 210)))
                    {
                        pMainData[j, i] = 1;//토끼완료
                    }
                    else if (((crPixel.R <= 190) && (crPixel.R >= 150)) && ((crPixel.G <= 190) && (crPixel.G >= 160)) && ((crPixel.B <= 190) && (crPixel.B >= 150)))
                    {
                        pMainData[j, i] = 2;//고양이완료
                    }
                    else if (((crPixel.R <= 255) && (crPixel.R >= 230)) && ((crPixel.G <= 180) && (crPixel.G >= 150)) && ((crPixel.B <= 180) && (crPixel.B >= 150)))
                    {
                        pMainData[j, i] = 3; //돼지완료
                    }
                    else if (((crPixel.R <= 80) && (crPixel.R >= 0)) && ((crPixel.G <= 250) && (crPixel.G >= 190)) && ((crPixel.B <= 250) && (crPixel.B >= 190)))
                    {
                        pMainData[j, i] = 4; //곰완료
                    }
                    else if (((crPixel.R <= 255) && (crPixel.R >= 230)) && ((crPixel.G <= 230) && (crPixel.G >= 190)) && ((crPixel.B <= 90) && (crPixel.B >= 65)))
                    {
                        pMainData[j, i] = 7; //병아리 했고,
                    }
                    else if (((crPixel.R <= 255) && (crPixel.R >= 220)) && ((crPixel.G <= 230) && (crPixel.G >= 180)) && ((crPixel.B <= 180) && (crPixel.B >= 140)))
                    {
                        pMainData[j, i] = 5; //원숭이
                    }
                    else if (((crPixel.R <= 230) && (crPixel.R >= 130)) && ((crPixel.G <= 250) && (crPixel.G >= 140)) && ((crPixel.B <= 150) && (crPixel.B >= 20)))
                    {
                        pMainData[j, i] = 6; //쥐
                    }
                    else if (((crPixel.R <= 80) && (crPixel.R >= 0)) && ((crPixel.G <= 80) && (crPixel.G >= 0)) && ((crPixel.B <= 80) && (crPixel.B >= 0)))
                    {
                        pMainData[j, i] = -1;
                    }
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetScreen();

            Point ptStart = new Point(0, 0);
            Point ptEnd = new Point(0, 0);

            bool bStart = PangData(ref ptStart, ref ptEnd);

            if(bStart) AutoMouse(ptStart, ptEnd);

        }

        int m_iTimeStop = 0;
        bool m_bTimerStart = false;
        private bool PangData(ref Point ptStart, ref Point ptEnd)
        {
            for (int i = 2; i < 9; i++)
            {
                for (int j = 2; j < 9; j++)
                {
                    int iCurrentAni = m_pMainData[i, j];

                    Point[] pIndex = new Point[8];
                    Point ptCenter = new Point();

                    pIndex[0] = new Point(j - 1, i - 1);
                    pIndex[1] = new Point(j, i - 1);
                    pIndex[2] = new Point(j + 1, i - 1);

                    pIndex[3] = new Point(j - 1, i);

                    ptCenter = new Point(j, i);

                    pIndex[4] = new Point(j + 1, i);

                    pIndex[5] = new Point(j - 1, i + 1);
                    pIndex[6] = new Point(j, i + 1);
                    pIndex[7] = new Point(j + 1, i + 1);

                    int iIndexCount = pIndex.Count();

                    for (int x = 0; x < iIndexCount; x++)
                    {
                        if (m_pMainData[ptCenter.X, ptCenter.Y] == m_pMainData[pIndex[x].X, pIndex[x].Y])
                        {
                            int iIndex = x;

                            switch (iIndex)
                            {
                                case 0:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444) && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 1] != -444) 
                                            && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y, ptCenter.X - 1, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] != -444) 
                                            && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 1, ptCenter.Y - 1, ptCenter.X, ptCenter.Y - 1);
                                            return true;
                                        }
                                    }
                                    break;
                                case 1:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                        && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 1, ptCenter.Y - 2, ptCenter.X, ptCenter.Y - 2);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 2] != -444)
                                            && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 2] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y - 3, ptCenter.X, ptCenter.Y - 2);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X  + 1, ptCenter.Y - 2, ptCenter.X, ptCenter.Y - 2);
                                            return true;
                                        }
                                    }
                                    break;
                                case 2:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                        && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y, ptCenter.X + 1, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 1, ptCenter.Y - 1, ptCenter.X, ptCenter.Y - 1);
                                            return true;
                                        }
                                    }
                                    break;
                                case 3:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                        && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 2, ptCenter.Y - 1, ptCenter.X - 2, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X - 2, pIndex[iIndex].Y] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 2, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 3, ptCenter.Y, ptCenter.X - 2, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 2, ptCenter.Y + 1, ptCenter.X - 2, ptCenter.Y);
                                            return true;
                                        }
                                    }
                                    break;
                                case 4:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                        && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y - 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 2, ptCenter.Y - 1, ptCenter.X + 2, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 2, pIndex[iIndex].Y] != -444)
                                             && (m_pMainData[pIndex[iIndex].X + 2, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 3, ptCenter.Y, ptCenter.X + 2, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 2, ptCenter.Y + 1, ptCenter.X + 2, ptCenter.Y);
                                            return true;
                                        }
                                    }
                                    break;
                                case 5:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                         && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y, ptCenter.X - 1, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 1, ptCenter.Y + 1, ptCenter.X, ptCenter.Y + 1);
                                            return true;
                                        }
                                    }
                                    break;
                                case 6:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                         && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X - 1, ptCenter.Y + 2, ptCenter.X, ptCenter.Y + 2);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 2] != -444)
                                             && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 2] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y + 3, ptCenter.X, ptCenter.Y + 2);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X + 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 1, ptCenter.Y + 2, ptCenter.X, ptCenter.Y + 2);
                                            return true;
                                        }
                                    }
                                    break;
                                case 7:
                                    if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] != -444)
                                         && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                    {
                                        if ((m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 1] != -444)
                                             && (m_pMainData[pIndex[iIndex].X, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        {
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X, ptCenter.Y, ptCenter.X + 1, ptCenter.Y);
                                            return true;
                                        }
                                        else if ((m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] != -444)
                                            && (m_pMainData[pIndex[iIndex].X - 1, pIndex[iIndex].Y + 1] == m_pMainData[ptCenter.X, ptCenter.Y]))
                                        { 
                                            PointSetting(ref ptStart, ref ptEnd, ptCenter.X + 1, ptCenter.Y + 1, ptCenter.X, ptCenter.Y + 1);
                                            return true;
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    if (iCurrentAni == -1) //폭탄
                    {
                        ptStart = new Point(i, j);
                        ptEnd = new Point(i, j);

                        PointSetting(ref ptStart, ref ptEnd, ptStart.X, ptStart.Y, ptEnd.X, ptEnd.Y); 
                        return true;
                    }
                }
            }

            return false;
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
                Graphics g = e.Graphics;

                for (int i = 2; i < 9; i++)
                {
                    for (int j = 2; j < 9; j++)
                    {
                        switch (m_pMainData[i, j])
                        {
                            case 1:
                                g.FillRectangle(Brushes.Red, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 2:
                                g.FillRectangle(Brushes.Orange, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 3:
                                g.FillRectangle(Brushes.Yellow, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 4:
                                g.FillRectangle(Brushes.Green, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 5:
                                g.FillRectangle(Brushes.Blue, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 6:
                                g.FillRectangle(Brushes.Black, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case 7:
                                g.FillRectangle(Brushes.Purple, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;
                            case -1:
                                g.FillRectangle(Brushes.DarkRed, new Rectangle(50 + (j * 50), 50 + (i * 50), 50, 50));
                                break;

                        }
                    }
                }
            }
        }

        private void PointSetting(ref Point ptStart, ref Point ptEnd, int iSX, int iSY, int iEX, int EY)
        {
            ptStart = new Point(iSX - 2, iSY - 2);
            ptEnd = new Point(iEX - 2, EY - 2);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            button3.Enabled = false;

            AniPangLoad();

            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}

