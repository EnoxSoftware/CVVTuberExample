using UnityEditor;
using UnityEngine;

namespace CVVTuber
{
    [CustomPropertyDrawer(typeof(InterfaceRestrictionAttribute))]
    public class InterfaceRestrictionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var restriction = (InterfaceRestrictionAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {

                if (property.objectReferenceValue != null)
                {

                    System.Type type = property.objectReferenceValue.GetType();
                    if (type.GetInterface(restriction.type.ToString()) == null)
                    {
                        property.objectReferenceValue = null;
                    }
                }
                EditorGUI.ObjectField(position, property, new GUIContent(ObjectNames.NicifyVariableName(property.name) + " (" + restriction.type.Name + ")"));

            }
            else
            {
                EditorGUI.PropertyField(position, property);
            }
        }
    }
}