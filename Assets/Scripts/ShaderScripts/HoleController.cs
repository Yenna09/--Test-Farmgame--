using UnityEngine;

public class HoleController : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody player;

    public float holeSize = 0.1f;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            float x = 0f;

            if (Vector3.Distance(hitCollider.transform.position, mainCamera.transform.position) < Vector3.Distance(player.centerOfMass + player.transform.position, mainCamera.transform.position))
            {
                x = holeSize;
            }

            try
            {
                Material[] materials = hitCollider.transform.GetComponent<Renderer>().materials;

                for (int m = 0; m < materials.Length; ++m)
                {
                    materials[m].SetFloat("_Step", x);
                }
            }
            catch { }
        }
    }
}
