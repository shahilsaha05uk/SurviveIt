using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class Movements : MonoBehaviour
{
    private float horizontal, vertical;
    private float m_hori, m_verti;
    public float min, max;

    private float speed;
    [SerializeField]private float upDistance;

    private Vector3 look;
    public Vector3 inputMove;
    public LayerMask layermask;
    private Rigidbody rb;
    public Ray ray;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = 1f;
    }

    private void Update()
    {
        if (!GameManager.canvasOpened)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            m_hori += Input.GetAxis("Mouse Y") * speed;
            m_verti += Input.GetAxis("Mouse X") * speed;

            m_hori = Mathf.Clamp(m_hori, min, max);

            look = new Vector3(-m_hori, m_verti, 0f);

            transform.rotation = Quaternion.Euler(look);

            inputMove = new Vector3(horizontal * speed, 0, vertical * speed);

        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 1.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 1f;
        }

    }

    void FixedUpdate()
    {
        Vector3 direction = transform.TransformDirection(inputMove);
        transform.position += direction * speed;
        CheckBounds();
    }

    void CheckBounds()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        //Debug.DrawLine(ray.origin, ray.direction * 100, Color.blue);

        if (Physics.Raycast(transform.position, Vector3.up * -1f, out hitInfo, Mathf.Infinity, layermask))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y + upDistance, transform.position.z);
        }
    }
}
