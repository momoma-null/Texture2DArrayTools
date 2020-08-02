using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using MomomaAssets.Utility;

namespace MomomaAssets
{

[CustomEditor(typeof(Texture2DArray))]
public class Texture2DArrayInspector : Editor
{
	Texture2DArray texture2DArray;
	int width;
	int height;
	int mipCount;
	TextureFormat format;

	List<Texture2D> m_Texture2DList = new List<Texture2D>();
	ReorderableList reorderableList;
	int activeIndex;

	GUIContent previewTitle = new GUIContent("");
	
	void OnEnable()
	{
		texture2DArray = target as Texture2DArray;
		width = texture2DArray.width;
		height = texture2DArray.height;
		format = texture2DArray.format;
		mipCount = serializedObject.FindProperty("m_MipCount").intValue;

		T2DAtoList();
		reorderableList = new ReorderableList(m_Texture2DList, typeof(Texture2D));
		reorderableList.onAddCallback += OnAdd;
		reorderableList.onCanRemoveCallback += OnCanRemove;
		reorderableList.onChangedCallback += OnChanged;
		reorderableList.drawHeaderCallback += DrawHeader;
		reorderableList.drawElementCallback += DrawElemnt;
		reorderableList.elementHeight = 64;

		previewTitle = new GUIContent(target.name);
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Depth", texture2DArray.depth.ToString());
		EditorGUILayout.LabelField(width + " x " + height + "  " + format.ToString());
		EditorGUILayout.LabelField("Mipmap Count", mipCount.ToString());
		EditorGUILayout.Space();
		reorderableList.DoLayoutList();
	}
	
	void T2DAtoList()
	{
		m_Texture2DList.Clear();

		for (int i = 0; i < texture2DArray.depth; i++)
		{
			Texture2D tex = new Texture2D(width, height, format, mipCount > 1, false);
			for (int j = 0; j < mipCount; j++)
			{
				Graphics.CopyTexture(texture2DArray, i, j, tex, 0, j);
			}
			m_Texture2DList.Add(tex);
		}
	}

	void ListtoT2DA()
	{
		if (m_Texture2DList.Count < 1)
			m_Texture2DList.Add(new Texture2D(width, height, format, mipCount > 1, false));

		var newTexture2DArray = new Texture2DArray(width, height, m_Texture2DList.Count, format, mipCount > 1, false);

		for (int i = 0; i < m_Texture2DList.Count; i++)
		{
			for (int j = 0; j < mipCount; j++)
			{
				Graphics.CopyTexture(m_Texture2DList[i], 0, j, newTexture2DArray, i, j);
			}
		}
		
		serializedObject.CopySerializedObject(new SerializedObject(newTexture2DArray), new string[1] {"m_Name"}, false);
		serializedObject.Update();
		serializedObject.FindProperty("m_IsReadable").boolValue = false;
		serializedObject.ApplyModifiedPropertiesWithoutUndo();
	}

	void OnAdd(ReorderableList l)
	{
		m_Texture2DList.Add(new Texture2D(width, height, format, mipCount > 1, false));
	}

	bool OnCanRemove(ReorderableList l)
	{
		return l.count > 1;
	}

	void OnChanged(ReorderableList l)
	{
		ListtoT2DA();
	}

	void DrawHeader(Rect rect)
	{
		EditorGUI.LabelField(rect, "Texture2D Array");
	}

	void DrawElemnt(Rect rect, int index, bool isActive, bool isFocused)
	{
		if (isActive)
			activeIndex = index;
		
		EditorGUI.BeginChangeCheck();
		var newTex = (Texture2D)EditorGUI.ObjectField(rect ,"Texture " + index.ToString(), m_Texture2DList[index], typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck())
		{
			if (newTex.width != width || newTex.height != height || newTex.format != format || newTex.mipmapCount != mipCount)
			{
				EditorUtility.DisplayDialog("Failed to copy", "Texture size , format or mipmap count is different", "OK");
				return;
			}

			m_Texture2DList[index] = newTex;
			ListtoT2DA();
		}
	}

	public override bool HasPreviewGUI()
    {
        return true;
    }

	public override GUIContent GetPreviewTitle()
    {
        return previewTitle;
    }

	public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        if (activeIndex >= m_Texture2DList.Count)
			activeIndex = 0;
		var tex = m_Texture2DList[activeIndex];
		EditorGUI.DrawPreviewTexture(rect, tex, null, ScaleMode.ScaleToFit);
    }
}

}// namespace MomomaAssets