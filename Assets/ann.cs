using UnityEngine;
using System.Collections.Generic;

public struct neuron {
    public List<float> weights;

    public neuron(int inputCount) {
         weights = new List<float>();
        for (int i = 0; i <= inputCount; i++) {
            weights.Add(Random.value);
        }
    }
}

public struct neuronLayer {
    public List<neuron> neurons;

    public neuronLayer(int neuronCount, int inputCount){
        neurons = new List<neuron>();
        for (int i = 0; i < neuronCount; i++) {
            neurons.Add(new neuron(inputCount));
        }
    }
}

public class ann {

    public int inputCount, outputCount, hiddenLayerCount, neuronsPerLayer;
    public List<neuronLayer> layers = new List<neuronLayer>();

    public List<float> inputs = new List<float>();
    public List<float> outputs = new List<float>();

    public ann(int _inputCount, int _outputCount, int _hiddenLayerCount, int _neuronsPerLayer, List<float> _inputs) {
        layers.Clear();

        inputCount = _inputCount;
        outputCount = _outputCount;
        hiddenLayerCount = _hiddenLayerCount;
        neuronsPerLayer = _neuronsPerLayer;
        inputs = _inputs;

        for (int i = 0; i < hiddenLayerCount; i++) {
            layers.Add(new neuronLayer(neuronsPerLayer, inputCount));
        }

        layers.Add(new neuronLayer(outputCount, neuronsPerLayer));
    }

    public void Next() {
        for (int i = 0; i < layers.Count; i++) {

            if (i > 0) {
                inputs = outputs;
            }

            outputs.Clear();
            List<neuron> thisNeurons = layers[i].neurons;
            for (int j = 0; j < thisNeurons.Count; j++) {
                List<float> thisWeights = thisNeurons[j].weights;
                float value = 0;
                for (int k = 0; k < inputs.Count-1; k++) {

                    value += thisWeights[k] * inputs[k];
                }

                value += (-1 * thisWeights[thisWeights.Count-1]);
                value = sigmoid(value);
                outputs.Add(value);
            }
        }
    }

    public void Next(List<float> _inputs) {
        inputs = _inputs;
        for (int i = 0; i < layers.Count; i++) {

            if (i > 0) {
                inputs = outputs;
            }

            outputs = new List<float>();
            List<neuron> thisNeurons = layers[i].neurons;
            for (int j = 0; j < thisNeurons.Count; j++) {
                List<float> thisWeights = thisNeurons[j].weights;
                float value = 0;
                for (int k = 0; k < inputs.Count-1; k++) {

                    value += thisWeights[k] * inputs[k];
                }
                value += (-1 * thisWeights[thisWeights.Count-1]);
                value = sigmoid(value);
                outputs.Add(value);
            }
        }
    }

    public void setWeights(List<List<float>> newWeights) {
        int total = 0;
        for(int i = 0; i < layers.Count; i++) {
            for(int j = 0; j < layers[i].neurons.Count; j++) {
                layers[i].neurons.ToArray()[j].weights = newWeights[total];
                total++;
            }
        }
    }

    private float sigmoid(float val) {
        return (2 / Mathf.Sqrt(1.0f + (val * val)))-1.0f;
    }
}