using CodeWalker.GameFiles;
using RageLib.Archives;
using RageLib.GTA5.Archives;
using RageLib.GTA5.ArchiveWrappers;
using RageLib.GTA5.ResourceWrappers.PC.Meta.Structures;
using RageLib.Resources.GTA5.PC.GameFiles;
using RageLib.Resources.GTA5.PC.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using static AkinaRPTool.ClothData;
using static AkinaRPTool.ClothNameResolver;
using MCAnchorProps = RageLib.GTA5.ResourceWrappers.PC.Meta.Structures.MCAnchorProps;
using MCComponentInfo = RageLib.GTA5.ResourceWrappers.PC.Meta.Structures.MCComponentInfo;

namespace AkinaRPTool
{
    public static class ResourceBuilder
    {
        public static string GenerateShopMeta(Sex targetSex, string collectionName)
        {
            string targetName = targetSex == Sex.Male ? "mp_m_freemode_01" : "mp_f_freemode_01";
            string dlcName = (targetSex == Sex.Male ? "mp_m_" : "mp_f_") + collectionName;
            string character = (targetSex == Sex.Male ? "SCR_CHAR_MULTIPLAYER" : "SCR_CHAR_MULTIPLAYER_F");
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<ShopPedApparel>
    <pedName>{targetName}</pedName>
    <dlcName>{dlcName}</dlcName>
    <fullDlcName>{targetName}_{dlcName}</fullDlcName>
    <eCharacter>{character}</eCharacter>
    <creatureMetaData>MP_CreatureMetadata_{collectionName}</creatureMetaData>
    <pedOutfits>
    </pedOutfits>
    <pedComponents>
    </pedComponents>
    <pedProps>
    </pedProps>
</ShopPedApparel>";
        }

        public static string GenerateStreamCfg(List<string> files, List<string> metas)
        {
            string filesText = "";
            for (int i = 0; i < files.Count; ++i)
            {
                if (i != 0)
                    filesText += "\n";
                filesText += "  " + files[i];
            }

            string metasText = "";
            for (int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    metasText += "\n";
                metasText += "  " + metas[i];
            }

            return $"files: [\n{filesText}/*\n]\nmeta: {{\n{metasText}\n}}\n";
        }

        public static string GenerateResourceCfg()
        {
            return "type: dlc,\nmain: stream.cfg,\nclient-files: [\n  stream/*\n]\n";
        }

        public static string GenerateResourceLua(List<string> metas)
        {
            string filesText = "";
            for (int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    filesText += ",\n";
                filesText += "  '" + metas[i] + "'";
            }

            string metasText = "";
            for (int i = 0; i < metas.Count; ++i)
            {
                if (i != 0)
                    metasText += "\n";
                metasText += "data_file 'SHOP_PED_APPAREL_META_FILE' '" + metas[i] + "'";
            }

            string manifestContent = "-- AkinaRP Clothes Tool\n\n";
            manifestContent += "fx_version 'cerulean'\n";
            manifestContent += "game {'gta5'}\n\n";
            manifestContent += "files {\n" + filesText + "\n}\n\n" + metasText + "";
            return manifestContent;
        }

        public static string GenerateContentXML(string collectionName, bool hasMale, bool hasFemale, bool hasMaleProps, bool hasFemaleProps)
        {
            string str = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<CDataFileMgr__ContentsOfDataFileXml>
<disabledFiles />
<includedXmlFiles />
<includedDataFiles />
<dataFiles>
";

            if (hasMale)
            {
                str += $@"    <Item>
<filename>dlc_{collectionName}:/common/data/mp_m_freemode_01_mp_m_{collectionName}.meta</filename>
<fileType>SHOP_PED_APPAREL_META_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""false"" />
</Item>
<Item>
<filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male.rpf</filename>
<fileType>RPF_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""true"" />
</Item>
";
            }

            if (hasFemale)
            {
                str += $@"    <Item>
<filename>dlc_{collectionName}:/common/data/mp_f_freemode_01_mp_f_{collectionName}.meta</filename>
<fileType>SHOP_PED_APPAREL_META_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""false"" />
</Item>
<Item>
<filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female.rpf</filename>
<fileType>RPF_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""true"" />
</Item>
";
            }

            if (hasMaleProps)
            {
                str += $@"    <Item>
<filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male_p.rpf</filename>
<fileType>RPF_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""true"" />
</Item>
";
            }

            if (hasFemaleProps)
            {
                str += $@"    <Item>
<filename>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female_p.rpf</filename>
<fileType>RPF_FILE</fileType>
<overlay value=""false"" />
<disabled value=""true"" />
<persistent value=""true"" />
</Item>
";
            }

            str += $@"    </dataFiles>
<contentChangeSets>
<Item>
<changeSetName>{collectionName.ToUpper()}_AUTOGEN</changeSetName>
<mapChangeSetData />
<filesToInvalidate />
<filesToDisable />
<filesToEnable>
";
            if (hasMale)
            {
                str += $"    <Item>dlc_{collectionName}:/common/data/mp_m_freemode_01_mp_m_{collectionName}.meta</Item>\n";
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male.rpf</Item>\n";
            }

            if (hasFemale)
            {
                str += $"    <Item>dlc_{collectionName}:/common/data/mp_f_freemode_01_mp_f_{collectionName}.meta</Item>\n";
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female.rpf</Item>\n";
            }

            if (hasMaleProps)
            {
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_male_p.rpf</Item>\n";
            }

            if (hasFemaleProps)
            {
                str += $"    <Item>dlc_{collectionName}:/%PLATFORM%/models/cdimages/{collectionName}_female_p.rpf</Item>\n";
            }

            str += $@"      </filesToEnable>
</Item>
</contentChangeSets>
<patchFiles />
</CDataFileMgr__ContentsOfDataFileXml>";

            return str;
        }

