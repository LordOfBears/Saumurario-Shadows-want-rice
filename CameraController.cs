using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float cameraPozZ = -15f;
    private Vector3 pos;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<Hero>().transform;
    }

    // Update is called once per frame
    private void Update()
    {
        pos = player.position;
        pos.z = cameraPozZ;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
    }
}
