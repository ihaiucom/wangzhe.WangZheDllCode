using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace CC.Runtime
{
    public partial class SeeAssetBundleInfoWindow : EditorWindow
    {

        [MenuItem("Game/Analyze/Save Mesh")]
        public static void SaveMesh()
        {
            GameObject go = Selection.activeGameObject;
            SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(SkinnedMeshRenderer render in skinnedMeshRenderers)
            {
                if (render.sharedMesh == null) continue;
                Mesh mesh = render.sharedMesh;
				if(!File.Exists("Assets/_Test/" + mesh.name + ".asset"))
				{
					Mesh newmesh =(Mesh) Mesh.Instantiate(mesh);
	                AssetDatabase.CreateAsset(newmesh, "Assets/_Test/" + mesh.name + ".asset");
				}

				foreach(Material material in render.materials)
				{
					if(!File.Exists("Assets/_Test/" + material.name.Replace(" (Instance)", "") + ".mat"))
						AssetDatabase.CreateAsset(material, "Assets/_Test/" + material.name.Replace(" (Instance)", "") + ".mat");

					if(material.mainTexture != null)
					{
						Texture2D t = (Texture2D)material.mainTexture;
//						t = (Texture2D) Texture2D.Instantiate(t);
//						Texture2D newTexture2D = ne
//						newTexture2D.SetPixels32(t.GetPixels32());
//						newTexture2D.Apply();
						//						var bytes = newTexture2D.EncodeToPNG();
//						AssetDatabase.CreateAsset(t, "Assets/_Test/" + t.name + "png.asset");
//						File.WriteAllBytes("Assets/_Test/" + material.mainTexture.name + ".png", bytes);

						Shader shader = Shader.Find("Sprites/Default");
						SaveRenderTextureToPNG(t, shader, "Assets/_Test/", t.name.Replace(" (Instance)", "") );
					}
				}
            }

			
			Animation[] animations = go.GetComponentsInChildren<Animation>();
			foreach (Animation animation in animations) 
			{
				foreach (AnimationState state in animation) 
				{
					if(state != null && state.clip != null)
					{
						AnimationClip clip = (AnimationClip) AnimationClip.Instantiate(state.clip);
						if(!File.Exists("Assets/_Test/" + clip.name.Replace("(Clone)", "") + ".anim"))
							AssetDatabase.CreateAsset(clip, "Assets/_Test/" + clip.name.Replace("(Clone)", "") + ".anim");
					}
				}

			}

        }

		
		public static bool SaveRenderTextureToPNG(Texture inputTex,Shader outputShader, string contents, string pngName)  
		{  
			RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);  
			Material mat = new Material(outputShader);  
			Graphics.Blit(inputTex, temp, mat);  
			bool ret = SaveRenderTextureToPNG(temp, contents,pngName);  
			RenderTexture.ReleaseTemporary(temp);  
			return ret;  
			
		}   
		
		//将RenderTexture保存成一张png图片  
		public static bool SaveRenderTextureToPNG(RenderTexture rt,string contents, string pngName)  
		{  
			RenderTexture prev = RenderTexture.active;  
			RenderTexture.active = rt;  
			Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);  
			png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);  
			byte[] bytes = png.EncodeToPNG();  
			if (!Directory.Exists(contents))  
				Directory.CreateDirectory(contents);  
			FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);  
			BinaryWriter writer = new BinaryWriter(file);  
			writer.Write(bytes);  
			file.Close();  
			Texture2D.DestroyImmediate(png);  
			png = null;  
			RenderTexture.active = prev;  
			return true;  
			
		}   

        public static SeeAssetBundleInfoWindow window;
        [MenuItem("Game/Analyze/查看AssetBundle")]

        public static void Open () 
        {
            window = EditorWindow.GetWindow <SeeAssetBundleInfoWindow>("查看AssetBundle");
            window.Show();
        }

        private List<string> ignoreExts = new List<string>(new string[] { ".meta", ".manifest" });
        private List<string> ignoreFiles = new List<string>(new string[] { ".ds_store" });
        public string assetbundleRoot = "C:/zengfeng/sdcard/com.gamesci.u1.hero.prod/files/patch/CommonAssets";
        public string assetbundleMain = "C:/zengfeng/sdcard/com.gamesci.u1.hero.prod/files/patch/CommonAssets/CommonAssets";

        public string[] assetbundleFiles = new string[] { };
        public string[] assetbundleFilenames = new string[] { };
        public Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>(); 

        AssetBundle assetBundle;
        AssetBundle assetBundleMain;
