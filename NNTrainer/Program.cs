using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;

namespace NNTrainer {
    class Program {
        static void Main(string[] args) {
            const int INPUT_VARIABLES = 52;

            var inputs = new List<double[]>();
            var outputs = new List<double[]>();

            var lines = File.ReadLines(@"../../states.txt");
            foreach (var line in lines) {
                var lineInput = new List<double>();
                var lineOutput = new List<double>();

                var weights = line.Split(new char[] { ',' });
                lineInput.AddRange(weights.Take(INPUT_VARIABLES).Select(value => double.Parse(value)));
                lineOutput.Add(double.Parse(weights[INPUT_VARIABLES]));

                inputs.Add(lineInput.ToArray());
                outputs.Add(lineOutput.ToArray());
            }//foreach

            var nnInput = inputs.ToArray();
            var nnIdeal = outputs.ToArray();

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, INPUT_VARIABLES));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 200));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 100));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            IMLDataSet trainingSet = new BasicMLDataSet(nnInput, nnIdeal);

            IMLTrain train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;

            do {
                train.Iteration();
                Console.WriteLine("Epoch #{0,-10} Error: {1}", epoch, train.Error);
                epoch++;
            } while (train.Error > 0.01);

            EncogDirectoryPersistence.SaveObject(new FileInfo("network.eg"), network);

            Console.ReadKey();
        }//Main
    }//Program
}
