//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGridEnum.cs (03/11/2016)                                                   	\\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Clase de Enums para MGrid   									\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod:  Version Inicial    												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using MoonPincho.Alexandria;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Clase de Enums para MGrid</para>
    /// </summary>
    public static class MGridEnum
    {
        #region API
        /// <summary>
        /// <para>Multiplica un Vector3 utilizando el valor inverso de un axis (por ejemplo, Axis.Y se convierte en Vector3 (1, 0, 1))</para>
        /// </summary>
        /// <param name="v">Vector3 que se multiplicara.</param>
        /// <param name="axis">Axis</param>
        /// <returns>La multiplicacion del vector3</returns>
        public static Vector3 InvertirAxisMask(Vector3 v, Axis axis)// Multiplica un Vector3 utilizando el valor inverso de un axis (por ejemplo, Axis.Y se convierte en Vector3 (1, 0, 1))
        {
            switch (axis)
            {
                case Axis.X:
                case Axis.NegX:
                    return Vector3.Scale(v, new Vector3(0f, 1f, 1f));

                case Axis.Y:
                case Axis.NegY:
                    return Vector3.Scale(v, new Vector3(1f, 0f, 1f));

                case Axis.Z:
                case Axis.NegZ:
                    return Vector3.Scale(v, new Vector3(1f, 1f, 0f));

                default:
                    return v;
            }
        }

        /// <summary>
        /// <para>Multiplica un Vector3 utilizando el valor de un axis (por ejemplo, Axis.Y se convierte en Vector3 (0, 1, 0))</para>
        /// </summary>
        /// <param name="v">Vector3 que se multiplicara.</param>
        /// <param name="axis">Axis</param>
        /// <returns>La multiplicacion del vector3</returns>
        public static Vector3 AxisMask(Vector3 v, Axis axis)// Multiplica un Vector3 utilizando el valor de un axis (por ejemplo, Axis.Y se convierte en Vector3 (0, 1, 0))
        {
            switch (axis)
            {
                case Axis.X:
                case Axis.NegX:
                    return Vector3.Scale(v, new Vector3(1f, 0f, 0f));

                case Axis.Y:
                case Axis.NegY:
                    return Vector3.Scale(v, new Vector3(0f, 1f, 0f));

                case Axis.Z:
                case Axis.NegZ:
                    return Vector3.Scale(v, new Vector3(0f, 0f, 1f));

                default:
                    return v;
            }
        }

        /// <summary>
        /// <para>Recoge el valor de la unidad del Grid.</para>
        /// </summary>
        /// <param name="uni">Unidad de Grid</param>
        /// <returns>Devuelve el valor en float de la unidad seleccionada.</returns>
        public static float ValorUnidadGrid(UnidadGrid uni)// Recoge el valor de la unidad del Grid.
        {
            switch (uni)
            {
                case UnidadGrid.Metros:
                    return MStrings.MGrid_Metros;
				case UnidadGrid.Centimetros:
					return MStrings.MGrid_Centimetros;
                case UnidadGrid.Milimetros:
					return MStrings.MGrid_Milimetros;
                default:
                    return MStrings.MGrid_Metros;
            }
        }
        #endregion
    }

    /// <summary>
    /// <para>Axis del Grid</para>
    /// </summary>
    public enum Axis// Axis del Grid
    {
        Null = 0x0,
        X = 0x1,
        Y = 0x2,
        Z = 0x4,
        NegX = 0x8,
        NegY = 0x16,
        NegZ = 0x32
    }

    /// <summary>
    /// <para>Unidad del Grid</para>
    /// </summary>
    public enum UnidadGrid// Unidad del Grid
    {
        Metros,
        Centimetros,
		Milimetros
    }
}
