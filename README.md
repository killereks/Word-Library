# Word-Library API

This API provides a set of filtering methods for a collection of words, allowing you to search for words based on various criteria such as length, letters, syllables, etc.

# Features
- Get words based on their length (minimum, maximum, or fixed length).
- Get words containing specific letters.
- Get words that start with, end with, or contain a specific string.
- Get words with a specific syllable count.
- Get palindromes.
- Get anagrams.

- Ability to sort results.
- Regex Matching
- Words with distinct or repeating letters.
- Mapping words


# Requirements
C# .NET Frametwork 4.0 or higher.

# Usage
Just download the file and drop it in your project.

# Examples
```cs
WordLibrary wordLibrary = new WordLibrary("words.txt");

// optionally u can specify all words to be lowercase
wordLibrary.ForceLowercase();

WordResults results = wordLibrary.GetAllWords().StartsWith("no").MinLength(7).WithoutLetter('a');
```
```cs
var a = wordLibrary.GetAllWords().FixedLength(6).WithoutUniqueLetters();
var b = wordLibrary.GetAllWords().MatchPattern("c***t"); // * = any letter, ? = vowel, ! = consonant, for more control use MatchRegex

var intersection = a.Intersect(b);
```

```cs
wordLibrary.IsValid("cat"); // do we have cat in our word list?

wordLibrary.GetWordsWithLetters("abctdog".ToCharArray()); // Returns: "cat", "dog", "bat"
```

```cs
wordLibrary.GetRandomWords(5); // returns 5 random words (could repeat)
```

```cs
// "cas" here is mispelt, we want to correct at most 1 letter to make it a valid word
wordLibrary.Autocomplete("cas", 1); // returns "cat".
```

```cs
wordLibrary.GetAnagrams("silent"); // "listen", "silent", "tinsel"
```

```cs
wordLibrary.GetPalindromes(); // "racecar" + ...
```

```cs
var a = wordLibrary.GetAllWords().Contains("a").MinLength(3);
// all words that have no A and are shorter or equal to 3
var noA = wordLibrary.GetAllWords().Except(a);
```

```cs
// distinct will remove all word duplicates, ensuring uniqueness
var funWords = wordLibrary.GetAllWords().MinLength(5).MaxLength(8).Distinct().SortByLength();
```

```cs
WordResult a = wordLibrary.GetAllWords();

// word result object has properties like
a.ToList();
a.ToArray();
a.ToHashSet();
a.Count; // how many words it contains
a[5]; // get 5th word
a.Words; // your results
```

# Contributions
Contributions are welcome! If you find a bug or would like to request a new feature, please open an issue. If you would like to contribute code, please create a pull request.

# License
This project is licensed under the MIT License.
