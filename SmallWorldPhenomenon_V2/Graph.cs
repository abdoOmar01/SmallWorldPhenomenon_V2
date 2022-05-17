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

            Console.Write("Nodes: ");
            foreach(KeyValuePair<int, int> pair in level)
            {
                Console.Write($"({numberEncoding[pair.Key]},{pair.Value}) ");
            }
            Console.WriteLine();


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
                int parents = level.Where(p => p.Value == currentLevel - 1 && actorMap[p.Key].Contains(path[index])).Count();
                if (pair.Value == currentLevel - 1 && actorMap[pair.Key].Contains(path[index]))
                {
                    path.Add(pair.Key);
                    if (parents > 1)
                    {
                        level.Remove(pair.Key);
                    }
                    currentLevel = pair.Value;
                    index++;
                }
                parents = 0;
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