using CodeWalker.GameFiles;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AkinaRPTool
{
    public class ClothData : INotifyPropertyChanged
    {
        public ClothNameResolver.Type clothType;
        public ClothNameResolver.DrawableType drawableType;
        public ClothNameResolver.PropRenderFlag propRenderFlag = ClothNameResolver.PropRenderFlag.NONE;

        public struct ComponentFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool isHighHeels;
        }

        public struct PedPropFlags
        {
            public bool unkFlag1;
            public bool unkFlag2;
            public bool unkFlag3;
            public bool unkFlag4;
            public bool unkFlag5;
        }

        public enum Sex
        {
            Male,
            Female,
            All
        }

        static int[] idsOffset = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static char offsetLetter = 'a';
        static string[] sexIcons = { "👨🏻", "👩🏻", "⚥" };
        static string[] typeIcons = { "🧥", "👓" };
        static string[] categoryIcons = { "👨‍🦲", "👺", "‍✂️", " 🧍", "👖  ", "🎒", "👟", "💍", "👕", "🛡️ ", " 🉐", "🧥 ",
                                           "👓", "👓", "👓", "👓", "👓", "👓", "👓", "👓", "👓", "👓", "👓", "👓",
                                           "👓", "👓" };

        public string mainPath = "";
        string origNumerics = "";
        string postfix = "";

        public ComponentFlags componentFlags;
        public PedPropFlags pedPropFlags;

        public string fpModelPath;
        public ObservableCollection<string> textures = new ObservableCollection<string>();

        public Sex targetSex;

        public string name = "";
        public int posi = 0;
        public double highHeelsNumber = 1;
        public bool isReskin = false;

        public string Icon
        {
            get
            {
                return sexIcons[(int)targetSex];
            }
        }

        public string Type
        {
            get
            {
                return typeIcons[(int)clothType];
            }
        }

        public string Category
        {
            get
            {
                return categoryIcons[(int)drawableType];
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Posi
        {
            get
            {
                return posi;
            }
            set
            {
                posi = value;
                OnPropertyChanged("Posi");
            }
        }

        public ClothData()
        {

        }

        public ClothData(string path, ClothNameResolver.Type _type, ClothNameResolver.DrawableType _drawableType, int _posi, string numeric, string _postfix, Sex sex)
        {
            if (!File.Exists(path))
                throw new Exception("YDD file not found");

            clothType = _type;
            drawableType = _drawableType;
            origNumerics = numeric;

            posi = _posi;

            targetSex = sex;
            postfix = _postfix;

            name = drawableType.ToString() + "_" + origNumerics;

            mainPath = path;
        }

        public void SearchForFPModel()
        {
            string rootPath = Path.GetDirectoryName(mainPath);
            string fileName = Path.GetFileNameWithoutExtension(mainPath);
            string relPath = rootPath + "\\" + fileName + "_1.ydd";
            if (File.Exists(relPath))
                fpModelPath = relPath;
            else
                fpModelPath = "";
        }

        public void SetFPModel(string path)
        {
            fpModelPath = path;
        }

        public void SearchForTextures()
        {
            string rootPath = Path.GetDirectoryName(mainPath);
            string[] files = Directory.GetFiles(rootPath);

            textures.Clear();

            if (IsComponent())
            {
                int index = 0;
                foreach (string file_ in files)
                {
                    foreach (string file in files)
                    {
                        string drawType = ClothNameResolver.DrawableTypeToString(drawableType).ToLower();
                        string drawTypeColoquial = drawableType.ToString().ToLower();
                        string noChange = "_diff_" + origNumerics + "_" + (char)(offsetLetter + index);

                        string tex_uni = drawType + noChange + "_uni.ytd";
                        string tex_uni_c = drawTypeColoquial + noChange + "_uni.ytd";
                        string tex_whi = drawType + noChange + "_whi.ytd";
                        string tex_whi_c = drawTypeColoquial + noChange + "_whi.ytd";

                        string fileLower = file.ToLower();

                        if (fileLower.EndsWith(tex_uni) || fileLower.EndsWith(tex_uni_c) || fileLower.EndsWith(tex_whi) || fileLower.EndsWith(tex_whi_c))
                        {
                            textures.Add(file);
                            index++;
                        }
                    }
                }
            }
            else
            {
                int index = 0;
                foreach (string file_ in files)
                {
                    foreach (string file in files)
                    {
                        string drawType = ClothNameResolver.DrawableTypeToString(drawableType).ToLower();
                        string drawTypeColoquial = drawableType.ToString().ToLower();
                        string noChange = "_diff_" + origNumerics + "_" + (char)(offsetLetter + index);

                        string tex = drawType + noChange + ".ytd";
                        string tex_c = drawTypeColoquial + noChange + ".ytd";

                        string fileLower = file.ToLower();

                        if (fileLower.EndsWith(tex) || fileLower.EndsWith(tex_c))
                        {
                            textures.Add(file);
                            index++;
                        }
                    }
                }
            }
        }

        public void AddTexture(string path)
        {
            if (!textures.Contains(path))
                textures.Add(path);
        }

        public override string ToString()
        {
            return sexIcons[(int)targetSex] + " " + name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool IsComponent()
        {
            if (drawableType <= ClothNameResolver.DrawableType.Top)
                return true;
            return false;
        }

        public byte GetComponentTypeID()
        {
            if (IsComponent())
                return (byte)drawableType;
            return 255;
        }

        public bool IsPedProp()
        {
            return !IsComponent();
        }

        public byte GetPedPropTypeID()
        {
            if (IsPedProp())
                return (byte)((int)drawableType - (int)ClothNameResolver.DrawableType.PropHead);
            return 255;
        }

        public string GetPrefix()
        {
            return ClothNameResolver.DrawableTypeToString(drawableType);
        }
    }
}
