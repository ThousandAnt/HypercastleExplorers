using System;
using Hypercastle.Web;
using NUnit.Framework;
using System.IO;
using NBitcoin;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using Application = UnityEngine.Application;
using Assert = NUnit.Framework.Assert;

namespace Hypercastle.Tests
{
    public partial class ContractTests
    {
        private HypercastleClient _client;
        private ConfigurationSecrets _config;
        private readonly int TOKEN_ID = 8057;
        private readonly ulong TIMESTAMP = (ulong)new DateTime(2222,2,22,2,22,22).ToUnixTimestamp();

        [SetUp]
        public void Setup()
        {
            _client = new HypercastleClient();
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (_config != null) return;
            var path = Path.Combine(Application.streamingAssetsPath, "secrets.json");
            var jsonAsset = File.ReadAllText(path);
            _config = JsonUtility.FromJson<ConfigurationSecrets>(jsonAsset);
        }

        [Test]
        public void ConfigurationsLoadedTest()
        {
            Assert.NotNull(_client.Configs);
            Assert.AreEqual(_config.EtherscanToken, _client.Configs.EtherscanToken);
            Assert.AreEqual(_config.InfuraProjectId, _client.Configs.InfuraProjectId);
        }

        [Test]
        public void GetAbiTest()
        {
            Assert.IsNull(_client.TerraformsContract);
            EditorCoroutineUtility.StartCoroutineOwnerless(LoadingInTheAbi());
        }

        //NOTE: FUNCTIONS PULLED FROM ADDRESS:
        //https://etherscan.io/address/0x4e1f41613c9084fdb9e34e11fae9412427480e56#readContract
        
