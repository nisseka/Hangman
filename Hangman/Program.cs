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
        static int secretWordLength;
        static char[] guessedWord;
        static int guessesLeft;
        static StringBuilder incorrectlyGuessedLetters;
        static bool externalWordlist;
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
            int randomWordIndex;
            int i;
            int guessedRightLetters;
            int numberOfGuesses = 10;
            string secretWord, str;
            char keyPressed;
            char keyPressedLowercase;
            bool exit = false;
            bool won;
            bool letterFound;
            bool letterAlreadyFound;
            string wordlistFilename;
            StringBuilder sbWordlistFilename = new StringBuilder(Directory.GetCurrentDirectory());   // Get the full path of the program executable file

            i = sbWordlistFilename.ToString().IndexOf("bin");                                        // Locate where the bin folder is in the path
            if (i >= 0)
            {
                sbWordlistFilename.Remove(i, sbWordlistFilename.Length - i);                         // Delete everything starting from the bin folder position to the end of the string
            } else
                sbWordlistFilename.Append('\\');                                                     // bin folder not found in the path; use the program file path

            sbWordlistFilename.Append("wordlist.txt");                                               // We now have a path to the word list file which should be located in the project folder            
            wordlistFilename = sbWordlistFilename.ToString();

            externalWordlist = false;

            if (File.Exists(wordlistFilename))
            {
                string[] fileContent = File.ReadAllLines(wordlistFilename);

                if (fileContent.Length >= 5)                                                          // Make sure that the wordlist file has atleast 5 words in it
                {                                                                                     // otherwise use the internal wordlist 
                    wordlist = fileContent;
                    externalWordlist = true;
                }
            }

            incorrectlyGuessedLetters = new StringBuilder();
            wordsCount = wordlist.Length;

	    for (i = 0; i < wordsCount; i++)                                                           // Convert all the words to lowercase
	    {
                str = wordlist[i];
                wordlist[i] = str.ToLower();
	    }

            do
            {
                randomWordIndex = rnd.Next(wordsCount);
                secretWord = wordlist[randomWordIndex];                                                 // Fetch the secret word for the list of wordlist
                secretWordLength = secretWord.Length;

                if (guessedWord==null || guessedWord.Length != secretWordLength)
                    guessedWord = new char[secretWordLength];                                           // only create a new array when a different size than the old one is needed

                for (i = 0; i < secretWordLength; i++)                                                  // Fill guessedWord array with the character '_'
                {
                    guessedWord[i] = '_';
                }

                guessedRightLetters = 0;
                incorrectlyGuessedLetters.Clear();
                won = false;
                for (guessesLeft = numberOfGuesses; guessesLeft > 0 && won==false; guessesLeft--)
                {
                    PrintHangmanInfo();

                    Console.Write("Press a key (Press 1 to guess the whole word, other keys to guess individual letter(s)) ");
                    keyPressed = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    if (keyPressed == '1')
                    {
                        str = PrintStringAndRequestStringFromUser($"\nGuess the whole word ({secretWordLength} letters). Enter a word:");
                        
                        if (String.Compare(str,secretWord,true) == 0)                   // Perform a case-insensitive comparison
                        {
                            Console.Write("\nCorrect!");
                            secretWord.CopyTo(0, guessedWord, 0, secretWordLength);     // Copy the secret word to guessed_word
                            won = true;
                        } else
                            Console.Write("\nWrong!");
 
                        Console.Write(" Press any key to continue..");
                        Console.ReadKey();

                        continue;                                                       // skip the rest of the code in the for loop
                    }
                    keyPressedLowercase = ConvertCharToLowercase(keyPressed);

                    if (keyPressedLowercase < 'a' || keyPressedLowercase > 'z' && keyPressedLowercase != 'ö' && keyPressedLowercase != 'ä' &&
                         keyPressedLowercase != 'å')
                    {                                                                   // User pressed a non-letter key
                        guessesLeft++;                                                  // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        continue;                                                       // skip the rest of the code in the for loop
                    }

                    letterFound = false;
                    letterAlreadyFound = false;
                    for (i = 0; i < secretWordLength; i++)                              // Scan through the secret word looking for the pressed key
                    {
                        if (secretWord[i] == keyPressedLowercase)
                        {                                                               // letter was found in secretWord
                            if (guessedWord[i] == '_')
                            {                                                           // User hasn't guessed the letter before
                                guessedWord[i] = keyPressedLowercase;
                                guessedRightLetters++;
                            }
                            else
                                letterAlreadyFound = true;
                            letterFound = true;
                        }
                    }

                    if (letterAlreadyFound)
                    {
                        guessesLeft++;                                                  // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        continue;                                                       // skip the rest of the code in the for loop
                    }

                    if (letterFound)
                    {                                                                   // letter was found in secretWord
                        if (guessedRightLetters >= secretWordLength)
                        {                                                               // The user has guessed the whole word
                            won = true;
                        }
                    }
                    else
                    {                                                                   // letter was not found in secretWord
                        if (incorrectlyGuessedLetters.ToString().IndexOf(keyPressedLowercase) >= 0)
                        {                                                               // User has guessed the letter before
                            guessesLeft++;                                              // don't consume a guess; increase guesses_left with 1 since the for loop will decrease it with 1
                        }
                        else
                        {                                                               // User hasn't guessed the letter before,
                            incorrectlyGuessedLetters.Append(keyPressedLowercase);      // add it to the list
                        }
                    }
                }

                PrintHangmanInfo();
                Console.WriteLine("");

                if (won)
                {
                    Console.WriteLine("You won! You guessed the word ({0}) in {1} tries", secretWord, numberOfGuesses - guessesLeft);
                } else
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
            Console.WriteLine("Hangman\n\nUsing {0} word list of {1} words\n",externalWordlist ? "an external" : "a",wordsCount);

            Console.Write("Word to guess ({0} letters): ",secretWordLength);

            for (int i = 0; i < secretWordLength; i++)                         // Print guessed word
            {
                Console.Write(" {0}", guessedWord[i]);
            }
            Console.WriteLine("\n\nGuesses left: {0}\nIncorrectly guessed letters: {1}", guessesLeft, incorrectlyGuessedLetters);

        }

/*
    * Function:    ConvertCharToLowercase
    * 
    * Converts a character to a lowercase character
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
