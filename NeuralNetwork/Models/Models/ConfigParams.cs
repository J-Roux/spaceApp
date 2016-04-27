using System.Collections.Generic;

namespace Models.Models
{
    public class ConfigParams
    {
        public double Alpha { get; private set; }

        public double Beta { get; private set; }

        public double Error { get; private set; }

        public int Timeout { get; private set; }

        public List<TeachSequence> TeachVectors { get; private set; }

        public int InputNeurons { get; set; }

        public int OutputNeurons { get; set; }

        public ConfigParams(int inNeurs, int outNeurs, double alpha, double beta, double error,
            int timeout, List<TeachSequence> vectors)
        {
            Alpha = alpha;
            Beta = beta;
            Error = error;
            Timeout = timeout;
            TeachVectors = vectors;
            InputNeurons = inNeurs;
            OutputNeurons = outNeurs;
        }
    }
}