        public static string GenerateSetup2XML(string collectionName, int order = 1000)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SSetupData>
    <deviceName>dlc_{collectionName}</deviceName>
    <datFile>content.xml</datFile>
    <timeStamp>07/07/2077 07:07:07</timeStamp>
    <nameHash>{collectionName}</nameHash>
    <contentChangeSets />
    <contentChangeSetGroups>
        <Item>
            <NameHash>GROUP_STARTUP</NameHash>
            <ContentChangeSets>
                <Item>{collectionName.ToUpper()}_AUTOGEN</Item>
            </ContentChangeSets>
        </Item>
    </contentChangeSetGroups>
    <startupScript />
    <scriptCallstackSize value=""0"" />
    <type>EXTRACONTENT_COMPAT_PACK</type>
    <order value=""1000"" />
    <minorOrder value=""0"" />
    <isLevelPack value=""false"" />
    <dependencyPackHash />
    <requiredVersion />
    <subPackCount value=""0"" />
</SSetupData>";
        }

        
        ///// OTHERS FORMATS FOR NOW ONLY FIVEM ////

        /*
        public static void BuildResourceAltv(string outputFolder, string collectionName)
        {
            List<string> streamCfgMetas = new List<string>();
            List<string> streamCfgIncludes = new List<string>();
            for (int sexNr = 0; sexNr < 2; ++sexNr)
            {
                //Male YMT generating
                YmtPedDefinitionFile ymt = new YmtPedDefinitionFile();

                ymt.metaYmtName = prefixes + collectionName;
                ymt.Unk_376833625.DlcName = RageLib.Hash.Jenkins.Hash(prefixes + collectionName);

                MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int[] propIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //ymt.Unk_376833625.Unk_1235281004 = 0;
                //ymt.Unk_376833625.Unk_4086467184 = 0;
                //ymt.Unk_376833625.Unk_911147899 = 0;
                //ymt.Unk_376833625.Unk_315291935 = 0;
                //ymt.Unk_376833625.Unk_2996560424 = ;

                bool isAnyClothAdded = false;
                bool isAnyPropAdded = false;

                foreach (ClothData cd in MainWindow.clothes)
                {
                    if (cd.IsComponent())
                    {

                        byte componentTypeID = cd.GetComponentTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            if (componentTextureBindings[componentTypeID] == null)
                                componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                            MUnk_1535046754 textureDescription = new MUnk_1535046754();

                            byte nextPropMask = 17;

                            switch (componentTypeID)
                            {
                                case 2:
                                case 7:
                                    nextPropMask = 11; break;
                                case 5:
                                case 8:
                                    nextPropMask = 65; break;
                                case 6:
                                case 9:
                                    nextPropMask = 1; break;
                                case 10:
                                    nextPropMask = 5; break;
                                case 11:
                                    nextPropMask = 1; break;
                                default:
                                    break;
                            }

                            byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                            string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                            string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";

                            if (cd.isReskin == true)
                            {
                                postfix = "r";
                                ytdPostfix = "whi";
                                nextPropMask = 17;
                            }

                            textureDescription.PropMask = nextPropMask;
                            textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                            foreach (string texPath in cd.textures)
                            {
                                MUnk_1036962405 texInfo = new MUnk_1036962405();
                                texInfo.Distribution = 255;
                                texInfo.TexId = texId;
                                textureDescription.ATexData.Add(texInfo);
                            }

                            textureDescription.ClothData.Unk_2828247905 = 0;

                            componentTextureBindings[componentTypeID].Unk_1756136273.Add(textureDescription);

                            byte componentTextureLocalId = (byte)(componentTextureBindings[componentTypeID].Unk_1756136273.Count - 1);

                            MCComponentInfo componentInfo = new MCComponentInfo();
                            componentInfo.Unk_802196719 = 0;
                            componentInfo.Unk_4233133352 = 0;
                            componentInfo.Unk_128864925.b0 = (byte)(cd.componentFlags.unkFlag1 ? 1 : 0);
                            componentInfo.Unk_128864925.b1 = (byte)(cd.componentFlags.unkFlag2 ? 1 : 0);
                            componentInfo.Unk_128864925.b2 = (byte)(cd.componentFlags.unkFlag3 ? 1 : 0);
                            componentInfo.Unk_128864925.b3 = (byte)(cd.componentFlags.unkFlag4 ? 1 : 0);
                            componentInfo.Unk_128864925.b4 = (byte)(cd.componentFlags.isHighHeels ? 1 : 0);
                            componentInfo.Flags = 0;
                            componentInfo.Inclusions = 0;
                            componentInfo.Exclusions = 0;
                            componentInfo.Unk_1613922652 = 0;
                            componentInfo.Unk_2114993291 = 0;
                            componentInfo.Unk_3509540765 = componentTypeID;
                            componentInfo.Unk_4196345791 = componentTextureLocalId;

                            targetYmt.Unk_376833625.CompInfos.Add(componentInfo);

                            if (!isAnyClothAdded)
                            {
                                isAnyClothAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes + "freemode_01_" + prefixes + collectionName);
                            }

                            int currentComponentIndex = componentIndexes[componentTypeID]++;

                            string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + postfix + ".ydd");

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd");

                            if (cd.fpModelPath != "")
                                File.Copy(cd.fpModelPath, outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd");
                        }
                    }
                    else
                    {
                        Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            YmtPedDefinitionFile targetYmt = ymt;

                            var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                            var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                            item.AnchorId = (byte)anchor;

                            for (int i = 0; i < cd.textures.Count; i++)
                            {
                                var texture = new MUnk_254518642();
                                texture.TexId = (byte)i;
                                item.TexData.Add(texture);
                            }

                            // Get or create linked anchor
                            var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                            if (aanchor == null)
                            {
                                aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                {
                                    Anchor = anchor
                                };

                                aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                            }
                            else
                            {
                                aanchor.PropsMap[item] = (byte)item.TexData.Count;
                            }

                            defs.Add(item);

                            if (!isAnyPropAdded)
                            {
                                isAnyPropAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes + "freemode_01_p_" + prefixes + collectionName);
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            var ydr = new YdrFile();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "\\" + prefix + "_" + componentNumerics + ".ydd", true);

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                            {
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + folderNames[sexNr] + "_p.rpf\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "\\" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd", true);
                            }
                        }
                    }
                }


                if (isAnyClothAdded)
                {

                    int arrIndex = 0;
                    for (int i = 0; i < componentTextureBindings.Length; ++i)
                    {
                        if (componentTextureBindings[i] != null)
                        {
                            byte id = (byte)(arrIndex++);
                            ymt.Unk_376833625.Unk_2996560424.SetByte(i, id);
                        }

                        ymt.Unk_376833625.Components[(Unk_884254308)i] = componentTextureBindings[i];
                    }
                }

                if (isAnyPropAdded)
                {
                    streamCfgIncludes.Add("stream/" + folderNames[sexNr] + "_p.rpf");
                }

                if (isAnyClothAdded || isAnyPropAdded)
                {
                    File.WriteAllText(outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName + ".meta", GenerateShopMeta((Sex)sexNr, collectionName));
                    streamCfgMetas.Add("stream/" + prefixes + "freemode_01_" + prefixes + collectionName + ".meta: SHOP_PED_APPAREL_META_FILE");
                    ymt.Save(outputFolder + "\\stream\\" + folderNames[sexNr] + ".rpf\\" + prefixes + "freemode_01_" + prefixes + collectionName + ".ymt");
                    streamCfgIncludes.Add("stream/" + folderNames[sexNr] + ".rpf");
                }

            }

            File.WriteAllText(outputFolder + "\\stream.cfg", GenerateStreamCfg(streamCfgIncludes, streamCfgMetas));
            File.WriteAllText(outputFolder + "\\resource.cfg", GenerateResourceCfg());

            MessageBox.Show("Resource built!");
        }

        public static void BuildResourceSingle(string outputFolder, string collectionName)
        {
            Utils.EnsureKeys();

            using (RageArchiveWrapper7 rpf = RageArchiveWrapper7.Create(outputFolder + @"\dlc.rpf"))
            {
                rpf.archive_.Encryption = RageArchiveEncryption7.NG;

                var dir = rpf.Root.CreateDirectory();
                dir.Name = "common";

                var dataDir = dir.CreateDirectory();
                dataDir.Name = "data";

                dir = rpf.Root.CreateDirectory();
                dir.Name = "x64";

                dir = dir.CreateDirectory();
                dir.Name = "models";

                var cdimagesDir = dir.CreateDirectory();
                cdimagesDir.Name = "cdimages";

                RageArchiveWrapper7 currComponentRpf = null;
                IArchiveDirectory currComponentDir = null;

                RageArchiveWrapper7 currPropRpf = null;
                IArchiveDirectory currPropDir = null;

                bool hasMale = false;
                bool hasFemale = false;
                bool hasMaleProps = false;
                bool hasFemaleProps = false;

                for (int sexNr = 0; sexNr < 2; ++sexNr)
                {
                    //Male YMT generating
                    YmtPedDefinitionFile ymt = new YmtPedDefinitionFile();

                    ymt.metaYmtName = prefixes + collectionName;
                    ymt.Unk_376833625.DlcName = RageLib.Hash.Jenkins.Hash(prefixes + collectionName);

                    MUnk_3538495220[] componentTextureBindings = { null, null, null, null, null, null, null, null, null, null, null, null };
                    int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    int[] propIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    //ymt.Unk_376833625.Unk_1235281004 = 0;
                    //ymt.Unk_376833625.Unk_4086467184 = 0;
                    //ymt.Unk_376833625.Unk_911147899 = 0;
                    //ymt.Unk_376833625.Unk_315291935 = 0;
                    //ymt.Unk_376833625.Unk_2996560424 = ;

                    bool isAnyClothAdded = false;
                    bool isAnyPropAdded = false;

                    foreach (ClothData cd in MainWindow.clothes)
                    {
                        if (cd.IsComponent())
                        {
                            byte componentTypeID = cd.GetComponentTypeID();

                            if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                            {
                                YmtPedDefinitionFile targetYmt = ymt;

                                if (componentTextureBindings[componentTypeID] == null)
                                    componentTextureBindings[componentTypeID] = new MUnk_3538495220();

                                MUnk_1535046754 textureDescription = new MUnk_1535046754();

                                byte nextPropMask = 17;

                                switch (componentTypeID)
                                {
                                    case 2:
                                    case 7:
                                        nextPropMask = 11; break;
                                    case 5:
                                    case 8:
                                        nextPropMask = 65; break;
                                    case 6:
                                    case 9:
                                        nextPropMask = 1; break;
                                    case 10:
                                        nextPropMask = 5; break;
                                    case 11:
                                        nextPropMask = 1; break;
                                    default:
                                        break;
                                }

                                byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                                string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                                string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";

                                if (cd.isReskin)
                                {
                                    postfix = "r";
                                    ytdPostfix = "whi";
                                    nextPropMask = 17;
                                }

                                textureDescription.PropMask = nextPropMask;
                                textureDescription.Unk_2806194106 = (byte)(cd.fpModelPath != "" ? 1 : 0);

                                foreach (string texPath in cd.textures)
                                {
                                    MUnk_1036962405 texInfo = new MUnk_1036962405();
                                    texInfo.Distribution = 255;
                                    texInfo.TexId = texId;
                                    textureDescription.ATexData.Add(texInfo);
                                }

                                textureDescription.ClothData.Unk_2828247905 = 0;

                                componentTextureBindings[componentTypeID].Unk_1756136273.Add(textureDescription);

                                byte componentTextureLocalId = (byte)(componentTextureBindings[componentTypeID].Unk_1756136273.Count - 1);

                                MCComponentInfo componentInfo = new MCComponentInfo();
                                componentInfo.Unk_802196719 = 0;
                                componentInfo.Unk_4233133352 = 0;
                                componentInfo.Unk_128864925.b0 = (byte)(cd.componentFlags.unkFlag1 ? 1 : 0);
                                componentInfo.Unk_128864925.b1 = (byte)(cd.componentFlags.unkFlag2 ? 1 : 0);
                                componentInfo.Unk_128864925.b2 = (byte)(cd.componentFlags.unkFlag3 ? 1 : 0);
                                componentInfo.Unk_128864925.b3 = (byte)(cd.componentFlags.unkFlag4 ? 1 : 0);
                                componentInfo.Unk_128864925.b4 = (byte)(cd.componentFlags.isHighHeels ? 1 : 0);
                                componentInfo.Flags = 0;
                                componentInfo.Inclusions = 0;
                                componentInfo.Exclusions = 0;
                                componentInfo.Unk_1613922652 = 0;
                                componentInfo.Unk_2114993291 = 0;
                                componentInfo.Unk_3509540765 = componentTypeID;
                                componentInfo.Unk_4196345791 = componentTextureLocalId;

                                targetYmt.Unk_376833625.CompInfos.Add(componentInfo);

                                if (!isAnyClothAdded)
                                {
                                    isAnyClothAdded = true;

                                    var ms = new MemoryStream();

                                    currComponentRpf = RageArchiveWrapper7.Create(ms, folderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf");
                                    currComponentRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                    currComponentDir = currComponentRpf.Root.CreateDirectory();
                                    currComponentDir.Name = prefixes + "freemode_01_" + prefixes + collectionName;
                                }

                                int currentComponentIndex = componentIndexes[componentTypeID]++;

                                string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                                string prefix = cd.GetPrefix();

                                var resource = currComponentDir.CreateResourceFile();
                                resource.Name = prefix + "_" + componentNumerics + "_" + postfix + ".ydd";
                                resource.Import(cd.mainPath);

                                char offsetLetter = 'a';

                                for (int i = 0; i < cd.textures.Count; ++i)
                                {
                                    resource = currComponentDir.CreateResourceFile();
                                    resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd";
                                    resource.Import(cd.textures[i]);
                                }

                                if (cd.fpModelPath != "")
                                {
                                    resource = currComponentDir.CreateResourceFile();
                                    resource.Name = prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd";
                                    resource.Import(cd.fpModelPath);
                                }

                            }
                        }
                        else
                        {
                            Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                            if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                            {
                                YmtPedDefinitionFile targetYmt = ymt;

                                var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                                var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                                item.AnchorId = (byte)anchor;

                                for (int i = 0; i < cd.textures.Count; i++)
                                {
                                    var texture = new MUnk_254518642();
                                    texture.TexId = (byte)i;
                                    item.TexData.Add(texture);
                                }

                                // Get or create linked anchor
                                var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                                if (aanchor == null)
                                {
                                    aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                    {
                                        Anchor = anchor
                                    };

                                    aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                    ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                                }
                                else
                                {
                                    aanchor.PropsMap[item] = (byte)item.TexData.Count;
                                }

                                defs.Add(item);

                                if (!isAnyPropAdded)
                                {
                                    isAnyPropAdded = true;

                                    var ms = new MemoryStream();

                                    currPropRpf = RageArchiveWrapper7.Create(ms, folderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf");
                                    currPropRpf.archive_.Encryption = RageArchiveEncryption7.NG;
                                    currPropDir = currPropRpf.Root.CreateDirectory();
                                    currPropDir.Name = prefixes + "freemode_01_p_" + prefixes + collectionName;
                                }

                                int currentPropIndex = propIndexes[(byte)anchor]++;

                                string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                                string prefix = cd.GetPrefix();

                                var resource = currPropDir.CreateResourceFile();
                                resource.Name = prefix + "_" + componentNumerics + ".ydd";
                                resource.Import(cd.mainPath);

                                char offsetLetter = 'a';
                                for (int i = 0; i < cd.textures.Count; ++i)
                                {
                                    resource = currPropDir.CreateResourceFile();
                                    resource.Name = prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd";
                                    resource.Import(cd.textures[i]);
                                }
                            }
                        }
                    }

                    if (isAnyClothAdded)
                    {
                        if (sexNr == 0)
                            hasMale = true;
                        else if (sexNr == 1)
                            hasFemale = true;

                        int arrIndex = 0;
                        for (int i = 0; i < componentTextureBindings.Length; ++i)
                        {
                            if (componentTextureBindings[i] != null)
                            {
                                byte id = (byte)(arrIndex++);
                                ymt.Unk_376833625.Unk_2996560424.SetByte(i, id);
                            }

                            ymt.Unk_376833625.Components[(Unk_884254308)i] = componentTextureBindings[i];
                        }
                    }

                    if (isAnyClothAdded || isAnyPropAdded)
                    {
                        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateShopMeta((Sex)sexNr, collectionName))))
                        {
                            var binFile = dataDir.CreateBinaryFile();
                            binFile.Name = prefixes + "freemode_01_" + prefixes + collectionName + ".meta";
                            binFile.Import(stream);
                        }
                        currComponentRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = folderNames[sexNr].Replace("ped_", collectionName + "_") + ".rpf";
                        binRpfFile.Import(currComponentRpf.archive_.BaseStream);

                        currComponentRpf.Dispose();
                    }

                    if (isAnyPropAdded)
                    {
                        if (sexNr == 0)
                            hasMaleProps = true;
                        else if (sexNr == 1)
                            hasFemaleProps = true;

                        currPropRpf.Flush();

                        var binRpfFile = cdimagesDir.CreateBinaryFile();
                        binRpfFile.Name = folderNames[sexNr].Replace("ped_", collectionName + "_") + "_p.rpf";
                        binRpfFile.Import(currPropRpf.archive_.BaseStream);

                        currPropRpf.Dispose();
                    }
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateContentXML(collectionName, hasMale, hasFemale, hasMaleProps, hasFemaleProps))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "content.xml";
                    binFile.Import(stream);
                }

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(GenerateSetup2XML(collectionName))))
                {
                    var binFile = rpf.Root.CreateBinaryFile();
                    binFile.Name = "setup2.xml";
                    binFile.Import(stream);
                }

                rpf.Flush();
                rpf.Dispose();

                MessageBox.Show("Resource built!");
            }
        }
        */

