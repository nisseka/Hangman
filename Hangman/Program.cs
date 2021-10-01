using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Assignment #2, Hangman 
 * 
 * by
 * 
 * Stefan Sundbeck
*/


namespace Hangman
{
    class Program
    {
        static int secret_word_length;
        static char[] guessed_word;
        static int guesses_left;
        static StringBuilder incorrectly_guessed_letters;
        static bool external_wordlist;
        static int wordsCount;

        static string[] wordlist = {
                         "pengar",
                         "regissör",
                         "blockera",
                         "illa",
                         "styrka",
                         "född",
                         "blott",
                         "värde",
                         "respektive",
                         "börja",
                         "drottning",
                         "grön",
                         "halva",
                         "serie",
                         "leva",
                         "mod",
                         "dator",
                         "perfekt",
                         "stoppa",
                         "förklara"
        };


        static void Main(string[] args)
        {
            Random rnd = new Random();
            int random_word_index;
            int i;
            int guessed_right_letters;
            int numberOfGuesses = 10;
            string secret_word, str;
            char key_pressed;
            char key_pressed_lowercase;
            bool exit = false;
            bool won;
            bool letter_found;
            bool letter_already_found;
            string wordlist_filename;
            StringBuilder sb_wordlist_filename = new StringBuilder(Directory.GetCurrentDirectory());   // Get the full path of the program executable file

            i = sb_wordlist_filename.ToString().IndexOf("bin");                                        // Locate where the bin folder is in the path
            if (i >= 0)
            {
                sb_wordlist_filename.Remove(i, sb_wordlist_filename.Length - i);                       // Delete everything starting from the bin folder position to the end of the string
            } else
                sb_wordlist_filename.Append('\\');                                                     // bin folder not found in the path; use the program file path

            sb_wordlist_filename.Append("wordlist.txt");                                               // We now have a path to the word list file which should be located in the project folder            
            wordlist_filename = sb_wordlist_filename.ToString();

            external_wordlist = false;

            if (File.Exists(wordlist_filename))
            {
                string[] file_content = File.ReadAllLines(wordlist_filename);

                if (file_content.Length >= 5)                                                          // Make sure that the wordlist file has atleast 5 words in it
                {                                                                                      // otherwise use the internal wordlist 
                    wordlist = file_content;
                    external_wordlist = true;
                }
            }

            incorrectly_guessed_letters = new StringBuilder();
            wordsCount = wordlist.Length;

	    for (i = 0; i < wordsCount; i++)                                                            // Convert all the words to lowercase
	    {
                str = wordlist[i];
                wordlist[i] = str.ToLower();
	    }

            do
            {
                random_word_index = rnd.Next(wordsCount);
                secret_word = wordlist[random_word_index];                              // Fetch the secret word for the list of wordlist
                secret_word_length = secret_word.Length;

                if (guessed_word==null || guessed_word.Length != secret_word_length)
                    guessed_word = new char[secret_word_length];                        // only create a new array when the new array is of a different size than the old one

                for (i = 0; i < secret_word_length; i++)                                // Fill guessed_word array with the character '_'
                {
                    guessed_word[i] = '_';
                }

                guessed_right_letters = 0;
                incorrectly_guessed_letters.Clear();
                won = false;
                for (guesses_left = numberOfGuesses; guesses_left > 0 && won==false; guesses_left--)
                {
                    PrintHangmanInfo();

                    Console.Write("Press a key (Press 1 to guess the whole word, other keys to guess individual letter(s)) ");
                    key_pressed = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    if (key_pressed == '1')
                    {
                        str = PrintStringAndRequestStringFromUser($"\nGuess the whole word ({secret_word_length} letters). Enter a word:");
                        
                        if (String.Compare(str,secret_word,true) == 0)                  // Perform a case insensitive comparison
                        {
                            Console.Write("\nCorrect!");
                            secret_word.CopyTo(0, guessed_word, 0, secret_word_length); // Copy the secret word to guessed_word
                            won = true;
                        } else
                            Console.Write("\nWrong!");
 
                        Console.Write(" Press any key to continue..");
                        Console.ReadKey();

                        continue;                                                       // skip the rest of the code in the for loop
                    }
                    key_pressed_lowercase = ConvertCharToLowercase(key_pressed);

                    if (key_pressed_lowercase < 'a' || key_pressed_lowercase > 'z' && key_pressed_lowercase != 'ö' && key_pressed_lowercase != 'ä' &&
                         key_pressed_lowercase != 'å')
                    {                                                                   // User pressed a non-letter key
                        guesses_left++;                                                 // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        continue;                                                       // skip the rest of the code in the for loop
                    }

                    letter_found = false;
                    letter_already_found = false;
                    for (i = 0; i < secret_word_length; i++)                            // Scan through the secret word looking for the pressed key
                    {
                        if (secret_word[i] == key_pressed_lowercase)
                        {                                                               // letter was found in secret_word
                            if (guessed_word[i] == '_')
                            {                                                           // User hasn't guessed the letter before,
                                guessed_word[i] = key_pressed_lowercase;
                                guessed_right_letters++;
                            }
                            else
                                letter_already_found = true;
                            letter_found = true;
                        }
                    }

                    if (letter_already_found)
                    {
                        guesses_left++;                                                 // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        continue;                                                       // skip the rest of the code in the for loop
                    }

                    if (letter_found)
                    {                                                                   // letter was found in secret_word
                        if (guessed_right_letters >= secret_word_length)
                        {                                                               // The user has guessed the whole word
                            won = true;
                        }
                    }
                    else
                    {                                                                   // letter was not found in secret_word
                        if (incorrectly_guessed_letters.ToString().IndexOf(key_pressed_lowercase) >= 0)
                        {                                                               // User has guessed the letter before
                            guesses_left++;                                             // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        }
                        else
                        {                                                               // User hasn't guessed the letter before,
                            incorrectly_guessed_letters.Append(key_pressed_lowercase);  // add it to the list
                        }
                    }
                }

                PrintHangmanInfo();
                Console.WriteLine("");

                if (won)
                {
                    Console.WriteLine("You won! You guessed the word ({0}) in {1} tries", secret_word, numberOfGuesses - guesses_left);
                }
                else
                {
                    Console.WriteLine("You lost!");
                }

                Console.WriteLine("");
                do
                {
                    str = PrintStringAndRequestStringFromUser("Run again? (y/n)").ToLower();

                } while (str != "y" && str != "n");

                if (str == "n")
                {
                    exit = true;
                }
            } while (!exit);
        }

        static void PrintHangmanInfo()
        {
            Console.Clear();
            Console.WriteLine("Hangman\n\nUsing {0} word list of {1} words\n",external_wordlist ? "an external" : "a",wordsCount);

            Console.Write("Word to guess ({0} letters): ",secret_word_length);

            for (int i = 0; i < secret_word_length; i++)                         // Print guessed word
            {
                Console.Write(" {0}", guessed_word[i]);
            }
            Console.WriteLine("\n\nGuesses left: {0}\nIncorrectly guessed letters: {1}", guesses_left, incorrectly_guessed_letters);

        }

/*
    * Function:    ConvertCharToLowercase
    * 
    * Converts aa character to a lowercase character
    * 
    * returns:    The converted character
*/
        static char ConvertCharToLowercase(char AChar)
        {
            if (AChar >= 'A' && AChar <= 'Z')
            {
                AChar -= 'A';
                AChar += 'a';
            }
            else
            if (AChar == 'Ö')
            {
                AChar = 'ö';
            }
            else
            if (AChar == 'Å')
            {
                AChar = 'Å';
            }
            else
            if (AChar == 'Ä')
            {
                AChar = 'ä';
            }

            return AChar;
        }

/*
    * Function:    PrintStringAndRequestStringFromUser
    * 
    * Outputs a title text specified by DisplayText in the console and records the users keypresses until return key is pressed
    * 
    * returns:    The recorded text string
*/
        static string PrintStringAndRequestStringFromUser(string DisplayText)
        {
            Console.Write("{0} ", DisplayText);
            return Console.ReadLine();
        }

    }
}
