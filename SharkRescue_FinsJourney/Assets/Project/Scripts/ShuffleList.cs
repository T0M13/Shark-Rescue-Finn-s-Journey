using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShuffleList
{
    //FISHER-YATES SHUFFLE
    //https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
    //https://stackoverflow.com/questions/273313/randomize-a-listt
    //https://www.youtube.com/watch?v=9ILhNCyWqVA

    private static readonly System.Random Random = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int index = list.Count; //Number of shuffles we'll be doing
        while(index > 1) //For each Shuffle...
        {
            index--;
            int randomIndex = Random.Next(index + 1); //Random number between 0 and n + 1 (will not return the maxValue)
            T value = list[randomIndex]; //Cache the value in the list at that random index
            list[randomIndex] = list[index]; //Put the item we are shuffling into the random spot
            list[index] = value; //And put the item that was a the random spot, into the spot we're shffling
        }
    }
}
