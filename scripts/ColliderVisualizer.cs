using UnityEngine;

public class ColliderVisualizer : MonoBehaviour
{
    public Color gizmoColor = Color.green;
    public bool drawWireframe = true;

    private void OnDrawGizmos()
    {
        Collider targetCollider = GetComponent<Collider>();
        if (targetCollider == null) return;

        Gizmos.color = gizmoColor;
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            transform.lossyScale
        );

        if (targetCollider is BoxCollider boxCollider)
        {
            if (drawWireframe)
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            else
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
        else if (targetCollider is SphereCollider sphereCollider)
        {
            if (drawWireframe)
                Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
            else
                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
        }
        else if (targetCollider is CapsuleCollider capsuleCollider)
        {
            DrawCapsule(
                capsuleCollider.center,
                capsuleCollider.radius,
                capsuleCollider.height,
                capsuleCollider.direction
            );
        }

        else if (targetCollider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
        {
            if (drawWireframe)
                Gizmos.DrawWireMesh(meshCollider.sharedMesh);
            else
                Gizmos.DrawMesh(meshCollider.sharedMesh);
        }

        Gizmos.matrix = oldMatrix;
    }

    private void DrawCapsule(Vector3 center, float radius, float height, int direction)
    {
        Vector3 axis = Vector3.up;
        Vector3 sideA = Vector3.right;
        Vector3 sideB = Vector3.forward;

        switch (direction)
        {
            case 0:
                axis = Vector3.right;
                sideA = Vector3.up;
                sideB = Vector3.forward;
                break;
            case 2:
                axis = Vector3.forward;
                sideA = Vector3.right;
                sideB = Vector3.up;
                break;
        }

        float cylinderHeight = Mathf.Max(0f, height - radius * 2f);
        Vector3 topCenter = center + axis * (cylinderHeight * 0.5f);
        Vector3 bottomCenter = center - axis * (cylinderHeight * 0.5f);

        if (drawWireframe)
        {
            Gizmos.DrawWireSphere(topCenter, radius);
            Gizmos.DrawWireSphere(bottomCenter, radius);

            Gizmos.DrawLine(topCenter + sideA * radius, bottomCenter + sideA * radius);
            Gizmos.DrawLine(topCenter - sideA * radius, bottomCenter - sideA * radius);
            Gizmos.DrawLine(topCenter + sideB * radius, bottomCenter + sideB * radius);
            Gizmos.DrawLine(topCenter - sideB * radius, bottomCenter - sideB * radius);
        }
        else
        {
            Gizmos.DrawSphere(topCenter, radius);
            Gizmos.DrawSphere(bottomCenter, radius);

            int segments = 8;
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 pos = Vector3.Lerp(bottomCenter, topCenter, t);
                Gizmos.DrawSphere(pos, radius);
            }
        }
    }
}
