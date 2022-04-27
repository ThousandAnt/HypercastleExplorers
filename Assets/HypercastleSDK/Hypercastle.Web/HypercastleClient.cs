//#define WRITE_FILES_FROM_CLIENT
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nethereum.Contracts;
using UnityEngine;
using UnityEngine.Networking;

namespace Hypercastle.Web
{
    [Serializable]
    internal class EtherscanResult
    {
        public int status;
        public string message;
        public string result;
    }
 
    public class HypercastleClient
    {
        const string ProviderUrl = "https://mainnet.infura.io/v3";

        //TODO: Can we avoid using Etherscan? We could just provide the ABI as JSON, unless the version changes?
        const string EtherscanUrl = "https://api.etherscan.io/api";

        static readonly string AbiPath = Path.Combine(Application.streamingAssetsPath, "abi.json");

        internal readonly ConfigurationSecrets Configs;
        readonly Web3 web3;
        readonly object[] tokenIds = new object[1];

        public Contract TerraformsContract => terraformsContract;
        public string Abi => abi;

        string abi;
        Contract terraformsContract;

        public HypercastleClient()
        {
            Configs = ConfigurationSecrets.Load();
            web3 = new Web3($"{ProviderUrl}/{Configs.InfuraProjectId}");
        }

        public IEnumerator<AsyncOperation> LoadContract(string contractAddress)
        {
            if (terraformsContract != null)
            {
                //Debug.LogWarning("You have already loaded the Terraforms contract, LoadContract will not reload the contract.");
                yield break;
            }

            if (File.Exists(AbiPath)) 
            {
                abi = File.ReadAllText(AbiPath);
                terraformsContract = web3.Eth.GetContract(abi, contractAddress);
                yield break;
            }

            var endpoint =
                $"{EtherscanUrl}?module=contract&action=getabi&address={contractAddress}&apikey={Configs.EtherscanToken}";

#if DEBUG
            Debug.Log($"Fetching from endpoint: {endpoint}");
#endif
            using (var webRequest = UnityWebRequest.Get(endpoint))
            {
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        var etherscanResult = JsonUtility.FromJson<EtherscanResult>(webRequest.downloadHandler.text);
                        abi = etherscanResult.result;
                        terraformsContract = web3.Eth.GetContract(abi, contractAddress);
                        break;
                }
            }
        }

        public async Task<MaxSupplyDTO> GetMaxSupply()
        {
            var maxSupplyFunction = TerraformsContract.GetFunction("MAX_SUPPLY");
            var result = await maxSupplyFunction.CallDeserializingToObjectAsync<MaxSupplyDTO>();
            return result;
        }
        
        public async Task<TokenScaleDTO> GetTokenScale()
        {
            var tokenScaleFunction = TerraformsContract.GetFunction("TOKEN_SCALE");
            var result = await tokenScaleFunction.CallDeserializingToObjectAsync<TokenScaleDTO>();
            return result;
        }

        public async Task<DreamersDTO> GetDreamers()
        {
            var tokenScaleFunction = TerraformsContract.GetFunction("TOKEN_SCALE");
            var result = await tokenScaleFunction.CallDeserializingToObjectAsync<DreamersDTO>();
            return result;
        }
        
        public async Task<NameDTO> GetName()
        {
            var nameFunction = TerraformsContract.GetFunction("name");
            var result = await nameFunction.CallDeserializingToObjectAsync<NameDTO>();
            return result;
        }
        
        public async Task<OwnerDTO> GetOwner()
        {
            var ownerFunction = TerraformsContract.GetFunction("owner");
            var result = await ownerFunction.CallDeserializingToObjectAsync<OwnerDTO>();
            return result;
        }
        
        public async Task<OwnerOfDTO> GetOwnerOf(int tokenId)
        {
            tokenIds[0] = tokenId;
            var ownerOfFunction = TerraformsContract.GetFunction("ownerOf");
            var result = await ownerOfFunction.CallDeserializingToObjectAsync<OwnerOfDTO>(tokenIds);
            return result;
        }
        
        public async Task<SeedDTO> GetSeed()
        {
            var seedFunction = TerraformsContract.GetFunction("seed");
            var result = await seedFunction.CallDeserializingToObjectAsync<SeedDTO>();
            return result;
        }
        
