using Assets;
using KModkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class presidentialElectionsScript : MonoBehaviour
{

    // Standard module variables
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMAudio Audio;
    public KMRuleSeedable ruleSeed;

    public MeshRenderer[] btnRenderers, ledRenderers;
    public Material[] colorMats;
    public Material correctMat, wrongMat, blackMat;
    public SpriteRenderer[] spriteRenderers;
    public GameObject[] spriteObjects;
    public GameObject votedSticker;
    public Sprite[] sprites;
    public TextMesh screenText;
    public KMSelectable[] btnSelectables;
    public KMHighlightable[] btnHighlights;

    static int moduleIdCounter = 1;
    int moduleId;
    bool solved = false;
    bool animationPlaying = false;

    // Full array variables
    static readonly string[] baseColorNames = { "red", "green", "blue", "yellow", "magenta", "cyan", "orange", "purple", "brown", "crimson", "forest", "navy", "black", "gray", "white" };
    static readonly string[] basePartyNames = { "Slowpoke", "Mischief", "Conspiracy", "Trivia Murder", "Rent Is Too Damn Low", "Experimental", "Quack Quack", "Birthday", "Carcinization", "Vine Boom", "Android", "Toxicity", "Little Guy", "Vote For This", "Aaaaaaaah", "Catpeople", "Perfect Pitch", "Follow Your", "Endangered", "Arson", "Reptilian", "Party", "It's So Over", "Dogpeople", "Super Mario" };
    static readonly string[] candidateNames = {
        "YOUR MOM", "DICK KICKEM", "GRUNKLE SQUEAKY", "YABBAGUY", "DEAF", "EPICTOAST", "IVAN IVANSKY IVANOVICH", "SHELDON COOPER", "MR BILL CLINTON SEX SCANDAL", "VERMIN SUPREME", "DEEZ NUTS",
        "YOUR DAD", "OMEGA", "COLORS", "VOID", "LUNA", "MADDYMOOS", "MCD", "CRAZYCALEB", "LIL UZI VERT", "BOB", "SIMON", "TWITCH PLAYS HIVEMIND", "TASHA", "JEB", "LOGBOT", "KANYE WEST", "JEAVER",
        "KONNOR", "MR PEANUT", "BLAN", "JENSON", "VANILLA", "CHOCOLA", "MARGARET THATCHER", "KONOKO", "THE DEMOGORGON", "BABE RUTH", "KEVIN LEE", "THE E PAWN", "MARCUS STUYVESANT", "KOOPA TROOPA",
        "HONG JIN-HO", "KAZEYOSHI IMAI", "DON CHEADLE", "MILLIE ROSE", "WARIO", "DEPRESSO", "GORDON FREEMAN", "NEIL CICIEREGA", "THE SHELLED ONE", "ANYONE BUT DEAF", "DART MONKEY", "SANTA CLAUS", "CTHULHU",
        "ONE BILLION LIONS","QUIZZINGTON J PUZZLE","WINSTON SMITH","ABBIE MINDWAVE","AXOCAT","KEANU REEVES","SHADOW THE HEDGEHOG","A BOILED EGG","WALTER WHITE","SPROUT SEEDLY","LADY GAGA","DR EVELYN LASAGNA",
        "WORM ON A STRING","PEKO PEKOYAMA","KIRUMI TOJO","KSI","GRIM REAPER","TETRIMIDION","KASANE TETO","OBAMNA","MR POTATO HEAD","YOU","TIMOTHEE CHALAMET","SADDAM HUSSEIN","A CUTE SYLVEON","RIKA","PLAYBOI CARTI",
        "KING VON","JOSEPH R. BIDEN JR","YOUR OVERLORD","NO ONE","ED BALLS","ALAN SMITHEE","DAVID","JACOB COLLIER","A BUNCH OF BALLS","BILL WURTZ","THE JOLLIEST RANCHER","PETER GRIFFIN",
    };
    static readonly string[] spacelessCandidateNames = candidateNames.Raw();

    static readonly int[] baseColorNumbers = { 12, 5, 13, 3, 7, 4, 8, 10, 14, 15, 2, 6, 1, 9, 11 };
    static readonly int[] basePartyNumbers = { 7, 15, 4, 2, 9, 8, 12, 13, 6, 11, 3, 16, 1, 5, 14, 10 };

    //Array variables that will be used for this module
    string[] colorNames;
    string[] partyNames;
    int[] colorNumbers;
    int[] partyNumbers;

    // Candidate variables
    int[] colors;
    int[] parties;
    int[] candidates;

    // Voting variables
    int votingMethod = 0;
    VotingMethod[] votingMethodNames;
    EdgeworkCalculation[] edgeworkCalculations;

    int[] sortingMethods = { 99, 99, 99, 99, 99, 99 };
    SortingMethod[] sortingMethodNames;

    int[][] votes =
    {
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
    };
    readonly int[] candidateScores = { 0, 0, 0, 0 };
    readonly bool[] pressedButtons = { false, false, false, false };
    readonly int[] candidatePlacement = { 0, 0, 0, 0 };
    int nextPlace = 0;

    // Other variables
    //                                       A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q,  R, S, T, U, V, W, X, Y, Z
    static readonly int[] scrabbleScores = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };
    static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    static readonly int[] numbersAlphabeticalOrder = { 19, 9, 18, 16, 5, 4, 12, 10, 0, 7, 14, 2, 17, 15, 6, 3, 13, 11, 1, 8 };
    byte whiteness = 255;

    private void Awake()
    {
        moduleId = moduleIdCounter++;
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            btnSelectables[i].OnInteract += delegate () { if (!solved) { PressButton(j); } btnSelectables[j].AddInteractionPunch(); Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, btnSelectables[j].transform); return false; };
            btnSelectables[i].OnHighlight += delegate () { if (!solved && !animationPlaying) { Highlight(j, true); } };
            btnSelectables[i].OnHighlightEnded += delegate () { if (!solved && !animationPlaying) { Highlight(j, false); } };
        }
    }

    private void SetupRuleseed()
    {
        MonoRandom rng = ruleSeed.GetRNG();
        if (rng.Seed == 1)
        {
            colorNames = baseColorNames.Take(15).ToArray();
            partyNames = basePartyNames.Take(16).ToArray();
            colorNumbers = baseColorNumbers;
            partyNumbers = basePartyNumbers;
            votingMethodNames = Enum.GetValues(typeof(VotingMethod)).Cast<VotingMethod>().ToArray();
            sortingMethodNames = Enum.GetValues(typeof(SortingMethod)).Cast<SortingMethod>().Take(18).ToArray();
            edgeworkCalculations = Enum.GetValues(typeof(EdgeworkCalculation)).Cast<EdgeworkCalculation>().ToArray();
        }
        else
        {
            colorNames = rng.ShuffleFisherYates((string[])baseColorNames.Clone()).Take(15).ToArray();
            partyNames = rng.ShuffleFisherYates((string[])basePartyNames.Clone()).Take(16).ToArray();
            colorNumbers = rng.ShuffleFisherYates((int[])baseColorNumbers.Clone());
            partyNumbers = rng.ShuffleFisherYates((int[])basePartyNumbers.Clone());
            votingMethodNames = rng.ShuffleFisherYates(Enum.GetValues(typeof(VotingMethod)).Cast<VotingMethod>().ToArray());
            sortingMethodNames = rng.ShuffleFisherYates(Enum.GetValues(typeof(SortingMethod)).Cast<SortingMethod>().ToArray()).Take(18).ToArray();
            edgeworkCalculations = rng.ShuffleFisherYates(Enum.GetValues(typeof(EdgeworkCalculation)).Cast<EdgeworkCalculation>().ToArray());
            DebugMessage(string.Join(",", colorNames));
            DebugMessage(string.Join(",", partyNames));
            DebugMessage("Color numbers: " + string.Join(",", colorNumbers.Select(i => i.ToString()).ToArray()));
            DebugMessage("Party numbers: " + string.Join(",", partyNumbers.Select(i => i.ToString()).ToArray()));
            DebugMessage(string.Join(",", votingMethodNames.Select(i => i.ToLogString()).ToArray()));
            DebugMessage(string.Join(";", sortingMethodNames.Select(i => i.ToLogString()).ToArray()));
            DebugMessage(string.Join(",", edgeworkCalculations.Select(i => i.ToString()).ToArray()));
        }
    }

    private void Start()
    {
        votedSticker.SetActive(false);
        StartCoroutine(TextFade());

        SetupRuleseed();

        colors = Enumerable.Range(0, colorNames.Length).ToArray().Shuffle().Take(4).ToArray();
        parties = Enumerable.Range(0, partyNames.Length).ToArray().Shuffle().Take(4).ToArray();
        candidates = Enumerable.Range(0, candidateNames.Length).ToArray().Shuffle().Take(4).ToArray();

        // Generate the candidates
        for (int i = 0; i < 4; i++)
        {
            int colorIndex = Array.IndexOf(baseColorNames, colorNames[colors[i]]);
            btnRenderers[i].material = colorMats[colorIndex];
            if (colorIndex >= 7 && colorIndex <= 12)
                spriteRenderers[i].color = Color.white;
            else
                spriteRenderers[i].color = Color.black;
            spriteRenderers[i].sprite = sprites[Array.IndexOf(basePartyNames, partyNames[parties[i]])];
            LogMessage("Button #" + (i + 1) + " is " + colorNames[colors[i]] + ", has the " + partyNames[parties[i]] + " symbol, and represents the candidate " + candidateNames[candidates[i]] + ".");
        }

        // Find the correct voting method
        int[] values = GetEdgeworkCalculationArray();
        int[] secondRound = new int[4];
        int[] thirdRound = new int[2];

        LogMessage("Simulating tournament...");
        LogMessage("Round one!");

        for (int i = 0; i < 4; i++)
        {
            if (values[i * 2] >= values[i * 2 + 1]) { secondRound[i] = i * 2; }
            else { secondRound[i] = i * 2 + 1; }
            LogMessage("Comparing " + values[i * 2] + " (" + votingMethodNames[i * 2].ToLogString() + ") and " + values[i * 2 + 1] + " (" + votingMethodNames[i * 2 + 1].ToLogString() + ")... " + votingMethodNames[secondRound[i]].ToLogString() + " wins!");
        }

        LogMessage("Round two!");
        for (int i = 0; i < 2; i++)
        {
            if (values[secondRound[i * 2]] <= values[secondRound[i * 2 + 1]]) { thirdRound[i] = secondRound[i * 2]; }
            else { thirdRound[i] = secondRound[i * 2 + 1]; }
            LogMessage("Comparing " + values[secondRound[i * 2]] + " (" + votingMethodNames[secondRound[i * 2]].ToLogString() + ") and " + values[secondRound[i * 2 + 1]] + " (" + votingMethodNames[secondRound[i * 2 + 1]].ToLogString() + ")... " + votingMethodNames[thirdRound[i]].ToLogString() + " wins!");
        }

        LogMessage("Round three!");
        if (numbersAlphabeticalOrder[values[thirdRound[0]] % 20] <= numbersAlphabeticalOrder[values[thirdRound[1]] % 20])
            votingMethod = thirdRound[0];
        else
            votingMethod = thirdRound[1];
        LogMessage("Comparing " + values[thirdRound[0]] + " (" + votingMethodNames[thirdRound[0]].ToLogString() + ") and " + values[thirdRound[1]] + " (" + votingMethodNames[thirdRound[1]].ToLogString() + ")... " + votingMethodNames[votingMethod].ToLogString() + " wins!");
        LogMessage("This election uses " + votingMethodNames[votingMethod].ToLogString() + ".");

        // Find the votes
        char[] rows = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] serialNum = Info.GetSerialNumber().ToCharArray();
        for (int i = 0; i < 6; i++)
        {
            // Find method
            int placeholder = Array.IndexOf(rows, serialNum[i]) % 18;
            while (sortingMethods.Contains(placeholder))
                placeholder = (placeholder + 1) % 18;
            sortingMethods[i] = placeholder;

            double[] keys;
            string[] keysButStringsOops;
            int[] items = new int[4] { 0, 1, 2, 3 };

            // Sort
            switch (sortingMethodNames[sortingMethods[i]])
            {
                case SortingMethod.ColorAlphabetical:
                    keysButStringsOops = new string[4] { colorNames[colors[0]], colorNames[colors[1]], colorNames[colors[2]], colorNames[colors[3]] };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case SortingMethod.ColorLength:
                    keys = new double[4] { colorNames[colors[0]].Length, colorNames[colors[1]].Length, colorNames[colors[2]].Length, colorNames[colors[3]].Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.ColorReadingOrder:
                    keys = new double[4] { colors[0], colors[1], colors[2], colors[3] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    votes[i] = items;
                    break;
                case SortingMethod.ClockwiseTopRight:
                    votes[i] = new int[4] { 1, 3, 2, 0 };
                    break;
                case SortingMethod.ScrabbleLastThree:
                    keys = new double[4] {
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][spacelessCandidateNames[candidates[0]].Length - 1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][spacelessCandidateNames[candidates[0]].Length - 2])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][spacelessCandidateNames[candidates[0]].Length - 3])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][spacelessCandidateNames[candidates[1]].Length - 1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][spacelessCandidateNames[candidates[1]].Length - 2])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][spacelessCandidateNames[candidates[1]].Length - 3])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][spacelessCandidateNames[candidates[2]].Length - 1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][spacelessCandidateNames[candidates[2]].Length - 2])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][spacelessCandidateNames[candidates[2]].Length - 3])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][spacelessCandidateNames[candidates[3]].Length - 1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][spacelessCandidateNames[candidates[3]].Length - 2])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][spacelessCandidateNames[candidates[3]].Length - 3])]};
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.ClockwiseBottomLeft:
                    votes[i] = new int[4] { 2, 0, 1, 3 };
                    break;
                case SortingMethod.PartyNumber:
                    keys = new double[4] { partyNumbers[parties[0]], partyNumbers[parties[1]], partyNumbers[parties[2]], partyNumbers[parties[3]] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.PartyReadingOrder:
                    keys = new double[4] { parties[0], parties[1], parties[2], parties[3] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    votes[i] = items;
                    break;
                case SortingMethod.NameAlphabetical:
                    keysButStringsOops = new string[4] { candidateNames[candidates[0]], candidateNames[candidates[1]], candidateNames[candidates[2]], candidateNames[candidates[3]] };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case SortingMethod.ColorNumber:
                    keys = new double[4] { colorNumbers[colors[0]], colorNumbers[colors[1]], colorNumbers[colors[2]], colorNumbers[colors[3]] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.PartyLength:
                    keys = new double[4] { partyNames[parties[0]].Raw().Length, partyNames[parties[1]].Raw().Length, partyNames[parties[2]].Raw().Length, partyNames[parties[3]].Raw().Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.ClockwiseBottomRight:
                    votes[i] = new int[4] { 3, 2, 0, 1 };
                    break;
                case SortingMethod.PartyAlphabetical:
                    keysButStringsOops = new string[4] { partyNames[parties[0]].Raw(), partyNames[parties[1]].Raw(), partyNames[parties[2]].Raw(), partyNames[parties[3]].Raw() };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case SortingMethod.ScrabbleFirstThree:
                    keys = new double[4] {
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][2])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][2])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][2])],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][1])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][2])] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.ScrabbleFirstAndLast:
                    keys = new double[4] {
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[0]].Last())],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[1]].Last())],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[2]].Last())],
                        scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]][0])] + scrabbleScores[Array.IndexOf(alphabet, spacelessCandidateNames[candidates[3]].Last())],};
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.NameLength:
                    keys = new double[4] { spacelessCandidateNames[candidates[0]].Length, spacelessCandidateNames[candidates[1]].Length, spacelessCandidateNames[candidates[2]].Length, spacelessCandidateNames[candidates[3]].Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case SortingMethod.ClockwiseTopLeft:
                    votes[i] = new int[4] { 0, 1, 3, 2 };
                    break;
                case SortingMethod.CopyLastCharacter: // same way the last character voted
                    if (i == 0)
                        goto case SortingMethod.ReadingOrder;
                    else
                        votes[i] = votes[i - 1].ToArray();
                    break;
                case SortingMethod.ReadingOrder:
                    votes[i] = new int[4] { 0, 1, 2, 3 };
                    break;
            }

            LogMessage("The candidates in vote #" + (i + 1) + " is sorted " + sortingMethodNames[sortingMethods[i]].ToLogString() + ".");
            LogMessage("The order they voted is: " + (votes[i][0] + 1) + +(votes[i][1] + 1) + (votes[i][2] + 1) + (votes[i][3] + 1) + ".");
        }

        // Calculate the placing

        LogMessage("Calculating the placing...");
        int[] votePositions = { 0, 0, 0, 0, 0, 0 };
        bool[] eliminated = { false, false, false, false };

        switch (votingMethodNames[votingMethod])
        {
            case VotingMethod.FirstPastThePost:
                foreach (var vote in votes)
                    candidateScores[vote[0]]++;
                break;
            case VotingMethod.LastPastThePost:
                foreach (var vote in votes)
                    candidateScores[vote[3]]--;
                break;
            case VotingMethod.InstantRunoff:
                for (int i = 0; i < 4; i++)
                {
                    int[] tempCandidateScores = { 0, 0, 0, 0 };
                    for (int j = 0; j < 6; j++)
                    {
                        while (eliminated[votes[j][votePositions[j]]])
                            votePositions[j]++;
                        tempCandidateScores[votes[j][votePositions[j]]]++;
                    }

                    int minimum = 0;
                    for (int j = 1; j < 4; j++)
                        if ((tempCandidateScores[j] <= tempCandidateScores[minimum] || eliminated[minimum]) && !eliminated[j])
                            minimum = j;
                    eliminated[minimum] = true;
                    candidateScores[minimum] = i;
                }

                break;
            case VotingMethod.CoombMethod:
                votePositions = new int[6] { 3, 3, 3, 3, 3, 3 };
                for (int i = 0; i < 4; i++)
                {
                    int[] tempCandidateScores = { 0, 0, 0, 0 };
                    for (int j = 0; j < 6; j++)
                    {
                        while (eliminated[votes[j][votePositions[j]]])
                            votePositions[j]--;
                        tempCandidateScores[votes[j][votePositions[j]]]++;
                    }

                    int maximum = 0;
                    for (int j = 1; j < 4; j++)
                        if ((tempCandidateScores[j] >= tempCandidateScores[maximum] || eliminated[maximum]) && !eliminated[j])
                            maximum = j;
                    eliminated[maximum] = true;
                    candidateScores[maximum] = i;
                }

                break;
            case VotingMethod.BordaCount:
                foreach (var vote in votes)
                    for (int i = 0; i < 4; i++)
                        candidateScores[vote[i]] += 4 - i;
                break;
            case VotingMethod.ApprovalVoting:
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < (i % 3) + 1; j++)
                        candidateScores[votes[i][j]]++;
                break;
            case VotingMethod.STV:
                for (int i = 0; i < 4; i++)
                {
                    int[] tempCandidateScores = { 0, 0, 0, 0 };
                    for (int j = 0; j < 6; j++)
                    {
                        while (eliminated[votes[j][votePositions[j]]])
                            votePositions[j]++;
                        tempCandidateScores[votes[j][votePositions[j]]]++;
                    }

                    int maximum = 3;
                    for (int j = 2; j >= 0; j--)
                        if ((tempCandidateScores[j] >= tempCandidateScores[maximum] || eliminated[maximum]) && !eliminated[j])
                            maximum = j;
                    eliminated[maximum] = true;
                    candidateScores[maximum] = 4 - i;
                }

                break;
            case VotingMethod.CondorcetMethod:
                int[][] condorcetTable = {
                    new int[4] { 0,0,0,0 },
                    new int[4] { 0,0,0,0 },
                    new int[4] { 0,0,0,0 },
                    new int[4] { 0,0,0,0 }
                };

                foreach (var vote in votes)
                {
                    condorcetTable[vote[0]][vote[1]]++;
                    condorcetTable[vote[0]][vote[2]]++;
                    condorcetTable[vote[0]][vote[3]]++;
                    condorcetTable[vote[1]][vote[2]]++;
                    condorcetTable[vote[1]][vote[3]]++;
                    condorcetTable[vote[2]][vote[3]]++;
                }

                for (int i = 0; i < 4; i++)
                {
                    if (condorcetTable[i].Where(x => x > 3).Count() == 3)
                        candidateScores[i] = 99;
                    else
                        candidateScores[i] = condorcetTable[i].Sum();
                }

                break;
        }

        int[] candidateOrder = { 0, 1, 2, 3 };
        int[] sortedCandidateScores = candidateScores;
        Array.Sort(sortedCandidateScores, candidateOrder);

        sortedCandidateScores = sortedCandidateScores.Reverse().ToArray(); // oops lol
        candidateOrder = candidateOrder.Reverse().ToArray();

        string[] ordinalNumbers = { "first", "second", "third", "fourth" };

        int lastScore = sortedCandidateScores[0];

        DebugMessage(string.Format("secrert debugg.g... (internal scores) {0} {1} {2} {3}", candidateScores[0], candidateScores[1], candidateScores[2], candidateScores[3]));
        LogMessage("Button #" + (candidateOrder[0] + 1) + " is in " + ordinalNumbers[candidatePlacement[candidateOrder[0]]] + " place.");

        for (int i = 1; i < 4; i++)
        {
            if (sortedCandidateScores[i] == lastScore)
            {
                DebugMessage(string.Format("secrert debugg.g... {0} {1}", sortedCandidateScores[i], lastScore));
                candidatePlacement[candidateOrder[i]] = candidatePlacement[candidateOrder[i - 1]];
                LogMessage("Button #" + (candidateOrder[i] + 1) + " is also in " + ordinalNumbers[candidatePlacement[candidateOrder[i]]] + " place.");
            }
            else
            {
                DebugMessage(string.Format("secrert debugg.g... {0} {1}", sortedCandidateScores[i], lastScore));
                candidatePlacement[candidateOrder[i]] = candidatePlacement[candidateOrder[i - 1]] + 1;
                lastScore = sortedCandidateScores[i];
                LogMessage("Button #" + (candidateOrder[i] + 1) + " is in " + ordinalNumbers[candidatePlacement[candidateOrder[i]]] + " place.");
            }
        }
    }

    void PressButton(int btnNum)
    {
        if (!pressedButtons[btnNum])
        {
            LogMessage("You pressed Button #" + (btnNum + 1) + ".");

            if (candidatePlacement[btnNum] == nextPlace)
            {
                pressedButtons[btnNum] = true;
                ledRenderers[btnNum].material = correctMat;
                LogMessage("That was correct.");

                if (pressedButtons.All(x => x))
                {
                    Module.HandlePass();
                    solved = true;
                    LogMessage("All buttons pressed. Module solved!");
                    StartCoroutine(SolveAnim());
                }

                for (int i = 0; i < 4; i++)
                    if (!pressedButtons[i] && candidatePlacement[i] == nextPlace) { nextPlace--; break; }

                nextPlace++;
            }

            else
            {
                LogMessage("That was not correct.");
                Module.HandleStrike();
                StartCoroutine(StrikeAnim(btnNum));
            }
        }
    }

    int[] GetEdgeworkCalculationArray()
    {
        int[] res = new int[8];
        for (int i = 0; i < 8; i++)
        {
            switch (edgeworkCalculations[i])
            {
                case EdgeworkCalculation.AABatteries:
                    res[i] = Info.GetBatteryCount(Battery.AA);
                    break;
                case EdgeworkCalculation.DBatteries:
                    res[i] = Info.GetBatteryCount(Battery.D) * 2;
                    break;
                case EdgeworkCalculation.LitIndicators:
                    res[i] = Info.GetOnIndicators().Count() * 2;
                    break;
                case EdgeworkCalculation.UnlitIndicators:
                    res[i] = Info.GetOffIndicators().Count() * 2;
                    break;
                case EdgeworkCalculation.Ports:
                    res[i] = Info.GetPortCount();
                    break;
                case EdgeworkCalculation.PortPlates:
                    res[i] = Info.GetPortPlateCount() * 2;
                    break;
                case EdgeworkCalculation.SecondDigitSN:
                    res[i] = Info.GetSerialNumberNumbers().ElementAt(1);
                    break;
                case EdgeworkCalculation.SecondLastDigitSN:
                    res[i] = Info.GetSerialNumberNumbers().Reverse().ElementAt(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("edgeworkCalculations[{0}]", i), edgeworkCalculations[i], null);
            }
        }
        return res;
    }

    void Highlight(int btnNum, bool on)
    {
        if (on)
        {
            screenText.text = candidateNames[candidates[btnNum]];
            if (screenText.text.Length > 22)
                screenText.characterSize = .12f;
            else if (screenText.text.Length > 18)
                screenText.characterSize = .15f;
            else
                screenText.characterSize = .175f;
        }
        else
            screenText.text = "";
    }

    IEnumerator StrikeAnim(int btnNum)
    {
        animationPlaying = true;
        screenText.characterSize = .17f;
        screenText.text = "EPIC DEMOCRACY FAIL";
        ledRenderers[btnNum].material = wrongMat;
        yield return new WaitForSeconds(1f);
        ledRenderers[btnNum].material = blackMat;
        animationPlaying = false;
        screenText.text = "";
    }

    IEnumerator SolveAnim()
    {
        screenText.characterSize = .17f;
        screenText.text = "";
        for (int i = 0; i < 4; i++)
        {
            btnRenderers[i].material = colorMats[12];
            spriteObjects[i].SetActive(false);
            ledRenderers[i].material = blackMat;

            yield return new WaitForSeconds(.25f);
        }

        votedSticker.SetActive(true);
        screenText.text = "PRESIDENT ELECTED";
        Audio.PlaySoundAtTransform("eagle", Module.transform);
    }

    IEnumerator TextFade()
    {
        bool fadingIn = false;
        while (true)
        {
            if (animationPlaying)
                screenText.color = new Color32(255, 0, 0, whiteness);
            else if (solved)
                screenText.color = new Color32(0, 255, 0, whiteness);
            else
                screenText.color = new Color32(255, 255, 255, whiteness);

            if (fadingIn)
                whiteness += 2;
            else
                whiteness -= 2;
            if (whiteness >= 255)
                fadingIn = false;
            else if (whiteness <= 175)
                fadingIn = true;

            yield return new WaitForSeconds(.01f);
        }
    }

    void LogMessage(string message)
    {
        Debug.LogFormat("[Presidential Elections #{0}] {1}", moduleId, message);
    }

    void DebugMessage(string message)
    {
        Debug.LogFormat("<Presidential Elections #{0}> {1}", moduleId, message);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use [!{0} cycle] to highlight all 4 buttons. [!{0} highlight # # #] to highlight whatever buttons you want. [!{0} press # # # #] to press a series of 1-4 buttons. Use numbers 1 to 4 (represents buttons in reading order)";
#pragma warning restore 414

    MethodInfo highlightMethod = null;
    object enumValue = null;
    Component[] highlights = null;

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        if (Regex.IsMatch(command, "^cycle$", RegexOptions.IgnoreCase))
        {
            yield return null;
            if (!Application.isEditor)
            {
                if (highlights == null || enumValue == null || highlightMethod == null) SetupHighlightables();

                for (int i = 0; i < 4; i++)
                {
                    btnSelectables[i].OnHighlight();
                    highlightMethod.Invoke(highlights[i], new[] { true, enumValue });
                    yield return new WaitForSecondsRealtime(3f);
                    highlightMethod.Invoke(highlights[i], new[] { false, enumValue });
                    btnSelectables[i].OnHighlightEnded();
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    btnSelectables[i].OnHighlight();
                    yield return new WaitForSecondsRealtime(3f);
                    btnSelectables[i].OnHighlightEnded();
                }
            }

        }
        else if (Regex.IsMatch(command, @"^highlight(\s+[1-4])+$"))
        {
            yield return null;
            int[] buttons = command.Split().Skip(1).Select(n => int.Parse(n) - 1).ToArray();
            if (!Application.isEditor)
            {
                if (highlights == null || enumValue == null || highlightMethod == null) SetupHighlightables();

                foreach (int button in buttons)
                {
                    btnSelectables[button].OnHighlight();
                    highlightMethod.Invoke(highlights[button], new[] { true, enumValue });
                    yield return new WaitForSecondsRealtime(3f);
                    highlightMethod.Invoke(highlights[button], new[] { false, enumValue });
                    btnSelectables[button].OnHighlightEnded();
                }
            }
            else
            {
                foreach (int button in buttons)
                {
                    btnSelectables[button].OnHighlight();
                    yield return new WaitForSecondsRealtime(3f);
                    btnSelectables[button].OnHighlightEnded();
                }
            }
        }
        else if (Regex.IsMatch(command, @"^press(\s+[1-4]){1,4}$"))
        {
            yield return null;
            int[] buttons = command.Split().Skip(1).Select(n => int.Parse(n) - 1).ToArray();
            foreach (int button in buttons)
            {
                btnSelectables[button].OnInteract();
                yield return new WaitForSecondsRealtime(.1f);
            }
        }

    }

    private void SetupHighlightables()
    {
        highlights = btnHighlights.Select(km => km.GetComponent("Highlightable")).ToArray();
        if (highlights.Length == 0) return;
        var e = highlights[0].GetType().GetNestedType("HighlightTypeEnum", BindingFlags.Public);
        highlightMethod = highlights[0].GetType().GetMethod("On", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool), e }, null);
        enumValue = Enum.ToObject(e, 1);
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        List<int> nbs = Enumerable.Range(0, 4).Where(n => !pressedButtons[n]).ToList();
        nbs = nbs.OrderBy(n => candidatePlacement[n]).ToList();
        foreach (int nb in nbs)
        {
            btnSelectables[nb].OnInteract();
            yield return new WaitForSecondsRealtime(.1f);
        }
    }
}
