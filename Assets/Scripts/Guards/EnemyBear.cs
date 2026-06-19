using UnityEngine;

public class EnemyBear : EnemigoPatrol
{
    [SerializeField] private float multiplicadorFuria = 1.5f;
    [SerializeField] private float velocidadMovimiento = 2f;
    // Local copy because base class field is not accessible
    private float tiempoSiguienteAtaque = 0f;

    public override void AttackState()
    {
        base.AttackState();

        if (target != null)
    {
            // EnemigoPatrol.MoverHaciaDestino is inaccessible; use local implementation
            MoverHaciaDestino(target.position);
        }

        // Lógica de daño
        if (Time.time >= tiempoSiguienteAtaque)
        {
            Atacar();
            tiempoSiguienteAtaque = Time.time + velocidadAtaque;
        }
    }

    public override void DeathState()
    {
        base.DeathState();
        
    }

    // Local movement implementation because EnemigoPatrol.MoverHaciaDestino is not accessible due to protection level
    private void MoverHaciaDestino(Vector3 destino)
    {
        // Move towards destination at configured speed
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);

        // Face the movement direction smoothly
        Vector3 direccion = destino - transform.position;
        if (direccion.sqrMagnitude > 0.0001f)
        {
            Quaternion objetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, 10f * Time.deltaTime);
        }
    }
}