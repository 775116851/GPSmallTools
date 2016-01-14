using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GPSmallTools
{
    /// <summary>
    /// 枚举，描述消息窗口加载的形式
    /// </summary>
    public enum LoadMode
    {
        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 提示
        /// </summary>
        Prompt
    }

    public partial class FormMessageBox : Form
    {
        public FormMessageBox()
        {
            InitializeComponent();
        }

        #region ***********************字 段***********************

        /// <summary>
        /// 窗体加载模式
        /// </summary>
        private static LoadMode FormMode = LoadMode.Prompt;

        /// <summary>
        /// 显示的消息正文
        /// </summary>
        private static string ShowMessage = null;

        /// <summary>
        /// 关闭窗口的定时器
        /// </summary>
        private Timer Timer_Close = new Timer();


        [DllImportAttribute("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);   // 该函数可以实现窗体的动画效果

        public const Int32 AW_HOR_POSITIVE = 0x00000001;   // 自左向右显示窗口。该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略 
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;   // 自右向左显示窗口。当使用了 AW_CENTER 标志时该标志被忽略

        public const Int32 AW_VER_POSITIVE = 0x00000004;   // 自顶向下显示窗口。该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略
        public const Int32 AW_VER_NEGATIVE = 0x00000008;   // 自下向上显示窗口。该标志可以在滚动动画和滑动动画中使用。当使用AW_CENTER标志时，该标志将被忽略

        public const Int32 AW_CENTER = 0x00000010;         // 若使用了AW_HIDE标志，则使窗口向内重叠；若未使用AW_HIDE标志，则使窗口向外扩展
        public const Int32 AW_HIDE = 0x00010000;           // 隐藏窗口，缺省则显示窗口
        public const Int32 AW_ACTIVATE = 0x00020000;       // 激活窗口。在使用了AW_HIDE标志后不要使用这个标志
        public const Int32 AW_SLIDE = 0x00040000;          // 使用滑动类型。缺省则为滚动动画类型。当使用AW_CENTER标志时，这个标志就被忽略
        public const Int32 AW_BLEND = 0x00080000;          // 使用淡入效果。只有当hWnd为顶层窗口的时候才可以使用此标志

        #endregion*************************************************


        #region ***********************方 法***********************

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="loadMode">加载模式</param>
        /// <param name="message">消息正文</param>

        public static void Show(LoadMode loadMode, string message)
        {
            FormMode = loadMode;
            ShowMessage = message;

            FormMessageBox box = new FormMessageBox();
            box.Show();
        }
        #endregion*************************************************

        private void FormMessageBox_Load(object sender, EventArgs e)
        {
            this.lblTitle.Text = "提示";
            if (FormMode == LoadMode.Error)
            {
                this.lblTitle.Text = "错误";
                //this.plShow.BackgroundImage = global::dataSource.Properties.Resources.error;    // 更换背景
            }
            else if (FormMode == LoadMode.Warning)
            {
                this.lblTitle.Text = "警告";
                //this.plShow.BackgroundImage = global::dataSource.Properties.Resources.warning;  // 更换背景
            }
            else
            {
                //this.plShow.BackgroundImage = global::dataSource.Properties.Resources.Prompt;   // 更换背景
            }

            this.lblMessage.Text = ShowMessage;

            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int top = height - 35 - this.Height;
            int left = width - this.Width - 5;
            this.Top = top;
            this.Left = left;
            this.TopMost = true;

            AnimateWindow(this.Handle, 500, AW_SLIDE + AW_VER_NEGATIVE);//开始窗体动画

            this.ShowInTaskbar = false;

            Timer_Close.Interval = 4000;
            Timer_Close.Tick += new EventHandler(Timer_Close_Tick);
            Timer_Close.Start();
        }

        /// <summary>
        /// 关闭窗口的定时器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Close_Tick(object sender, EventArgs e)
        {
            Timer_Close.Stop();

            this.Close();
        }

        /// <summary>
        /// 窗口已经关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            AnimateWindow(this.Handle, 500, AW_SLIDE + AW_VER_POSITIVE + AW_HIDE);

            Timer_Close.Stop();
            Timer_Close.Dispose();
        }

        /// <summary>
        /// 鼠标移动在消息框上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plShow_MouseMove(object sender, MouseEventArgs e)
        {
            this.Timer_Close.Stop();
        }

        /// <summary>
        /// 鼠标移动离开消息框上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plShow_MouseLeave(object sender, EventArgs e)
        {
            this.Timer_Close.Start();
        }


    }
}
