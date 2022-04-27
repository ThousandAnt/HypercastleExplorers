using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Hypercastle.Web
{
    [FunctionOutput]
    public class MaxSupplyDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "")]
        public virtual BigInteger MaxSupply { get; set; }
    }
    
    [FunctionOutput]
    public class TokenScaleDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "")]
        public virtual BigInteger TokenScale { get; set; }
    }
    
    [FunctionOutput]
    public class DreamersDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "")]
        public virtual BigInteger Dreamers { get; set; }
    }

    [FunctionOutput]
    public class NameDTO : IFunctionOutputDTO
    {
        [Parameter("string", "")]
        public virtual string Name { get; set; }
    }
    
    [FunctionOutput]
    public class OwnerDTO : IFunctionOutputDTO
    {
        [Parameter("address", "")]
        public virtual string Owner { get; set; }
    }
    
    [FunctionOutput]
    public class OwnerOfDTO : IFunctionOutputDTO
    {
        [Parameter("address", "")]
        public virtual string OwnerOf { get; set; }
    }
    
    [FunctionOutput]
    public class SeedDTO : IFunctionOutputDTO
    {
        [Parameter("uint256", "")]
        public virtual BigInteger Seed { get; set; }
    }
    
    [FunctionOutput]
    public class StructureDataTupleDTO : IFunctionOutputDTO
    {
        [Parameter("tuple[20]","structure",1,"struct Terraforms.StructureLevel[20]")]
        public virtual List<StructureDataDTO> StructureLevel { get; set; }
        
        public override string ToString()
        {
            var data = "\n";
            foreach (var item in StructureLevel)
            {
                data += $"{item}\n";
            }
            return data;
        }
    }

    [FunctionOutput]
    public class StructureDataDTO: IFunctionOutputDTO
    {
        [Parameter("uint256","levelNumber")]
        public virtual BigInteger LevelNumber { get; set; }
        
        [Parameter("uint256","tokensOnLevel", 2)]
        public virtual BigInteger TokensOnLevel { get; set; }
        
        [Parameter("int256","structureSpaceX", 3)]
        public virtual BigInteger StructureSpaceX { get; set; }
        
        [Parameter("int256","structureSpaceY", 4)]
        public virtual BigInteger StructureSpaceY { get; set; }
        
        [Parameter("int256","structureSpaceZ", 5)]
        public virtual BigInteger StructureSpaceZ { get; set; }

        public override string ToString()
        {
            var data = $"Level Number: {LevelNumber} Tokens On Level: {TokensOnLevel} ";
            data += $"SpaceVector: x: {StructureSpaceX} y: {StructureSpaceY} z: {StructureSpaceZ}";
            return data;
        }
    }

    [FunctionOutput]
    public class SymbolDTO : IFunctionOutputDTO
    {
        [Parameter("string","")]
        public virtual string Symbol { get; set; }
    }
    
    [FunctionOutput]
    public class TokenByIndexDTO : IFunctionOutputDTO
    {
        [Parameter("uint256","")]
        public virtual BigInteger TokenIndex { get; set; }
    }

    [FunctionOutput]
    public class TokenCharactersDTO : IFunctionOutputDTO
    {
        [Parameter("string[32][32]","")]
        public virtual List<List<string>> TokenCharacters { get; set; }

        public override string ToString()
        {
            var str = "\n";
            foreach (var list in TokenCharacters)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];
                    str += $"{item} ";
                    if (index == list.Count - 1)
                        str += "\n";
                }
            }

            return str;
        }
    }

    [FunctionOutput]
    public class TokenCounterDTO : IFunctionOutputDTO
    {
        [Parameter("uint256","")]
        public virtual BigInteger TokenCount { get; set; }
    }
    
    [FunctionOutput]
    public class TokenHTMLDTO: IFunctionOutputDTO
    {
        [Parameter("string","result")]
        public virtual string TokenHTML { get; set; }
    }
    
    [FunctionOutput]
    public class TokenHeightmapIndicesDTO: IFunctionOutputDTO
    {
        [Parameter("uint[32][32]","")]
        public virtual List<List<BigInteger>> HeighmapIndices { get; set; }
        public override string ToString()
        {
            var str = "\n";
            foreach (var list in HeighmapIndices)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];
                    str += $"{item} ";
                    if (index == list.Count - 1)
                        str += "\n";
                }
            }

            return str;
        }
    }

    [FunctionOutput]
    public class TokenDataTupleDTO : IFunctionOutputDTO
    {
        [Parameter("tuple","return",1,"struct Terraforms.TokenData")]
        public virtual TokenDataDTO tokenData { get; set; }
    }

    [FunctionOutput]
    public class TokenDataDTO: IFunctionOutputDTO
    {
        [Parameter("uint256","tokenId")]
        public virtual BigInteger TokenId { get; set; }
        [Parameter("uint256","level", 2)]
        public virtual BigInteger Level { get; set; }
        [Parameter("uint256","xCoordinate", 3)]
        public virtual BigInteger XCoordinate { get; set; }
        [Parameter("uint256","yCoordinate", 4)]
        public virtual BigInteger YCoordinate { get; set; }
        [Parameter("int256","elevation", 5)]
        public virtual BigInteger Elevation { get; set; }
        [Parameter("int256","structureSpaceX", 6)]
        public virtual BigInteger StructureSpaceX { get; set; }
        [Parameter("int256","structureSpaceY", 7)]
        public virtual BigInteger StructureSpaceY { get; set; }
        [Parameter("int256","structureSpaceZ", 8)]
        public virtual BigInteger StructureSpaceZ { get; set; }
        [Parameter("string","zoneName", 9)]
        public virtual string ZoneName { get; set; }
        [Parameter("string[10]","zoneColors", 10)]
        public virtual List<string> ZoneColors { get; set; }
        [Parameter("string[9]","characterSet", 11)]
        public virtual List<string> CharacterSet { get; set; }

        public override string ToString()
        {
            var data = $"{Level}\n{XCoordinate}\n{YCoordinate}\n{Elevation}\n{StructureSpaceX}\n{StructureSpaceY}\n";
            data += $"{StructureSpaceZ}\n{ZoneName}\n";

            foreach (var element in ZoneColors)
            {
                data += $"{element}\n";
            }

            foreach (var element in CharacterSet)
            {
                data += $"{element}\n";
            }

            return data;
        }
    }
    
    [FunctionOutput]
    public class TokenTerrainValuesDTO : IFunctionOutputDTO
    {
        [Parameter("int256[32][32]","")]
        public virtual List<List<BigInteger>> TokenTerrainValues { get; set; }

        public override string ToString()
        {
            var str = "\n";
            foreach (var list in TokenTerrainValues)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];
                    str += $"{item} ";
                    if (index == list.Count - 1)
                        str += "\n";
                }
            }
            return str;
        }
    }
    
    [FunctionOutput]
    public class TokenSVGDTO : IFunctionOutputDTO
    {
        [Parameter("string","")]
        public virtual string TokenSVG { get; set; }
    }
    
    [FunctionOutput]
    public class TokenURIDTO : IFunctionOutputDTO
    {
        [Parameter("string","result")]
        public virtual string TokenURI { get; set; }
    }
    
    [FunctionOutput]
    public class TotalSupplyDTO : IFunctionOutputDTO
    {
        [Parameter("uint256","")]
        public virtual BigInteger TotalSupply { get; set; }
    }
}
