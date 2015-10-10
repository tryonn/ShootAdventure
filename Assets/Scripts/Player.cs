using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Camera viewCamera;

    private PlayerController playerController;
    private GunController gunController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        gunController = GetComponent<GunController>();
    }

    protected override void Start()
    {
        base.Start();
    }
	// Update is called once per frame
	void Update ()
    {
        // movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        playerController.Move(moveVelocity);

        // look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);

            // player olha para o mouse/camera
            playerController.LookAt(point);
        }

        // weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
