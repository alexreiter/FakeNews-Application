using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FakenewsApplication

{
    class Program
    {
        private static dynamic path = Directory.EnumerateFiles(@"C:\Users\buisness\Desktop\Brexit_articles\", "*.txt");  
        private static dynamic cleanedFiles;
        private static String[] wordsInInput = Regex.Split(File.ReadAllText(@"C:\Users\buisness\Desktop\Brexit_articles\Input\Input.txt"), @"[\s,;:.!?-]+");
        private static List<String> BagOfWords = new List<String>();
        private static List<Vector> listOfVectors = new List<Vector>();
        private static List<String> listOfInputWords = new List<String>();
        private static int[] inputVector;

        // The kValue decides how many neighbours we should compare our text to. We want as many as makes sense. 
        private static int kValue = 5;

        static void Main(string[] args)
        {
            CreateBoW();
            CreateVector();
            GetWordsFromInput();
            generateInputVector();
            kNNCalculationsAndResult();
        }



        /// <summary>
        /// tokenize the words in my dictionary than add them to a list 
        /// 
        /// </summary>
        public static void CreateBoW()
        {

            int brexit = 1;
 
            foreach (string file in path)
            {
                
                var everyWords = Regex.Split(File.ReadAllText(file), @"[\s,;:.!?-]+");

                //Creating new files with the cleaned words with SteamWriter
                TextWriter tw;
                if (file.Contains("rw"))
                {
                    tw = new StreamWriter(@"C:\Users\buisness\Desktop\Brexit_articles\CleanedText\rw" + brexit.ToString() + ".txt");
                }
                else if (file.Contains("lw"))
                {
                    tw = new StreamWriter(@"C:\Users\buisness\Desktop\Brexit_articles\CleanedText\lw" + brexit.ToString() + ".txt");
                }
                else
                {
                    tw = new StreamWriter(@"C:\Users\buisness\Desktop\Brexit_articles\CleanedText\unkown" + brexit.ToString() + ".txt");
                }

                // Get all words from txt files
                foreach (var word in everyWords)
                {
                    // Start a StringBuilder to create a mutable sequence of characters
                    var builder = new StringBuilder();

                    // Go through each char in a single word 
                    foreach (char letter in word)
                    {
                        // If the char matches basic latin alphabet (without special chars and punctuations) 
                        //than the program will add them to the StringBuilder
                        if (letter <= 90 && letter >= 65 || letter >= 97 && letter <= 122)
                        {
                            builder.Append(letter.ToString().ToLower());
                        }
                    }

                    // All words which are longer than 2 chars will be added to the list
                    if (builder.Length > 2)
                    {
                        // word is now written in a text file
                        tw.WriteLine(builder);

                        if (!BagOfWords.Contains(builder.ToString()))
                        {
                            BagOfWords.Add(builder.ToString());
                        }
                    }
                }
                tw.Close();
                brexit++;
            }
        }


        /// <summary>
        /// 
        /// Creates vectors based on the BOW and compare texts to each other
        /// </summary>
        public static void CreateVector()
        {
            cleanedFiles = Directory.EnumerateFiles(@"C:\Users\buisness\Desktop\Brexit_articles\CleanedText\", "*.txt");
           
            foreach (var cleanedFile in cleanedFiles)
            {
                // Load all words in that file into a string array.
                String[] Words = Regex.Split(File.ReadAllText(cleanedFile), @"[\s,;:.!?-]+");

                String label = ""; 

                // Create a new int array with the length corresponding to the BagOfWords length.
                int[] temporaryVectorList = new int[BagOfWords.Count];

                // For every word in BagOfWords, try to look it up in cleaned Textfile. If the BagOfWord WORD is present in the file, return value 1, else 0. 
                for (int i = 0; i < BagOfWords.Count; i++)
                {
                    if (Words.Contains(BagOfWords[i]))
                    {
                        temporaryVectorList[i] = 1;
                    }
                    else
                    {
                        temporaryVectorList[i] = 0;
                    }
                }

                // Based on the file name of the cleaned textfile, identify which theme it belongs to and save that as the attached label.
                if (cleanedFile.Contains("rw"))
                {
                    label = "rw";
                }
                else if (cleanedFile.Contains("lw"))
                {
                    label = "lw";
                }

                // Add a new entry into my listOfVector with the Object Vector, which consists of a label to indentify it's belonging, along with an int vector of 0,0,0,0,1,1,1 etc.
                listOfVectors.Add(new Vector(label, temporaryVectorList));
            }
        }

        /// <summary>
        /// 
        /// Creates a list based on a new input text, add words to that list from the input text file
        /// </summary>
        private static void GetWordsFromInput()
        {
            foreach (var word in wordsInInput)
            {

                var builder = new StringBuilder();

                foreach (char letter in word)
                {

                    if (letter <= 90 && letter >= 69 || letter >= 97 && letter <= 122)
                    {
                        builder.Append(letter);
                    }

                    listOfInputWords.Add(builder.ToString().ToLower());
                }
            }
        }

        /// <summary>
        /// 
        /// Creates vectors based on the input text file and compare its words to BoW
        /// </summary>
        static void generateInputVector()
        {

            inputVector = new int[BagOfWords.Count];

            for (int i = 0; i < BagOfWords.Count; i++)
            {
                if (wordsInInput.Contains(BagOfWords[i]))
                {
                    inputVector[i] = 1;
                }
                else
                {
                    inputVector[i] = 0;
                }
            }

        }

        /// <summary>
        /// 
        /// Calculate the distance between vectors and based on that the application will detect if the input file belongs to left wing or right wing category
        /// </summary>
        private static void kNNCalculationsAndResult()
        {
            //a list initialized which will contain all distances along with their labels
            List<Result> vectorResults = new List<Result>();

            // For every testdata vector, do the following:
            for (int l = 0; l < listOfVectors.Count; l++)
            {
                double denominator = 0;

                // Calculate the difference between index[i] in the input vector with the index[i] of first testvector
                for (int i = 0; i < inputVector.Length; i++)
                {
                    denominator += Math.Pow(inputVector[i] - (int)listOfVectors.ElementAt(l).vector.GetValue(i), 2);
                }
                // Based on the total sum above, get the distance by doing a square root calculation 
                double distance = Math.Sqrt(denominator);

                //save that result in result list
                vectorResults.Add(new Result(listOfVectors.ElementAt(l).label, distance));
            }

            //sort the distances from lowest to highest and save the result in a new list
            List<Result> orderedResultList = vectorResults.OrderBy(o => o.result).ToList();

            //For testing only - I use this to print our all distances + their labels.
            foreach (var result in orderedResultList)
            {
                Console.WriteLine("TEST "+result.result.ToString() + " | " + result.label.ToString());
            }

            int counterRightWing = 0;
            int counterLeftWing = 0;

            //
            for (int i = 0; i < kValue; i++)
            {
                if (orderedResultList.ElementAt(i).label.ToString().Equals("rw"))
                {
                    counterRightWing++;
                }
                else if (orderedResultList.ElementAt(i).label.ToString().Equals("lw"))
                {
                    counterLeftWing++;
                }
            }
            // check if the input text whether is a right or a left wing article
            if (counterRightWing > counterLeftWing)
            {
                Console.WriteLine("Your text is recognized as a right wing article");
            }
            else if (counterRightWing < counterLeftWing)
            {
                Console.WriteLine("Your text is recognized as being a Left Wing oriented article");
            }
            else if (counterRightWing == counterLeftWing)
            {
                Console.WriteLine("With the current k value we can't determine what the article resembles the most");
            }
        }
    
        }
    }
   








        

      