        /////////////////////////////////////////////

        public static void BuildResourceFivem(string outputFolder, string collectionName)
        {
            List<string> resourceLUAMetas = new List<string>();

            Sex newTargetSex = Sex.Male;

            for (int sexNr = 0; sexNr < 2; ++sexNr)
            {
                int[] componentIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                int[] propIndexes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                string prefixes = "";
                //string folderNames = "";

                if (newTargetSex == Sex.Male)
                {
                    prefixes = "mp_m_";
                    //folderNames = "ped_male";
                }
                else if (newTargetSex == Sex.Female)
                {
                    prefixes = "mp_f_";
                    //folderNames = "ped_female";
                }

                bool isAnyClothAdded = false;
                bool isAnyPropAdded = false;

                foreach (ClothData cd in MainWindow.clothes.Where(i => i.targetSex == newTargetSex))
                {
                    if (cd.IsComponent())
                    {
                        byte componentTypeID = cd.GetComponentTypeID();

                        if (cd.textures.Count > 0 && cd.targetSex == newTargetSex)
                        {
                            if (!isAnyClothAdded)
                            {
                                isAnyClothAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName);
                            }

                            byte texId = (byte)(cd.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                            string postfix = cd.mainPath.EndsWith("_u.ydd") ? "u" : "r";
                            string ytdPostfix = cd.mainPath.EndsWith("_u.ydd") ? "uni" : "whi";

                            if (cd.isReskin)
                            {
                                postfix = "r";
                                ytdPostfix = "whi";
                            }

                            int currentComponentIndex = componentIndexes[componentTypeID]++;
                            string componentNumerics = currentComponentIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            // COPY THE MODEL YDD
                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefixes + "freemode_01_" + prefixes + collectionName + "^" + prefix + "_" + componentNumerics + "_" + postfix + ".ydd", true);

                            // COPY TEXTURES
                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefixes + "freemode_01_" + prefixes + collectionName + "^" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + "_" + ytdPostfix + ".ytd", true);

                            // COPY FPS MODEL
                            if (cd.fpModelPath != "")
                                File.Copy(cd.fpModelPath, outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName + "\\" + prefixes + "freemode_01_" + prefixes + collectionName + "^" + prefix + "_" + componentNumerics + "_" + postfix + "_1.ydd", true);
                        }
                    }
                    else
                    {

                        Unk_2834549053 anchor = (Unk_2834549053)cd.GetPedPropTypeID();

                        if (cd.textures.Count > 0 && (int)cd.targetSex == sexNr)
                        {
                            //YmtPedDefinitionFile targetYmt = ymt;

                            //var defs = ymt.Unk_376833625.PropInfo.Props[anchor] ?? new List<MUnk_94549140>();
                            //var item = new MUnk_94549140(ymt.Unk_376833625.PropInfo);

                            //item.AnchorId = (byte)anchor;

                            /*
                            for (int i = 0; i < cd.textures.Count; i++)
                            {
                                var texture = new MUnk_254518642();
                                texture.TexId = (byte)i;
                                item.TexData.Add(texture);
                            }
                            

                            // Get or create linked anchor
                            var aanchor = ymt.Unk_376833625.PropInfo.AAnchors.Find(e => e.Anchor == anchor);

                            if (aanchor == null)
                            {
                                aanchor = new MCAnchorProps(ymt.Unk_376833625.PropInfo)
                                {
                                    Anchor = anchor
                                };

                                aanchor.PropsMap[item] = (byte)item.TexData.Count;

                                ymt.Unk_376833625.PropInfo.AAnchors.Add(aanchor);
                            }
                            else
                            {
                                aanchor.PropsMap[item] = (byte)item.TexData.Count;
                            }

                            defs.Add(item);
                            */

                            if (!isAnyPropAdded)
                            {
                                isAnyPropAdded = true;
                                Directory.CreateDirectory(outputFolder + "\\stream");
                                Directory.CreateDirectory(outputFolder + "\\stream\\" + prefixes + "freemode_01_p_" + prefixes + collectionName);
                            }

                            int currentPropIndex = propIndexes[(byte)anchor]++;

                            string componentNumerics = currentPropIndex.ToString().PadLeft(3, '0');
                            string prefix = cd.GetPrefix();

                            File.Copy(cd.mainPath, outputFolder + "\\stream\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "^" + prefix + "_" + componentNumerics + ".ydd", true);

                            char offsetLetter = 'a';
                            for (int i = 0; i < cd.textures.Count; ++i)
                            {
                                File.Copy(cd.textures[i], outputFolder + "\\stream\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "\\" + prefixes + "freemode_01_p_" + prefixes + collectionName + "^" + prefix + "_diff_" + componentNumerics + "_" + (char)(offsetLetter + i) + ".ytd", true);
                            }
                        }

                    }
                }

                if (isAnyClothAdded || isAnyPropAdded)
                {
                    string YMTfilePath = outputFolder + "\\stream\\" + prefixes + "freemode_01_" + prefixes + collectionName + ".ymt";
                    GenerateYMT(outputFolder, YMTfilePath, collectionName, newTargetSex);

                    File.WriteAllText(outputFolder + "\\" + prefixes + "freemode_01_" + prefixes + collectionName + ".meta", GenerateShopMeta(newTargetSex, collectionName));
                    resourceLUAMetas.Add(prefixes + "freemode_01_" + prefixes + collectionName + ".meta");
                }

                newTargetSex = Sex.Female;
            }

            if (MainWindow.clothes.Any(c => c.drawableType == DrawableType.Shoes) || MainWindow.clothes.Any(c => c.drawableType == DrawableType.PropHead))
            {
                string CreaturefilePath = outputFolder + "\\stream\\" + "mp_creaturemetadata_" + collectionName + ".ymt";
                SaveCreature(outputFolder, CreaturefilePath);
            }

            File.WriteAllText(outputFolder + "\\fxmanifest.lua", GenerateResourceLua(resourceLUAMetas));

            MessageBox.Show("Resource built!");
        }

