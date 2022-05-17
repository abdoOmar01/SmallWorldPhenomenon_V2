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
            // Doesn't need to check actorMap as it will be the first time accessing it
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

            int str = GetRelationStrength(paths);
            Console.WriteLine($"Relation strength: {str}\n");
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
                
                foreach(int actor in actorMap[node])
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
            HashSet<List<int>> paths = new HashSet<List<int>>();
            List<int> defaultPath = GetPath(level);
            paths.Add(defaultPath);
            List<int> newPath = GetPath(level);
            while (!defaultPath.SequenceEqual(newPath))
            {
                paths.Add(newPath);
                defaultPath = newPath;
                newPath = GetPath(level);
            }

            return paths;
        }

        private static int GetRelationStrength(HashSet<List<int>> paths)
        {
            int[] strengths = new int[paths.Count];
            int count = 0;
            foreach (List<int> path in paths)
            {
                int total = 0;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    int actor = path[i];
                    int otherActor = path[i + 1];

                    List<string> movieList = movieMap[actor];
                    List<string> otherMovieList = movieMap[otherActor];

                    total += movieList.Intersect(otherMovieList).Count();
                }

                strengths[count] = total;
                count++;
            }

            return strengths.Max();
        }

        public static void GetActorMap()
        {
            foreach(KeyValuePair<int, HashSet<int>> pair in actorMap)
            {
                Console.Write($"{numberEncoding[pair.Key]} --> ");
                foreach(int actor in pair.Value)
                {
                    Console.Write($"{numberEncoding[actor]} ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintPaths(HashSet<List<int>> paths)
        {
            foreach(List<int> path in paths)
            {
                Console.Write("Path: ");
                foreach (int p in path)
                {
                    Console.Write(numberEncoding[p] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}