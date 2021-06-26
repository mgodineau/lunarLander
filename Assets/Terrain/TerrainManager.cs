using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    //singleton
    private static TerrainManager _instance;
    public static TerrainManager Instance
    {
        get { return _instance; }
    }


    //paramètres de la génération du terrain
    [SerializeField]
    private float _terrainWidth = 10;
    public float TerrainWidth
    {
        get { return _terrainWidth; }
    }



    [SerializeField] private Transform _lightReference;      //l'objet depuis lequel calculer l'orientation de la lumière (souvent le lander)
    public Transform LightReference {
        get{ return _lightReference; }
    }
    
    [Range(0, 1)]
    [SerializeField] private float lightFadeLimit = 0.1f;
    public Light mainLight;
    public Vector3 globalLightDir = Vector3.right;

    [SerializeField]
    private int sampleCount = 100;
    [SerializeField]
    private int bgSampleCount = 5;
    [SerializeField]
    private float bottomY = -1;
    [SerializeField]
    private float bottomZ = 0;

    [SerializeField]
    private float objSideThreshold = 1.0f;


    //matérials du terrain
    [SerializeField]
    private Material terrainSideMaterial;
    [SerializeField]
    private Material terrainMaterial;

    //générateur le la planète
    [SerializeField]
    private PlanetGen _planet;
    public PlanetGen Planet
    {
        get { return _planet; }
    }

    //prefab des objets
    [SerializeField] public GameObject lzDefaultPref;
    [SerializeField] public GameObject lzFuelPref;

    [SerializeField] public GameObject crystalPref;


    //instances des ZA
    private Dictionary<LocalizedObject, GameObject> objToPrefInstance = new Dictionary<LocalizedObject, GameObject>();

    //propriétés de la tranche visualisée
    private Vector3 _sliceNormal = Vector3.up;
    public Vector3 SliceNormal
    {
        get { return _sliceNormal; }
    }
    private Vector3 _sliceOrigine = Vector3.forward;
    public Vector3 SliceOrigin
    {
        get { return _sliceOrigine; }
    }

    //propriétés privées du terrain
    private EdgeCollider2D[] terrainColliders;
    private Mesh terrainSideMesh;
    private Mesh terrainMesh;

    private Vector2[] points;
    private Vector3[] vertices;
    private Vector3[] bgVertices;

    //rendu wireframe du terrain
    private LineData frontLine;
    private LineData[] backgroundXlines;
    private LineData[] backgroundZlines;

    private void Awake()
    {
        _instance = this; //singleton

        //récupération du générateur le planête si il est pas connu
        if (_planet == null)
        {
            _planet = GetComponent<PlanetGen>();
        }



        //creation des objets du terrain
        terrainSideMesh = new Mesh();
        terrainMesh = new Mesh();
        
        frontLine = new LineData();
        
        int terrainCount = 2;
        terrainColliders = new EdgeCollider2D[terrainCount];
        backgroundXlines = new LineData[terrainCount];
        backgroundZlines = new LineData[terrainCount];
        for( int i=0; i<terrainCount; i++ ) {
            backgroundXlines[i] = new LineData();
            backgroundZlines[i] = new LineData();
        }
        
        
        for (int i = 0; i < terrainCount; i++)
        {
            //création de l'objet sur le côté, et ajustement de sa position
            GameObject terrain = new GameObject("terrain_" + i);
            SetTerrainParent(terrain.transform, transform, i);

            //ajout d'un MeshRenderer et d'un collider à l'objet
            terrain.AddComponent<MeshFilter>().mesh = terrainSideMesh;
            terrain.AddComponent<MeshRenderer>().material = terrainSideMaterial;
            terrainColliders[i] = terrain.AddComponent<EdgeCollider2D>();


            //création de l'arrière plan du terrain
            GameObject terrainBackground = new GameObject("terrainBackground_" + i);
            SetTerrainParent(terrainBackground.transform, terrain.transform, 0);
            terrainBackground.AddComponent<MeshFilter>().mesh = terrainMesh;
            terrainBackground.AddComponent<MeshRenderer>().material = terrainMaterial;
        }

        //création de la géométrie du terrain et du collider
        UpdateTerrainStructure();
    }
    
    
    
    private void Start() {
        
        foreach( LineData line in backgroundXlines ) {
            WireframeRender.Instance.linesGeometry.Add(line);
        }
        foreach( LineData line in backgroundZlines ) {
            WireframeRender.Instance.linesGeometry.Add(line);
        }
        WireframeRender.Instance.linesGeometry.Add(frontLine);
    }
    

    private void Update()
    {
        //MAJ de l'orientation de la lumière
        Vector3 localLightDir = Quaternion.Inverse(Quaternion.LookRotation(_sliceNormal, ConvertXtoDir(_lightReference.position.x))) * globalLightDir;

        mainLight.transform.rotation = Quaternion.LookRotation(localLightDir, Vector3.up);
        mainLight.intensity = Mathf.Clamp01(Mathf.Asin(-localLightDir.y) * 2.0f / Mathf.PI / lightFadeLimit);
        // mainLight.enabled = localLightDir.y <= 0;

    }


    /// <summary>
    /// affecte le parent d'un transform, et réinitialise ses transformations locales en fonction de i
    /// </summary>
    /// <param name="child">Le transform enfant</param>
    /// <param name="parent">Le tranform parent</param>
    /// <param name="i">Le nombre de décalage d'amplitude _terrainWidth à effectuer sur X</param>
    private void SetTerrainParent(Transform child, Transform parent, int i)
    {
        child.SetParent(parent);
        child.localPosition = Vector3.zero - Vector3.right * _terrainWidth * i;
        child.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }


    /// <summary>
    /// Créé  une nouvelle structure de terrain, en mettant à jour la géométrie du mesh et des colliders
    /// </summary>
    /// <remarks>la variable mesh doit être initialisé</remarks>
    private void UpdateTerrainStructure()
    {
        points = new Vector2[sampleCount + 1];
        vertices = new Vector3[(sampleCount + 1) * 2];

        for (int i = 0; i <= sampleCount; i++)
        {

            //création des points du collider et du mesh
            float x = _terrainWidth * i / sampleCount;
            points[i] = new Vector2(x, 1);
            vertices[i * 2] = new Vector3(x, 1, 0);
            vertices[i * 2 + 1] = new Vector3(x, bottomY, bottomZ);

        }


        //création des triangles
        int[] triangles = new int[sampleCount * 3 * 2];
        for (int i = 0; i < sampleCount; i++)
        {
            triangles[i * 6] = i * 2 + 1;
            triangles[i * 6 + 1] = i * 2;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 1;
            triangles[i * 6 + 4] = i * 2 + 2;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        //affectation de la géométrie au Mesh
        terrainSideMesh.vertices = vertices;
        terrainSideMesh.triangles = triangles;


        //création de la géométrie de l'arrière plan

        //création des vertices
        bgVertices = new Vector3[(sampleCount + 1) * (bgSampleCount + 1)];
        for (int z = 0; z <= bgSampleCount; z++)
        {
            float realZ = _terrainWidth * z / sampleCount;
            for (int x = 0; x <= sampleCount; x++)
            {
                bgVertices[z * (sampleCount + 1) + x] = new Vector3(_terrainWidth * x / sampleCount, 0, realZ);
            }
        }
        //création des triangles
        int[] bgTriangles = new int[sampleCount * bgSampleCount * 3 * 2];
        for (int i = 0; i < bgSampleCount; i++)
        {
            for (int j = 0; j < sampleCount; j++)
            {
                bgTriangles[(i * sampleCount + j) * 6] = i * (sampleCount + 1) + j;
                bgTriangles[(i * sampleCount + j) * 6 + 1] = (i + 1) * (sampleCount + 1) + j;
                bgTriangles[(i * sampleCount + j) * 6 + 2] = i * (sampleCount + 1) + (j + 1);

                bgTriangles[(i * sampleCount + j) * 6 + 3] = (i + 1) * (sampleCount + 1) + (j + 1);
                bgTriangles[(i * sampleCount + j) * 6 + 4] = i * (sampleCount + 1) + (j + 1);
                bgTriangles[(i * sampleCount + j) * 6 + 5] = (i + 1) * (sampleCount + 1) + j;
            }
        }
        terrainMesh.vertices = bgVertices;
        terrainMesh.triangles = bgTriangles;


        //MAJ des données de hauteur du terrain
        UpdateTerrain();
    }


    /// <summary>
    /// Met à jours la géométrie du mesh et du collider des terrains
    /// </summary>
    /// <remarks>les tableaux points, vertices et bgVertices doivent être initialisés, ainsi que mesh</remarks>
    private void UpdateTerrain()
    {

        //MAJ des vertices du premier plan
        for (int i = 0; i <= sampleCount; i++)
        {
            //récupération de la hauteur
            float ratio = (float)i / sampleCount;
            float angle = 360.0f * ratio;
            Vector3 samplePosition = Quaternion.AngleAxis(angle, _sliceNormal) * _sliceOrigine;
            float sample = _planet.GetHeight(samplePosition);

            //création des points du collider et du mesh
            float x = _terrainWidth * ratio;
            points[i].y = sample;
            vertices[i * 2].y = sample;

        }



        terrainSideMesh.vertices = vertices;
        terrainSideMesh.RecalculateBounds();
        foreach (EdgeCollider2D col in terrainColliders)
        { //MAJ des colliders
            col.points = points;
        }


        //création de la hauteur de l'arrière plan
        for (int x = 0; x <= sampleCount; x++)
        {
            bgVertices[x].y = points[x].y;
        }
        for (int z = 1; z <= bgSampleCount; z++)
        {
            for (int x = 0; x <= sampleCount; x++)
            {
                float angleZ = 360.0f * x / sampleCount;
                float angleX = 360.0f * z / sampleCount;
                Vector3 samplePosition = Quaternion.AngleAxis(angleZ, _sliceNormal) * _sliceOrigine;
                samplePosition = Quaternion.AngleAxis(angleX, Vector3.Cross(samplePosition, _sliceNormal)) * samplePosition;
                bgVertices[z * (sampleCount + 1) + x].y = _planet.GetHeight(samplePosition);
            }
        }
        terrainMesh.vertices = bgVertices;
        terrainMesh.RecalculateBounds();
        terrainMesh.RecalculateNormals();
        
        
        
        //MAJ du rendu de la surface du terrain
        frontLine.points = new List<Vector2>(points).ConvertAll(v2 => new Vector3(v2.x - _terrainWidth, v2.y, 0));
        frontLine.points.AddRange(ShiftLine(frontLine.points, _terrainWidth));
        
        
        backgroundXlines[0].points.Clear();
        backgroundXlines[0].points.Add(bgVertices[0]);
        bool leftToRight = true;
        for (int z = 1; z <= bgSampleCount; z++)
        {
            if (leftToRight)
            {
                for (int x = 0; x <= sampleCount; x++)
                {
                    int verticeId = z * (sampleCount + 1) + x;
                    backgroundXlines[0].points.Add(bgVertices[verticeId]);
                }
            }
            else
            {
                for (int x = sampleCount; x >= 0; x--)
                {
                    int verticeId = z * (sampleCount + 1) + x;
                    backgroundXlines[0].points.Add(bgVertices[verticeId]);
                }
            }
            leftToRight = !leftToRight;
        }
        backgroundXlines[1].points = ShiftLine(backgroundXlines[0].points, -_terrainWidth);
        
        
        backgroundZlines[0].points.Clear();
        
        bool frontToBack = true;
        for (int x = 0; x <= sampleCount; x++)
        {
            if (frontToBack)
            {
                for (int z = 0; z <= bgSampleCount; z++)
                {
                    int verticeId = z * (sampleCount + 1) + x;
                    backgroundZlines[0].points.Add(bgVertices[verticeId]);
                }
            }
            else
            {
                for (int z = bgSampleCount; z >= 0; z--)
                {
                    int verticeId = z * (sampleCount + 1) + x;
                    backgroundZlines[0].points.Add(bgVertices[verticeId]);
                }
            }
            frontToBack = !frontToBack;
        }
        backgroundZlines[1].points = ShiftLine(backgroundZlines[0].points, -_terrainWidth);
        
        //TODO factoriser ce bordel





        //MAJ des zones d'atterrissages et des cristaux
        UpdateObjetsDisplay(_planet.landingZones, true);
        UpdateObjetsDisplay(_planet.crystals, false);

    }



    private void UpdateObjetsDisplay(IEnumerable<LocalizedObject> objects, bool flattenTerrain)
    {
        float objCosThreshold = objSideThreshold / _terrainWidth;

        foreach (LocalizedObject obj in objects)
        {
            float normalCos = Mathf.Abs(Vector3.Dot(obj.Position, _sliceNormal));

            if (normalCos < objCosThreshold)
            {
                Vector3 displayPosition = Vector3.ProjectOnPlane(obj.Position, _sliceNormal);
                float angle = Vector3.SignedAngle(_sliceOrigine, displayPosition, _sliceNormal);
                if (angle < 0)
                {
                    angle += 360.0f;
                }

                int positionId = (int)(sampleCount * angle / 360.0f);

                float localRatio = flattenTerrain ? 0.5f : (angle * sampleCount / 360.0f) % 1.0f;

                float y = Mathf.Lerp(points[positionId].y, points[positionId + 1].y, localRatio);

                if (flattenTerrain)
                {
                    points[positionId].y = y;
                    points[positionId + 1].y = y;
                    vertices[positionId * 2].y = y;
                    vertices[(positionId + 1) * 2].y = y;
                }




                float x = points[positionId].x;
                x += _terrainWidth / sampleCount * localRatio;

                if (x > TerrainWidth * 0.5f)
                {
                    x -= TerrainWidth;
                }

                float z = _terrainWidth / (sampleCount * 2);
                Vector3 position = new Vector3(x, y, z);

                if (!objToPrefInstance.ContainsKey(obj))
                {
                    objToPrefInstance.Add(obj, obj.CreateInstance(position, Quaternion.identity, transform));
                }
                else
                {
                    GameObject instance = objToPrefInstance[obj];
                    instance.transform.SetParent(transform, false);
                    instance.transform.localPosition = position;
                }



            }
            else if (objToPrefInstance.ContainsKey(obj))
            {
                Destroy(objToPrefInstance[obj]);
                objToPrefInstance.Remove(obj);
            }
        }
    }





    private List<Vector3> ShiftLine(List<Vector3> line, float Xoffset)
    {
        return line.ConvertAll(pos => new Vector3(pos.x + Xoffset, pos.y, pos.z));
    }


    /// <summary>
    /// Effectue une rotation du plan d'évolution du jeu, et met à jour les objets du terrain.
    /// </summary>
    /// <param name="axis2dPosition">la position x de l'axe de rotation, dans le repère 2D (ex : la position du joueur)</param>
    /// <param name="angle">L'angle de rotation, en degré</param>
    public void RotateAround(float axis2dPosition, float angle)
    {
        RotateAround(ConvertXtoDir(axis2dPosition), angle);
    }

    /// <summary>
    /// Effectue une rotation du plan d'évolution du jeu, et met à jour les objets du terrain.
    /// </summary>
    /// <param name="rotationAxis">L'axe de rotation, dans le repère globale (non nul)</param>
    /// <param name="angle">L'angle de rotation, en degré</param>
    public void RotateAround(Vector3 rotationAxis, float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
        _sliceNormal = (rotation * _sliceNormal).normalized;
        _sliceOrigine = (rotation * _sliceOrigine).normalized;

        UpdateTerrain();
    }


    public bool IsLZvisible(LandingZone lz)
    {
        return objToPrefInstance.ContainsKey(lz);
    }

    public Vector3 ConvertXtoDir(float x)
    {
        x = x % _terrainWidth;
        return (Quaternion.AngleAxis(360.0f * x / _terrainWidth, _sliceNormal) * _sliceOrigine).normalized;
    }


    public float GetHeightOf(Vector3 pos)
    {

        if (pos.x < 0)
        {
            pos.x += TerrainWidth;
        }

        float sampleSize = _terrainWidth / sampleCount;


        int idX = (int)(pos.x / sampleSize);
        int idZ = (int)(pos.z / sampleSize);
        
        
        float xRatio = (pos.x / sampleSize) - idX;
        float heightFront = HeightXLerp(idX, idZ, xRatio);
        float heightBack = HeightXLerp(idX, idZ+1, xRatio);
        
        float zRatio = (pos.z / sampleSize) - idZ;
        
        return Mathf.Lerp( heightFront, heightBack, zRatio );
        
        // float heightXmin = bgVertices[idX + idZ * (sampleCount + 1)].y;
        // float heightMax = bgVertices[(idX + 1) + idZ * (sampleCount + 1)].y;


        // return Mathf.Lerp(heightXmin, heightMax, xRatio);
    }


    private float HeightXLerp(int idX, int idZ, float ratio)
    {

        return Mathf.Lerp(
            bgVertices[idX + idZ * (sampleCount + 1)].y,
            bgVertices[(idX + 1) + idZ * (sampleCount + 1)].y,
            ratio);
    }


}
