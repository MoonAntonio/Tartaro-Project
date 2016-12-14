//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// MGrid.cs (03/11/2016)                                              	        \\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion: Editor de MGrid    												\\
// Fecha Mod:       03/11/2016                                                  \\
// Ultima Mod: Version Inicial     												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using MoonPincho.Alexandria;
using MoonPincho.Extensiones;
#endregion

namespace MoonPincho.MEditor.MGrid
{
    /// <summary>
    /// <para>Inicializa grid al iniciar unity</para>
    /// </summary>
    [InitializeOnLoad]
    public static class MGridInicializacion
    {
        static MGridInicializacion()
        {
            if (EditorApplication.timeSinceStartup < 10f && EditorPrefs.GetBool(MStrings.MGridActivado))
                MGrid.Init();
        }
    }

    /// <summary>
    /// <para>Editor de MGrid  </para>
    /// </summary>
    public class MGrid : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Variables
        #region Constructor
        /// <summary>
        /// <para>Constructor de MGrid</para>
        /// </summary>
        public static MGrid instance                                            // Constructor de MGrid
        {
            get
            {
                if (_instance == null)
                {
                    MGrid[] editor = Resources.FindObjectsOfTypeAll<MGrid>();

                    if (editor != null && editor.Length > 0)
                    {
                        _instance = editor[0];

                        for (int i = 1; i < editor.Length; i++)
                        {
                            GameObject.DestroyImmediate(editor[i]);
                        }
                    }
                }

                return _instance;
            }

            set
            {
                _instance = value;
            }
        }
        /// <summary>
        /// <para>Constructor de MGrid</para>
        /// </summary>
        private static MGrid _instance;                                         // Constructor de MGrid
        #endregion

        #region Variables Privadas
        /// <summary>
        /// <para>Ruta de los iconos de MGrid</para>
        /// </summary>
        private string MGridIconsPath = "Assets/Moon Pincho/Dev/MGrid/GUI";     // Ruta de los iconos de MGrid
        /// <summary>
        /// <para>Color antiguo</para>
        /// </summary>
        private Color viejoColor;                                               // Color antiguo
        /// <summary>
        /// <para>Unidad del grid</para>
        /// </summary>
        private UnidadGrid ajusteUnidad = UnidadGrid.Metros;                    // Unidad del grid
        /// <summary>
        /// <para>El valor instantaneo real, teniendo en cuenta el tamaño de la unidad</para>
        /// </summary>
        private float valorAjuste = 1f;                                         // El valor instantaneo real, teniendo en cuenta el tamaño de la unidad
        /// <summary>
        /// <para>El valor que el usuario ve</para>
        /// </summary>
        private float usuarioValorAjuste = 1f;                                  // El valor que el usuario ve
        /// <summary>
        /// <para>Fijar las axis</para>
        /// </summary>
        private bool usarAxisConstantes = false;                                // Fijar las axis
        /// <summary>
        /// <para>Renderizado del plano del grid</para>
        /// </summary>
        private Axis renderPlano = Axis.Y;                                      // Renderizado del plano del grid
        /// <summary>
        /// <para>Activar el ajuste</para>
        /// </summary>
        private bool ajusteEnabled = true;                                      // Activar el ajuste
        /// <summary>
        /// <para>Activado/Desactivado el arrastre del grid</para>
        /// </summary>
        private bool drawGrid = true;                                           // Activado/Desactivado el arrastre del grid
        /// <summary>
        /// <para>Activado/Desactivado el arrastre de angulos</para>
        /// </summary>
        private bool drawAngulos = false;                                       // Activado/Desactivado el arrastre de angulos
        /// <summary>
        /// <para>Activado/Desactivado el repintado del grid</para>
        /// </summary>
        private bool gridRepintar = true;                                       // Activado/Desactivado el repintado del grid
        /// <summary>
        /// <para>Activado/Desactivado el bloqueo del grid</para>
        /// </summary>
        private bool bloquearGrid = false;                                      // Activado/Desactivado el bloqueo del grid
        /// <summary>
        /// <para>Textura para abrir el editor</para>
        /// </summary>
        private Texture2D icono_Abrir;                                          // Textura para abrir el editor
        /// <summary>
        /// <para>Textura para cerrar el editor</para>
        /// </summary>
        private Texture2D icono_Cierre;                                         // Textura para cerrar el editor
        /// <summary>
        /// <para>Estilo del GUI</para>
        /// </summary>
        private GUISkin stiloSkin;                                              // Estilo del GUI
        /// <summary>
        /// <para>La incrementacion del color primario</para>
        /// </summary>
        private int IncrementacionColorPrimario = 10;                           // La incrementacion del color primario
        /// <summary>
        /// <para>Pivote General</para>
        /// </summary>
        private Vector3 pivote = Vector3.zero;                                  // Pivote General
        /// <summary>
        /// <para>Color previo</para>
        /// </summary>
        private Color colorPrevio;                                              // Color previo
        /// <summary>
        /// <para>GameObject temporal</para>
        /// </summary>
        private GameObject go;                                                  // GameObject temporal
        /// <summary>
        /// <para>Estilo de los botones del grid</para>
        /// </summary>
        private GUIStyle gridBtnStyle = new GUIStyle();                         // Estilo de los botones del grid
        /// <summary>
        /// <para>Estilo extendido del grid</para>
        /// </summary>
        private GUIStyle extenStyle = new GUIStyle();                           // Estilo extendido del grid
        /// <summary>
        /// <para>Estilo de los botones del grid en blanco</para>
        /// </summary>
        private GUIStyle gridBtnStyleBlanco = new GUIStyle();                   // Estilo de los botones del grid en blanco
        /// <summary>
        /// <para>Estilo de fondo </para>
        /// </summary>
        private GUIStyle backgroundStyle = new GUIStyle();                      // Estilo de fondo
        /// <summary>
        /// <para>Condicional para la inicializacion de la GUI</para>
        /// </summary>
        private bool inicializacionGUI = false;                                 // Condicional para la inicializacion de la GUI
        /// <summary>
        /// <para>Ultimo transform registrado</para>
        /// </summary>
        private Transform ultimoTransform;                                      // Ultimo transform registrado
        /// <summary>
        /// <para>Toggle de axis constantes</para>
        /// </summary>
        private bool toggleAxisConstantes = false;                              // Toggle de axis constantes
        /// <summary>
        /// <para>Toogle temporal</para>
        /// </summary>
        private bool toggleTempAjustado = false;                                // Toogle temporal
        /// <summary>
        /// <para>Ultima posicion registrada</para>
        /// </summary>
        private Vector3 ultimaPosition = Vector3.zero;                          // Ultima posicion registrada
        /// <summary>
        /// <para>Ultima escala registrada</para>
        /// </summary>
        private Vector3 ultimaEscala = Vector3.one;                             // Ultima escala registrada
        /// <summary>
        /// <para>Ultimo pivote registrado</para>
        /// </summary>
        private Vector3 ultimoPivote = Vector3.zero;                            // Ultimo pivote registrado
        /// <summary>
        /// <para>Direccion de la camara actual</para>
        /// </summary>
        private Vector3 camDir = Vector3.zero;                                  // Direccion de la camara actual
        /// <summary>
        /// <para>Antigua direccion de la camara</para>
        /// </summary>
        private Vector3 prevCamDir = Vector3.zero;                              // Antigua direccion de la camara
        /// <summary>
        /// <para>Distancia de la camara al pivote en la ultima vez que se actualizo el grid.</para>
        /// </summary>
        private float ultimaDistancia = 0f;                                     // Distancia de la camara al pivote en la ultima vez que se actualizo el grid
        /// <summary>
        /// <para>Primer movimiento</para>
        /// </summary>
        private bool primerMovimiento = true;                                   // Primer movimiento
        /// <summary>
        /// <para>Antigua camara ortografica</para>
        /// </summary>
        private bool prevOrtho = false;                                         // Antigua camara ortografica
        /// <summary>
        /// <para>Grid de arrastres</para>
        /// </summary>
        private float arrastrePlanoGrid = 0f;                                   // Grid de arrastres
        /// <summary>
        /// <para>Menu extendido</para>
        /// </summary>
        private const int menuExtendido = 8;                                    // Menu extendido
        /// <summary>
        /// <para>PAD Temporal</para>
        /// </summary>
        private const int PAD = 3;                                              // PAD Temporal
        /// <summary>
        /// <para>Rectangulo de la GUI</para>
        /// </summary>
        private Rect r = new Rect(8, menuExtendido, 42, 16);                    // Rectangulo de la GUI
        /// <summary>
        /// <para>Fondo del rectangulo</para>
        /// </summary>
        private Rect backgroundRect = new Rect(0, 0, 0, 0);                     // Fondo del rectangulo
        /// <summary>
        /// <para>Rectangulo del boton extendido</para>
        /// </summary>
        private Rect rectBtnExtendido = new Rect(0, 0, 0, 0);                   // Rectangulo del boton extendido
        /// <summary>
        /// <para>Condicional de Menu abierto</para>
        /// </summary>
        private bool menuOpen = true;                                           // Condicional de Menu abierto
        /// <summary>
        /// <para>Menu Start</para>
        /// </summary>
        private float menuStart = menuExtendido;                                // Menu Start
        /// <summary>
        /// <para>Velocidad de movimiento del menu</para>
        /// </summary>
        private const float velMenu = 500f;                                     // Velocidad de movimiento del menu
        /// <summary>
        /// <para>Temporal para calculo del delta time</para>
        /// </summary>
        private float deltaTime = 0f;                                           // Temporal para calculo del delta time
        /// <summary>
        /// <para>Temporal para saber el tiempo antes del deltaTime y calcular el ping</para>
        /// </summary>
        private float lastTime = 0f;                                            // Temporal para saber el tiempo antes del deltaTime y calcular el ping
        /// <summary>
        /// <para>Velocidad de desvanecimiento del menu</para>
        /// </summary>
        private const float velDesvanecimiento = 2.5f;                          // Velocidad de desvanecimiento del menu
        /// <summary>
        /// <para>Desvanecimiento del fondo del menu</para>
        /// </summary>
        private float backgroundFade = 1f;                                      // Desvanecimiento del fondo del menu
        /// <summary>
        /// <para>Condicional para saber si el raton esta dentro del menu</para>
        /// </summary>
        private bool mouseOverMenu = false;                                     // Condicional para saber si el raton esta dentro del menu
        /// <summary>
        /// <para>Color del fondo del menu</para>
        /// </summary>
        private Color menuBackgroundColor = new Color(0f, 0f, 0f, .5f);         // Color del fondo del menu
        /// <summary>
        /// <para>Color del menu extendido normal</para>
        /// </summary>
        private Color extendoNormalColor = new Color(.9f, .9f, .9f, .7f);       // Color del menu extendido normal
        /// <summary>
        /// <para>Color del menu extendido hover</para>
        /// </summary>
        private Color extendoHoverColor = new Color(0f, 1f, .4f, 1f);           // Color del menu extendido hover
        /// <summary>
        /// <para>Condicional del btn hover</para>
        /// </summary>
        private bool extendidoBtnHover = false;                                 // Condicional del btn hover
        /// <summary>
        /// <para>Condicional del menu en ortografico</para>
        /// </summary>
        private bool menuEnOrtho = false;                                       // Condicional del menu en ortografico
        #endregion

