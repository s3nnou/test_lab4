using System;
using System.IO;
using System.Windows.Forms;


namespace lab4
{
    public partial class MainForm : Form
    {
        private string parentFolderPath;

        public MainForm()
        {
            InitializeComponent();
            this.dirView.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            dirItemsList.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem item = null;

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.FullName, 1);
                if(item.ToString().Contains(".txt"))
                    dirItemsList.Items.Add(item);
            }

            dirItemsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            txtShow.Show();
            string[] lines = File.ReadAllLines(dirItemsList.SelectedItems[0].Text);

            foreach (string item in lines)
            {
                txtShow.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dirView.BeginUpdate();
            TreeNode node = dirView.SelectedNode;
            
            string path = null;
            if (node.Parent != null)
            {
                Object anode = node.Tag;
                path = node.Parent.Tag.ToString() + "\\" + dirView.SelectedNode.Tag.ToString();
            }
            else
            {
                Object aNode = dirView.SelectedNode.Tag;
                path = aNode.ToString();
            }

            string fullpath = Path.Combine(path, textBox1.Text);
            Directory.CreateDirectory(fullpath);
            dirView.Nodes.Clear();

            TreeViewWorks stuff = new TreeViewWorks();
            stuff.PopulateTreeView(parentFolderPath, dirView);
            MonitorDirectory(parentFolderPath);
            dirView.EndUpdate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dirView.BeginUpdate();

            TreeNode node = dirView.SelectedNode;

            string path = null;

            if (node.Parent != null)
            {
                Object anode = node.Tag;
                path = node.Parent.Tag.ToString() + "\\" + dirView.SelectedNode.Tag.ToString();
            }
            else
            {
                Object aNode = dirView.SelectedNode.Tag;
                path = aNode.ToString();
            }

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Directory.Delete(path);

            dirView.Nodes.Clear();
            TreeViewWorks stuff = new TreeViewWorks();
            stuff.PopulateTreeView(parentFolderPath, dirView);
            MonitorDirectory(parentFolderPath);
            dirView.EndUpdate();
        }

        private void MonitorDirectory(string path)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;

            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e) => MessageBox.Show(text: $"File: {e.Name} renamed to {e.FullPath}");

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
                parentFolderPath = dialog.SelectedPath;
            }

            TreeViewWorks stuff = new TreeViewWorks();
            stuff.PopulateTreeView(parentFolderPath, dirView);
            MonitorDirectory(parentFolderPath);

        }
    }
}
