using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//using CS;
//[System.Serializable]
//public class HeaderExampleClass
//{
//    [Header("My Fields", 3)]
//    public string someString = "ssttrriinngg";
//    public int someInt = 5;
//    [SerializeField] private float privateFloat = 0.9f;

//    [System.Serializable]
//    public class SomeSubClass
//    {
//        // depth represents the depth of this header in the inspector foldout heirarchy
//        [Header("Some SubClass Fields", count = 2, depth = 1)]
//        public int one = 1;
//        public int two = 2;
//    }

//    [Header("My SubClass", "Note: This will show a subclass with its own nested header when expanded", 1)]
//    public SomeSubClass mySubClass;
//}
namespace LoxVMod
{
    [InitializeOnLoad]
    public class HeaderAttribute : PropertyAttribute
    {
        public int count;
        public int depth;

        public string label;
        public string tooltip;

        /// <summary>
        /// Add a header above a field
        /// </summary>
        /// <param name="label">A title for the header label</param>
        /// <param name="count">the number of child elements under this header</param>
        /// <param name="depth">the depth of this header element in the inspector foldout</param>
        public HeaderAttribute(string label, int count = default, int depth = default)
        {
            this.count = count;
            this.depth = depth;
            this.label = label;
        }

        /// <summary>
        /// Add a header above a field with a tooltip
        /// </summary>
        /// <param name="label">A title for the header label</param>
        /// <param name="tooltip">A note or instruction shown when hovering over the header</param>
        /// <param name="count">the number of child elements under this header</param>
        /// <param name="depth">the depth of this header element in the inspector foldout</param>
        public HeaderAttribute(string label, string tooltip, int count = default, int depth = default)
        {
            this.count = count;
            this.depth = depth;
            this.label = label;
            this.tooltip = tooltip;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(HeaderAttribute))]
        public class HeaderDrawer : DecoratorDrawer
        {
            const float spacing = 8f;
            const float padding = 2f;
            const float margin = -20f;
            const float barHeight = 4f;

            public override void OnGUI(Rect position)
            {
                var attr = (attribute as HeaderAttribute);
                var pos = EditorGUI.IndentedRect(position);
                var rowHeight = (EditorGUIUtility.singleLineHeight * attr.count) + (EditorGUIUtility.standardVerticalSpacing * attr.count);


                // draw header background and label
                var headerRect = new Rect(pos.x + margin, pos.y + spacing, (pos.width - margin) + (padding * 2), pos.height - (spacing + barHeight + spacing));
                EditorGUI.DrawRect(headerRect, Constants.BackgroundColor);
                EditorGUI.LabelField(headerRect, new GUIContent(attr.label, attr.tooltip), Constants.LabelStyle);

                // only draw bar and child background if this header has children
                if (attr.count > 0)
                {
                    // draw depth color bar
                    var barRect = new Rect(headerRect.x, headerRect.y + headerRect.height + (spacing / 2), headerRect.width, barHeight);
                    EditorGUI.DrawRect(barRect, Constants.ColorForDepth(attr.depth));

                    // draw child background
                    // this assumes that all child elements are of default line height
                    // additional height could optionally be added as an attribute field to compensate for size variance as needed
                    var childrenRect = new Rect(headerRect.x, position.y + position.height - padding, headerRect.width + (padding * 2), rowHeight + (padding * 2));
                    EditorGUI.DrawRect(childrenRect, Constants.BackgroundColor);
                }
            }

            public override float GetHeight()
            {
                return EditorGUIUtility.singleLineHeight * 2.5f;
            }
        }
#endif

        public static class Constants
        {
            private static readonly Color[] _barColors = new Color[5] {
                new Color(0.3411765f, 0.6039216f, 0.7803922f),
                new Color(0.145098f, 0.6f, 0.509804f),
                new Color(0.9215686f, 0.6431373f, 0.282353f),
                new Color(0.8823529f, 0.3529412f, 0.4039216f),
                new Color(0.9529412f, 0.9294118f, 0.682353f)
            };

            public static Color ColorForDepth(int depth) => _barColors[depth % _barColors.Length];

            public static Color BackgroundColor { get; } = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f, 0.75f) : new Color(0.82f, 0.82f, 0.82f, 0.75f);

            public static GUIStyle LabelStyle { get; } = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}