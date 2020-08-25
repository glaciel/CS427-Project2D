using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class RadarGraph : MonoBehaviour
{

    public Transform axisBar;   //Axis of Radar Graph
    public int axisNum;         //Number of Axis in graph
    public Material mater;      //Radar Graph material

    private float angleNum;     //Angle at which each axis is rotated
    private Dictionary<string, float> data = new Dictionary<string, float>(); //Stores data to be displayed
    private List<Vector3> points = new List<Vector3>();     //Stores vertex positions to generate mesh
    // Use this for initialization
    void Start()
    {
        float rotoAngle = 0;    //Angle of rotation for current Axis being added;
        angleNum = 360 / axisNum;

        //Function for acquiring data. Just pulls Random values, atm.
        getData();

        //Adds origin for center of Graph
        points.Add(transform.position);

        //Adds in and sets values of Axis
        for (int i = 0; i < axisNum; i++)
        {
            //Instantiates Axis
            Transform axis = Instantiate(axisBar, transform.position, Quaternion.identity, transform) as Transform;

            //Grabs current data Item
            var item = data.ElementAt(i);

            //Accesses Axis script and sets Marker's position locally in accordance to its value
            ////Its position is determined by where the data falls between the lowest and highest possible data points (as a percentage of 100).
            GraphAxis gAxis = axis.gameObject.GetComponent<GraphAxis>();
            Vector3 pos = gAxis.marker.position;
            pos.y = gAxis.transform.position.y + (gAxis.top.position.y - gAxis.transform.position.y) * item.Value;
            gAxis.marker.position = pos;

            //Temp vector3 for multiple purposes
            Vector3 temp = axis.eulerAngles;

            //Assign Axis angle
            temp.z = rotoAngle;
            axis.eulerAngles = temp;

            /* Straitens out Text Labels (3D GUI in this instance) if Axis have them (Optional)
            temp = gAxis.text.transform.eulerAngles;
            temp.z = 0;
            gAxis.text.transform.eulerAngles = temp;

            //Adjusts Text position as to not obstruct data (Optional)
            TextMesh text = gAxis.text;
            temp = text.transform.position;
            if (rotoAngle < 180  rotoAngle != 0)
                temp.x -= ((text.renderer.bounds.size.x / 2) + .01f);
            else if (rotoAngle > 180)
            temp.x += ((text.renderer.bounds.size.x / 2) + .01f);

        text.transform.position = temp;
        text.text = item.Key;*/

        //Adds Marker's position to the list of data points to be used as a vertex
        points.Add(gAxis.marker.position);

        //Adjusts angle of rotation for next access and parents current Axis
        rotoAngle += angleNum;
    }

    //Displays graph
    showGraph();
    }
    // Update is called once per frame
    void Update()
    {

    }

    //Grabs Data
    void getData()
    {
        data.Add("listeningPoint", MainPlayer.instance.playerData.listeningPoint.value);
        data.Add("vocabularyPoint", MainPlayer.instance.playerData.vocabularyPoint.value);
        data.Add("grammarPoint", MainPlayer.instance.playerData.grammarPoint.value);
    }

    //
    void showGraph()
    {
        GameObject chart = new GameObject("Block"); //GameObject mesh is assigned to
        Mesh newMesh = new Mesh();                  //Mesh to be used as Graph
        chart.AddComponent<MeshFilter>();           //Filter and Renderer Necessary to display Mesh
        chart.AddComponent<MeshRenderer>();

        //An array of the vertex points (as Vector3s) to be used for mesh
        Vector3[] verts = new Vector3[points.Count];

        //converts list of points to array (Mesh only accepts arrays
        for (int i = 0; i < points.Count; i++)
        {
            verts[i] = points[i];
        }

        //Assign verts to mesh
        newMesh.vertices = verts;

        //Assign UVs to Mesh
        Vector2[] uvs = new Vector2[newMesh.vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(newMesh.vertices[i].x, newMesh.vertices[i].z);
        }
        newMesh.uv = uvs;

        //New Array for Mesh Tris
        int[] tris = new int[(axisNum) * 3];

        int p1 = 1;     //first point of trangle other than origin (0) going counter clockwise
        int p2 = 2;     //second point of trangle other than origin (0) going counter clockwise
        int pLast = p1;     //Value for making the last triangle (and there for closing the mesh

        //Assigns Tris in a clockwise fashion (counterclockwise diplays the mesh backwards)
        for (int i = 0; i < (axisNum - 1) * 3; i += 3)
        {
            tris[i] = 0;
            tris[i + 1] = p2;
            tris[i + 2] = p1;

            ++p1;
            ++p2;
        }

        //Assigns end point of triangles, "closing" the mesh
        tris[((axisNum) * 3) - 3] = 0;
        tris[((axisNum) * 3) - 1] = p1;
        tris[((axisNum) * 3) - 2] = pLast;

        //Assign Tris (I think we get it by now)
        newMesh.triangles = tris;

        //Assigning the mesh to gameObject and other fine details
        newMesh.RecalculateNormals();
        chart.GetComponent<MeshFilter>().mesh = newMesh;
        Renderer renderer = chart.GetComponent<Renderer>();
        renderer.material = mater;
        chart.AddComponent<MeshCollider>();
        chart.AddComponent<Image>();
        Image image = chart.GetComponent<Image>();
        image.color = new Color(0.6f, 0.8509f, 0.9411f);
        //Parent new graph to this Script's owner
        chart.transform.SetParent(gameObject.transform);
    }

}