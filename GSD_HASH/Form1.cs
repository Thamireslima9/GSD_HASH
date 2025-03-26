using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSD_HASH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: esta linha de código carrega dados na tabela 'tRABALHOHASHcj3022099DataSet.Acessos'. Você pode movê-la ou removê-la conforme necessário.
            this.acessosTableAdapter.Fill(this.tRABALHOHASHcj3022099DataSet.Acessos);

        }

        private void acessosBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.acessosBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.tRABALHOHASHcj3022099DataSet);

        }

        private void acessosBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }
    }
}
