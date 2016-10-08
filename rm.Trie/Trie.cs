﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rm.Trie
{
    /// <summary>
    /// Trie data structure.
    /// </summary>
    internal class Trie : ITrie
    {
        #region Data Members

        /// <summary>
        /// Root TrieNode.
        /// </summary>
        private TrieNode rootTrieNode { get; set; }

        #endregion

        #region Ctors

        /// <summary>
        /// Create a new Trie instance.
        /// </summary>
        /// <param name="rootTrieNode">The root TrieNode.</param>
        internal Trie(TrieNode rootTrieNode)
        {
            this.rootTrieNode = rootTrieNode;
        }

        #endregion

        #region ITrie methods

        /// <summary>
        /// Add a word to the Trie.
        /// </summary>
        public void AddWord(string word)
        {
            AddWord(rootTrieNode, word.ToCharArray());
        }

        /// <summary>
        /// Remove word from the Trie.
        /// </summary>
        public void RemoveWord(string word)
        {
            RemoveWord(GetTrieNodesStack(word));
        }

        /// <summary>
        /// Remove words by prefix from the Trie.
        /// </summary>
        public void RemovePrefix(string prefix)
        {
            var trieNodes = GetTrieNodesStack(prefix, false);
            // Clear last trieNode, no need to remove unneeded nodes
            if (trieNodes.Any())
            {
                trieNodes.Pop().Clear();
            }
        }

        /// <summary>
        /// Get all words in the Trie.
        /// </summary>
        public ICollection<string> GetWords()
        {
            var words = new List<string>();
            var buffer = new StringBuilder();
            GetWords(rootTrieNode, words, buffer);
            return words;
        }

        /// <summary>
        /// Get words for given prefix.
        /// </summary>
        public ICollection<string> GetWords(string prefix)
        {
            // Empty list if no prefix match
            var words = new List<string>();
            var trieNode = GetTrieNode(prefix);
            if (trieNode != null)
            {
                var buffer = new StringBuilder();
                buffer.Append(prefix);
                GetWords(trieNode, words, buffer);
            }
            return words;
        }

        /// <summary>
        /// Returns true or false if the word is present in the Trie.
        /// </summary>
        public bool HasWord(string word)
        {
            var trieNode = GetTrieNode(word);
            return trieNode != null && trieNode.IsWord;
        }

        /// <summary>
        /// Returns the count for the word in the Trie.
        /// </summary>
        public int WordCount(string word)
        {
            var trieNode = GetTrieNode(word);
            return (trieNode == null ? 0 : trieNode.WordCount);
        }

        /// <summary>
        /// Get longest words from the Trie.
        /// </summary>
        public ICollection<string> GetLongestWords()
        {
            var longestWords = new List<string>();
            var buffer = new StringBuilder();
            var length = new Wrapped<int>(0);
            GetLongestWords(rootTrieNode, longestWords, buffer, length);
            return longestWords;
        }

        /// <summary>
        /// Get shortest words from the Trie.
        /// </summary>
        public ICollection<string> GetShortestWords()
        {
            var shortestWords = new List<string>();
            var buffer = new StringBuilder();
            var length = new Wrapped<int>(int.MaxValue);
            GetShortestWords(rootTrieNode, shortestWords, buffer, length);
            return shortestWords;
        }

        /// <summary>
        /// Clear all words from the Trie.
        /// </summary>
        public void Clear()
        {
            rootTrieNode.Clear();
        }

        /// <summary>
        /// Get total word count in the Trie.
        /// </summary>
        public int Count()
        {
            var count = new Wrapped<int>(0);
            GetCount(rootTrieNode, count, false);
            return count.Value;
        }

        /// <summary>
        /// Get unique word count in the Trie.
        /// </summary>
        public int UniqueCount()
        {
            var count = new Wrapped<int>(0);
            GetCount(rootTrieNode, count, true);
            return count.Value;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get the equivalent TrieNode in the Trie for given prefix. 
        /// If prefix not present, then return null.
        /// </summary>
        private TrieNode GetTrieNode(string prefix)
        {
            var trieNode = rootTrieNode;
            foreach (var prefixChar in prefix)
            {
                trieNode = trieNode.GetChild(prefixChar);
                if (trieNode == null)
                {
                    break;
                }
            }
            return trieNode;
        }

        /// <summary>
        /// Recursive method to add word. Gets the first char of the word, 
        /// creates the child TrieNode if null, and recurses with the first
        /// char removed from the word. If the word length is 0, return.
        /// </summary>
        private void AddWord(TrieNode trieNode, char[] word)
        {
            foreach (var c in word)
            {
                var child = trieNode.GetChild(c);
                if (child == null)
                {
                    child = TrieFactory.CreateTrieNode(c);
                    trieNode.SetChild(child);
                }
                trieNode = child;
            }
            trieNode.WordCount++;
        }

        /// <summary>
        /// Recursive method to get all the words starting from given TrieNode.
        /// </summary>
        private void GetWords(TrieNode trieNode, ICollection<string> words,
            StringBuilder buffer)
        {
            if (trieNode.IsWord)
            {
                words.Add(buffer.ToString());
            }
            foreach (var child in trieNode.GetChildren())
            {
                buffer.Append(child.Character);
                GetWords(child, words, buffer);
                // Remove recent character
                buffer.Length--;
            }
        }

        /// <summary>
        /// Recursive method to get longest words starting from given TrieNode.
        /// </summary>
        private void GetLongestWords(TrieNode trieNode,
            ICollection<string> longestWords, StringBuilder buffer, Wrapped<int> length)
        {
            if (trieNode.IsWord)
            {
                if (buffer.Length > length.Value)
                {
                    longestWords.Clear();
                    length.Value = buffer.Length;
                }
                if (buffer.Length == length.Value)
                {
                    longestWords.Add(buffer.ToString());
                }
            }
            foreach (var child in trieNode.GetChildren())
            {
                buffer.Append(child.Character);
                GetLongestWords(child, longestWords, buffer, length);
                // Remove recent character
                buffer.Length--;
            }
        }

        /// <summary>
        /// Recursive method to get shortest words starting from given TrieNode.
        /// </summary>
        private void GetShortestWords(TrieNode trieNode,
            ICollection<string> shortestWords, StringBuilder buffer, Wrapped<int> length)
        {
            if (trieNode.IsWord)
            {
                if (buffer.Length < length.Value)
                {
                    shortestWords.Clear();
                    length.Value = buffer.Length;
                }
                if (buffer.Length == length.Value)
                {
                    shortestWords.Add(buffer.ToString());
                }
            }
            foreach (var child in trieNode.GetChildren())
            {
                buffer.Append(child.Character);
                GetShortestWords(child, shortestWords, buffer, length);
                // Remove recent character
                buffer.Length--;
            }
        }

        /// <summary>
        /// Get stack of trieNodes for given string.
        /// </summary>
        private Stack<TrieNode> GetTrieNodesStack(string s, bool isWord = true)
        {
            var nodes = new Stack<TrieNode>(s.Length + 1);
            var trieNode = rootTrieNode;
            nodes.Push(trieNode);
            foreach (var c in s)
            {
                trieNode = trieNode.GetChild(c);
                if (trieNode == null)
                {
                    nodes.Clear();
                    break;
                }
                nodes.Push(trieNode);
            }
            if (isWord)
            {
                if (trieNode == null || !trieNode.IsWord)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("{0} does not exist in trie.", s)
                        );
                }
            }
            return nodes;
        }

        /// <summary>
        /// Remove unneeded nodes except root from stack of trieNodes.
        /// </summary>
        private void RemoveWord(Stack<TrieNode> trieNodes)
        {
            // Mark the last trieNode as not a word
            trieNodes.Peek().WordCount = 0;
            while (trieNodes.Count > 1)
            {
                var trieNode = trieNodes.Pop();
                var parentTrieNode = trieNodes.Peek();
                if (trieNode.IsWord || trieNode.GetChildren().Any())
                {
                    break;
                }
                parentTrieNode.RemoveChild(trieNode.Character);
            }
        }

        /// <summary>
        /// Get word count in the Trie.
        /// </summary>
        private void GetCount(TrieNode trieNode, Wrapped<int> count, bool isUnique)
        {
            if (trieNode.IsWord)
            {
                count.Value += isUnique ? 1 : trieNode.WordCount;
            }
            foreach (var child in trieNode.GetChildren())
            {
                GetCount(child, count, isUnique);
            }
        }

        #endregion
    }
}