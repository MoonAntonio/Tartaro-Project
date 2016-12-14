//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGridExtensions.cs (04/11/2016)                                              \\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion: Extension necesaria para el MGrid    							\\
// Fecha Mod:       04/11/2016                                                  \\
// Ultima Mod: Version Inicial     												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using System.Linq;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Extension necesaria para el MGrid </para>
    /// </summary>
    public class MGridExtensions : MonoBehaviour
    {
        #region Variables Privadas
        /// <summary>
        /// <para>Epsilon para el Ajuste</para>
        /// </summary>
        private const float EPSILON = .0001f;                                   // Epsilon para el Ajuste
        #endregion

        #region Funcionalidad
        /// <summary>
        /// <para>Consigue un color con un valor string</para>
        /// </summary>
        /// <param name="valor">String para el color, (Valido 01234567890.,)</param>
        /// <returns>Devuelve el color Parseado</returns>
        public static Color ColorConString(string valor)// Consigue un color con un valor string
        {
            string stringPermitidos = "01234567890.,";
            valor = new string(valor.Where(c => stringPermitidos.Contains(c)).ToArray());
            string[] rgba = valor.Split(',');

            if (rgba.Length < 4)
                return new Color(1f, 0f, 1f, 1f);

            return new Color(
                float.Parse(rgba[0]),
                float.Parse(rgba[1]),
                float.Parse(rgba[2]),
                float.Parse(rgba[3]));
        }

        /// <summary>
        /// <para>Obtiene una mascara de un vector</para>
        /// </summary>
        /// <param name="vec">Vector para conseguir su mascara</param>
        /// <returns>Una mascara.</returns>
        private static Vector3 GetVectorAMask(Vector3 vec)// Obtiene una mascara de un vector
        {
            return new Vector3(Mathf.Abs(vec.x) > Mathf.Epsilon ? 1f : 0f,
                                Mathf.Abs(vec.y) > Mathf.Epsilon ? 1f : 0f,
                                Mathf.Abs(vec.z) > Mathf.Epsilon ? 1f : 0f);
        }

        /// <summary>
        /// <para>Obtiene el valor de la mascara.</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="mask">Mascara</param>
        /// <returns>Valor de la mascara</returns>
        public static float GetValorDeMascara(Vector3 val, Vector3 mask)// Obtiene el valor de la mascara
        {
            if (Mathf.Abs(mask.x) > .0001f)
                return val.x;
            else if (Mathf.Abs(mask.y) > .0001f)
                return val.y;
            else
                return val.z;
        }

        /// <summary>
        /// <para>Obtiene un Axis de una Mascara</para>
        /// </summary>
        /// <param name="mascara">Mascara para conseguir un Axis</param>
        /// <returns>Un Axis.</returns>
        private static Axis GetMaskAAxis(Vector3 mascara)// Obtiene un Axis de una Mascara
        {
            Axis axis = Axis.Null;
            if (Mathf.Abs(mascara.x) > 0) axis |= Axis.X;
            if (Mathf.Abs(mascara.y) > 0) axis |= Axis.Y;
            if (Mathf.Abs(mascara.z) > 0) axis |= Axis.Z;
            return axis;
        }

        /// <summary>
        /// <para>Obtiene el mejor Axis</para>
        /// </summary>
        /// <param name="mascara">Mascara para conseguir un Axis</param>
        /// <returns>Un Axis.</returns>
        private static Axis GetMejorAxis(Vector3 mascara)// Obtiene el mejor Axis
        {
            float x = Mathf.Abs(mascara.x);
            float y = Mathf.Abs(mascara.y);
            float z = Mathf.Abs(mascara.z);

            return (x > y && x > z) ? Axis.X : ((y > x && y > z) ? Axis.Y : Axis.Z);
        }

        /// <summary>
        /// <para>Calcula el movimiento del Grid dependiendo de la camara.</para>
        /// </summary>
        /// <param name="movimiento">Movimiento del Grid</param>
        /// <param name="camara">Camara en el espacio</param>
        /// <returns>El calculo del arrastre del Axis</returns>
        public static Axis CalcularArrastreDeAxis(Vector3 movimiento, Camera camara)// Calcula el movimiento del Grid dependiendo de la camara
        {
            Vector3 mask = GetVectorAMask(movimiento);

            if (mask.x + mask.y + mask.z == 2)
            {
                return GetMaskAAxis(Vector3.one - mask);
            }
            else
            {
                switch (GetMaskAAxis(mask))
                {
                    case Axis.X:
                        if (Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.up)) < Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.forward)))
                            return Axis.Z;
                        else
                            return Axis.Y;

                    case Axis.Y:
                        if (Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.right)) < Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.forward)))
                            return Axis.Z;
                        else
                            return Axis.X;

                    case Axis.Z:
                        if (Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.right)) < Mathf.Abs(Vector3.Dot(camara.transform.forward, Vector3.up)))
                            return Axis.Y;
                        else
                            return Axis.X;
                    default:

                        return Axis.Null;
                }
            }
        }
        #endregion

        #region API de Ajuste
        /// <summary>
        /// <para>Ajuste general de MGrid</para>
        /// </summary>
        /// <param name="val">Valor del Grid</param>
        /// <param name="ronda">Ronda del Grid</param>
        /// <returns>Devuelve la ronda del grid</returns>
        public static float Ajustar(float val, float ronda)// Ajuste general de MGrid
        {
            return ronda * Mathf.Round(val / ronda);
        }

        /// <summary>
        /// <para>Ajusta el valor del Grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>Ajuste del Grid</returns>
        public static Vector3 AjustarValor(Vector3 val, float valorAjuste)// Ajusta el valor del Grid
        {
            float _x = val.x, _y = val.y, _z = val.z;
            return new Vector3(
                Ajustar(_x, valorAjuste),
                Ajustar(_y, valorAjuste),
                Ajustar(_z, valorAjuste)
                );
        }

        /// <summary>
        /// <para>Ajusta el valor del Grid con la mascara</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="mask">Mascara</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>Ajuste del Grid con la mascara</returns>
        public static Vector3 AjustarValor(Vector3 val, Vector3 mask, float valorAjuste)// Ajusta el valor del Grid con la mascara
        {

            float _x = val.x, _y = val.y, _z = val.z;
            return new Vector3(
                (Mathf.Abs(mask.x) < EPSILON ? _x : Ajustar(_x, valorAjuste)),
                (Mathf.Abs(mask.y) < EPSILON ? _y : Ajustar(_y, valorAjuste)),
                (Mathf.Abs(mask.z) < EPSILON ? _z : Ajustar(_z, valorAjuste))
                );
        }

        /// <summary>
        /// <para>Ajustar la celda del grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="mask">Mascara</param>
        /// <param name="valorAjuste">Valor de ajuste<</param>
        /// <returns>El ajuste de la celda</returns>
        public static Vector3 AjustarCelda(Vector3 val, Vector3 mask, float valorAjuste)// Ajustar la celda del grid
        {
            float _x = val.x, _y = val.y, _z = val.z;
            return new Vector3(
                (Mathf.Abs(mask.x) < EPSILON ? _x : AjustarCelda(_x, valorAjuste)),
                (Mathf.Abs(mask.y) < EPSILON ? _y : AjustarCelda(_y, valorAjuste)),
                (Mathf.Abs(mask.z) < EPSILON ? _z : AjustarCelda(_z, valorAjuste))
                );
        }

        /// <summary>
        /// <para>Ajustar la celda del grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>El ajuste de la celda</returns>
        public static float AjustarCelda(float val, float valorAjuste)// Ajustar la celda del grid
        {
            return valorAjuste * Mathf.Ceil(val / valorAjuste);
        }

        /// <summary>
        /// <para>Ajusta el suelo del grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>El ajuste del suelo</returns>
        public static Vector3 AjustarSuelo(Vector3 val, float valorAjuste)// Ajusta el suelo del grid
        {
            float _x = val.x, _y = val.y, _z = val.z;
            return new Vector3(
                AjustarSuelo(_x, valorAjuste),
                AjustarSuelo(_y, valorAjuste),
                AjustarSuelo(_z, valorAjuste)
                );
        }

        /// <summary>
        /// <para>Ajusta el suelo del grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="mask">Mascara</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>El ajuste del suelo</returns>
        public static Vector3 AjustarSuelo(Vector3 val, Vector3 mask, float valorAjuste)// Ajusta el suelo del grid
        {
            float _x = val.x, _y = val.y, _z = val.z;
            return new Vector3(
                (Mathf.Abs(mask.x) < EPSILON ? _x : AjustarSuelo(_x, valorAjuste)),
                (Mathf.Abs(mask.y) < EPSILON ? _y : AjustarSuelo(_y, valorAjuste)),
                (Mathf.Abs(mask.z) < EPSILON ? _z : AjustarSuelo(_z, valorAjuste))
                );
        }

        /// <summary>
        /// <para>Ajusta el suelo del grid</para>
        /// </summary>
        /// <param name="val">Valor</param>
        /// <param name="valorAjuste">Valor de ajuste</param>
        /// <returns>El ajuste del suelo</returns>
        public static float AjustarSuelo(float val, float valorAjuste)// Ajusta el suelo del grid
        {
            return valorAjuste * Mathf.Floor(val / valorAjuste);
        }

        /// <summary>
        /// <para>Devuelve el suelo superior</para>
        /// </summary>
        /// <param name="v">vector del suelo</param>
        /// <returns>El vector del suelo superior</returns>
        public static Vector3 SueloSuperior(Vector3 v)// Devuelve el suelo superior
        {
            v.x = v.x < 0 ? -1 : 1;
            v.y = v.y < 0 ? -1 : 1;
            v.z = v.z < 0 ? -1 : 1;

            return v;
        }
        #endregion
    }

    /// <summary>
    /// <para>Un sustituto de GUIContent que ofrece alguna funcionalidad adicional.</para>
    /// </summary>
    [System.Serializable]
    public class MGGuiContent
    {
        #region Variables
        /// <summary>
        /// <para>Texto cuando el tooltip esta ON</para>
        /// </summary>
        public string textoOn;                                      // Texto cuando el tooltip esta ON
        /// <summary>
        /// <para>Texto cuando el tooltip esta OFF</para>
        /// </summary>
        public string textoOff;                                     // Texto cuando el tooltip esta OFF
        /// <summary>
        /// <para>Imagen cuando el tooltip esta ON</para>
        /// </summary>
        public Texture2D imagenOn;                                  // Imagen cuando el tooltip esta ON
        /// <summary>
        /// <para>Imagen cuando el tooltip esta OFF</para>
        /// </summary>
        public Texture2D imagenOff;                                 // Imagen cuando el tooltip esta OFF
        /// <summary>
        /// <para>String del tooltip</para>
        /// </summary>
        public string tooltip;                                      // String del tooltip
        /// <summary>
        /// <para>Gui content extendido</para>
        /// </summary>
        GUIContent gce = new GUIContent();                          // Gui content extendido
        #endregion

        #region Funcionalidad GUI
        /// <summary>
        /// <para>GUIContent extendido</para>
        /// </summary>
        /// <param name="t_on">Texto cuando el tooltip esta ON</param>
        /// <param name="t_off">Texto cuando el tooltip esta OFF</param>
        /// <param name="tooltip">String del tooltip</param>
        public MGGuiContent(string t_on, string t_off, string tooltip)// GUIContent extendido
        {
            this.textoOn = t_on;
            this.textoOff = t_off;
            this.imagenOn = (Texture2D)null;
            this.imagenOff = (Texture2D)null;
            this.tooltip = tooltip;

            gce.tooltip = tooltip;
        }

        /// <summary>
        /// <para>GUIContent extendido</para>
        /// </summary>
        /// <param name="t_on">Texto cuando el tooltip esta ON</param>
        /// <param name="t_off">Texto cuando el tooltip esta OFF</param>
        /// <param name="i_on">Imagen cuando el tooltip esta ON</param>
        /// <param name="i_off">Imagen cuando el tooltip esta OFF</param>
        /// <param name="tooltip">String del tooltip</param>
        public MGGuiContent(string t_on, string t_off, Texture2D i_on, Texture2D i_off, string tooltip)// GUIContent extendido
        {
            this.textoOn = t_on;
            this.textoOff = t_off;
            this.imagenOn = i_on;
            this.imagenOff = i_off;
            this.tooltip = tooltip;

            gce.tooltip = tooltip;
        }

        /// <summary>
        /// <para>Boton con funcionalidad extendida.</para>
        /// </summary>
        /// <param name="r">Dimensiones del boton</param>
        /// <param name="contenido">Contenido</param>
        /// <param name="enabled">Estado del boton</param>
        /// <param name="imageStyle">Estilo de imagen</param>
        /// <param name="altStyle">Estilo</param>
        /// <returns>Boton extendido</returns>
        public static bool ToggleBtn(Rect r, MGGuiContent contenido, bool enabled, GUIStyle imageStyle, GUIStyle altStyle)// Boton con funcionalidad extendida
        {
            contenido.gce.image = enabled ? contenido.imagenOn : contenido.imagenOff;
            contenido.gce.text = contenido.gce.image == null ? (enabled ? contenido.textoOn : contenido.textoOff) : "";

            return GUI.Button(r, contenido.gce, contenido.gce.image != null ? imageStyle : altStyle);
        }
        #endregion
    }
}
