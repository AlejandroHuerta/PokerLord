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
            var inputs = new List<double[]>();
            var outputs = new List<double[]>();

            var lines = File.ReadLines(@"../../states.txt");
            foreach (var line in lines) {
                var lineInput = new List<double>();
                var lineOutput = new List<double>();

                var weights = line.Split(new char[] { ',' });
                lineInput.AddRange(weights.Take(8).Select(value => double.Parse(value)));
                lineOutput.Add(double.Parse(weights[8]));
                lineInput.AddRange(weights.Skip(9).Select(value => double.Parse(value)));

                inputs.Add(lineInput.ToArray());
                outputs.Add(lineOutput.ToArray());
            }//foreach

            var nnInput = inputs.ToArray();
            var nnIdeal = outputs.ToArray();

            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 16));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, 300));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();

            IMLDataSet trainingSet = new BasicMLDataSet(nnInput, nnIdeal);

            IMLTrain train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;

            do {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + "Error:" + train.Error);
                epoch++;
            } while (train.Error > 0.01);

            EncogDirectoryPersistence.SaveObject(new FileInfo("network.eg"), network);

            Console.ReadKey();
        }//Main
    }//Program
}
