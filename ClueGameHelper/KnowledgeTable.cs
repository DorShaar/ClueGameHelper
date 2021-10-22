using System;
using System.Collections.Generic;
using System.Text;

namespace ClueGameHelper
{
    class KnowledgeTable
    {
        // Dictionary<playerName, Dictionary<clueCard, chanceToHave>>
        private Dictionary<string, Dictionary<string, char>> mKnowledgeTable
             = new Dictionary<string, Dictionary<string, char>>();

        // Dictionary<playerName, List<List<clueCards>>.
        // Clue card counts as possibility
        // <=>
        // Opponent chance to have clue card is not 0
        // AND
        // Every opponent does not have clue card chanve 100.
        private Dictionary<string, List<List<string>>> mPossibleClueCardsWeakGroup
            = new Dictionary<string, List<List<string>>>();

        // Can have only groups of 2 or 3 clue cards.
        private Dictionary<string, List<List<string>>> mPossibleClueCardsStrongGroup
            = new Dictionary<string, List<List<string>>>();

        private readonly List<string> mOpponentPlayers,mPeopleSuspects, mWeaponSuspects, mRoomSuspects,
                                      mAllClueCardsList;

        private string mKnownPeople, mKnownWeapon ,mKnownRoom;

        private readonly string mPlayerNameSpace;
        private readonly string mClueCardSpace;
        private int mMaxSuspectNameLength;

        public KnowledgeTable(List<string> opponentPlayers, List<string> peopleSuspects,
                              List<string> weaponSuspects, List<string> roomSuspects)
        {
            mOpponentPlayers = opponentPlayers;
            mPeopleSuspects = peopleSuspects;
            mWeaponSuspects = weaponSuspects;
            mRoomSuspects = roomSuspects;

            mPlayerNameSpace = BuildPlayerNameSpace(opponentPlayers);
            mClueCardSpace = BuildClueCardSpace(peopleSuspects, weaponSuspects, roomSuspects);

            mAllClueCardsList = new List<string>();
            mAllClueCardsList.AddRange(peopleSuspects);
            mAllClueCardsList.AddRange(weaponSuspects);
            mAllClueCardsList.AddRange(roomSuspects);
        }

        private int GetMaxClueCardLength(List<string> peopleSuspects,
                                         List<string> weaponSuspects, List<string> roomSuspects)
        {
            int maxClueCardLength = 0;

            foreach (string people in peopleSuspects)
            {
                if (maxClueCardLength < people.Length)
                {
                    maxClueCardLength = people.Length;
                }
            }

            foreach (string weapon in weaponSuspects)
            {
                if (maxClueCardLength < weapon.Length)
                {
                    maxClueCardLength = weapon.Length;
                }
            }

            foreach (string room in roomSuspects)
            {
                if (maxClueCardLength < room.Length)
                {
                    maxClueCardLength = room.Length;
                }
            }

            return maxClueCardLength;
        }

        private int GetMaxPlayerNameLength(List<string> opponentPlayers)
        {
            int maxNameLength = 0;

            foreach (string player in opponentPlayers)
            {
                if (maxNameLength < player.Length)
                {
                    maxNameLength = player.Length;
                }
            }

            return maxNameLength;
        }

        private string BuildSpace(int spaceLength)
        {
            StringBuilder space = new StringBuilder();
            for (int i = 0; i < spaceLength; ++i)
            {
                space.Append(" ");
            }

            return space.ToString();
        }

        private string BuildSpace(int lengthAllreadyHave, int lengthShouldStart)
        {
            int spaceLength = lengthShouldStart - lengthAllreadyHave;
            return BuildSpace(spaceLength);
        }

        private string BuildClueCardSpace(List<string> peopleSuspects,
                                              List<string> weaponSuspects, List<string> roomSuspects)
        {
            mMaxSuspectNameLength = GetMaxClueCardLength(peopleSuspects, weaponSuspects, roomSuspects);
            StringBuilder space = new StringBuilder();
            for (int i = 0; i < mMaxSuspectNameLength; ++i)
            {
                space.Append(" ");
            }

            return space.ToString();
        }