        #region Variables Publicas
        /// <summary>
        /// <para>Valor del angulo del grid</para>
        /// </summary>
        public float valorAngulo = 45f;                                         // Valor del angulo del grid
        /// <summary>
        /// <para>Activado/Desactivado la prediccion del grid</para>
        /// </summary>
        public bool prediccionGrid = true;                                      // Activado/Desactivado la prediccion del grid
        /// <summary>
        /// <para>El alphaBump temporal</para>
        /// </summary>
        public static float alphaBump;                                          // El alphaBump temporal
        /// <summary>
        /// <para>Color del Grid en X</para>
        /// </summary>
        public Color gridColorX;                                                // Color del Grid en X
        /// <summary>
        /// <para>Color del Grid en Y</para>
        /// </summary>
        public Color gridColorY;                                                // Color del Grid en Y
        /// <summary>
        /// <para>Color del Grid en Z</para>
        /// </summary>
        public Color gridColorZ;                                                // Color del Grid en Z
        /// <summary>
        /// <para>Color primario del Grid en X</para>
        /// </summary>
        public Color gridColorXPrimario;                                        // Color primario del Grid en X
        /// <summary>
        /// <para>Color primario del Grid en Y</para>
        /// </summary>
        public Color gridColorYPrimario;                                        // Color primario del Grid en Y
        /// <summary>
        /// <para>Color primario del Grid en Z</para>
        /// </summary>
        public Color gridColorZPrimario;                                        // Color primario del Grid en Z
        /// <summary>
        /// <para>OffSet usado en el menu</para>
        /// </summary>
        public float offset = 0f;                                               // OffSet usado en el menu
        #endregion

        #region Propiedades
        /// <summary>
        /// <para>Ajuste de la escala del grid</para>
        /// </summary>
        private bool _mGridAjusteEscalaActivado = false;                        // Ajuste de la escala del grid
        /// <summary>
        /// <para>Ajuste de la escala del grid</para>
        /// </summary>
        public bool MGridAjusteEscalaActivado                                   // Ajuste de la escala del grid
        {
            get
            {
                return EditorPrefs.HasKey(MStrings.MGridAjusteEscalaActivado) ? EditorPrefs.GetBool(MStrings.MGridAjusteEscalaActivado) : false;
            }

            set
            {
                _mGridAjusteEscalaActivado = value;
                EditorPrefs.SetBool(MStrings.MGridAjusteEscalaActivado, _mGridAjusteEscalaActivado);
            }
        }
        /// <summary>
        /// <para>Grid entero</para>
        /// </summary>
        public bool fullGrid { get; private set; }                              // Grid entero
        /// <summary>
        /// <para>Ajuste de los grupos del grid</para>
        /// </summary>
        private bool _mGridAjusteDeGrupo = true;                                // Ajuste de los grupos del grid
        /// <summary>
        /// <para>Ajuste de los grupos del grid</para>
        /// </summary>
        public bool MGridAjusteDeGrupo                                          // Ajuste de los grupos del grid
        {
            get
            {
                return EditorPrefs.HasKey(MStrings.MGridAjusteDeGrupo) ? EditorPrefs.GetBool(MStrings.MGridAjusteDeGrupo) : true;
            }

            set
            {
                _mGridAjusteDeGrupo = value;
                EditorPrefs.SetBool(MStrings.MGridAjusteDeGrupo, _mGridAjusteDeGrupo);
            }
        }
        /// <summary>
        /// <para>Camara ortografica</para>
        /// </summary>
        public bool ortografico { get; private set; }                           // Camara ortografica
        /// <summary>
        /// <para>Menu oculto</para>
        /// </summary>
        private int menuOculto { get { return menuEnOrtho ? -192 : -173; } }    // Menu oculto<
        #endregion

        #region Constantes
        /// <summary>
        /// <para>Altura de la GUI</para>
        /// </summary>
	    public const int guiAltura = 240;                                       // Altura de la GUI
        /// <summary>
        /// <para>La cantidad maxima de lineas que se mostraran en la pantalla</para>
        /// </summary>
        public const int guiMaxLineas = 150;                                    // La cantidad maxima de lineas que se mostraran en la pantalla
        /// <summary>
        /// <para>Tamaño del boton de la GUI</para>
        /// </summary>
        public const int sizeBtn = 46;                                          // Tamaño del boton de la GUI
        /// <summary>
        /// <para>Key constante del axis</para>
        /// </summary>
        private const string keyAxisConstantes = "s";                           // Key constante del axis
        /// <summary>
        /// <para>Key temporal</para>
        /// </summary>
        private const string keyTempDesabilitada = "d";                         // Key temporal
        #endregion

