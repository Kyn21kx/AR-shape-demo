using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemLimits {
    //Auxiliar enum type to determine the sides of a rectification
	public enum SideType {
        /// <summary>
        /// AB where A: upperLeft; B: upperRight 
        /// </summary>
        UP,
        /// <summary>
        /// AB where A: lowerLeft; B: lowerRight
        /// </summary>
        DOWN,
        /// <summary>
        /// AB where A: lowerLeft; B: upperLeft
        /// </summary>
        LEFT, 
        /// <summary>
        /// AB where A: lowerRight; B: upperRight
        /// </summary>
        RIGHT
	}

    //Use 4 points to delimit the item we will create

    public Vector3[] Vertices { get; private set; }

    public static readonly int[] TRIANGLES_ARRAY = new int[] {
        0, 1, 2, //Add them like this for orientation purposes
        1, 3, 2
    };

    public static readonly Vector2[] UV_ARRAY = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    private float planeHeight;
    private const int MAX_VERTICES = 4;

    public ItemLimits(float planeHeight, Vector3[] vertices) {
        this.planeHeight = planeHeight;
        //Adding a validator to avoid any issues while creating the struct
        if (vertices.Length != MAX_VERTICES) {
            throw new System.Exception("[ERROR]: The item limits' vertex array must be 4 elements long");
		}
        this.Vertices = new Vector3[MAX_VERTICES];
        //Adjust the height for each of the points in the plane
        for (int i = 0; i < MAX_VERTICES; i++) { 
            this.Vertices[i] = this.AdjustToPlaneHeight(vertices[i]);
        }
	}

    private Vector3 AdjustToPlaneHeight(Vector3 v) {
        return new Vector3(v.x, planeHeight, v.z);
	}

}
