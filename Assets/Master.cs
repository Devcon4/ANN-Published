using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class Master : MonoBehaviour {

    public Text cycleT, bestT, avgT, avgH; 

    public static Master master;

    public GameObject tank, point;
    public int tankCount, pointCount;
    public int screenX, screenY;
    public float tankSpeed, waitTime;

    public List<string> Log = new List<string>();
    public List<float> avgHistory = new List<float>();
    public Dictionary<byte, float> codex = new Dictionary<byte, float>();
    public List<Organism> Census = new List<Organism>();
    public List<GameObject> activePoints = new List<GameObject>();
    public List<float> localFitness = new List<float>();
    public ga ecosystem;

    private float lastCycle = 0;
    private int totalPopulation = 1;

    void Start() {
        InitCodex();
        InitPoints();

        ecosystem = new ga(tankCount, 24, codex);
        ecosystem.mattingDelay = waitTime / Time.timeScale;

        CreatePopulation();

        StartCoroutine(ecosystem.ReproductionCycle());
    }

    void Awake() {
        if (master == null) {
            DontDestroyOnLoad(gameObject);
            master = this;
        } else if (master != this) {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (ecosystem.cycle > lastCycle) {
            cycleT.text = "Cycle: " + ecosystem.cycle;
            CalculateLocalFitness();
            //RemoveWorst();
            RandomizeTanks();
            lastCycle = ecosystem.cycle;
            //Census.ForEach((t) => t.brain.setWeights(ecosystem.Decode()));
            //ecosystem.Decode().ForEach((t) => t.ForEach((f) => Debug.Log(f)));
        }
    }

    private void InitCodex() {

        codex.Add(1, .1f);
        codex.Add(2, .2f);
        codex.Add(3, .3f);
        codex.Add(4, .4f);
        codex.Add(5, .5f);
        codex.Add(6, .6f);
        codex.Add(7, .7f);
        codex.Add(8, .8f);
        codex.Add(9, .9f);
        codex.Add(0, .0f);

        codex.Add(11, .01f);
        codex.Add(12, .02f);
        codex.Add(13, .03f);
        codex.Add(14, .04f);
        codex.Add(15, .05f);
        codex.Add(16, .06f);
        codex.Add(17, .07f);
        codex.Add(18, .08f);
        codex.Add(19, .09f);
        codex.Add(10, .00f);

        codex.Add(21, .001f);
        codex.Add(22, .002f);
        codex.Add(23, .003f);
        codex.Add(24, .004f);
        codex.Add(25, .005f);
        codex.Add(26, .006f);
        codex.Add(27, .007f);
        codex.Add(28, .008f);
        codex.Add(29, .009f);
        codex.Add(20, .000f);

        codex.Add(31, .0001f);
        codex.Add(32, .0002f);
        codex.Add(33, .0003f);
        codex.Add(34, .0004f);
        codex.Add(35, .0005f);
        codex.Add(36, .0006f);
        codex.Add(37, .0007f);
        codex.Add(38, .0008f);
        codex.Add(39, .0009f);
        codex.Add(30, .0000f);

        codex.Add(41, .00001f);
        codex.Add(42, .00002f);
        codex.Add(43, .00003f);
        codex.Add(44, .00004f);
        codex.Add(45, .00005f);
        codex.Add(46, .00006f);
        codex.Add(47, .00007f);
        codex.Add(48, .00008f);
        codex.Add(49, .00009f);
        codex.Add(40, .00000f);
    }

    private void InitPoints() {
        for(int i = 0; i < pointCount; i++) {
            activePoints.Add(AddPoint());
        }
    }

    private void CreatePopulation() {
        for (int i = 0; i < tankCount; i++) {
            Census.Add(CreateOrganism("Tank-" + totalPopulation));
        }
    }

    private Organism CreateOrganism(string name) {
        GameObject body = AddTank();
        Vector3 inputVal = UnityTools.ClosestWithTag(tank, "point").transform.position;
        ann brain = new ann(6, 3, 2, 5, new List<float>() { tank.transform.position.x,
                                                                tank.transform.position.y,
                                                                tank.transform.position.z,
                                                                inputVal.x,
                                                                inputVal.y,
                                                                inputVal.z});
        chromosome dna = new chromosome(24, ecosystem);
        Organism thisOrg = new Organism(body, brain, dna, ecosystem);
        thisOrg.name = name;
        body.name = name;
        totalPopulation++;
        StartCoroutine(thisOrg.BodyClock());
        Log.Add(name + "was born.");
        return thisOrg;
    }

    private void CalculateLocalFitness() {
        localFitness = new List<float>();
        foreach (Organism org in Census) {
            localFitness.Add(org.dna.fitness);
        }

        avgHistory.Add(localFitness.Average());

        bestT.text = "Best Fitness: " + localFitness.Max();
        avgT.text = "Average Fitness: " + localFitness.Average();
        avgH.text = "Total Average: " + avgHistory.Average();

        Log.Add(bestT.text);
        Log.Add(avgT.text);
        Log.Add(avgH.text);
    }

    public GameObject AddTank() {
        return Instantiate(tank, new Vector3(Random.Range(-screenX, screenX), .1f, Random.Range(-screenY, screenY)), Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
    }

    public void RemoveWorst() {
        Organism worst = Census.MinOf(t => t.dna.fitness);
        Log.Add(worst.name + " died of competition.");
        Census.Remove(worst);
        Destroy(worst.body);
        Census.Add(CreateOrganism("Tank-" + totalPopulation));
    }

    public GameObject AddPoint() {
        return Instantiate(point, new Vector3(Random.Range(-screenX, screenX), .1f, Random.Range(-screenY, screenY)), Quaternion.identity) as GameObject;
    }

    public void RemovePoint(GameObject point) {
        activePoints.Remove(point);
        Destroy(point);
    }

    public void RandomizeTanks() {
        for(int i = 0; i < Census.Count; i++) {
            Destroy(Census[i].body);
            Census[i].body = AddTank();
            Census[i].body.name = Census[i].name;
            Census[i].dna.fitness = 0;
        }
    }

    public void SpeedControl(int val) {
        Time.timeScale = val;
        Time.fixedDeltaTime = val;
    }
}