using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AkinaRPTool
{
    public partial class ProjectBuild : Window
    {
        public enum TargetResourceType
        {
            Altv,
            Single,
            Fivem
        }
        public string outputFolder = "";
        public string collectionName = "";
        public ProjectBuild()
        {
            InitializeComponent();
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            TargetResourceType resType = TargetResourceType.Altv;

            if (isSinglePlayerRadio.IsChecked == true)
                resType = TargetResourceType.Single;
            else if (isFivemResourceRadio.IsChecked == true)
                resType = TargetResourceType.Fivem;

            collectionName = collectionNameText.Text.ToLower().Replace("_", "");

            if (collectionName == null || collectionName == "")
            {
                MessageBox.Show("There is no cloth collection name path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (outputFolder == null || outputFolder == "")
            {
                MessageBox.Show("There is no output path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MainWindow.clothes.Count <= 0)
            {
                MessageBox.Show("There is no clothes imported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            outputFolder += @"\" + collectionName;

            Directory.CreateDirectory(outputFolder);

            switch (resType)
            {
                case TargetResourceType.Altv:
                    //ResourceBuilder.BuildResourceAltv(outputFolder, collectionName);
                    break;

                case TargetResourceType.Single:
                    //ResourceBuilder.BuildResourceSingle(outputFolder, collectionName);
                    break;

                case TargetResourceType.Fivem:
                    ResourceBuilder.BuildResourceFivem(outputFolder, collectionName);
                    break;
            }
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputFolder = folderBrowserDialog.SelectedPath;
                outFolderPathText.Content = outputFolder;
            }
        }

        private void ValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[A-Za-z_]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CollectionNameText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}
