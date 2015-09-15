using UnityEngine;

public class TankController : MonoBehaviour {

    public float fitness = 0;

    // Update is called once per frame
    void Update() {
        Vector3 newPosition = transform.position;

        if (transform.position.x > Master.master.screenX) {
            newPosition.x = -newPosition.x +1;
        } else if (transform.position.x < -Master.master.screenX) {
            newPosition.x = -newPosition.x -1;
        }

        if (transform.position.z > Master.master.screenY) {
            newPosition.z = -newPosition.z +1;
        } else if (transform.position.z < -Master.master.screenY) {
            newPosition.z = -newPosition.z -1;
        }

        transform.position = newPosition;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "point") {
            fitness += 1;
            Master.master.RemovePoint(col.gameObject);
            Master.master.AddPoint();
        }
    }
}
