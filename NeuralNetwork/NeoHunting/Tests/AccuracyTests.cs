using System;
using System.Collections.Generic;
using System.IO;
using Models.Models;
using NeuralNetwork;
using NUnit.Framework;

namespace NeoHunting.Tests
{
    [TestFixture]
    public class PerceptronTests
    {
        private int numberOfTestingTimes = 1;
        private string testRunLogPath = @"D:\Projects\NeoTestData\TestRunLog.txt";
        

        [Test]
        public void MassRecognize()
        {
            for (var i = 0; i < 100; i++)
            {
                RecognizeTens();
            }
        }

        [Test]
        public void RecognizeTens()
        {
            var neasPath = @"D:\Projects\NeoTestData\ToRecognize\Neas.txt";
            var notNeasPath = @"D:\Projects\NeoTestData\ToRecognize\NotNeas8+.txt";

            var notNeoTeachSeqPath = @"D:\Projects\NeoTestData\Teach\TeachNotNeoAllTypes.txt";
            var neoTeachSeqPath = @"D:\Projects\NeoTestData\Teach\TeachNeo32.txt";

            var alpha = 0.0001;
            var beta = 0.1;
            var errorRate = 0.01;
            var threshold = 80;
            var timeout = 20000;
            var inNeurs = 5;
            var outNeurs = 1;

            var parcer = new MPCDataParcer();
            var teachVectors = parcer.ParseToTeachSeqs(notNeoTeachSeqPath, neoTeachSeqPath);

            var configParams = new ConfigParams(inNeurs, outNeurs, alpha, beta, errorRate, timeout, teachVectors);

            var teachedPerceptron = Teach(configParams);

            var linesToRecognize = File.ReadAllLines(neasPath);
            var neasRecognitionResult = Recognize(linesToRecognize, threshold, teachedPerceptron);

            linesToRecognize = File.ReadAllLines(notNeasPath);
            var notNeasRecognitionResult = Recognize(linesToRecognize, threshold, teachedPerceptron);

            LogTestResults(configParams, threshold, neasRecognitionResult, notNeasRecognitionResult);
        }

        private Perceptron Teach(ConfigParams netConfigParams)
        {
            var perceptron = new Perceptron(netConfigParams);

            perceptron.Teach(netConfigParams.TeachVectors);

            return perceptron;
        }

        private double Recognize(string[] recognizeLines, double threshold, Perceptron perceptron)
        {
            double[] recognitionPercentage;
            var resultList = new List<string>();
            var parcer = new MPCDataParcer();
            
            var toRecognizeVectors = new List<TeachSequence>();

            foreach (var line in recognizeLines)
            {
                if (line == String.Empty)
                    continue;
                var normalized = parcer.Normalize(parcer.ParseAndConvertToDouble(line));
                toRecognizeVectors.Add(new TeachSequence { Values = normalized });
            }

            var correctRecognition = 0;
            foreach (TeachSequence toRecognizeVector in toRecognizeVectors)
            {
                recognitionPercentage = perceptron.Recognize(toRecognizeVector);
                if (recognitionPercentage[0] < threshold)
                {
                    correctRecognition++;
                }
                resultList.Add(recognitionPercentage[0].ToString());
            }

            var totalCorrectRating = (double)correctRecognition / (double)toRecognizeVectors.Count * 100;
            
            File.AppendAllLines(testRunLogPath, resultList);
            File.AppendAllLines(testRunLogPath, new[] { DateTime.Now.ToString(), Environment.NewLine });

            return totalCorrectRating;
        }

        private void LogTestResults(ConfigParams configParams,
            int threshold, double neaRecognition, double notNeaRecognition)
        {
            var outputLine = string.Format("Alpha: {0}, Beta: {1}, ErrorRate: {2}," +
                                           " Timeout: {3}, Threshold: {4}, NotNeasInNea: {5:N2}%" +
                                           " NotNeasInNotNeas: {6:N2}%, {7}",
                configParams.Alpha, configParams.Beta, configParams.Error,
                configParams.Timeout, threshold, neaRecognition, notNeaRecognition, DateTime.Now);

            File.AppendAllLines(@"D:\Projects\NeoTestData\TestLog.txt", new[] {outputLine});
        }
        
    }
}