        #region GUIContent
        [SerializeField]
        private MGGuiContent MGrid_AjusteGrid = new MGGuiContent("Ajustar", "", "Ajustar todos los objetos de la escena seleccionados al grid.");
        [SerializeField]
        private MGGuiContent MGrid_GridEnabled = new MGGuiContent("Ocultar", "Mostrar", "Activa o desactiva el dibujo de las lineas de guia.");
        [SerializeField]
        private MGGuiContent MGrid_AjusteEnabled = new MGGuiContent("On", "Off", "Activar o Desactivar el S Modular.");
        [SerializeField]
        private MGGuiContent MGrid_BloqueoGrid = new MGGuiContent("Bloqueo", "Desbloqueo", "Bloquear el centro del grid de la perspectiva.");
        [SerializeField]
        private MGGuiContent MGrid_AnguloEnabled = new MGGuiContent("> On", "> Off", "Si esta activado, MGrid dibujara grid de linea anguladas. El angulo se puede ajustar en grados.");
        [SerializeField]
        private MGGuiContent MGrid_RenderPlanoX = new MGGuiContent("X", "X", "Renderizar grid en el plano X.");
        [SerializeField]
        private MGGuiContent MGrid_RenderPlanoY = new MGGuiContent("Y", "Y", "Renderizar grid en el plano Y.");
        [SerializeField]
        private MGGuiContent MGrid_RenderPlanoZ = new MGGuiContent("Z", "Z", "Renderizar grid en el plano Z.");
        [SerializeField]
        private MGGuiContent MGrid_RenderPerspectivaGrid = new MGGuiContent("Full", "Plano", "Renderizar en 3D");
        [SerializeField]
        private GUIContent MGrid_MenuExtendido = new GUIContent("", "Muestra o oculta el menu.");
        [SerializeField]
        private GUIContent MGrid_Ajuste = new GUIContent("", "Selecciona el ajuste.");
        #endregion

        #region Eventos
        [SerializeField]
        private static List<System.Action<float>> pushToGridListeners = new List<System.Action<float>>();
        [SerializeField]
        private static List<System.Action<bool>> toolbarEventSubscribers = new List<System.Action<bool>>();
        #endregion
        #endregion

        #region Actualizadores
        /// <summary>
        /// <para>Actualizador</para>
        /// </summary>
        private void Update()// Actualizador
        {
            deltaTime = Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;

            if (menuOpen && menuStart < menuExtendido || !menuOpen && menuStart > menuOculto)
            {
                menuStart += deltaTime * velMenu * (menuOpen ? 1f : -1f);
                menuStart = Mathf.Clamp(menuStart, menuOculto, menuExtendido);
                RenderSceneView();
            }

            float a = menuBackgroundColor.a;
            backgroundFade = (mouseOverMenu || !menuOpen) ? velDesvanecimiento : -velDesvanecimiento;

            menuBackgroundColor.a = Mathf.Clamp(menuBackgroundColor.a + backgroundFade * deltaTime, 0f, .5f);
            extendoNormalColor.a = menuBackgroundColor.a;
            extendoHoverColor.a = (menuBackgroundColor.a / .5f);

            if (!Mathf.Approximately(menuBackgroundColor.a, a))
                RenderSceneView();
        }
        #endregion

        #region Metodos
        /// <summary>
        /// <para>Carga los datos de MGrid</para>
        /// </summary>
        public void Cargar()// Carga los datos de MGrid
        {
            // FIX Serializar para que no tire error
            /*if (EditorPrefs.HasKey(MStrings.MGridAjusteEnabled))
            {
                ajusteEnabled = EditorPrefs.GetBool(MStrings.MGridAjusteEnabled);
            }

            SetValorAjuste(
                EditorPrefs.HasKey(MStrings.MGridUnidades) ? (UnidadGrid)EditorPrefs.GetInt(MStrings.MGridUnidades) : UnidadGrid.Metros,
                EditorPrefs.HasKey(MStrings.MGridValorAjuste) ? EditorPrefs.GetFloat(MStrings.MGridValorAjuste) : 1,
                EditorPrefs.HasKey(MStrings.MGridMultiplicadorAjuste) ? EditorPrefs.GetInt(MStrings.MGridMultiplicadorAjuste) : 100
                );

            if (EditorPrefs.HasKey(MStrings.MGridUsarAxisConstantes))
                usarAxisConstantes = EditorPrefs.GetBool(MStrings.MGridUsarAxisConstantes);

            bloquearGrid = EditorPrefs.GetBool(MStrings.MGridBloqueo);

            if (bloquearGrid)
            {
                if (EditorPrefs.HasKey(MStrings.MGridBloqueoPivote))
                {
                    string piv = EditorPrefs.GetString(MStrings.MGridBloqueoPivote);
                    string[] pivsplit = piv.Replace("(", "").Replace(")", "").Split(',');

                    float x, y, z;
                    if (!float.TryParse(pivsplit[0], out x)) goto NoParseForYou;
                    if (!float.TryParse(pivsplit[1], out y)) goto NoParseForYou;
                    if (!float.TryParse(pivsplit[2], out z)) goto NoParseForYou;

                    pivote.x = x;
                    pivote.y = y;
                    pivote.z = z;

                NoParseForYou:
                    ;
                }

            }

            fullGrid = EditorPrefs.GetBool(MStrings.MGridPerspectiva);

            renderPlano = EditorPrefs.HasKey(MStrings.MGridAxis) ? (Axis)EditorPrefs.GetInt(MStrings.MGridAxis) : Axis.Y;

            alphaBump = .1f;

            gridColorX = new Color(.9f, .46f, .46f, .08f);
            gridColorXPrimario = new Color(gridColorX.r, gridColorX.g, gridColorX.b, gridColorX.a + alphaBump);
            gridColorY = new Color(.46f, .9f, .46f, .08f);
            gridColorYPrimario = new Color(gridColorY.r, gridColorY.g, gridColorY.b, gridColorY.a + alphaBump);
            gridColorZ = new Color(.46f, .46f, .9f, .08f);
            gridColorZPrimario = new Color(gridColorZ.r, gridColorZ.g, gridColorZ.b, gridColorZ.a + alphaBump);

            drawGrid = true;

            prediccionGrid = EditorPrefs.HasKey(MStrings.MGridPrediccion) ? EditorPrefs.GetBool(MStrings.MGridPrediccion) : true;

            _mGridAjusteDeGrupo = MGridAjusteDeGrupo;
            _mGridAjusteEscalaActivado = MGridAjusteEscalaActivado;
            */
        }

        /// <summary>
        /// <para>Cierra MGrid</para>
        /// </summary>
        public void Cerrar()// Cierra MGrid
        {
            EditorPrefs.SetBool(MStrings.MGridActivado, false);
            GameObject.DestroyImmediate(this);
        }

        /// <summary>
        /// <para>Cierra MGrid</para>
        /// </summary>
        /// <param name="estaSiendoDestruido">Si ya esta siendo destruido</param>
        public void Cerrar(bool estaSiendoDestruido)// Cierra MGrid
        {
            MGridMainRenderer.Destroy();

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            EditorApplication.update -= Update;
            EditorApplication.hierarchyWindowChanged -= HierarchyWindowChanged;

            instance = null;

            foreach (System.Action<bool> listener in toolbarEventSubscribers)
                listener(false);

            SceneView.RepaintAll();
        }

        /// <summary>
        /// <para>Cuando se destruye el grid</para>
        /// </summary>
        private void OnDestroy()// Cuando se destruye el grid
        {
            this.Cerrar(true);
        }

