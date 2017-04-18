using UnityEngine;
using System.Collections;

public class FaultTerrainGenerator : MonoBehaviour {

    // Width of the terrain.
    protected int width = 128;
    // Height of the terrain.
    protected int height = 128;
    protected Color32[] colors;
    protected Texture2D texture;
    protected float averageHeight;
    protected int planetValue;
    protected float[][] heightMap;

    // Settings for the fault terraing heightmap generation.
    // Number of repetitions to apply fault lines in.
    protected float numberOfFaultIterations = 500;
    // Height changes at the first iteration. 
    protected float maxChange = 1.25f;
    // Height changes at the last iteration. 
    protected float minChange = 0.25f;

    /// <summary>
    /// Method for unity object initialization.
    /// </summary>
    public void Start()
    {

        texture = new Texture2D(width, height);
        GetComponent<Renderer>().material.mainTexture = texture;

        colors = new Color32[width * height];

        heightMap = FaultTerrain();
        averageHeight = CalculateAverageHeight();

        Draw(heightMap);

        float end_milliseconds = Time.time * 1000;

        System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Education\\4course\\PCG\\speed.txt");
        file.WriteLine("Speed of Fault algorithm: " + (Time.realtimeSinceStartup * 1000));

        file.Close();

        texture.SetPixels32(colors);
        texture.Apply();
    }

    /// <summary>
    /// Update object on mouse click.
    /// </summary>
    public void Update()
    {

        // Click mouse button to generate new.
        if (Input.GetMouseButton(0))
        {
            heightMap = FaultTerrain();
            Draw(heightMap);
            texture.SetPixels32(colors);
            texture.Apply();
        }

    }

    /// <summary>
    /// Terrain HeightMap generator on the base of Fault Algorithm.
    /// </summary>
    /// <returns>Two dimensional array of all points of the terrain with height assaigned to each of them.</returns>
    public float[][] FaultTerrain()
    {
        float[][] heightMap;
        // Random points in the terrain.
        Vector2 point1;
        Vector2 point2;
        // The fault line vector. 
        Vector2 faultLine;
        // Vector from the fault line to the point.
        Vector2 terraintPoint;
        float heightChange;
        // Extra row and column added to draw the outer quads.
        int fWidth = width + 1;
        int fHeight = height + 1;

        heightMap = InitializeEmptyArray<float>(fWidth, fHeight);

        for (int i = 0; i < numberOfFaultIterations; i++)
        {
            // Amount of height that will be changed for the iteration.
            // Decreases from iteration to iteration.
            heightChange = LinearInterpolation(maxChange, minChange, i / numberOfFaultIterations);

            // Choose two different random points of the terrain.
            while (true)
            {
                point1 = new Vector2(Random.Range(0f, 1f) * fWidth, Random.Range(0f, 1f) * fHeight);
                point2 = new Vector2(Random.Range(0f, 1f) * fWidth, Random.Range(0f, 1f) * fHeight);

                if (point1 != point2)
                {
                    break;
                }
            }

            // Create the "fault line" from point1 to point2.
            faultLine = point2 - point1;

            // Go through the x vertices from left to right.
            for (int j = 0; j < fWidth; j++)
            {
                // Go through the y vertice from top to bottom.    
                for (int k = 0; k < fHeight; k++)
                {
                    // Vector from the initial point of the fault line to the point of the terrain.
                    terraintPoint = new Vector2(j, k) - point1;

                    // If the point is on the correct side from the fault line, then icrease it's height.
                    if (PerpDotProduct(terraintPoint, faultLine) == true)
                    {
                        heightMap[j][k] += heightChange;
                    }
                }
            }
        }

        return heightMap;
    }

    /// <summary>
    /// Initialize an array with default values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="width">Number of columns in the two dimensional array.</param>
    /// <param name="height">Number of rows in the two dimensional array.</param>
    /// <returns>Array with default values.</returns>
    public static T[][] InitializeEmptyArray<T>(int width, int height)
    {
        T[][] array = new T[width][];

        for (int i = 0; i < width; i++)
        {
            array[i] = new T[height];
        }

        return array;
    }

    /// <summary>
    /// Linear interpolation function.
    /// </summary>
    /// <param name="value1">First point.</param>
    /// <param name="value2">Second point.</param>
    /// <param name="amount">Amount in the closed interval from 0 to 1.</param>
    /// <returns></returns>
    public static float LinearInterpolation(float value1, float value2, float amount)
    {
        return value1 + (value2 - value1) * amount;
    }

    /// <summary>
    /// Perpendicular Dot Product of two vectors.
    /// </summary>
    /// <param name="TPD">Tested Point's Direction vector.</param>
    /// <param name="FLV">The Fault Line's Vector.</param>
    /// <returns>True if TPD placed on the correct side according to FLV, otherwise returns false.</returns>
    public static bool PerpDotProduct(Vector2 TPD, Vector2 FLV)
    {
        float product = (TPD.x * FLV.y - TPD.y * FLV.x);
        if (product < 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Calculates average height of the heightmap.
    /// </summary>
    /// <returns>Value of the average height of the map</returns>
    public float CalculateAverageHeight()
    {
        float totalHeight = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                totalHeight += heightMap[i][j];
            }
        }

        return (totalHeight / (height * width));
    }

    /// <summary>
    /// Assign a color to cocrete point on the base of it's height.
    /// Color sheme is used on the base of the planet type.
    /// </summary>
    /// <param name="pointHeight">Height of the terrain in the concrete point.</param>
    /// <returns>A color based on the height's value.</returns>
    public Color GetColor(float pointHeight)
    {

        if (planetValue == 1)
        {
            // WaterGrass coloring scheme.

            if (pointHeight < averageHeight)
            {
                return Color.blue;
            }
            else
            {
                return Color.green;
            }
        }
        else if (planetValue == 2)
        {
            // LavaAsh coloring scheme.

            if (pointHeight < averageHeight)
            {
                return new Color(0.647f, 0.1647f, 0.1647f);
            }
            else
            {
                return new Color(0.2F, 0.3F, 0.4F);
            }

        }
        else
        {
            // IceSnow coloring scheme.

            if (pointHeight < averageHeight)
            {
                return Color.gray;
            }
            else
            {
                return Color.white;
            }

        }
    }

    /// <summary>
    /// Forms a colors map of the terrain.
    /// </summary>
    /// <param name="heightMap">Array of heights in each point of the terrain.</param>
    public void Draw(float[][] heightMap)
    {
        int colorsNumber = 0;
        planetValue = Random.Range(1, 4);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                colors[colorsNumber] = GetColor(heightMap[i][j]);
                colorsNumber++;
            }
        }

    }
}