        public async Task<StructureDataTupleDTO> GetStructureData(ulong timestamp)
        {
            var structureDataFunction = TerraformsContract.GetFunction("structureData");
            var result = await structureDataFunction.CallDeserializingToObjectAsync<StructureDataTupleDTO>(timestamp);
#if WRITE_FILES_FROM_CLIENT
            await File.WriteAllTextAsync(Path.Combine(Application.streamingAssetsPath, $"structureData.txt"),
                result.ToString());
#endif
            return result;
        }
        
        public async Task<SymbolDTO> GetSymbol()
        {
            var symbolFunction = TerraformsContract.GetFunction("symbol");
            var result = await symbolFunction.CallDeserializingToObjectAsync<SymbolDTO>();
            return result;
        }
        
        public async Task<TokenByIndexDTO> GetTokenByIndex(int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenByIndexFunction = TerraformsContract.GetFunction("tokenByIndex");
            var result = await tokenByIndexFunction.CallDeserializingToObjectAsync<TokenByIndexDTO>(tokenIds);
            return result;
        }

        public async Task<TokenCharactersDTO> GetTokenCharacters(int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenCharacterFunction = TerraformsContract.GetFunction("tokenCharacters");
            var result = await tokenCharacterFunction.CallDeserializingToObjectAsync<TokenCharactersDTO>(tokenIds);
            return result;
        }
        
        public async Task<TokenCounterDTO> GetTokenCounter()
        {
            var tokenCounterFunction = TerraformsContract.GetFunction("tokenCounter");
            var result = await tokenCounterFunction.CallDeserializingToObjectAsync<TokenCounterDTO>();
            return result;
        }

        public async Task<TokenHTMLDTO> GetTokenHTML(int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenHTMLFunction = TerraformsContract.GetFunction("tokenHTML");
            var result = await tokenHTMLFunction.CallDeserializingToObjectAsync<TokenHTMLDTO>(tokenId);
            return result;
        }
        
        public async Task<TokenHeightmapIndicesDTO> GetTokenHeightmapIndices (int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenHTMLFunction = TerraformsContract.GetFunction("tokenHeightmapIndices");
            var result = await tokenHTMLFunction.CallDeserializingToObjectAsync<TokenHeightmapIndicesDTO>(tokenId);
#if WRITE_FILES_FROM_CLIENT           
            await File.WriteAllTextAsync(Path.Combine(Application.streamingAssetsPath, $"{tokenId}_heightmap_indices.txt"), result.ToString());
#endif
            return result;
        }

        public async Task<TokenSVGDTO> GetTokenSVG(int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenSvgFunction = TerraformsContract.GetFunction("tokenSVG");
            var result = await tokenSvgFunction.CallAsync<TokenSVGDTO>(tokenIds);
#if WRITE_FILES_FROM_CLIENT           
            await File.WriteAllTextAsync(Path.Combine(Application.streamingAssetsPath, $"{tokenId}.svg"), result.TokenSVG);
#endif
            return result;
        }

        public async Task<TokenDataTupleDTO> GetTokenSupplementalData(int tokenId)
        {
            tokenIds[0] = tokenId;
            var supplementalDataFunction = TerraformsContract.GetFunction("tokenSupplementalData");
            var result = await supplementalDataFunction.CallDeserializingToObjectAsync<TokenDataTupleDTO>(tokenIds);
            return result;
        }

        public async Task<TokenTerrainValuesDTO> GetTokenTerrainValues(int tokenId)
        {
            tokenIds[0] = tokenId;
            var heightMapFunction = TerraformsContract.GetFunction("tokenTerrainValues");
            var result = await heightMapFunction.CallDeserializingToObjectAsync<TokenTerrainValuesDTO>(tokenIds);
#if WRITE_FILES_FROM_CLIENT
            await File.WriteAllTextAsync(Path.Combine(Application.streamingAssetsPath, $"{tokenId}_heightMap.txt"),
                result.ToString());
#endif
            return result;
        }
        
        public async Task<TokenURIDTO> GetTokenURI(int tokenId)
        {
            tokenIds[0] = tokenId;
            var tokenSvgFunction = TerraformsContract.GetFunction("tokenURI");
            var result = await tokenSvgFunction.CallAsync<TokenURIDTO>(tokenIds);
            return result;
        }
        
        public async Task<TotalSupplyDTO> GetTotalSupply()
        {
            var tokenSvgFunction = TerraformsContract.GetFunction("totalSupply");
            var result = await tokenSvgFunction.CallAsync<TotalSupplyDTO>();
            return result;
        }
    }
}
