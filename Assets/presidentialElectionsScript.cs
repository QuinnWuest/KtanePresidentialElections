using System.Collections;
using UnityEngine;
using KModkit;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

public class presidentialElectionsScript : MonoBehaviour {

    // Standard module variables
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMAudio Audio;

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

    // Candidate variables
    int[] colors = { 99, 99, 99, 99 };
    int[] parties = { 99, 99, 99, 99 };
    int[] candidates = { 99, 99, 99, 99 };

    static readonly string[] colorNames = { "red", "green", "blue", "yellow", "magenta", "cyan", "orange", "purple", "brown", "crimson", "forest", "navy", "black", "gray", "white" };
    static readonly string[] partyNames = { "Slowpoke", "Mischief", "Conspiracy", "Trivia Murder", "Rent Is Too Damn Low", "Experimental", "Quack Quack", "Birthday", "Carcinization", "Vine Boom", "Android", "Toxicity", "Little Guy", "Vote For This", "Aaaaaaaah", "Catpeople" };
    static readonly string[] candidateNames = {
        "YOUR MOM", "DICK KICKEM", "GRUNKLE SQUEAKY", "YABBAGUY", "DEAF", "EPICTOAST", "BAGELS", "IVAN IVANSKY IVANOVICH", "SHELDON COOPER", "MR BILL CLINTON SEX SCANDAL", "VERMIN SUPREME", "DEEZ NUTS", 
        "YOUR DAD", "OMEGA", "COLORS", "VOID", "LUNA", "COOLDOOM", "MCD", "CRAZYCALEB", "LIL UZI VERT", "BOB", "SIMON", "TWITCH PLAYS HIVEMIND", "TASHA", "JEB", "LOGBOT", "KANYE WEST", "JEAVER",
        "KONNOR", "MR PEANUT", "BLAN", "JENSON", "VANILLA", "CHOCOLA", "MARGARET THATCHER", "KONOKO", "THE DEMOGORGON", "BABE RUTH", "KEVIN LEE", "THE E PAWN", "MARCUS STUYVESANT", "KOOPA TROOPA",
        "HONG JIN-HO", "KAZEYOSHI IMAI", "DON CHEADLE", "MILLIE ROSE", "WARIO", "DEPRESSO", "GORDON FREEMAN", "NEIL CICIEREGA", "THE SHELLED ONE", "ANYONE BUT DEAF", "DART MONKEY", "SANTA CLAUS", "CTHULHU"
    };
    static readonly string[] spacelessPartyNames = { "SLOWPOKE", "MISCHIEF", "CONSPIRACY", "TRIVIAMURDER", "RENTISTOODAMNLOW", "EXPERIMENTAL", "QUACKQUACK", "BIRTHDAY", "CARCINIZATION", "VINEBOOM", "ANDROID", "TOXICITY", "LITTLEGUY", "VOTEFORTHIS", "AAAAAAAAH", "CATPEOPLE" };
    static readonly string[] spacelessCandidateNames = { // i am choosing violence because c# is not letting me replace a space with the empty string
       "YOURMOM","DICKKICKEM","GRUNKLESQUEAKY","YABBAGUY","DEAF","EPICTOAST","BAGELS","IVANIVANSKYIVANOVICH","SHELDONCOOPER","MRBILLCLINTONSEXSCANDAL","VERMINSUPREME","DEEZNUTS",
        "YOURDAD","OMEGA","COLORS","VOID","LUNA","COOLDOOM","MCD","CRAZYCALEB","LILUZIVERT","BOB","SIMON","TWITCHPLAYSHIVEMIND","TASHA","JEB","LOGBOT","KANYEWEST","JEAVER",
        "KONNOR","MRPEANUT","BLAN","JENSON","VANILLA","CHOCOLA","MARGARETTHATCHER","KONOKO","THEDEMOGORGON","BABERUTH","KEVINLEE","THEEPAWN","MARCUSSTUYVESANT","KOOPATROOPA",
        "HONGJINHO","KAZEYOSHIIMAI","DONCHEADLE","MILLIEROSE","WARIO","DEPRESSO","GORDONFREEMAN","NEILCICIEREGA","THESHELLEDONE","ANYONEBUTDEAF", "DARTMONKEY", "SANTACLAUS", "CTHULHU"
    };
    static readonly int[] colorNumbers = { 12, 5, 13, 3, 7, 4, 8, 10, 14, 15, 2, 6, 1, 9, 11 };
    static readonly int[] partyNumbers = { 7, 15, 4, 2, 9, 8, 12, 13, 6, 11, 3, 16, 1, 5, 14, 10 };

