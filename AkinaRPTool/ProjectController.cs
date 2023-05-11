using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static AkinaRPTool.ClothData;
using static System.Net.Mime.MediaTypeNames;

namespace AkinaRPTool
{
    class ProjectController
    {
        static ProjectController singleton = null;
        static bool areReskin = false;
        static ArrayList omitedFiles = new ArrayList();
        static ArrayList propFiles = new ArrayList();

        public static ProjectController Instance()
        {
            if (singleton == null)
                singleton = new ProjectController();
            return singleton;
        }

        public void AddFolder(Sex targetSex)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            areReskin = false;
            omitedFiles.Clear();
            propFiles.Clear();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folder = folderBrowserDialog.SelectedPath;
                string[] files = Directory.GetFiles(folder, "*.ydd", SearchOption.AllDirectories);

                int totalFiles = files.Length;
                int currentFiles = 0;

                if (targetSex == Sex.All)
                {
                    totalFiles *= 2;
                }

                if (totalFiles > 126)
                {
                    var result = MessageBox.Show("More than 126 [" + totalFiles + "] items have been detected.\nIt is not recommended to create a pack with more than 126 items, as this causes clothing selection to not synchronize correctly.\n\nDo you still want to proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                if (files.Length > 0)
                {
                    foreach (string filename in files)
                    {
                        ImportPed(filename, targetSex);

                        if (targetSex == Sex.All)
                        {
                            currentFiles += 2;
                        }
                        else
                        {
                            currentFiles++;
                        }

                        MainWindow.Instance.currentProgress.Value = currentFiles * 100 / totalFiles;
                        MainWindow.Instance.currentProgress.Invalidate();
                        MainWindow.Instance.currentProgress.Update();
                    }

                    MainWindow.Instance.currentProgress.Value = 0;
                    MainWindow.Instance.currentProgress.Invalidate();
                    MainWindow.Instance.currentProgress.Update();

                    if (areReskin)
                    {
                        MessageBox.Show("You have imported some race clothing items, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (omitedFiles.Count > 0)
                    {
                        MessageBox.Show("Some items you have imported do not have textures, so they have been skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (propFiles.Count > 0)
                    {
                        MessageBox.Show("You have tried to import items of type \"prop\". These types of items are not yet supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void AddFiles(Sex targetSex)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes geometry (*.ydd)|*.ydd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ydd";
            openFileDialog.Multiselect = true;
            switch (targetSex)
            {
                case ClothData.Sex.All:
                    openFileDialog.Title = "Adding clothes for All sex";
                    break;
                case Sex.Male:
                    openFileDialog.Title = "Adding clothes for Male sex";
                    break;
                case Sex.Female:
                    openFileDialog.Title = "Adding clothes for Female sex";
                    break;
            }

            areReskin = false;
            omitedFiles.Clear();
            propFiles.Clear();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int currentFiles = 0;
                int totalFiles = openFileDialog.FileNames.Length;

                if (targetSex == Sex.All)
                {
                    totalFiles *= 2;
                }

                if (totalFiles > 126)
                {
                    var result = MessageBox.Show("More than 126 [" + totalFiles + "] items have been detected.\nIt is not recommended to create a pack with more than 126 items, as this causes clothing selection to not synchronize correctly.\n\nDo you still want to proceed?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                foreach (string filename in openFileDialog.FileNames)
                {
                    ImportPed(filename, targetSex);

                    if (targetSex == Sex.All)
                    {
                        currentFiles += 2;
                    }
                    else
                    {
                        currentFiles++;
                    }

                    MainWindow.Instance.currentProgress.Value = currentFiles * 100 / totalFiles;
                    MainWindow.Instance.currentProgress.Invalidate();
                    MainWindow.Instance.currentProgress.Update();
                }

                MainWindow.Instance.currentProgress.Value = 0;
                MainWindow.Instance.currentProgress.Invalidate();
                MainWindow.Instance.currentProgress.Update();

                if (areReskin)
                {
                    MessageBox.Show("You have imported some race clothing items, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (omitedFiles.Count > 0)
                {
                    MessageBox.Show("Some items you have imported do not have textures, so they have been skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (propFiles.Count > 0)
                {
                    MessageBox.Show("You have tried to import items of type \"prop\". These types of items are not yet supported.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ImportPed(string filename, Sex targetSex)
        {
            string baseFileName = Path.GetFileName(filename);

            if (baseFileName.EndsWith(".ydd"))
            {
                ClothNameResolver cData = new ClothNameResolver(baseFileName);

                if (!cData.isVariation)
                {
                        int loops = 1;

                        if (targetSex == Sex.All)
                        {
                            targetSex = Sex.Male;
                            loops = 2;
                        }

                        for (int i = 0; i < loops; i++)
                        {
                            int newPosi = 0;

                            if (MainWindow.clothes.Count > 0)
                            {
                                newPosi = MainWindow.clothes.Last().Posi + 1;
                            }

                            ClothData nextCloth = new ClothData(filename, cData.clothType, cData.drawableType, newPosi, cData.bindedNumber, cData.postfix, targetSex);

                            if (nextCloth.drawableType == ClothNameResolver.DrawableType.Accessories || nextCloth.mainPath.EndsWith("_r.ydd"))
                            {
                                nextCloth.isReskin = true;

                                areReskin = true;
                            }

                            nextCloth.SearchForTextures();

                            if (cData.clothType == ClothNameResolver.Type.Component)
                            {
                                nextCloth.SearchForFPModel();                              
                                StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                            }

                            if (nextCloth.textures.Count > 0)
                            {
                                UpdateClothesList(nextCloth);
                            }
                            else
                            {
                                omitedFiles.Add(filename);
                                MessageBox.Show("This file don't have textures:\n" + filename, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }  

                            targetSex = Sex.Female;
                        }

                        StatusController.SetStatus("Items added. Total: " + MainWindow.clothes.Count);
                    
                }
                else
                {
                    StatusController.SetStatus("Item " + baseFileName + " can't be added. Looks like it's variant of another item");
                }
            }
        }

        public void AddTexture(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes texture (*.ytd)|*.ytd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ytd";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (filename.EndsWith(".ytd"))
                    {
                        cloth.AddTexture(filename);
                    }
                }
            }
        }

        public void SetFPModel(ClothData cloth)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Clothes drawable (*.ydd)|*.ydd";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "ydd";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (filename.EndsWith(".ydd"))
                    {
                        cloth.SetFPModel(filename);
                    }
                }
            }
        }

        public void UpdateClothesList(ClothData nextCloth = null)
        {
            var _clothes = MainWindow.clothes.ToList();

            if (nextCloth != null)
            {
                _clothes.Add(nextCloth);
            }

            int newID = 0;

            foreach (var item in _clothes)
            {
                item.Posi = newID;
                newID++;
            }

            _clothes = _clothes.OrderBy(x => x.posi).ToList();

            MainWindow.clothes.Clear();

            foreach (var cloth in _clothes)
            {
                MainWindow.clothes.Add(cloth);
            }
        }
    }
}
