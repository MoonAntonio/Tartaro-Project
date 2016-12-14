//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGridRenderer.cs (03/11/2016)                                              	\\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Se encarga de renderizar el editor en escena   				\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod:  Version Inicial    												\\
//******************************************************************************\\

#if UNITY_EDITOR

#region Librerias
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Se encarga de renderizar el editor en escena</para>
    /// </summary>
    [ExecuteInEditMode]
    public class MGridRenderer : MonoBehaviour
    {
        #region Variables Privadas
        /// <summary>
        /// <para>Mascara de visibilidad</para>
        /// </summary>
        HideFlags SceneCamaraHideFlags = (HideFlags)(1 | 4 | 8);            // Mascara de visibilidad
        #endregion

        #region Variables Publicas
        /// <summary>
        /// <para>Malla del renderer de la MGrid</para>
        /// </summary>
        public Mesh mesh;                                                   // Malla del renderer de la MGrid
        /// <summary>
        /// <para>Material del renderer de la MGrid</para>
        /// </summary>
        public Material material;                                           // Material del renderer de la MGrid
        #endregion

        #region Metodos
        /// <summary>
        /// <para>Llamada cuando se destruya</para>
        /// </summary>
        private void OnDestroy()// Llamada cuando se destruya
        {
            if (mesh) DestroyImmediate(mesh);
            if (material) DestroyImmediate(material);
        }

        /// <summary>
        /// <para>Cuando se renderiza el objeto</para>
        /// </summary>
        private void OnRenderObject()// Cuando se renderiza el objeto
        {
            // Comprobacion de camaras.
            if ((Camera.current.gameObject.hideFlags & SceneCamaraHideFlags) != SceneCamaraHideFlags || Camera.current.name != "SceneCamera")
                return;

            // Comprobacion de material y malla
            if (material == null || mesh == null)
            {
                GameObject.DestroyImmediate(this.gameObject);
                return;
            }

            material.SetPass(0);
            // Renderiza la malla en la ubicacion especificada
            Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
        }
        #endregion
    }
}
#endif
