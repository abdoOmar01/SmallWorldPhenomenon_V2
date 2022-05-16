namespace SmallWorldPhenomenon_V2
{
    static class Graph
    {
        private static Dictionary<string, List<string>> movieMap = new Dictionary<string, List<string>>();
        private static Dictionary<string, HashSet<string>> actorMap = new Dictionary<string, HashSet<string>>();
        public static void Initialize(string actor, string movie)
        {
            // Doesn't need to check actorMap as it will be the first time accessing it
            if (!movieMap.ContainsKey(actor))
            {
                movieMap.Add(actor, new List<string>());
                actorMap.Add(actor, new HashSet<string>());
            }
            movieMap[actor].Add(movie);
        }

        public static void AddAdjacent(string actor, string otherActor)
        {
            actorMap[actor].Add(otherActor);
            actorMap[otherActor].Add(actor);
        }

        public static void ParseQuery(string src, string dst)
        {
            //Console.WriteLine($"{src} --> {dst} = {GetDegreeOfSeparation(src, dst)}");
            GetDegreeOfSeparation(src, dst);
        }

        private static int GetDegreeOfSeparation(string src, string dst)
        {
            Queue<string> nodes = new Queue<string>();
            List<string> visited = new List<string>();

            nodes.Enqueue(src);
            visited.Add(src);

            Dictionary<string, int> level = new Dictionary<string, int>();
            level.Add(src, 0);

            while (nodes.Count != 0)
            {
                string node = nodes.Dequeue();
                
                foreach(string actor in actorMap[node])
                {
                    if (!visited.Contains(actor))
                    {
                        visited.Add(actor);
                        level.Add(actor, level[node] + 1);

                        if (actor.Equals(dst))
                        {
                            return level[actor];
                        }

                        nodes.Enqueue(actor);
                    }
                }
            }
            return -1;
        }

        public static void GetMovieMap()
        {
            foreach (string actor in movieMap.Keys)
            {
                Console.Write($"{actor} --> ");
                foreach (string movie in movieMap[actor])
                {
                    Console.Write($"{movie} ");
                }
                Console.WriteLine();
            }
        }

        public static void GetActorMap()
        {
            foreach (string actor in actorMap.Keys)
            {
                Console.Write($"{actor} --> ");
                foreach (string otherActor in actorMap[actor])
                {
                    Console.Write($"{otherActor} ");
                }
                Console.WriteLine();
            }
        }
    }
}
