using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organism {

    public string name;
    public GameObject body;
    public ann brain;
    public chromosome dna;
    public ga ecosystem;
    public float cycleDelay;

    public Organism(GameObject _body, ann _brain, chromosome _dna, ga _ecosystem) {
        body = _body;
        brain = _brain;
        dna = _dna;
        ecosystem = _ecosystem;

        cycleDelay = 0.016f;
        name = "";

    }

    public IEnumerator BodyClock() {
        while (true && body != null) {
            Movement();
            yield return new WaitForSeconds(cycleDelay / Time.timeScale);
        }
    }

    private void Movement() {
        Vector3 inputVal = UnityTools.ClosestWithTag(body, "point").transform.position;
        brain.Next(new List<float>() { body.transform.position.x,
                                       body.transform.position.y,
                                       body.transform.position.z,
                                       inputVal.x,
                                       inputVal.y,
                                       inputVal.z});

        //brain.Next();
        List<Vector3> thoughts = new List<Vector3>();
        List<float> floats = brain.outputs;
        for(int i = 0; i < floats.Count; i+=3) {
            thoughts.Add(new Vector3(floats[i], floats[i + 1], floats[i + 2]));
        }

        Vector3 nextPos = new Vector3();
        thoughts.ForEach((t) => nextPos += t);
        nextPos.y = 0;

        body.transform.Translate(nextPos.normalized);
        dna.fitness = body.GetComponent<TankController>().fitness;
    }
}
