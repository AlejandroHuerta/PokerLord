using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Persist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class HiveMind {

        private static HiveMind instance;

        public static HiveMind Instance {
            get {
                if (instance == null) {
                    instance = new HiveMind();
                }
                return instance;
            }
        }

        BasicNetwork network;

        private HiveMind() {
            network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(new FileInfo(@"../../network.eg"));
        }

        public double Compute(double[] state) {
            double[] output = new double[1];
            network.Compute(state, output);
            return output[0];
        }
    }
}
