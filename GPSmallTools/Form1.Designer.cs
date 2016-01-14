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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAllRemove = new System.Windows.Forms.Button();
            this.lblSHZS1 = new System.Windows.Forms.Label();
            this.lblSZZS1 = new System.Windows.Forms.Label();
            this.lblSHZS2 = new System.Windows.Forms.Label();
            this.lblSHZS3 = new System.Windows.Forms.Label();
            this.lblSZZS2 = new System.Windows.Forms.Label();
            this.lblSZZS3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 71);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.dataGridView1.Size = new System.Drawing.Size(270, 269);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(7, 346);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "刷  新";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(169, 346);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 2;
            this.btnRight.Text = "》";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(88, 346);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "︾";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(6, 389);
            this.txtCode.MaxLength = 6;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(93, 21);
            this.txtCode.TabIndex = 4;
            this.txtCode.Text = "600030";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(104, 389);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(158, 389);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(49, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "删除";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAllRemove
            // 
            this.btnAllRemove.Location = new System.Drawing.Point(212, 389);
            this.btnAllRemove.Name = "btnAllRemove";
            this.btnAllRemove.Size = new System.Drawing.Size(49, 23);
            this.btnAllRemove.TabIndex = 7;
            this.btnAllRemove.Text = "全删";
            this.btnAllRemove.UseVisualStyleBackColor = true;
            this.btnAllRemove.Click += new System.EventHandler(this.btnAllRemove_Click);
            // 
            // lblSHZS1
            // 
            this.lblSHZS1.AutoSize = true;
            this.lblSHZS1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSHZS1.Location = new System.Drawing.Point(10, 5);
            this.lblSHZS1.Name = "lblSHZS1";
            this.lblSHZS1.Size = new System.Drawing.Size(29, 20);
            this.lblSHZS1.TabIndex = 8;
            this.lblSHZS1.Text = "A1";
            // 
            // lblSZZS1
            // 
            this.lblSZZS1.AutoSize = true;
            this.lblSZZS1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSZZS1.Location = new System.Drawing.Point(163, 5);
            this.lblSZZS1.Name = "lblSZZS1";
            this.lblSZZS1.Size = new System.Drawing.Size(29, 20);
            this.lblSZZS1.TabIndex = 9;
            this.lblSZZS1.Text = "B1";
            // 
            // lblSHZS2
            // 
            this.lblSHZS2.AutoSize = true;
            this.lblSHZS2.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSHZS2.Location = new System.Drawing.Point(7, 30);
            this.lblSHZS2.Name = "lblSHZS2";
            this.lblSHZS2.Size = new System.Drawing.Size(26, 18);
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
            this.lblSZZS2.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSZZS2.Location = new System.Drawing.Point(155, 30);
            this.lblSZZS2.Name = "lblSZZS2";
            this.lblSZZS2.Size = new System.Drawing.Size(26, 18);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 438);
            this.Controls.Add(this.lblSZZS3);
            this.Controls.Add(this.lblSZZS2);
            this.Controls.Add(this.lblSHZS3);
            this.Controls.Add(this.lblSHZS2);
            this.Controls.Add(this.lblSZZS1);
            this.Controls.Add(this.lblSHZS1);
            this.Controls.Add(this.btnAllRemove);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Opacity = 0.6D;
            this.ShowIcon = false;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAllRemove;
        private System.Windows.Forms.Label lblSHZS1;
        private System.Windows.Forms.Label lblSZZS1;
        private System.Windows.Forms.Label lblSHZS2;
        private System.Windows.Forms.Label lblSHZS3;
        private System.Windows.Forms.Label lblSZZS2;
        private System.Windows.Forms.Label lblSZZS3;
    }
}

