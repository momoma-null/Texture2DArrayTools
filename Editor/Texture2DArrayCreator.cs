using UnityEngine;
using UnityEditor;

namespace MomomaAssets
{

public class Texture2DArrayCreator
{
	[MenuItem("Assets/Create/Texture 2D Array", false, 310)]
	static void CreateTexture2DArray()
	{
		var src = Selection.activeObject;
		if (!(src is Texture2D))
			return;
		var texture2D = (Texture2D)src;
		if (!AssetDatabase.Contains(texture2D))
			return;
		var path = AssetDatabase.GetAssetPath(texture2D);
		path = System.IO.Path.ChangeExtension(path, "asset");
		path = AssetDatabase.GenerateUniqueAssetPath(path);

		var texture2DArray = new Texture2DArray(texture2D.width, texture2D.height, 1, texture2D.format, texture2D.mipmapCount > 1, false);
		for (int k = 0; k < texture2D.mipmapCount; k++)
		{
			Graphics.CopyTexture(texture2D, 0, k, texture2DArray, 0, k);
		}
		var so = new SerializedObject(texture2DArray);
		so.Update();
		so.FindProperty("m_IsReadable").boolValue = false;
		so.ApplyModifiedPropertiesWithoutUndo();
		AssetDatabase.CreateAsset(texture2DArray, path);
	}

	[MenuItem("Assets/Create/Texture 2D Array", validate = true)]
	static bool CreateTexture2DArrayValidation()
	{
		var select = Selection.activeObject;
		return (select is Texture2D) && (AssetDatabase.Contains(select));
	}
}

}// namespace MomomaAssets