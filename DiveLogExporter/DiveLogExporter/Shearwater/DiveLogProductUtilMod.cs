using DiveLogModels;
using ExtendedCoreParserUtilities;

namespace Shearwater
{
    public class DiveLogProductUtilMod
    {

        static DiveLogProductUtilMod()
        {
            DiveLogProductUtil.RetrieveFriendlyProductName = (int productId) =>
            {
                return productId switch
                {
                    0 => "GF",
                    1 => "Pursuit",
                    2 => "Predator",
                    3 => "Petrel",
                    4 => "NERD",
                    5 => "Perdix",
                    6 => "Perdix AI",
                    7 => "NERD 2",
                    8 => "Teric",
                    9 => "Peregrine",
                    10 => "Petrel 3",
                    11 => "Perdix 2",
                    12 => "Tern",
                    13 => "Peregrine TX",
                    _ => "Unknown",
                };
            };

        }

        public static string FriendlyProductName(FinalLog finalLog)
        {
            return DiveLogProductUtil.FriendlyProductName(finalLog);
        }
    }
}