        private string BuildPlayerNameSpace(List<string> opponentPlayers)
        {
            int maxNameLength = GetMaxPlayerNameLength(opponentPlayers);
            StringBuilder space = new StringBuilder();
            for (int i = 0; i < maxNameLength; ++i)
            {
                space.Append(" ");
            }

            return space.ToString();
        }

        private void PrintOpponentPlayersRow()
        {
            Console.Write($"  {BuildSpace(mMaxSuspectNameLength)}");
            foreach (string opponent in mOpponentPlayers)
            {
                Console.Write($"{opponent}{mPlayerNameSpace}");
            }

            Console.WriteLine();
        }

        private void PrintClueCardListInfo(List<string> dictionaryInfo)
        {
            foreach (string element in dictionaryInfo)
            {
                Console.Write($"{element}: ");
                string opponent = mOpponentPlayers[0];
                char chanceToHave = (mKnowledgeTable[opponent])[element];
                string space = BuildSpace(element.Length + 1, mMaxSuspectNameLength);
                Console.Write($"  {space}{chanceToHave}{mPlayerNameSpace}");

                for(int i = 1; i < mOpponentPlayers.Count; ++i)
                {
                    opponent = mOpponentPlayers[i];
                    chanceToHave = (mKnowledgeTable[opponent])[element];
                    space = BuildSpace(element.Length + 1, mMaxSuspectNameLength);
                    Console.Write($"{chanceToHave}{mPlayerNameSpace}");
                }

                Console.WriteLine();
            }
        }

        private void PrintPossibilities()
        {
            Console.WriteLine("Strong Possibilities:");
            InnerPrintStrongPossibilities(mPossibleClueCardsStrongGroup);

            Console.WriteLine();

            Console.WriteLine("Weak Possibilities:");
            InnerPrintStrongPossibilities(mPossibleClueCardsWeakGroup);
        }

        private void PrintKnownClueCards()
        {
            Console.WriteLine("Known clue cards:");
            foreach(string clueCard in mAllClueCardsList)
            {
                foreach (string opponentPlayers in mKnowledgeTable.Keys)
                {
                    if((mKnowledgeTable[opponentPlayers])[clueCard] == 'V')
                    {
                        Console.WriteLine($"{clueCard}: {opponentPlayers}");
                    }
                    else
                    {
                        Console.WriteLine($"{clueCard}: {string.Empty}");
                    }
                }
            }
        }

        private void InnerPrintStrongPossibilities(
                Dictionary<string, List<List<string>>> possibilitiesGroups)
        {
            foreach (string person in possibilitiesGroups.Keys)
            {
                Console.Write($"{person}: ");
                foreach (List<string> possibilitiesGroup in possibilitiesGroups[person])
                {
                    Console.Write("<");
                    foreach (string suspect in possibilitiesGroup)
                    {
                        Console.Write($"{suspect}, ");
                    }

                    Console.Write(">, ");
                }

                Console.WriteLine();
            }
        }

        private bool IsOpponentKnownToPossesClueCard(string opponent, string suspect)
        {
            return mKnowledgeTable[opponent][suspect] == 'V';
        }

        private bool IsSomeOpponentKnownToPossesClueCard(string suspect)
        {
            bool isClueCardKnownToBePossesed = false;

            foreach(string opponent in mKnowledgeTable.Keys)
            {
                if (IsOpponentKnownToPossesClueCard(opponent, suspect))
                {
                    isClueCardKnownToBePossesed = true;
                    break;
                }
            }

            return isClueCardKnownToBePossesed;
        }

        private bool ShouldAddSuspectAsPossibility(string opponentResponseName, string suspect)
        {
            return (mKnowledgeTable[opponentResponseName][suspect] != 'X') &&
                    !IsSomeOpponentKnownToPossesClueCard(suspect);
        }

