namespace GSEditor.Core.PokegoldCore;

public sealed class PGPokemon
{
    private static readonly byte[] reverseBits = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };
    private static readonly Dictionary<byte, int> _genderRates = new()
    {
        [0x00] = 0,
        [0x1f] = 1,
        [0x3f] = 2,
        [0x5f] = 3,
        [0x7f] = 4,
        [0x9f] = 5,
        [0xbf] = 6,
        [0xdf] = 7,
        [0xfe] = 8,
        [0xff] = 9,
    };
    private static readonly Dictionary<byte, int> _growthRates = new()
    {
        [0] = 5,
        [1] = 3,
        [2] = 2,
        [3] = 1,
        [4] = 0,
        [5] = 4,
    };

    private readonly bool[] _tmhms = new bool[64];
    private readonly List<PGEvolution> _evolutions = new();
    private readonly List<PGLearnMove> _learnMoves = new();

    public byte No { get; set; }
    public byte HP { get; set; }
    public byte Attack { get; set; }
    public byte Defence { get; set; }
    public byte Speed { get; set; }
    public byte SpAttack { get; set; }
    public byte SpDefence { get; set; }
    public byte Type1 { get; set; }
    public byte Type2 { get; set; }
    public byte CatchRate { get; set; }
    public byte Exp { get; set; }
    public byte Item1 { get; set; }
    public byte Item2 { get; set; }
    public byte GenderRate { get; set; }
    public byte Unk1 { get; set; }
    public byte EggType { get; set; }
    public byte Unk2 { get; set; }
    public byte ImageSizeType { get; set; }
    public byte Padding1 { get; set; }
    public byte Padding2 { get; set; }
    public byte Padding3 { get; set; }
    public byte Padding4 { get; set; }
    public byte GrowthRate { get; set; }
    public byte EggGroup { get; set; }

    public bool[] TMHMs => _tmhms;

    public List<PGEvolution> Evolutions => _evolutions;
    public List<PGLearnMove> LearnMoves => _learnMoves;

    public static PGPokemon FromBytes(byte[] bytes)
    {
        var newItem = new PGPokemon
        {
            No = bytes[0],
            HP = bytes[1],
            Attack = bytes[2],
            Defence = bytes[3],
            Speed = bytes[4],
            SpAttack = bytes[5],
            SpDefence = bytes[6],
            Type1 = bytes[7],
            Type2 = bytes[8],
            CatchRate = bytes[9],
            Exp = bytes[10],
            Item1 = bytes[11],
            Item2 = bytes[12],
            GenderRate = bytes[13],
            Unk1 = bytes[14],
            EggType = bytes[15],
            Unk2 = bytes[16],
            ImageSizeType = bytes[17],
            Padding1 = bytes[18],
            Padding2 = bytes[19],
            Padding3 = bytes[20],
            Padding4 = bytes[21],
            GrowthRate = bytes[22],
            EggGroup = bytes[23],
        };

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var index = (i * 8) + j;
                newItem.TMHMs[index] = (bytes[24 + i] & reverseBits[j]) != 0;
            }
        }

        return newItem;
    }

    public byte[] ToBytes()
    {
        var bytes = new byte[] {
      No,
      HP,
      Attack,
      Defence,
      Speed,
      SpAttack,
      SpDefence,
      Type1,
      Type2,
      CatchRate,
      Exp,
      Item1,
      Item2,
      GenderRate,
      Unk1,
      EggType,
      Unk2,
      ImageSizeType,
      Padding1,
      Padding2,
      Padding3,
      Padding4,
      GrowthRate,
      EggGroup,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
    };

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                if (TMHMs[(i * 8) + j])
                    bytes[24 + i] = (byte)(bytes[24 + i] | reverseBits[j]);
            }
        }

        return bytes;
    }

    public int GetImageTileSize()
    {
        return ImageSizeType switch
        {
            0x55 => 5,
            0x66 => 6,
            0x77 => 7,
            _ => 5,
        };
    }

    public void SetImageTileSize(int size)
    {
        ImageSizeType = size switch
        {
            5 => 0x55,
            6 => 0x66,
            7 => 0x77,
            _ => 0x55,
        };
    }

    public int GetGenderRateType()
    {
        return _genderRates[GenderRate];
    }

    public void SetGenderRateType(int index)
    {
        foreach (var key in _genderRates.Keys)
        {
            var e = _genderRates[key];
            if (e == index)
            {
                GenderRate = key;
                break;
            }
        }
    }

    public int GetGrowthRateType()
    {
        return _growthRates[GrowthRate];
    }

    public void SetGrowthRateType(int index)
    {
        foreach (var key in _growthRates.Keys)
        {
            var e = _growthRates[key];
            if (e == index)
            {
                GrowthRate = key;
                break;
            }
        }
    }
}
