using System;
using UnityEngine;

using AmazingAssets.BeastEditor;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class BeastLitShader : BaseShaderGUI
    {
        private LitGUI.LitProperties litProperties;
        private LitDetailGUI.LitProperties litDetailProperties;
        private SavedBool m_DetailInputsFoldout;



        //Beast        
        MaterialProperty _Beast_Tessellation_Type = null;
        MaterialProperty _Beast_TessellationFactor = null;
        MaterialProperty _Beast_TessellationMinDistance = null;
        MaterialProperty _Beast_TessellationMaxDistance = null;
        MaterialProperty _Beast_TessellationEdgeLength = null;
        MaterialProperty _Beast_TessellationPhong = null;
        MaterialProperty _Beast_TessellationDisplaceMap = null;
        MaterialProperty _Beast_TessellationDisplaceMapUVSet = null;
        MaterialProperty _Beast_TessellationDisplaceMapChannel = null;
        MaterialProperty _Beast_TessellationDisplaceStrength = null;
        MaterialProperty _Beast_TessellationShadowPassLOD = null;
        MaterialProperty _Beast_TessellationDepthPassLOD = null;
        MaterialProperty _Beast_TessellationUseSmoothNormals = null;
        MaterialProperty _Beast_Generate = null;
        MaterialProperty _Beast_TessellationNormalCoef;
        MaterialProperty _Beast_TessellationTangentCoef = null;



        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);
            m_DetailInputsFoldout = new SavedBool($"{headerStateKey}.DetailInputsFoldout", true);
        }

        public override void DrawAdditionalFoldouts(Material material)
        {
            m_DetailInputsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_DetailInputsFoldout.value, LitDetailGUI.Styles.detailInputs);
            if (m_DetailInputsFoldout.value)
            {
                LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();            
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
            litDetailProperties = new LitDetailGUI.LitProperties(properties);



            //Beast
            _Beast_Tessellation_Type = FindProperty("_Beast_Tessellation_Type", properties);
            _Beast_TessellationFactor = FindProperty("_Beast_TessellationFactor", properties);
            _Beast_TessellationMinDistance = FindProperty("_Beast_TessellationMinDistance", properties);
            _Beast_TessellationMaxDistance = FindProperty("_Beast_TessellationMaxDistance", properties);
            _Beast_TessellationEdgeLength = FindProperty("_Beast_TessellationEdgeLength", properties);
            _Beast_TessellationPhong = FindProperty("_Beast_TessellationPhong", properties);
            _Beast_TessellationDisplaceMap = FindProperty("_Beast_TessellationDisplaceMap", properties);
            _Beast_TessellationDisplaceMapUVSet = FindProperty("_Beast_TessellationDisplaceMapUVSet", properties);
            _Beast_TessellationDisplaceMapChannel = FindProperty("_Beast_TessellationDisplaceMapChannel", properties);
            _Beast_TessellationDisplaceStrength = FindProperty("_Beast_TessellationDisplaceStrength", properties);
            _Beast_TessellationShadowPassLOD = FindProperty("_Beast_TessellationShadowPassLOD", properties);
            _Beast_TessellationDepthPassLOD = FindProperty("_Beast_TessellationDepthPassLOD", properties);
            _Beast_TessellationUseSmoothNormals = FindProperty("_Beast_TessellationUseSmoothNormals", properties);
            _Beast_Generate = FindProperty("_Beast_Generate", properties);
            _Beast_TessellationNormalCoef = FindProperty("_Beast_TessellationNormalCoef", properties);
            _Beast_TessellationTangentCoef = FindProperty("_Beast_TessellationTangentCoef", properties);
        }

        // material changed check
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);


            SetupBeastKeywords(material);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            if (litProperties.workflowMode != null)
            {
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, Enum.GetNames(typeof(LitGUI.WorkflowMode)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if (EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                }
            }

            base.DrawAdvancedOptions(material);


            GUILayout.Space(10);
            DoBeast(material, materialEditor);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            MaterialChanged(material);
        }



        //Beast//////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum TessellationMode
        {
            Fixed,
            DistanceBased,
            EdgeLength,
            Phong
        }
        public enum Recalculate
        {
            None,
            Normals,
            Tangents,
        }

        static string[] tessellationTypeNames = new string[] { "Fixed", "Distance Based", "Edge Length", "Phong" };


        void DoBeast(Material material, MaterialEditor editor)
        {
            DrawGroupHeader("Beast Tessellation");


            EditorGUI.BeginChangeCheck();
            editor.ShaderProperty(_Beast_Tessellation_Type, "Type");
            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }


            TessellationMode mode = (TessellationMode)_Beast_Tessellation_Type.floatValue;

            switch (mode)
            {
                case TessellationMode.Fixed:
                    editor.RangeProperty(_Beast_TessellationFactor, "Factor");
                    break;

                case TessellationMode.DistanceBased:
                    editor.RangeProperty(_Beast_TessellationFactor, "Factor");

                    using (new AmazingAssets.EditorGUIUtility.EditorGUIIndentLevel(1))
                    {
                        EditorGUI.BeginChangeCheck();
                        editor.FloatProperty(_Beast_TessellationMinDistance, "Min Distance");
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (_Beast_TessellationMinDistance.floatValue < 0)
                                _Beast_TessellationMinDistance.floatValue = 0;

                            if (_Beast_TessellationMinDistance.floatValue > _Beast_TessellationMaxDistance.floatValue)
                                _Beast_TessellationMaxDistance.floatValue = _Beast_TessellationMinDistance.floatValue;
                        }

                        EditorGUI.BeginChangeCheck();
                        editor.FloatProperty(_Beast_TessellationMaxDistance, "Max Distance");
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (_Beast_TessellationMaxDistance.floatValue < 0)
                                _Beast_TessellationMaxDistance.floatValue = 0;
                            if (_Beast_TessellationMaxDistance.floatValue < _Beast_TessellationMinDistance.floatValue)
                                _Beast_TessellationMinDistance.floatValue = _Beast_TessellationMaxDistance.floatValue;
                        }
                    }
                    break;

                case TessellationMode.EdgeLength:
                    editor.RangeProperty(_Beast_TessellationEdgeLength, "Edge Length");
                    break;

                case TessellationMode.Phong:
                    editor.RangeProperty(_Beast_TessellationEdgeLength, "Edge Length");
                    editor.RangeProperty(_Beast_TessellationPhong, "Phong");
                    break;
            }

            if (mode != TessellationMode.Phong)
            {
                using (new AmazingAssets.EditorGUIUtility.EditorGUIUtilityFieldWidth(UnityEditor.EditorGUIUtility.fieldWidth * 2))
                {
                    editor.TexturePropertySingleLine(new GUIContent("Displace Map"), _Beast_TessellationDisplaceMap);
                }
                editor.ShaderProperty(_Beast_TessellationDisplaceMapChannel, new GUIContent("Channel"), 1);
                editor.ShaderProperty(_Beast_TessellationDisplaceStrength, "Strength", 1);

                editor.TextureScaleOffsetProperty(_Beast_TessellationDisplaceMap);
                editor.ShaderProperty(_Beast_TessellationDisplaceMapUVSet, "UV Set");

            }

            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            editor.ShaderProperty(_Beast_Generate, "Recalculate");
            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            switch ((Recalculate)_Beast_Generate.floatValue)
            {
                case Recalculate.Normals:
                    editor.ShaderProperty(_Beast_TessellationNormalCoef, "   Normal Coef");
                    break;

                case Recalculate.Tangents:
                    editor.ShaderProperty(_Beast_TessellationNormalCoef, "   Normal Coef");
                    editor.ShaderProperty(_Beast_TessellationTangentCoef, "   Tangent Coef");
                    break;
            }

            editor.RangeProperty(_Beast_TessellationShadowPassLOD, "Shadow Pass LOD");
            editor.RangeProperty(_Beast_TessellationDepthPassLOD, "Depth Pass LOD");


            GUILayout.Space(5);
            bool useSmoothNormals = _Beast_TessellationUseSmoothNormals.floatValue > 0.5f;
            EditorGUI.BeginChangeCheck();
            useSmoothNormals = EditorGUILayout.Toggle("Use Smooth Normals", useSmoothNormals);
            if (EditorGUI.EndChangeCheck())
            {
                _Beast_TessellationUseSmoothNormals.floatValue = useSmoothNormals ? 1 : 0;
            }

            if (useSmoothNormals)
            {
                if (editor.HelpBoxWithButton(new GUIContent("Shader will use smooth normals from mesh UV4."), new GUIContent("Bake")))
                {
                    using (new AmazingAssets.EditorGUIUtility.GUIEnabled(Selection.activeGameObject != null))
                    {
                        BeastEditorWindow.ShowWindow();
                    }
                }
            }
        }
        void SetupBeastKeywords(Material material)
        {
            switch ((Recalculate)_Beast_Generate.floatValue)
            {
                case Recalculate.None:
                    material.DisableKeyword("_BEAST_GENERATE_NORMALS");
                    material.DisableKeyword("_BEAST_GENERATE_NORMALS_AND_TANGENT");
                    break;

                case Recalculate.Normals:
                    material.EnableKeyword("_BEAST_GENERATE_NORMALS");
                    material.DisableKeyword("_BEAST_GENERATE_NORMALS_AND_TANGENT");
                    break;

                case Recalculate.Tangents:
                    material.DisableKeyword("_BEAST_GENERATE_NORMALS");
                    material.EnableKeyword("_BEAST_GENERATE_NORMALS_AND_TANGENT");
                    break;
            }



            switch ((TessellationMode)_Beast_Tessellation_Type.floatValue)
            {
                case TessellationMode.Fixed:
                    material.EnableKeyword("_BEAST_TESSELLATION_TYPE_FIXED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_DISTANCE_BASED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_EDGE_LENGTH");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_PHONG");
                    break;

                case TessellationMode.DistanceBased:
                    material.EnableKeyword("_BEAST_TESSELLATION_TYPE_DISTANCE_BASED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_FIXED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_EDGE_LENGTH");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_PHONG");
                    break;

                case TessellationMode.EdgeLength:
                    material.EnableKeyword("_BEAST_TESSELLATION_TYPE_EDGE_LENGTH");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_FIXED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_DISTANCE_BASED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_PHONG");
                    break;

                case TessellationMode.Phong:
                    material.EnableKeyword("_BEAST_TESSELLATION_TYPE_PHONG");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_EDGE_LENGTH");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_FIXED");
                    material.DisableKeyword("_BEAST_TESSELLATION_TYPE_DISTANCE_BASED");
                    break;
            }
        }
        void DrawGroupHeader(string label)
        {
            Rect labelRect = EditorGUILayout.GetControlRect();


            Rect headerRect = labelRect;
            headerRect.xMin = 10;
            headerRect.yMax -= 2;
            EditorGUI.DrawRect(headerRect, Color.black * 0.6f);


            Rect lineRect = headerRect;
            lineRect.yMin = lineRect.yMax;
            lineRect.height = 2;
            EditorGUI.DrawRect(lineRect, new Color(0.92f, 0.65f, 0, 1));


            EditorGUI.LabelField(labelRect, label, EditorStyles.whiteLabel);
        }
    }
}
