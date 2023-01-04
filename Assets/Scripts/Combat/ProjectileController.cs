using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;

    private Piece owner;
    private Vector3 directionToFlyAt;

    void Update()
    {
        transform.position += directionToFlyAt * movementSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(LaserChess.Utilities.LayerUtilities.IsObjectInLayer(other.gameObject, owner.damagePiecesOnThisLayer))
        {
            other.gameObject.GetComponent<Piece>().TakeDamage(owner.AttackPower);
            Destroy(this.gameObject);
        }
    }

    public void SetupProjectile(Piece ownerOfProjectile, Piece target)
    {
        owner = ownerOfProjectile;

        directionToFlyAt = target.transform.position - this.transform.position;
    }
}