    // Voting variables
    int votingMethod = 0;
    static readonly string[] votingMethodNames = { "first-past-the-post", "last-past-the-post", "instant runoff", "Coomb's method", "Borda count", "approval voting", "STV", "Condorcet method" };

    int[] sortingMethods = { 99, 99, 99, 99, 99, 99 };
    static readonly string[] sortingMethodNames = { "in alphabetical order by their color names", "by the length of their color names", "in reading order based on the color name table",
    "clockwise, starting from the top-right", "by the Scrabble score of the last three letters of their names", "clockwise, starting from the bottom-left", "by the number next to their party names",
    "in reading order based on the party name table", "in alphabetical order by their names", "by the number next to their color names", "by the length of their party names",
    "clockwise, starting from the bottom-right", "in alphabetical order by their party names", "by the Scrabble score of the first three letters of their names",
    "by the Scrabble score of the first and last letters of their names", "by the length of their names", "clockwise, starting from the top-left", "the same way the last character voted", "in reading order" };

    int[][] votes =
    {
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
        new int[4] { 0, 0, 0, 0 },
    };
    int[] candidateScores = { 0, 0, 0, 0 };
    bool[] pressedButtons = { false, false, false, false };
    int[] candidatePlacement = { 0, 0, 0, 0 };
    int nextPlace = 0;

    // Other variables
    //                                       A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q,  R, S, T, U, V, W, X, Y, Z
    static readonly int[] scrabbleScores = { 1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3, 10, 1, 1, 1, 1, 4, 4, 8, 4, 10 };
    static readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    static readonly int[] numbersAlphabeticalOrder = { 19, 9,18,16,5,4, 12,10,0,7,14, 2,17,15,6,3, 13,11,1,8 };
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