        private static (int[], int) generateAvailComp()
        {
            int[] genAvailComp = { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            int compCount = 0;

            foreach (DrawableType avComp in Enum.GetValues(typeof(DrawableType)))
            {
                if(MainWindow.clothes.Any(i => i.drawableType == avComp) && avComp <= DrawableType.Top)
                {
                    genAvailComp[((int)avComp)] = compCount;
                    compCount++;
                }
            }

            return (genAvailComp, compCount);
        }

        private static int countAvailTex(int componentIndex)
        {
            int _textures = 0;

            foreach (ClothData comp in MainWindow.clothes.Where(i => i.clothType == ClothNameResolver.Type.Component && (int)i.drawableType == componentIndex))
            {
                _textures = _textures + comp.textures.Count;
            }

            return _textures % 256; //numAvailTex is byte -> (gunrunning female, uppr has 720 .ytd's and in .ymt its 208)
        }


        private static XElement YMTXML_Schema(string collectionName, Sex targetSex)
        {
            bool bHasTexVariations = false;
            bool bHasDrawblVariations = true;

            bool bHasLowLODs = false;
            bool bIsSuperLOD = false;

            XElement xml;

            // TOP OF FILE || START -> CPedVariationInfo
            if (targetSex == Sex.Male)
            {
                xml = new XElement("CPedVariationInfo", new XAttribute("name", "mp_m_" + collectionName));
            }
            else if(targetSex == Sex.Female)
            {
                xml = new XElement("CPedVariationInfo", new XAttribute("name", "mp_f_" + collectionName));
            }
            else
            {
                xml = new XElement("CPedVariationInfo");
            }

            xml.Add(new XElement("bHasTexVariations", new XAttribute("value", bHasTexVariations)));
            xml.Add(new XElement("bHasDrawblVariations", new XAttribute("value", bHasDrawblVariations)));
            xml.Add(new XElement("bHasLowLODs", new XAttribute("value", bHasLowLODs)));
            xml.Add(new XElement("bIsSuperLOD", new XAttribute("value", bIsSuperLOD)));

            (int[] availComp, int availCompCount) = generateAvailComp();
            xml.Add(new XElement("availComp", String.Join(" ", availComp)));

            // START -> aComponentData3
            XElement components = new XElement("aComponentData3", new XAttribute("itemType", "CPVComponentData"));

            for (int i = 0; i < availComp.Length; i++)
            {
                if (availComp[i] != 255)
                {
                    XElement compIndex = new XElement("Item");
                    compIndex.Add(new XElement("numAvailTex", new XAttribute("value", countAvailTex(i))));
                    XElement drawblData = new XElement("aDrawblData3", new XAttribute("itemType", "CPVDrawblData"));
                    compIndex.Add(drawblData);

                    foreach (ClothData item in MainWindow.clothes.Where(iCat => iCat.clothType == ClothNameResolver.Type.Component && (int)iCat.drawableType == i))
                    {
                        XElement drawblDataIndex = new XElement("Item");

                        byte componentTypeID = item.GetComponentTypeID();

                        int _propMask = 1;

                        switch (componentTypeID)
                        {
                            case 2:
                            case 7:
                                _propMask = 11;
                                break;
                            case 5:
                            case 8:
                                _propMask = 65;
                                break;
                            case 10:
                                _propMask = 5;
                                break;
                            default:
                                break;
                        }

                        if (item.isReskin)
                        {
                            _propMask = 17;
                        }

                        int _numAlternatives = 0; // Siempre y cuando se llegua a implementar algo asi.

                        drawblDataIndex.Add(new XElement("propMask", new XAttribute("value", _propMask)));
                        drawblDataIndex.Add(new XElement("numAlternatives", new XAttribute("value", _numAlternatives)));

                        XElement TexDataIndex = new XElement("aTexData", new XAttribute("itemType", "CPVTextureData"));

                        drawblDataIndex.Add(TexDataIndex);

                        
                        byte texId = (byte)(item.mainPath.EndsWith("_u.ydd") ? 0 : 1);
                        

                        foreach (string texPath in item.textures)
                        {
                            XElement TexDataItem = new XElement("Item");
                            TexDataItem.Add(new XElement("texId", new XAttribute("value", texId)));
                            TexDataItem.Add(new XElement("distribution", new XAttribute("value", 255)));
                            TexDataIndex.Add(TexDataItem);
                        }

                        XElement clothDataItem = new XElement("clothData");
                        bool _clothData = false; // Siempre y cuando no se acabe implementando

                        clothDataItem.Add(new XElement("ownsCloth", new XAttribute("value", _clothData)));
                        drawblDataIndex.Add(clothDataItem);
                        drawblData.Add(drawblDataIndex);
                    }

                    if (!drawblData.IsEmpty)
                    {
                        components.Add(compIndex);
                    }
                }
            }

            xml.Add(components);
            // END -> aComponentData3

            // START -> aSelectionSets
            xml.Add(new XElement("aSelectionSets", new XAttribute("itemType", "CPedSelectionSet"))); //never seen it used anywhere i think(?)
            // END -> aSelectionSets

            // START -> compInfos
            XElement compInfo = new XElement("compInfos", new XAttribute("itemType", "CComponentInfo"));

            foreach (ClothData cloth in MainWindow.clothes.Where(i => i.clothType == ClothNameResolver.Type.Component))
            {
                XElement compInfoItem = new XElement("Item");

                //string compType = "PV_COMP_" + cloth.GetPrefix().ToUpper(); // Algun dia se utilizara pero hoy no

                compInfoItem.Add(new XElement("hash_2FD08CEF", "none")); //not sure what it does
                compInfoItem.Add(new XElement("hash_FC507D28", "none")); //not sure what it does

                string[] highHeels = new string[] { "0", "0", "0", "0", "0" };

                if (cloth.componentFlags.isHighHeels && cloth.drawableType == DrawableType.Shoes)
                {
                    highHeels = new string[] { cloth.highHeelsNumber, "0", "0", "0", "0" };
                }

                compInfoItem.Add(new XElement("hash_07AE529D", String.Join(" ", highHeels)));  //component expressionMods (?) - gives ability to do heels
                compInfoItem.Add(new XElement("flags", new XAttribute("value", 0))); //not sure what it does
                compInfoItem.Add(new XElement("inclusions", "0")); //not sure what it does
                compInfoItem.Add(new XElement("exclusions", "0")); //not sure what it does
                compInfoItem.Add(new XElement("hash_6032815C", "PV_COMP_HEAD")); //No tengo ni puta idea para que sirbe
                compInfoItem.Add(new XElement("hash_7E103C8B", new XAttribute("value", 0))); //not sure what it does
                compInfoItem.Add(new XElement("hash_D12F579D", new XAttribute("value", cloth.GetComponentTypeID()))); //component id (jbib = 11, lowr = 4, etc)
                compInfoItem.Add(new XElement("hash_FA1F27BF", new XAttribute("value", cloth.posi))); // drawable index (000, 001, 002 etc)

                compInfo.Add(compInfoItem);
            }
            
            xml.Add(compInfo);
            // END -> compInfos

            // PROPS EXPORT MAYBE ONE DAY


            // START -> propInfo
            int numAvailPropsCount = 0;

            numAvailPropsCount = MainWindow.clothes.Where(i => i.clothType == ClothNameResolver.Type.PedProp).Count();

            XElement propInfo = new XElement("propInfo");
            propInfo.Add(new XElement("numAvailProps", new XAttribute("value", numAvailPropsCount % 256)));

            XElement aPropMetaData = new XElement("aPropMetaData", new XAttribute("itemType", "CPedPropMetaData"));

            foreach (ClothData prop in MainWindow.clothes.Where(i => i.clothType == ClothNameResolver.Type.PedProp))
            {
                XElement aPropMetaDataItem = new XElement("Item");
                aPropMetaDataItem.Add(new XElement("audioId", "none")); // Añade un sonido al prop (No implementado) (Audio ID)

                String[] expressionsMods = new string[] { "0", "0", "0", "0", "0" };

                if (prop.componentFlags.isHighHeels && prop.drawableType == DrawableType.PropHead)
                {
                    expressionsMods = new string[] { "0", "0", "0", "0", prop.highHeelsNumber };
                }

                aPropMetaDataItem.Add(new XElement("expressionMods", String.Join(" ", expressionsMods)));

                XElement texData = new XElement("texData", new XAttribute("itemType", "CPedPropTexData"));

                byte texId = (byte)(prop.mainPath.EndsWith("_u.ydd") ? 0 : 1);

                for (int i = 0; i < prop.textures.Count; i++)
                {
                    XElement texDataItem = new XElement("Item");
                    texDataItem.Add(new XElement("inclusions", "0")); // No se que hace
                    texDataItem.Add(new XElement("exclusions", "0")); // No se que hace
                    texDataItem.Add(new XElement("texId", new XAttribute("value", i))); // El indice que tiene la textura.
                    texDataItem.Add(new XElement("inclusionId", new XAttribute("value", "0"))); // No se que gace
                    texDataItem.Add(new XElement("exclusionId", new XAttribute("value", "0"))); // No se que hace
                    texDataItem.Add(new XElement("distribution", new XAttribute("value", 255))); // No se que hace
                    texData.Add(texDataItem);
                }
                aPropMetaDataItem.Add(texData);

                //check if there is renderFlags set, if not save it as "<renderFlags />" - earlier it was "<renderFlags></renderFlags>"
                if (prop.propRenderFlag != ClothNameResolver.PropRenderFlag.NONE)
                {
                    aPropMetaDataItem.Add(new XElement("renderFlags", prop.propRenderFlag.ToString()));
                }
                else
                {
                    aPropMetaDataItem.Add(new XElement("renderFlags"));
                }

                int anchor = (int)prop.drawableType - (int)ClothNameResolver.DrawableType.PropHead;
                int propIndex = 0;

                foreach (DrawableType type in Enum.GetValues(typeof(DrawableType)))
                {
                    if ((int)type == (int)prop.drawableType - (int)ClothNameResolver.DrawableType.PropHead)
                    {
                        foreach (ClothData item in MainWindow.clothes.Where(i => i.clothType == ClothNameResolver.Type.PedProp && i.drawableType == type))
                        {
                            if (item == prop)
                            {
                                break;
                            }

                            propIndex++;
                        }

                        break;
                    }
                }

                aPropMetaDataItem.Add(new XElement("propFlags", new XAttribute("value", "0"))); // No se que hace
                aPropMetaDataItem.Add(new XElement("flags", new XAttribute("value", "0"))); // No se que hace
                aPropMetaDataItem.Add(new XElement("anchorId", new XAttribute("value", anchor))); // El indice del item en su categoria
                aPropMetaDataItem.Add(new XElement("propId", new XAttribute("value", propIndex))); //propId is index of a prop in the same anchorid, so we can use index instead
                aPropMetaDataItem.Add(new XElement("hash_AC887A91", new XAttribute("value", "0"))); // No se quehace
                aPropMetaData.Add(aPropMetaDataItem);                
            }

            propInfo.Add(aPropMetaData);

            XElement aAnchors = new XElement("aAnchors", new XAttribute("itemType", "CAnchorProps"));
            foreach (DrawableType type in Enum.GetValues(typeof(DrawableType)))
            {
                if (MainWindow.clothes.Any(i => i.drawableType == type) && type > DrawableType.Top)
                {
                    XElement aAnchorsItem = new XElement("Item");
                    string[] props_tex_count = new string[MainWindow.clothes.Where(i => i.drawableType == type).Count()];

                    for (int i = 0; i < props_tex_count.Length; i++)
                    {
                        props_tex_count[i] = MainWindow.clothes.Where(i => i.drawableType == type).ToList()[i].textures.Count().ToString();
                    }

                    aAnchorsItem.Add(new XElement("props", String.Join(" ", props_tex_count)));

                    // PARECE QUE FUNCIONA PERO TENGO MIEDO
                    aAnchorsItem.Add(new XElement("anchor", "ANCHOR_" + PropTypeToString(type).ToUpper()));

                    aAnchors.Add(aAnchorsItem);
                }
            }

            propInfo.Add(aAnchors);

            xml.Add(propInfo);
            // END -> propInfo
           

            // dlcName Field

            string targetDLC = "";

            if (targetSex == Sex.Male)
            {
                targetDLC = "mp_m_";
            }
            else if(targetSex == Sex.Female)
            {
                targetDLC = "mp_f_";
            }
            else
            {
                targetDLC = "";
            }

            targetDLC += collectionName;

            XElement dlcNameField = new XElement("dlcName", targetDLC);
            xml.Add(dlcNameField);

            return xml;
            // END OF FILE || END -> CPedVariationInfo
        }

        private static XElement Creature_Schema()
        {
            XElement xml = new XElement("CCreatureMetaData");

            XElement pedCompExpressions = new XElement("pedCompExpressions");
            if (MainWindow.clothes.Where(c => c.drawableType == DrawableType.Shoes).Count() > 0)
            {
                /* lets test without first entry in components
                 * 
                //heels doesn't have that first entry but without it, fivem was sometimes crashing(?)
                XElement FirstpedCompItem = new XElement("Item");
                FirstpedCompItem.Add(new XElement("pedCompID", new XAttribute("value", String.Format("0x{0:X}", 6))));
                FirstpedCompItem.Add(new XElement("pedCompVarIndex", new XAttribute("value", String.Format("0x{0:X}", -1))));
                FirstpedCompItem.Add(new XElement("pedCompExpressionIndex", new XAttribute("value", String.Format("0x{0:X}", -1))));
                FirstpedCompItem.Add(new XElement("tracks", new XAttribute("content", "char_array"), 33));
                FirstpedCompItem.Add(new XElement("ids", new XAttribute("content", "short_array"), 28462));
                FirstpedCompItem.Add(new XElement("types", new XAttribute("content", "char_array"), 2));
                FirstpedCompItem.Add(new XElement("components", new XAttribute("content", "char_array"), 1));
                pedCompExpressions.Add(FirstpedCompItem);

                */

                foreach (ClothData comp in MainWindow.clothes.Where(c => c.drawableType == DrawableType.Shoes))
                {
                    XElement pedCompItem = new XElement("Item");
                    pedCompItem.Add(new XElement("pedCompID", new XAttribute("value", String.Format("0x{0:X}", 6))));
                    pedCompItem.Add(new XElement("pedCompVarIndex", new XAttribute("value", String.Format("0x{0:X}", comp.posi))));
                    pedCompItem.Add(new XElement("pedCompExpressionIndex", new XAttribute("value", String.Format("0x{0:X}", 4))));
                    pedCompItem.Add(new XElement("tracks", new XAttribute("content", "char_array"), 33));
                    pedCompItem.Add(new XElement("ids", new XAttribute("content", "short_array"), 28462));
                    pedCompItem.Add(new XElement("types", new XAttribute("content", "char_array"), 2));
                    pedCompItem.Add(new XElement("components", new XAttribute("content", "char_array"), 1));
                    pedCompExpressions.Add(pedCompItem);
                }
            }
            xml.Add(pedCompExpressions);

            XElement pedPropExpressions = new XElement("pedPropExpressions");
            if (MainWindow.clothes.Where(p => p.drawableType == DrawableType.PropHead).Count() > 0)
            {
                //all original GTA have that one first entry, without it, fivem was sometimes crashing(?)
                XElement FirstpedPropItem = new XElement("Item");
                FirstpedPropItem.Add(new XElement("pedPropID", new XAttribute("value", String.Format("0x{0:X}", 0))));
                FirstpedPropItem.Add(new XElement("pedPropVarIndex", new XAttribute("value", String.Format("0x{0:X}", -1))));
                FirstpedPropItem.Add(new XElement("pedPropExpressionIndex", new XAttribute("value", String.Format("0x{0:X}", -1))));
                FirstpedPropItem.Add(new XElement("tracks", new XAttribute("content", "char_array"), 33));
                FirstpedPropItem.Add(new XElement("ids", new XAttribute("content", "short_array"), 13201));
                FirstpedPropItem.Add(new XElement("types", new XAttribute("content", "char_array"), 2));
                FirstpedPropItem.Add(new XElement("components", new XAttribute("content", "char_array"), 1));
                pedPropExpressions.Add(FirstpedPropItem);

                foreach (ClothData prop in MainWindow.clothes.Where(p => p.drawableType == DrawableType.PropHead))
                {
                    XElement pedPropItem = new XElement("Item");
                    pedPropItem.Add(new XElement("pedPropID", new XAttribute("value", String.Format("0x{0:X}", 0))));
                    pedPropItem.Add(new XElement("pedPropVarIndex", new XAttribute("value", String.Format("0x{0:X}", prop.posi))));
                    pedPropItem.Add(new XElement("pedPropExpressionIndex", new XAttribute("value", String.Format("0x{0:X}", 0))));
                    pedPropItem.Add(new XElement("tracks", new XAttribute("content", "char_array"), 33));
                    pedPropItem.Add(new XElement("ids", new XAttribute("content", "short_array"), 13201));
                    pedPropItem.Add(new XElement("types", new XAttribute("content", "char_array"), 2));
                    pedPropItem.Add(new XElement("components", new XAttribute("content", "char_array"), 1));
                    pedPropExpressions.Add(pedPropItem);
                }
            }
            xml.Add(pedPropExpressions);

            return xml;
        }

        private static void GenerateYMT(string outputFolder, string YMTfilePath, string collectionName, Sex targetSex)
        {
            XElement xmlFile = YMTXML_Schema(collectionName, targetSex);

            xmlFile.Save(YMTfilePath);

            string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + xmlFile.ToString();

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(result);

            Meta meta = XmlMeta.GetMeta(xmlDocument);
            byte[] newYmtBytes = CodeWalker.GameFiles.ResourceBuilder.Build(meta, 2);

            File.WriteAllText(outputFolder + @"\ClothData_Debug.xml", result); // Only for Debug propouse.

            File.WriteAllBytes(YMTfilePath, newYmtBytes);
        }

        public static void SaveCreature(string outputFolder, string creaturePath)
        {
            XElement CreatureFile = Creature_Schema();

            CreatureFile.Save(creaturePath);

            string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + CreatureFile.ToString();

            //create XmlDocument from XElement
            var xmldoc = new XmlDocument();

            xmldoc.LoadXml(result);

            RbfFile rbf = XmlRbf.GetRbf(xmldoc);

            File.WriteAllText(outputFolder + @"\ClothData_CreatureMeta_Debug.xml", result); // Only for Debug propouse.

            File.WriteAllBytes(creaturePath, rbf.Save());
        }

    }
}
