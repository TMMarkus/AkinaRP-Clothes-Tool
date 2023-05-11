using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Forms;
using System.Windows.Input;
using static AkinaRPTool.ClothData;

using ProgressBar = System.Windows.Forms.ProgressBar;
using System.Diagnostics;
using System.Windows.Shapes;

namespace AkinaRPTool
{
    public class ComboTypeItem : object
    {
        protected string m_Name;
        protected ClothNameResolver.Type m_Value;

        public ComboTypeItem(string name, ClothNameResolver.Type in_value)
        {
            m_Name = name;
            m_Value = in_value;
        }

        public ClothNameResolver.Type GetValue()
        {
            return m_Value;
        }

        public override string ToString()
        {
            return m_Name;
        }
    };

    public class ComboCategoryItem : object
    {
        protected string m_Name;
        protected ClothNameResolver.DrawableType m_Value;

        public ComboCategoryItem(string name, ClothNameResolver.DrawableType in_value)
        {
            m_Name = name;
            m_Value = in_value;
        }

        public ClothNameResolver.DrawableType GetValue()
        {
            return m_Value;
        }

        public override string ToString()
        {
            return m_Name;
        }
    };

    public partial class MainWindow : Window
    {
        public static TextBlock statusTextBlock = null;
        public ProgressBar statusBar = null;

        public static ObservableCollection<ClothData> clothes;

        private static ClothData selectedCloth = null;
        public static ProjectBuild projectBuildWindow = null;

        private static string appVersion = "v0.0.1";

        public static MainWindow Instance { get; private set; }

        private static ListCollectionView clothesList;
        bool updating = false;

        public MainWindow()
        {
            InitializeComponent();

            statusTextBlock = ((TextBlock)FindName("currentStatusBar"));
            statusBar = ((ProgressBar)FindName("currentProgress"));

            clothes = new ObservableCollection<ClothData>();
            clothesList = new ListCollectionView(clothes);

            allListBox.ItemsSource = clothesList;

            editGroupBox.Visibility = Visibility.Hidden;
            clothEditWindow.Visibility = Visibility.Hidden;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            appVersion = "[Alpha v" + fvi.FileVersion + "] AkinaRP Clothes Tool by TMMarkus";

            highHeelsNumberText.LostFocus += HighHeels_LostFocus;

            WindowMain.Title = appVersion;

            Instance = this;

            MessageBox.Show("This application is under development, this means that there may be errors and it may be unstable. If you see any errors, I encourage you to report them on my GitHub.\n" +
                "https://github.com/TMMarkus/AkinaRP-Clothes-Tool", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void SetStatus(string status)
        {
            statusTextBlock.Text = status;
        }


        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar el menú en la posición del cursor del mouse
            Button boton = (Button)sender;
            boton.ContextMenu.PlacementTarget = boton;
            boton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            boton.ContextMenu.IsOpen = true;
        }

        private void OpenMenuGrid_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar el menú en la posición del cursor del mouse
            Grid boton = (Grid)sender;
            boton.ContextMenu.PlacementTarget = boton;
            boton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            boton.ContextMenu.IsOpen = true;
        }

        private void ShowFileBrowser(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null) return;

            string folderPath = System.IO.Path.GetDirectoryName(selectedCloth.mainPath);

            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }

        private void UpdateSelection()
        {
            ClothData temp = selectedCloth;

            selectedCloth = null;

            foreach (var cloth in allListBox.Items)
            {
                if ((ClothData)cloth == temp)
                {
                    selectedCloth = temp;
                    allListBox.SelectedItem = cloth;
                    break;
                }
            }
        }
        

        private void AddAllClothes_Click_Folder(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFolder(Sex.All);
        }

