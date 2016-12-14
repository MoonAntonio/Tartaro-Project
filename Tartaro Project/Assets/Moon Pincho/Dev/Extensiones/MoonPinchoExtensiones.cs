//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MoonPinchoExtensiones.cs (03/11/2016)                                        \\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Extension de Moon Pincho   									\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod: Version Inicial     												\\
//******************************************************************************\\

#if UNITY_EDITOR
#define DEBUG
#endif

#region Librerias
using UnityEngine;
#endregion

namespace MoonPincho.Extensiones
{
    /// <summary>
    /// <para>[Moon Pincho]Extensiones varias</para>
    /// </summary>
    public static class MoonPinchoExtensiones
    {
        #region Matematicas
        /// <summary>
        /// <para>Valor de tipo permanente.</para>
        /// </summary>
        /// <returns>La constante</returns>
        public static bool Constantes(this Transform[] t_arr, Transform t)// Valor de tipo permanente
        {
            for (int i = 0; i < t_arr.Length; i++)
                if (t_arr[i] == t)
                    return true;
            return false;
        }

        /// <summary>
        /// <para>Suma un vector pasado.</para>
        /// </summary>
        /// <param name="v">Vector que sumar</param>
        /// <returns>Devuelve la suma de un vector</returns>
        public static float SumaVector(this Vector3 v)// Suma un vector pasado
        {
            return v[0] + v[1] + v[2];
        }

        /// <summary>
        /// <para>Para sacar los planos del renderer(El frustum es una porcion de una figura geometrica comprendida entre dos planos paralelos)</para>
        /// </summary>
        /// <param name="camara">Camara de la vista</param>
        /// <param name="punto">Punto de flexion</param>
        /// <returns>Devuelve el Frustum</returns>
        public static bool MFrustum(this Camera camara, Vector3 punto)// Para sacar los planos del renderer(El frustum es una porcion de una figura geometrica comprendida entre dos planos paralelos)
        {
            Vector3 p = camara.WorldToViewportPoint(punto);
            return (p.x >= 0f && p.x <= 1f) &&
                    (p.y >= 0f && p.y <= 1f) &&
                    p.z >= 0f;
        }
        #endregion
    }

    /// <summary>
    /// <para>[Moon Pincho] Extension de Debug</para>
    /// </summary>
    public static class MDebug
    {
        #region API
        /// <summary>
        /// <para>Es una copilacion de prueba</para>
        /// </summary>
        public static bool esDebugBuild// Es una copilacion de prueba
        {
            get { return Debug.isDebugBuild; }
        }
        #endregion

        #region LOG
        /// <summary>
        /// <para>Debug log por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object msj)// Debug log por consola
        {
            Debug.Log("<color=lime><b>LOG</b></color>: " + msj);
        }

        /// <summary>
        /// <para>Debug log por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        /// <param name="contenido">Objeto de unity</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]// Debug log por consola
        public static void Log(object msj, UnityEngine.Object contenido)
        {
            Debug.Log("<color=lime><b>LOG</b></color>: " + msj, contenido);
        }
        #endregion

        #region ERROR
        /// <summary>
        /// <para>Debug log error por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(object msj)// Debug log error por consola
        {
            Debug.LogError("<color=red><b>ERROR</b></color>: " + msj);
        }

        /// <summary>
        /// <para>Debug log error por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        /// <param name="contenido">Objeto de unity</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]// Debug log error por consola
        public static void LogError(object msj, UnityEngine.Object contenido)
        {
            Debug.LogError("<color=red><b>ERROR</b></color>: " + msj, contenido);
        }
        #endregion

        #region WARNING
        /// <summary>
        /// <para>Debug log Warning por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object msj)// Debug log Warning por consola
        {
            Debug.LogWarning("<color=purple><b>ADVERTENCIA</b></color>: " + msj.ToString());
        }

        /// <summary>
        /// <para>Debug log Warning por consola</para>
        /// </summary>
        /// <param name="msj">Mensaje que mostrar</param>
        /// <param name="contenido">Objeto de unity</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object msj, UnityEngine.Object contenido)// Debug log Warning por consola
        {
            Debug.LogWarning("<color=purple><b>ADVERTENCIA</b></color>: " + msj.ToString(), contenido);
        }
        #endregion

        #region RayCast
        /// <summary>
        /// <para>Dibuja una linea</para>
        /// </summary>
        /// <param name="inicio">Inicio de la linea</param>
        /// <param name="fin">Fin de la linea</param>
        /// <param name="color">Color de la linea</param>
        /// <param name="duracion">Duracion de la linea</param>
        /// <param name="depth">Profundidad</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 inicio, Vector3 fin, Color color = default(Color), float duracion = 0.0f, bool depth = true)// Dibuja una linea
        {
            Debug.DrawLine(inicio, fin, color, duracion, depth);
        }

        /// <summary>
        /// Dibuja un rayo
        /// </summary>
        /// <param name="inicio">Inicio del rayo</param>
        /// <param name="fin">Fin del rayo</param>
        /// <param name="color">Color del rayo</param>
        /// <param name="duracion">Duracion del rayo</param>
        /// <param name="depth">Profundidad</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 inicio, Vector3 fin, Color color = default(Color), float duracion = 0.0f, bool depth = true)// Dibuja un rayo
        {
            Debug.DrawRay(inicio, fin, color, duracion, depth);
        }
        #endregion
    }
}
