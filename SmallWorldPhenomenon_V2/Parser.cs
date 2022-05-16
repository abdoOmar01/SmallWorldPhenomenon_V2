namespace SmallWorldPhenomenon_V2
{
    static class Parser
    {
        public static void ReadMovieFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] items = line.Split('/');
                string movie = items[0];

                for(int i = 1; i < items.Length; i++)
                {
                    string actor = items[i];
                    Graph.Initialize(actor, movie);

                    for (int j = i - 1; j >= 1; j--)
                    {
                        Graph.AddAdjacent(actor, items[j]);
                    }
                }
            }
        }

        public static void ReadQueryFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            foreach(string line in lines)
            {
                string[] query = line.Split('/');
                Graph.ParseQuery(query[0], query[1]);
            }
        }
    }
}