using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public struct chromosome {

    public ga ecosystem;
    public List<byte> genes;
    public float fitness;

    public chromosome(int chromosomeLength, ga _ecosystem) {
        genes = new List<byte>();
        ecosystem = _ecosystem;
        fitness = 1;

        for (int i = 0; i < chromosomeLength; i++) {
            genes.Add((byte)Random.Range(0, ecosystem.codex.Count));
        }
    }
}

public class ga {

    public Dictionary<byte, float> codex = new Dictionary<byte, float>();

    private List<KeyValuePair<object, float>> thisProb = new List<KeyValuePair<object, float>>();
    private ProbGen probGen;

    public int populationSize, cycle = 0;
    public float mattingDelay = 0, bestFitness = 0, avgFitness = 0;
    public List<chromosome> population = new List<chromosome>();

    public ga(int _populationSize, int chromosomeLength, Dictionary<byte, float> _codex) {
        populationSize = _populationSize;
        codex = _codex;

        thisProb.Add(new KeyValuePair<object, float>(true, .35f));
        thisProb.Add(new KeyValuePair<object, float>(false, .65f));

        probGen = new ProbGen(thisProb);

        for (int i = 0; i < populationSize; i++) {
            population.Add(new chromosome(chromosomeLength, this));
        }
    }

    public List<List<float>> Decode() {
        List<List<float>> returnVal = new List<List<float>>();

        foreach (chromosome chr in population) {
            List<float> chrGenes = new List<float>();
            foreach (byte key in chr.genes) {
                float val;
                if (codex.TryGetValue(key, out val)) {
                    chrGenes.Add(val);
                } else {
                    chrGenes.Add(0);
                }
            }
            returnVal.Add(chrGenes);
        }
        return returnVal;
    }

    public List<float> Decode(chromosome chromosomeToDecode) {
        List<float> returnVal = new List<float>();

        foreach (byte key in chromosomeToDecode.genes) {
            float val;
            if (codex.TryGetValue(key, out val)) {
                returnVal.Add(val);
            } else {
                returnVal.Add(0);
            }
        }
        return returnVal;
    }

    private void CyclePopulation() {
        List<KeyValuePair<object, float>> fitnessProbList = new List<KeyValuePair<object, float>>();
        float magnitude = 0;
        foreach(chromosome chr in population) {
            magnitude += chr.fitness;
        }
        for(int k = 0; k < population.Count-1; k++) {
            fitnessProbList.Add(new KeyValuePair<object, float>(k, population[k].fitness-1 / magnitude));
        }
        ProbGen fitnessProb = new ProbGen(fitnessProbList);

        for (int i = 0; i < population.Count-1; i++) {
            chromosome thisChr = population[i];
            chromosome partner = population[(int)fitnessProb.next()];

            for(int j = 0; j < thisChr.genes.Count; j++) {
                if(Random.value > .5f) {
                    thisChr.genes[j] = partner.genes[j];
                }
            }
            population[i] = thisChr;
        }
    }

    private void Mutate() {
        foreach (chromosome chr in population) {
            for (int i = 0; i < chr.genes.Count; i++) {
                if ((bool)probGen.next()) {
                    chr.genes[i] = (byte)~chr.genes[i];
                }
            }
        }
    }

    public IEnumerator ReproductionCycle() {
        while (true) {
            yield return new WaitForSeconds(mattingDelay);

            CyclePopulation();
            Mutate();

            cycle++;
        }
    }
}
