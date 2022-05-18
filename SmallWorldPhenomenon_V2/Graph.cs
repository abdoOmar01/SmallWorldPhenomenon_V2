namespace SmallWorldPhenomenon_V2
{
    static class Graph
    {
        private static Dictionary<int, List<string>> movieMap = new Dictionary<int, List<string>>();
        private static Dictionary<int, HashSet<int>> actorMap = new Dictionary<int, HashSet<int>>();

        private static Dictionary<string, int> actorEncoding = new Dictionary<string, int>();
        private static Dictionary<int, string> numberEncoding = new Dictionary<int, string>();
        private static int counter = 0;

        public static void Initialize(string actor, string movie)
        {
            if (!actorEncoding.ContainsKey(actor))
            {
                actorEncoding.Add(actor, counter);
                numberEncoding.Add(counter++, actor);

                if (!movieMap.ContainsKey(actorEncoding[actor]))
                {
                    movieMap.Add(actorEncoding[actor], new List<string>());
                    actorMap.Add(actorEncoding[actor], new HashSet<int>());
                }
            }

            movieMap[actorEncoding[actor]].Add(movie);
        }

        public static void AddAdjacent(string actor, string otherActor)
        {
            actorMap[actorEncoding[actor]].Add(actorEncoding[otherActor]);
            actorMap[actorEncoding[otherActor]].Add(actorEncoding[actor]);
        }

        public static void ParseQuery(string src, string dst)
        {
            Console.WriteLine($"{src} --> {dst}");

            Dictionary<int, int> level = GetDegreeOfSeparation(src, dst);

            Console.WriteLine($"Degree of Separation: {level.Last().Value}");
            HashSet<List<int>> paths = GetAllPaths(level);

            List<List<string>> chainsOfMovies = GetRelationStrength(paths);

            List<string> shortestChain = FindShortestChainOfMovies(chainsOfMovies);
            PrintShortestChainOfMovies(shortestChain);
        }

        private static Dictionary<int, int> GetDegreeOfSeparation(string src, string dst)
        {
            int srcInt = actorEncoding[src];
            int dstInt = actorEncoding[dst];

            Queue<int> nodes = new Queue<int>();
            bool[] visited = new bool[actorMap.Count];

            nodes.Enqueue(srcInt);
            visited[srcInt] = true;

            Dictionary<int, int> level = new Dictionary<int, int>();
            level.Add(srcInt, 0);

            while (nodes.Count != 0)
            {
                int node = nodes.Dequeue();
                
                foreach (int actor in actorMap[node])
                {
                    if (!visited[actor])
                    {
                        visited[actor] = true;
                        level.Add(actor, level[node] + 1);

                        if (actor == dstInt)
                        {
                            return level;
                        }

                        nodes.Enqueue(actor);
                    }
                }
            }

            return level;
        }

        private static List<int> GetPath(Dictionary<int, int> level)
        {
            List<int> path = new List<int>();

            path.Add(level.Last().Key);
            int index = 0;
            int currentLevel = level.Last().Value;

            foreach (KeyValuePair<int, int> pair in level.Reverse())
            {
                if (actorMap[pair.Key].Contains(path[index]))
                {
                    if (pair.Value == currentLevel - 1)
                    {
                        path.Add(pair.Key);
                        currentLevel = pair.Value;
                        index++;
                        continue;
                    }

                    else if (pair.Value == currentLevel && index != 0)
                    {
                        if (actorMap[pair.Key].Contains(path[index - 1]))
                        {
                            foreach (int p in path)
                            {
                                if (level.ContainsKey(p) && level[p] == pair.Value)
                                {
                                    level.Remove(p);
                                    break;
                                }
                            }

                        }
                    }
                }
            }

            path.Reverse();

            return path;
        }

        private static HashSet<List<int>> GetAllPaths(Dictionary<int, int> level)
        {
            //GetPath is called multiple times until it runs out of unique paths

            HashSet<List<int>> paths = new HashSet<List<int>>();

            List<int> firstPath = GetPath(level);
            paths.Add(firstPath);

            List<int> newPath = GetPath(level);
            while (!firstPath.SequenceEqual(newPath))
            {
                paths.Add(newPath);
                firstPath = newPath;
                newPath = GetPath(level);
            }

            return paths;
        }

        private static List<List<string>> GetRelationStrength(HashSet<List<int>> paths)
        {
            int[] strengths = new int[paths.Count];
            List<List<string>> chains = new List<List<string>>();
            int count = 0;

            foreach (List<int> path in paths)
            {
                int total = 0;
                chains.Insert(count, new List<string>());

                for (int i = 0; i < path.Count - 1; i++)
                {
                    int actor = path[i];
                    int otherActor = path[i + 1];

                    List<string> movieList = movieMap[actor];
                    List<string> otherMovieList = movieMap[otherActor];

                    IEnumerable<string> commonMovies = movieList.Intersect(otherMovieList);
                    foreach (string movie in commonMovies)
                    {
                        chains[count].Add(movie);
                    }
                    
                    chains[count].Add("=>");
                    total += commonMovies.Count();
                }   
                strengths[count] = total;
                count++;
            }

            Console.WriteLine($"Relation strength: {strengths.Max()}");

            return chains;
        }

        private static List<string> FindShortestChainOfMovies(List<List<string>> chains)
        {
            // Shortest chain of movies is defined by two attributes
            // 1) The chain with the shallowest depth
            // 2) The chains with the most movies

            // If two chains are found to have the same depth, then the chain with the most movies
            // is selected

            int minLevel = -1;
            int maxMovies = -1;
            int chainIndex = -1;
            for (int j = 0; j < chains.Count; j++)
            {
                int levelCount = 0;
                int movieCount = 0;

                foreach (string c in chains[j])
                {
                    if (c.Equals("=>"))
                    {
                        levelCount++;
                    }
                    else
                    {
                        movieCount++;
                    }
                }

                if (minLevel == -1 || levelCount < minLevel)
                {
                    minLevel = levelCount;
                    chainIndex = j;
                }
                else if (levelCount == minLevel && movieCount > maxMovies)
                {
                    chainIndex = j;
                }

                if (maxMovies == -1 || movieCount > maxMovies)
                {
                    maxMovies = movieCount;
                }
            }

            List<string> shortestChain = chains[chainIndex];

            return shortestChain;
        }
        private static void PrintShortestChainOfMovies(List<string> shortestChain)
        {
            Console.Write("Chain of movies: ");

            int i = 0;
            while (i < shortestChain.Count)
            {
                if (!shortestChain[i].Equals("=>"))
                {
                    Console.Write(shortestChain[i]);
                }

                while (i + 1 < shortestChain.Count)
                {
                    if (shortestChain[i + 1].Equals("=>"))
                    {
                        if (i + 2 < shortestChain.Count)
                        {
                            Console.Write(" => ");
                            Console.Write(shortestChain[i + 2]);
                            i += 2;
                            continue;
                        }
                        break;
                    }
                    Console.Write($" or {shortestChain[i + 1]}");
                    i++;
                }

                i++;
            }

            Console.WriteLine('\n');
        }

        public static void PrintActorMap()
        {
            foreach (KeyValuePair<int, HashSet<int>> pair in actorMap)
            {
                Console.Write($"{numberEncoding[pair.Key]} --> ");
                foreach (int actor in pair.Value)
                {
                    Console.Write($"{numberEncoding[actor]} ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintMovieMap()
        {
            foreach (KeyValuePair<int, List<string>> pair in movieMap)
            {
                Console.Write($"{numberEncoding[pair.Key]} --> ");
                foreach (string movie in pair.Value)
                {
                    Console.Write($"{movie} / ");
                }
                Console.WriteLine();
            }
        }

    }
}