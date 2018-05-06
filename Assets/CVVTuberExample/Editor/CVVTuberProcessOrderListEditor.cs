using UnityEditor;
using UnityEditorInternal;

using UnityEngine;

namespace CVVTuber
{

    [CustomEditor (typeof(CVVTuberProcessOrderList))]
    public class CVVTuberProcessOrderListEditor : Editor
    {

        ReorderableList m_list;

        void OnEnable ()
        {

            m_list = new ReorderableList (
                serializedObject,
                serializedObject.FindProperty ("processOrderList")
            );

            m_list.drawElementCallback += (rect, index, isActive, isFocused) => {

                //Debug.Log("index "+index);

                var element = m_list.serializedProperty.GetArrayElementAtIndex (index);
                if (element != null) {
                    rect.y += 2;

                    if (element.objectReferenceValue == null) {
                        EditorGUI.ObjectField (new UnityEngine.Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, typeof(CVVTuberProcess), UnityEngine.GUIContent.none);
                    } else {
                        EditorGUI.ObjectField (new UnityEngine.Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, typeof(CVVTuberProcess), new UnityEngine.GUIContent ((index + 1) + ". " + ((CVVTuberProcess)element.objectReferenceValue).GetDescription ()));
                    }

                }
            };

            m_list.drawHeaderCallback += (rect) => {
                EditorGUI.LabelField (rect, "");
            };

        }

        public override void OnInspectorGUI ()
        {
            //DrawDefaultInspector();

            serializedObject.Update ();

            m_list.DoLayoutList ();

            serializedObject.ApplyModifiedProperties ();
        }
    }
}


