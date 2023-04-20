using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AkinaRPTool.ClothData;
using static System.Net.Mime.MediaTypeNames;

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
        private static TextBlock statusTextBlock = null;
        private static ProgressBar statusProgress = null;
        public static ObservableCollection<ClothData> clothes;
        public static ObservableCollection<ClothData> maleClothes;
        public static ObservableCollection<ClothData> femaleClothes;
        private static ClothData selectedCloth = null;
        public static ProjectBuild projectBuildWindow = null;
        private static string appVersion = "v0.0.1";

        public MainWindow()
        {
            InitializeComponent();

            statusTextBlock = ((TextBlock)FindName("currentStatusBar"));
            statusProgress = ((ProgressBar)FindName("currentProgress"));

            clothes = new ObservableCollection<ClothData>();
            maleClothes = new ObservableCollection<ClothData>();
            femaleClothes = new ObservableCollection<ClothData>();

            foreach (var cloth in clothes)
            {
                if (cloth.targetSex == Sex.Male)
                {
                    maleClothes.Add(cloth);
                }
                else if (cloth.targetSex == Sex.Female)
                {
                    femaleClothes.Add(cloth);
                }
            }

            allListBox.ItemsSource = clothes;
            maleListBox.ItemsSource = maleClothes;
            femaleListBox.ItemsSource = femaleClothes;

            allListBox.Visibility = Visibility.Visible;
            maleListBox.Visibility = Visibility.Hidden;
            femaleListBox.Visibility = Visibility.Hidden;

            editGroupBox.Visibility = Visibility.Hidden;
            clothEditWindow.Visibility = Visibility.Hidden;
            //pedPropEditWindow.Visibility = Visibility.Hidden;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            appVersion = "[Alpha v" + fvi.FileVersion + "] AkinaRP Clothes Tool by TMMarkus";

            highHeelsNumberText.LostFocus += HighHeels_LostFocus;

            WindowMain.Title = appVersion;

            MessageBox.Show("This application is under development, this means that there may be errors and it may be unstable. If you see any errors, I encourage you to report them on my GitHub.\nhttps://github.com/TMMarkus/AkinaRP-Clothes-Tool\r\n\r\nFor now, only the \"clothing\" part is available, the \"props\" part is not yet developed.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void SetStatus(string status)
        {
            statusTextBlock.Text = status;
        }

        public static void SetProgress(double progress)
        {
            if (progress > 1)
                progress = 1;
            if (progress < 0)
                progress = 0;

            statusProgress.Value = statusProgress.Maximum * progress;
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar el menú en la posición del cursor del mouse
            Button boton = (Button)sender;
            boton.ContextMenu.PlacementTarget = boton;
            boton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            boton.ContextMenu.IsOpen = true;
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
                maleClothes.Clear();
                femaleClothes.Clear();

                foreach (var cloth in _clothes)
                {
                    if (cloth != selectedCloth)
                    {
                        clothes.Add(cloth);
                    }
                }

                foreach (var cloth in clothes)
                {
                    if (cloth.targetSex == Sex.Male)
                    {
                        maleClothes.Add(cloth);
                    }
                    else if (cloth.targetSex == Sex.Female)
                    {
                        femaleClothes.Add(cloth);
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
            if (e.AddedItems.Count > 0)
            {
                selectedCloth = (ClothData)e.AddedItems[0];

                if (selectedCloth != null)
                {
                    editGroupBox.Visibility = Visibility.Visible;
                    clothEditWindow.Visibility = Visibility.Hidden;
                    //pedPropEditWindow.Visibility = Visibility.Hidden;

                    if (selectedCloth.IsComponent())
                    {
                        clothEditWindow.Visibility = Visibility.Visible;
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
                    }
                    else
                    {
                        /*
                        pedPropEditWindow.Visibility = Visibility.Visible;
                        drawableName.Text = selectedCloth.Name;
                        pedPropName.Text = selectedCloth.Name;

                        pedPropTexturesList.ItemsSource = selectedCloth.textures;

                        pedPropFlag1.IsChecked = selectedCloth.pedPropFlags.unkFlag1;
                        pedPropFlag2.IsChecked = selectedCloth.pedPropFlags.unkFlag2;
                        pedPropFlag3.IsChecked = selectedCloth.pedPropFlags.unkFlag3;
                        pedPropFlag4.IsChecked = selectedCloth.pedPropFlags.unkFlag4;
                        pedPropFlag5.IsChecked = selectedCloth.pedPropFlags.unkFlag5;
                        */
                    }

                    UpdateTypeList();                    
                }
            }
        }

        private void UpdateTypeList()
        {
            itemType.Items.Clear();

            foreach (ClothNameResolver.Type typ in Enum.GetValues(typeof(ClothNameResolver.Type)))
            {
                if (typ == ClothNameResolver.Type.PedProp)
                {
                    itemType.Items.Add(new ComboTypeItem(typ.ToString() + " (Dont Work!)", typ));
                }
                else
                {
                    itemType.Items.Add(new ComboTypeItem(typ.ToString(), typ));
                }
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
            maleClothes.Clear();
            femaleClothes.Clear();

            selectedCloth = null;

            clothEditWindow.Visibility = Visibility.Hidden;
            //pedPropEditWindow.Visibility = Visibility.Hidden;
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
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag2Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag2 = unkFlag2Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag3Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag3 = unkFlag3Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag4Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag4 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
        }

        private void IsHighHeelsCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
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
            if (selectedCloth != null)
                selectedCloth.isReskin = isReskinCheck.IsChecked.GetValueOrDefault(false);
        }

        private void ClearFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.fpModelPath = "";
            fpModelPath.Text = "Not selected...";
        }

        private void SelectFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                ProjectController.Instance().SetFPModel(selectedCloth);
            fpModelPath.Text = selectedCloth.fpModelPath != "" ? selectedCloth.fpModelPath : "Not selected...";
        }

        /*
        private void PedPropName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCloth != null)
            {
                selectedCloth.Name = drawableName.Text;
            }
        }

        private void PedPropFlag1_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag2_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag2 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag3_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag3 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag4_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag4 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag5_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag5 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }
        */

        private void ViewOnlySex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listsItems == null) return;

            ComboBox cmb = sender as ComboBox;

            allListBox.Visibility = Visibility.Hidden;
            maleListBox.Visibility = Visibility.Hidden;
            femaleListBox.Visibility = Visibility.Hidden;

            if (cmb.SelectedIndex == 0)
            {
                allListBox.Visibility = Visibility.Visible;
            }
            else if (cmb.SelectedIndex == 1)
            {
                maleListBox.Visibility = Visibility.Visible;
            }
            else if (cmb.SelectedIndex == 2)
            {
                femaleListBox.Visibility = Visibility.Visible;
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemType == null || selectedCloth == null) return;

            ComboBox cmb = sender as ComboBox;
            ComboTypeItem targeType = cmb.SelectedItem as ComboTypeItem;

            if (targeType == null) return;

            selectedCloth.clothType = targeType.GetValue();
            ProjectController.Instance().UpdateClothesList();
            UpdateCategoryList();
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemCategory == null || selectedCloth == null) return;

            ComboBox cmb = sender as ComboBox;
            ComboCategoryItem targeCategory = cmb.SelectedItem as ComboCategoryItem;

            if (targeCategory == null) return;

            selectedCloth.drawableType = targeCategory.GetValue();
            ProjectController.Instance().UpdateClothesList();
        }
    }
}
