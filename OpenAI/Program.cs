// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace OpenAI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the game of fooling! \nThe AI is tricky and wants to fool you and your job is to guess if the AI is playing tricks, or answering the questions correctly!\nLET'S START! \n \n");

            while (true)
            {                
                KeepPlayingGame();
            }
        }      
        

        static void KeepPlayingGame()
        {
            
            Hashtable questions = new Hashtable();
            questions.Add(0, "Were the Olympics in Barcelona in 1992?");
            questions.Add(1, "Is Canada the biggest country in the world?");
            questions.Add(2, "Is Joe Biden the president of the United States?");
            questions.Add(3, "Did Johnny Depp play Captain Jack Sparrow?");
            questions.Add(4, "Did Liverpool won the Champions League in 2005?");
            questions.Add(5, "Is Brazil population more than 300 million people?");
            questions.Add(6, "Is China the most populated country in the world?");
            questions.Add(7, "Did someone live over 120 years?");
            questions.Add(8, "Does 865*17=14705?");
            questions.Add(9, "Is Georgia in Asia?");

            int key = RandomizeQuestion(questions.Count);
            int initialHashtableSize = questions.Count;
            int score = 0;
            string answerToQuestion = AskQuestion(questions[key].ToString());

            Console.WriteLine(questions[key].ToString());
            int shouldLie = ShouldLie();
            if (shouldLie == 1) answerToQuestion = ModifyAnswer(answerToQuestion);
            Console.WriteLine(answerToQuestion);
            Console.WriteLine("Was the AI's answer correct? (Y/N)");
            string userAnswer = Console.ReadLine();
            if ((userAnswer == ("Y") && shouldLie == 0) || (userAnswer == ("y") && shouldLie == 0) || (userAnswer == ("N") && shouldLie == 1) || (userAnswer == ("n") && shouldLie == 1))
            {
                Console.WriteLine("You're right!\n");
                score += 1;
            }
            else Console.WriteLine("You're wrong!\n");           
            
            
        }
        private static string ModifyAnswer(string originalAnswer)
        {
            string modifiedAnswer = "Yes";
            if (originalAnswer.Contains("Yes")) modifiedAnswer = "No.";
            if (originalAnswer.Contains("No")) modifiedAnswer = "Yes.";

            return modifiedAnswer;
        }
        private static int ShouldLie()
        {
            return new Random().Next(2);
        }

        private static int RandomizeQuestion(int hashtableSize)
        {
            return new Random().Next(hashtableSize);
        }

        private static string AskQuestion(string question)
        {
            string questionAfterLearning = TeachAIAnswerModel(question);
            string answer = callOpenAI(150, questionAfterLearning, "text-davinci-002", 0, 1, 0, 0);
            return answer;
        }

        private static string TeachAIAnswerModel(string question)
        {
            return "Is Michael Jackson alive? No. Is Barack Obama the president of the United States? No. Is Andrzej Duda the president of Poland? Yes. Is 2*2=4? Yes. " + question;
        }

        private static string callOpenAI(int tokens, string input, string engine,
          double temperature, int topP, int frequencyPenalty, int presencePenalty)
        {
            var openAiKey = "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";
            var apiCall = "https://api.openai.com/v1/engines/" + engine + "/completions";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), apiCall))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + openAiKey);
                        request.Content = new StringContent("{\n  \"prompt\": \"" + input + "\",\n  \"temperature\": " +
                                                            temperature.ToString(CultureInfo.InvariantCulture) + ",\n  \"max_tokens\": " + tokens + ",\n  \"top_p\": " + topP +
                                                            ",\n  \"frequency_penalty\": " + frequencyPenalty + ",\n  \"presence_penalty\": " + presencePenalty + "\n}");
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        var response = httpClient.SendAsync(request).Result;
                        var json = response.Content.ReadAsStringAsync().Result;
                        dynamic dynObj = JsonConvert.DeserializeObject(json);
                        if (dynObj != null)
                        {
                            return dynObj.choices[0].text.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        //


    }

}

