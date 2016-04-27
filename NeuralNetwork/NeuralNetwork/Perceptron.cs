using System;
using System.Collections.Generic;
using Models.Models;
using NLog;

namespace NeuralNetwork
{
    public class Perceptron
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private Logger logger = LogManager.GetCurrentClassLogger();

        private ConfigParams perceptron;

        private int inputNeurNumber;

        private byte hiddenLayerNeurNumber;

        private int ouputNeurNumber;

        private double[,] hiddenLayerValues;

        private double[,] outputLayerValues;

        private double[] hiddenLayerThreshold;

        private double[] thresholdOfOutputLayer;

        private double[] hiddenLayerWithSigma;

        private double[] outputThresholdWithSigma;

        private double[] errorsOfOutputLayer;

        private double[] errorsOfHiddenLayer;

        public int NumberOfIterations { get; private set; }

        public Perceptron(ConfigParams perceptron)
        {
            this.perceptron = perceptron;
            this.inputNeurNumber = perceptron.InputNeurons;
            hiddenLayerNeurNumber = (byte)(inputNeurNumber / 2);
            this.ouputNeurNumber = perceptron.OutputNeurons;

            hiddenLayerValues = GenerateMatrix(inputNeurNumber, hiddenLayerNeurNumber);
            outputLayerValues = GenerateMatrix(hiddenLayerNeurNumber, ouputNeurNumber);

            hiddenLayerThreshold = new double[hiddenLayerNeurNumber];
            thresholdOfOutputLayer = new double[ouputNeurNumber];
            
            hiddenLayerWithSigma = new double[hiddenLayerNeurNumber];
            outputThresholdWithSigma = new double[ouputNeurNumber];
            
            errorsOfOutputLayer = new double[hiddenLayerNeurNumber];
            errorsOfHiddenLayer = new double[ouputNeurNumber];
        }

        private double[,] GenerateMatrix(int width, int height)
        {
            return new double[width, height];
        }
        
        private void InitializeRandomly(double[,] layer, int width, int height)
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    layer[i, j] = GetRandomValue();
                }
            }
            
        }

        private void InitializeRandomly(double[] layer)
        {
            for (var i = 0; i < layer.Length; i++)
            {
                layer[i] = GetRandomValue();
            }
        }

        private double GetRandomValue()
        {
            var rand = Random.NextDouble();

            if (Random.Next(0, 2) == 1)
            {
                rand = -rand;
            }

            return rand;
        }

        private void InitializeLayers()
        {
            InitializeRandomly(hiddenLayerValues, inputNeurNumber, hiddenLayerNeurNumber);
            InitializeRandomly(outputLayerValues, hiddenLayerNeurNumber, ouputNeurNumber);

            InitializeRandomly(hiddenLayerThreshold);
            InitializeRandomly(thresholdOfOutputLayer);
        }

        private double TeachVector(TeachSequence inputVector)
        {
            var maxError = 0.0;
            
            for (var i = 0; i < hiddenLayerNeurNumber; i++)
            {
                var q = hiddenLayerThreshold[i]; //random initialization

                for (var j = 0; j < inputNeurNumber; j++)
                {
                    q += hiddenLayerValues[j, i] * inputVector.Values[j];
                }

                hiddenLayerWithSigma[i] = Sigma(q);
            }
            
            for (var i = 0; i < ouputNeurNumber; i++)
            {
                var t = thresholdOfOutputLayer[i];

                for (var j = 0; j < hiddenLayerNeurNumber; j++)
                {
                    t += outputLayerValues[j, i] * hiddenLayerWithSigma[j];
                }

                outputThresholdWithSigma[i] = Sigma(t);
            }
            
            for (var i = 0; i < ouputNeurNumber; i++)
            {
                errorsOfHiddenLayer[i] = inputVector.IsCorrect - outputThresholdWithSigma[i];
               
                var abs = Math.Abs(errorsOfHiddenLayer[i]);

                if (abs > maxError)
                {
                    maxError = abs;
                }
            }
            
            for (var i = 0; i < hiddenLayerNeurNumber; i++)
            {
                errorsOfOutputLayer[i] = 0.0;

                for (var j = 0; j < ouputNeurNumber; j++)
                {
                    errorsOfOutputLayer[i] += errorsOfHiddenLayer[j] * (outputThresholdWithSigma[j] * (1 - outputThresholdWithSigma[j])) * outputLayerValues[i, j];
                }
            }
            
            for (var i = 0; i < hiddenLayerNeurNumber; i++)
            {
                for (var j = 0; j < ouputNeurNumber; j++)
                {
                    outputLayerValues[i, j] += perceptron.Alpha * outputThresholdWithSigma[j] * (1 - outputThresholdWithSigma[j]) * errorsOfHiddenLayer[j] * hiddenLayerWithSigma[i];
                }
            }
            
            for (var i = 0; i < ouputNeurNumber; i++)
            {
                thresholdOfOutputLayer[i] += perceptron.Alpha * outputThresholdWithSigma[i] * (1 - outputThresholdWithSigma[i]) * errorsOfHiddenLayer[i];
            }      
            
            for (var i = 0; i < inputNeurNumber; i++)
            {
                for (var j = 0; j < hiddenLayerNeurNumber; j++)
                {
                    hiddenLayerValues[i, j] += perceptron.Beta * hiddenLayerWithSigma[j] * (1 - hiddenLayerWithSigma[j]) * errorsOfOutputLayer[j] * inputVector.Values[i];
                }        
            }   
            
            for (var i = 0; i < hiddenLayerNeurNumber; i++)
            {
                hiddenLayerThreshold[i] += perceptron.Beta * hiddenLayerWithSigma[i] * (1 - hiddenLayerWithSigma[i]) * errorsOfOutputLayer[i];
            }

            return maxError;
        }

        private double Sigma(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }

        private bool IsTeached(double maxError)
        {
            return maxError < perceptron.Error || NumberOfIterations >= perceptron.Timeout;
        }

        public void Teach(List<TeachSequence> vectors)
        {
            InitializeLayers();

            NumberOfIterations = 0;
            double maxError;
            var teached = false;
            var consoleCount = 0;

            while (!teached)
            {
                NumberOfIterations++;
                
                maxError = 0.0;

                foreach (var vector in vectors)
                {
                    var error = TeachVector(vector);
                    if (error > maxError)
                    {
                        maxError = error;
                    }
                    consoleCount++;

                    if (consoleCount == 100)
                    {
                        logger.Info("Iterations: {0}, Error: {1}", NumberOfIterations, maxError);
                        consoleCount = 0;
                    }
                }

                teached = IsTeached(maxError);
            }
        }

        public double[] Recognize(TeachSequence seqToRecognize)
        {
            for (var i = 0; i < hiddenLayerNeurNumber; i++)
            {
                var q = hiddenLayerThreshold[i];

                for (var j = 0; j < inputNeurNumber; j++)
                {
                    q += hiddenLayerValues[j, i] * seqToRecognize.Values[j];
                }

                hiddenLayerWithSigma[i] = Sigma(q);
            }
            
            for (var i = 0; i < ouputNeurNumber; i++)
            {
                var t = thresholdOfOutputLayer[i];

                for (var j = 0; j < hiddenLayerNeurNumber; j++)
                {
                    t += outputLayerValues[j, i] * hiddenLayerWithSigma[j];
                }

                outputThresholdWithSigma[i] = Sigma(t);
            }

            var result = new double[ouputNeurNumber];

            for (var i = 0; i < ouputNeurNumber; i++)
            {
                result[i] = outputThresholdWithSigma[i] * 100;
            }

            return result;
        }
    }
}
