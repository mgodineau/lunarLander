using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterMesh
{
    private List<Vertex> _vertices;
    private List<Triangle> _triangles;

    public List<Vertex> Vertices
    {
        get { return _vertices; }
    }
    public List<Triangle> Triangles
    {
        get { return _triangles; }
    }

    private Mesh _linkedMesh;
    public Mesh LinkedMesh
    {
        get { return _linkedMesh; }
        set
        {
            _linkedMesh = value;

            _vertices = new List<Vertex>();
            if (_linkedMesh.normals.Length < _linkedMesh.vertexCount)
            {
                for (int i = 0; i < _linkedMesh.vertexCount; i++)
                {
                    _vertices.Add(new Vertex(_linkedMesh.vertices[i]));
                }
            }
            else if (_linkedMesh.uv.Length < _linkedMesh.vertexCount)
            {
                for (int i = 0; i < _linkedMesh.vertexCount; i++)
                {
                    _vertices.Add(new Vertex(_linkedMesh.vertices[i], _linkedMesh.normals[i]));
                }
            }
            else
            {
                for (int i = 0; i < _linkedMesh.vertexCount; i++)
                {
                    _vertices.Add(new Vertex(_linkedMesh.vertices[i], _linkedMesh.normals[i], _linkedMesh.uv[i]));
                }
            }

            _triangles = new List<Triangle>();
            int[] meshTriangles = _linkedMesh.triangles;
            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                _triangles.Add(new Triangle(_vertices[meshTriangles[i]], _vertices[meshTriangles[i + 1]], _vertices[meshTriangles[i + 2]]));
            }
        }
    }


    public Triangle[] VerticeToTriangle(Vertex vertice)
    {
        if (!_vertices.Contains(vertice))
        {
            return new Triangle[0];
        }

        List<Triangle> output = new List<Triangle>();
        foreach (Triangle triangle in _triangles)
        {
            if (new List<Vertex>(triangle.Vertices).Contains(vertice))
            {
                output.Add(triangle);
            }
        }
        return output.ToArray();
    }

    public void RemoveVertex(Vertex vertice)
    {
        if (!_vertices.Contains(vertice))
        {
            return;
        }

        _triangles.RemoveAll(triangle => new List<Vertex>(triangle.Vertices).Contains(vertice));
        _vertices.Remove(vertice);
    }

    public void RemoveTriangle(Triangle triangle)
    {
        _triangles.Remove(triangle);
    }


    public void AddVertex(Vertex vertice)
    {
        if (!_vertices.Contains(vertice))
        {
            _vertices.Add(vertice);
        }
    }

    public void AddTriangles(IEnumerable<Triangle> triangles)
    {
        foreach (Triangle triangle in triangles)
        {
            AddTriangle(triangle);
        }
    }

    public void AddTriangle(Triangle triangle)
    {
        if (!_triangles.Contains(triangle))
        {
            _triangles.Add(triangle);
            foreach (Vertex vertice in triangle.Vertices)
            {
                AddVertex(vertice);
            }
        }
    }


    public Vector3[] GetMeshVertices()
    {
        return _vertices.ConvertAll<Vector3>(vertice => vertice.position).ToArray();
    }

    public int[] GetMeshTriangles()
    {
        int[] meshTriangles = new int[_triangles.Count * 3];
        for (int i = 0; i < _triangles.Count; i++)
        {
            Vertex[] triangleVertices = _triangles[i].Vertices;
            for (int j = 0; j < 3; j++)
            {
                meshTriangles[i * 3 + j] = _vertices.IndexOf(triangleVertices[j]);
            }
        }

        return meshTriangles;
    }

    public Vector3[] GetMeshNormals()
    {
        return _vertices.ConvertAll(vertex => vertex.normal).ToArray();
    }

    public Vector2[] GetMeshUVs()
    {
        return _vertices.ConvertAll(vertex => vertex.uv).ToArray();
    }


    public void UpdateMesh()
    {
        _linkedMesh.Clear();

        _linkedMesh.vertices = GetMeshVertices();
        _linkedMesh.normals = GetMeshNormals();
        _linkedMesh.uv = GetMeshUVs();

        _linkedMesh.triangles = GetMeshTriangles();
    }


    public BetterMesh(Mesh mesh)
    {
        LinkedMesh = mesh;
    }


}