        private void RemoveSuspectFromPossibleClueCardsWeakGroupsByOpponent(string opponent,
                                                                          string suspect)
        {
            if (mPossibleClueCardsWeakGroup.ContainsKey(opponent))
            {
                for (int i = 0; i < mPossibleClueCardsWeakGroup[opponent].Count;)
                {
                    List<string> weakPossibilitiesGroup =
                            mPossibleClueCardsWeakGroup[opponent][i];
                    weakPossibilitiesGroup.Remove(suspect);

                    // Updating in case no options left and removing the list.
                    if (weakPossibilitiesGroup.Count == 0)
                    {
                        mPossibleClueCardsWeakGroup[opponent].RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
        }

        private void UpdatePossibleClueCardsWeakGroup(List<string> clueCardsHoldPossibilities,
                                                      string opponent)
        {
            if (!mPossibleClueCardsWeakGroup.ContainsKey(opponent))
            {
                mPossibleClueCardsWeakGroup.Add(
                    opponent,
                    new List<List<string>>
                    {
                            clueCardsHoldPossibilities
                    });
            }
            else
            {
                (mPossibleClueCardsWeakGroup[opponent]).Add(clueCardsHoldPossibilities);
            }
        }

        private void RemoveSuspectFromPossibleClueCardsStrongGroupsByOpponent(string opponent, 
                                                                            string suspect)
        {
            if(mPossibleClueCardsStrongGroup.ContainsKey(opponent))
            {
                for (int i = 0; i < mPossibleClueCardsStrongGroup[opponent].Count;)
                {
                    List<string> strongPossibilitiesGroup =
                        mPossibleClueCardsStrongGroup[opponent][i];
                    strongPossibilitiesGroup.Remove(suspect);

                    // Updating in case only one option left and removing the list.
                    if (strongPossibilitiesGroup.Count == 1)
                    {
                        UpdateClueCardToOpponentPlayer(opponent, strongPossibilitiesGroup[0]);
                        mPossibleClueCardsStrongGroup[opponent].RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
        }

        private void RemoveSuspectFromAllPossibleClueCardsGroups(string knownClueCard)
        {
            foreach (string opponent in mKnowledgeTable.Keys)
            {
                RemoveSuspectFromPossibleClueCardsStrongGroupsByOpponent(opponent, knownClueCard);
                RemoveSuspectFromPossibleClueCardsWeakGroupsByOpponent(opponent, knownClueCard);
            }
        }

        private void UpdatePossibleClueCardsStrongGroup(List<string> clueCardsHoldPossibilities,
                                                        string opponentResponse)
        {
            if (clueCardsHoldPossibilities.Count == 1)
            {
                string knownClueCard = clueCardsHoldPossibilities[0];
                UpdateClueCardToOpponentPlayer(opponentResponse, knownClueCard);
            }
            else // Count is 2 or 3.
            {
                if(!mPossibleClueCardsStrongGroup.ContainsKey(opponentResponse))
                {
                    mPossibleClueCardsStrongGroup.Add(
                        opponentResponse,
                        new List<List<string>>
                        {
                            clueCardsHoldPossibilities
                        });
                }
                else
                {
                    (mPossibleClueCardsStrongGroup[opponentResponse]).Add(clueCardsHoldPossibilities);
                }
            }
        }

        public void AddOpponentDictionary(string opponent,
                                          Dictionary<string, char> clueCardAndChanceDict)
        {
            mKnowledgeTable.Add(opponent, clueCardAndChanceDict);
        }

        public void UpdateChanceToAllOpponent(string clueCard, char chance)
        {
            foreach (string opponent in mOpponentPlayers)
            {
                (mKnowledgeTable[opponent])[clueCard] = chance;
            }
        }

        public void UpdateClueCardToOpponentPlayer(string opponent, string clueCard)
        {
            (mKnowledgeTable[opponent])[clueCard] = 'V';
            foreach (string otherOpponent in mOpponentPlayers)
            {
                if (otherOpponent != opponent)
                {
                    (mKnowledgeTable[otherOpponent])[clueCard] = 'X';
                }
            }

            RemoveSuspectFromAllPossibleClueCardsGroups(clueCard);
        }

        public void UpdateSuspectsAsNonExist(string opponentResponseName,
            string peopleSuspect, string weaponSuspect, string roomSuspect)
        {
            (mKnowledgeTable[opponentResponseName])[peopleSuspect] = 'X';
            (mKnowledgeTable[opponentResponseName])[weaponSuspect] = 'X';
            (mKnowledgeTable[opponentResponseName])[roomSuspect] = 'X';

            RemoveSuspectFromPossibleClueCardsStrongGroupsByOpponent(
                opponentResponseName, peopleSuspect);
            RemoveSuspectFromPossibleClueCardsStrongGroupsByOpponent(
                opponentResponseName, weaponSuspect);
            RemoveSuspectFromPossibleClueCardsStrongGroupsByOpponent(
                opponentResponseName, roomSuspect);

            RemoveSuspectFromPossibleClueCardsWeakGroupsByOpponent(
                opponentResponseName, peopleSuspect);
            RemoveSuspectFromPossibleClueCardsWeakGroupsByOpponent(
                opponentResponseName, weaponSuspect);
            RemoveSuspectFromPossibleClueCardsWeakGroupsByOpponent(
                opponentResponseName, roomSuspect);
        }

        /// <summary>
        /// Adding clue card as possibiliy in case:
        /// 1. Clue card is not known as related to every opponent player.
        /// 2. Clue card holding chance is not 0 at the specific opponent player.
        /// </summary>
        /// <param name="opponentResponseName"></param>
        /// <param name="peopleSuspect"></param>
        /// <param name="weaponSuspect"></param>
        /// <param name="roomSuspect"></param>
        public void UpdateOpponentRevealedClueCard(string opponentResponseName,
            string peopleSuspect, string weaponSuspect, string roomSuspect)
        {
            bool isOpponentPossesPeopleSuspect =
                IsOpponentKnownToPossesClueCard(opponentResponseName, peopleSuspect);

            bool isOpponentPossesWeaponSuspect =
                IsOpponentKnownToPossesClueCard(opponentResponseName, weaponSuspect);

            bool isOpponentPossesRoomSuspect =
                IsOpponentKnownToPossesClueCard(opponentResponseName, roomSuspect);

            List<string> clueCardsHoldPossibilities = new List<string>();
            if (ShouldAddSuspectAsPossibility(opponentResponseName, peopleSuspect))
            {
                clueCardsHoldPossibilities.Add(peopleSuspect);
            }

            if (ShouldAddSuspectAsPossibility(opponentResponseName, weaponSuspect))
            {
                clueCardsHoldPossibilities.Add(weaponSuspect);
            }

            if (ShouldAddSuspectAsPossibility(opponentResponseName, roomSuspect))
            {
                clueCardsHoldPossibilities.Add(roomSuspect);
            }

            if (clueCardsHoldPossibilities.Count == 0) { return; }

            // Strong deduce situation.
            if (!isOpponentPossesPeopleSuspect &&
                !isOpponentPossesWeaponSuspect &&
                !isOpponentPossesRoomSuspect)
            {
                UpdatePossibleClueCardsStrongGroup(clueCardsHoldPossibilities, opponentResponseName);
            }
            else // Weak deduce situation.
            {
                UpdatePossibleClueCardsWeakGroup(clueCardsHoldPossibilities, opponentResponseName);
            }
        }

        public void PrintKnowledgeTable()
        {
            PrintOpponentPlayersRow();
            PrintClueCardListInfo(mPeopleSuspects);
            PrintClueCardListInfo(mWeaponSuspects);
            PrintClueCardListInfo(mRoomSuspects);

            Console.WriteLine();
            PrintPossibilities();

            Console.WriteLine();
            Console.WriteLine($"Known Murderer: {mKnownPeople}");
            Console.WriteLine($"Known Weapon: {mKnownWeapon}");
            Console.WriteLine($"Known Room: {mKnownRoom}");
        }

        public void UpdateKnownCards()
        {
            //foreach()
        }
    }
}
