using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ibounds{
    void CreateVertices();
    Bounds Bounds { get; }
    Vector3[] Vertices { get; }
    Transform Transform { get; }
}
