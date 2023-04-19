//using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static AkinaRPTool.ClothData;

namespace AkinaRPTool
{
    class ProjectController
    {
        static ProjectController singleton = null;
        static bool WarnShowed = false;
        public static ProjectController Instance()
        {
            if (singleton == null)
                singleton = new ProjectController();
            return singleton;
        }

        public void ShowFolderSelection(ClothData.Sex targetSex)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            WarnShowed = false;

            // Mostrar el diálogo para que el usuario seleccione una carpeta
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string carpeta = folderBrowserDialog.SelectedPath; // Obtener la carpeta seleccionada
                FolderRecursive(targetSex, carpeta);
            }
        }

        public void FolderRecursive(ClothData.Sex targetSex, string filepath)
        {
            // Comprobar si la ruta seleccionada es un archivo
            if (File.Exists(filepath))
            {
                return;
            }
            // Comprobar si la ruta es una carpeta
            else if (Directory.Exists(filepath))
            {
                // Obtener la lista de archivos y subdirectorios en la carpeta actual
                string[] files = Directory.GetFiles(filepath);
                string[] subDirectories = Directory.GetDirectories(filepath);

                // Recorrer los subdirectorios y llamar a la función BuscarArchivos de manera recursiva
                foreach (string subDirectory in subDirectories)
                {
                    FolderRecursive(targetSex, subDirectory);
                }
                // Recorrer los archivos en la carpeta actual y mostrarlos en la consola
                foreach (string filename in files)
                {
                    string baseFileName = Path.GetFileName(filename);

                    if (baseFileName.EndsWith(".ydd"))
                    {
                        ClothNameResolver cData = new ClothNameResolver(baseFileName);

                        if (!cData.isVariation)
                        {
                            if (cData.clothType == ClothNameResolver.Type.PedProp)
                            {
                                MessageBox.Show("You can't import a 'prop' item.\nThis freature isn't yet developed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (targetSex == ClothData.Sex.All)
                                {
                                    
                                    ClothData.Sex newTarget = ClothData.Sex.Male;

                                    for (int i = 0; i < 2; i++)
                                    {
                                        int newPosi = 0;

                                        if (MainWindow.clothes.Count > 0)
                                        {
                                            newPosi = MainWindow.clothes.Last().Posi + 1;
                                        }

                                        ClothData nextCloth = new ClothData(filename, cData.clothType, cData.drawableType, newPosi, cData.bindedNumber, cData.postfix, newTarget);

                                        if (nextCloth.drawableType == ClothNameResolver.DrawableType.Accessories || nextCloth.mainPath.EndsWith("_r.ydd"))
                                        {
                                            nextCloth.isReskin = true;

                                            if (!WarnShowed)
                                            {
                                                WarnShowed = true;
                                                MessageBox.Show("You have imported a race clothing item, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            }
                                        }

                                        if (cData.clothType == ClothNameResolver.Type.Component)
                                        {
                                            nextCloth.SearchForFPModel();
                                            nextCloth.SearchForTextures();

                                            UpdateClothesList(nextCloth);

                                            StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                        }
                                        else
                                        {
                                            nextCloth.SearchForTextures();

                                            UpdateClothesList(nextCloth);

                                            StatusController.SetStatus(nextCloth.ToString() + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                        }

                                        newTarget = ClothData.Sex.Female;
                                    }

                                    StatusController.SetStatus("Items added. Total: " + MainWindow.clothes.Count);
                                }
                                else
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
                                        if (!WarnShowed)
                                        {
                                            WarnShowed = true;
                                            MessageBox.Show("You have imported a race clothing item, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }

                                    if (cData.clothType == ClothNameResolver.Type.Component)
                                    {
                                        nextCloth.SearchForFPModel();
                                        nextCloth.SearchForTextures();

                                        UpdateClothesList(nextCloth);

                                        StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                    }
                                    else
                                    {
                                        nextCloth.SearchForTextures();

                                        UpdateClothesList(nextCloth);

                                        StatusController.SetStatus(nextCloth.ToString() + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                    }
                                }

                            }
                        }
                        else
                        {
                            StatusController.SetStatus("Item " + baseFileName + " can't be added. Looks like it's variant of another item");
                        }
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

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                WarnShowed = false;

                foreach (string filename in openFileDialog.FileNames)
                {
                    string baseFileName = Path.GetFileName(filename);
                    ClothNameResolver cData = new ClothNameResolver(baseFileName);

                    if (!cData.isVariation)
                    {
                        if (cData.clothType == ClothNameResolver.Type.PedProp)
                        {
                            MessageBox.Show("You can't import a 'prop' item.\nThis freature isn't yet developed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (targetSex == ClothData.Sex.All)
                            {
                                ClothData.Sex newTarget = ClothData.Sex.Male;

                                for (int i = 0; i < 2; i++)
                                {
                                    int newPosi = 0;

                                    if (MainWindow.clothes.Count > 0)
                                    {
                                        newPosi = MainWindow.clothes.Last().Posi + 1;
                                    }

                                    ClothData nextCloth = new ClothData(filename, cData.clothType, cData.drawableType, newPosi, cData.bindedNumber, cData.postfix, newTarget);

                                    if (nextCloth.drawableType == ClothNameResolver.DrawableType.Accessories || nextCloth.mainPath.EndsWith("_r.ydd"))
                                    {
                                        nextCloth.isReskin = true;

                                        if (!WarnShowed)
                                        {
                                            WarnShowed = true;
                                            MessageBox.Show("You have imported a race clothing item, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }

                                    if (cData.clothType == ClothNameResolver.Type.Component)
                                    {
                                        nextCloth.SearchForFPModel();
                                        nextCloth.SearchForTextures();

                                        UpdateClothesList(nextCloth);

                                        StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                    }
                                    else
                                    {
                                        nextCloth.SearchForTextures();

                                        UpdateClothesList(nextCloth);

                                        StatusController.SetStatus(nextCloth.ToString() + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                    }

                                    newTarget = ClothData.Sex.Female;
                                }

                                StatusController.SetStatus("Items added. Total: " + MainWindow.clothes.Count);
                            }
                            else
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

                                    if (!WarnShowed)
                                    {
                                        WarnShowed = true;
                                        MessageBox.Show("You have imported a race clothing item, so the 'Skin Tone?' option has been activated. This will export the clothing item with the '_r' prefix.\n\nIf it does not display correctly in GTA V, try disabling this option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }

                                }

                                if (cData.clothType == ClothNameResolver.Type.Component)
                                {
                                    nextCloth.SearchForFPModel();
                                    nextCloth.SearchForTextures();

                                    UpdateClothesList(nextCloth);

                                    StatusController.SetStatus(nextCloth.ToString() + " added (FP model found: " + (nextCloth.fpModelPath != "" ? "Yes" : "No") + ", Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                }
                                else
                                {
                                    nextCloth.SearchForTextures();

                                    UpdateClothesList(nextCloth);

                                    StatusController.SetStatus(nextCloth.ToString() + " added, Textures: " + (nextCloth.textures.Count) + "). Total: " + MainWindow.clothes.Count);
                                }
                            }
                        }
                    }
                    else
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
            MainWindow.maleClothes.Clear();
            MainWindow.femaleClothes.Clear();

            foreach (var cloth in _clothes)
            {
                MainWindow.clothes.Add(cloth);
            }

            foreach (var cloth in MainWindow.clothes)
            {
                if (cloth.targetSex == Sex.Male)
                {
                    MainWindow.maleClothes.Add(cloth);
                }
                else if (cloth.targetSex == Sex.Female)
                {
                    MainWindow.femaleClothes.Add(cloth);
                }
            }
        }
    }
}