        private void AddAllClothes_Click_File(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.All);
        }

        private void AddMaleClothes_Click_Folder(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFolder(Sex.Male);
        }

        private void AddMaleClothes_Click_File(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Male);
        }

        private void AddFemaleClothes_Click_Folder(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFolder(Sex.Female);
        }

        private void AddFemaleClothes_Click_File(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Female);
        }

        private void RemoveUnderCursor_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
            {
                var _clothes = clothes.OrderBy(x => x.posi).ToList();

                clothes.Clear();

                foreach (var cloth in _clothes)
                {
                    if (cloth != selectedCloth)
                    {
                        clothes.Add(cloth);
                    }
                }

                selectedCloth = null;
                editGroupBox.Visibility = Visibility.Hidden;
                ProjectController.Instance().UpdateClothesList();
            }
        }

        private void RestID_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.Posi > 0)
            {
                int oldPosi = 1;
                int newPosi = 0;
                oldPosi = selectedCloth.Posi;
                newPosi = selectedCloth.Posi - 1;

                ClothData oldCloth = clothes[oldPosi];
                ClothData newCloth = clothes[newPosi];

                oldCloth.Posi = newPosi;
                newCloth.Posi = oldPosi;

                clothes[newPosi] = oldCloth;
                clothes[oldPosi] = newCloth;

                ID.Text = newPosi.ToString();

                ProjectController.Instance().UpdateClothesList();
            }
        }

        private void PlusID_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.Posi < clothes.Count - 1) {
                int oldPosi = 1;
                int newPosi = 0;
                oldPosi = selectedCloth.Posi;
                newPosi = selectedCloth.Posi + 1;

                ClothData oldCloth = clothes[oldPosi];
                ClothData newCloth = clothes[newPosi];

                oldCloth.Posi = newPosi;
                newCloth.Posi = oldPosi;

                clothes[newPosi] = oldCloth;
                clothes[oldPosi] = newCloth;

                ID.Text = newPosi.ToString();

                ProjectController.Instance().UpdateClothesList();
            }
        }

        private void ClothesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allListBox.Items.Count > 0 && !updating)
            {
                selectedCloth = (ClothData)allListBox.SelectedItem;
                UpdateWindows();
            }
        }

        private void UpdateWindows()
        {
            if (selectedCloth != null && !updating)
            {
                updating = true;
                editGroupBox.Visibility = Visibility.Visible;
                clothEditWindow.Visibility = Visibility.Visible;

                UpdateTypeList();

                if (selectedCloth.IsComponent())
                {
                    editGroupBox.Header = "Drawable Edit";
                    headerDrawableName.Header = "Item Name";

                    drawableName.Text = selectedCloth.Name;

                    texturesList.ItemsSource = selectedCloth.textures;
                    fpModelPath.Text = selectedCloth.fpModelPath != "" ? selectedCloth.fpModelPath : "Not selected...";

                    unkFlag1Check.IsChecked = selectedCloth.componentFlags.unkFlag1;
                    unkFlag2Check.IsChecked = selectedCloth.componentFlags.unkFlag2;
                    unkFlag3Check.IsChecked = selectedCloth.componentFlags.unkFlag3;
                    unkFlag4Check.IsChecked = selectedCloth.componentFlags.unkFlag4;

                    isHighHeelsCheck.IsChecked = selectedCloth.componentFlags.isHighHeels;
                    isReskinCheck.IsChecked = selectedCloth.isReskin;
                    ID.Text = selectedCloth.Posi.ToString();

                    isHighHeelsCheck.Visibility = Visibility.Visible;
                    isReskinCheck.Visibility = Visibility.Visible;
                    FPSHeader.Visibility = Visibility.Visible;
                    highHeelsNumberText.Visibility = Visibility.Visible;
                    unkFlag5Check.Visibility = Visibility.Hidden;
                }
                else
                {
                    editGroupBox.Header = "Ped Prop Edit";
                    headerDrawableName.Header = "Ped Prop Name";

                    drawableName.Text = selectedCloth.Name;

                    texturesList.ItemsSource = selectedCloth.textures;

                    unkFlag1Check.IsChecked = selectedCloth.pedPropFlags.unkFlag1;
                    unkFlag2Check.IsChecked = selectedCloth.pedPropFlags.unkFlag2;
                    unkFlag3Check.IsChecked = selectedCloth.pedPropFlags.unkFlag3;
                    unkFlag4Check.IsChecked = selectedCloth.pedPropFlags.unkFlag4;
                    unkFlag5Check.IsChecked = selectedCloth.pedPropFlags.unkFlag5;

                    ID.Text = selectedCloth.Posi.ToString();

                    unkFlag5Check.Visibility = Visibility.Visible;
                    isHighHeelsCheck.Visibility = Visibility.Hidden;
                    isReskinCheck.Visibility = Visibility.Hidden;
                    FPSHeader.Visibility = Visibility.Hidden;
                    highHeelsNumberText.Visibility = Visibility.Hidden;
                }

                UpdateSelection();

                updating = false;
            }
        }

        private void UpdateTypeList()
        {
            itemType.Items.Clear();

            foreach (ClothNameResolver.Type typ in Enum.GetValues(typeof(ClothNameResolver.Type)))
            {
                itemType.Items.Add(new ComboTypeItem(typ.ToString(), typ));
            }

            bool found = false;

            foreach (ComboTypeItem item in itemType.Items)
            {
                if (item.GetValue() == selectedCloth.clothType)
                {
                    found = true;
                    itemType.SelectedItem = item;
                    break;
                }
            }

            if (!found)
            {
                itemType.SelectedIndex = 0;
            }

            UpdateCategoryList();
        }

        private void UpdateCategoryList()
        {
            itemCategory.Items.Clear();

            if (selectedCloth.clothType == ClothNameResolver.Type.Component)
            {
                foreach (ClothNameResolver.DrawableType cat in Enum.GetValues(typeof(ClothNameResolver.DrawableType)))
                {
                    if (!cat.ToString().ToLower().Contains("prop") && cat.ToString().ToLower() != "count")
                    {
                        itemCategory.Items.Add(new ComboCategoryItem(cat.ToString() + " [" + ClothNameResolver.DrawableTypeToString(cat) + "]", cat));
                    }
                }
            }
            else
            {
                foreach (ClothNameResolver.DrawableType cat in Enum.GetValues(typeof(ClothNameResolver.DrawableType)))
                {
                    if (cat.ToString().ToLower().Contains("prop") && cat.ToString().ToLower() != "count")
                    {
                        itemCategory.Items.Add(new ComboCategoryItem(cat.ToString() + " [" + ClothNameResolver.DrawableTypeToString(cat) + "]", cat));
                    }
                }
            }

            bool found = false;

            foreach (ComboCategoryItem item in itemCategory.Items)
            {
                if (item.GetValue() == selectedCloth.drawableType)
                {
                    found = true;
                    itemCategory.SelectedItem = item;
                    break;
                }
            }

            if (!found)
            { 
                itemCategory.SelectedIndex = 0;

                ComboCategoryItem temp_ = (ComboCategoryItem)itemCategory.SelectedItem;
                selectedCloth.drawableType = temp_.GetValue();
            }
        }

        private void DrawableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCloth != null)
            {
                selectedCloth.Name = drawableName.Text;
            }
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            clothes.Clear();

            selectedCloth = null;

            clothEditWindow.Visibility = Visibility.Hidden;
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "altv-cloth.json";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    ProjectBuilder.LoadProject(filename);
                }
            }

            ProjectController.Instance().UpdateClothesList();
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.DefaultExt = "altv-cloth.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                foreach (string filename in saveFileDialog.FileNames)
                {
                    ProjectBuilder.BuildProject(filename);
                }
            }
        }

        private void AddTexture_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                ProjectController.Instance().AddTexture(selectedCloth);
        }

        private void RemoveTexture_Click(object sender, RoutedEventArgs e)
        {
            if (texturesList.SelectedItem != null)
            {
                ((ObservableCollection<string>)texturesList.ItemsSource).Remove((string)texturesList.SelectedItem);
            }
        }

        private void BuildProjectButton_Click(object sender, RoutedEventArgs e)
        {
            projectBuildWindow = new ProjectBuild();
            projectBuildWindow.Show();
        }

        private void UnkFlag1Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null)
            {
                return;
            }

            if (selectedCloth.IsComponent())
            {
                selectedCloth.componentFlags.unkFlag1 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
            else
            {
                selectedCloth.pedPropFlags.unkFlag1 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
        }

        private void UnkFlag2Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null)
            {
                return;
            }

            if (selectedCloth.IsComponent())
            {
                selectedCloth.componentFlags.unkFlag2 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
            else
            {
                selectedCloth.pedPropFlags.unkFlag2 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
        }

        private void UnkFlag3Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null)
            {
                return;
            }

            if (selectedCloth.IsComponent())
            {
                selectedCloth.componentFlags.unkFlag3 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
            else
            {
                selectedCloth.pedPropFlags.unkFlag3 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
        }

        private void UnkFlag4Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null)
            {
                return;
            }

            if (selectedCloth.IsComponent())
            {
                selectedCloth.componentFlags.unkFlag4 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }
            else
            {
                selectedCloth.pedPropFlags.unkFlag4 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
            }    
        }

        private void UnkFlag5Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth == null)
            {
                return;
            }

            if (!selectedCloth.IsComponent())
            {
                selectedCloth.pedPropFlags.unkFlag5 = unkFlag5Check.IsChecked.GetValueOrDefault(false);
            }
        }

        private void IsHighHeelsCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.IsComponent())
            {
                selectedCloth.componentFlags.isHighHeels = isHighHeelsCheck.IsChecked.GetValueOrDefault(false);
                highHeelsNumberText.IsEnabled = !highHeelsNumberText.IsEnabled;
            }
        }

        private void HighHeels_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(highHeelsNumberText.Text))
            {
                highHeelsNumberText.Text = "0";
            }
        }

        private void NumberValidationTextBox(object sender, KeyEventArgs e)
        {
            // Permitir sólo números, el punto y la tecla de retroceso.
            if ((e.Key < Key.D0 || e.Key > Key.D9) && e.Key != Key.Back && e.Key != Key.Decimal && e.Key != Key.OemPeriod)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(highHeelsNumberText.Text))
                {
                    highHeelsNumberText.Text = "0";
                }
                Keyboard.ClearFocus();
            }
        }

        private void ClearFocusKeyboard(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void HihgHeelsNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCloth == null || !selectedCloth.IsComponent())
            {
                return;
            }

            // Obtener el texto actual del TextBox.
            string text = (sender as TextBox).Text;

            // Validar el formato del número ingresado.

            bool notValid = false;

            if (text.Length >= 1 && !Regex.IsMatch(text[0].ToString(), @"^[0-9]$"))
            {
                notValid = true;
            }

            if (text.Length >= 2 && !Regex.IsMatch(text[1].ToString(), @"^\.$"))
            {
                notValid = true;
            }

            if (text.Length >= 3 && !Regex.IsMatch(text[2].ToString(), @"^[0-9]$"))
            {
                notValid = true;
            }

            if (text.Length > 3)
            {
                notValid = true;
            }


            if (notValid)
            {
                int caretIndex = (sender as TextBox).CaretIndex;
                (sender as TextBox).Text = text.Remove(caretIndex - 1, 1);
                (sender as TextBox).CaretIndex = caretIndex - 1;
                return;
            }

            if (selectedCloth != null && double.TryParse(text, out double result))
            {
                selectedCloth.highHeelsNumber = result;
            }
        }


        private void isReskinCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.IsComponent())
                selectedCloth.isReskin = isReskinCheck.IsChecked.GetValueOrDefault(false);
        }

        private void ClearFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.IsComponent())
            {
                selectedCloth.fpModelPath = "";
                fpModelPath.Text = "Not selected...";
            }
        }

        private void SelectFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null && selectedCloth.IsComponent())
            {
                ProjectController.Instance().SetFPModel(selectedCloth);
                fpModelPath.Text = selectedCloth.fpModelPath != "" ? selectedCloth.fpModelPath : "Not selected...";
            }
        }

        private void ViewOnlySex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allListBox == null) return;

            ComboBox cmb = sender as ComboBox;


            if (cmb.SelectedIndex == 0)
            {
                clothesList.Filter = null;
            }
            else if (cmb.SelectedIndex == 1)
            {
                clothesList.Filter = (item) => ((ClothData)item).targetSex == Sex.Male;
            }
            else if (cmb.SelectedIndex == 2)
            {
                clothesList.Filter = (item) => ((ClothData)item).targetSex == Sex.Female;
            }

            clothesList.Refresh();
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemType == null || selectedCloth == null || updating) return;

            ComboBox cmb = sender as ComboBox;
            ComboTypeItem targeType = cmb.SelectedItem as ComboTypeItem;

            if (targeType == null) return;

            selectedCloth.clothType = targeType.GetValue();

            ProjectController.Instance().UpdateClothesList();
            
            UpdateWindows();
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemCategory == null || selectedCloth == null || updating) return;

            ComboBox cmb = sender as ComboBox;
            ComboCategoryItem targeCategory = cmb.SelectedItem as ComboCategoryItem;

            if (targeCategory == null) return;

            selectedCloth.drawableType = targeCategory.GetValue();

            ProjectController.Instance().UpdateClothesList();

            UpdateWindows();
        }
    }
}
