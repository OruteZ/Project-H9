using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnitStat))]
public class UnitStatPropertyDrawer : PropertyDrawer
{
    // 각 배열의 Foldout 상태를 관리하기 위한 bool 값
    private bool originalFoldout = true;
    private bool additionalFoldout = true;
    private bool multiplierFoldout = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 기본 줄 높이와 간격 설정
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float padding = EditorGUIUtility.standardVerticalSpacing;

        float yOffset = position.y;

        // original, _additional, _multiplier 프로퍼티를 찾습니다.
        var originalProp = property.FindPropertyRelative("original");
        var additionalProp = property.FindPropertyRelative("_additional");
        var multiplierProp = property.FindPropertyRelative("_multiplier");

        // original 배열 드롭다운
        yOffset = DrawStatArrayFoldout(ref yOffset, position, originalProp, "Original Stats", ref originalFoldout, lineHeight, padding);

        // _additional 배열 드롭다운
        yOffset = DrawStatArrayFoldout(ref yOffset, position, additionalProp, "Additional Stats", ref additionalFoldout, lineHeight, padding);

        // _multiplier 배열 드롭다운
        yOffset = DrawStatArrayFoldout(ref yOffset, position, multiplierProp, "Multiplier Stats", ref multiplierFoldout, lineHeight, padding);

        EditorGUI.EndProperty();
    }

    private float DrawStatArrayFoldout(ref float yOffset, Rect position, SerializedProperty arrayProp, string label, ref bool foldout, float lineHeight, float padding)
    {
        // Foldout을 그리기 위한 Rect 설정
        Rect foldoutRect = new Rect(position.x, yOffset, position.width, lineHeight);
        foldout = EditorGUI.Foldout(foldoutRect, foldout, label);

        if (foldout)
        {
            EditorGUI.indentLevel++;
            yOffset += lineHeight + padding;

            // 배열의 각 요소 출력
            for (int i = 1; i < arrayProp.arraySize; i++)
            {
                SerializedProperty elementProp = arrayProp.GetArrayElementAtIndex(i);

                Rect rect = new Rect(position.x, yOffset, position.width, lineHeight);
                EditorGUI.PropertyField(rect, elementProp, new GUIContent(((StatType)(i)).ToString()));
                yOffset += lineHeight + padding;
            }

            EditorGUI.indentLevel--;
        }
        else
        {
            yOffset += lineHeight + padding;
        }

        return yOffset;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float padding = EditorGUIUtility.standardVerticalSpacing;

        float totalHeight = 0f;

        // 각 배열 Foldout 상태에 따른 높이 계산
        totalHeight += GetArrayHeight(property.FindPropertyRelative("original"), originalFoldout, lineHeight, padding);
        totalHeight += GetArrayHeight(property.FindPropertyRelative("_additional"), additionalFoldout, lineHeight, padding);
        totalHeight += GetArrayHeight(property.FindPropertyRelative("_multiplier"), multiplierFoldout, lineHeight, padding);

        return totalHeight;
    }

    private float GetArrayHeight(SerializedProperty arrayProp, bool foldout, float lineHeight, float padding)
    {
        float height = lineHeight + padding;

        if (foldout)
        {
            height += (lineHeight + padding) * arrayProp.arraySize;
        }

        return height;
    }
}
