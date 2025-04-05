
namespace kursovoy
{
    partial class OrderForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewOrder = new System.Windows.Forms.DataGridView();
            this.AddOrder = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewOrder
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Comic Sans MS", 9.75F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewOrder.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewOrder.Location = new System.Drawing.Point(17, 12);
            this.dataGridViewOrder.Name = "dataGridViewOrder";
            this.dataGridViewOrder.Size = new System.Drawing.Size(838, 213);
            this.dataGridViewOrder.TabIndex = 0;
            this.dataGridViewOrder.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOrder_CellContentClick);
            this.dataGridViewOrder.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOrder_CellEndEdit);
            // 
            // AddOrder
            // 
            this.AddOrder.BackColor = System.Drawing.Color.LimeGreen;
            this.AddOrder.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.AddOrder.Location = new System.Drawing.Point(717, 389);
            this.AddOrder.Name = "AddOrder";
            this.AddOrder.Size = new System.Drawing.Size(138, 44);
            this.AddOrder.TabIndex = 1;
            this.AddOrder.Text = "оформить";
            this.AddOrder.UseVisualStyleBackColor = false;
            this.AddOrder.Click += new System.EventHandler(this.AddOrder_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LimeGreen;
            this.button1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.button1.Location = new System.Drawing.Point(741, 231);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 44);
            this.button1.TabIndex = 2;
            this.button1.Text = "очистить";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.label1.Location = new System.Drawing.Point(327, 404);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Итоговая сумма:_______";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.ForestGreen;
            this.button2.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(20, 394);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 39);
            this.button2.TabIndex = 4;
            this.button2.Text = "назад";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(15, 233);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(428, 29);
            this.label2.TabIndex = 5;
            this.label2.Text = "*При заказе больше 2000 руб скидка 15%";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.label3.Location = new System.Drawing.Point(278, 375);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(321, 29);
            this.label3.TabIndex = 6;
            this.label3.Text = "Скидка 15% от 2000р:_______";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Comic Sans MS", 15.75F);
            this.label4.Location = new System.Drawing.Point(297, 346);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(287, 29);
            this.label4.TabIndex = 7;
            this.label4.Text = "Cумма без скидки:_______";
            // 
            // OrderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.ClientSize = new System.Drawing.Size(867, 443);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AddOrder);
            this.Controls.Add(this.dataGridViewOrder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OrderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Оформление заказа";
            this.Load += new System.EventHandler(this.OrderForm_Load);
            this.Shown += new System.EventHandler(this.OrderForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewOrder;
        private System.Windows.Forms.Button AddOrder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer Timer;
    }
}