        /// <summary>
        /// MAX_SUPPLY<br/>
        /// Returns maximum number of potential tokens that can be minted by this contract
        /// NOTE: 11104
        /// </summary>
        [Test]
        public void GetMaxSupplyTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetMaxSupply());
        }
        
        /// <summary>
        /// OWNER_ALLOTMENT<br/>
        /// The number of tokens reserved by Mathcastles Team prior to mint
        /// NOTE: 1200
        /// </summary>
        [Test]
        public void GetOwnerAllotmentTest()
        {
            Debug.Log("<color=red>GetOwnerAllotmentTest functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// PRICE<br/>
        /// The Initial Mint Price .16 ETH or 160000000000000000 Wei
        /// </summary>
        [Test]
        public void GetPriceTest()
        {
            Debug.Log("<color=red>GetPriceTest functionality omitted, was .16 ETH during public mint</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// REVEAL TIMESTAMP<br/>
        /// 1640354289 or Friday, December 24, 2021 1:58:09 PM
        /// </summary>
        [Test]
        public void GetRevealTimestampTest()
        {
            var msg = "<color=red>GetRevealTimestampTest functionality omitted, was ";
            msg += "1640354289 or Friday, December 24, 2021 1:58:09 PM</color>";
            Debug.Log(msg);
            Assert.Pass();
        }
        
        /// <summary>
        /// SUPPLY<br/>
        /// 9904 11104 Total - 1200 Reserve = 9904 public mintable tokens
        /// </summary>
        [Test]
        public void GetSupplyTest()
        {
            Debug.Log("<color=red>GetSupplyTest functonality omitted, always returns 9904</color>");
            Assert.Pass();
        }

        /// <summary>
        /// TOKEN_SCALE<br/>
        /// This constant is the length of a token in 3D space
        /// returns: 211808 int256
        /// </summary>
        [Test]
        public void GetTokenScaleTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenScale());
        }

        /// <summary>
        /// balanceOf<br/>
        /// shows the balance of ETH on contract
        /// </summary>
        [Test]
        public void GetBalanceOfTest()
        {
            Debug.Log("<color=red>GetBalanceOfTest functionality omitted</color>");
            Assert.Pass();
        }

        /// <summary>
        /// dreamers
        /// number of dreamers in contract currently
        /// </summary>
        [Test]
        public void DreamersTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetDreamers());
        }

        /// <summary>
        /// earlyMintActive
        /// was set to true at some point
        /// </summary>
        [Test]
        public void GetEarlyMintActiveTest()
        {
            Debug.Log("<color=red>GetEarlyMintActiveTest functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// getApproved <br/>
        /// deprecated in C#, contract functionality, always returns null address
        /// </summary>
        [Test]
        public void GetApprovedTest()
        {
            Debug.Log($"<color=red>GetApprovedTest functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// isApprovedForAll<br/>
        /// deprecated in C#, contract functionality
        /// </summary>
        [Test]
        public void GetIsApprovedForAllTest()
        {
            Debug.Log($"<color=red>GetApprovedForAllTest: functionality omitted</color>");
            Assert.Pass();
        }
       
        /// <summary>
        /// mintingPaused<br/>
        /// deprecated in C#, contract functionality
        /// </summary>
        [Test]
        public void GetMintingPaused()
        {
            Debug.Log($"<color=red>GetMintingPausedTest: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// name<br/>
        /// returns Terraforms as a string
        /// </summary>
        [Test]
        public void GetNameTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetName());
        }
        
        /// <summary>
        /// owner<br/>
        /// return contract owner address
        /// </summary>
        [Test]
        public void GetOwnerTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetOwner());
        }
        
        /// <summary>
        /// ownerOf<br/>
        /// returns address of owner of a given token
        /// </summary>
        [Test]
        public void GetOwnerOfTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetOwnerOf());
        }
        
        /// <summary>
        /// seed<br/>
        /// fixed return of 10196
        /// </summary>
        [Test]
        public void GetSeedTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetSeed());
        }
        
        /// <summary>
        /// structureData<br/>
        /// returns an array of tuples with containing
        /// <code>
        /// struct StructureLevel
        /// {
        ///   uint256 levelNumber;
        ///   uint256 tokensOnLevel;
        ///   int256 structureSpaceX;
        ///   int256 structureSpaceY;
        ///   int256 structureSpaceZ;
        /// }
        /// </code>
        /// defines the relative position and number of terrain plots on each level
        /// of the "ice cream cone"
        /// </summary>
        [Test]
        public void GetStructureDataTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetStructureData());
        }
        
        /// <summary>
        /// supportsInterface<br/>
        /// inputs interfaceId(bytes4), and returns bool<br/>
        /// deprecated in C#, contract functionality
        /// </summary>
        [Test]
        public void GetSupportsInterfaceTest()
        {
            Debug.Log($"<color=red>GetSupportsInterfaceTest: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// symbol<br/>
        /// return TERRAFORMS as a string;
        /// </summary>
        [Test]
        public void GetSymbolTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetSymbol());
        }
        
        /// <summary>
        /// terraformsAugmentationsAddress<br/>
        /// Address of contract managing augmentations:
        /// 0x2521beb44d433a5b916ad9d5ab51b98378870072<br/>
        /// deprecated in C#, contract functionality
        /// </summary>
        [Test]
        public void GetTerraformsAugmentationAddressTest()
        {
            var str = $"<color=red>GetTerraformsAugmentationAddressTest: functionality omitted\n";
            str += "Address of contract managing augmentations: 0x2521beb44d433a5b916ad9d5ab51b98378870072</color>";
            Debug.Log(str);
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenByIndex
        /// takes index(uint256) and returns uint256 address
        /// </summary>
        [Test]
        public void GetTokenByIndexTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenByIndex());
        }
        
        /// <summary>
        /// tokenCharacters<br/>
        /// gets an array of token characters from the contract
        /// </summary>
        [Test]
        public void GetTokenCharactersTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenCharacters());
        }

        /// <summary>
        /// tokenCounter<br/>
        /// returns total of minted tokens
        /// </summary>
        [Test]
        public void GetTokenCounterTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenCounter());
        }
        
        /// <summary>
        /// tokenHTML<br/>
        /// returns an iframe wrapped token svg
        /// </summary>
        [Test]
        public void GetTokenHTMLTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenHTML());
        }

        /// <summary>
        /// tokenHeightMapIndices<br/>
        /// This is creates an array that references the indices of the TokenSupplementalData
        /// to replace characters (as a starting point, there are multiple behaviors that effect
        /// token cycling as well)
        /// </summary>
        [Test]
        public void GetTokenHeightmapIndicesTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenHeightmapIndices());
        }
        
        /// <summary>
        /// tokenOfOwnerByIndex<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenOfOwnerByIndex()
        {
            Debug.Log($"<color=red>GetTokenOfOwnerByIndex: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenSVG<br/>
        /// takes a tokenId and returns the compiled SVG from the contract<br/>
        /// The returned data contains both the generated font, as well as
        /// the most comprehensive animation data for any given terraform.<br/>
        /// Can be considered the "ground truth" output
        /// </summary>
        [Test]
        public void GetTokenSVGTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenSVG());
        }
        
        /// <summary>
        /// GetTokenSupplementalDataTest<br/>
        /// Takes a tokenId and returns a tuple(struct)
        /// <code>
        /// struct TokenData <br/>
        /// {<br/>
        ///   uint tokenId; <br/>
        ///   uint level;<br/>
        ///   uint xCoordinate;<br/>
        ///   uint yCoordinate;<br/>
        ///   int elevation;<br/>
        ///   int structureSpaceX;<br/>
        ///   int structureSpaceY;<br/>
        ///   int structureSpaceZ;<br/>
        ///   string zoneName;<br/>
        ///   string[10] zoneColors;<br/>
        ///   string[9] characterSet;<br/>
        /// }<br/></code>
        /// </summary>
        [Test]
        public void GetTokenSupplementalDataTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenSupplementalData());
        }
        
        /// <summary>
        /// tokenTerrainValues<br/>
        /// this function returns the height map data in a very raw form
        /// values tend to range from -30000 to 30000 likely do to solidities
        /// lack of floating point values, suggest remapping output values for consistency
        /// </summary>
        [Test]
        public void GetTokenTerrainValuesTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenTerrainValues());
        }
        
        /// <summary>
        /// tokenToAuthorizedDreamer<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenToAuthorizedDreamerTest()
        {
            Debug.Log($"<color=red>GetTokenToAuthorizedDreamerTest: functionality omitted</color>");
            Assert.Pass();
        }

        /// <summary>
        /// tokenToCanvasBlock<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenToCanvasBlockTest()
        {
            Debug.Log($"<color=red>GetTokenToCanvasBlockTest: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenToDreamBlock<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenToDreamBlockTest()
        {
            Debug.Log($"<color=red>GetTokenToDreamBlockTest: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenToPlacement<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenToPlacementTest()
        {
            Debug.Log($"<color=red>GetTokenToPlacement: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenToDreamBlock<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void TokenToStatusTest()
        {
            Debug.Log($"<color=red>GetTokenToStatus: functionality omitted</color>");
            Assert.Pass();
        }
        
        /// <summary>
        /// tokenURI<br/>
        /// returns metadata of all traits and wrapped svg based on tokenId
        /// </summary>
        [Test]
        public void GetTokenURITest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTokenURI());
        }
        
        /// <summary>
        /// tokenUri<br/>
        /// This functionality is omitted
        /// </summary>
        [Test]
        public void GetTokenURIAddressTest()
        {
            Debug.Log($"<color=red>GetTokenURIAddressTest: functionality omitted</color>");
            Assert.Pass();
        }
        
        //TODO: totalSupply
        /// <summary>
        /// totalSupply<br/>
        /// returns the total number of Terraforms that have been minted, exactly the same as tokenCounter
        /// </summary>
        [Test]
        public void GetTotalSupplyTest()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetTotalSupply());
        }
    }
}