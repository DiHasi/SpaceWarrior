using UnityEngine;

public class CubeScript : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        // transform.RotateAround(new Vector3(323f, 33f, 666f), Vector3.up, 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // print(1);
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        Destroy(collision.gameObject); 
    }
}