        /// <summary>
        /// <para>Carga de Recursos</para>
        /// </summary>
        private void CargarGUIResources()// Carga de Recursos
        {
            MGrid_GridEnabled.imagenOn = CargarIconos("MGrid_GUI_PGrid_V_On.png");
            MGrid_GridEnabled.imagenOff = CargarIconos("MGrid_GUI_PGrid_V_Off.png");

            MGrid_AjusteEnabled.imagenOn = CargarIconos("MGrid_GUI_PGrid_Modular_On.png");
            MGrid_AjusteEnabled.imagenOff = CargarIconos("MGrid_GUI_PGrid_Modular_Off.png");

            MGrid_AjusteGrid.imagenOn = CargarIconos("MGrid_GUI_PGrid_Grid_Normal.png");

            MGrid_BloqueoGrid.imagenOn = CargarIconos("MGrid_GUI_PGrid_Bloqueo_On.png");
            MGrid_BloqueoGrid.imagenOff = CargarIconos("MGrid_GUI_PGrid_Bloqueo_Off.png");

            MGrid_AnguloEnabled.imagenOn = CargarIconos("MGrid_GUI_Angulo_On.png");
            MGrid_AnguloEnabled.imagenOff = CargarIconos("MGrid_GUI_Angulo_Off.png");

            MGrid_RenderPlanoX.imagenOn = CargarIconos("MGrid_GUI_PGrid_X_On.png");
            MGrid_RenderPlanoX.imagenOff = CargarIconos("MGrid_GUI_PGrid_X_Off.png");

            MGrid_RenderPlanoY.imagenOn = CargarIconos("MGrid_GUI_PGrid_Y_On.png");
            MGrid_RenderPlanoY.imagenOff = CargarIconos("MGrid_GUI_PGrid_Y_Off.png");

            MGrid_RenderPlanoZ.imagenOn = CargarIconos("MGrid_GUI_PGrid_Z_On.png");
            MGrid_RenderPlanoZ.imagenOff = CargarIconos("MGrid_GUI_PGrid_Z_Off.png");

            MGrid_RenderPerspectivaGrid.imagenOn = CargarIconos("MGrid_GUI_PGrid_On.png");
            MGrid_RenderPerspectivaGrid.imagenOff = CargarIconos("MGrid_GUI_PGrid_Off.png");

            icono_Abrir = CargarIconos("MGrid_Menu_Abierto.png");
            icono_Cierre = CargarIconos("MGrid_Menu_Cerrado.png");
        }

        /// <summary>
        /// <para>Carga el icono indicado</para>
        /// </summary>
        /// <param name="nombreIcono">Nombre del icono</param>
        /// <returns>Carga el icono indicado</returns>
        private Texture2D CargarIconos(string nombreIcono)// Carga el icono indicado
        {
            string iconPath = MGridIconsPath + "/" + nombreIcono;
            if (!File.Exists(iconPath))
            {
                string[] path = Directory.GetFiles("Assets", nombreIcono, SearchOption.AllDirectories);

                if (path.Length > 0)
                {
                    MGridIconsPath = Path.GetDirectoryName(path[0]);
                    iconPath = path[0];
                }
                else
                {

                    MDebug.LogError("Fallo en la carga del icono: " + nombreIcono + ".\nNo se ha encontrado el icono.");
                    return (Texture2D)null;
                }
            }

            return LoadAssetAtPath<Texture2D>(iconPath);
        }

        /// <summary>
        /// <para>Carga asset desde una ruta</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object// Carga asset desde una ruta
        {
            return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }

        /// <summary>
        /// <para>Renderiza la Scene View</para>
        /// </summary>
        private void RenderSceneView()// Renderiza la Scene View
        {
            SceneView.RepaintAll();
        }

        /// <summary>
        /// <para>Cuando la escena esta on , se representa la GUI</para>
        /// </summary>
        /// <param name="scnview">SceneView activa</param>
        public void OnSceneGUI(SceneView scnview)// Cuando la escena esta on , se representa la GUI
        {
            bool isCurrentView = scnview == SceneView.lastActiveSceneView;

            if (isCurrentView)
            {
                Handles.BeginGUI();
                DibujarSceneGUI();
                Handles.EndGUI();
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)

                return;
            Event e = Event.current;

            if (isCurrentView && e.type == EventType.MouseMove)
            {
                bool tmp = extendidoBtnHover;
                extendidoBtnHover = rectBtnExtendido.Contains(e.mousePosition);

                if (extendidoBtnHover != tmp)
                    RenderSceneView();

                mouseOverMenu = backgroundRect.Contains(e.mousePosition);
            }

            if (e.Equals(Event.KeyboardEvent(keyAxisConstantes)))
            {
                toggleAxisConstantes = true;
            }

            if (e.Equals(Event.KeyboardEvent(keyTempDesabilitada)))
            {
                toggleTempAjustado = true;
            }

            if (e.type == EventType.KeyUp)
            {
                toggleAxisConstantes = false;
                toggleTempAjustado = false;
                bool used = true;

                switch (e.keyCode)
                {
                    case KeyCode.Equals:
                        IncrementarGridSize();
                        break;

                    case KeyCode.Minus:
                        DecrementarGridSize();
                        break;

                    default:
                        used = false;
                        break;
                }

                if (used)
                    e.Use();
            }

            Camera cam = Camera.current;

            if (cam == null)
                return;

            ortografico = cam.orthographic && Redondear(scnview.rotation.eulerAngles.normalized);

            camDir = MGridExtensions.SueloSuperior(pivote - cam.transform.position);

            if (ortografico && !prevOrtho || ortografico != menuEnOrtho)
                OnSceneOrtho(isCurrentView);

            if (!ortografico && prevOrtho)
                OnScenePerspec(isCurrentView);

            prevOrtho = ortografico;

            float camDistance = Vector3.Distance(cam.transform.position, ultimoPivote);

            if (fullGrid)
            {
                pivote = bloquearGrid || Selection.activeTransform == null ? pivote : Selection.activeTransform.position;
            }
            else
            {
                Vector3 sceneViewPlanePivot = pivote;

                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                Plane plane = new Plane(Vector3.up, pivote);
                float dist;

                if ((bloquearGrid && !cam.MFrustum(pivote)) || !bloquearGrid || scnview != SceneView.lastActiveSceneView)
                {
                    if (plane.Raycast(ray, out dist))
                        sceneViewPlanePivot = ray.GetPoint(Mathf.Min(dist, arrastrePlanoGrid / 2f));
                    else
                        sceneViewPlanePivot = ray.GetPoint(Mathf.Min(cam.farClipPlane / 2f, arrastrePlanoGrid / 2f));
                }

                if (bloquearGrid)
                {
                    pivote = MGridEnum.InvertirAxisMask(sceneViewPlanePivot, renderPlano) + MGridEnum.AxisMask(pivote, renderPlano);
                }
                else
                {
                    pivote = Selection.activeTransform == null ? pivote : Selection.activeTransform.position;

                    if (Selection.activeTransform == null || !cam.MFrustum(pivote))
                    {
                        pivote = MGridEnum.InvertirAxisMask(sceneViewPlanePivot, renderPlano) + MGridEnum.AxisMask(Selection.activeTransform == null ? pivote : Selection.activeTransform.position, renderPlano);

                    }
                }
            }

            if (drawGrid)
            {
                if (ortografico)
                {
                    DibujarGridOrtografico(cam);
                }
                else
                {
                    if (gridRepintar || pivote != ultimoPivote || Mathf.Abs(camDistance - ultimaDistancia) > ultimaDistancia / 2 || camDir != prevCamDir)
                    {
                        prevCamDir = camDir;
                        gridRepintar = false;
                        ultimoPivote = pivote;
                        ultimaDistancia = camDistance;

                        if (fullGrid)
                        {
                            MGridMainRenderer.DibujarPerspectivaGrid(cam, pivote, valorAjuste, new Color[3] { gridColorX, gridColorY, gridColorZ }, alphaBump);
                        }
                        else
                        {
                            if ((renderPlano & Axis.X) == Axis.X)
                                arrastrePlanoGrid = MGridMainRenderer.DibujarPlano(cam, pivote + Vector3.right * offset, Vector3.up, Vector3.forward, valorAjuste, gridColorX, alphaBump);

                            if ((renderPlano & Axis.Y) == Axis.Y)
                                arrastrePlanoGrid = MGridMainRenderer.DibujarPlano(cam, pivote + Vector3.up * offset, Vector3.right, Vector3.forward, valorAjuste, gridColorY, alphaBump);

                            if ((renderPlano & Axis.Z) == Axis.Z)
                                arrastrePlanoGrid = MGridMainRenderer.DibujarPlano(cam, pivote + Vector3.forward * offset, Vector3.up, Vector3.right, valorAjuste, gridColorZ, alphaBump);

                        }
                    }
                }
            }
            if (!Selection.transforms.Constantes(ultimoTransform))
            {
                if (Selection.activeTransform)
                {
                    ultimoTransform = Selection.activeTransform;
                    ultimaPosition = Selection.activeTransform.position;
                    ultimaEscala = Selection.activeTransform.localScale;
                }
            }


            if (e.type == EventType.MouseUp)
                primerMovimiento = true;

            if (!ajusteEnabled || GUIUtility.hotControl < 1)
                return;

            if (Selection.activeTransform)
            {
                if (!FuzzyEquals(ultimoTransform.position, ultimaPosition))
                {
                    Transform selected = ultimoTransform;

                    if (!toggleTempAjustado)
                    {
                        Vector3 old = selected.position;
                        Vector3 mask = old - ultimaPosition;

                        bool constraintsOn = toggleAxisConstantes ? !usarAxisConstantes : usarAxisConstantes;

                        if (constraintsOn)
                            selected.position = MGridExtensions.AjustarValor(old, mask, valorAjuste);
                        else
                            selected.position = MGridExtensions.AjustarValor(old, valorAjuste);

                        Vector3 offset = selected.position - old;

                        if (prediccionGrid && primerMovimiento && !fullGrid)
                        {
                            primerMovimiento = false;
                            Axis dragAxis = MGridExtensions.CalcularArrastreDeAxis(offset, scnview.camera);

                            if (dragAxis != Axis.Null && dragAxis != renderPlano)
                                SetRenderPlano(dragAxis);
                        }

                        if (_mGridAjusteDeGrupo)
                        {
                            OffsetTransforms(Selection.transforms, selected, offset);
                        }
                        else
                        {
                            foreach (Transform t in Selection.transforms)
                                t.position = constraintsOn ? MGridExtensions.AjustarValor(t.position, mask, valorAjuste) : MGridExtensions.AjustarValor(t.position, valorAjuste);
                        }
                    }

                    ultimaPosition = selected.position;
                }

                if (!FuzzyEquals(ultimoTransform.localScale, ultimaEscala) && _mGridAjusteEscalaActivado)
                {
                    if (!toggleTempAjustado)
                    {
                        Vector3 old = ultimoTransform.localScale;
                        Vector3 mask = old - ultimaEscala;

                        if (prediccionGrid)
                        {
                            Axis dragAxis = MGridExtensions.CalcularArrastreDeAxis(Selection.activeTransform.TransformDirection(mask), scnview.camera);
                            if (dragAxis != Axis.Null && dragAxis != renderPlano)
                                SetRenderPlano(dragAxis);
                        }

                        foreach (Transform t in Selection.transforms)
                            t.localScale = MGridExtensions.AjustarValor(t.localScale, mask, valorAjuste);

                        ultimaEscala = ultimoTransform.localScale;
                    }
                }
            }
        }

