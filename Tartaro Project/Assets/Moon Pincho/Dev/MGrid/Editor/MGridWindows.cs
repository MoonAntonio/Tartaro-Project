//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGridWindows.cs (03/11/2016)                                              	\\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Windows de MGrid   											\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod: Version Inicial     												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using System.Collections;
using MoonPincho.Alexandria;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Windows de MGrid </para>
    /// </summary>
    public class MGridWindows : EditorWindow
    {
        #region Variables
        public MGrid editor;
        GUIContent MGridPredictivoGrid = new GUIContent("Predictivo Grid", "Si esta activado, el grid se renderizara automaticamente en el eje optimo segun el movimiento.");
        GUIContent MGridAjustarGrupo = new GUIContent("Ajustar A Grupos", "Si esta activado, los objetos seleccionados mantendran sus desplazamientos relativos al moverse. Si esta inhabilitado, todos los objetos de la seleccion se encajaran automaticamente en el grid.");
        #endregion

        #region GUI
        void OnGUI()
        {
            GUILayout.Label("Ajuste Modular", EditorStyles.boldLabel);

            float ajuste = editor.GetAjusteIncremental();

            EditorGUI.BeginChangeCheck();

            ajuste = EditorGUILayout.FloatField("Valor Ajuste", ajuste);

            if (EditorGUI.EndChangeCheck())
                editor.SetAjusteIncremental(ajuste);

            editor.MGridAjusteEscalaActivado = EditorGUILayout.Toggle("Ajuste en Escala", editor.MGridAjusteEscalaActivado);

            UnidadGrid _gridUnits = (UnidadGrid)(EditorPrefs.HasKey(MStrings.MGridUnidades) ? EditorPrefs.GetInt(MStrings.MGridUnidades) : 0);

            bool ajusteEnGrupo = editor.MGridAjusteDeGrupo;
            ajusteEnGrupo = EditorGUILayout.Toggle(MGridAjustarGrupo, ajusteEnGrupo);
            if (ajusteEnGrupo != editor.MGridAjusteDeGrupo)
                editor.MGridAjusteDeGrupo = ajusteEnGrupo;

            EditorGUI.BeginChangeCheck();

            _gridUnits = (UnidadGrid)EditorGUILayout.EnumPopup("Grid unidades", _gridUnits);

            EditorGUI.BeginChangeCheck();
            editor.valorAngulo = EditorGUILayout.Slider("Angulo", editor.valorAngulo, 0f, 180f);
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(MStrings.MGridUnidades, (int)_gridUnits);
                editor.Cargar();
            }

            bool tmp = editor.prediccionGrid;
            tmp = EditorGUILayout.Toggle(MGridPredictivoGrid, tmp);
            if (tmp != editor.prediccionGrid)
            {
                editor.prediccionGrid = tmp;
                EditorPrefs.SetBool(MStrings.MGridPerspectiva, tmp);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("┌∩┐(◣_◢)┌∩┐"))
                this.Close();
        }
        #endregion
    }
}
