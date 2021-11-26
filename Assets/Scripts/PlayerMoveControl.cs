using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Cursor = UnityEngine.Cursor;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CalculatedDataTransporter
{
    public float QuadrantTriangleLegA { get; set; }

    public float QvadrantTriangleLegB { get; set; }
}

public class AxisLocker
{
    public bool PlusX { get; set; } = false;

    public bool PlusZ { get; set; } = false;

    public bool MinusX { get; set; } = false;

    public bool MinusZ { get; set; } = false;
}

public class PlayerMoveControl : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed, rotateSpeed, jumpSpeed;
    private bool isOnTheGround = true;
    public Rigidbody rb;
    public Camera playerCam;
    private bool canMove;
    public float gravity;
    private AxisLocker axisLocker;


    private void Start()
    {
        Physics.gravity = new Vector3(0, gravity, 0);
        rb = GetComponent<Rigidbody>();
        axisLocker = new AxisLocker();
    }


    private void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 5, 5), "");
    }


    public void FixedUpdate()
    {
        // if (canMove)
        // {
        //     if (Input.GetKey(KeyCode.W))
        //     {
        //         MovePlayerForward(transform.position,
        //             GetCoordinatesByAngelToMoveBackAndForward(player.transform.localEulerAngles, moveSpeed));
        //     }
        //
        //     if (Input.GetKey(KeyCode.S))
        //     {
        //         MovePlayerBack(transform.position,
        //             GetCoordinatesByAngelToMoveBackAndForward(player.transform.localEulerAngles, moveSpeed));
        //     }
        //
        //     if (Input.GetKey(KeyCode.A))
        //     {
        //         MovePlayerLeft(GetCoordinatesByAngelToMoveRightAndLeft(player.transform.localEulerAngles, moveSpeed),
        //             transform.position);
        //     }
        //
        //     if (Input.GetKey(KeyCode.D))
        //     {
        //         MovePlayerRight(GetCoordinatesByAngelToMoveRightAndLeft(player.transform.localEulerAngles, moveSpeed),
        //             transform.position);
        //     }
        //
        //     if (Input.GetKey(KeyCode.R))
        //     {
        //         ResetLocker();
        //         player.transform.position = new Vector3(0, 0, 0);
        //     }
        //
        //     if (Input.GetKey(KeyCode.Space))
        //     {
        //         if (isOnTheGround)
        //         {
        //             JumpPlayer();
        //         }
        //     }
        // }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * (Time.deltaTime * moveSpeed), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * (Time.deltaTime * moveSpeed), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * (Time.deltaTime * moveSpeed), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * (Time.deltaTime * moveSpeed), ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (isOnTheGround)
            {
                JumpPlayer();
            }
        }
    }

    private void ResetLocker()
    {
        axisLocker.PlusX = false;
        axisLocker.PlusZ = false;
        axisLocker.MinusX = false;
        axisLocker.MinusZ = false;
    }

    private void JumpPlayer()
    {
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode.Impulse);
    }

    private void MovePlayerForward(Vector3 position, float[] coord)
    {
        player.transform.position = new Vector3(GetCorrectXMoveValue(position.x + coord[0], position), position.y,
            GetCorrectZMoveValue(position.z + coord[2], position));
    }

    private void MovePlayerBack(Vector3 position, float[] coord)
    {
        player.transform.position = new Vector3(GetCorrectXMoveValue(position.x - coord[0], position), position.y,
            GetCorrectZMoveValue(position.z - coord[2], position));
    }

    private void MovePlayerLeft(float[] coord, Vector3 position)
    {
        player.transform.position = new Vector3(GetCorrectXMoveValue(position.x - coord[0], position), position.y,
            GetCorrectZMoveValue(position.z - coord[2], position));
    }

    private void MovePlayerRight(float[] coord, Vector3 position)
    {
        player.transform.position = new Vector3(GetCorrectXMoveValue(position.x + coord[0], position), position.y,
            GetCorrectZMoveValue(position.z + coord[2], position));
    }

    private float GetCorrectXMoveValue(float newX, Vector3 position)
    {
        if (newX > position.x && !axisLocker.PlusX)
        {
            axisLocker.MinusX = false;
            return newX;
        }
        else if (newX > position.x && axisLocker.PlusX)
        {
            return position.x;
        }
        else if (newX < position.x && !axisLocker.MinusX)
        {
            axisLocker.PlusX = false;
            return newX;
        }
        else if (newX < position.x && axisLocker.MinusX)
        {
            return position.x;
        }

        return position.x;
    }

    private float GetCorrectZMoveValue(float newZ, Vector3 position)
    {
        if (newZ > position.z && !axisLocker.PlusZ)
        {
            axisLocker.MinusZ = false;
            return newZ;
        }
        else if (newZ > position.z && axisLocker.PlusZ)
        {
            return position.z;
        }
        else if (newZ < position.z && !axisLocker.MinusZ)
        {
            axisLocker.PlusZ = false;
            return newZ;
        }
        else if (newZ < position.z && axisLocker.MinusZ)
        {
            return position.z;
        }

        return position.z;
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
                var localEulerAngles = rotateSpeed * new Vector3(-Input.GetAxisRaw("Mouse Y"), 0, 0);
                playerCam.transform.eulerAngles += localEulerAngles;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var playerPosition = transform.position;

        float coordCollisionX = (float) Math.Round(collision.contacts[0].point.x, 2);
        float coordPlayerX = (float) Math.Round(playerPosition.x, 2);
        float coordCollisionZ = (float) Math.Round(collision.contacts[0].point.z, 2);
        float coordPlayerZ = (float) Math.Round(playerPosition.z, 2);

        if (!coordCollisionX.Equals(coordPlayerX))
        {
            Debug.Log("!!!!!!!!!!!!!!!kolize na ose XXXXXXXXX");
            if (coordPlayerX > coordCollisionX)
            {
                axisLocker.MinusX = true;
            }
            else
            {
                axisLocker.PlusX = true;
            }
        }

        if (!coordCollisionZ.Equals(coordPlayerZ))
        {
            Debug.Log("!!!!!!!!!!!!!!!kolize na ose ZZZZZZZZZ");

            if (coordPlayerZ > coordCollisionZ)
            {
                axisLocker.MinusZ = true;
            }
            else
            {
                axisLocker.PlusZ = true;
            }
        }

        if (coordCollisionZ.Equals(coordPlayerZ) && coordCollisionX.Equals(coordPlayerX))
        {
            isOnTheGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerPosition = transform.position;
        Debug.Log("exit");
    }

    private void OnCollisionExit(Collision collision)
    {
        var playerPosition = transform.position;
        //isOnTheGround = false;
        //Debug.Log("exit");
        //
        // float coordCollisionX = (float) Math.Round(collision.contacts[0].point.x, 2);
        // float coordPlayerX = (float) Math.Round(playerPosition.x, 2);
        // float coordCollisionZ = (float) Math.Round(collision.contacts[0].point.z, 2);
        // float coordPlayerZ = (float) Math.Round(playerPosition.z, 2);
        //
        // if (!coordCollisionX.Equals(coordPlayerX))
        // {
        //     Debug.Log("!!!!!!!!!!!!!!!kolize na ose XXXXXXXXX");
        //     if (coordPlayerX > coordCollisionX)
        //     {
        //         axisLocker.MinusX = false;
        //     }
        //     else
        //     {
        //         axisLocker.PlusX = false;
        //     }
        // }
        //
        // if (!coordCollisionZ.Equals(coordPlayerZ))
        // {
        //     Debug.Log("!!!!!!!!!!!!!!!kolize na ose ZZZZZZZZZ");
        //
        //     if (coordPlayerZ > coordCollisionZ)
        //     {
        //         axisLocker.MinusZ = false;
        //     }
        //     else
        //     {
        //         axisLocker.PlusZ = false;
        //     }
        // }
        //
        // if (coordCollisionZ.Equals(coordPlayerZ) && coordCollisionX.Equals(coordPlayerX))
        // {
        //     isOnTheGround = false;
        // }
    }

    private CalculatedDataTransporter CalculateXZCoordinates(Vector3 angels, float step, float reduceBy)
    {
        CalculatedDataTransporter transporter = new CalculatedDataTransporter();
        transporter.QuadrantTriangleLegA = (float) (step * Math.Sin(Math.PI * (angels.y - reduceBy) / 180.0));
        transporter.QvadrantTriangleLegB =
            (float) Math.Sqrt(Math.Pow(step, 2) - Math.Pow(transporter.QuadrantTriangleLegA, 2));

        return transporter;
    }

    private float[] GetCoordinatesByAngelToMoveBackAndForward(Vector3 angels, float step)
    {
        float[] coord = {0, 0, 0};

        float direction = angels.y;

        if (0 < direction && direction < 90)
        {
            CalculatedDataTransporter transporter = CalculateXZCoordinates(angels, step, 0);

            coord[0] = transporter.QuadrantTriangleLegA;
            coord[2] = transporter.QvadrantTriangleLegB;
        }
        else if (90 < direction && direction < 180)
        {
            CalculatedDataTransporter transporter = CalculateXZCoordinates(angels, step, 90);

            coord[0] = transporter.QvadrantTriangleLegB;
            coord[2] = -transporter.QuadrantTriangleLegA;
        }
        else if (180 < direction && direction < 270)
        {
            CalculatedDataTransporter transporter = CalculateXZCoordinates(angels, step, 180);

            coord[0] = -transporter.QuadrantTriangleLegA;
            coord[2] = -transporter.QvadrantTriangleLegB;
        }
        else if (270 < direction && direction < 360)
        {
            CalculatedDataTransporter transporter = CalculateXZCoordinates(angels, step, 270);

            coord[0] = -transporter.QvadrantTriangleLegB;
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