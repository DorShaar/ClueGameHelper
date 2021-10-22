using System;
using System.Collections.Generic;
using System.Text;

namespace ClueGameHelper
{
    class ClueGameHelper
    {
        private readonly List<string> mPeopleSuspects = new List<string>
        {
            "Miss Scarlett",
            "Orchid",
            "Peach",
            "Mustard",
            "Peacock",
            "White"
        };
        private readonly List<string> mWeaponSuspects = new List<string>
        {
            "Rope",
            "Pipe",
            "Dagger",
            "Candlestick",
            "Wrench",
            "Pistol"
        };
        private readonly List<string> mRoomSuspects = new List<string>
        {
            "Ballroom",
            "Billiard Room",
            "Conservatory",
            "Dining Room",
            "Hall",
            "Kitchen",
            "Library",
            "Lounge",
            "Study",
        };

        private List<string> mOpponentsPlayers = new List<string>();
        private KnowledgeTable mKnowledgeTable;

        private bool IsClueCardValid(string clueCard)
        {
            bool isClueCardValid = false;

            if (mPeopleSuspects.Contains(clueCard) ||
               mWeaponSuspects.Contains(clueCard) ||
               mRoomSuspects.Contains(clueCard))
            {
                isClueCardValid = true;
            }

            if (!isClueCardValid)
            {
                Console.WriteLine($"{clueCard} is not valid people, weapon or room");
            }

            return isClueCardValid;
        }

        private void GetStarterClueCards()
        {
            Console.WriteLine("What cards did you get? Write 'end' when finished.");

            string clueCard = string.Empty;
            while (clueCard.ToLower() != "end")
            {
                clueCard = Console.ReadLine();
                if (clueCard.ToLower() != "end")
                {
                    if (IsClueCardValid(clueCard))
                    {
                        mKnowledgeTable.UpdateChanceToAllOpponent(clueCard, 'X');
                    }
                }
            }
        }

        private void InitKnowledgeTable()
        {
            mKnowledgeTable = new KnowledgeTable(
                mOpponentsPlayers,
                mPeopleSuspects,
                mWeaponSuspects,
                mRoomSuspects);

            foreach (string opponent in mOpponentsPlayers)
            {
                Dictionary<string, char> clueCardAndChanceDict = new Dictionary<string, char>();
                //int chanceToHoldCard = (int)((1d / (double)(mOpponentsPlayers.Count + 1)) * 100);
                char chanceToHoldCard = '?';

                GeneralUtilities.AddKeyValueIntoDictFromCollection(
                    clueCardAndChanceDict, chanceToHoldCard, mPeopleSuspects);

                GeneralUtilities.AddKeyValueIntoDictFromCollection(
                    clueCardAndChanceDict, chanceToHoldCard, mWeaponSuspects);

                GeneralUtilities.AddKeyValueIntoDictFromCollection(
                    clueCardAndChanceDict, chanceToHoldCard, mRoomSuspects);

                mKnowledgeTable.AddOpponentDictionary(opponent, clueCardAndChanceDict);
            }
        }

        private void PrintMenu()
        {
            Console.WriteLine(
@"
1. Update clue card to opponent player
2. Question-raised turn
3. Print knowledge table
4. 'end' to exit game");
        }

        private void UpdateClueCardToOpponentPlayer()
        {
            Console.Write("Type player to update: ");
            string opponentName = Console.ReadLine();
            if (GeneralUtilities.IsElementValid(opponentName, mOpponentsPlayers))
            {
                Console.Write($"Type the clue card {opponentName} has: ");
                string clueCard = Console.ReadLine();
                if (IsClueCardValid(clueCard))
                {
                    mKnowledgeTable.UpdateClueCardToOpponentPlayer(opponentName, clueCard);
                }
            }
        }

        private void GetConclusionFromOpponentResponse(string opponentResponseName,
            string peopleSuspect, string weaponSuspect, string roomSuspect)
        {
            Console.WriteLine($@"Choose option:
1. {opponentResponseName} did not revealed nothing
2. {opponentResponseName} revealed one card");

            string input = Console.ReadLine();
            if (input.Equals("1"))
            {
                mKnowledgeTable.UpdateSuspectsAsNonExist(
                opponentResponseName,
                peopleSuspect,
                weaponSuspect,
                roomSuspect);
            }
            else if (input.Equals("2"))
            {
                mKnowledgeTable.UpdateOpponentRevealedClueCard(
                opponentResponseName,
                peopleSuspect,
                weaponSuspect,
                roomSuspect);
            }
            else if (input.ToLower().Equals("end"))
            {
                Console.WriteLine("Invalid input");
            }
        }

        private void GetConclusionFromGuess()
        {
            //Console.Write("Type who made the guess: ");
            //string opponentName = Console.ReadLine();
            //if (GeneralUtilities.IsElementValid(opponentName, mOpponentsPlayers))
            //{
            Console.Write("Type people suspect guess: ");
            string peopleSuspect = Console.ReadLine();
            if (GeneralUtilities.IsElementValid(peopleSuspect, mPeopleSuspects))
            {
                Console.Write("Type weapon suspect guess: ");
                string weaponSuspect = Console.ReadLine();
                if (GeneralUtilities.IsElementValid(weaponSuspect, mWeaponSuspects))
                {
                    Console.Write("Type room suspect guess: ");
                    string roomSuspect = Console.ReadLine();
                    if (GeneralUtilities.IsElementValid(roomSuspect, mRoomSuspects))
                    {
                        string opponentResponseName = string.Empty;
                        while (!opponentResponseName.Equals("end"))
                        {
                            Console.Write($"Type opponent to response to: ({peopleSuspect}, {weaponSuspect}, {roomSuspect})");
                            opponentResponseName = Console.ReadLine();
                            if (GeneralUtilities.IsElementValid(
                                opponentResponseName,
                                mOpponentsPlayers))
                            {
                                GetConclusionFromOpponentResponse(
                                    opponentResponseName,
                                    peopleSuspect,
                                    weaponSuspect,
                                    roomSuspect);
                            }
                        }
                    }
                }
            }
            //}
        }

        private void RunGameRounds()
        {
            string input = string.Empty;
            while (input.ToLower() != "end")
            {
                mKnowledgeTable.UpdateKnownCards();
                PrintMenu();
                input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    UpdateClueCardToOpponentPlayer();
                }
                else if (input.Equals("2"))
                {
                    GetConclusionFromGuess();
                }
                else if (input.Equals("3"))
                {
                    mKnowledgeTable.PrintKnowledgeTable();
                }
                else if (!input.ToLower().Equals("end"))
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }

        public void RegisterPlayers()
        {
            Console.WriteLine(
                @"Please Write the names of the other players. Write 'end' when finished");
            string playerName = string.Empty;
            while (playerName.ToLower() != "end")
            {
                playerName = Console.ReadLine();
                if (playerName.ToLower() != "end")
                {
                    if (!mOpponentsPlayers.Contains(playerName))
                    {
                        mOpponentsPlayers.Add(playerName);
                    }
                    else
                    {
                        Console.WriteLine($"{playerName} is already exist");
                    }
                }
            }

            Console.Write("The players you entered are:");
            foreach (string name in mOpponentsPlayers)
            {
                Console.Write($"{name}, ");
            }

            Console.Write(Environment.NewLine);
            InitKnowledgeTable();
        }

        public void RunGame()
        {
            mKnowledgeTable.PrintKnowledgeTable();
            GetStarterClueCards();
            mKnowledgeTable.PrintKnowledgeTable();

            RunGameRounds();
            Console.WriteLine();
            Console.WriteLine("Bye bye :)");
        }
    }
}
