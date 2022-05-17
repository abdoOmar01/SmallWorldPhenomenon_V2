namespace SmallWorldPhenomenon_V2
{
    class Program
    {
        public static void Main(string[] args)
        {
            Parser.ReadMovieFile(Paths.SAMPLE);
            Parser.ReadQueryFile(Paths.SAMPLE_QUERY);
            //Graph.GetMovieMap();
            Graph.GetActorMap();
        }
    }
}