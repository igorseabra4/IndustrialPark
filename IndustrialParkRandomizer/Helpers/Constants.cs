namespace IndustrialPark.Randomizer
{
    public static class Constants
    {
        public const string probExample = "Example: say Wooden, Thunder, Shhh and Stone Tiki probabilities are set to 1, 3, 0 and 2 respectively. Add up the numbers and the total is 6. Tikis have 1/6 probability of being Wooden, 3/6 Thunder and 2/6 Stone. There will be no Shhh Tikis. This applies similarly to enemies. Setting the value to -1 makes the randomizer ignore all existing instances of that tiki/enemy completely (doesn't replace them).";
        public const string minMaxExample = "A random value between minimum and maximum is chosen for each property.";
        public const string posHelpExample = "All position helpers are randomized together. Markers must be on for all of them to work. Note that this is the step which takes longest to perform; disable it if you want a faster randomization.";
        public const string powerupExample = "Begin game with powerup.";
    }
}