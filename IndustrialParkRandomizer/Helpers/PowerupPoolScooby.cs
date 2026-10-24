using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class PowerupPoolScooby
    {
        public int Shovel { get; set; }
        public int Springs { get; set; }
        public int Slippers { get; set; }
        public int Lampshade { get; set; }
        public int Helmet { get; set; }
        public int Knight_Helmet { get; set; }
        public int Boots { get; set; }
        public int Super_Smash { get; set; }
        public int Stun_Smash { get; set; }
        public int Plungers { get; set; }
        public int Umbrella { get; set; }
        public int Gum_Machine { get; set; }
        public int Soap_Bubble { get; set; }
        public int Monster_Token { get; set; }
        public int Gum_Pack { get; set; }
        public int Soap_Pack { get; set; }

        public void Reset()
        {
            Shovel = 1;
            Springs = 1;
            Slippers = 1;
            Lampshade = 1;
            Helmet = 1;
            Knight_Helmet = 1;
            Boots = 1;
            Super_Smash = 1;
            Stun_Smash = 1;
            Plungers = 1;
            Umbrella = 1;
            Gum_Machine = 1;
            Soap_Bubble = 1;
            Monster_Token = 21;
            Gum_Pack = 6;
            Soap_Pack = 7;
        }

        public void Standardize()
        {
            var other = new PowerupPoolScooby();
            Shovel -= other.Shovel;
            Springs -= other.Springs;
            Slippers -= other.Slippers;
            Lampshade -= other.Lampshade;
            Helmet -= other.Helmet;
            Knight_Helmet -= other.Knight_Helmet;
            Boots -= other.Boots;
            Super_Smash -= other.Super_Smash;
            Stun_Smash -= other.Stun_Smash;
            Plungers -= other.Plungers;
            Umbrella -= other.Umbrella;
            Gum_Machine -= other.Gum_Machine;
            Soap_Bubble -= other.Soap_Bubble;
            Monster_Token -= other.Monster_Token;
            Gum_Pack -= other.Gum_Pack;
            Soap_Pack -= other.Soap_Pack;
        }
    }
}