    private void Start()
    {
        votedSticker.SetActive(false);
        StartCoroutine(TextFade());

        // Generate the candidates
        for (int i = 0; i < 4; i++)
        {
            while (colors.Where(x => x == colors[i]).Count() > 1 || colors[i] == 99)
                colors[i] = Random.Range(0, colorNames.Length);
            while (parties.Where(x => x == parties[i]).Count() > 1 || parties[i] == 99)
                parties[i] = Random.Range(0, partyNames.Length);
            while (candidates.Where(x => x == candidates[i]).Count() > 1 || candidates[i] == 99)
                candidates[i] = Random.Range(0, candidateNames.Length);

            btnRenderers[i].material = colorMats[colors[i]];
            if (colors[i] >= 7 && colors[i] <= 12)
                spriteRenderers[i].color = Color.white;
            else
                spriteRenderers[i].color = Color.black;
            spriteRenderers[i].sprite = sprites[parties[i]];
            DebugMsg("Button #" + (i+1) + " is " + colorNames[colors[i]] + ", has the " + partyNames[parties[i]] + " symbol, and represents the candidate " + candidateNames[candidates[i]] + ".");
        }

        // Find the correct voting method
        int[] values = { Info.GetBatteryCount(Battery.AA), Info.GetBatteryCount(Battery.D) * 2, Info.GetOnIndicators().Count() * 2, Info.GetOffIndicators().Count() * 2, Info.GetPortCount(), Info.GetPortPlateCount() * 2, Info.GetSerialNumberNumbers().ElementAt(1), Info.GetSerialNumberNumbers().Reverse().ElementAt(1) };
        int[] secondRound = new int[4];
        int[] thirdRound = new int[2];

        DebugMsg("Simulating tournament...");
        DebugMsg("Round one!");
        
        for (int i = 0; i < 4; i++)
        {
            if (values[i * 2] >= values[i * 2 + 1]) { secondRound[i] = i * 2; }
            else { secondRound[i] = i * 2 + 1; }
            DebugMsg("Comparing " + values[i*2] + " (" + votingMethodNames[i*2] + ") and " + values[i*2+1] + " (" + votingMethodNames[i*2+1] + ")... " + votingMethodNames[secondRound[i]] + " wins!");
        }

        DebugMsg("Round two!");
        for (int i = 0; i < 2; i++)
        {
            if (values[secondRound[i * 2]] <= values[secondRound[i * 2 + 1]]) { thirdRound[i] = secondRound[i * 2]; }
            else { thirdRound[i] = secondRound[i * 2 + 1]; }
            DebugMsg("Comparing " + values[secondRound[i * 2]] + " (" + votingMethodNames[secondRound[i * 2]] + ") and " + values[secondRound[i * 2 + 1]] + " (" + votingMethodNames[secondRound[i * 2 + 1]] + ")... " + votingMethodNames[thirdRound[i]] + " wins!");
        }

        DebugMsg("Round three!");
        if (numbersAlphabeticalOrder[values[thirdRound[0]] % 20] <= numbersAlphabeticalOrder[values[thirdRound[1]] % 20])
            votingMethod = thirdRound[0];
        else
            votingMethod = thirdRound[1];
        DebugMsg("Comparing " + values[thirdRound[0]] + " (" + votingMethodNames[thirdRound[0]] + ") and " + values[thirdRound[1]] + " (" + votingMethodNames[thirdRound[1]] + ")... " + votingMethodNames[votingMethod] + " wins!");
        DebugMsg("This election uses " + votingMethodNames[votingMethod] + ".");

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
            switch (sortingMethods[i])
            {
                case 0: // alphabetical by color names
                    keysButStringsOops = new string[4] { colorNames[colors[0]], colorNames[colors[1]], colorNames[colors[2]], colorNames[colors[3]] };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case 1: // length of color names
                    keys = new double[4] { colorNames[colors[0]].Length, colorNames[colors[1]].Length, colorNames[colors[2]].Length, colorNames[colors[3]].Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case 2: // reading order based on color table
                    keys = new double[4] { colors[0], colors[1], colors[2], colors[3] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1;  }
                    Array.Sort(keys, items);
                    votes[i] = items;
                    break;
                case 3: // clockwise from top-right
                    votes[i] = new int[4] { 1, 3, 2, 0 };
                    break;
                case 4: // Scrabble score of last three letters (FEAR ME)
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
                case 5: // clockwise from bottom-left
                    votes[i] = new int[4] { 2, 0, 1, 3 };
                    break;
                case 6: // number next to party names
                    keys = new double[4] { partyNumbers[parties[0]], partyNumbers[parties[1]], partyNumbers[parties[2]], partyNumbers[parties[3]] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case 7: // reading order based on party table
                    keys = new double[4] { parties[0], parties[1], parties[2], parties[3] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    votes[i] = items;
                    break;
                case 8: // alphabetical by names
                    keysButStringsOops = new string[4] { candidateNames[candidates[0]], candidateNames[candidates[1]], candidateNames[candidates[2]], candidateNames[candidates[3]] };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case 9: // number next to color names
                    keys = new double[4] { colorNumbers[colors[0]], colorNumbers[colors[1]], colorNumbers[colors[2]], colorNumbers[colors[3]] };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case 10: // length of party names
                    keys = new double[4] { spacelessPartyNames[parties[0]].Length, spacelessPartyNames[parties[1]].Length, spacelessPartyNames[parties[2]].Length, spacelessPartyNames[parties[3]].Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case 11: // clockwise from bottom-right
                    votes[i] = new int[4] { 3, 2, 0, 1 };
                    break;
                case 12: // alphabetical by party names
                    keysButStringsOops = new string[4] { spacelessPartyNames[parties[0]], spacelessPartyNames[parties[1]], spacelessPartyNames[parties[2]], spacelessPartyNames[parties[3]] };
                    Array.Sort(keysButStringsOops, items);
                    votes[i] = items;
                    break;
                case 13: // Scrabble score of first three letters
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
                case 14: // Scrabble score of first and last letters
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
                case 15: // length of names
                    keys = new double[4] { spacelessCandidateNames[candidates[0]].Length, spacelessCandidateNames[candidates[1]].Length, spacelessCandidateNames[candidates[2]].Length, spacelessCandidateNames[candidates[3]].Length };
                    for (int j = 0; j < 4; j++) { keys[j] -= j * .1; }
                    Array.Sort(keys, items);
                    Array.Reverse(items);
                    votes[i] = items;
                    break;
                case 16: // clockwise from top left
                    votes[i] = new int[4] { 0, 1, 3, 2 };
                    break;
                case 17: // same way the last character voted
                    if (i == 0) { votes[i] = new int[4] { 0, 1, 2, 3 }; sortingMethods[i]++; }
                    else
                        votes[i] = votes[i - 1];
                    break;
            }

            DebugMsg("The candidates in vote #" + (i+1) + " is sorted " + sortingMethodNames[sortingMethods[i]] + ".");
            DebugMsg("The order they voted is: " + (votes[i][0]+1) + + (votes[i][1] + 1) + (votes[i][2] + 1) + (votes[i][3] + 1) + ".");
        }

        // Calculate the placing

        DebugMsg("Calculating the placing...");
        int[] votePositions = { 0, 0, 0, 0, 0, 0 };
        bool[] eliminated = { false, false, false, false };

        switch (votingMethod)
        {
            case 0: // first-past-the-post
                foreach (var vote in votes)
                    candidateScores[vote[0]]++;
                break;
            case 1: // last-past-the-post
                foreach (var vote in votes)
                    candidateScores[vote[3]]--;
                break;
            case 2: // instant runoff
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
            case 3: // coom's method
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
            case 4: // borda count
                foreach (var vote in votes)
                    for (int i = 0; i < 4; i++)
                        candidateScores[vote[i]] += 4 - i;
                break;
            case 5: // approval voting
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < (i % 3) + 1; j++)
                        candidateScores[votes[i][j]]++;
                break;
            case 6: // single transferrable vote
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
                    candidateScores[maximum] = 4-i;
                }
                
                break;
            case 7: // condorcet method
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

        Debug.LogFormat("secrert debugg.g... (internal scores) {0} {1} {2} {3}", candidateScores[0], candidateScores[1], candidateScores[2], candidateScores[3]);
        DebugMsg("Button #" + (candidateOrder[0] + 1) + " is in " + ordinalNumbers[candidatePlacement[candidateOrder[0]]] + " place.");

        for (int i = 1; i < 4; i++)
        {
            if (sortedCandidateScores[i] == lastScore)
            {
                Debug.LogFormat("secrert debugg.g... {0} {1}", sortedCandidateScores[i], lastScore);
                candidatePlacement[candidateOrder[i]] = candidatePlacement[candidateOrder[i - 1]];
                DebugMsg("Button #" + (candidateOrder[i]+1) + " is also in " + ordinalNumbers[candidatePlacement[candidateOrder[i]]] + " place.");
            }
            else
            {
                Debug.LogFormat("secrert debugg.g... {0} {1}", sortedCandidateScores[i], lastScore);
                candidatePlacement[candidateOrder[i]] = candidatePlacement[candidateOrder[i - 1]] + 1;
                lastScore = sortedCandidateScores[i];
                DebugMsg("Button #" + (candidateOrder[i] + 1) + " is in " + ordinalNumbers[candidatePlacement[candidateOrder[i]]] + " place.");
            }
        }
    }

    void PressButton(int btnNum)
    {
        if (!pressedButtons[btnNum])
        {
            DebugMsg("You pressed Button #" + (btnNum + 1) + ".");

            if (candidatePlacement[btnNum] == nextPlace)
            {
                pressedButtons[btnNum] = true;
                ledRenderers[btnNum].material = correctMat;
                DebugMsg("That was correct.");

                if (pressedButtons.All(x => x))
                {
                    Module.HandlePass();
                    solved = true;
                    DebugMsg("All buttons pressed. Module solved!");
                    StartCoroutine(SolveAnim());
                }

                for (int i = 0; i < 4; i++)
                    if (!pressedButtons[i] && candidatePlacement[i] == nextPlace) { nextPlace--; break; }

                nextPlace++;
            }

            else
            {
                DebugMsg("That was not correct.");
                Module.HandleStrike();
                StartCoroutine(StrikeAnim(btnNum));
            }
        }
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

    void DebugMsg(string message)
    {
        Debug.LogFormat("[Presidential Elections #{0}] {1}", moduleId, message);
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use [!{0} cycle] to highlight all 4 buttons. [!{0} highlight # # #] to highlight whatever buttons you want. [!{0} press # # # #] to press a series of 1-4 buttons. Use numbers 1 to 4 (represents buttons in reading order)";
#pragma warning restore 414

    MethodInfo highlightMethod = null;
    object enumValue = null;
    Component[] highlights = null;

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim();
        if(Regex.IsMatch(command, "^cycle$", RegexOptions.IgnoreCase))
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
                if(highlights == null || enumValue == null || highlightMethod == null) SetupHighlightables();

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
            foreach(int button in buttons)
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
        foreach(int nb in nbs)
        {
            btnSelectables[nb].OnInteract();
            yield return new WaitForSecondsRealtime(.1f);
        }
    }
}
