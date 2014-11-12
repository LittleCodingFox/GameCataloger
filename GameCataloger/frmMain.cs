using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace GameCataloger
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            txtFilterName.TextChanged += new EventHandler(txtFilterName_TextChanged);
            txtFilterTags.TextChanged += new EventHandler(txtFilterTags_TextChanged);

            listData.MouseUp += new MouseEventHandler(listData_MouseUp);

            DoFiltering();
        }

        void listData_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listData.SelectedItems.Count == 1)
            {
                Point mousePoint = new Point();
                mousePoint.X = this.Location.X + listData.Location.X + e.Location.X + 25;
                mousePoint.Y = this.Location.Y + listData.Location.Y + e.Location.Y + 55;

                if (((GameInfo)listData.SelectedItems[0].Tag).Done)
                {
                    markAsDoneToolStripMenuItem.Visible = false;
                    markAsNotDoneToolStripMenuItem.Visible = true;
                }
                else
                {
                    markAsDoneToolStripMenuItem.Visible = true;
                    markAsNotDoneToolStripMenuItem.Visible = false;
                }

                //Show context menu 
                contextItem.Show(mousePoint);
            }
        }

        void txtFilterTags_TextChanged(object sender, EventArgs e)
        {
            DoFiltering();
        }

        void txtFilterName_TextChanged(object sender, EventArgs e)
        {
            DoFiltering();
        }

        void DoFiltering()
        {
            ArrayList Games = GameData.Instance.Find(txtFilterName.Text, txtFilterTags.Text);
            listData.Items.Clear();

            for (int i = 0; i < Games.Count; i++)
            {
                GameInfo Info = (GameInfo)Games[i];
                ListViewItem Name = new ListViewItem(Info.Name);

                ListViewItem.ListViewSubItem Rating = new ListViewItem.ListViewSubItem(Name, Info.Rating.ToString());
                Name.SubItems.Add(Rating);
                ListViewItem.ListViewSubItem Tags = new ListViewItem.ListViewSubItem(Name, Info.TagString());
                Name.SubItems.Add(Tags);
                ListViewItem.ListViewSubItem Done = new ListViewItem.ListViewSubItem(Name, Info.Done ? "Yes" : "No");
                Name.SubItems.Add(Done);

                Name.Tag = Games[i];

                listData.Items.Add(Name);
            }
        }

        private void markAsDoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((GameInfo)listData.SelectedItems[0].Tag).Done = true;
            DoFiltering();
        }

        private void markAsNotDoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((GameInfo)listData.SelectedItems[0].Tag).Done = false;
            DoFiltering();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (!GameData.Load(openFileDialog1.FileName))
                {
                    MessageBox.Show("Unable to load the database!");
                }
                else
                {
                    txtFilterName.Text = "";
                    txtFilterTags.Text = "";
                }

                DoFiltering();
            }
        }

        private void savToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (!GameData.Save(saveFileDialog1.FileName))
                {
                    MessageBox.Show("Unable to save the database!");
                }
                else
                {
                    txtFilterName.Text = "";
                    txtFilterTags.Text = "";
                }

                DoFiltering();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditGame EditGame = new frmEditGame();

            if (EditGame.ShowDialog(this) == DialogResult.OK)
            {
                GameInfo Info = (GameInfo)listData.SelectedItems[0].Tag;

                Info.Name = EditGame.txtName.Text;
                Info.Rating = (int)EditGame.nudRating.Value;
                Info.Done = EditGame.chkCompleted.Checked;
                Info.Tags.Clear();

                String[] TagFragments = EditGame.txtTags.Text.Split(",".ToCharArray());

                for (int i = 0; i < TagFragments.Length; i++)
                {
                    String FinalTag = TagFragments[i].Trim();

                    if (FinalTag.Length > 0)
                        Info.Tags.Add(FinalTag);
                }

                DoFiltering();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete '" + ((GameInfo)listData.SelectedItems[0].Tag).Name +
                "'?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GameData.Instance.Games.Remove(listData.SelectedItems[0].Tag);
                DoFiltering();
            }
        }

        private void addGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditGame EditGame = new frmEditGame();

            if (EditGame.ShowDialog(this) == DialogResult.OK)
            {
                GameInfo Info = new GameInfo();

                Info.Name = EditGame.txtName.Text;
                Info.Rating = (int)EditGame.nudRating.Value;
                Info.Done = EditGame.chkCompleted.Checked;

                String[] TagFragments = EditGame.txtTags.Text.Split(",".ToCharArray());

                for (int i = 0; i < TagFragments.Length; i++)
                {
                    String FinalTag = TagFragments[i].Trim();

                    if (FinalTag.Length > 0)
                        Info.Tags.Add(FinalTag);
                }

                GameData.Instance.Games.Add(Info);

                DoFiltering();
            }
        }
    }
}
