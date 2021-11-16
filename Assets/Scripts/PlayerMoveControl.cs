using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Cursor = UnityEngine.Cursor;
using Vector3 = UnityEngine.Vector3;

public class CalculatedDataTransporter
{
    public float X
    {
        get => QuadrantTriangleLegA;
        set => QuadrantTriangleLegA = value;
    }

    public float Z
    {
        get => CvadrantTriangleLegB;
        set => CvadrantTriangleLegB = value;
    }

    public float QuadrantTriangleLegA;
    public float CvadrantTriangleLegB;
}

public class PlayerMoveControl : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed, rotateSpeed, jumpSpeed;
    private bool isOnTheGround = false;
    private Rigidbody rb;
    public Camera playerCam;
    private bool canMove;
    private CalculatedDataTransporter transporter;
    public Animator animator;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transporter = new CalculatedDataTransporter();
    }


    private void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 5, 5), "");
    }

    public void FixedUpdate()
    {
        if (canMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                float[] coord = GetCoordinatesByAngelToMoveBackAndForward(player.transform.localEulerAngles, moveSpeed);
                var position = transform.position;
                player.transform.position = new Vector3(position.x + coord[0], position.y, position.z + coord[2]);
            }

            if (Input.GetKey(KeyCode.S))
            {
                float[] coord = GetCoordinatesByAngelToMoveBackAndForward(player.transform.localEulerAngles, moveSpeed);
                var position = transform.position;
                player.transform.position = new Vector3(position.x - coord[0], position.y, position.z - coord[2]);
            }

            if (Input.GetKey(KeyCode.A))
            {
                float[] coord = GetCoordinatesByAngelToMoveRightAndLeft(player.transform.localEulerAngles, moveSpeed);


                var position = transform.position;
                player.transform.position = new Vector3(position.x - coord[0], position.y, position.z - coord[2]);
            }

            if (Input.GetKey(KeyCode.D))
            {
                float[] coord = GetCoordinatesByAngelToMoveRightAndLeft(player.transform.localEulerAngles, moveSpeed);


                var position = transform.position;
                player.transform.position = new Vector3(position.x + coord[0], position.y, position.z + coord[2]);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (isOnTheGround)
                {
                    rb.AddForce(Vector2.up * jumpSpeed, ForceMode.Impulse);
                    
                    isOnTheGround = false;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("ms down");
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            canMove = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            canMove = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (canMove)
        {
            if (Input.GetAxis("Mouse X") != 0)
            {
            
                 var localEulerAngles = rotateSpeed * new Vector3(0, Input.GetAxisRaw("Mouse X"), 0);
                player.transform.localEulerAngles += localEulerAngles;
            }
            
            if (Input.GetAxis("Mouse Y") != 0)
            {
                var localEulerAngles = rotateSpeed * new Vector3(-Input.GetAxisRaw("Mouse Y"),
                    0, 0);
                playerCam.transform.eulerAngles += localEulerAngles;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("landed");
        isOnTheGround = true;
    }

    private CalculatedDataTransporter CalculateXZCoordinates(Vector3 angels, float step, float reduceBy)
    {
        transporter.QuadrantTriangleLegA = (float) (step * Math.Sin(Math.PI * (angels.y - reduceBy) / 180.0));
        transporter.CvadrantTriangleLegB = (float) Math.Sqrt(Math.Pow(step, 2) - Math.Pow(transporter.QuadrantTriangleLegA, 2));

        return transporter;
    }

    private float[] GetCoordinatesByAngelToMoveBackAndForward(Vector3 angels, float step)
    {
        float[] coord = {0, 0, 0};

        float direction = angels.y;

        if (0 < direction && direction < 90)
        {
            transporter = CalculateXZCoordinates(angels, step, 0);

            coord[0] = transporter.QuadrantTriangleLegA;
            coord[2] = transporter.CvadrantTriangleLegB;
        }
        else if (90 < direction && direction < 180)
        {
            transporter = CalculateXZCoordinates(angels, step, 90);

            coord[0] = transporter.CvadrantTriangleLegB;
            coord[2] = -transporter.QuadrantTriangleLegA;
        }
        else if (180 < direction && direction < 270)
        {
            transporter = CalculateXZCoordinates(angels, step, 180);

            coord[0] = -transporter.QuadrantTriangleLegA;
            coord[2] = -transporter.CvadrantTriangleLegB;
        }
        else if (270 < direction && direction < 360)
        {
            transporter = CalculateXZCoordinates(angels, step, 270);

            coord[0] = -transporter.CvadrantTriangleLegB;
            coord[2] = transporter.QuadrantTriangleLegA;
        }
        else
        {
            switch (direction)
            {
                case 0:
                    coord[0] = 0;
                    coord[2] = step;
                    break;
                case 90:
                    coord[0] = step;
                    coord[2] = 0;
                    break;
                case 180:
                    coord[0] = 0;
                    coord[2] = step * (-1);
                    break;
                case 270:
                    coord[0] = step * (-1);
                    coord[2] = 0;
                    break;
                case 360:
                    coord[0] = 0;
                    coord[2] = step;
                    break;
            }
        }

        return coord;
    }

    private float[] GetCoordinatesByAngelToMoveRightAndLeft(Vector3 angels, float step)
    {
        float[] coord = {0, 0, 0};

        float direction = angels.y;
        float b = 0;
        float a = 0;
        float c = step;
        float aPowered = 0;
        float cPowered = 0;

        if (0 < direction && direction < 90)
        {
            float directionTemp = direction;
            float angle = (float) (Math.PI * directionTemp / 180.0);
            a = (float) (c * Math.Sin(angle));
            aPowered = a * a;
            cPowered = c * c;
            b = (float) Math.Sqrt((cPowered - aPowered));

            coord[0] = b;
            coord[2] = -a;
        }
        else if (90 < direction && direction < 180)
        {
            float directionTemp = direction - 90;
            float angle = (float) (Math.PI * directionTemp / 180.0);
            a = (float) (c * Math.Sin(angle));
            aPowered = a * a;
            cPowered = c * c;
            b = (float) Math.Sqrt((cPowered - aPowered));

            coord[0] = -a;
            coord[2] = -b;
        }
        else if (180 < direction && direction < 270)
        {
            float directionTemp = direction - 180;
            float angle = (float) (Math.PI * directionTemp / 180.0);
            a = (float) (c * Math.Sin(angle));
            aPowered = a * a;
            cPowered = c * c;
            b = (float) Math.Sqrt((cPowered - aPowered));

            coord[0] = -b;
            coord[2] = a;
        }
        else if (270 < direction && direction < 360)
        {
            float directionTemp = direction - 270;
            float angle = (float) (Math.PI * directionTemp / 180.0);
            a = (float) (c * Math.Sin(angle));
            aPowered = a * a;
            cPowered = c * c;
            b = (float) Math.Sqrt((cPowered - aPowered));

            coord[0] = a;
            coord[2] = b;
        }
        else
        {
            switch (direction)
            {
                case 0:
                    coord[0] = step * (-1);
                    coord[2] = 0;
                    break;
                case 90:
                    coord[0] = 0;
                    coord[2] = step;
                    break;
                case 180:
                    coord[0] = step;
                    coord[2] = 0;
                    break;
                case 270:
                    coord[0] = 0;
                    coord[2] = step * (-1);
                    break;
                case 360:
                    coord[0] = step * (-1);
                    coord[2] = 0;
                    break;
            }
        }

        return coord;
    }
}