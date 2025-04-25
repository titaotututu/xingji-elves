using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelApp.models;

namespace TravelApp.controller
{
    public partial class CommunityCell : UserControl
    {
        public long JournalId;
        public Refresh Refresh;
        public ChangePanel ChangePanel;
        public CommunityCell(long journalId, ChangePanel changePanel)
        {
            InitializeComponent();
            this.JournalId = journalId;
            this.ChangePanel = changePanel;
            
        }


        private void CommunityCell_Click(object sender, EventArgs e)
        {
            long id = this.JournalId;
            CommunityDetail CommunityDetail = new CommunityDetail(id, ChangePanel);
            this.ChangePanel(CommunityDetail);
        }

        private void title_Click(object sender, EventArgs e)
        {
            CommunityCell_Click(sender, e);
        }

        private void userName_Click(object sender, EventArgs e)
        {
            CommunityCell_Click(sender, e);
        }

        private void time_Click(object sender, EventArgs e)
        {
            CommunityCell_Click(sender, e);
        }

        private void image_Click(object sender, EventArgs e)
        {
            CommunityCell_Click(sender, e);
        }
    }
}
