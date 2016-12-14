//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGridMainRenderer.cs (03/11/2016)                                            \\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion: Main renderer en escena del MGrid    							\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod: Version Inicial     												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Main renderer en escena del MGrid </para>
    /// </summary>
    public class MGridMainRenderer
    {
        #region Variables
        /// <summary>
        /// <para>Variable para esconder el objeto en el inspector</para>
        /// </summary>
        private static readonly HideFlags MGridHideFlag = HideFlags.HideAndDontSave;        // Variable para esconder el objeto en el inspector
        /// <summary>
        /// <para>Nombre del objeto</para>
        /// </summary>
        private const string nombreObject = "MGridObject";                                  // Nombre del objeto
        /// <summary>
        /// <para>Nombre del material del objeto</para>
        /// </summary>
        private const string nombreMaterial = "MGridMaterialObject";                        // Nombre del material del objeto
        /// <summary>
        /// <para>Nombre de la malla del objeto</para>
        /// </summary>
        private const string nombreMesh = "MGridMeshObject";                                // Nombre de la malla del objeto
        /// <summary>
        /// <para>Shader para el grid</para>
        /// </summary>
        private const string shaderGrid = "Hidden/Moon Pincho/MP_GridShader";               // Shader para el grid
        /// <summary>
        /// <para>Maximo de lineas en el view</para>
        /// </summary>
        private const int maxLineas = 256;                                                  // Maximo de lineas en el view
        /// <summary>
        /// <para>Objeto del grid</para>
        /// </summary>
        private static GameObject gridObject;                                               // Objeto del grid
        /// <summary>
        /// <para>Malla del grid</para>
        /// </summary>
        private static Mesh gridMesh;                                                       // Malla del grid
        /// <summary>
        /// <para>Material del grid</para>
        /// </summary>
        private static Material gridMaterial;                                               // Material del grid
        /// <summary>
        /// <para>Variables temporales para los calculos</para>
        /// </summary>
        private static int tempIter, tempIterByte, tempMax = maxLineas, tempDiv = 1;        // Variables temporales para los calculos
        #endregion

        #region Inicializadores
        /// <summary>
        /// <para>Destruye algun objeto que se encuentre en escena, y inicializa otro nuevo</para>
        /// </summary>
        public static void Init()// Destruye algun objeto que se encuentre en escena, y inicializa otro nuevo
        {
            Destroy();

            gridObject = EditorUtility.CreateGameObjectWithHideFlags(nombreObject, MGridHideFlag, new System.Type[2] { typeof(MeshFilter), typeof(MeshRenderer) });

            MGridRenderer renderer = gridObject.AddComponent<MGridRenderer>();

            gridMesh = new Mesh();
            gridMesh.name = nombreMesh;
            gridMesh.hideFlags = MGridHideFlag;

            gridMaterial = new Material(Shader.Find(shaderGrid));
            gridMaterial.name = nombreMaterial;
            gridMaterial.hideFlags = MGridHideFlag;

            renderer.mesh = gridMesh;
            renderer.material = gridMaterial;
        }
        #endregion

        #region API
        /// <summary>
        /// <para>Destruye el objeto del grid</para>
        /// </summary>
        public static void Destroy()// Destruye el objeto del grid
        {
            DestroyObjetosConNombre(nombreMesh, typeof(Mesh));
            DestroyObjetosConNombre(nombreMaterial, typeof(Material));
            DestroyObjetosConNombre(nombreObject, typeof(GameObject));
        }

        /// <summary>
        /// <para>Destruye los objetos con el nombre fijado</para>
        /// </summary>
        /// <param name="Nombre">Nombre del objeto</param>
        /// <param name="tipo">Tipo del objeto</param>
        private static void DestroyObjetosConNombre(string Nombre, System.Type tipo)// Destruye los objetos con el nombre fijado
        {
            IEnumerable go = Resources.FindObjectsOfTypeAll(tipo).Where(x => x.name.Contains(Nombre));

            foreach (Object t in go)
            {
                GameObject.DestroyImmediate(t);
            }
        }

        /// <summary>
        /// <para>Devuelve la distancia desde el pivote al plano del Frustum en el orden de un float[]</para>
        /// </summary>
        /// <param name="cam">Camara view</param>
        /// <param name="pivote">Pivote de la camara</param>
        /// <param name="tan">tan del plano</param>
        /// <param name="bitan">bitan del plano</param>
        /// <param name="minDist">Distancia minima</param>
        /// <returns>Distancia desde el pivote al plano del Frustum</returns>
        private static float[] GetDistanciaAlFrustum(Camera cam, Vector3 pivote, Vector3 tan, Vector3 bitan, float minDist)// Devuelve la distancia desde el pivote al plano del Frustum en el orden de un float[]
        {
            Ray[] rays = new Ray[4]
            {
                new Ray(pivote, tan),
                new Ray(pivote, bitan),
                new Ray(pivote, -tan),
                new Ray(pivote, -bitan)
             };

            float[] intersecciones = new float[4] { minDist, minDist, minDist, minDist };
            bool[] interseccionesEncontradas = new bool[4] { false, false, false, false };

            Plane[] planos = GeometryUtility.CalculateFrustumPlanes(cam);
            foreach (Plane p in planos)
            {
                float dist;
                float t = 0;

                for (int i = 0; i < 4; i++)
                {
                    if (p.Raycast(rays[i], out dist))
                    {
                        t = Vector3.Distance(pivote, rays[i].GetPoint(dist));

                        if (t < intersecciones[i] || !interseccionesEncontradas[i])
                        {
                            interseccionesEncontradas[i] = true;
                            intersecciones[i] = Mathf.Max(minDist, t);
                        }
                    }
                }
            }
            return intersecciones;
        }
        #endregion

        #region Funciones
        /// <summary>
        /// <para>Dibuja el plano del grid</para>
        /// </summary>
        /// <param name="cam">Camara</param>
        /// <param name="pivote">Pivote camara</param>
        /// <param name="tangente">Tangente</param>
        /// <param name="bitangente">Bitangente</param>
        /// <param name="ajusteValor">Ajuste del valor</param>
        /// <param name="color">Color</param>
        /// <param name="alphaBump">Alpha</param>
        /// <returns></returns>
        public static float DibujarPlano(Camera cam, Vector3 pivote, Vector3 tangente, Vector3 bitangente, float ajusteValor, Color color, float alphaBump)// Dibuja el plano del grid
        {
            if (!gridMesh || !gridMaterial || !gridObject)
                Init();

            gridMaterial.SetFloat("_AlphaCutoff", .1f);
            gridMaterial.SetFloat("_AlphaFade", .6f);

            pivote = MGridExtensions.AjustarValor(pivote, ajusteValor);

            Vector3 p = cam.WorldToViewportPoint(pivote);
            bool inFrustum = (p.x >= 0f && p.x <= 1f) &&
                             (p.y >= 0f && p.y <= 1f) &&
                              p.z >= 0f;

            float[] distancias = GetDistanciaAlFrustum(cam, pivote, tangente, bitangente, 24f);

            if (inFrustum)
            {
                tempIter = (int)(Mathf.Ceil((Mathf.Abs(distancias[0]) + Mathf.Abs(distancias[2])) / ajusteValor));
                tempIterByte = (int)(Mathf.Ceil((Mathf.Abs(distancias[1]) + Mathf.Abs(distancias[3])) / ajusteValor));

                tempMax = Mathf.Max(tempIter, tempIterByte);

                if (tempMax > Mathf.Min(tempIter, tempIterByte) * 2)
                    tempMax = (int)Mathf.Min(tempIter, tempIterByte) * 2;

                tempDiv = 1;

                float dot = Vector3.Dot(cam.transform.position - pivote, Vector3.Cross(tangente, bitangente));

                if (tempMax > maxLineas)
                {
                    if (Vector3.Distance(cam.transform.position, pivote) > 50f * ajusteValor && Mathf.Abs(dot) > .8f)
                    {
                        while (tempMax / tempDiv > maxLineas)
                            tempDiv += tempDiv;
                    }
                    else
                    {
                        tempMax = maxLineas;
                    }
                }
            }

            DibujarFullGrid(cam, pivote, tangente, bitangente, ajusteValor * tempDiv, tempMax / tempDiv, tempDiv, color, alphaBump);

            return ((ajusteValor * tempDiv) * (tempMax / tempDiv));
        }

        /// <summary>
        /// <para>Dibuja la mitad del grid</para>
        /// </summary>
        /// <param name="cam">Camara</param>
        /// <param name="pivote">Pivote de la camara</param>
        /// <param name="tan">Tan</param>
        /// <param name="bitan">Bitan</param>
        /// <param name="incremento">Incremento</param>
        /// <param name="iteracion">Iteracion del grid</param>
        /// <param name="clrSegundario">Color segundario</param>
        /// <param name="alphaBump">Alpha</param>
        /// <param name="vertices">Vertices del Grid</param>
        /// <param name="normales">Normales del Grid</param>
        /// <param name="colores">Colores del Grid</param>
        /// <param name="indices">Indices del Grid</param>
        /// <param name="offset">OffSet</param>
        private static void DibujarMitadGrid(Camera cam, Vector3 pivote, Vector3 tan, Vector3 bitan, float incremento, int iteracion, Color clrSegundario, float alphaBump,
    out Vector3[] vertices,
    out Vector3[] normales,
    out Color[] colores,
    out int[] indices, int offset)// Dibuja la mitad del grid
        {
            Color primario = clrSegundario;
            primario.a += alphaBump;

            float len = incremento * iteracion;

            int offsetTan = (int)((MGridExtensions.GetValorDeMascara(pivote, tan) % (incremento * 10f)) / incremento);
            int offsetBitan = (int)((MGridExtensions.GetValorDeMascara(pivote, bitan) % (incremento * 10f)) / incremento);

            iteracion++;

            float fade = .75f;
            float fadeDist = len * fade;
            Vector3 nrm = Vector3.Cross(tan, bitan);

            vertices = new Vector3[iteracion * 6 - 3];
            normales = new Vector3[iteracion * 6 - 3];
            indices = new int[iteracion * 8 - 4];
            colores = new Color[iteracion * 6 - 3];

            vertices[0] = pivote;
            vertices[1] = (pivote + bitan * fadeDist);
            vertices[2] = (pivote + bitan * len);

            normales[0] = nrm;
            normales[1] = nrm;
            normales[2] = nrm;

            indices[0] = 0 + offset;
            indices[1] = 1 + offset;
            indices[2] = 1 + offset;
            indices[3] = 2 + offset;

            colores[0] = primario;
            colores[1] = primario;
            colores[2] = primario;
            colores[2].a = 0f;


            int n = 4;
            int v = 3;

            for (int i = 1; i < iteracion; i++)
            {
                vertices[v + 0] = pivote + i * tan * incremento;
                vertices[v + 1] = (pivote + bitan * fadeDist) + i * tan * incremento;
                vertices[v + 2] = (pivote + bitan * len) + i * tan * incremento;

                vertices[v + 3] = pivote + i * bitan * incremento;
                vertices[v + 4] = (pivote + tan * fadeDist) + i * bitan * incremento;
                vertices[v + 5] = (pivote + tan * len) + i * bitan * incremento;

                normales[v + 0] = nrm;
                normales[v + 1] = nrm;
                normales[v + 2] = nrm;
                normales[v + 3] = nrm;
                normales[v + 4] = nrm;
                normales[v + 5] = nrm;

                indices[n + 0] = v + 0 + offset;
                indices[n + 1] = v + 1 + offset;
                indices[n + 2] = v + 1 + offset;
                indices[n + 3] = v + 2 + offset;
                indices[n + 4] = v + 3 + offset;
                indices[n + 5] = v + 4 + offset;
                indices[n + 6] = v + 4 + offset;
                indices[n + 7] = v + 5 + offset;

                float alpha = (i / (float)iteracion);
                alpha = alpha < fade ? 1f : 1f - ((alpha - fade) / (1 - fade));

                Color col = (i + offsetTan) % 10 == 0 ? primario : clrSegundario;
                col.a *= alpha;

                colores[v + 0] = col;
                colores[v + 1] = col;
                colores[v + 2] = col;
                colores[v + 2].a = 0f;

                col = (i + offsetBitan) % 10 == 0 ? primario : clrSegundario;
                col.a *= alpha;

                colores[v + 3] = col;
                colores[v + 4] = col;
                colores[v + 5] = col;
                colores[v + 5].a = 0f;

                n += 8;
                v += 6;
            }
        }

        /// <summary>
        /// <para>Dibuja la perspectiva del grid</para>
        /// </summary>
        /// <param name="cam">Camara</param>
        /// <param name="pivote">Pivote de la camara</param>
        /// <param name="ajusteValor">Valor del ajuste</param>
        /// <param name="colores">Colores del grid</param>
        /// <param name="alphaBump">Alpha</param>
        public static void DibujarPerspectivaGrid(Camera cam, Vector3 pivote, float ajusteValor, Color[] colores, float alphaBump)// Dibuja la perspectiva del grid
        {
            if (!gridMesh || !gridMaterial || !gridObject)
                Init();

            gridMaterial.SetFloat("_AlphaCutoff", 0f);
            gridMaterial.SetFloat("_AlphaFade", 0f);

            Vector3 camDir = (pivote - cam.transform.position).normalized;
            pivote = MGridExtensions.AjustarValor(pivote, ajusteValor);

            Vector3 right = camDir.x < 0f ? Vector3.right : Vector3.right * -1f;
            Vector3 up = camDir.y < 0f ? Vector3.up : Vector3.up * -1f;
            Vector3 forward = camDir.z < 0f ? Vector3.forward : Vector3.forward * -1f;

            Ray ray_x = new Ray(pivote, right);
            Ray ray_y = new Ray(pivote, up);
            Ray ray_z = new Ray(pivote, forward);

            float xDist = 10f, y_dist = 10f, z_dist = 10f;
            bool xInterseccion = false, yInterseccion = false, zInterseccion = false;

            Plane[] planos = GeometryUtility.CalculateFrustumPlanes(cam);
            foreach (Plane p in planos)
            {
                float dist;
                float t = 0;

                if (p.Raycast(ray_x, out dist))
                {
                    t = Vector3.Distance(pivote, ray_x.GetPoint(dist));
                    if (t < xDist || !xInterseccion)
                    {
                        xInterseccion = true;
                        xDist = t;
                    }
                }

                if (p.Raycast(ray_y, out dist))
                {
                    t = Vector3.Distance(pivote, ray_y.GetPoint(dist));
                    if (t < y_dist || !yInterseccion)
                    {
                        yInterseccion = true;
                        y_dist = t;
                    }
                }

                if (p.Raycast(ray_z, out dist))
                {
                    t = Vector3.Distance(pivote, ray_z.GetPoint(dist));
                    if (t < z_dist || !zInterseccion)
                    {
                        zInterseccion = true;
                        z_dist = t;
                    }
                }
            }

            int xIter = (int)(Mathf.Ceil(Mathf.Max(xDist, y_dist)) / ajusteValor);
            int yIter = (int)(Mathf.Ceil(Mathf.Max(xDist, z_dist)) / ajusteValor);
            int zIter = (int)(Mathf.Ceil(Mathf.Max(z_dist, y_dist)) / ajusteValor);

            int max = Mathf.Max(Mathf.Max(xIter, yIter), zIter);
            int div = 1;
            while (max / div > maxLineas)
            {
                div++;
            }

            Vector3[] vertices = null;
            Vector3[] normales = null;
            Color[] coloresN = null;
            int[] indices = null;

            List<Vector3> verticesA = new List<Vector3>();
            List<Vector3> normalesA = new List<Vector3>();
            List<Color> coloresA = new List<Color>();
            List<int> indicesA = new List<int>();

            DibujarMitadGrid(cam, pivote, up, right, ajusteValor * div, xIter / div, colores[0], alphaBump, out vertices, out normales, out coloresN, out indices, 0);
            verticesA.AddRange(vertices);
            normalesA.AddRange(normales);
            coloresA.AddRange(coloresN);
            indicesA.AddRange(indices);

            DibujarMitadGrid(cam, pivote, right, forward, ajusteValor * div, yIter / div, colores[1], alphaBump, out vertices, out normales, out coloresN, out indices, verticesA.Count);
            verticesA.AddRange(vertices);
            normalesA.AddRange(normales);
            coloresA.AddRange(coloresN);
            indicesA.AddRange(indices);

            DibujarMitadGrid(cam, pivote, forward, up, ajusteValor * div, zIter / div, colores[2], alphaBump, out vertices, out normales, out coloresN, out indices, verticesA.Count);
            verticesA.AddRange(vertices);
            normalesA.AddRange(normales);
            coloresA.AddRange(coloresN);
            indicesA.AddRange(indices);

            gridMesh.Clear();
            gridMesh.vertices = verticesA.ToArray();
            gridMesh.normals = normalesA.ToArray();
            gridMesh.subMeshCount = 1;
            gridMesh.uv = new Vector2[verticesA.Count];
            gridMesh.colors = coloresA.ToArray();
            gridMesh.SetIndices(indicesA.ToArray(), MeshTopology.Lines, 0);

        }

        /// <summary>
        /// <para>Dibuja el grid completo</para>
        /// </summary>
        /// <param name="cam">Camara</param>
        /// <param name="pivote">Pivote del grid</param>
        /// <param name="tan">Tan</param>
        /// <param name="bitan">Bitan</param>
        /// <param name="incremento">Incremento</param>
        /// <param name="iteracion">Iteracion del Grid</param>
        /// <param name="div">Division</param>
        /// <param name="clrSegundario">Color segundario del grid</param>
        /// <param name="alphaBump">Alpha</param>
        private static void DibujarFullGrid(Camera cam, Vector3 pivote, Vector3 tan, Vector3 bitan, float incremento, int iteracion, int div, Color clrSegundario, float alphaBump)// Dibuja el grid completo
        {
            Color primario = clrSegundario;
            primario.a += alphaBump;

            float len = iteracion * incremento;

            iteracion++;

            Vector3 start = pivote - tan * (len / 2f) - bitan * (len / 2f);
            start = MGridExtensions.AjustarValor(start, bitan + tan, incremento);

            float inc = incremento;
            int offsetTan = (int)((MGridExtensions.GetValorDeMascara(start, tan) % (inc * 10f)) / inc);
            int offsetBitan = (int)((MGridExtensions.GetValorDeMascara(start, bitan) % (inc * 10f)) / inc);

            Vector3[] lines = new Vector3[iteracion * 4];
            int[] indices = new int[iteracion * 4];
            Color[] colores = new Color[iteracion * 4];

            int v = 0, t = 0;

            for (int i = 0; i < iteracion; i++)
            {
                Vector3 a = start + tan * i * incremento;
                Vector3 b = start + bitan * i * incremento;

                lines[v + 0] = a;
                lines[v + 1] = a + bitan * len;

                lines[v + 2] = b;
                lines[v + 3] = b + tan * len;

                indices[t++] = v;
                indices[t++] = v + 1;
                indices[t++] = v + 2;
                indices[t++] = v + 3;

                Color col = (i + offsetTan) % 10 == 0 ? primario : clrSegundario;

                colores[v + 0] = col;
                colores[v + 1] = col;

                col = (i + offsetBitan) % 10 == 0 ? primario : clrSegundario;

                colores[v + 2] = col;
                colores[v + 3] = col;

                v += 4;
            }

            Vector3 nrm = Vector3.Cross(tan, bitan);
            Vector3[] nrms = new Vector3[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                nrms[i] = nrm;


            gridMesh.Clear();
            gridMesh.vertices = lines;
            gridMesh.normals = nrms;
            gridMesh.subMeshCount = 1;
            gridMesh.uv = new Vector2[lines.Length];
            gridMesh.colors = colores;
            gridMesh.SetIndices(indices, MeshTopology.Lines, 0);
        }
        #endregion
    }
}