public class Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;


    private static Vector2 GetLocalUVCoord(Edge edge1, Edge edge2, Vector3 position)
    {
        Vertex sharedVertex = GetVertexInCommon(edge1, edge2);
        Vector3 vect1 = edge1.vertex_1.position - edge1.vertex_0.position;
        Vector3 vect2 = edge2.vertex_1.position - edge2.vertex_0.position;
        if (edge1.vertex_1 == sharedVertex)
        {
            vect1 = -vect1;
        }
        if (edge2.vertex_1 == sharedVertex)
        {
            vect2 = -vect2;
        }
        position -= sharedVertex.position;



        //résolution du système : vect1 * coords.x + vect2 * coords.y = position
        Vector2 coords = Vector2.zero;
        if (vect1.x == 0)
        {
            if (vect2.x != 0)
            {
                coords.y = position.x / vect2.x;
                coords.x = (vect1.y == 0) ?
                (position.z - coords.y * vect2.z) / vect1.z :
                (position.y - coords.y * vect2.y) / vect1.y;
            }
            else
            {
                if (vect1.y == 0)
                {
                    if (vect2.y != 0)
                    {
                        coords.y = position.y / vect2.y;
                        coords.x = (position.z - coords.y * vect2.z) / vect1.z;
                    }
                }
                else
                {
                    float zOverY = vect1.z / vect1.y;
                    coords.y = (position.z - position.y * zOverY) / (vect2.z - vect2.y * zOverY);
                    coords.x = (position.y - coords.y * vect2.y) / vect1.y;
                }
            }

        }
        else
        {
            float yOverX = vect1.y / vect1.x;
            float zOverX = vect1.z / vect1.x;
            float tmp = vect2.y - vect2.x * yOverX;
            coords.y = (tmp == 0) ?
                (position.z - position.x * zOverX) / (vect2.z - vect2.x * zOverX) :
                (position.y - position.x * yOverX) / tmp;
            coords.x = (position.x - vect2.x * coords.y) / vect1.x;
        }

        return coords;
    }

    private static Vertex GetVertexInCommon(Edge edge1, Edge edge2)
    {
        if (edge1.ContainsVertex(edge2.vertex_0))
        {
            return edge2.vertex_0;
        }
        else if (edge1.ContainsVertex(edge2.vertex_1))
        {
            return edge2.vertex_1;
        }
        return null;
    }


    public Vertex(Vector3 position) : this(position, Vector3.up) { }

    public Vertex(Vector3 position, Vector3 normal) : this(position, normal, Vector2.zero) { }

    public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        this.position = position;
        this.normal = normal.normalized;
        this.uv = uv;
    }


    public Vertex(Vector3 position, Triangle triangle)
    {
        this.position = position;

        Vertex[] vertices = triangle.Vertices;
        Edge edge_0 = new Edge(vertices[0], vertices[1]);
        Edge edge_1 = new Edge(vertices[0], vertices[2]);
        Vector2 tmpUV = GetLocalUVCoord(edge_0, edge_1, position);
        tmpUV.x = Mathf.Clamp01(tmpUV.x);
        tmpUV.y = Mathf.Clamp01(tmpUV.y);
        Vector2 uv0 = vertices[0].uv;
        uv = uv0 + (vertices[1].uv - uv0) * tmpUV.x + (vertices[2].uv - uv0) * tmpUV.y;
        Vector3 normal0 = vertices[0].normal;
        normal = normal0 + (vertices[1].normal - normal0) * tmpUV.x + (vertices[2].normal - normal0) * tmpUV.y;
        normal.Normalize();
    }


}

public class Edge
{
    public Vertex vertex_0;
    public Vertex vertex_1;

    public bool ContainsVertex(Vertex vertex)
    {
        return vertex_0 == vertex || vertex_1 == vertex;
    }

    public bool ReplaceVertex(Vertex oldVertex, Vertex newVertex)
    {
        if (vertex_0 == oldVertex)
        {
            vertex_0 = newVertex;
            return true;
        }
        if (vertex_1 == oldVertex)
        {
            vertex_1 = newVertex;
            return true;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        Edge otherEdge = obj as Edge;
        if (otherEdge == null)
        {
            return false;
        }
        return vertex_0 == otherEdge.vertex_0 && vertex_1 == otherEdge.vertex_1;
    }

    public override int GetHashCode()
    {
        int hashCode = -1574376401;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(vertex_0);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(vertex_1);
        return hashCode;
    }

    public Edge(Vertex vertex_0, Vertex vertex_1)
    {


        if (EqualityComparer<Vertex>.Default.GetHashCode(vertex_0) > EqualityComparer<Vertex>.Default.GetHashCode(vertex_1))
        {
            this.vertex_0 = vertex_1;
            this.vertex_1 = vertex_0;
        }
        else
        {
            this.vertex_0 = vertex_0;
            this.vertex_1 = vertex_1;
        }
    }
}


public class Triangle
{
    private Vertex[] _vertices;
    public Vertex[] Vertices
    {
        get { return _vertices; }
    }


    public Edge[] GetEdges()
    {
        return new Edge[3]{
            new Edge(_vertices[0], _vertices[1]) ,
            new Edge(_vertices[1], _vertices[2]) ,
            new Edge(_vertices[2], _vertices[0]) };
    }

    public Vector3 GetNormal()
    {
        Vector3 normal = Vector3.Cross(_vertices[1].position - _vertices[0].position, _vertices[2].position - _vertices[0].position).normalized;

        //si la normal est nulle a cause de la précision, on multiplie par 10 les positions jusqu'a ce qu'on ait un truc
        int i = 0;
        while (i < 5 && normal == Vector3.zero)
        {
            Vector3[] pos = new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                pos[j] = _vertices[j].position * (10 * i);
            }
            normal = Vector3.Cross(pos[1] - pos[0], pos[2] - pos[0]).normalized;
            i++;
        }
        return normal;
    }

    public Triangle(Vertex vertex_0, Vertex vertex_1, Vertex vertex_2)
    {
        _vertices = new Vertex[3] { vertex_0, vertex_1, vertex_2 };
    }
}

