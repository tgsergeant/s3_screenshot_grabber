namespace s3grabber
{
    partial class FormCutting
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
            this.panelCutting = new s3grabber.FormCutting.CuttingPanel();
            this.SuspendLayout();
            // 
            // panelCutting
            // 
            this.panelCutting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCutting.Location = new System.Drawing.Point(0, 0);
            this.panelCutting.Name = "panelCutting";
            this.panelCutting.Size = new System.Drawing.Size(284, 262);
            this.panelCutting.TabIndex = 0;
            this.panelCutting.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCutting_Paint);
            this.panelCutting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelCutting_MouseDown);
            this.panelCutting.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelCutting_MouseMove);
            this.panelCutting.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelCutting_MouseUp);
            // 
            // FormCutting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panelCutting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCutting";
            this.Text = "FormCutting";
            this.Deactivate += new System.EventHandler(this.FormCutting_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCutting_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormCutting_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private FormCutting.CuttingPanel panelCutting;

    }
}