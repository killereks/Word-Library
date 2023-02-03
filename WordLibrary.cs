using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Random = System.Random;

public class WordLibrary {
	private HashSet<string> _words;
	private string[] _wordsArray;

	public static Random rand = new Random();

	/// <summary>
	/// Creates a new word library from a file.
	/// </summary>
	/// <param name="filepath">File path to load words from, separated by new line</param>
	public WordLibrary(string filepath){
		_words = new HashSet<string>();
		LoadWords(filepath);
		
		_wordsArray = _words.ToArray();
	}

	/// <summary>
	/// Creates a new word library from a list of words.
	/// </summary>
	/// <param name="words">List of words to use</param>
	public WordLibrary(string[] words){
		_words = new HashSet<string>(words);
		_wordsArray = _words.ToArray();
	}
	
	/// <summary>
	/// Forces the word library to use lowercase words. Helpful for case-insensitive uses.
	/// </summary>
	public void ForceLowercase(){
		_words = new HashSet<string>(_words.Select(x => x.ToLower()));
		_wordsArray = _words.ToArray();
	}

	void LoadWords(string filepath){
		string contents = File.ReadAllText(filepath);

		string[] lines = contents.Split('\n');

		foreach (string line in lines){
			if (string.IsNullOrEmpty(line)) continue;
			
			_words.Add(line.Trim());
		}
	}

	/// <summary>
	/// Returns random number of words from the library.
	/// </summary>
	/// <param name="amount">How many random words to return</param>
	/// <returns></returns>
	public WordResults GetRandomWords(int amount){
		int index = rand.Next(0, _words.Count);

		string[] words = new string[amount];

		for (int i = 0; i < amount; i++){
			words[i] = _wordsArray[index];
			index = rand.Next(0, _words.Count);
		}

		return new WordResults(words);
	}

