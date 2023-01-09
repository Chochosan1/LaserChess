using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Each flying projectile is being controlled by this class.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float movementSpeed = 2f;

    private Piece owner;
    private Vector3 directionToFlyAt;

    void Update()
    {
        transform.position += directionToFlyAt * movementSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    /// <summary>Sets the owner of the projectile and its target.</summary>
    public void SetupProjectile(Piece ownerOfProjectile, Piece target)
    {
        owner = ownerOfProjectile;

        directionToFlyAt = target.transform.position - this.transform.position;
    }

    private void HandleCollision(GameObject collidedWithGameobject)
    {
        if (LaserChess.Utilities.LayerUtilities.IsObjectInLayer(collidedWithGameobject, owner.DamagePiecesOnThisLayer))
        {
            collidedWithGameobject.GetComponent<Piece>().TakeDamage(owner.AttackPower);
            Destroy(this.gameObject);
        }
    }
}