        /// <summary>
        /// <para>Representa la GUI en la escena</para>
        /// </summary>
        private void DibujarSceneGUI()// Representa la GUI en la escena
        {
            GUI.backgroundColor = menuBackgroundColor;
            backgroundRect.x = r.x - 4;
            backgroundRect.y = 0;
            backgroundRect.width = r.width + 8;
            backgroundRect.height = r.y + r.height + PAD;
            GUI.Box(backgroundRect, "", backgroundStyle);
            backgroundRect.width += 32f;
            backgroundRect.height += 32f;
            GUI.backgroundColor = Color.white;

            if (!inicializacionGUI)
            {
                extenStyle.normal.background = menuOpen ? icono_Cierre : icono_Abrir;
                extenStyle.hover.background = menuOpen ? icono_Cierre : icono_Abrir;

                inicializacionGUI = true;
                backgroundStyle.normal.background = EditorGUIUtility.whiteTexture;

                Texture2D icon_button_normal = CargarIconos("MGrid_Btn_Normal.png");
                Texture2D icon_button_hover = CargarIconos("MGrid_Btn_Hover.png");

                if (icon_button_normal == null)
                {
                    gridBtnStyleBlanco = new GUIStyle("button");
                }
                else
                {
                    gridBtnStyleBlanco.normal.background = icon_button_normal;
                    gridBtnStyleBlanco.hover.background = icon_button_hover;
                    gridBtnStyleBlanco.normal.textColor = icon_button_normal != null ? Color.white : Color.black;
                    gridBtnStyleBlanco.hover.textColor = new Color(.7f, .7f, .7f, 1f);
                }

                gridBtnStyleBlanco.padding = new RectOffset(1, 2, 1, 2);
                gridBtnStyleBlanco.alignment = TextAnchor.MiddleCenter;
            }

            r.y = menuStart;

            MGrid_Ajuste.text = usuarioValorAjuste.ToString();

            if (GUI.Button(r, MGrid_Ajuste, gridBtnStyleBlanco))
            {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		        MGridWindows options = EditorWindow.GetWindow<MGridWindows>(true, "MGrid Ajustes", true);
		        Rect screenRect = SceneView.lastActiveSceneView.position;
		        options.editor = this;
		        options.position = new Rect(screenRect.x + r.x + r.width + PAD,
										screenRect.y + r.y + 24,
										256,
										162);
#else
                MGridWindows options = ScriptableObject.CreateInstance<MGridWindows>();
                Rect screenRect = SceneView.lastActiveSceneView.position;
                options.editor = this;
                options.ShowAsDropDown(new Rect(screenRect.x + r.x + r.width + PAD,
                                                screenRect.y + r.y + 24,
                                                0,
                                                0),
                                                new Vector2(256, 162));
#endif
            }

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_GridEnabled, drawGrid, gridBtnStyle, EditorStyles.miniButton))
                SetGridEnabled(!drawGrid);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_AjusteEnabled, ajusteEnabled, gridBtnStyle, EditorStyles.miniButton))
                SetAjusteEnabled(!ajusteEnabled);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_AjusteGrid, true, gridBtnStyle, EditorStyles.miniButton))
                SnapToGrid(Selection.transforms);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_BloqueoGrid, bloquearGrid, gridBtnStyle, EditorStyles.miniButton))
            {
                bloquearGrid = !bloquearGrid;
                EditorPrefs.SetBool(MStrings.MGridBloqueo, bloquearGrid);
                EditorPrefs.SetString(MStrings.MGridBloqueoPivote, pivote.ToString());

                if (!bloquearGrid)
                    offset = 0f;

                gridRepintar = true;

                RenderSceneView();
            }

            if (menuEnOrtho)
            {
                r.y += r.height + PAD;

                if (MGGuiContent.ToggleBtn(r, MGrid_AnguloEnabled, drawAngulos, gridBtnStyle, EditorStyles.miniButton))
                    SetDibujoDeAngulos(!drawAngulos);
            }

            r.y += r.height + PAD + 4;

            if (MGGuiContent.ToggleBtn(r, MGrid_RenderPlanoX, (renderPlano & Axis.X) == Axis.X && !fullGrid, gridBtnStyle, EditorStyles.miniButton))
                SetRenderPlano(Axis.X);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_RenderPlanoY, (renderPlano & Axis.Y) == Axis.Y && !fullGrid, gridBtnStyle, EditorStyles.miniButton))
                SetRenderPlano(Axis.Y);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_RenderPlanoZ, (renderPlano & Axis.Z) == Axis.Z && !fullGrid, gridBtnStyle, EditorStyles.miniButton))
                SetRenderPlano(Axis.Z);

            r.y += r.height + PAD;

            if (MGGuiContent.ToggleBtn(r, MGrid_RenderPerspectivaGrid, fullGrid, gridBtnStyle, EditorStyles.miniButton))
            {
                fullGrid = !fullGrid;
                gridRepintar = true;
                EditorPrefs.SetBool(MStrings.MGridPerspectiva, fullGrid);
                RenderSceneView();
            }

            r.y += r.height + PAD;

            rectBtnExtendido.x = r.x;
            rectBtnExtendido.y = r.y;
            rectBtnExtendido.width = r.width;
            rectBtnExtendido.height = r.height;

            GUI.backgroundColor = extendidoBtnHover ? extendoHoverColor : extendoNormalColor;
            MGrid_MenuExtendido.text = icono_Abrir == null ? (menuOpen ? "Cerrar" : "Abrir") : "";
            if (GUI.Button(r, MGrid_MenuExtendido, icono_Abrir ? extenStyle : gridBtnStyleBlanco))
            {
                ToggleMenuVisibility();
                extendidoBtnHover = false;
            }
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// <para>Decrementa el tamaño del Grid</para>
        /// </summary>
        public static void DecrementarGridSize()// Decrementa el tamaño del Grid
        {
            if (instance == null) return;

            int multiplier = EditorPrefs.HasKey(MStrings.MGridMultiplicadorAjuste) ? EditorPrefs.GetInt(MStrings.MGridMultiplicadorAjuste) : 100;
            float val = EditorPrefs.HasKey(MStrings.MGridValorAjuste) ? EditorPrefs.GetFloat(MStrings.MGridValorAjuste) : 1f;

            multiplier /= 2;

            instance.SetValorAjuste(instance.ajusteUnidad, val, multiplier);

            SceneView.RepaintAll();
        }

        /// <summary>
        /// <para>Incrementa el tamaño del grid</para>
        /// </summary>
        public static void IncrementarGridSize()// Incrementa el tamaño del grid
        {
            if (instance == null) return;

            int multiplier = EditorPrefs.HasKey(MStrings.MGridMultiplicadorAjuste) ? EditorPrefs.GetInt(MStrings.MGridMultiplicadorAjuste) : 100;
            float val = EditorPrefs.HasKey(MStrings.MGridValorAjuste) ? EditorPrefs.GetFloat(MStrings.MGridValorAjuste) : 1f;

            multiplier *= 2;

            instance.SetValorAjuste(instance.ajusteUnidad, val, multiplier);

            SceneView.RepaintAll();
        }

        /// <summary>
        /// <para>Cuando la escena esta en orthografica</para>
        /// </summary>
        /// <param name="esVistaActual">La vista actual de la escena</param>
        private void OnSceneOrtho(bool esVistaActual)// Cuando la escena esta en orthografica
        {
            MGridMainRenderer.Destroy();

            if (esVistaActual && ortografico != menuEnOrtho)
                SetMenuExten(menuOpen);
        }

        /// <summary>
        /// <para>Cuando la escena esta en perspectiva</para>
        /// </summary>
        /// <param name="esVistaActual"></param>
        private void OnScenePerspec(bool esVistaActual)// Cuando la escena esta en perspectiva
        {
            if (esVistaActual && ortografico != menuEnOrtho)
                SetMenuExten(menuOpen);
        }

        /// <summary>
        /// <para>Cuando la jerarquia cambia</para>
        /// </summary>
        private void HierarchyWindowChanged()// Cuando la jerarquia cambia
        {
            if (Selection.activeTransform != null)
                ultimaPosition = Selection.activeTransform.position;
        }

        /// <summary>
        /// <para>Visibilidad del menu</para>
        /// </summary>
        private void ToggleMenuVisibility()// Visibilidad del menu
        {
            menuOpen = !menuOpen;

            extenStyle.normal.background = menuOpen ? icono_Cierre : icono_Abrir;
            extenStyle.hover.background = menuOpen ? icono_Cierre : icono_Abrir;

            foreach (System.Action<bool> listener in toolbarEventSubscribers)
                listener(menuOpen);

            RenderSceneView();
        }

        /// <summary>
        /// <para>Igualar Fuzzy</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private static bool FuzzyEquals(Vector3 lhs, Vector3 rhs)// Igualar Fuzzy
        {
            return Mathf.Abs(lhs.x - rhs.x) < .001f && Mathf.Abs(lhs.y - rhs.y) < .001f && Mathf.Abs(lhs.z - rhs.z) < .001f;
        }

        /// <summary>
        /// <para>Transform del offset</para>
        /// </summary>
        /// <param name="trsfrms"></param>
        /// <param name="ignore"></param>
        /// <param name="offset"></param>
        public void OffsetTransforms(Transform[] trsfrms, Transform ignore, Vector3 offset)// Transform del offset
        {
            foreach (Transform t in trsfrms)
            {
                if (t != ignore)
                    t.position += offset;
            }
        }
        #endregion

        #region API
        /// <summary>
        /// <para>Fija el valor del ajuste del grid</para>
        /// </summary>
        /// <param name="uniGrid">Unidad del grid</param>
        /// <param name="val">Valor</param>
        /// <param name="multiplicador">Multiplicador del valor</param>
        public void SetValorAjuste(UnidadGrid uniGrid, float val, int multiplicador)// Fija el valor del ajuste del grid
        {
            int clampMultiplicador = (int)(Mathf.Min(Mathf.Max(25, multiplicador), 102400));
            float valorMultiplicado = clampMultiplicador / 100f;

		    valorAjuste = MGridEnum.ValorUnidadGrid(uniGrid) * val * valorMultiplicado;
            RenderSceneView();
		
		    EditorPrefs.SetInt(MStrings.MGridUnidades, (int)uniGrid);
		    EditorPrefs.SetFloat(MStrings.MGridValorAjuste, val);
		    EditorPrefs.SetInt(MStrings.MGridMultiplicadorAjuste, clampMultiplicador);
		
		    usuarioValorAjuste = val * valorMultiplicado;
            ajusteUnidad = uniGrid;

            IncrementacionColorPrimario = 10;

            gridRepintar = true;
        }

        /// <summary>
        /// <para>Fija el render del plano en un axis</para>
        /// </summary>
        /// <param name="axis">Axis a fijar el render</param>
        public void SetRenderPlano(Axis axis)// Fija el render del plano en un axis
        {
            offset = 0f;
            fullGrid = false;
            renderPlano = axis;
            EditorPrefs.SetBool(MStrings.MGridPerspectiva, fullGrid);
            EditorPrefs.SetInt(MStrings.MGridAxis, (int)renderPlano);
            gridRepintar = true;
            RenderSceneView();
        }

        /// <summary>
        /// <para>Fija el menu extendido</para>
        /// </summary>
        /// <param name="esExtendido">Condicional si esta extendido o sino</param>
        private void SetMenuExten(bool esExtendido)// Fija el menu extendido
        {
            menuOpen = esExtendido;
            menuEnOrtho = ortografico;
            menuStart = menuOpen ? menuExtendido : menuOculto;

            menuBackgroundColor.a = 0f;
            extendoNormalColor.a = menuBackgroundColor.a;
            extendoHoverColor.a = (menuBackgroundColor.a / .5f);

            extenStyle.normal.background = menuOpen ? icono_Cierre : icono_Abrir;
            extenStyle.hover.background = menuOpen ? icono_Cierre : icono_Abrir;

            foreach (System.Action<bool> listener in toolbarEventSubscribers)
                listener(menuOpen);
        }

        /// <summary>
        /// <para>Fijar los ajustes en activados o desactivados</para>
        /// </summary>
        /// <param name="enable">Condicional del ajuste</param>
        public void SetAjusteEnabled(bool enable)// Fijar los ajustes en activados o desactivados
        {
            EditorPrefs.SetBool(MStrings.MGridAjusteEnabled, enable);

            if (Selection.activeTransform)
            {
                ultimoTransform = Selection.activeTransform;
                ultimaPosition = Selection.activeTransform.position;
            }

            ajusteEnabled = enable;
            gridRepintar = true;
            RenderSceneView();
        }

        /// <summary>
        /// <para>Fija el valor del ajuste</para>
        /// </summary>
        /// <param name="mgrid">Unidad de medida actual</param>
        /// <param name="val">Valor a añadir</param>
        /// <param name="multiplicador">Multiplicador</param>
        public void SetAjusteValor(UnidadGrid mgrid, float val, int multiplicador)// Fija el valor del ajuste
        {
            int clamp_multiplier = (int)(Mathf.Min(Mathf.Max(25, multiplicador), 102400));

            float value_multiplier = clamp_multiplier / 100f;

		valorAjuste = MGridEnum.ValorUnidadGrid(mgrid) * val * value_multiplier;
		RenderSceneView();
		
		EditorPrefs.SetInt(MStrings.MGridUnidades, (int)mgrid);
		EditorPrefs.SetFloat(MStrings.MGridValorAjuste, val);
		EditorPrefs.SetInt(MStrings.MGridMultiplicadorAjuste, clamp_multiplier);
		
		usuarioValorAjuste = val * value_multiplier;
		ajusteUnidad = mgrid;

		switch(mgrid)
		{
			default:
				IncrementacionColorPrimario = 10;
				break;
		}

		gridRepintar = true;
        }

        /// <summary>
        /// <para>Fija el grid en activado o desactivado</para>
        /// </summary>
        /// <param name="enable">Condicional del grid</param>
        public void SetGridEnabled(bool enable)// Fija el grid en activado o desactivado
        {
            drawGrid = enable;
            if (!drawGrid)
                MGridMainRenderer.Destroy();

            EditorPrefs.SetBool("showgrid", enable);

            gridRepintar = true;
            RenderSceneView();
        }

        /// <summary>
        /// <para>Fija los angulos</para>
        /// </summary>
        /// <param name="enable">Condicional para dibujar los angulos</param>
        public void SetDibujoDeAngulos(bool enable)// Fija los angulos
        {
            drawAngulos = enable;
            gridRepintar = true;
            RenderSceneView();
        }

        /// <summary>
        /// <para>Fija la incrementacion del ajuste</para>
        /// </summary>
        /// <param name="inc">Incrementacion</param>
        public void SetAjusteIncremental(float inc)// Fija la incrementacion del ajuste
        {
            SetAjusteValor(ajusteUnidad, Mathf.Max(inc, .001f), 100);
        }

        /// <summary>
        /// <para>Ajustar al Grid</para>
        /// </summary>
        /// <param name="transforms"></param>
        private void SnapToGrid(Transform[] transforms)// Ajustar al Grid
        {
            Undo.RecordObjects(transforms as Object[], "Snap to Grid");

            foreach (Transform t in transforms)
                t.position = MGridExtensions.AjustarValor(t.position, valorAjuste);

            gridRepintar = true;

            PushToGrid(valorAjuste);
        }

        /// <summary>
        /// <para>Obtiene la incrementacion del ajuste</para>
        /// </summary>
        /// <returns>La incrementacion del ajuste</returns>
        public float GetAjusteIncremental()// Obtiene la incrementacion del ajuste
        {
            return usuarioValorAjuste;
        }
        #endregion

        #region Init
        #region Menus
        [MenuItem(MStrings.titNombreMGridON, false, MStrings.posMGridON)]
        public static void Init()
        {
            if (instance == null)
            {
                EditorPrefs.SetBool(MStrings.MGridActivado, true);
                instance = ScriptableObject.CreateInstance<MGrid>();
                instance.hideFlags = HideFlags.DontSave;
                EditorApplication.delayCall += instance.Initialize;
            }
            else
            {
                CerrarMGrid();
            }

            SceneView.RepaintAll();
        }

        [MenuItem(MStrings.titNombreMGridOFF, true, MStrings.posMGridOFF)]
        public static bool VerificarCierreMGrid()
        {
            return instance != null || Resources.FindObjectsOfTypeAll<MGrid>().Length > 0;
        }

        [MenuItem(MStrings.titNombreMGridOFF)]
        public static void CerrarMGrid()
        {
            foreach (MGrid editor in Resources.FindObjectsOfTypeAll<MGrid>())
                editor.Cerrar();
        }
        #endregion

        /// <summary>
        /// <para>Inicializa el sistema MGrid</para>
        /// </summary>
        public void Initialize()// Inicializa el sistema MGrid
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            EditorApplication.update += Update;
            EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

            CargarGUIResources();
            Cargar();
            instance = this;
            MGridMainRenderer.Init();

            SetMenuExten(false);
            lastTime = Time.realtimeSinceStartup;
            ToggleMenuVisibility();

            gridRepintar = true;
            RenderSceneView();
        }

        #region Serializacion
        /// <summary>
        /// <para>Antes de serializar</para>
        /// </summary>
        public void OnBeforeSerialize() { }// Antes de serializar

        /// <summary>
        /// <para>Despues de serializar</para>
        /// </summary>
        public void OnAfterDeserialize()// Despues de serializar
        {
            instance = this;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            EditorApplication.update += Update;
            EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

            Cargar();
        }
        #endregion
        #endregion

        #region Funcionalidad
        /// <summary>
        /// <para>Redondea temporalmente el grid</para>
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>El redondeo del vector</returns>
        public bool Redondear(Vector3 v)// Redondea temporalmente el grid
        {
            return (Mathf.Approximately(v.x, 1f) || Mathf.Approximately(v.y, 1f) || Mathf.Approximately(v.z, 1f)) || v == Vector3.zero;
        }

        /// <summary>
        /// <para>Ajusta la unidad con un string</para>
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>una Unidad</returns>
        public UnidadGrid AjustaUnidadConString(string str)// Ajusta la unidad con un string
        {
            foreach (UnidadGrid su in UnidadGrid.GetValues(typeof(UnidadGrid)))
            {
                if (su.ToString() == str)
                    return su;
            }
            return (UnidadGrid)0;
        }

        /// <summary>
        /// <para>Obtienes un Axis desde un Vector</para>
        /// </summary>
        /// <param name="val">Vector3 para el axis</param>
        /// <returns>Un Axis</returns>
        public Axis AxisConVector(Vector3 val)// Obtienes un Axis desde un Vector
        {
            Vector3 v = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), Mathf.Abs(val.z));

            if (v.x > v.y && v.x > v.z)
            {
                if (val.x > 0)
                    return Axis.X;
                else
                    return Axis.NegX;
            }
            else
            if (v.y > v.x && v.y > v.z)
            {
                if (val.y > 0)
                    return Axis.Y;
                else
                    return Axis.NegY;
            }
            else
            {
                if (val.z > 0)
                    return Axis.Z;
                else
                    return Axis.NegZ;
            }
        }

        /// <summary>
        /// <para>Obtienes un Vector desde un Axis</para>
        /// </summary>
        /// <param name="axis">Axis para el Vector</param>
        /// <returns>Un Vector</returns>
        public Vector3 VectorConAxis(Axis axis)// Obtienes un Vector desde un Axis
        {
            switch (axis)
            {
                case Axis.X:
                    return Vector3.right;
                case Axis.Y:
                    return Vector3.up;
                case Axis.Z:
                    return Vector3.forward;
                case Axis.NegX:
                    return -Vector3.right;
                case Axis.NegY:
                    return -Vector3.up;
                case Axis.NegZ:
                    return -Vector3.forward;

                default:
                    return Vector3.forward;
            }
        }

        /// <summary>
        /// <para>Redondea un axis</para>
        /// </summary>
        /// <param name="v">Vector 3</param>
        /// <returns>Redondeo</returns>
        public Vector3 RedondearAxis(Vector3 v)// Redondea un axis
        {
            return VectorConAxis(AxisConVector(v));
        }

        /// <summary>
        /// <para>Dibuja el Grid ortografico</para>
        /// </summary>
        /// <param name="cam">Camara</param>
        private void DibujarGridOrtografico(Camera cam)// Dibuja el Grid ortografico
        {
            Axis camAxis = AxisConVector(Camera.current.transform.TransformDirection(Vector3.forward).normalized);

            if (drawGrid)
            {
                switch (camAxis)
                {
                    case Axis.X:
                    case Axis.NegX:
                        DibujarGridOrtografico(cam, camAxis, gridColorXPrimario, gridColorX);
                        break;

                    case Axis.Y:
                    case Axis.NegY:
                        DibujarGridOrtografico(cam, camAxis, gridColorYPrimario, gridColorY);
                        break;

                    case Axis.Z:
                    case Axis.NegZ:
                        DibujarGridOrtografico(cam, camAxis, gridColorZPrimario, gridColorZ);
                        break;
                }
            }
        }

        /// <summary>
        /// <para>Dibuja el Grid ortografico</para>
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="camAxis"></param>
        /// <param name="primaryColor"></param>
        /// <param name="secondaryColor"></param>
        private void DibujarGridOrtografico(Camera cam, Axis camAxis, Color primaryColor, Color secondaryColor)// Dibuja el Grid ortografico
        {
            colorPrevio = Handles.color;
            Handles.color = primaryColor;

            Vector3 bottomLeft = MGridExtensions.AjustarSuelo(cam.ScreenToWorldPoint(Vector2.zero), valorAjuste);
            Vector3 bottomRight = MGridExtensions.AjustarSuelo(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, 0f)), valorAjuste);
            Vector3 topLeft = MGridExtensions.AjustarSuelo(cam.ScreenToWorldPoint(new Vector2(0f, cam.pixelHeight)), valorAjuste);
            Vector3 topRight = MGridExtensions.AjustarSuelo(cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, cam.pixelHeight)), valorAjuste);

            Vector3 axis = VectorConAxis(camAxis);

            float width = Vector3.Distance(bottomLeft, bottomRight);
            float height = Vector3.Distance(bottomRight, topRight);

            bottomLeft += axis * 10f;
            topRight += axis * 10f;
            bottomRight += axis * 10f;
            topLeft += axis * 10f;

            Vector3 cam_right = cam.transform.right;
            Vector3 cam_up = cam.transform.up;

            float _snapVal = valorAjuste;

            int segs = (int)Mathf.Ceil(width / _snapVal) + 2;

            float n = 2f;
            while (segs > guiMaxLineas)
            {
                _snapVal = _snapVal * n;
                segs = (int)Mathf.Ceil(width / _snapVal) + 2;
                n++;
            }

            Vector3 bl = cam_right.SumaVector() > 0 ? MGridExtensions.AjustarSuelo(bottomLeft, cam_right, _snapVal * IncrementacionColorPrimario) : MGridExtensions.AjustarCelda(bottomLeft, cam_right, _snapVal * IncrementacionColorPrimario);
            Vector3 start = bl - cam_up * (height + _snapVal * 2);
            Vector3 end = bl + cam_up * (height + _snapVal * 2);

            segs += IncrementacionColorPrimario;

            Vector3 line_start = Vector3.zero;
            Vector3 line_end = Vector3.zero;

            for (int i = -1; i < segs; i++)
            {
                line_start = start + (i * (cam_right * _snapVal));
                line_end = end + (i * (cam_right * _snapVal));
                Handles.color = i % IncrementacionColorPrimario == 0 ? primaryColor : secondaryColor;
                Handles.DrawLine(line_start, line_end);
            }

            segs = (int)Mathf.Ceil(height / _snapVal) + 2;

            n = 2;
            while (segs > guiMaxLineas)
            {
                _snapVal = _snapVal * n;
                segs = (int)Mathf.Ceil(height / _snapVal) + 2;
                n++;
            }

            Vector3 tl = cam_up.SumaVector() > 0 ? MGridExtensions.AjustarCelda(topLeft, cam_up, _snapVal * IncrementacionColorPrimario) : MGridExtensions.AjustarSuelo(topLeft, cam_up, _snapVal * IncrementacionColorPrimario);
            start = tl - cam_right * (width + _snapVal * 2);
            end = tl + cam_right * (width + _snapVal * 2);

            segs += (int)IncrementacionColorPrimario;

            for (int i = -1; i < segs; i++)
            {
                line_start = start + (i * (-cam_up * _snapVal));
                line_end = end + (i * (-cam_up * _snapVal));
                Handles.color = i % IncrementacionColorPrimario == 0 ? primaryColor : secondaryColor;
                Handles.DrawLine(line_start, line_end);
            }


		    if(drawAngulos)
		    {
			    Vector3 cen = MGridExtensions.AjustarValor(((topRight + bottomLeft) / 2f), valorAjuste);

			    float half = (width > height) ? width : height;

			    float opposite = Mathf.Tan( Mathf.Deg2Rad*valorAngulo ) * half;

			    Vector3 up = cam.transform.up * opposite;
			    Vector3 right = cam.transform.right * half;

			    Vector3 bottomLeftAngle 	= cen - (up+right);
			    Vector3 topRightAngle 		= cen + (up+right);

			    Vector3 bottomRightAngle	= cen + (right-up);
			    Vector3 topLeftAngle 		= cen + (up-right);

			    Handles.color = primaryColor;
			    Handles.DrawLine(bottomLeftAngle, topRightAngle);
			    Handles.DrawLine(topLeftAngle, bottomRightAngle);	
		    }

            Handles.color = colorPrevio;
        }

        /// <summary>
        /// <para>Usa Axis constantes</para>
        /// </summary>
        /// <returns></returns>
        public static bool usarAxisConstante()// Usa Axis constantes
        {
            return instance != null ? instance.GetUseAxisConstraints() : false;
        }

        /// <summary>
        /// <para>Ajusta el valor del ajuste</para>
        /// </summary>
        /// <returns></returns>
        public static float AjustarValor()// Ajusta el valor del ajuste
        {
            return instance != null ? instance.GetSnapValue() : 0f;
        }

        /// <summary>
        /// <para>Activa el Ajuste</para>
        /// </summary>
        /// <returns></returns>
        public static bool AjusteEnabled()// Activa el Ajuste
        {
            return instance == null ? false : instance.GetSnapEnabled();
        }

        #region Eventos Sistema
        public static void AddPushToGridListener(System.Action<float> listener)
        {
            pushToGridListeners.Add(listener);
        }
        public static void RemovePushToGridListener(System.Action<float> listener)
        {
            pushToGridListeners.Remove(listener);
        }
        public static void AddToolbarEventSubscriber(System.Action<bool> listener)
        {
            toolbarEventSubscribers.Add(listener);
        }
        public static void RemoveToolbarEventSubscriber(System.Action<bool> listener)
        {
            toolbarEventSubscribers.Remove(listener);
        }
        public static bool SceneToolbarActive()
        {
            return instance != null;
        }
        private void PushToGrid(float valorAjuste)
        {
            foreach (System.Action<float> listener in pushToGridListeners)
                listener(valorAjuste);
        }
        public static void OnHandleMove(Vector3 worldDirection)
        {
            if (instance != null)
                instance.OnHandleMove_Internal(worldDirection);
        }
        private void OnHandleMove_Internal(Vector3 worldDirection)
        {
            if (prediccionGrid && primerMovimiento && !fullGrid)
            {
                primerMovimiento = false;
                Axis dragAxis = MGridExtensions.CalcularArrastreDeAxis(worldDirection, SceneView.lastActiveSceneView.camera);

                if (dragAxis != Axis.Null && dragAxis != renderPlano)
                    SetRenderPlano(dragAxis);
            }
        }
        #endregion
        #region Interno
        internal bool GetUseAxisConstraints() { return toggleAxisConstantes ? !usarAxisConstantes : usarAxisConstantes; }
        internal float GetSnapValue() { return valorAjuste; }
        internal bool GetSnapEnabled() { return (toggleTempAjustado ? !ajusteEnabled : ajusteEnabled); }
        #endregion
        #endregion
    }
}
