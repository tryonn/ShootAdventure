using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour {

    [SerializeField] private float speed = 5f;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float damage;


    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }	
	// Update is called once per frame
	void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);	
	}

    private void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    private void OnHitObject(RaycastHit hit)
    {
        //print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        Debug.Log(damage);
        if (damageableObject != null)
        {
            Debug.Log(damage);
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);
    }
}
