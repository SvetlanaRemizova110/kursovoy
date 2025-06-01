
namespace kursovoy
{
    partial class Orders
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Orders));
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchText = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.labelCount = new System.Windows.Forms.Label();
            this.labelVSE = new System.Windows.Forms.Label();
            this.buttonPag2 = new System.Windows.Forms.Button();
            this.buttonPag1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.просмотретьЗаказToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button5 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.toggleFullscreenButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.BackColor = System.Drawing.Color.ForestGreen;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(774, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 38);
            this.button1.TabIndex = 11;
            this.button1.Text = "в меню";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Comic Sans MS", 9.75F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Comic Sans MS", 9.75F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(75, 113);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Comic Sans MS", 9.75F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.Size = new System.Drawing.Size(740, 375);
            this.dataGridView1.TabIndex = 10;
            this.dataGridView1.TabStop = false;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightGreen;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(430, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 26);
            this.label2.TabIndex = 12;
            this.label2.Text = "Сортировка (по дате)";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "сначало новые",
            "сначало старые"});
            this.comboBox1.Location = new System.Drawing.Point(640, 71);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(175, 34);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.LightGreen;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(75, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(247, 26);
            this.label3.TabIndex = 14;
            this.label3.Text = "Поиск (по номеру заказа)";
            // 
            // SearchText
            // 
            this.SearchText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SearchText.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchText.Location = new System.Drawing.Point(328, 72);
            this.SearchText.MaxLength = 3;
            this.SearchText.Name = "SearchText";
            this.SearchText.Size = new System.Drawing.Size(87, 34);
            this.SearchText.TabIndex = 1;
            this.SearchText.TextChanged += new System.EventHandler(this.SearchText_TextChanged);
            this.SearchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchText_KeyPress);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LimeGreen;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(566, 24);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(231, 36);
            this.button2.TabIndex = 6;
            this.button2.Text = "Сформировать отчет";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CalendarFont = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerEnd.Location = new System.Drawing.Point(238, 28);
            this.dateTimePickerEnd.MaxDate = new System.DateTime(2025, 7, 1, 0, 0, 0, 0);
            this.dateTimePickerEnd.MinDate = new System.DateTime(2025, 2, 1, 0, 0, 0, 0);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(200, 29);
            this.dateTimePickerEnd.TabIndex = 5;
            this.dateTimePickerEnd.Value = new System.DateTime(2025, 5, 13, 11, 16, 43, 0);
            this.dateTimePickerEnd.ValueChanged += new System.EventHandler(this.dateTimePickerEnd_ValueChanged);
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CalendarFont = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerStart.Location = new System.Drawing.Point(17, 28);
            this.dateTimePickerStart.MaxDate = new System.DateTime(2025, 7, 1, 0, 0, 0, 0);
            this.dateTimePickerStart.MinDate = new System.DateTime(2025, 2, 1, 0, 0, 0, 0);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(200, 29);
            this.dateTimePickerStart.TabIndex = 4;
            this.dateTimePickerStart.Value = new System.DateTime(2025, 5, 13, 11, 16, 53, 0);
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);
            // 
            // labelCount
            // 
            this.labelCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelCount.AutoSize = true;
            this.labelCount.BackColor = System.Drawing.Color.LightGreen;
            this.labelCount.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCount.Location = new System.Drawing.Point(75, 491);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(205, 26);
            this.labelCount.TabIndex = 112;
            this.labelCount.Text = "Количество записей:";
            // 
            // labelVSE
            // 
            this.labelVSE.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelVSE.AutoSize = true;
            this.labelVSE.BackColor = System.Drawing.Color.LightGreen;
            this.labelVSE.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVSE.Location = new System.Drawing.Point(301, 491);
            this.labelVSE.Name = "labelVSE";
            this.labelVSE.Size = new System.Drawing.Size(64, 26);
            this.labelVSE.TabIndex = 116;
            this.labelVSE.Text = "Всего";
            // 
            // buttonPag2
            // 
            this.buttonPag2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPag2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPag2.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonPag2.Location = new System.Drawing.Point(106, 524);
            this.buttonPag2.Name = "buttonPag2";
            this.buttonPag2.Size = new System.Drawing.Size(25, 37);
            this.buttonPag2.TabIndex = 113;
            this.buttonPag2.TabStop = false;
            this.buttonPag2.Text = ">";
            this.buttonPag2.UseVisualStyleBackColor = true;
            this.buttonPag2.Click += new System.EventHandler(this.buttonPag2_Click);
            // 
            // buttonPag1
            // 
            this.buttonPag1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPag1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonPag1.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonPag1.Location = new System.Drawing.Point(75, 524);
            this.buttonPag1.Name = "buttonPag1";
            this.buttonPag1.Size = new System.Drawing.Size(25, 37);
            this.buttonPag1.TabIndex = 114;
            this.buttonPag1.TabStop = false;
            this.buttonPag1.Text = "<";
            this.buttonPag1.UseVisualStyleBackColor = true;
            this.buttonPag1.Click += new System.EventHandler(this.buttonPag1_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.dateTimePickerStart);
            this.panel1.Controls.Add(this.dateTimePickerEnd);
            this.panel1.Font = new System.Drawing.Font("Comic Sans MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel1.Location = new System.Drawing.Point(17, 567);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(859, 84);
            this.panel1.TabIndex = 117;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightGreen;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(642, 541);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(234, 28);
            this.label1.TabIndex = 112;
            this.label1.Text = "Отчет дохода за период";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.просмотретьЗаказToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(178, 26);
            // 
            // просмотретьЗаказToolStripMenuItem
            // 
            this.просмотретьЗаказToolStripMenuItem.Name = "просмотретьЗаказToolStripMenuItem";
            this.просмотретьЗаказToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.просмотретьЗаказToolStripMenuItem.Text = "просмотреть заказ";
            this.просмотретьЗаказToolStripMenuItem.Click += new System.EventHandler(this.viewOrder_Click);
            // 
            // button5
            // 
            this.button5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button5.BackColor = System.Drawing.Color.LightCoral;
            this.button5.Enabled = false;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button5.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.button5.Location = new System.Drawing.Point(768, 495);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(47, 22);
            this.button5.TabIndex = 120;
            this.button5.TabStop = false;
            this.button5.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(628, 492);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 26);
            this.label4.TabIndex = 118;
            this.label4.Text = "Заказ отменён";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(12, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 29);
            this.label7.TabIndex = 122;
            this.label7.Text = "Роль: ";
            // 
            // toggleFullscreenButton
            // 
            this.toggleFullscreenButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.toggleFullscreenButton.BackColor = System.Drawing.Color.LimeGreen;
            this.toggleFullscreenButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.toggleFullscreenButton.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toggleFullscreenButton.Image = global::kursovoy.Properties.Resources._222;
            this.toggleFullscreenButton.Location = new System.Drawing.Point(730, 8);
            this.toggleFullscreenButton.Name = "toggleFullscreenButton";
            this.toggleFullscreenButton.Size = new System.Drawing.Size(38, 38);
            this.toggleFullscreenButton.TabIndex = 123;
            this.toggleFullscreenButton.UseVisualStyleBackColor = false;
            this.toggleFullscreenButton.Click += new System.EventHandler(this.toggleFullscreenButton_Click);
            // 
            // Orders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.ClientSize = new System.Drawing.Size(889, 661);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.ControlBox = false;
            this.Controls.Add(this.toggleFullscreenButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelVSE);
            this.Controls.Add(this.buttonPag2);
            this.Controls.Add(this.buttonPag1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Orders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Заказы";
            this.Load += new System.EventHandler(this.Orders_Load);
            this.SizeChanged += new System.EventHandler(this.Orders_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SearchText;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Label labelVSE;
        private System.Windows.Forms.Button buttonPag2;
        private System.Windows.Forms.Button buttonPag1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem просмотретьЗаказToolStripMenuItem;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button toggleFullscreenButton;
    }
}