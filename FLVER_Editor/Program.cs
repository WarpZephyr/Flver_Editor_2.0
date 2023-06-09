using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using SoulsFormats;

namespace FLVER_Editor
{
    public class VertexInfo
    {
        public int meshIndex;
        public int vertexIndex;
    }

    internal static partial class Program
    {
        public static FLVER2 flver2;
        public static TPF tpf = null;
        public static List<VertexInfo> verticesInfo = new List<VertexInfo>();
        public static Dictionary<string, string> boneParentList;
        public static List<FLVER.Vertex> vertices = new List<FLVER.Vertex>();
        public static string filePath = "";
        public static Vector3 checkingPoint;
        public static Vector3 checkingPointNormal;
        public static bool useCheckingPoint;
        public static bool loadTexture = true;
        public static int boneFindParentTimes = 15;
        public static bool boneDisplay = true;
        public static bool dummyDisplay = true;
        public static bool setVertexPos = false;
        public static float setVertexX = 0;
        public static float setVertexY = 1.75f;
        public static float setVertexZ = 0;
        public static MainWindow window;
        public static string windowTitle = "FLVER Editor";

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            MainWindow.arguments = args.ToList();
            window = new MainWindow();
            window.Text = windowTitle;
            window.ShowDialog();
        }

        private static int FindBoneIndexByName(FLVER2 flverFile, string name)
        {
            for (var i = 0; i < flverFile.Bones.Count; ++i)
                if (flverFile.Bones[i].Name == name)
                    return i;
            return -1;
        }

        public static void SetMaterialPath(FLVER2.Material material, string typeName, string newPath)
        {
            foreach (FLVER2.Texture t in material.Textures.Where(t => t.Type == typeName))
            {
                t.Path = newPath;
                return;
            }
            var scale = new Vector2(1.0f, 1.0f);

            var newTexture = new FLVER2.Texture { Type = typeName, Path = newPath, Scale = scale, Unk10 = 1, Unk11 = true };
            material.Textures.Add(newTexture);
        }

        public static string FilterInvalidPathChars(string path)
        {
            return string.Concat(path.Split(Path.GetInvalidPathChars()));
        }

        public static FLVER2.Material GetBaseMaterial(string albedoPath = null, string metallicPath = null, string normalPath = null)
        {
            var baseMaterial = new FLVER2.Material("", "C[AMSN]_e.mtd", 390);
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_2_AlbedoMap_0",
                albedoPath != null ? Path.GetFileNameWithoutExtension(FilterInvalidPathChars(albedoPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_0_MetallicMap_0",
                metallicPath != null ? Path.GetFileNameWithoutExtension(FilterInvalidPathChars(metallicPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_7_NormalMap_4",
                normalPath != null ? Path.GetFileNameWithoutExtension(FilterInvalidPathChars(normalPath)) + ".tif" : "");
            return baseMaterial;
        }
    }
}