//        AssetBundleManifest assetBundleManifest;
        string[] assetNames;
        UnityEngine.Object[] assetObjcts;
        Type[] assetTypes;
        Type typeGameObject = typeof(GameObject);
        Type typeTexture2D = typeof(Texture2D);
        Type typeSprite = typeof(Sprite);
        int assetNameHeight = 200;
        string assetNameStr = "";

        Vector2 scrollPos;
        Vector2 infoScrollPos;


        void OnGUI ()
        {

            EditorGUILayout.BeginVertical();

            assetbundleMain = EditorGUILayout.TextField("Assetbundle Main:", assetbundleMain, GUILayout.ExpandWidth(true));


            if (GUILayout.Button("加载Main"))
            {
//                assetBundleMain = AssetBundle.LoadFromFile(assetbundleMain);
//                assetBundleManifest = assetBundleMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
//                string[] assetBundles = assetBundleManifest.GetAllAssetBundles();
//                Debug.Log(String.Join("\n", assetBundles));
            }

            GUILayout.Space(20);


            assetbundleRoot = EditorGUILayout.TextField("Assetbundle Root:", assetbundleRoot, GUILayout.ExpandWidth(true));


            if (GUILayout.Button("生成AssetBundle列表"))
            {
                assetbundleFiles = Directory.GetFiles(assetbundleRoot, "*.*", SearchOption.AllDirectories)
                .Where(s => !ignoreExts.Contains(Path.GetExtension(s).ToLower()) && !ignoreFiles.Contains(Path.GetFileName(s).ToLower())).ToArray();

                assetbundleFilenames = new string[assetbundleFiles.Length];
                for (int i = 0; i < assetbundleFiles.Length; i++)
                {
                    assetbundleFilenames[i] = Path.GetFileName(assetbundleFiles[i]);
                }
            }


            if (GUILayout.Button("加载所有AssetBundle列表"))
            {
                assetBundleDict.Clear();
                for (int i = 0; i < assetbundleFiles.Length; i++)
                {
					Debug.Log(assetbundleFiles[i]);
					assetBundleDict[assetbundleFiles[i]] = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(assetbundleFiles[i]));
                }
            }


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
            for (int i = 0; i < assetbundleFiles.Length; i++)
            {
                if (GUILayout.Button(assetbundleFilenames[i], GUILayout.ExpandWidth(true)))
                {
                    //if(assetBundle != null)
                    //{
                    //    assetBundle.Unload(false);
                    //}

                    if(assetBundleDict.ContainsKey(assetbundleFiles[i]))
                        assetBundle = assetBundleDict[assetbundleFiles[i]];
                    else
					{

						assetBundle = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(assetbundleFiles[i]));
//						assetBundle = AssetBundle.CreateFromFile(assetbundleFiles[i])
					}

					Debug.Log(assetBundle);
//                    string[] assetNames = assetBundle.GetAllAssetNames();
                    assetObjcts = assetBundle.LoadAll();
                    assetTypes = new Type[assetObjcts.Length];

                    //assetNameHeight = Mathf.Max(200, assetNames.Length * 20 + 40);

                    //assetNameStr = "";
                    //for (int j = 0; j < assetNames.Length; j++)
                    //{
                    //    assetNameStr += i + "  " + assetNames[j] + "\n";
                    //}


					GameObject go = new GameObject(Path.GetFileName(assetbundleFiles[i]));
                    for (int j = 0; j < assetObjcts.Length; j++)
                    {
                        assetTypes[j] = assetObjcts[j].GetType();
                        
                        if(assetTypes[j] == typeGameObject)
                        {
							GameObject gameObject = (GameObject)GameObject.Instantiate((GameObject)assetObjcts[j]);
                            gameObject.transform.SetParent(go.transform);
                        }
                    }

                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(20);


            infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos, GUILayout.ExpandWidth(true), GUILayout.MinHeight(500));

            EditorGUILayout.LabelField("AssetNames:");
            //assetNameStr = GUILayout.TextArea(assetNameStr, GUILayout.Height(assetNameHeight));

            //GUILayout.Space(20);

            if (assetObjcts != null && assetTypes != null )
            {

                for (int i = 0; i < assetObjcts.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(assetObjcts[i].ToString(), GUILayout.Width(500));
                    if(assetTypes[i] == typeGameObject)
                    {

                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical ();


        }
    }

}
