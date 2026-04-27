using UnityEngine;

public class DitheredController : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody player;

    public float maxDistance = 5f;
    public float ditherStrength = 1f;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, maxDistance);

        foreach (var hitCollider in hitColliders)
        {
            Renderer rend = hitCollider.GetComponent<Renderer>();
            if (rend == null) continue;

            float value = 0f;

            float distToCamera = Vector3.Distance(hitCollider.transform.position, mainCamera.transform.position);
            float playerDistToCamera = Vector3.Distance(player.worldCenterOfMass, mainCamera.transform.position);

            
            if (distToCamera < playerDistToCamera)
            {
                value = ditherStrength;
            }

            Material[] materials = rend.materials;

            for (int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetFloat("_DitherAmount", value);
            }
        }
    }
}