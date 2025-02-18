using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class Dialog : Form
    {
        public string inText { get; set; }
        public Dialog()
        {
            InitializeComponent();
            inText = string.Empty;
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                inText = textBox1.Text; 
                return;
            }
            else
                inText= textBox1.Text;
            
        }
    }
}
