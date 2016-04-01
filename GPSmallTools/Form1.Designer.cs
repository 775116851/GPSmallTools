namespace GPSmallTools
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lblSHZS1 = new System.Windows.Forms.Label();
            this.lblSZZS1 = new System.Windows.Forms.Label();
            this.lblSHZS2 = new System.Windows.Forms.Label();
            this.lblSHZS3 = new System.Windows.Forms.Label();
            this.lblSZZS2 = new System.Windows.Forms.Label();
            this.lblSZZS3 = new System.Windows.Forms.Label();
            this.notifyIconMsg = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripMsg = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBoxInfo1 = new System.Windows.Forms.GroupBox();
            this.cboIsTX = new System.Windows.Forms.CheckBox();
            this.btnDownX = new System.Windows.Forms.Button();
            this.txtOpacity = new System.Windows.Forms.TextBox();
            this.trackBarOpacity = new System.Windows.Forms.TrackBar();
            this.cboTopMost = new System.Windows.Forms.CheckBox();
            this.btnAllRemove = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.groupBoxGJ = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.SysNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.代码 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.涨百 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.跌百 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.涨元 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.跌元 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStripMsg.SuspendLayout();
            this.contextMenuStripGridView.SuspendLayout();
            this.groupBoxInfo1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).BeginInit();
            this.groupBoxGJ.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 71);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(968, 269);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(6, 346);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(83, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "刷  新";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(178, 346);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(83, 23);
            this.btnRight.TabIndex = 2;
            this.btnRight.Text = "》";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(92, 346);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(83, 23);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "︾";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lblSHZS1
            // 
            this.lblSHZS1.AutoSize = true;
            this.lblSHZS1.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSHZS1.Location = new System.Drawing.Point(10, 5);
            this.lblSHZS1.Name = "lblSHZS1";
            this.lblSHZS1.Size = new System.Drawing.Size(26, 18);
            this.lblSHZS1.TabIndex = 8;
            this.lblSHZS1.Text = "A1";
            // 
            // lblSZZS1
            // 
            this.lblSZZS1.AutoSize = true;
            this.lblSZZS1.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSZZS1.Location = new System.Drawing.Point(163, 5);
            this.lblSZZS1.Name = "lblSZZS1";
            this.lblSZZS1.Size = new System.Drawing.Size(26, 18);
            this.lblSZZS1.TabIndex = 9;
            this.lblSZZS1.Text = "B1";
            // 
            // lblSHZS2
            // 
            this.lblSHZS2.AutoSize = true;
            this.lblSHZS2.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSHZS2.Location = new System.Drawing.Point(1, 30);
            this.lblSHZS2.Name = "lblSHZS2";
            this.lblSHZS2.Size = new System.Drawing.Size(21, 14);
            this.lblSHZS2.TabIndex = 10;
            this.lblSHZS2.Text = "A2";
            // 
            // lblSHZS3
            // 
            this.lblSHZS3.AutoSize = true;
            this.lblSHZS3.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSHZS3.Location = new System.Drawing.Point(6, 52);
            this.lblSHZS3.Name = "lblSHZS3";
            this.lblSHZS3.Size = new System.Drawing.Size(21, 14);
            this.lblSHZS3.TabIndex = 11;
            this.lblSHZS3.Text = "A3";
            // 
            // lblSZZS2
            // 
            this.lblSZZS2.AutoSize = true;
            this.lblSZZS2.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSZZS2.Location = new System.Drawing.Point(148, 30);
            this.lblSZZS2.Name = "lblSZZS2";
            this.lblSZZS2.Size = new System.Drawing.Size(21, 14);
            this.lblSZZS2.TabIndex = 12;
            this.lblSZZS2.Text = "B2";
            // 
            // lblSZZS3
            // 
            this.lblSZZS3.AutoSize = true;
            this.lblSZZS3.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSZZS3.Location = new System.Drawing.Point(157, 52);
            this.lblSZZS3.Name = "lblSZZS3";
            this.lblSZZS3.Size = new System.Drawing.Size(21, 14);
            this.lblSZZS3.TabIndex = 13;
            this.lblSZZS3.Text = "B3";
            // 
            // notifyIconMsg
            // 
            this.notifyIconMsg.BalloonTipText = "DDF";
            this.notifyIconMsg.BalloonTipTitle = "QQQ";
            this.notifyIconMsg.ContextMenuStrip = this.contextMenuStripMsg;
            this.notifyIconMsg.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMsg.Icon")));
            this.notifyIconMsg.Tag = "风";
            this.notifyIconMsg.Text = "我只是低调的在这待着，别惹我";
            this.notifyIconMsg.Visible = true;
            this.notifyIconMsg.DoubleClick += new System.EventHandler(this.notifyIconMsg_DoubleClick);
            // 
            // contextMenuStripMsg
            // 
            this.contextMenuStripMsg.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.contextMenuStripMsg.Name = "contextMenuStripMsg";
            this.contextMenuStripMsg.Size = new System.Drawing.Size(101, 26);
            this.contextMenuStripMsg.Text = "退出系统";
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // contextMenuStripGridView
            // 
            this.contextMenuStripGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.置顶ToolStripMenuItem,
            this.删除ToolStripMenuItem});
            this.contextMenuStripGridView.Name = "contextMenuStripGridView";
            this.contextMenuStripGridView.Size = new System.Drawing.Size(101, 48);
            // 
            // 置顶ToolStripMenuItem
            // 
            this.置顶ToolStripMenuItem.Name = "置顶ToolStripMenuItem";
            this.置顶ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.置顶ToolStripMenuItem.Text = "置顶";
            this.置顶ToolStripMenuItem.Click += new System.EventHandler(this.置顶ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBoxInfo1
            // 
            this.groupBoxInfo1.Controls.Add(this.cboIsTX);
            this.groupBoxInfo1.Controls.Add(this.btnDownX);
            this.groupBoxInfo1.Controls.Add(this.txtOpacity);
            this.groupBoxInfo1.Controls.Add(this.trackBarOpacity);
            this.groupBoxInfo1.Controls.Add(this.cboTopMost);
            this.groupBoxInfo1.Controls.Add(this.btnAllRemove);
            this.groupBoxInfo1.Controls.Add(this.btnRemove);
            this.groupBoxInfo1.Controls.Add(this.btnAdd);
            this.groupBoxInfo1.Controls.Add(this.txtCode);
            this.groupBoxInfo1.Location = new System.Drawing.Point(6, 378);
            this.groupBoxInfo1.Name = "groupBoxInfo1";
            this.groupBoxInfo1.Size = new System.Drawing.Size(255, 97);
            this.groupBoxInfo1.TabIndex = 18;
            this.groupBoxInfo1.TabStop = false;
            this.groupBoxInfo1.Text = "基础设置";
            // 
            // cboIsTX
            // 
            this.cboIsTX.AutoSize = true;
            this.cboIsTX.Location = new System.Drawing.Point(202, 64);
            this.cboIsTX.Name = "cboIsTX";
            this.cboIsTX.Size = new System.Drawing.Size(48, 16);
            this.cboIsTX.TabIndex = 26;
            this.cboIsTX.Text = "提醒";
            this.cboIsTX.UseVisualStyleBackColor = true;
            this.cboIsTX.CheckedChanged += new System.EventHandler(this.cboIsTX_CheckedChanged);
            // 
            // btnDownX
            // 
            this.btnDownX.Location = new System.Drawing.Point(202, 59);
            this.btnDownX.Name = "btnDownX";
            this.btnDownX.Size = new System.Drawing.Size(49, 23);
            this.btnDownX.TabIndex = 25;
            this.btnDownX.Text = "︾";
            this.btnDownX.UseVisualStyleBackColor = true;
            this.btnDownX.Visible = false;
            this.btnDownX.Click += new System.EventHandler(this.btnDownX_Click);
            // 
            // txtOpacity
            // 
            this.txtOpacity.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOpacity.Location = new System.Drawing.Point(111, 64);
            this.txtOpacity.MaxLength = 3;
            this.txtOpacity.Name = "txtOpacity";
            this.txtOpacity.Size = new System.Drawing.Size(33, 14);
            this.txtOpacity.TabIndex = 24;
            this.txtOpacity.Text = "100";
            this.txtOpacity.MouseLeave += new System.EventHandler(this.txtOpacity_MouseLeave);
            // 
            // trackBarOpacity
            // 
            this.trackBarOpacity.Location = new System.Drawing.Point(-2, 50);
            this.trackBarOpacity.Maximum = 100;
            this.trackBarOpacity.Name = "trackBarOpacity";
            this.trackBarOpacity.Size = new System.Drawing.Size(115, 45);
            this.trackBarOpacity.TabIndex = 23;
            this.trackBarOpacity.TickFrequency = 10;
            this.trackBarOpacity.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarOpacity.Value = 33;
            this.trackBarOpacity.Scroll += new System.EventHandler(this.trackBarOpacity_Scroll);
            // 
            // cboTopMost
            // 
            this.cboTopMost.AutoSize = true;
            this.cboTopMost.Location = new System.Drawing.Point(150, 64);
            this.cboTopMost.Name = "cboTopMost";
            this.cboTopMost.Size = new System.Drawing.Size(48, 16);
            this.cboTopMost.TabIndex = 22;
            this.cboTopMost.Text = "置顶";
            this.cboTopMost.UseVisualStyleBackColor = true;
            this.cboTopMost.CheckedChanged += new System.EventHandler(this.cboTopMost_CheckedChanged);
            // 
            // btnAllRemove
            // 
            this.btnAllRemove.Location = new System.Drawing.Point(202, 24);
            this.btnAllRemove.Name = "btnAllRemove";
            this.btnAllRemove.Size = new System.Drawing.Size(49, 23);
            this.btnAllRemove.TabIndex = 21;
            this.btnAllRemove.Text = "全删";
            this.btnAllRemove.UseVisualStyleBackColor = true;
            this.btnAllRemove.Click += new System.EventHandler(this.btnAllRemove_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(149, 24);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(49, 23);
            this.btnRemove.TabIndex = 20;
            this.btnRemove.Text = "删除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(95, 24);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 23);
            this.btnAdd.TabIndex = 19;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(8, 24);
            this.txtCode.MaxLength = 6;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(81, 21);
            this.txtCode.TabIndex = 18;
            this.txtCode.Text = "600030";
            // 
            // groupBoxGJ
            // 
            this.groupBoxGJ.Controls.Add(this.dataGridView2);
            this.groupBoxGJ.Location = new System.Drawing.Point(273, 346);
            this.groupBoxGJ.Name = "groupBoxGJ";
            this.groupBoxGJ.Size = new System.Drawing.Size(701, 129);
            this.groupBoxGJ.TabIndex = 19;
            this.groupBoxGJ.TabStop = false;
            this.groupBoxGJ.Text = "高级设置";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SysNo,
            this.名称,
            this.代码,
            this.涨百,
            this.跌百,
            this.涨元,
            this.跌元});
            this.dataGridView2.Location = new System.Drawing.Point(6, 18);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(689, 105);
            this.dataGridView2.TabIndex = 0;
            this.dataGridView2.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellEndEdit);
            this.dataGridView2.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView2_CellMouseDown);
            // 
            // SysNo
            // 
            this.SysNo.DataPropertyName = "SysNo";
            this.SysNo.Frozen = true;
            this.SysNo.HeaderText = "SysNo";
            this.SysNo.Name = "SysNo";
            this.SysNo.ReadOnly = true;
            this.SysNo.Visible = false;
            // 
            // 名称
            // 
            this.名称.ContextMenuStrip = this.contextMenuStripGridView;
            this.名称.DataPropertyName = "name";
            this.名称.Frozen = true;
            this.名称.HeaderText = "名称";
            this.名称.Name = "名称";
            // 
            // 代码
            // 
            this.代码.ContextMenuStrip = this.contextMenuStripGridView;
            this.代码.DataPropertyName = "code";
            this.代码.HeaderText = "代码";
            this.代码.Name = "代码";
            // 
            // 涨百
            // 
            this.涨百.ContextMenuStrip = this.contextMenuStripGridView;
            this.涨百.DataPropertyName = "ZFB";
            this.涨百.HeaderText = "涨百";
            this.涨百.Name = "涨百";
            this.涨百.Width = 103;
            // 
            // 跌百
            // 
            this.跌百.ContextMenuStrip = this.contextMenuStripGridView;
            this.跌百.DataPropertyName = "DFB";
            this.跌百.HeaderText = "跌百";
            this.跌百.Name = "跌百";
            this.跌百.Width = 103;
            // 
            // 涨元
            // 
            this.涨元.ContextMenuStrip = this.contextMenuStripGridView;
            this.涨元.DataPropertyName = "ZFY";
            this.涨元.HeaderText = "涨元";
            this.涨元.Name = "涨元";
            this.涨元.Width = 120;
            // 
            // 跌元
            // 
            this.跌元.ContextMenuStrip = this.contextMenuStripGridView;
            this.跌元.DataPropertyName = "DFY";
            this.跌元.HeaderText = "跌元";
            this.跌元.Name = "跌元";
            this.跌元.Width = 120;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 372);
            this.Controls.Add(this.groupBoxGJ);
            this.Controls.Add(this.groupBoxInfo1);
            this.Controls.Add(this.lblSZZS3);
            this.Controls.Add(this.lblSZZS2);
            this.Controls.Add(this.lblSHZS3);
            this.Controls.Add(this.lblSHZS2);
            this.Controls.Add(this.lblSZZS1);
            this.Controls.Add(this.lblSHZS1);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0.6D;
            this.Text = "测试";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Leave += new System.EventHandler(this.Form1_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStripMsg.ResumeLayout(false);
            this.contextMenuStripGridView.ResumeLayout(false);
            this.groupBoxInfo1.ResumeLayout(false);
            this.groupBoxInfo1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).EndInit();
            this.groupBoxGJ.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Label lblSHZS1;
        private System.Windows.Forms.Label lblSZZS1;
        private System.Windows.Forms.Label lblSHZS2;
        private System.Windows.Forms.Label lblSHZS3;
        private System.Windows.Forms.Label lblSZZS2;
        private System.Windows.Forms.Label lblSZZS3;
        private System.Windows.Forms.NotifyIcon notifyIconMsg;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMsg;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripGridView;
        private System.Windows.Forms.ToolStripMenuItem 置顶ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBoxInfo1;
        private System.Windows.Forms.Button btnDownX;
        private System.Windows.Forms.TextBox txtOpacity;
        private System.Windows.Forms.TrackBar trackBarOpacity;
        private System.Windows.Forms.CheckBox cboTopMost;
        private System.Windows.Forms.Button btnAllRemove;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.GroupBox groupBoxGJ;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn SysNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn 名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 代码;
        private System.Windows.Forms.DataGridViewTextBoxColumn 涨百;
        private System.Windows.Forms.DataGridViewTextBoxColumn 跌百;
        private System.Windows.Forms.DataGridViewTextBoxColumn 涨元;
        private System.Windows.Forms.DataGridViewTextBoxColumn 跌元;
        private System.Windows.Forms.CheckBox cboIsTX;
    }
}