	/// <summary>
	/// Gets all words that can be made from the given letters.
	/// Example:
	/// Letters: "abctdog"
	/// Words: "cat", "dog", "bat"
	/// </summary>
	/// <param name="letters"></param>
	/// <returns></returns>
	public WordResults GetWordsWithLetters(char[] letters){
		Dictionary<char, int> letterCounts = new Dictionary<char, int>();
		foreach (char c in letters){
			if (letterCounts.ContainsKey(c)){
				letterCounts[c]++;
			}
			else{
				letterCounts[c] = 1;
			}
		}

		List<string> result = new List<string>();
		foreach (string word in _words){
			Dictionary<char, int> wordCounts = new Dictionary<char, int>();
			foreach (char c in word){
				if (wordCounts.ContainsKey(c)){
					wordCounts[c]++;
				}
				else{
					wordCounts[c] = 1;
				}
			}

			bool isValid = true;
			foreach (char c in wordCounts.Keys){
				if (!letterCounts.ContainsKey(c) || wordCounts[c] > letterCounts[c]){
					isValid = false;
					break;
				}
			}

			if (isValid){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Does this library contain the given word?
	/// </summary>
	/// <param name="word">Word to check, case sensitive</param>
	/// <returns></returns>
	public bool IsValid(string word) => _words.Contains(word);

	/// <summary>
	/// Is given letter a vowel?
	/// </summary>
	/// <param name="c"></param>
	/// <returns></returns>
	public static bool IsVowel(char c) => "aeiou".Contains(c);
	/// <summary>
	/// Is a given letter a consonant?
	/// </summary>
	/// <param name="c"></param>
	/// <returns></returns>
	public static bool IsConsonant(char c) => "bcdfghjklmnpqrstvwxyz".Contains(c);

	/// <summary>
	/// Returns all words that can be corrected to the given word given the correction amount limit.
	/// </summary>
	/// <param name="invalidWord"></param>
	/// <param name="maxCorrectionCharacters"></param>
	/// <returns></returns>
	public WordResults Autocomplete(string invalidWord, int maxCorrectionCharacters){
		List<string> result = new List<string>();

		foreach (string word in _words){
			if (word.Length < invalidWord.Length){
				continue;
			}

			int difference = 0;
			for (int i = 0; i < invalidWord.Length; i++){
				if (invalidWord[i] != word[i]){
					difference++;
				}
			}

			if (difference <= maxCorrectionCharacters){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Gets all anagrams of the given word.
	/// Example:
	/// "silent" -> "listen", "silent", "tinsel"
	/// </summary>
	/// <param name="word"></param>
	/// <returns></returns>
	public WordResults GetAnagrams(string word){
		char[] charArray = word.ToCharArray();
		Array.Sort(charArray);
		string sortedWord = new string(charArray);

		List<string> result = new List<string>();
		foreach (string storedWord in _words)
		{
			char[] storedCharArray = storedWord.ToCharArray();
			Array.Sort(storedCharArray);
			string storedSortedWord = new string(storedCharArray);

			if (sortedWord == storedSortedWord)
			{
				result.Add(storedWord);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Returns all palindromes in the library.
	/// Everything that is word == word.Reverse()
	/// Example:
	/// racecar -> racecar
	/// </summary>
	/// <returns></returns>
	public WordResults GetPalindromes()
	{
		List<string> result = new List<string>();
		int wordCount = _words.Count;

		for (int i = 0; i < wordCount; i++)
		{
			string word = _wordsArray[i];
			int length = word.Length;
			int j = 0;

			for (j = 0; j < length / 2; j++)
			{
				if (word[j] != word[length - j - 1])
				{
					break;
				}
			}

			if (j == length / 2)
			{
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Get all words in the library as a filterable object.
	/// </summary>
	/// <returns></returns>
	public WordResults GetAllWords(){
		return new WordResults(_words.ToArray());
	}
}

public class WordResults {
	string[] _words;
	/// <summary>
	/// All words in the result.
	/// </summary>
	public string[] Words => _words;
	
	/// <summary>
	/// Number of words in the result.
	/// </summary>
	public int Count => _words.Length;
	
	/// <summary>
	/// Ability to index the object like an array.
	/// </summary>
	/// <param name="index"></param>
	public string this[int index] => _words[index];

	/// <summary>
	/// Create a new WordResults object.
	/// </summary>
	/// <param name="words"></param>
	public WordResults(string[] words){
		_words = words;
	}
	
	/// <summary>
	/// Filter all words that start with the given string.
	/// </summary>
	/// <param name="prefix"></param>
	/// <returns></returns>
	public WordResults StartsWith(string prefix){
		if (string.IsNullOrEmpty(prefix)){
			return this;
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.StartsWith(prefix)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Filter all words that end with the given string.
	/// </summary>
	/// <param name="suffix"></param>
	/// <returns></returns>
	public WordResults EndsWith(string suffix){
		if (string.IsNullOrEmpty(suffix)){
			return this;
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.EndsWith(suffix)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Filter all words that contain the given string.
	/// </summary>
	/// <param name="substring"></param>
	/// <returns></returns>
	public WordResults Contains(string substring){
		if (string.IsNullOrEmpty(substring)){
			return this;
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.Contains(substring)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Filter all words that are minimum the given length.
	/// </summary>
	/// <param name="length"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public WordResults MinLength(int length){
		if (length <= 0){
			throw new ArgumentException("Length must be greater than or equal to 1");
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.Length >= length){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Filter all words that are maximum the given length.
	/// </summary>
	/// <param name="length"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public WordResults MaxLength(int length){
		if (length <= 0){
			throw new ArgumentException("Length must be greater than or equal to 1");
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.Length <= length){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Filter all words that are exactly the given length.
	/// </summary>
	/// <param name="length"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public WordResults FixedLength(int length){
		if (length <= 0){
			throw new ArgumentException("Length must be greater than or equal to 1");
		}
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.Length == length){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Filter with a custom function.
	/// </summary>
	/// <param name="predicate"></param>
	/// <returns></returns>
	public WordResults FilterCustom(Func<string, bool> predicate){
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (predicate(word)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Sort the words in the result alphabetically.
	/// </summary>
	/// <param name="ascending"></param>
	/// <returns></returns>
	public WordResults SortAlphabetically(bool ascending = true){
		List<string> result = new List<string>(_words);
		result.Sort();
		if (!ascending){
			result.Reverse();
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Sort the words in the result by length.
	/// </summary>
	/// <param name="ascending"></param>
	/// <returns></returns>
	public WordResults SortByLength(bool ascending = true){
		List<string> result = new List<string>(_words);
		result.Sort((a, b) => a.Length.CompareTo(b.Length));
		if (!ascending){
			result.Reverse();
		}

		return new WordResults(result.ToArray());
	}
	/// <summary>
	/// Sort the words by how many times they appear in the given text.
	/// </summary>
	/// <param name="ascending"></param>
	/// <returns></returns>
	public WordResults SortByOccurrence(bool ascending = true){
		Dictionary<string, int> occurrences = new Dictionary<string, int>();
		foreach (string word in _words){
			if (occurrences.ContainsKey(word)){
				occurrences[word]++;
			}
			else{
				occurrences[word] = 1;
			}
		}

		List<string> result = new List<string>(_words);
		result.Sort((a, b) => occurrences[a].CompareTo(occurrences[b]));
		if (!ascending){
			result.Reverse();
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Returns all unique words in the result.
	/// </summary>
	/// <returns></returns>
	public WordResults Distinct(){
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (!result.Contains(word)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Reverses the order of the words in the result.
	/// </summary>
	/// <returns></returns>
	public WordResults Reverse(){
		return new WordResults(_words.Reverse().ToArray());
	}
	
	/// <summary>
	/// Transform all words into a hashset.
	/// </summary>
	/// <returns></returns>
	public HashSet<string> ToHashSet() => new HashSet<string>(_words);
	/// <summary>
	/// Transform all words into an array.
	/// </summary>
	/// <returns></returns>
	public string[] ToArray() => _words;
	/// <summary>
	/// Transform all words into a list.
	/// </summary>
	/// <returns></returns>
	public List<string> ToList() => new List<string>(_words);

	/// <summary>
	/// Calculates all words that are in both results.
	/// Example:
	/// { "a", "b", "c" }.Intersect({ "b", "c", "d" }) = { "b", "c" }
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public WordResults Intersect(WordResults other){
		HashSet<string> otherSet = other.ToHashSet();
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (otherSet.Contains(word)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray()).Distinct();
	}
	/// <summary>
	/// Calculates all words that are in either result.
	/// Example:
	/// { "a", "b", "c" }.Union({ "b", "c", "d" }) = { "a", "b", "c", "d" }
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public WordResults Union(WordResults other){
		HashSet<string> otherSet = other.ToHashSet();
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (!otherSet.Contains(word)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray()).Distinct();
	}
	/// <summary>
	/// Calculates all words that are in this result but not in the other result.
	/// Example:
	/// { "a", "b", "c" }.Except({ "b", "c", "d" }) = { "a" }
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public WordResults Except(WordResults other){
		HashSet<string> otherSet = other.ToHashSet();
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (!otherSet.Contains(word)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray()).Distinct();
	}
	
	/// <summary>
	/// Applies a function to all words in the result.
	/// </summary>
	/// <param name="mapper"></param>
	/// <returns></returns>
	public WordResults Map(Func<string, string> mapper){
		List<string> result = new List<string>();
		foreach (string word in _words){
			result.Add(mapper(word));
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Filters words that contain a letter
	/// </summary>
	/// <param name="letter"></param>
	/// <returns></returns>
	public WordResults WithLetter(char letter){
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (word.Contains(letter)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Filters words that don't contain a letter
	/// </summary>
	/// <param name="letter"></param>
	/// <returns></returns>
	public WordResults WithoutLetter(char letter){
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (!word.Contains(letter)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Filters words that don't repeat any letters.
	/// </summary>
	/// <returns></returns>
	public WordResults WithUniqueLetters(){
		List<string> result = new List<string>();
		foreach (string word in _words){
			HashSet<char> letters = new HashSet<char>();
			foreach (char letter in word){
				if (letters.Contains(letter)){
					continue;
				}
				letters.Add(letter);
			}
			
			if (letters.Count == word.Length){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Filters words that contain atleast one letter repetition.
	/// </summary>
	/// <returns></returns>
	public WordResults WithoutUniqueLetters(){
		List<string> result = new List<string>();
		foreach (string word in _words){
			HashSet<char> letters = new HashSet<char>();
			foreach (char letter in word){
				if (letters.Contains(letter)){
					result.Add(word);
					break;
				}
				letters.Add(letter);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Match letters with a pattern.
	/// Use * for any letter.
	/// Use ? for any vowel.
	/// Use ! for any consonant.
	/// </summary>
	/// <param name="pattern"></param>
	/// <returns></returns>
	public WordResults MatchPattern(string pattern){
		// example pattern
		// "c*t" -> * means any letter, here valid words are "cat", "cot", "cut"
		
		// ? means any vowel
		// "c?t" -> valid words are "cat", "cot"
		
		// ! means any consonant
		// "c!t" -> valid words are "cat", "cut"
		
		// split pattern into letters
		char[] patternLetters = pattern.ToCharArray();
		
		// filter words that match the pattern
		List<string> result = new List<string>();
		
		foreach (string word in _words){
			// if word is too short, skip
			if (word.Length < patternLetters.Length){
				continue;
			}
			
			// if word is too long, skip
			if (word.Length > patternLetters.Length){
				continue;
			}
			
			// check if word matches pattern
			bool matches = true;
			
			for (int i = 0; i < patternLetters.Length; i++){
				char patternLetter = patternLetters[i];
				char wordLetter = word[i];
				
				if (patternLetter == '*'){
					// any letter
					continue;
				}
				
				if (patternLetter == '?'){
					// any vowel
					if (WordLibrary.IsVowel(wordLetter)){
						continue;
					}
				}
				
				if (patternLetter == '!'){
					// any consonant
					if (!WordLibrary.IsVowel(wordLetter)){
						continue;
					}
				}
				
				if (patternLetter == wordLetter){
					// exact match
					continue;
				}
				
				// no match
				matches = false;
				break;
			}
			
			if (matches){
				result.Add(word);
			}
		}
		
		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Filters words that match a regex pattern.
	/// </summary>
	/// <param name="regex"></param>
	/// <returns></returns>
	public WordResults MatchRegex(string regex){
		List<string> result = new List<string>();
		foreach (string word in _words){
			if (Regex.IsMatch(word, regex)){
				result.Add(word);
			}
		}

		return new WordResults(result.ToArray());
	}

	/// <summary>
	/// Returns random words.
	/// </summary>
	/// <param name="amount">Amount of words to return.</param>
	/// <returns></returns>
	public WordResults Random(int amount){
		List<string> result = new List<string>();
		List<string> words = new List<string>(_words);
		
		for (int i = 0; i < amount; i++){
			if (words.Count == 0){
				break;
			}
			
			int index = WordLibrary.rand.Next(words.Count);
			result.Add(words[index]);
			words.RemoveAt(index);
		}

		return new WordResults(result.ToArray());
	}
	
	/// <summary>
	/// Save current set of words to a file.
	/// </summary>
	/// <param name="path"></param>
	public void Save(string path){
		string text = string.Join(Environment.NewLine, _words);
		File.WriteAllText(path, text);
	}
}
